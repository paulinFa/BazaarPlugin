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

        public static bool IsShowing = false, IsFocusMode = false, IsF6Active = false, IsF5Active = false;

        public static CardData InGameHoveredCard = null;
        public static string UnknownHoveredId = "";
        public static float InGameTimer = 0f;
        public static bool IsMouseOverHoverWindow = false;

        public static Texture2D TexBg, TexPanel, TexBtn, TexBtnHover, TexBtnActiveDark, TexInput, TexGoldLine;
        public static Vector2 SafeMousePos;

        void Awake()
        {
            TexBg = MakeTex(2, 2, new Color(0.055f, 0.047f, 0.086f, 1f));
            TexPanel = MakeTex(2, 2, new Color(0.094f, 0.082f, 0.149f, 1f));
            TexBtn = MakeTex(2, 2, new Color(0.141f, 0.125f, 0.220f, 1f));
            TexBtnHover = MakeTex(2, 2, new Color(0.204f, 0.180f, 0.314f, 1f));
            TexBtnActiveDark = MakeTex(2, 2, new Color(0.18f, 0.16f, 0.28f, 1f));
            TexInput = MakeTex(2, 2, new Color(0.039f, 0.035f, 0.059f, 1f));
            TexGoldLine = MakeTex(2, 2, new Color(0.957f, 0.706f, 0.102f, 0.8f));

            LoadData();
            new Harmony("com.mod.bazaardb.patch").PatchAll();
            Logger.LogInfo("BazaarPlugin v5.0.2 : Agrandissement Monstre et suppression du blocage d'EventSystem.");
        }

        private void LoadData()
        {
            string path = Path.Combine(Paths.PluginPath, "BazaarDB", "clean_cards.json");
            if (File.Exists(path))
            {
                Database = JsonConvert.DeserializeObject<Dictionary<Guid, CardData>>(File.ReadAllText(path));
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

            if (IsF5Active && InGameHoveredCard != null)
            {
                MonsterUI.Draw(InGameHoveredCard);
            }

            if (IsF6Active && InGameHoveredCard != null)
            {
                CardDetailsUI.DrawHover(InGameHoveredCard);
            }

            if (IsF5Active || IsF6Active)
            {
                GUILayout.BeginArea(new Rect(380, 20, 350, 80), GUI.skin.box);
                GUILayout.Label("<color=magenta><b>FOCUS SOURIS ACTIF</b></color>");
                GUILayout.Space(5);

                if (InGameHoveredCard != null)
                {
                    GUILayout.Label($"<color=white><size=14><b>{InGameHoveredCard.Name}</b></size></color> <color=cyan>({InGameHoveredCard.Category})</color>");
                }
                else if (UnknownHoveredId != "")
                {
                    GUILayout.Label($"<color=red><size=14><b>ID INCONNU : {UnknownHoveredId}</b></size></color>");
                }
                else
                {
                    GUILayout.Label("<color=grey><size=14><i>En attente de survol...</i></size></color>");
                }
                GUILayout.EndArea();
            }
        }

        private void DrawMiniHUD()
        {
            Rect hudRect = new Rect(Screen.width - 320, 10, 310, 30);
            GUI.DrawTexture(hudRect, MakeTex(2, 2, new Color(0.05f, 0.05f, 0.08f, 0.8f)));
            GUILayout.BeginArea(hudRect);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawHudText("F5 MONSTERS", IsF5Active);
            GUILayout.Space(10);
            DrawHudText("F6 HOVER", IsF6Active);
            GUILayout.Space(10);
            DrawHudText("F8 BAZAAR DB", IsShowing);
            GUILayout.Space(10);
            DrawHudText("F9 FOCUS", IsFocusMode);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawHudText(string label, bool isActive)
        {
            string color = isActive ? "#F4B41A" : "#555555";
            GUILayout.Label($"<color={color}><size=11><b>{label}</b></size></color>", new GUIStyle(GUI.skin.label) { richText = true, alignment = TextAnchor.MiddleCenter });
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