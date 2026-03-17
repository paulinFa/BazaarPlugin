using UnityEngine;
using System.IO;
using System.Collections.Generic;
using BepInEx;

namespace BazaarPlugin
{
    public static class MonsterUI
    {
        public static Rect MobRect;
        private static bool _init = false;
        private static GUIStyle _mobWindowStyle;

        private static Dictionary<string, Texture2D> _mobTextures = new Dictionary<string, Texture2D>();

        public static Texture2D GetMobTexture(string mobName)
        {
            if (_mobTextures.ContainsKey(mobName)) return _mobTextures[mobName];

            string path = Path.Combine(Paths.PluginPath, "mob", mobName + ".png");
            if (File.Exists(path))
            {
                byte[] fileData = File.ReadAllBytes(path);
                // mipChain: false -> Désactive les mipmaps pour garder une texture brute et nette en UI
                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);

                var method = typeof(UnityEngine.ImageConversion).GetMethod("LoadImage", new System.Type[] { typeof(Texture2D), typeof(byte[]) });
                if (method != null)
                {
                    method.Invoke(null, new object[] { tex, fileData });
                }

                // Filtrage Bilinear sans mipmaps = rendu le plus fidèle pour l'UI 2D
                tex.filterMode = FilterMode.Bilinear;
                tex.anisoLevel = 16; // Maximum de détails sur les textures
                tex.Apply(); 

                _mobTextures[mobName] = tex;
                return tex;
            }

            _mobTextures[mobName] = null;
            return null;
        }

        public static void Draw(CardData card)
        {
            if (card == null) return;

            Texture2D tex = GetMobTexture(card.Name);
            if (tex == null) return;

            if (!_init)
            {
                float width = 1400f;
                float height = (tex.height * width) / tex.width;
                MobRect = new Rect(Screen.width / 2f - (width / 2f), 80f, width, height);
                _mobWindowStyle = new GUIStyle(GUI.skin.window);
                _mobWindowStyle.normal.background = null;
                _mobWindowStyle.onNormal.background = null;
                _init = true;
            }

            // Utilisation de Mathf.Round pour s'assurer que le Rect tombe pile sur les pixels de l'écran
            MobRect = GUI.Window(102, MobRect, (id) => {
                GUI.DrawTexture(new Rect(0, 0, Mathf.Round(MobRect.width), Mathf.Round(MobRect.height)), tex, ScaleMode.ScaleToFit);
                
                DatabaseUI.HandleResize(id, ref MobRect);
                GUI.DragWindow(new Rect(0, 0, MobRect.width, MobRect.height));
            }, "", _mobWindowStyle);
        }
    }
}