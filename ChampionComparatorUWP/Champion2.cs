// ChampionsComparator.Champion1

using System.Collections.Generic;

namespace ChampionComparatorUWP
{
    public class Champion2
    {
        public class Image
        {
            public string Full { get; set; }

            public string Sprite { get; set; }

            public string Group { get; set; }

            public double X { get; set; }

            public double Y { get; set; }

            public double W { get; set; }

            public double H { get; set; }
        }

        public class Skin
        {
            public string Id { get; set; }

            public double Num { get; set; }

            public string Name { get; set; }

            public bool Chromas { get; set; }
        }

        public class Info
        {
            public double Attack { get; set; }

            public double Defense { get; set; }

            public double Magic { get; set; }

            public double Difficulty { get; set; }
        }

        public class Stats
        {
            public double Hp { get; set; }

            public double HpPerLevel { get; set; }

            public double Mp { get; set; }

            public double MpPerLevel { get; set; }

            public double MoveSpeed { get; set; }

            public double Armor { get; set; }

            public double ArmorPerLevel { get; set; }

            public double SpellBlock { get; set; }

            public double SpellBlockPerLevel { get; set; }

            public double AttackRange { get; set; }

            public double HpRegen { get; set; }

            public double HpRegenPerLevel { get; set; }

            public double MpRegen { get; set; }

            public double MpRegenPerLevel { get; set; }

            public double Crit { get; set; }

            public double CritPerLevel { get; set; }

            public double AttackDamage { get; set; }

            public double AttackDamagePerLevel { get; set; }

            public double AttackSpeedPerLevel { get; set; }

            public double AttackSpeed { get; set; }
        }

        public class Leveltip
        {
            public List<string> Label { get; set; }

            public List<string> Effect { get; set; }
        }

        public class Datavalues
        {
        }

        public class Spell
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public string Tooltip { get; set; }

            public Leveltip LevelTip { get; set; }

            public double MaxRank { get; set; }

            public List<double> Cooldown { get; set; }

            public string CooldownBurn { get; set; }

            public List<double> Cost { get; set; }

            public string CostBurn { get; set; }

            public Datavalues Datavalues { get; set; }

            public List<List<double>> Effect { get; set; }

            public List<string> EffectBurn { get; set; }

            public List<object> Vars { get; set; }

            public string CostType { get; set; }

            public string MaxAmmo { get; set; }

            public List<double> Range { get; set; }

            public string RangeBurn { get; set; }

            public Image Image { get; set; }

            public string Resource { get; set; }
        }

        public class Passive
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public Image Image { get; set; }
        }

        public class Champion
        {
            public string Id { get; set; }

            public string Key { get; set; }

            public string Name { get; set; }

            public string Title { get; set; }

            public Image Image { get; set; }

            public List<Skin> Skins { get; set; }

            public string Lore { get; set; }

            public string Blurb { get; set; }

            public List<string> AllyTips { get; set; }

            public List<string> EnemyTips { get; set; }

            public List<string> Tags { get; set; }

            public string ParType { get; set; }

            public Info Info { get; set; }

            public Stats Stats { get; set; }

            public List<Spell> Spells { get; set; }

            public Passive Passive { get; set; }

            public List<object> Recommended { get; set; }
        }

        public class Root
        {
            public string Type { get; set; }

            public string Format { get; set; }

            public string Version { get; set; }

            public Dictionary<string, Champion> Data { get; set; }
        }
    }
}
