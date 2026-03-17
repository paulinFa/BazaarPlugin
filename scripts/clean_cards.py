import json
import os
import re
import sys

# --- GESTION DES CHEMINS DYNAMIQUES ---
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
BASE_DIR = os.path.dirname(SCRIPT_DIR)

# Par défaut, on cherche dans data_source, mais on peut passer le dossier du jeu en argument
if len(sys.argv) > 1:
    game_path = sys.argv[1]
    INPUT_FILE = os.path.join(game_path, "TheBazaar_Data", "StreamingAssets", "cards.json")
else:
    INPUT_FILE = os.path.join(BASE_DIR, 'assets', 'data_source', 'cards.json')

OUTPUT_DIR = os.path.join(BASE_DIR, 'BazaarDB')
os.makedirs(OUTPUT_DIR, exist_ok=True)
OUTPUT_FILE = os.path.join(OUTPUT_DIR, 'clean_cards.json')

TIME_ATTRIBUTES = ["SlowAmount", "FreezeAmount", "HasteAmount", "ChargeAmount", "CooldownMax", "ReductionAmount"]

TARGET_HIDDEN_TAGS = [
    "Ammo", "Burn", "Charge", "Cooldown", "Crit", "Damage", "XP", 
    "Flying", "Freeze", "Gold", "Haste", "Heal", "Health", "Income", 
    "Level", "Poison", "Regen", "Shield", "Slow", "Value"
]

def safe_get(data, *keys):
    for k in keys:
        if isinstance(data, dict) and k in data:
            data = data[k]
        else:
            return None
    return data

def get_numeric_value(obj):
    if isinstance(obj, (int, float)): return obj
    if not isinstance(obj, dict): return None
    
    if obj.get("$type") == "TFixedValue": return obj.get("Value")
    if "Amount" in obj and isinstance(obj["Amount"], (int, float)): return obj["Amount"]
    if "Value" in obj: return get_numeric_value(obj["Value"])
    return None

def find_card_attribute_reference(obj):
    if isinstance(obj, dict):
        if obj.get("$type") == "TReferenceValueCardAttribute": return obj.get("AttributeType")
        for k, v in obj.items():
            if k in ["Target", "Conditions", "Modifier"]: continue
            res = find_card_attribute_reference(v)
            if res: return res
    elif isinstance(obj, list):
        for item in obj:
            res = find_card_attribute_reference(item)
            if res: return res
    return None

def get_attribute_for_action(action):
    a_type = action.get("$type", "")
    
    if "Slow" in a_type: return "SlowAmount", "SlowTargets"
    if "Freeze" in a_type: return "FreezeAmount", "FreezeTargets"
    if "Haste" in a_type: return "HasteAmount", "HasteTargets"
    if "Charge" in a_type: return "ChargeAmount", "ChargeTargets"
    if "Damage" in a_type: return "DamageAmount", "TargetCount"
    if "Heal" in a_type: return "HealAmount", "TargetCount"
    if "Shield" in a_type: return "ShieldApplyAmount", "TargetCount"
    if "Poison" in a_type: return "PoisonApplyAmount", "TargetCount"
    if "Burn" in a_type: return "BurnApplyAmount", "TargetCount"
    if "Rage" in a_type: return "RageApplyAmount", "TargetCount"
    if "Regen" in a_type: return "RegenApplyAmount", "TargetCount"
    if "Reload" in a_type: return "ReloadAmount", "TargetCount"
    if "FlyingStop" in a_type or "FlyingStart" in a_type: return "FlyingTargets", "TargetCount"
    if "Upgrade" in a_type: return "TargetCount", "TargetCount"
    if "CardGenerate" in a_type: return "TargetCount", "TargetCount"
    
    if "ModifyAttribute" in a_type:
        attr_mod = action.get("AttributeType", "")
        if attr_mod in ["CooldownMax", "PercentCooldownMaxReduction", "Multicast", "SellPrice", "CritChance"]: 
            return "Custom_0", "TargetCount"
        return attr_mod, "TargetCount"

    return None, None

