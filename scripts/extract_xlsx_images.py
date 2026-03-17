import os
import shutil
import xml.etree.ElementTree as ET
import json

# Chemins relatifs au projet
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
BASE_DIR = os.path.dirname(SCRIPT_DIR)
UNZIPPED_PATH = os.path.join(os.environ.get('USERPROFILE'), ".gemini", "tmp", "pauli", "bazaar_xlsx_unzipped")
OUTPUT_BASE = os.path.join(BASE_DIR, "BazaarPlugin", "BazaarDB", "meta")
XLSX_SOURCE = os.path.join(BASE_DIR, "meta_source", "Bazaar Meta Jota.xlsx")

# Namespaces XML
NS = {
    'ss': 'http://schemas.openxmlformats.org/spreadsheetml/2006/main',
    'xdr': 'http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing',
    'a': 'http://schemas.openxmlformats.org/drawingml/2006/main',
    'r': 'http://schemas.openxmlformats.org/officeDocument/2006/relationships'
}

def get_shared_strings():
    ss_path = os.path.join(UNZIPPED_PATH, 'xl', 'sharedStrings.xml')
    tree = ET.parse(ss_path)
    root = tree.getroot()
    strings = []
    for si in root.findall('ss:si', NS):
        # Handle simple text or rich text
        t = si.find('ss:t', NS)
        if t is not None:
            strings.append(t.text if t.text else "")
        else:
            # Join all text parts in rich text
            text_parts = []
            for r in si.findall('ss:r', NS):
                t_part = r.find('ss:t', NS)
                if t_part is not None:
                    text_parts.append(t_part.text if t_part.text else "")
            strings.append("".join(text_parts))
    return strings

def get_sheet_rels(sheet_id):
    rels_path = os.path.join(UNZIPPED_PATH, 'xl', 'worksheets', '_rels', f'sheet{sheet_id}.xml.rels')
    if not os.path.exists(rels_path): return {}
    tree = ET.parse(rels_path)
    root = tree.getroot()
    rels = {}
    for rel in root.findall('{http://schemas.openxmlformats.org/package/2006/relationships}Relationship'):
        rels[rel.get('Id')] = rel.get('Target')
    return rels

def get_drawing_rels(drawing_name):
    rels_path = os.path.join(UNZIPPED_PATH, 'xl', 'drawings', '_rels', f'{drawing_name}.rels')
    if not os.path.exists(rels_path): return {}
    tree = ET.parse(rels_path)
    root = tree.getroot()
    rels = {}
    for rel in root.findall('{http://schemas.openxmlformats.org/package/2006/relationships}Relationship'):
        rels[rel.get('Id')] = rel.get('Target')
    return rels

def process_hero(hero_name, sheet_id, shared_strings):
    print(f"Traitement de {hero_name}...")
    hero_output = os.path.join(OUTPUT_BASE, hero_name.upper())
    if not os.path.exists(hero_output): os.makedirs(hero_output)

    # 1. Lire la feuille pour mapper row -> archetype
    sheet_path = os.path.join(UNZIPPED_PATH, 'xl', 'worksheets', f'sheet{sheet_id}.xml')
    tree = ET.parse(sheet_path)
    root = tree.getroot()
    
    row_to_archetype = {}
    for row in root.findall('.//ss:row', NS):
        row_idx = int(row.get('r')) - 1 # 0-indexed
        # Archetype est dans la colonne A (Col 0)
        c = row.find('ss:c[@r]', NS) # Find first cell, hope it's A
        if c is not None and c.get('r').startswith('A'):
            t = c.get('t')
            v = c.find('ss:v', NS)
            if v is not None:
                if t == 's':
                    name = shared_strings[int(v.text)]
                else:
                    name = v.text
                if name and name.strip() and name.strip().lower() not in ["archetype / variants", "variants", "vanessa builds", "dooley builds", "pyg builds", "mak builds", "stelle builds", "jules builds", "karnok builds"]:
                    row_to_archetype[row_idx] = name.strip().replace("/", "_").replace(":", "").replace('"', "")

    # 2. Lire les relations pour trouver le drawing
    sheet_rels = get_sheet_rels(sheet_id)
    drawing_rel_target = None
    for target in sheet_rels.values():
        if 'drawings/drawing' in target:
            drawing_rel_target = os.path.basename(target)
            break
    
    if not drawing_rel_target:
        print(f"Pas de dessins pour {hero_name}")
        return

    # 3. Lire le drawing pour mapper row -> imageId
    drawing_path = os.path.join(UNZIPPED_PATH, 'xl', 'drawings', drawing_rel_target)
    drawing_rels = get_drawing_rels(drawing_rel_target)
    
    tree = ET.parse(drawing_path)
    root = tree.getroot()
    
    # On cherche les oneCellAnchor ou twoCellAnchor
    # Dans Google Sheets export, c'est souvent oneCellAnchor
    count = 0
    for anchor in root:
        from_tag = anchor.find('xdr:from', NS)
        if from_tag is not None:
            row_idx = int(from_tag.find('xdr:row', NS).text)
            pic = anchor.find('.//xdr:pic', NS)
            if pic is not None:
                blip = pic.find('.//a:blip', NS)
                if blip is not None:
                    embed_id = blip.get('{http://schemas.openxmlformats.org/officeDocument/2006/relationships}embed')
                    image_rel_path = drawing_rels.get(embed_id)
                    if image_rel_path:
                        # image_rel_path is like ../media/image1.png
                        image_file = os.path.basename(image_rel_path)
                        src_path = os.path.join(UNZIPPED_PATH, 'xl', 'media', image_file)
                        
                        # Trouver l'archetype correspondant à cette ligne ou les lignes précédentes (car l'image peut être décalée)
                        archetype = None
                        # On cherche l'archetype le plus proche au-dessus ou sur la même ligne
                        for r in range(row_idx, -1, -1):
                            if r in row_to_archetype:
                                archetype = row_to_archetype[r]
                                break
                        
                        if archetype:
                            # Gérer les multiples images par archetype (image1, image2, etc)
                            # On va juste compter combien on en a déjà pour cet archetype
                            existing = [f for f in os.listdir(hero_output) if f.startswith(archetype)]
                            suffix = f"_{len(existing) + 1}" if len(existing) > 0 else ""
                            dest_name = f"{archetype}{suffix}.png"
                            shutil.copy(src_path, os.path.join(hero_output, dest_name))
                            count += 1
    
    print(f"  > {count} images extraites pour {hero_name}")

def main():
    shared_strings = get_shared_strings()
    
    heroes = [
        ("Vanessa", 1),
        ("Dooley", 2),
        ("Pyg", 3),
        ("Mak", 4),
        ("Stelle", 5),
        ("Jules", 6),
        ("Karnok", 7)
    ]
    
    for name, s_id in heroes:
        process_hero(name, s_id, shared_strings)

if __name__ == "__main__":
    main()
