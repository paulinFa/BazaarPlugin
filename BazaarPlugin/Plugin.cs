using BepInEx;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using BazaarGameClient.Domain.Cards;
using BazaarGameClient.Domain.Hints;
using BazaarGameClient.Domain.Models.Cards;

namespace BazaarPlugin
{
    [BepInPlugin("com.mod.bazaardb", "Bazaar Database", "5.0.2")]
    public class Plugin : BaseUnityPlugin
    {
        public static Dictionary<Guid, CardData> Database = new Dictionary<Guid, CardData>();

        public static bool IsShowing = false, IsFocusMode = false, IsF6Active = false, IsF5Active = false, IsF7Active = false;
        public static bool ShowDebugFocus = false; // Set to true to show the focus debug info

        public static bool IsMouseOverUI()
        {
            if (!IsShowing && !IsF7Active && !IsF5Active && !IsF6Active) return false;
            
            Vector2 m = new Vector2(SafeMousePos.x, Screen.height - SafeMousePos.y);
            
            if (IsShowing)
            {
                if (DatabaseUI.WinRect.Contains(m)) return true;
                if (DatabaseUI.Selected != null && CardDetailsUI.SearchRect.Contains(m)) return true;
            }
            if (IsF7Active && MetaUI.WinRect.Contains(m)) return true;
            if (IsF5Active && MonsterUI.MobRect.Contains(m)) return true;
            if (IsF6Active && CardDetailsUI.HoverRect.Contains(m)) return true;
            
            return false;
        }

        public static CardData InGameHoveredCard = null;
        public static string UnknownHoveredId = "";
        public static float InGameTimer = 0f;
        public static bool IsMouseOverHoverWindow = false;

        public static Texture2D TexBg, TexPanel, TexBtn, TexBtnHover, TexBtnActiveDark, TexInput, TexGoldLine, TexCyanLine;
        public static Vector2 SafeMousePos;

        void Awake()
        {
            TexBg = MakeTex(2, 2, new Color(0.043f, 0.039f, 0.071f, 0.98f)); // #0B0A12
            TexPanel = MakeTex(2, 2, new Color(0.094f, 0.086f, 0.157f, 1f)); // #181628
            TexBtn = MakeTex(2, 2, new Color(0.125f, 0.114f, 0.208f, 1f)); // #201D35
            TexBtnHover = MakeTex(2, 2, new Color(0.188f, 0.173f, 0.314f, 1f)); // #302C50
            TexBtnActiveDark = MakeTex(2, 2, new Color(0.25f, 0.23f, 0.4f, 1f));
            TexInput = MakeTex(2, 2, new Color(0.02f, 0.02f, 0.04f, 1f));
            TexGoldLine = MakeTex(2, 2, new Color(0.957f, 0.706f, 0.102f, 1f)); // #F4B41A
            TexCyanLine = MakeTex(2, 2, new Color(0f, 0.898f, 1f, 1f)); // #00E5FF

            LoadData();
            new Harmony("com.mod.bazaardb.patch").PatchAll();
            Logger.LogInfo("BazaarPlugin v5.1.0 : Modern UI Update.");
        }

        private void LoadData()
        {
            string dbDir = Path.Combine(Paths.PluginPath, "BazaarDB");
            if (!Directory.Exists(dbDir)) Directory.CreateDirectory(dbDir);

            string path = Path.Combine(dbDir, "clean_cards.json");
            if (File.Exists(path))
            {
                Database = JsonConvert.DeserializeObject<Dictionary<Guid, CardData>>(File.ReadAllText(path));
            }

            string metaPath = Path.Combine(dbDir, "meta_data.json");
            if (File.Exists(metaPath))
            {
                MetaUI.AllBuilds = JsonConvert.DeserializeObject<List<MetaBuild>>(File.ReadAllText(metaPath));
            }
        }