def format_val(val, is_time=False):
    if val is None: return "0"
    try:
        f_val = float(val)
        if is_time and abs(f_val) >= 20: f_val /= 1000.0
        return f"{int(f_val)}" if f_val.is_integer() else f"{f_val}"
    except: return str(val)

def resolve_placeholder(match, context_obj, attrs_list):
    path = match.group(1).split('.')
    if len(path) < 2: return match.group(0)

    prefix, key = path[0], path[1]
    sub_path = path[2] if len(path) > 2 else None

    source = {}
    if prefix == "ability": source = context_obj.get("Abilities", {}).get(key, {})
    elif prefix == "aura": source = context_obj.get("Auras", {}).get(key, {})
    if not source: return "?"

    action = source.get("Action", {})

    if sub_path == "mod":
        val = get_numeric_value(action.get("Value"))
        if val is None: val = get_numeric_value(action.get("Modifier"))
        if val is not None: return format_val(val)
        return "1"

    attr_ref = find_card_attribute_reference(source)
    attr_main, attr_targets = get_attribute_for_action(action)
    target_attr = attr_targets if sub_path == "targets" else (attr_ref or attr_main)
    
    is_time = False
    if target_attr and any(x in target_attr for x in TIME_ATTRIBUTES): is_time = True
    if "Cooldown" in action.get("AttributeType", ""): is_time = True

    if sub_path == "targets" or target_attr == "TargetCount":
        val = get_numeric_value(action.get("TargetCount"))
        if val is not None: return format_val(val)

    if target_attr:
        vals = []
        for d in attrs_list:
            if target_attr in d: vals.append(d[target_attr])
            elif "Custom_0" in d and not attr_ref: vals.append(d["Custom_0"])
            elif "Custom_1" in d and not attr_ref: vals.append(d["Custom_1"])
            
        unique_vals = []
        for v in vals:
            if v not in unique_vals: unique_vals.append(v)
            
        if unique_vals: return " » ".join([format_val(v, is_time) for v in unique_vals])

    val = get_numeric_value(action.get("Value"))
    if val is not None: return format_val(val, is_time)

    for k, v in action.items():
        if isinstance(v, (int, float)) and k not in ["Id", "Duration"]:
            return format_val(v, is_time)

    return "?"

def clean_text(txt):
    txt = txt.replace("[?]", "").replace("[", "").replace("]", "")
    txt = txt.replace("+?", "+").replace("-?", "-")
    txt = re.sub(r'\s*\?\s*', ' ', txt)
    txt = " ".join(txt.split())
    
    def fix_ms_to_s(match):
        nums_str = match.group(1)
        suffix = match.group(2)
        parts = nums_str.split('»')
        new_parts = []
        for p in parts:
            p = p.strip()
            try:
                val = float(p)
                if val >= 200: val = val / 1000.0
                new_parts.append(f"{int(val)}" if val.is_integer() else f"{val}")
            except:
                new_parts.append(p)
        return " » ".join(new_parts) + suffix

    time_pattern = r'(\d+(?:\.\d+)?(?:\s*»\s*\d+(?:\.\d+)?)*)(\s*second\(s\)|\s*seconds?|\s*Slow duration|\s*Freeze duration|\s*Haste duration|\s*Charge duration)'
    txt = re.sub(time_pattern, fix_ms_to_s, txt, flags=re.IGNORECASE)
    return txt.strip()

