import csv
import json
import requests
import os

# Mapping des GIDs pour chaque héros
HERO_GIDS = {
    "VANESSA": "0",
    "DOOLEY": "1135351900",
    "PYG": "1986857609",
    "MAK": "1857853773",
    "STELLE": "951918977",
    "JULES": "170220885",
    "KARNOK": "430933151"
}

BASE_URL = "https://docs.google.com/spreadsheets/d/1PE3qqsS5qjOzrqzWkCIv6rRRV0NylciecDPlTsC60Qg/export?format=csv&gid="

def clean_items(items_str):
    if not items_str: return []
    # Nettoyage basique (séparateurs virgules, sauts de ligne, etc.)
    items = items_str.replace("\n", ",").split(",")
    return [i.strip() for i in items if i.strip()]

def extract_hero_data(hero_name, gid):
    url = BASE_URL + gid
    print(f"Extraction {hero_name} (GID {gid})...")
    response = requests.get(url)
    if response.status_code != 200:
        print(f"Erreur lors de l'accès à l'onglet {hero_name}")
        return []

    decoded_content = response.content.decode('utf-8')
    cr = csv.reader(decoded_content.splitlines(), delimiter=',')
    rows = list(cr)

    builds = []
    current_category = "GENERAL"

    # On commence après l'entête (souvent ligne 1 ou 2)
    # Dans ce sheet, les colonnes sont : Archetype, Core Items, Notes...
    for row in rows[1:]:
        if not row or not row[0]: continue

        archetype = row[0].strip().strip('"') # Nettoyage des guillemets

        # Détection de catégorie : si la ligne n'a pas de Core Items et n'est pas une entête
        core_items_str = row[1].strip() if len(row) > 1 else ""
        notes = row[2].strip() if len(row) > 2 else ""

        # Si c'est une ligne de titre (Catégorie)
        if archetype and not core_items_str and not notes:
            if archetype.lower() not in ["archetype / variants", "variants"]:
                current_category = archetype.upper()
                print(f"  > Nouvelle catégorie détectée : {current_category}")
            continue

        # Ignorer les entêtes parasites
        if archetype.lower() in ["archetype / variants", "variants"]: continue

        builds.append({
            "Hero": hero_name,
            "Category": current_category,
            "Archetype": archetype,
            "CoreItems": clean_items(core_items_str),
            "Notes": notes
        })

    return builds

all_builds = []
for hero, gid in HERO_GIDS.items():
    all_builds.extend(extract_hero_data(hero, gid))

# Sauvegarde du JSON
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
BASE_DIR = os.path.dirname(SCRIPT_DIR)
output_dir = os.path.join(BASE_DIR, "BazaarDB")
os.makedirs(output_dir, exist_ok=True)
output_path = os.path.join(output_dir, "meta_data.json")
with open(output_path, 'w', encoding='utf-8') as f:
    json.dump(all_builds, f, indent=2, ensure_ascii=False)

print(f"Extraction terminée ! {len(all_builds)} builds sauvegardés dans meta_data.json")
