# BazaarPlugin ◈ Modern Utility for The Bazaar (v0.3)

A high-end BepInEx plugin designed to enhance your **The Bazaar** experience with real-time data, searchable databases, and community-sourced Meta builds.

> [!IMPORTANT]
> **Mandatory Requirements:**
> 1. **BepInEx 5.x**: Required to load the plugin.
> 2. **Python 3.x**: Required by the installer to generate live Meta data and images.

---

## 📸 Presentation & Visuals

### 🗃️ Card Database (F8)
*Search for any item or skill with deep filters.*
![Database Screenshot](docs/images/F8.png)

### 🏆 Meta Browser (F7)
*Browse Jota's latest builds with endgame board images.*
![Meta Browser Screenshot](docs/images/F7.png)

### 🔍 Hover Details (F6)
*Real-time data overlay for any card in-game.*
![Hover Details Screenshot](docs/images/F6.png)

### 👾 Monster Info (F5)
*High-definition portraits and stats for every encounter.*
![Monster Info Screenshot](docs/images/F5.png)

---

## 📦 Prerequisites (Mandatory)

### 1. BepInEx 5.x
To use this plugin, you **must** have **BepInEx 5.x (x64)** installed in your game directory.
1. Download BepInEx 5.4.x from the [official repository](https://github.com/BepInEx/BepInEx/releases).
2. Extract it into your "The Bazaar" folder (where `TheBazaar.exe` is).
3. Run the game once to initialize BepInEx.

### 2. Python 3.x
The installer needs Python to download the latest builds from Jota's spreadsheet and extract board images.
1. **Download**: Install Python from [python.org](https://www.python.org/downloads/) or the Microsoft Store.
2. **How to find your Python path?**
   *   Open a terminal (CMD or PowerShell) and type: `where python`
   *   If it's not found, you can manually select the `python.exe` in the installer.

---

## 🚀 Installation & Deployment

### 📥 Download & Install
1. Download the latest **`BazaarPlugin_Release.zip`** from the [Releases](https://github.com/paulinFa/BazaarPlugin/releases) page.
2. Extract the zip folder.
3. Run **`BazaarInstaller.exe`**.
4. Select your "The Bazaar" game folder (e.g., `E:\Programmes\Steam\steamapps\common\The Bazaar`).
5. If Python is not found automatically, the installer will ask you to select your `python.exe`.
6. The installer will automatically update Meta data and deploy the plugin files.

---

## 🛠️ In-Game Controls

| Key | Action |
| :--- | :--- |
| **F5** | Toggle **Monster Info** (On hover) |
| **F6** | Toggle **Hover Overlay** (On hover) |
| **F7** | Open **Meta Browser** |
| **F8** | Open **Card Database** |
| **F9** | Toggle **Focus Mode** |

---

## 📁 Project Structure (Developers)

The project is divided into several folders to separate source code from data processing tools.

### 📂 `src/` (Source Code)
*   Contains the **C# project (BazaarPlugin.csproj)**.
*   BepInEx plugin logic and Unity UI.

### 📂 `assets/` (Raw Resources)
*   **`mob/`** : High-definition monster portraits (PNG).
*   **`data_source/`** : Local JSON files.
*   **`meta_source/`** : Downloaded Meta Excel file.

### 📂 `scripts/` (Data Processing)
*   **`clean_cards.py`** : Cleans card descriptions from the game files.
*   **`download_meta_excel.py`** : Downloads Jota's latest XLSX.
*   **`extract_meta.py`** : Extracts builds into JSON format.
*   **`extract_xlsx_images.py`** : Extracts endgame board images.

### 📂 `tools/` (Build & Packaging)
*   **`deploy_plugin.py`** : Source code for the installer.
*   **`make_release.py`** : Script to compile and package the release into `dist/`.

### 📂 `dist/` (Output & Distribution)
This folder is generated during the build process and contains the final files:
*   **`BazaarInstaller.exe`**: The standalone installer compiled from `deploy_plugin.py`.
*   **`BazaarPlugin_Release/`**: The **complete portable package**. This is the folder you should zip and share with users. It contains the DLL, the installer, and all required assets and scripts.

---

## 🏗️ Build Cycle (Developers)

To generate a new version of the plugin:
1.  Modify the C# code in `src/`.
2.  Run: `python tools/make_release.py` from the root.
3.  The complete folder will be in `dist/BazaarPlugin_Release`.

---

## 🚧 Roadmap

*   🛡️ Input protection (blocking clicks through UI).
*   🖼️ High-quality images in the F8 database.
*   📜 Deep-dive into event details.
*   📊 HUD overhaul for better integration.
