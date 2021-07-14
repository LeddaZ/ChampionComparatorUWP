using System.Collections.Generic;

namespace ChampionComparatorUWP
{
    public class Item
    {
        public class Rune
        {
            public bool Isrune { get; set; }
            public int Tier { get; set; }
            public string Type { get; set; }
        }

        public class Gold
        {
            public int Base { get; set; }
            public int Total { get; set; }
            public int Sell { get; set; }
            public bool Purchasable { get; set; }
        }

        public class Stats
        {
            public int FlatHPPoolMod { get; set; }
            public int RFlatHPModPerLevel { get; set; }
            public int FlatMPPoolMod { get; set; }
            public int RFlatMPModPerLevel { get; set; }
            public int PercentHPPoolMod { get; set; }
            public int PercentMPPoolMod { get; set; }
            public int FlatHPRegenMod { get; set; }
            public int RFlatHPRegenModPerLevel { get; set; }
            public int PercentHPRegenMod { get; set; }
            public int FlatMPRegenMod { get; set; }
            public int RFlatMPRegenModPerLevel { get; set; }
            public int PercentMPRegenMod { get; set; }
            public int FlatArmorMod { get; set; }
            public int RFlatArmorModPerLevel { get; set; }
            public int PercentArmorMod { get; set; }
            public int RFlatArmorPenetrationMod { get; set; }
            public int RFlatArmorPenetrationModPerLevel { get; set; }
            public int RPercentArmorPenetrationMod { get; set; }
            public int RPercentArmorPenetrationModPerLevel { get; set; }
            public int FlatPhysicalDamageMod { get; set; }
            public int RFlatPhysicalDamageModPerLevel { get; set; }
            public int PercentPhysicalDamageMod { get; set; }
            public int FlatMagicDamageMod { get; set; }
            public int RFlatMagicDamageModPerLevel { get; set; }
            public int PercentMagicDamageMod { get; set; }
            public int FlatMovementSpeedMod { get; set; }
            public int RFlatMovementSpeedModPerLevel { get; set; }
            public int PercentMovementSpeedMod { get; set; }
            public int RPercentMovementSpeedModPerLevel { get; set; }
            public int FlatAttackSpeedMod { get; set; }
            public int PercentAttackSpeedMod { get; set; }
            public int RPercentAttackSpeedModPerLevel { get; set; }
            public int RFlatDodgeMod { get; set; }
            public int RFlatDodgeModPerLevel { get; set; }
            public int PercentDodgeMod { get; set; }
            public int FlatCritChanceMod { get; set; }
            public int RFlatCritChanceModPerLevel { get; set; }
            public int PercentCritChanceMod { get; set; }
            public int FlatCritDamageMod { get; set; }
            public int RFlatCritDamageModPerLevel { get; set; }
            public int PercentCritDamageMod { get; set; }
            public int FlatBlockMod { get; set; }
            public int PercentBlockMod { get; set; }
            public int FlatSpellBlockMod { get; set; }
            public int RFlatSpellBlockModPerLevel { get; set; }
            public int PercentSpellBlockMod { get; set; }
            public int FlatEXPBonus { get; set; }
            public int PercentEXPBonus { get; set; }
            public int RPercentCooldownMod { get; set; }
            public int RPercentCooldownModPerLevel { get; set; }
            public int RFlatTimeDeadMod { get; set; }
            public int RFlatTimeDeadModPerLevel { get; set; }
            public int RPercentTimeDeadMod { get; set; }
            public int RPercentTimeDeadModPerLevel { get; set; }
            public int RFlatGoldPer10Mod { get; set; }
            public int RFlatMagicPenetrationMod { get; set; }
            public int RFlatMagicPenetrationModPerLevel { get; set; }
            public int RPercentMagicPenetrationMod { get; set; }
            public int RPercentMagicPenetrationModPerLevel { get; set; }
            public int FlatEnergyRegenMod { get; set; }
            public int RFlatEnergyRegenModPerLevel { get; set; }
            public int FlatEnergyPoolMod { get; set; }
            public int RFlatEnergyModPerLevel { get; set; }
            public int PercentLifeStealMod { get; set; }
            public int PercentSpellVampMod { get; set; }
        }

        public class Maps
        {
            public bool _1 { get; set; }
            public bool _8 { get; set; }
            public bool _10 { get; set; }
            public bool _12 { get; set; }
            public bool _11 { get; set; }
            public bool _21 { get; set; }
            public bool _22 { get; set; }
        }

        public class Basic
        {
            public string Name { get; set; }
            public Rune Rune { get; set; }
            public Gold Gold { get; set; }
            public string Group { get; set; }
            public string Description { get; set; }
            public string Colloq { get; set; }
            public string Plaintext { get; set; }
            public bool Consumed { get; set; }
            public int Stacks { get; set; }
            public int Depth { get; set; }
            public bool ConsumeOnFull { get; set; }
            public List<object> From { get; set; }
            public List<object> Into { get; set; }
            public int SpecialRecipe { get; set; }
            public bool InStore { get; set; }
            public bool HideFromAll { get; set; }
            public string RequiredChampion { get; set; }
            public string RequiredAlly { get; set; }
            public Stats Stats { get; set; }
            public List<object> Tags { get; set; }
            public Maps Maps { get; set; }
        }

        public class Image
        {
            public string Full { get; set; }
            public string Sprite { get; set; }
            public string Group { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int W { get; set; }
            public int H { get; set; }
        }

        public class Effect
        {
            public string Effect1Amount { get; set; }
            public string Effect2Amount { get; set; }
            public string Effect3Amount { get; set; }
            public string Effect4Amount { get; set; }
            public string Effect5Amount { get; set; }
            public string Effect6Amount { get; set; }
            public string Effect7Amount { get; set; }
            public string Effect8Amount { get; set; }
            public string Effect9Amount { get; set; }
            public string Effect10Amount { get; set; }
            public string Effect11Amount { get; set; }
            public string Effect12Amount { get; set; }
            public string Effect13Amount { get; set; }
            public string Effect14Amount { get; set; }
            public string Effect15Amount { get; set; }
            public string Effect16Amount { get; set; }
            public string Effect17Amount { get; set; }
            public string Effect18Amount { get; set; }
        }

        public class Group
        {
            public string Id { get; set; }
            public string MaxGroupOwnable { get; set; }
        }

        public class Tree
        {
            public string Header { get; set; }
            public List<string> Tags { get; set; }
        }

        public class Root
        {
            public string Type { get; set; }
            public string Version { get; set; }
            public Basic Basic { get; set; }
            public List<Group> Groups { get; set; }
            public List<Tree> Tree { get; set; }
        }


    }
}
