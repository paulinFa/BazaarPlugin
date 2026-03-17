import os
import shutil
import subprocess

# Paths
PROJECT_ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DIST_DIR = os.path.join(PROJECT_ROOT, "dist")
RELEASE_DIR = os.path.join(DIST_DIR, "BazaarPlugin_Release")
PYTHON_EXE = r"C:\Users\pauli\AppData\Local\Python\pythoncore-3.14-64\python.exe"
ZIP_NAME = "BazaarPlugin_v0.3_Release" # You can update the version here

def run_command(command, cwd=None):
    print(f"Executing: {' '.join(command) if isinstance(command, list) else command}")
    subprocess.run(command, check=True, cwd=cwd, shell=True)

def build():
    # 1. Cleanup old builds and archives
    print("--- Cleaning up old builds ---")
    if os.path.exists(RELEASE_DIR):
        shutil.rmtree(RELEASE_DIR)
    
    zip_path = os.path.join(DIST_DIR, f"{ZIP_NAME}.zip")
    if os.path.exists(zip_path):
        os.remove(zip_path)
        
    os.makedirs(RELEASE_DIR)

    # 2. Build C# Project
    print("\n--- Building C# Plugin ---")
    csproj_path = os.path.join(PROJECT_ROOT, "src", "BazaarPlugin", "BazaarPlugin.csproj")
    run_command(["dotnet", "build", csproj_path, "-c", "Release"])

    # 3. Build Installer EXE
    print("\n--- Building Installer EXE ---")
    installer_script = os.path.join(PROJECT_ROOT, "tools", "deploy_plugin.py")
    run_command([PYTHON_EXE, "-m", "PyInstaller", "--onefile", "--noconsole", "--name", "BazaarInstaller", installer_script], cwd=PROJECT_ROOT)

    # 4. Package Release Folder
    print("\n--- Packaging Release ---")
    
    # DLL
    dll_src = os.path.join(PROJECT_ROOT, "src", "BazaarPlugin", "bin", "Release", "BazaarPlugin.dll")
    shutil.copy(dll_src, os.path.join(RELEASE_DIR, "BazaarPlugin.dll"))

    # Installer EXE
    shutil.copy(os.path.join(DIST_DIR, "BazaarInstaller.exe"), os.path.join(RELEASE_DIR, "BazaarInstaller.exe"))

    # Mob Folder
    shutil.copytree(os.path.join(PROJECT_ROOT, "assets", "mob"), os.path.join(RELEASE_DIR, "mob"))

    # Scripts Folder
    shutil.copytree(os.path.join(PROJECT_ROOT, "scripts"), os.path.join(RELEASE_DIR, "scripts"))

    # Assets Folder (Data sources for scripts)
    assets_dst = os.path.join(RELEASE_DIR, "assets")
    os.makedirs(assets_dst)
    shutil.copytree(os.path.join(PROJECT_ROOT, "assets", "data_source"), os.path.join(assets_dst, "data_source"))
    shutil.copytree(os.path.join(PROJECT_ROOT, "assets", "meta_source"), os.path.join(assets_dst, "meta_source"))

    # 5. Create ZIP Archive
    print(f"\n--- Creating ZIP Archive: {ZIP_NAME}.zip ---")
    shutil.make_archive(os.path.join(DIST_DIR, ZIP_NAME), 'zip', RELEASE_DIR)

    print(f"\nSUCCESS! Release created in: {RELEASE_DIR}")
    print(f"ZIP Archive ready for upload: {zip_path}")

if __name__ == "__main__":
    build()
