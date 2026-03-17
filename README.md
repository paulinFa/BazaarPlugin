# BazaarPlugin ◈ Modern Utility for The Bazaar

A high-end BepInEx plugin designed to enhance your **The Bazaar** experience with real-time data, searchable databases, and community-sourced Meta builds.

> [!IMPORTANT]
> **This plugin requires BepInEx to function.** You must install BepInEx before adding this plugin to your game.

---

## 📸 Presentation & Visuals

> [!TIP]
> *Insert screenshots of the Database (F8), Meta Browser (F7), and In-game Hover (F6) here.*

BazaarPlugin is the ultimate companion for players who want to dive deep into the game's mechanics. Whether you're a newcomer looking for the best builds or a veteran searching for specific item interactions, this plugin provides all the tools you need directly in-game.

---

## 📦 Prerequisites (Mandatory)

To use this plugin, you **must** have **BepInEx 5.x (x64)** installed in your game directory.
1. Download BepInEx 5.4.x from the [official repository](https://github.com/BepInEx/BepInEx/releases).
2. Extract it into your "The Bazaar" folder (where `TheBazaar.exe` is).
3. Run the game once to initialize BepInEx.

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

1. **Clone the Repository** (or download the Release zip).
2. **Initial Setup**:
   *   Double-click **`SETUP_PLUGIN.bat`**.
   *   A window will appear: **Select your "The Bazaar" game folder**.
   *   The script will automatically fetch game data, download Meta builds, and configure paths.
3. **Build & Update**:
   *   Run **`build_and_deploy.ps1`** to compile and copy all assets to your game folder.

---

## 🚧 Future Features (Roadmap)

We are constantly improving the plugin. Here is what's coming next:
*   🛡️ **Input Protection**: Blocking mouse scrolling and clicks for the background game when interacting with the plugin UI.
*   🖼️ **Visual Database**: Displaying high-quality images for all items and skills in the F8 database.
*   📜 **Event Details**: Deep-dive into event choices, showing exactly what each option gives or takes.
*   📊 **HUD Overhaul**: A more integrated and customizable in-game HUD for better readability.
*   🔄 **Live Sync**: Automatic detection of game updates to refresh the `cards.json` parser.

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