def parse_bazaar():
    if not os.path.exists(INPUT_FILE):
        print(f"ERREUR : Impossible de trouver {INPUT_FILE}")
        return

    print(f"Lecture des données depuis : {INPUT_FILE}")
    with open(INPUT_FILE, 'r', encoding='utf-8') as f:
        data = json.load(f)

    cards = data if isinstance(data, list) else []
    if isinstance(data, dict):
        for v in data.values():
            if isinstance(v, list): cards.extend(v)

    final_db = {}
    for card in cards:
        try:
            c_id = card.get("Id")
            if not c_id: continue

            # --- GESTION SPÉCIFIQUE DES COMBAT ENCOUNTERS ---
            if card.get("Type") == "CombatEncounter" or card.get("$type") == "TCardEncounterCombat":
                name = safe_get(card, "Localization", "Title", "Text") or "Inconnu"
                lvl = safe_get(card, "CombatantType", "Level")
                gold = card.get("RewardCombatGold", 0)
                xp = card.get("RewardCombatXp", 0)
                
                desc_parts = []
                if lvl is not None: desc_parts.append(f"Level: {lvl}")
                if gold > 0: desc_parts.append(f"Reward: {gold} Gold")
                if xp > 0: desc_parts.append(f"Reward: {xp} XP")
                
                h_tags = []
                if gold > 0: h_tags.append("Gold")
                if xp > 0: h_tags.append("XP")
                
                final_db[c_id] = {
                    "Id": c_id,
                    "Name": name,
                    "Category": "CombatEncounter",
                    "Size": card.get("Size", ""),
                    "Heroes": card.get("Heroes", []),
                    "Tier": card.get("StartingTier", "Bronze"),
                    "Tags": card.get("Tags", []),
                    "HiddenTags": h_tags,
                    "Cooldown": "",
                    "Description": " \n ".join(desc_parts),
                    "Enchantments": {}
                }
                continue

            # --- TRAITEMENT CLASSIQUE ---
            tiers = card.get("Tiers", {})
            attrs_progression = []
            for t_name in ["Bronze", "Silver", "Gold", "Diamond"]:
                if t_name in tiers:
                    last_attr = attrs_progression[-1].copy() if attrs_progression else {}
                    last_attr.update(tiers[t_name].get("Attributes", {}))
                    attrs_progression.append(last_attr)
            if not attrs_progression: attrs_progression = [card.get("Attributes", {})]

            cd_vals = [a.get("CooldownMax", 0) for a in attrs_progression]
            unique_cds = []
            for cd in cd_vals:
                if cd and float(cd) > 0:
                    formatted_cd = format_val(cd, True)
                    if formatted_cd not in unique_cds:
                        unique_cds.append(formatted_cd)
            
            cooldown_str = " » ".join([f"{cd}s" for cd in unique_cds]) if unique_cds else ""

            tooltips = card.get("Localization", {}).get("Tooltips", [])
            desc_resolved = []
            for t in tooltips:
                txt = safe_get(t, "Content", "Text")
                if txt:
                    resolved = re.sub(r'\{([a-zA-Z0-9_.]+)\}', lambda m: resolve_placeholder(m, card, attrs_progression), txt)
                    desc_resolved.append(clean_text(resolved))
            
            full_description = "\n".join(desc_resolved)

            hidden_tags = []
            desc_lower = full_description.lower()
            for tag in TARGET_HIDDEN_TAGS:
                if re.search(r'\b' + re.escape(tag.lower()) + r'\b', desc_lower):
                    hidden_tags.append(tag)

            enchants_resolved = {}
            raw_enchants = card.get("Enchantments", {})
            for e_name, e_data in raw_enchants.items():
                e_tooltips = safe_get(e_data, "Localization", "Tooltips")
                if not e_tooltips: continue
                e_text = safe_get(e_tooltips[0], "Content", "Text")
                if e_text:
                    e_attrs = [e_data.get("Attributes", {})]
                    resolved_e = re.sub(r'\{([a-zA-Z0-9_.]+)\}', lambda m: resolve_placeholder(m, e_data, e_attrs), e_text)
                    enchants_resolved[e_name] = clean_text(resolved_e)

            final_db[c_id] = {
                "Id": c_id,
                "Name": safe_get(card, "Localization", "Title", "Text") or "Inconnu",
                "Category": card.get("Type", "").replace("TCard", ""), 
                "Size": card.get("Size", ""), 
                "Heroes": card.get("Heroes", []),
                "Tier": card.get("StartingTier", "Bronze"),
                "Tags": card.get("Tags", []),
                "HiddenTags": hidden_tags, 
                "Cooldown": cooldown_str,
                "Description": full_description,
                "Enchantments": enchants_resolved
            }
        except Exception: pass

    with open(OUTPUT_FILE, 'w', encoding='utf-8') as f:
        json.dump(final_db, f, indent=4, ensure_ascii=False)
    
    print(f"SUCCÈS ! {len(final_db)} cartes générées dans {OUTPUT_FILE}")

if __name__ == "__main__":
    parse_bazaar()
