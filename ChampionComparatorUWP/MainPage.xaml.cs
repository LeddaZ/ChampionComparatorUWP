﻿using Newtonsoft.Json;
using Octokit;
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

namespace ChampionComparatorUWP
{
    public sealed partial class MainPage : Windows.UI.Xaml.Controls.Page
    {
        public string latestPatch;
        private string champ1, champ2, ch1, ch2, winVer, winBuild;
        private Version latestVersion;
        private readonly string[] stats = new string[46];
        private readonly HttpClient client = new HttpClient();
        private int clickCount = 0;
        // Acrylic brushes
        private readonly AcrylicBrush darkDialogBrush = new AcrylicBrush()
        {
            BackgroundSource = AcrylicBackgroundSource.HostBackdrop,
            Opacity = 0.8,
            TintOpacity = 0.1
        };
        private readonly AcrylicBrush lightDialogBrush = new AcrylicBrush()
        {
            BackgroundSource = AcrylicBackgroundSource.HostBackdrop,
            Opacity = 0.8,
            TintOpacity = 1
        };
        private readonly AcrylicBrush backgroundBrush = new AcrylicBrush()
        {
            BackgroundSource = AcrylicBackgroundSource.HostBackdrop,
            Opacity = 0.3,
            TintOpacity = 0.45
        };
        // Other brushes
        private readonly SolidColorBrush lightGreen = new SolidColorBrush(Colors.Green);
        private readonly SolidColorBrush darkGreen = new SolidColorBrush(ColorHelper.FromArgb(255, 114, 245, 71));
        private readonly SolidColorBrush lightRed = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush darkRed = new SolidColorBrush(ColorHelper.FromArgb(255, 247, 75, 69));
        private readonly SolidColorBrush lightBlue = new SolidColorBrush(ColorHelper.FromArgb(255, 15, 82, 186));
        private readonly SolidColorBrush darkBlue = new SolidColorBrush(ColorHelper.FromArgb(255, 63, 130, 232));

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

        // Start of the big mess
        public MainPage()
        {
            InitializeComponent();
            // Acrylic background
            InitialPage.Background = backgroundBrush;
            GetWinVer();
            // Set font to Segoe UI Variable on Windows 11
            if (winVer.Equals("11"))
            {
                InitialPage.FontFamily = new FontFamily("Segoe UI Variable Display");
            }
            // Set grid height to hide extra space
            MainGrid.Height = 1280;
            // Hide stats, champ names and level text
            foreach (TextBlock textBlock in GetAllTextBlocks())
            {
                if (textBlock.Name.StartsWith("Res") || textBlock.Name.EndsWith("Name") || textBlock.Name.Equals("Level"))
                {
                    textBlock.Visibility = Visibility.Collapsed;
                }
            }
            // Hide advanced stats
            foreach (TextBlock t in GetAdvancedStats())
            {
                t.Visibility = Visibility.Collapsed;
            }
            // Hide level slider
            LevelSlider.Visibility = Visibility.Collapsed;
            // Hide patch notes link
            PatchNotes.Visibility = Visibility.Collapsed;
            // Disable advanced stats button
            AdvancedBtn.IsEnabled = false;
            // Display app version
            Version.Text += GetVersion();
            Level.Text = "Level: 1";
            LevelSlider.Value = 1;
            GetPatch();
            CheckUpdates();
        }

        // Get current system theme
        private string GetSystemTheme()
        {
            Windows.UI.ViewManagement.UISettings uiSettings = new Windows.UI.ViewManagement.UISettings();
            Color color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background);
            return color.ToString();
        }

