using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using Windows.ApplicationModel;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ChampionComparatorUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static string latestPatch;
        private string champ1, champ2, ch1, ch2;
        private readonly string[] stats = new string[46];
        private readonly HttpClient client = new HttpClient();

        // Autocomplete source
        private readonly List<string> autosrc = new List<string>()
        {
            "Aatrox", "Ahri", "Akali", "Alistar", "Amumu", "Anivia", "Annie", "Aphelios", "Ashe", "AurelionSol",
            "Azir", "Bard", "Blitzcrank", "Brand", "Braum", "Caitlyn", "Camille", "Cassiopeia", "Chogath",
            "Corki", "Darius", "Diana", "Draven", "DrMundo", "Ekko", "Elise", "Evelynn", "Ezreal", "Fiddlesticks",
            "Fiora", "Fizz", "Galio", "Gangplank", "Garen", "Gnar", "Gragas", "Graves", "Gwen", "Hecarim",
            "Heimerdinger", "Illaoi", "Irelia", "Ivern", "Janna", "JarvanIV", "Jax", "Jayce", "Jhin", "Jinx",
            "Kaisa", "Kalista", "Karma", "Karthus", "Kassadin", "Katarina", "Kayle", "Kayn", "Kennen", "Khazix",
            "Kindred", "Kled", "KogMaw", "Leblanc", "LeeSin", "Leona", "Lillia", "Lissandra", "Lucian", "Lulu",
            "Lux", "Malphite", "Malzahar", "Maokai", "MasterYi", "MissFortune", "Wukong", "Mordekaiser", "Morgana",
            "Nami", "Nasus", "Nautilus", "Neeko", "Nidalee", "Nocturne", "Nunu", "Olaf", "Orianna", "Ornn",
            "Pantheon", "Poppy", "Pyke", "Qiyana", "Quinn", "Rakan", "Rammus", "RekSai", "Rell", "Renekton",
            "Rengar", "Riven", "Rumble", "Ryze", "Samira", "Sejuani", "Senna", "Seraphine", "Sett", "Shaco", "Shen",
            "Shyvana", "Singed", "Sion", "Sivir", "Skarner", "Sona", "Soraka", "Swain", "Sylas", "Syndra",
            "TahmKench", "Taliyah", "Talon", "Taric", "Teemo", "Thresh", "Tristana", "Trundle", "Tryndamere",
            "TwistedFate", "Twitch", "Udyr", "Urgot", "Varus", "Vayne", "Veigar", "Velkoz", "Vi", "Viego", "Viktor",
            "Vladimir", "Volibear", "Warwick", "Xayah", "Xerath", "XinZhao", "Yasuo", "Yone", "Yorick", "Yuumi",
            "Zac", "Zed", "Ziggs", "Zilean", "Zoe", "Zyra"
        };


        // Gets all TextBlock items in a Grid
        private List<TextBlock> GetAllTextBlocks()
        {
            List<TextBlock> allBlocks = new List<TextBlock>();
            foreach (UIElement child in MainGrid.Children)
            {
                if (child is TextBlock block)
                {
                    allBlocks.Add(block);
                }
            }
            return allBlocks;
        }

        // Called when the user types something
        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            List<string> suitableItems = new List<string>();
            string[] splitText = sender.Text.ToLower().Split(" ");
            foreach (string champ in autosrc)
            {
                bool found = splitText.All((key) =>
                {
                    return champ.ToLower().Contains(key);
                });
                if (found)
                {
                    suitableItems.Add(champ);
                }
            }
            if (suitableItems.Count == 0)
            {
                suitableItems.Add("No results found");
            }
            sender.ItemsSource = suitableItems;

        }

        // Called when the user clicks on a suggestion
        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        // Set the level and update stats
        private void SetLevel()
        {
            Level.Text = "Level: " + LevelSlider.Value;
            UpdateStats();
        }

        // Method to update stats
        private void UpdateStats()
        {
            //Change HP Stats
            Res1.Text = Math.Round(double.Parse(stats[0], CultureInfo.InvariantCulture) + (double.Parse(stats[2]) * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 2, MidpointRounding.ToEven).ToString();
            Res2.Text = Math.Round(double.Parse(stats[1], CultureInfo.InvariantCulture) + (double.Parse(stats[3]) * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 2, MidpointRounding.ToEven).ToString();
            UpdateColors(Res1, Res2);

            //Change HP Regen Stats
            Res5.Text = Math.Round(double.Parse(stats[4], CultureInfo.InvariantCulture) + (double.Parse(stats[6]) / 5 * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 1, MidpointRounding.ToEven).ToString();
            Res6.Text = Math.Round(double.Parse(stats[5], CultureInfo.InvariantCulture) + (double.Parse(stats[7]) / 5 * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 1, MidpointRounding.ToEven).ToString();

            //Change Mana Stats
            Res11.Text = Math.Round(double.Parse(stats[10], CultureInfo.InvariantCulture) + (double.Parse(stats[12]) * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 2, MidpointRounding.ToEven).ToString();
            Res12.Text = Math.Round(double.Parse(stats[11], CultureInfo.InvariantCulture) + (double.Parse(stats[13]) * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 2, MidpointRounding.ToEven).ToString();
            UpdateColors(Res11, Res12);

            //Change Mana Regen Stats
            Res15.Text = Math.Round(double.Parse(stats[14], CultureInfo.InvariantCulture) + (double.Parse(stats[16]) / 5 * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 1, MidpointRounding.ToEven).ToString();
            Res16.Text = Math.Round(double.Parse(stats[15], CultureInfo.InvariantCulture) + (double.Parse(stats[17]) / 5 * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 1, MidpointRounding.ToEven).ToString();

            //Change Attack Stats
            Res19.Text = Math.Round(double.Parse(stats[18], CultureInfo.InvariantCulture) + (double.Parse(stats[20]) * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 2, MidpointRounding.ToEven).ToString();
            Res20.Text = Math.Round(double.Parse(stats[19], CultureInfo.InvariantCulture) + (double.Parse(stats[21]) * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 2, MidpointRounding.ToEven).ToString();

            UpdateColors(Res19, Res20);

            //Change Attack Stats
            string result1 = Math.Round(0 + (Convert.ToDouble(stats[26].TrimEnd('%')) * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 3, MidpointRounding.ToEven).ToString();
            string result2 = Math.Round(0 + (Convert.ToDouble(stats[27].TrimEnd('%')) * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 3, MidpointRounding.ToEven).ToString();
            Res25.Text = Math.Round(double.Parse(stats[24], CultureInfo.InvariantCulture) * (1 + (double.Parse(result1) / 100)), 3, MidpointRounding.ToEven).ToString();
            Res26.Text = Math.Round(double.Parse(stats[25], CultureInfo.InvariantCulture) * (1 + (double.Parse(result2) / 100)), 3, MidpointRounding.ToEven).ToString();

            UpdateColors(Res25, Res26);

            //Change Armor Stats
            Res29.Text = Math.Round(double.Parse(stats[28], CultureInfo.InvariantCulture) + (double.Parse(stats[30]) * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 2, MidpointRounding.ToEven).ToString();
            Res30.Text = Math.Round(double.Parse(stats[29], CultureInfo.InvariantCulture) + (double.Parse(stats[31]) * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 2, MidpointRounding.ToEven).ToString();

            UpdateColors(Res29, Res30);

            //Change Magic Resistance Stats
            Res33.Text = Math.Round(double.Parse(stats[32], CultureInfo.InvariantCulture) + (double.Parse(stats[34]) * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 2, MidpointRounding.ToEven).ToString();
            Res34.Text = Math.Round(double.Parse(stats[33], CultureInfo.InvariantCulture) + (double.Parse(stats[35]) * (LevelSlider.Value - 1) * (0.7025 + (0.0175 * (LevelSlider.Value - 1)))), 2, MidpointRounding.ToEven).ToString();
            UpdateColors(Res33, Res34);
        }

        // Called when the user stops sliding the slider
        private void Slider_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            SetLevel();
        }

        // Updates stats colors
        private void UpdateColors(TextBlock tb1, TextBlock tb2)
        {
            if (Convert.ToDouble(tb1.Text) > Convert.ToDouble(tb2.Text))
            {
                tb1.Foreground = new SolidColorBrush(Colors.Green);
                tb2.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (Convert.ToDouble(tb1.Text) == Convert.ToDouble(tb2.Text))
            {
                tb1.Foreground = new SolidColorBrush(Colors.Blue);
                tb2.Foreground = new SolidColorBrush(Colors.Blue);
            }
            else
            {
                tb2.Foreground = new SolidColorBrush(Colors.Green);
                tb1.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        /* Updates stats colors directly with stats, useful for setting
         * colors for the first time */
        private void UpdateColors(double stat1, double stat2, TextBlock tb1, TextBlock tb2)
        {
            if (stat1 > stat2)
            {
                tb1.Foreground = new SolidColorBrush(Colors.Green);
                tb2.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (stat1 == stat2)
            {
                tb1.Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 15, 82, 186));
                tb2.Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 15, 82, 186));
            }
            else
            {
                tb2.Foreground = new SolidColorBrush(Colors.Green);
                tb1.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        // Get all textblocks that contain advanced stats
        private List<TextBlock> GetAdvancedStats()
        {
            List<TextBlock> advancedStats = new List<TextBlock>
            {
                HP2, HP4, Mana3, Mana5, Attack2, Attack5, Armor2, Magic2, Res3, Res4,
                Res7, Res8, Res13, Res14, Res17, Res18, Res21, Res22, Res27, Res28,
                Res31, Res32, Res35, Res36, AdvancedTitle
            };
            return advancedStats;
        }

        public MainPage()
        {
            InitializeComponent();
            // Set font to Segoe UI Variable on Windows 11
            string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong version = ulong.Parse(deviceFamilyVersion);
            ulong build = (version & 0x00000000FFFF0000L) >> 16;
            if (build >= 22000)
            {
                foreach (UIElement child in MainGrid.Children)
                {
                    if (child is TextBlock block)
                    {
                        block.FontFamily = new FontFamily("Segoe UI Variable Display");
                    }
                    if (child is AutoSuggestBox box)
                    {
                        box.FontFamily = new FontFamily("Segoe UI Variable Display");
                    }
                }
            }
            // Set grid height to hide extra space
            MainGrid.Height = 1280;
            // Hide stats and champ names
            foreach (TextBlock textBlock in GetAllTextBlocks())
            {
                if (textBlock.Name.StartsWith("Res") || textBlock.Name.EndsWith("Name"))
                {
                    textBlock.Visibility = Visibility.Collapsed;
                }
            }
            // Hide advanced stats
            foreach (TextBlock t in GetAdvancedStats())
            {
                t.Visibility = Visibility.Collapsed;
            }
            // Hide patch notes link
            PatchNotes.Visibility = Visibility.Collapsed;
            // Disable advanced stats button
            AdvancedBtn.IsEnabled = false;
            // Display app version
            Version.Text += GetVersion();
            Level.Text = "Level: 1";
            LevelSlider.Value = 1;
            GetPatch();
        }

        // Gets latest patch number and displays it
        public async void GetPatch()
        {
            HttpResponseMessage response = await client.GetAsync(new Uri("https://ddragon.leagueoflegends.com/api/versions.json"));
            string jsonString = await response.Content.ReadAsStringAsync();
            latestPatch = jsonString.Split(',')[0].TrimStart('[').TrimStart('"').TrimEnd('"');
            string trimmedLatestPatch = latestPatch.Substring(0, (latestPatch.Length - 2));
            string majorVer = trimmedLatestPatch.Substring(0, 2);
            string minorVer = trimmedLatestPatch.Substring(3);
            Patch.Text = "Game Patch: " + trimmedLatestPatch;
            Uri patchLink = new Uri($"https://na.leagueoflegends.com/en-us/news/game-updates/patch-{majorVer}-{minorVer}-notes/");
            PNHyperlink.NavigateUri = patchLink;
            PatchNotes.Visibility = Visibility.Visible;
        }

        // Returns app version
        public static string GetVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;
            return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }

        // Called when clicking on the advanced stats button
        private void AdvancedBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AdvancedBtn.Content.ToString().StartsWith("Show"))
            {
                foreach (TextBlock t in GetAdvancedStats())
                {
                    t.Visibility = Visibility.Visible;
                }
                AdvancedBtn.Content = "Hide advanced stats";
                MainGrid.Height = 1620;
            }
            else
            {
                foreach (TextBlock t in GetAdvancedStats())
                {
                    t.Visibility = Visibility.Collapsed;
                }
                AdvancedBtn.Content = "Show advanced stats";
                MainGrid.Height = 1280;
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            // Store champion names to display them later
            ch1 = FirstChampBox.Text;
            ch2 = SecondChampBox.Text;
            champ1 = ch1.ToLower().Contains("mundo") ? "DrMundo" : (ch1.ToLower().Contains("sol") ? "AurelionSol" : (ch1.ToLower().Contains("jarvan") ? "JarvanIV" : ((!ch1.ToLower().Equals("kai'sa") && !ch1.ToLower().Equals("kai sa")) ? (ch1.ToLower().Contains("kha") ? "Khazix" : (ch1.ToLower().Contains("kog") ? "KogMaw" : ((!ch1.ToLower().Equals("leesin") && !ch1.ToLower().Equals("lee sin")) ? (ch1.ToLower().Contains("master") ? "MasterYi" : (ch1.ToLower().Contains("miss") ? "MissFortune" : (ch1.ToLower().Contains("wukong") ? "MonkeyKing" : ((!ch1.ToLower().Equals("rek'sai") && !ch1.ToLower().Equals("reksai") && !ch1.ToLower().Equals("rek sai")) ? (ch1.ToLower().Contains("tahm") ? "TahmKench" : (ch1.ToLower().Contains("twisted") ? "TwistedFate" : ((ch1.ToLower().Equals("vel'koz") || ch1.ToLower().Equals("vel koz")) ? "Velkoz" : (ch1.ToLower().Contains("xin") ? "XinZhao" : (ch1.ToLower().Contains("pasquetto") ? "Shaco" : ((!ch1.Contains(" ")) ? (char.ToUpper(ch1[0]) + ch1.Substring(1).ToLower()) : (char.ToUpper(ch1[0]) + ch1.Substring(1).ToLower().TrimEnd(' ')))))))) : "RekSai")))) : "LeeSin"))) : "Kaisa")));
            champ2 = ch2.ToLower().Contains("mundo") ? "DrMundo" : (ch2.ToLower().Contains("sol") ? "AurelionSol" : (ch2.ToLower().Contains("jarvan") ? "JarvanIV" : ((!ch2.ToLower().Equals("kai'sa") && !ch2.ToLower().Equals("kai sa")) ? (ch2.ToLower().Contains("kha") ? "Khazix" : (ch2.ToLower().Contains("kog") ? "KogMaw" : ((!ch2.ToLower().Equals("leesin") && !ch2.ToLower().Equals("lee sin")) ? (ch2.ToLower().Contains("master") ? "MasterYi" : (ch2.ToLower().Contains("miss") ? "MissFortune" : (ch2.ToLower().Contains("wukong") ? "MonkeyKing" : ((!ch2.ToLower().Equals("rek'sai") && !ch2.ToLower().Equals("reksai") && !ch2.ToLower().Equals("rek sai")) ? (ch2.ToLower().Contains("tahm") ? "TahmKench" : (ch2.ToLower().Contains("twisted") ? "TwistedFate" : ((ch2.ToLower().Equals("vel'koz") || ch2.ToLower().Equals("vel koz")) ? "Velkoz" : (ch2.ToLower().Contains("xin") ? "XinZhao" : (ch2.ToLower().Contains("pasquetto") ? "Shaco" : ((!ch2.Contains(" ")) ? (char.ToUpper(ch2[0]) + ch2.Substring(1).ToLower()) : (char.ToUpper(ch2[0]) + ch2.Substring(1).ToLower().TrimEnd(' ')))))))) : "RekSai")))) : "LeeSin"))) : "Kaisa")));
            DisplayStats();
        }

        // Gets stats and displays champ names, images and stats
        public async void DisplayStats()
        {
            // Get data from LOL servers
            HttpResponseMessage response1 = await client.GetAsync(new Uri($@"http://ddragon.leagueoflegends.com/cdn/{latestPatch}/data/en_US/champion/{champ1}.json"));
            string json1 = await response1.Content.ReadAsStringAsync();
            Champion1.Root requestChampion1 = JsonConvert.DeserializeObject<Champion1.Root>(json1);
            List<Champion1.Spell> spells = requestChampion1?.data[champ1].spells;

            HttpResponseMessage response2 = await client.GetAsync(new Uri($@"http://ddragon.leagueoflegends.com/cdn/{latestPatch}/data/en_US/champion/{champ2}.json"));
            string json2 = await response2.Content.ReadAsStringAsync();
            Champion2.Root requestChampion2 = JsonConvert.DeserializeObject<Champion2.Root>(json2);
            List<Champion2.Spell> spells2 = requestChampion2?.data[champ2].spells;

            if (requestChampion2 != null && requestChampion1 != null && requestChampion1.data.TryGetValue(champ1, out var champion1) && requestChampion2.data.TryGetValue(champ2, out var champion2))
            {
                // Q cooldown
                Champion1.Spell spell = spells[0];
                Champion2.Spell spell2 = spells2[0];
                // W cooldown
                Champion1.Spell spell3 = spells[1];
                Champion2.Spell spell4 = spells2[1];
                // E cooldown
                Champion1.Spell spell5 = spells[2];
                Champion2.Spell spell6 = spells2[2];
                // R cooldown
                Champion1.Spell spell7 = spells[3];
                Champion2.Spell spell8 = spells2[3];

                /* Store stats for both champs in a string array. I died inside to do this
                 * in the original ChampionComparatorGUI */
                stats[0] = champion1.stats.hp.ToString();
                stats[1] = champion2.stats.hp.ToString();
                stats[2] = $"+{champion1.stats.hpperlevel}";
                stats[3] = $"+{champion2.stats.hpperlevel}";
                stats[4] = (champion1.stats.hpregen / 5).ToString();
                stats[5] = (champion2.stats.hpregen / 5).ToString();
                stats[6] = $"+{champion1.stats.hpregenperlevel / 5}";
                stats[7] = $"+{champion2.stats.hpregenperlevel / 5}";
                stats[8] = champion1.partype;
                stats[9] = champion2.partype;
                stats[10] = champion1.stats.mp.ToString();
                stats[11] = champion2.stats.mp.ToString();
                stats[12] = $"+{champion1.stats.mpperlevel}";
                stats[13] = $"+{champion2.stats.mpperlevel}";
                stats[14] = (champion1.stats.mpregen / 5).ToString();
                stats[15] = (champion2.stats.mpregen / 5).ToString();
                stats[16] = $"+{champion1.stats.mpregenperlevel / 5}";
                stats[17] = $"+{champion2.stats.mpregenperlevel / 5}";
                stats[18] = champion1.stats.attackdamage.ToString();
                stats[19] = champion2.stats.attackdamage.ToString();
                stats[20] = $"+{champion1.stats.attackdamageperlevel}";
                stats[21] = $"+{champion2.stats.attackdamageperlevel}";
                stats[22] = champion1.stats.attackrange.ToString();
                stats[23] = champion2.stats.attackrange.ToString();
                stats[24] = champion1.stats.attackspeed.ToString();
                stats[25] = champion2.stats.attackspeed.ToString();
                stats[26] = $"{champion1.stats.attackspeedperlevel}%";
                stats[27] = $"{champion2.stats.attackspeedperlevel}%";
                stats[28] = champion1.stats.armor.ToString();
                stats[29] = champion2.stats.armor.ToString();
                stats[30] = $"+{champion1.stats.armorperlevel}";
                stats[31] = $"+{champion2.stats.armorperlevel}";
                stats[32] = champion1.stats.spellblock.ToString();
                stats[33] = champion2.stats.spellblock.ToString();
                stats[34] = $"+{champion1.stats.spellblockperlevel}";
                stats[35] = $"+{champion2.stats.spellblockperlevel}";
                stats[36] = champion1.stats.movespeed.ToString();
                stats[37] = champion2.stats.movespeed.ToString();
                stats[38] = spell.cooldownBurn;
                stats[39] = spell2.cooldownBurn;
                stats[40] = spell3.cooldownBurn;
                stats[41] = spell4.cooldownBurn;
                stats[42] = spell5.cooldownBurn;
                stats[43] = spell6.cooldownBurn;
                stats[44] = spell7.cooldownBurn;
                stats[45] = spell8.cooldownBurn;

                /* Set the colors of the most important stats. Netu died inside to do this,
                 * but Ledda made it more simple */
                UpdateColors(champion1.stats.hp, champion2.stats.hp, Res1, Res2);

                if (champion1.partype == champion2.partype)
                {
                    UpdateColors(champion1.stats.mp, champion2.stats.mp, Res11, Res12);
                }
                else
                {
                    Res11.Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 119, 123, 126));
                    Res12.Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 119, 123, 126));
                }

                UpdateColors(champion1.stats.attackdamage, champion2.stats.attackdamage, Res19, Res20);

                UpdateColors(champion1.stats.attackrange, champion2.stats.attackrange, Res23, Res24);

                UpdateColors(champion1.stats.attackspeed, champion2.stats.attackspeed, Res25, Res26);

                UpdateColors(champion1.stats.armor, champion2.stats.armor, Res29, Res30);

                UpdateColors(champion1.stats.spellblock, champion2.stats.spellblock, Res33, Res34);

                UpdateColors(champion1.stats.movespeed, champion2.stats.movespeed, Res37, Res38);

                // Create an array to store stats labels and champ names
                TextBlock[] blocks = new TextBlock[46];

                /* Set every stat label and show stats, champ names and pics
                 * (except advanced stats) */
                int i = 0;
                foreach (TextBlock textBlock in GetAllTextBlocks())
                {
                    if (textBlock.Name.StartsWith("Res"))
                    {
                        blocks[i] = textBlock;
                        blocks[i].Visibility = Visibility.Visible;
                        blocks[i].Text = stats[i];
                        i++;
                    }
                    if (textBlock.Name.EndsWith("Name"))
                    {
                        textBlock.Visibility = Visibility.Visible;
                    }
                }
                foreach (TextBlock textBlock in GetAdvancedStats())
                {
                    textBlock.Visibility = Visibility.Collapsed;
                }
                FirstChampName.Text = champion1.name;
                SecondChampName.Text = champion2.name;
                Uri champion1Path = new Uri($@"ms-appx:///Assets/Champions/{ch1}.png");
                Uri champion2Path = new Uri($@"ms-appx:///Assets/Champions/{ch2}.png");
                if (File.Exists(champion1Path.ToString()))
                {
                    FirstChampImage.Source = new BitmapImage(champion1Path);
                }
                else
                {
                    FirstChampImage.Source = new BitmapImage(new Uri($@"http://ddragon.leagueoflegends.com/cdn/{latestPatch}/img/champion/{champ1}.png"));
                }
                if (File.Exists(champion2Path.ToString()))
                {
                    SecondChampImage.Source = new BitmapImage(champion2Path);
                }
                else
                {
                    SecondChampImage.Source = new BitmapImage(new Uri($@"http://ddragon.leagueoflegends.com/cdn/{latestPatch}/img/champion/{champ2}.png"));
                }

                // Clean AutoSuggestBoxes
                FirstChampBox.Text = "";
                SecondChampBox.Text = "";

                // Enable advanced stats button
                AdvancedBtn.IsEnabled = true;
            }
        }

        // Easter eggs
        private async void TextBlock_Clicked(object sender, TappedRoutedEventArgs e)
        {
            // The URI to launch
            Uri uri = new Uri(@"https://raw.githubusercontent.com/LeddaZ/LeddaZ.github.io/master/files/heh.gif");

            // Launch the URI
            bool success = await Windows.System.Launcher.LaunchUriAsync(uri);

            if (success)
            {
                Movement.Foreground = new SolidColorBrush(Colors.Magenta);
                Movement.Text = "you got rickrolled lmao";
            }
        }
    }
}
