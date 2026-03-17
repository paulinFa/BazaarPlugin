using UnityEngine;
using System;
using System.Collections.Generic;

namespace BazaarPlugin
{
    public static class CardDetailsUI
    {
        public static Rect SearchRect = new Rect(550, 50, 620, 750);
        private static Vector2 _searchScroll;

        public static Rect HoverRect = new Rect(1190, 50, 620, 750);
        private static Vector2 _hoverScroll;

        private static GUIStyle _tagStyle, _descBoxStyle, _enchTitleStyle;

        private static void InitLocalStyles()
        {
            if (_tagStyle == null)
            {
                _tagStyle = new GUIStyle(GUI.skin.box)
                {
                    normal = { background = Plugin.TexBtn },
                    padding = new RectOffset(10, 10, 6, 6),
                    margin = new RectOffset(0, 6, 0, 0)
                };
                _descBoxStyle = new GUIStyle(GUI.skin.box) { padding = new RectOffset(20, 20, 20, 20), normal = { background = Plugin.TexPanel } };
                _enchTitleStyle = new GUIStyle(DatabaseUI.LabelStyle) { alignment = TextAnchor.MiddleCenter };
            }
        }

        public static void DrawSearch(CardData card)
        {
            InitLocalStyles();
            SearchRect = GUILayout.Window(100, SearchRect, (id) => {
                _searchScroll = DrawContent(card, _searchScroll, SearchRect.width, () => DatabaseUI.Selected = null);
                DatabaseUI.HandleResize(id, ref SearchRect);
                GUI.DragWindow(new Rect(0, 0, 10000, 30));
            }, "◈ CARD DETAILS (SEARCH)", DatabaseUI.WinStyle);
        }

        public static void DrawHover(CardData card)
        {
            InitLocalStyles();
            if (Event.current != null && Event.current.type == EventType.Repaint)
            {
                Plugin.IsMouseOverHoverWindow = HoverRect.Contains(Event.current.mousePosition);
            }

            HoverRect = GUILayout.Window(101, HoverRect, (id) => {
                _hoverScroll = DrawContent(card, _hoverScroll, HoverRect.width, () => Plugin.InGameHoveredCard = null);
                DatabaseUI.HandleResize(id, ref HoverRect);
                GUI.DragWindow(new Rect(0, 0, 10000, 30));
            }, "◈ CARD DETAILS (HOVER)", DatabaseUI.WinStyle);
        }

