using System;
using System.Collections.Generic;

namespace BazaarPlugin
{
    public class CardData
    {
        public Guid Id;
        public string Name = "";
        public string Category = "";
        public string Size = "";
        public List<string> Heroes = new List<string>();
        public string Tier = "";
        public List<string> Tags = new List<string>();
        public List<string> HiddenTags = new List<string>();
        public string Cooldown = "";
        public string Description = "";
        public Dictionary<string, string> Enchantments = new Dictionary<string, string>();
    }
}