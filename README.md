# BazaarPlugin ◈ Modern Utility for The Bazaar (v0.3)

A high-end BepInEx plugin designed to enhance your **The Bazaar** experience with real-time data, searchable databases, and community-sourced Meta builds.

> [!IMPORTANT]
> **Mandatory Requirements:**
> 1. **BepInEx 5.x**: Required to load the plugin.
> 2. **Python 3.x**: Required by the installer to generate live Meta data and images.
> 
> **Note:** You must know the installation folder of your game and the location of your `python.exe` to complete the setup successfully.

---

## 📸 Presentation & Visuals

### 🗃️ The Database (F8)
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
2. **Setup**: During installation, check the box **"Add Python to PATH"**.
3. **How to find your Python path?**
   *   Open a terminal (CMD or PowerShell) and type: `where python`

---

## 🚀 Version 0.3 Release

The latest release includes an improved **Smart Installer** with dynamic path detection and live data fetching!

### 📥 Download & Install
1. Download the latest **`BazaarPlugin_v0.3_Release.zip`** from the [Releases](https://github.com/paulinFa/BazaarPlugin/releases) page.
2. Extract the zip folder.
3. Run **`BazaarInstaller.exe`**.
4. Select your "The Bazaar" game folder.
5. If Python is not found automatically, the installer will ask you to select your `python.exe`.
6. The installer will automatically install required libraries and deploy the plugin.

---

## 🛠️ Controls

| Key | Action |
| :--- | :--- |
| **F5** | Toggle **Monster Info** (Hover over a mob) |
| **F6** | Toggle **Hover Overlay** (Hover over a card) |
| **F7** | Open **Meta Browser** |
| **F8** | Open **Card Database** |
| **F9** | Toggle **Focus Mode** (Syncs hover with selection) |

---

## 🚧 Future Features (Roadmap)

*   🛡️ **Input Protection**: Blocking mouse interactions for the background game.
*   🖼️ **Visual Database**: Displaying high-quality images for all items and skills in the F8 menu.
*   📜 **Event Details**: Deep-dive into event choices, showing exactly what each option gives or takes.
*   📊 **HUD Overhaul**: A more integrated and customizable in-game HUD for better readability.
*   👾 **Monster Rework**: Advanced monster info section showing every card in the mob's deck, updated via a dedicated sync script.

---

## 📁 Project Structure (Developers)

*   `BazaarPlugin/`: C# source code.
*   `scripts/`: Python utility scripts for data processing.
*   `SETUP_PLUGIN.bat`: One-click developer setup.
*   `build_and_deploy.ps1`: Automated build and deployment.