        void Update()
        {
            // --- GESTION DU MINUTEUR ---
            if (InGameHoveredCard != null || UnknownHoveredId != "")
            {
                if (IsMouseOverHoverWindow)
                {
                    InGameTimer = 0f;
                }
                else
                {
                    InGameTimer += Time.deltaTime;
                    if (InGameTimer >= 10f)
                    {
                        InGameHoveredCard = null;
                        UnknownHoveredId = "";
                        InGameTimer = 0f;
                    }
                }
            }

            // NOTE : La logique toxique de désactivation de l'EventSystem a été supprimée ici.
        }

        void OnGUI()
        {
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
            GUI.depth = -1000;

            if (Event.current != null)
            {
                SafeMousePos = Event.current.mousePosition;

                if (Event.current.type == EventType.KeyDown)
                {
                    if (Event.current.keyCode == KeyCode.F8) { IsShowing = !IsShowing; Event.current.Use(); }
                    if (Event.current.keyCode == KeyCode.F9) { IsFocusMode = !IsFocusMode; Event.current.Use(); }
                    if (Event.current.keyCode == KeyCode.F6) { IsF6Active = !IsF6Active; Event.current.Use(); }
                    if (Event.current.keyCode == KeyCode.F5) { IsF5Active = !IsF5Active; Event.current.Use(); }
                    if (Event.current.keyCode == KeyCode.F7) { IsF7Active = !IsF7Active; Event.current.Use(); }
                }
            }

            DrawMiniHUD();

            if (IsShowing)
            {
                DatabaseUI.Draw(Database);
                if (DatabaseUI.Selected != null)
                {
                    CardDetailsUI.DrawSearch(DatabaseUI.Selected);
                }
            }

            if (IsF7Active)
            {
                MetaUI.Draw();
            }

            if (IsF5Active && InGameHoveredCard != null)
            {
                MonsterUI.Draw(InGameHoveredCard);
            }

            if (IsF6Active && InGameHoveredCard != null)
            {
                CardDetailsUI.DrawHover(InGameHoveredCard);
            }

            if (ShowDebugFocus && (IsF5Active || IsF6Active))
            {
                Rect focusRect = new Rect(380, 20, 350, 85);
                GUI.DrawTexture(focusRect, TexBg);
                GUI.DrawTexture(new Rect(focusRect.x, focusRect.y, 3, focusRect.height), TexCyanLine);
                GUILayout.BeginArea(focusRect, GUI.skin.box);
                GUILayout.Label("<color=#00E5FF><b>◈ MOUSE FOCUS ACTIVE</b></color>");
                GUILayout.Space(5);

                if (InGameHoveredCard != null)
                {
                    GUILayout.Label("<color=white><size=14><b>" + InGameHoveredCard.Name + "</b></size></color> <color=#A39EBD>(" + InGameHoveredCard.Category + ")</color>");
                }
                else if (UnknownHoveredId != "")
                {
                    GUILayout.Label("<color=#FF4B4B><size=14><b>UNKNOWN ID: " + UnknownHoveredId + "</b></size></color>");
                }
                else
                {
                    GUILayout.Label("<color=grey><size=14><i>Waiting for hover...</i></size></color>");
                }
                GUILayout.EndArea();
            }
        }

