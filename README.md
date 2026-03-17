# BazaarPlugin ◈ Modern Utility for The Bazaar

A high-end BepInEx plugin designed to enhance your **The Bazaar** experience with real-time data, searchable databases, and community-sourced Meta builds.

---

## ✨ Key Features

### 1. 🗃️ The Bazaar Database (F8)
*   **Search Anything**: Instantly find stats for any Item or Skill in the game.
*   **Deep Filters**: Filter by Hero, Official Tags, or even **Hidden Mechanics** (Burn, Crit, Haste, etc.).
*   **Detailed View**: Click a card to see its full description, cooldown progression across tiers, and all available enchantments.

### 2. 🏆 Meta Browser (F7)
*   **Jota's builds**: Community-curated builds directly from Jota's Meta spreadsheet.
*   **Board Visuals**: See high-quality screenshots of endgame boards for every archetype.
*   **Strategy**: Get tips on core items and how to pilot each build.

### 3. 👾 Monster Insights (F5)
*   **Know your enemy**: Hover over any monster in-game and press F5 to see its detailed visual info.
*   **Full Resolution**: High-definition monster portraits used for strategic planning.

### 4. 🔍 Smart Hover (F6)
*   **In-Game HUD**: Hover over any card in your shop, hand, or inventory to see a detailed overlay.
*   **Always Updated**: Fetches data directly from your local database for 100% accuracy.

### 5. 🛡️ User Experience
*   **Input Protection**: Mouse scrolling and clicks are blocked for the background game when interacting with the plugin UI. No more accidental camera zooming!
*   **Modern UI**: Sleek, dark-themed interface with gold and cyan accents.
*   **Focus Mode (F9)**: Highlights the hovered card in the database automatically.

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

## 🚀 Installation & Setup

1. **Clone the Repository**: Download the project to your local machine.
2. **Initial Setup**:
   *   Double-click **`SETUP_PLUGIN.bat`**.
   *   A window will appear: **Select your "The Bazaar" game folder**.
   *   The script will automatically:
       *   Fetch `cards.json` from the game files.
       *   Download the latest Meta Excel from Google Sheets.
       *   Extract board images and build the database.
       *   Configure the deployment paths.
3. **Build & Update**:
   *   Run **`build_and_deploy.ps1`** (via PowerShell) to compile the C# code and copy all assets to your game folder.

---

## 📁 Project Structure

*   `BazaarPlugin/`: C# source code for the plugin.
*   `data_source/`: Raw `cards.json` fetched from the game.
*   `meta_source/`: Source Excel file for Meta builds.
*   `mobs/`: Source images for monster portraits.
*   `scripts/`: Python utility scripts for data processing.
*   `SETUP_PLUGIN.bat`: One-click setup and configuration.
*   `build_and_deploy.ps1`: Automated build and deployment script.

---

## 🧪 Technical Stack

*   **Plugin**: C# (.NET Framework 4.8) + BepInEx 5 + Harmony.
*   **Data**: Python 3 + `requests` + `Pillow` for image extraction and JSON parsing.
*   **UI**: Unity IMGUI with custom high-definition textures.

---
*Created with ❤️ for The Bazaar community.*
