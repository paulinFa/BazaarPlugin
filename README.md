# BazaarPlugin ◈ Modern Utility for The Bazaar

A high-end BepInEx plugin designed to enhance your **The Bazaar** experience with real-time data, searchable databases, and community-sourced Meta builds.

> [!IMPORTANT]
> **Mandatory Requirements:**
> 1. **BepInEx 5.x**: Required to load the plugin.
> 2. **Python 3.x**: Required by the installer to generate live Meta data and images.

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
   *   It will show you the exact path (e.g., `C:\Users\...\Programs\Python\Python310\python.exe`).

---

## 🚀 Version 0.2 Release

The latest release includes a **Smart Installer** that fetches live data for you!

### 📥 Download & Install
1. Download the latest **`BazaarPlugin_v0.2_Release.zip`** from the [Releases](https://github.com/paulinFa/BazaarPlugin/releases) page.
2. Extract the zip folder.
3. Run **`BazaarInstaller.exe`**.
4. Select your "The Bazaar" game folder.
5. If Python is not found automatically, the installer will ask you to select your `python.exe`.
6. The installer will automatically install required libraries (`requests`, `Pillow`) and deploy the plugin.

---

## 📸 Presentation & Visuals

> [!TIP]
> *Insert screenshots of the Database (F8), Meta Browser (F7), and In-game Hover (F6) here.*

BazaarPlugin provides all the tools you need directly in-game: real-time monster info, a searchable card database, and the latest community builds.

---

## ✨ Key Features

### 1. 🗃️ The Bazaar Database (F8)
*   **Search Anything**: Instantly find stats for any Item or Skill in the game.
*   **Deep Filters**: Filter by Hero, Official Tags, or even **Hidden Mechanics** (Burn, Crit, Haste, etc.).

### 2. 🏆 Meta Browser (F7)
*   **Jota's builds**: Community-curated builds directly from Jota's Meta spreadsheet.
*   **Board Visuals**: Live-fetched screenshots of endgame boards for every archetype.

### 3. 👾 Monster Insights (F5)
*   **Know your enemy**: Hover over any monster in-game and press F5 to see its portrait.

### 4. 🔍 Smart Hover (F6)
*   **In-Game HUD**: Hover over any card to see a detailed data overlay.

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
*   🖼️ **Visual Database**: High-quality icons for all items in the F8 menu.
*   📜 **Event Details**: Showing detailed results for every event choice.
*   📊 **HUD Overhaul**: A more integrated and customizable in-game HUD for better readability.

---

## 📁 Project Structure (Developers)

*   `BazaarPlugin/`: C# source code.
*   `scripts/`: Python utility scripts for data processing.
*   `SETUP_PLUGIN.bat`: One-click developer setup.
*   `build_and_deploy.ps1`: Automated build and deployment.
