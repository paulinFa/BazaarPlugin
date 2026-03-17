import os
import shutil
import subprocess
import tkinter as tk
from tkinter import filedialog, messagebox
import sys

def deploy():
    # 0. Determine executable location
    if getattr(sys, 'frozen', False):
        current_dir = os.path.dirname(sys.executable)
    else:
        current_dir = os.path.dirname(os.path.abspath(__file__))

    # 1. Select game folder
    default_game_path = r"E:\Programmes\Steam\steamapps\common\The Bazaar"
    root = tk.Tk()
    root.withdraw()

    game_dir = filedialog.askdirectory(
        title="Select 'The Bazaar' Game Folder",
        initialdir=default_game_path if os.path.exists(default_game_path) else None
    )

    if not game_dir:
        return

    # 2. Select python.exe
    python_exe = filedialog.askopenfilename(
        title="Select your python.exe",
        filetypes=[("Executable", "*.exe")],
        initialfile="python.exe"
    )

    if not python_exe:
        return

    # 3. Check for BepInEx
    bepinex_dir = os.path.join(game_dir, "BepInEx")
    if not os.path.exists(bepinex_dir):
        messagebox.showerror("Error", "BepInEx is not installed in the game folder. Please install it first.")
        return

    plugins_dir = os.path.join(bepinex_dir, "plugins")
    if not os.path.exists(plugins_dir):
        os.makedirs(plugins_dir)

    # 4. Install requirements
    print("Installing requirements (requests)...")
    try:
        subprocess.run([python_exe, "-m", "pip", "install", "requests"], check=True, capture_output=True)
    except Exception as e:
        print(f"Pip install warning: {e}")

    # 5. Move DLL
    possible_dll_paths = [
        os.path.join(current_dir, "BazaarPlugin.dll"),
        os.path.join(os.path.dirname(current_dir), "src", "BazaarPlugin", "bin", "Release", "BazaarPlugin.dll"),
        os.path.join(current_dir, "src", "BazaarPlugin", "bin", "Release", "BazaarPlugin.dll"),
    ]
    
    dll_found_path = None
    for path in possible_dll_paths:
        if os.path.exists(path):
            dll_found_path = path
            break
    
    if dll_found_path:
        shutil.copy(dll_found_path, os.path.join(plugins_dir, "BazaarPlugin.dll"))
    else:
        messagebox.showwarning("Warning", "BazaarPlugin.dll not found.")

    # 6. Move mob folder
    mob_src = os.path.join(current_dir, "mob")
    mob_dst = os.path.join(plugins_dir, "mob")
    if os.path.exists(mob_src):
        if os.path.exists(mob_dst): shutil.rmtree(mob_dst)
        shutil.copytree(mob_src, mob_dst)
    
    # 7. Run Python scripts in SPECIFIC ORDER
    scripts_dir = os.path.join(current_dir, "scripts")
    if os.path.exists(scripts_dir):
        ordered_scripts = [
            "download_meta_excel.py",
            "extract_meta.py",
            "extract_xlsx_images.py",
            "clean_cards.py"
        ]
        
        for script_name in ordered_scripts:
            script_path = os.path.join(scripts_dir, script_name)
            if os.path.exists(script_path):
                print(f"Running {script_name}...")
                try:
                    # Run and capture output to show error if failure
                    result = subprocess.run([python_exe, script_path, game_dir], check=True, cwd=current_dir, capture_output=True, text=True)
                    print(result.stdout)
                except subprocess.CalledProcessError as e:
                    error_msg = f"Error in {script_name}:\n\nSTDOUT: {e.stdout}\n\nSTDERR: {e.stderr}"
                    print(error_msg)
                    messagebox.showerror("Script Error", error_msg)
                    return # Stop deployment if a script fails
    
    # 8. Move generated BazaarDB folder
    bazaardb_src = os.path.join(current_dir, "BazaarDB")
    bazaardb_dst = os.path.join(plugins_dir, "BazaarDB")

    if os.path.exists(bazaardb_src):
        if os.path.exists(bazaardb_dst): shutil.rmtree(bazaardb_dst)
        shutil.copytree(bazaardb_src, bazaardb_dst)
        shutil.rmtree(bazaardb_src)
    else:
        if not os.path.exists(bazaardb_dst): os.makedirs(bazaardb_dst)

    messagebox.showinfo("Success", "Installation complete! Live data and images have been deployed.")

if __name__ == "__main__":
    deploy()