        private static Vector2 DrawContent(CardData card, Vector2 scroll, float currentWidth, Action onClose)
        {
            GUI.DrawTexture(new Rect(0, 0, currentWidth, 3), Plugin.TexCyanLine);

            if (GUI.Button(new Rect(currentWidth - 40, 10, 30, 30), "<color=#FF6666><b>✖</b></color>", DatabaseUI.CloseBtnStyle))
            {
                onClose?.Invoke();
            }

            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.Label($"<size=32><b><color=#FFF8E7>{card.Name}</color></b></size>", DatabaseUI.LabelStyle);
            GUILayout.FlexibleSpace();
            if (!string.IsNullOrEmpty(card.Cooldown))
                GUILayout.Label($"<size=22><color=#F4B41A><b>◈ {card.Cooldown}</b></color></size>", DatabaseUI.LabelStyle);
            GUILayout.Space(30);
            GUILayout.EndHorizontal();

            GUILayout.Space(12);

            GUILayout.BeginHorizontal();
            if (card.Heroes.Count > 0) DrawB(card.Heroes[0], Plugin.GetElementColor(card.Heroes[0]));
            DrawB(card.Tier, Plugin.GetElementColor(card.Tier));
            if (!string.IsNullOrEmpty(card.Size)) DrawB(card.Size, Plugin.GetElementColor(card.Size));
            
            if (card.Tags != null)
            {
                foreach (var tag in card.Tags) DrawB(tag, "#A39EBD");
            }
            GUILayout.EndHorizontal();

            if (card.HiddenTags != null && card.HiddenTags.Count > 0)
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                foreach (var hTag in card.HiddenTags) DrawB("★ " + hTag, Plugin.GetElementColor(hTag));
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(25);

            GUIStyle dStyle = new GUIStyle(GUI.skin.label) { richText = true, wordWrap = true, fontSize = 18, normal = { textColor = new Color(0.9f, 0.9f, 1f) } };
            GUILayout.BeginVertical(_descBoxStyle);
            GUILayout.Label(card.Description, dStyle);
            GUILayout.EndVertical();

            if (card.Enchantments != null && card.Enchantments.Count > 0)
            {
                GUILayout.Space(35);
                GUILayout.Label("<color=#F4B41A><size=14><b>AVAILABLE ENCHANTMENTS</b></size></color>", DatabaseUI.LabelStyle);
                GUILayout.Space(12);

                int count = 0; GUILayout.BeginHorizontal();
                foreach (var e in card.Enchantments)
                {
                    string hex = GetEHex(e.Key);
                    Color mainCol = GetEColor(e.Key);
                    GUIStyle boxS = new GUIStyle(GUI.skin.box)
                    {
                        normal = { background = Plugin.MakeTex(2, 2, new Color(mainCol.r, mainCol.g, mainCol.b, 0.15f)) },
                        padding = new RectOffset(15, 15, 15, 15),
                        margin = new RectOffset(0, 10, 0, 10)
                    };

                    GUILayout.BeginVertical(boxS, GUILayout.Width(currentWidth / 3 - 25), GUILayout.Height(140));
                    GUILayout.Label($"<color={hex}><size=15><b>{e.Key.ToUpper()}</b></size></color>", _enchTitleStyle);
                    
                    Rect lineRect = GUILayoutUtility.GetRect(1f, 2f, GUILayout.ExpandWidth(true));
                    GUI.DrawTexture(lineRect, Plugin.MakeTex(2, 2, new Color(mainCol.r, mainCol.g, mainCol.b, 0.4f)));
                    
                    GUILayout.Space(10);
                    GUILayout.Label($"<size=13><color=#EAEAEA>{e.Value}</color></size>", new GUIStyle(GUI.skin.label) { richText = true, wordWrap = true });
                    GUILayout.EndVertical();

                    if (++count % 3 == 0) { GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(); }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.Space(15);
            if (GUILayout.Button("<color=#666677><b>CLOSE DETAILS</b></color>", DatabaseUI.BtnStyle, GUILayout.Height(40)))
            {
                onClose?.Invoke();
            }

            return scroll;
        }

        private static void DrawB(string t, string c) { GUILayout.Label($"<color={c}><size=11><b>{t.ToUpper()}</b></size></color>", _tagStyle); }

        private static string GetEHex(string n)
        {
            switch (n.ToLower())
            {
                case "golden": return "#FFD700";
                case "shielded": return "#FFE600";
                case "heavy": return "#FFA500";
                case "icy": return "#00E5FF";
                case "turbo": return "#00FF7F";
                case "toxic": return "#7FFF00";
                case "restorative": return "#32CD32";
                case "fiery": return "#FF4500";
                case "deadly": return "#FF0000";
                case "obsidian": return "#DA70D6";
                case "shiny": return "#E0FFFF";
                case "radiant": return "#FFFACD";
                default: return "#FFFFFF";
            }
        }

        private static Color GetEColor(string n)
        {
            switch (n.ToLower())
            {
                case "golden": return new Color(1f, 0.84f, 0f);
                case "shielded": return new Color(1f, 0.9f, 0f);
                case "heavy": return new Color(1f, 0.6f, 0f);
                case "icy": return new Color(0f, 0.9f, 1f);
                case "turbo": return new Color(0f, 1f, 0.5f);
                case "toxic": return new Color(0.5f, 1f, 0f);
                case "restorative": return new Color(0.2f, 0.8f, 0.2f);
                case "fiery": return new Color(1f, 0.2f, 0f);
                case "deadly": return new Color(1f, 0f, 0f);
                case "obsidian": return new Color(0.85f, 0.44f, 0.84f);
                case "shiny": return new Color(0.88f, 1f, 1f);
                case "radiant": return new Color(1f, 0.98f, 0.8f);
                default: return Color.gray;
            }
        }
    }
}