        private void DrawMiniHUD()
        {
            Rect hudRect = new Rect(Screen.width - 450, 10, 440, 35);
            GUI.DrawTexture(hudRect, MakeTex(2, 2, new Color(0.04f, 0.04f, 0.06f, 0.9f)));
            GUI.DrawTexture(new Rect(hudRect.x, hudRect.y + hudRect.height - 2, hudRect.width, 2), TexGoldLine);
            
            GUILayout.BeginArea(hudRect);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawHudText("F5 MOBS", IsF5Active);
            GUILayout.Space(15);
            DrawHudText("F6 HOVER", IsF6Active);
            GUILayout.Space(15);
            DrawHudText("F7 META", IsF7Active);
            GUILayout.Space(15);
            DrawHudText("F8 DB", IsShowing);
            GUILayout.Space(15);
            DrawHudText("F9 FOCUS", IsFocusMode);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawHudText(string label, bool isActive)
        {
            string color = isActive ? "#F4B41A" : "#666677";
            string prefix = isActive ? "● " : "○ ";
            GUILayout.Label("<color=" + color + "><size=12><b>" + prefix + label + "</b></size></color>", new GUIStyle(GUI.skin.label) { richText = true, alignment = TextAnchor.MiddleCenter });
        }

        public static Texture2D MakeTex(int w, int h, Color col)
        {
            Texture2D res = new Texture2D(w, h);
            Color[] pix = new Color[w * h];
            for (int i = 0; i < pix.Length; i++) pix[i] = col;
            res.SetPixels(pix); res.Apply();
            return res;
        }

        public static string GetElementColor(string element)
        {
            if (string.IsNullOrEmpty(element)) return "#CCCCCC";
            switch (element.ToUpper())
            {
                case "PYGMALIEN": case "PYG": return "#FF69B4";
                case "DOOLEY": case "DOO": return "#9370DB";
                case "VANESSA": case "VAN": return "#1E90FF";
                case "MAK": return "#228B22";
                case "STELLE": case "STE": return "#00FFFF";
                case "JULES": case "JUL": return "#FF8C00";
                case "KARNOK": case "KAR": return "#DC143C";
                case "COMMON": case "ALL": return "#AAAAAA";
                case "ITEM": case "ITEMS": return "#00E5FF";
                case "SKILL": case "SKILLS": return "#FF8C00";
                case "ENCOUNTER": case "EVENTS": return "#B388FF";
                case "SMALL": return "#888888";
                case "MEDIUM": return "#AAAAAA";
                case "LARGE": return "#DDDDDD";
                case "WEAPON": case "DAMAGE": case "CRIT": case "BURN": return "#FF4B4B";
                case "GOLD": case "XP": return "#FFD700";
                default: return "#CCCCCC";
            }
        }

        [HarmonyPatch(typeof(UnityEngine.EventSystems.EventSystem), "IsPointerOverGameObject", new Type[] { })]
        public class BlockClickPatch
        {
            static bool Prefix(ref bool __result)
            {
                if (IsMouseOverUI())
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(UnityEngine.Input), "GetAxis")]
        public class BlockScrollPatch
        {
            static bool Prefix(string axisName, ref float __result)
            {
                if ((axisName == "Mouse ScrollWheel") && IsMouseOverUI())
                {
                    __result = 0f;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(UnityEngine.Input), "mouseScrollDelta", MethodType.Getter)]
        public class BlockScrollDeltaPatch
        {
            static bool Prefix(ref Vector2 __result)
            {
                if (IsMouseOverUI())
                {
                    __result = Vector2.zero;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(HintService), "ListHints")]
        public class HoverItemPatch
        {
            static void Postfix(BazaarGameClient.Domain.Models.Cards.Card me)
            {
                if (me == null || (!IsF6Active && !IsF5Active)) return;

                InGameTimer = 0f;
                if (Database.TryGetValue(me.TemplateId, out CardData found))
                {
                    InGameHoveredCard = found;
                    UnknownHoveredId = "";
                }
                else
                {
                    InGameHoveredCard = null;
                    UnknownHoveredId = me.TemplateId.ToString();
                }
            }
        }

        [HarmonyPatch(typeof(CardController), "IsHovering", MethodType.Setter)]
        public class EncounterHoverEvent
        {
            static void Postfix(CardController __instance, bool value)
            {
                if (__instance == null || (!IsF5Active && !IsF6Active)) return;

                if (value == true)
                {
                    if (__instance.CardData != null)
                    {
                        InGameTimer = 0f;
                        if (Database.TryGetValue(__instance.CardData.TemplateId, out CardData found))
                        {
                            InGameHoveredCard = found;
                            UnknownHoveredId = "";
                        }
                        else
                        {
                            InGameHoveredCard = null;
                            UnknownHoveredId = __instance.CardData.TemplateId.ToString();
                        }
                    }
                }
            }
        }
    }
}