        // Check for updates
        private async System.Threading.Tasks.Task CheckUpdates()
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue("doodoo"));
            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("LeddaZ", "ChampionComparatorUWP");

            //Setup versions
            latestVersion = new Version(releases[0].Name);
            Version localVersion = new Version(GetVersion());

            //Compare the Versions
            int versionComparison = localVersion.CompareTo(latestVersion);
            if (versionComparison < 0)
            {
                ContentDialog updateDialog = new ContentDialog
                {
                    Title = "Update available",
                    Content = $"Current version: {GetVersion()}\nLatest version: {latestVersion}",
                    PrimaryButtonText = "Update",
                    CloseButtonText = "Ignore"
                };
                updateDialog.Background = GetSystemTheme().Equals("#FFFFFFFF") ? lightDialogBrush : darkDialogBrush;
                ContentDialogResult result = await updateDialog.ShowAsync();

                // Go to GitHub if the user clicks the "Update" button
                if (result == ContentDialogResult.Primary)
                {
                    Uri uri = new Uri(@"https://github.com/LeddaZ/ChampionComparatorUWP/releases/latest");
                    _ = await Windows.System.Launcher.LaunchUriAsync(uri);
                }
            }
        }

        // Get Windows version and build
        private void GetWinVer()
        {
            // Build
            string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong version = ulong.Parse(deviceFamilyVersion);
            ulong build = (version & 0x00000000FFFF0000L) >> 16;
            ulong revision = (version & 0x000000000000FFFFL);
            winBuild = build.ToString() + "." + revision.ToString();
            // Version
            winVer = build >= 21996 ? "11" : "10";
        }

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
                tb1.Foreground = GetSystemTheme().Equals("#FFFFFFFF") ? lightGreen : darkGreen;
                tb2.Foreground = GetSystemTheme().Equals("#FFFFFFFF") ? lightRed : darkRed;
            }
            else if (Convert.ToDouble(tb1.Text) == Convert.ToDouble(tb2.Text))
            {
                tb1.Foreground = GetSystemTheme().Equals("#FFFFFFFF") ? lightBlue : darkBlue;
                tb2.Foreground = GetSystemTheme().Equals("#FFFFFFFF") ? lightBlue : darkBlue;
            }
            else
            {
                tb2.Foreground = GetSystemTheme().Equals("#FFFFFFFF") ? lightGreen : darkGreen;
                tb1.Foreground = GetSystemTheme().Equals("#FFFFFFFF") ? lightRed : darkRed;
            }
        }

        /* Updates stats colors directly with stats, useful for setting
         * colors for the first time */
        private void UpdateColors(double stat1, double stat2, TextBlock tb1, TextBlock tb2)
        {
            if (stat1 > stat2)
            {
                tb1.Foreground = GetSystemTheme().Equals("#FFFFFFFF") ? lightGreen : darkGreen;
                tb2.Foreground = GetSystemTheme().Equals("#FFFFFFFF") ? lightRed : darkRed;
            }
            else if (stat1 == stat2)
            {
                tb1.Foreground = GetSystemTheme().Equals("#FFFFFFFF") ? lightBlue : darkBlue;
                tb2.Foreground = GetSystemTheme().Equals("#FFFFFFFF") ? lightBlue : darkBlue;
            }
            else
            {
                tb2.Foreground = GetSystemTheme().Equals("#FFFFFFFF") ? lightGreen : darkGreen;
                tb1.Foreground = GetSystemTheme().Equals("#FFFFFFFF") ? lightRed : darkRed;
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
        public string GetVersion()
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

        // Called when the Confirm button is clicked
        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            // Increase click count
            clickCount++;

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
            List<Champion1.Spell> spells = requestChampion1?.Data[champ1].Spells;

            HttpResponseMessage response2 = await client.GetAsync(new Uri($@"http://ddragon.leagueoflegends.com/cdn/{latestPatch}/data/en_US/champion/{champ2}.json"));
            string json2 = await response2.Content.ReadAsStringAsync();
            Champion2.Root requestChampion2 = JsonConvert.DeserializeObject<Champion2.Root>(json2);
            List<Champion2.Spell> spells2 = requestChampion2?.Data[champ2].Spells;

            if (requestChampion2 != null && requestChampion1 != null && requestChampion1.Data.TryGetValue(champ1, out Champion1.Champion champion1) && requestChampion2.Data.TryGetValue(champ2, out Champion2.Champion champion2))
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
                stats[0] = champion1.Stats.Hp.ToString();
                stats[1] = champion2.Stats.Hp.ToString();
                stats[2] = $"+{champion1.Stats.HpPerLevel}";
                stats[3] = $"+{champion1.Stats.HpPerLevel}";
                stats[4] = (champion1.Stats.HpRegen / 5).ToString();
                stats[5] = (champion2.Stats.HpRegen / 5).ToString();
                stats[6] = $"+{champion1.Stats.HpRegenPerLevel / 5}";
                stats[7] = $"+{champion2.Stats.HpRegenPerLevel / 5}";
                stats[8] = champion1.ParType;
                stats[9] = champion2.ParType;
                stats[10] = champion1.Stats.Mp.ToString();
                stats[11] = champion2.Stats.Mp.ToString();
                stats[12] = $"+{champion1.Stats.MpPerLevel}";
                stats[13] = $"+{champion2.Stats.MpPerLevel}";
                stats[14] = (champion1.Stats.MpRegen / 5).ToString();
                stats[15] = (champion2.Stats.MpRegen / 5).ToString();
                stats[16] = $"+{champion1.Stats.MpRegenPerLevel / 5}";
                stats[17] = $"+{champion2.Stats.MpRegenPerLevel / 5}";
                stats[18] = champion1.Stats.AttackDamage.ToString();
                stats[19] = champion2.Stats.AttackDamage.ToString();
                stats[20] = $"+{champion1.Stats.AttackDamagePerLevel}";
                stats[21] = $"+{champion2.Stats.AttackDamagePerLevel}";
                stats[22] = champion1.Stats.AttackRange.ToString();
                stats[23] = champion2.Stats.AttackRange.ToString();
                stats[24] = champion1.Stats.AttackSpeed.ToString();
                stats[25] = champion2.Stats.AttackSpeed.ToString();
                stats[26] = $"{champion1.Stats.AttackSpeedPerLevel}%";
                stats[27] = $"{champion2.Stats.AttackSpeedPerLevel}%";
                stats[28] = champion1.Stats.Armor.ToString();
                stats[29] = champion2.Stats.Armor.ToString();
                stats[30] = $"+{champion1.Stats.ArmorPerLevel}";
                stats[31] = $"+{champion2.Stats.ArmorPerLevel}";
                stats[32] = champion1.Stats.SpellBlock.ToString();
                stats[33] = champion2.Stats.SpellBlock.ToString();
                stats[34] = $"+{champion1.Stats.SpellBlockPerLevel}";
                stats[35] = $"+{champion2.Stats.SpellBlockPerLevel}";
                stats[36] = champion1.Stats.MoveSpeed.ToString();
                stats[37] = champion2.Stats.MoveSpeed.ToString();
                stats[38] = spell.CooldownBurn;
                stats[39] = spell2.CooldownBurn;
                stats[40] = spell3.CooldownBurn;
                stats[41] = spell4.CooldownBurn;
                stats[42] = spell5.CooldownBurn;
                stats[43] = spell6.CooldownBurn;
                stats[44] = spell7.CooldownBurn;
                stats[45] = spell8.CooldownBurn;

                /* Set the colors of the most important stats. Netu died inside to do this,
                 * but Ledda made it more simple */
                UpdateColors(champion1.Stats.Hp, champion2.Stats.Hp, Res1, Res2);

                if (champion1.ParType == champion2.ParType)
                {
                    UpdateColors(champion1.Stats.Mp, champion2.Stats.Mp, Res11, Res12);
                }
                else
                {
                    Res11.Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 119, 123, 126));
                    Res12.Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 119, 123, 126));
                }

                UpdateColors(champion1.Stats.AttackDamage, champion2.Stats.AttackDamage, Res19, Res20);
                UpdateColors(champion1.Stats.AttackRange, champion2.Stats.AttackRange, Res23, Res24);
                UpdateColors(champion1.Stats.AttackSpeed, champion2.Stats.AttackSpeed, Res25, Res26);
                UpdateColors(champion1.Stats.Armor, champion2.Stats.Armor, Res29, Res30);
                UpdateColors(champion1.Stats.SpellBlock, champion2.Stats.SpellBlock, Res33, Res34);
                UpdateColors(champion1.Stats.MoveSpeed, champion2.Stats.MoveSpeed, Res37, Res38);

                // Create an array to store stats labels and champ names
                TextBlock[] blocks = new TextBlock[46];

                /* Set every stat label and show stats, champ names and pics
                 * (except advanced stats if the Confirm button has been clicked
                 * for the first time since app launch) */
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
                if (clickCount == 1)
                {
                    foreach (TextBlock textBlock in GetAdvancedStats())
                    {
                        textBlock.Visibility = Visibility.Collapsed;
                    }
                }
                FirstChampName.Text = champion1.Name;
                SecondChampName.Text = champion2.Name;
                Uri champion1Path = new Uri($@"ms-appx:///Assets/Champions/{ch1}.png");
                Uri champion2Path = new Uri($@"ms-appx:///Assets/Champions/{ch2}.png");
                FirstChampImage.Source = File.Exists(champion1Path.ToString())
                    ? new BitmapImage(champion1Path)
                    : new BitmapImage(new Uri($@"http://ddragon.leagueoflegends.com/cdn/{latestPatch}/img/champion/{champ1}.png"));
                SecondChampImage.Source = File.Exists(champion2Path.ToString())
                    ? new BitmapImage(champion2Path)
                    : new BitmapImage(new Uri($@"http://ddragon.leagueoflegends.com/cdn/{latestPatch}/img/champion/{champ2}.png"));

                // Clean AutoSuggestBoxes
                FirstChampBox.Text = "";
                SecondChampBox.Text = "";

                // Enable advanced stats button
                if (!AdvancedBtn.IsEnabled)
                {
                    AdvancedBtn.IsEnabled = true;
                }

                // Show level text and slider
                if (Level.Visibility == Visibility.Collapsed || LevelSlider.Visibility == Visibility.Collapsed)
                {
                    Level.Visibility = Visibility.Visible;
                    LevelSlider.Visibility = Visibility.Visible;
                }
            }
        }

        // Called when the About button is clicked
        private async void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog aboutDialog = new ContentDialog
            {
                Title = "About this app",
                Content = $"UWP app to compare two League of Legends champions. Created from ChampionComparatorGUI, which is based on ChampionComparator, the console-only version of CCGUI (not public).\nRunning on Windows {winVer} build {winBuild}\nHow do I know? Magic.",
                CloseButtonText = "Thank you LeddaZ, very cool!"
            };
            aboutDialog.Background = GetSystemTheme().Equals("#FFFFFFFF") ? lightDialogBrush : darkDialogBrush;
            _ = await aboutDialog.ShowAsync();
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
