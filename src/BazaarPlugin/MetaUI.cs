using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;

namespace BazaarPlugin
{
    public class MetaBuild
    {
        public string Hero;
        public string Category;
        public string Archetype;
        public List<string> CoreItems;
        public string Notes;
        public List<string> Images;
    }

    public static class MetaUI
    {
        public static Rect WinRect = new Rect(100, 100, 1100, 850);
        public static List<MetaBuild> AllBuilds = new List<MetaBuild>();
        private static string _selectedHero = "VANESSA";
        private static Vector2 _scrollPos;
        private static Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();
        
        public static GUIStyle WinStyle, TabStyle, TabActiveStyle, CardStyle, TitleStyle, NoteStyle, ItemTagStyle, CategoryStyle;
        private static bool _init = false;

        private static void InitStyles()
        {
            if (_init) return;
            
            WinStyle = new GUIStyle(GUI.skin.window);
            WinStyle.normal.background = Plugin.TexBg;
            WinStyle.onNormal.background = Plugin.TexBg;
            WinStyle.padding = new RectOffset(15, 15, 45, 15);
            
            TabStyle = new GUIStyle(GUI.skin.button);
            TabStyle.fontSize = 13;
            TabStyle.fixedHeight = 45;
            TabStyle.fontStyle = FontStyle.Bold;
            TabStyle.normal.background = Plugin.TexBtn;
            TabStyle.normal.textColor = new Color(0.6f, 0.6f, 0.7f);

            TabActiveStyle = new GUIStyle(TabStyle);
            TabActiveStyle.normal.background = Plugin.TexBtnActiveDark;
            TabActiveStyle.normal.textColor = new Color(0.957f, 0.706f, 0.102f);
            
            CardStyle = new GUIStyle(GUI.skin.box);
            CardStyle.padding = new RectOffset(20, 20, 20, 20);
            CardStyle.margin = new RectOffset(0, 0, 10, 10);
            CardStyle.normal.background = Plugin.TexPanel;
            
            TitleStyle = new GUIStyle(GUI.skin.label);
            TitleStyle.fontSize = 24;
            TitleStyle.fontStyle = FontStyle.Bold;
            TitleStyle.normal.textColor = new Color(0.957f, 0.706f, 0.102f);

            CategoryStyle = new GUIStyle(GUI.skin.label);
            CategoryStyle.fontSize = 13;
            CategoryStyle.fontStyle = FontStyle.Bold;
            CategoryStyle.normal.textColor = new Color(0f, 0.898f, 1f);
            
            NoteStyle = new GUIStyle(GUI.skin.label);
            NoteStyle.fontSize = 15;
            NoteStyle.wordWrap = true;
            NoteStyle.richText = true;
            NoteStyle.normal.textColor = new Color(0.9f, 0.9f, 0.95f);

            ItemTagStyle = new GUIStyle(GUI.skin.box);
            ItemTagStyle.fontSize = 12;
            ItemTagStyle.fontStyle = FontStyle.Bold;
            ItemTagStyle.padding = new RectOffset(10, 10, 5, 5);
            ItemTagStyle.normal.background = Plugin.TexBtn;
            ItemTagStyle.normal.textColor = Color.white;

            _init = true;
        }

        private static Texture2D LoadTexture(string hero, string name, int index)
        {
            string fileName = name;
            if (index > 1) fileName = name + "_" + index;
            
            string key = string.Concat(hero, "_", fileName);
            if (_textureCache.ContainsKey(key)) return _textureCache[key];

            string path = Path.Combine(Paths.PluginPath, "BazaarDB");
            path = Path.Combine(path, "meta");
            path = Path.Combine(path, hero);
            path = Path.Combine(path, string.Concat(fileName, ".png"));

            if (File.Exists(path))
            {
                byte[] data = File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                
                var method = typeof(UnityEngine.ImageConversion).GetMethod("LoadImage", new System.Type[] { typeof(Texture2D), typeof(byte[]) });
                if (method != null) method.Invoke(null, new object[] { tex, data });

                tex.filterMode = FilterMode.Bilinear;
                tex.Apply();
                _textureCache[key] = tex;
                return tex;
            }
            return null;
        }

