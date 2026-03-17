using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BazaarPlugin
{
    public static class DatabaseUI
    {
        public static Rect WinRect = new Rect(50, 50, 480, 750);
        private static string _query = "";
        private static Vector2 _scroll;
        private static List<CardData> _results = new List<CardData>();
        public static CardData Selected;

        private static HashSet<string> _activeCats = new HashSet<string>();
        private static HashSet<string> _activeHeroes = new HashSet<string>();
        private static HashSet<string> _activeTags = new HashSet<string>();
        private static HashSet<string> _activeSizes = new HashSet<string>();
        private static HashSet<string> _activeHiddenTags = new HashSet<string>();

        private static bool _showCat = false, _showHero = false, _showTag = false, _showSize = false, _showHiddenTag = false;

        private static int _resizingWindowID = -1;
        private static bool _firstOpen = true;

        public static GUIStyle WinStyle, SearchStyle, ItemStyle, BtnStyle, BtnActiveStyle, LabelStyle, CloseBtnStyle, AccordionStyle;
        private static bool _init = false;

        private static void Init()
        {
            if (_init && WinStyle != null) return;

            LabelStyle = new GUIStyle(GUI.skin.label) { richText = true };
            WinStyle = new GUIStyle(GUI.skin.window) { 
                normal = { background = Plugin.TexBg, textColor = new Color(0.957f, 0.706f, 0.102f) }, 
                onNormal = { background = Plugin.TexBg, textColor = new Color(0.957f, 0.706f, 0.102f) }, 
                padding = new RectOffset(15, 15, 30, 15),
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            
            SearchStyle = new GUIStyle(GUI.skin.textField) { 
                normal = { background = Plugin.TexInput, textColor = Color.white }, 
                padding = new RectOffset(12, 12, 10, 10), 
                fontSize = 15 
            };
            
            ItemStyle = new GUIStyle(GUI.skin.button) { 
                normal = { background = Plugin.TexPanel, textColor = new Color(0.9f, 0.9f, 0.9f) }, 
                hover = { background = Plugin.TexBtnHover }, 
                alignment = TextAnchor.MiddleLeft, 
                richText = true, 
                padding = new RectOffset(15, 15, 12, 12), 
                margin = new RectOffset(0, 0, 4, 4) 
            };
            
            BtnStyle = new GUIStyle(GUI.skin.button) { 
                normal = { background = Plugin.TexBtn, textColor = new Color(0.7f, 0.7f, 0.8f) }, 
                hover = { background = Plugin.TexBtnHover, textColor = Color.white }, 
                richText = true, 
                alignment = TextAnchor.MiddleCenter, 
                padding = new RectOffset(5, 5, 5, 5) 
            };
            
            BtnActiveStyle = new GUIStyle(BtnStyle) { 
                normal = { background = Plugin.TexBtnActiveDark, textColor = new Color(0.957f, 0.706f, 0.102f) } 
            };
            
            CloseBtnStyle = new GUIStyle(BtnStyle) { 
                normal = { background = Plugin.TexPanel }, 
                hover = { background = Plugin.MakeTex(2, 2, new Color(0.8f, 0.2f, 0.2f)) } 
            };

            AccordionStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { background = null, textColor = new Color(0.64f, 0.62f, 0.74f) },
                hover = { background = null, textColor = Color.white },
                alignment = TextAnchor.MiddleLeft,
                richText = true,
                padding = new RectOffset(0, 0, 8, 8),
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };

            _init = true;
        }

        public static void Draw(Dictionary<Guid, CardData> db)
        {
            Init();

            if (_firstOpen) { Refresh(db); _firstOpen = false; }

            WinRect = GUILayout.Window(99, WinRect, (id) => {
                GUI.DrawTexture(new Rect(0, 0, WinRect.width, 3), Plugin.TexGoldLine);

                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                _query = GUILayout.TextField(_query, SearchStyle, GUILayout.Height(40));

                if (GUILayout.Button("<color=#FF6666><b>✖</b></color>", CloseBtnStyle, GUILayout.Width(40), GUILayout.Height(40)))
                {
                    _query = "";
                    _activeCats.Clear(); _activeHeroes.Clear(); _activeTags.Clear(); _activeSizes.Clear(); _activeHiddenTags.Clear();
                    GUI.FocusControl(null); Refresh(db);
                }
                GUILayout.EndHorizontal();

                if (GUI.changed) Refresh(db);
                GUILayout.Space(12);

                DrawAccordionSection("CATEGORY", ref _showCat, _activeCats, new[] { "Item", "Skill", "Encounter" }, new[] { "ITEMS", "SKILLS", "EVENTS" }, db);
                DrawAccordionSection("HERO", ref _showHero, _activeHeroes, new[] { "Pygmalien", "Dooley", "Vanessa", "Mak", "Stelle", "Jules", "Karnok", "Common" }, new[] { "PYG", "DOO", "VAN", "MAK", "STE", "JUL", "KAR", "ALL" }, db, 4);
                DrawAccordionSection("OFFICIAL TAGS", ref _showTag, _activeTags, new[] { "Weapon", "Food", "Tool", "Tech", "Property", "Potion", "Friend", "Apparel", "Core", "Aquatic", "Dinosaur", "Relic", "Loot" }, new[] { "WEAPON", "FOOD", "TOOL", "TECH", "PROPERTY", "POTION", "FRIEND", "APPAREL", "CORE", "AQUATIC", "DINO", "RELIC", "LOOT" }, db, 4);
                DrawAccordionSection("MECHANICS (HIDDEN)", ref _showHiddenTag, _activeHiddenTags, new[] { "Ammo", "Burn", "Charge", "Cooldown", "Crit", "Damage", "XP", "Flying", "Freeze", "Gold", "Haste", "Heal", "Health", "Income", "Level", "Poison", "Regen", "Shield", "Slow", "Value" }, new[] { "AMMO", "BURN", "CHARGE", "COOLDOWN", "CRIT", "DAMAGE", "XP", "FLYING", "FREEZE", "GOLD", "HASTE", "HEAL", "HEALTH", "INCOME", "LEVEL", "POISON", "REGEN", "SHIELD", "SLOW", "VALUE" }, db, 4);
                DrawAccordionSection("SIZE", ref _showSize, _activeSizes, new[] { "Small", "Medium", "Large" }, new[] { "SMALL", "MEDIUM", "LARGE" }, db);

                GUILayout.Space(10);
                Rect sepRect = GUILayoutUtility.GetRect(1f, 2f, GUILayout.ExpandWidth(true));
                GUI.DrawTexture(sepRect, Plugin.TexPanel);
                GUILayout.Space(5);

                _scroll = GUILayout.BeginScrollView(_scroll);
                foreach (var c in _results)
                {
                    string cCol = Plugin.GetElementColor(c.Category);
                    string hName = c.Heroes.Count > 0 ? c.Heroes[0].Substring(0, Math.Min(3, c.Heroes[0].Length)).ToUpper() : "ALL";

                    if (GUILayout.Button($"<color=#77728B>[{hName}]</color> <size=15><b>{c.Name}</b></size> <color={Plugin.GetElementColor(c.Tier)}>[{c.Tier}]</color>\n<size=11><color={cCol}>{c.Category.ToUpper()}</color></size>", ItemStyle))
                        Selected = c;

                    if (Plugin.IsFocusMode && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) Selected = c;
                }
                GUILayout.EndScrollView();

                HandleResize(id, ref WinRect);
                GUI.DragWindow(new Rect(0, 0, 10000, 30));
            }, "◈ THE BAZAAR DATABASE", WinStyle);
        }

        private static void DrawAccordionSection(string title, ref bool isExpanded, HashSet<string> activeSet, string[] values, string[] labels, Dictionary<Guid, CardData> db, int itemsPerRow = 3)
        {
            string arrow = isExpanded ? "▼" : "▶";
            string color = activeSet.Count > 0 ? "#F4B41A" : "#A39EBD";

            if (GUILayout.Button($"<color={color}><size=13><b>{arrow} FILTRE : {title}</b></size></color>", AccordionStyle))
            {
                isExpanded = !isExpanded;
            }

            if (isExpanded)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                int count = 0;
                GUILayout.BeginHorizontal();
                for (int i = 0; i < values.Length; i++)
                {
                    DrawToggle(labels[i], values[i], activeSet, db);
                    if (++count % itemsPerRow == 0 && i < values.Length - 1)
                    {
                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.Space(5);
            }
        }

        private static void DrawToggle(string label, string value, HashSet<string> activeSet, Dictionary<Guid, CardData> db)
        {
            bool isActive = activeSet.Contains(value);
            string textColor = isActive ? Plugin.GetElementColor(value) : "#666677";

            if (GUILayout.Button($"<color={textColor}><size=11><b>{label}</b></size></color>", isActive ? BtnActiveStyle : BtnStyle, GUILayout.Height(28)))
            {
                if (isActive) activeSet.Remove(value);
                else activeSet.Add(value);
                Refresh(db);
            }
        }

        private static void Refresh(Dictionary<Guid, CardData> db)
        {
            _results.Clear();
            string[] t = _query.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var c in db.Values)
            {
                // --- RESTRICTION AUX ITEMS ET SKILLS UNIQUEMENT ---
                if (c.Category != "Item" && c.Category != "Skill") continue;

                if (_activeCats.Count > 0 && !_activeCats.Contains(c.Category)) continue;
                if (_activeHeroes.Count > 0 && (c.Heroes == null || !c.Heroes.Any(h => _activeHeroes.Contains(h)))) continue;
                if (_activeTags.Count > 0 && (c.Tags == null || !c.Tags.Any(tag => _activeTags.Contains(tag)))) continue;
                if (_activeSizes.Count > 0 && (string.IsNullOrEmpty(c.Size) || !_activeSizes.Contains(c.Size))) continue;
                if (_activeHiddenTags.Count > 0 && (c.HiddenTags == null || !c.HiddenTags.Any(ht => _activeHiddenTags.Contains(ht)))) continue;

                bool match = true;
                foreach (var term in t)
                {
                    bool found = c.Name.ToLower().Contains(term) || c.Tier.ToLower().Contains(term) || c.Category.ToLower().Contains(term) ||
                                 (!string.IsNullOrEmpty(c.Size) && c.Size.ToLower().Contains(term)) ||
                                 (c.Heroes != null && c.Heroes.Any(h => h.ToLower().Contains(term))) ||
                                 (c.Tags != null && c.Tags.Any(tg => tg.ToLower().Contains(term))) ||
                                 (c.HiddenTags != null && c.HiddenTags.Any(ht => ht.ToLower().Contains(term)));
                    if (!found) { match = false; break; }
                }
                if (match) _results.Add(c);
                if (_results.Count > 150) break;
            }
        }

        public static void HandleResize(int id, ref Rect r)
        {
            Rect h = new Rect(r.width - 20, r.height - 20, 20, 20);
            GUI.Label(h, "<color=#F4B41A><size=20>◢</size></color>", LabelStyle);
            if (Event.current.type == EventType.MouseDown && h.Contains(Event.current.mousePosition)) _resizingWindowID = id;
            if (_resizingWindowID == id)
            {
                r.width = Mathf.Max(350, Event.current.mousePosition.x + 5);
                r.height = Mathf.Max(300, Event.current.mousePosition.y + 5);
                if (Event.current.type == EventType.MouseUp) _resizingWindowID = -1;
            }
        }
    }
}