        private static readonly string[] _heroes = { "VANESSA", "DOOLEY", "PYG", "MAK", "STELLE", "JULES", "KARNOK" };

        public static void Draw()
        {
            InitStyles();

            WinRect = GUI.Window(103, WinRect, (id) => {
                GUI.DrawTexture(new Rect(0, 0, WinRect.width, 3), Plugin.TexGoldLine);

                GUILayout.BeginHorizontal();
                foreach (var h in _heroes)
                {
                    if (GUILayout.Button(h, _selectedHero == h ? TabActiveStyle : TabStyle))
                    {
                        _selectedHero = h;
                        _scrollPos = Vector2.zero;
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(25);

                _scrollPos = GUILayout.BeginScrollView(_scrollPos);
                var builds = AllBuilds.Where(b => b.Hero.ToUpper() == _selectedHero).ToList();
                
                if (builds.Count == 0)
                {
                    GUILayout.Space(100);
                    string msg = "<center><color=grey><size=22><i>No Meta data found for " + _selectedHero + "</i></size></color></center>";
                    GUILayout.Label(msg, new GUIStyle(GUI.skin.label) { richText = true });
                }

                foreach (var build in builds)
                {
                    DrawBuildCard(build);
                    GUILayout.Space(40);
                }
                
                GUILayout.EndScrollView();

                DatabaseUI.HandleResize(id, ref WinRect);
                GUI.DragWindow(new Rect(0, 0, WinRect.width, 40));
            }, "◈ BAZAAR META BROWSER", WinStyle);
        }

        private static void DrawBuildCard(MetaBuild build)
        {
            GUILayout.BeginVertical(CardStyle);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(build.Archetype.ToUpper(), TitleStyle);
            GUILayout.FlexibleSpace();
            if(!string.IsNullOrEmpty(build.Category))
                GUILayout.Label("« " + build.Category.ToUpper() + " »", CategoryStyle);
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            Texture2D img1 = LoadTexture(build.Hero, build.Archetype, 1);
            Texture2D img2 = LoadTexture(build.Hero, build.Archetype, 2);

            if (img1 != null || img2 != null)
            {
                GUILayout.BeginHorizontal();
                if (img1 != null) DrawImage(img1, 1);
                if (img2 != null) DrawImage(img2, 2);
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(120));
                GUILayout.Label("<color=#FF4B4B>BOARD IMAGE MISSING</color>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                GUILayout.Label("Ensure BazaarDB/meta/" + build.Hero + "/" + build.Archetype + ".png exists.", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 12 });
                GUILayout.EndVertical();
            }

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            
            GUILayout.BeginVertical(GUILayout.Width(280));
            GUILayout.Label("<color=#A39EBD><size=14><b>CORE ITEMS</b></size></color>");
            GUILayout.Space(10);
            foreach (var item in build.CoreItems)
            {
                GUILayout.BeginHorizontal();
                GUI.DrawTexture(GUILayoutUtility.GetRect(4, 25), Plugin.TexGoldLine);
                GUILayout.Space(5);
                GUILayout.Label(item, ItemTagStyle, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
                GUILayout.Space(4);
            }
            GUILayout.EndVertical();

            GUILayout.Space(30);

            GUILayout.BeginVertical();
            GUILayout.Label("<color=#00E5FF><size=14><b>STRATEGY & TIPS</b></size></color>");
            GUILayout.Space(10);
            GUILayout.Label(build.Notes, NoteStyle);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private static void DrawImage(Texture2D tex, int index)
        {
            float maxWidth = (WinRect.width - 100) / 2f;
            float aspect = (float)tex.height / tex.width;
            float w = Mathf.Min(maxWidth, tex.width);
            float h = w * aspect;
            
            Rect r = GUILayoutUtility.GetRect(w, h);
            GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit);
        }
    }
}