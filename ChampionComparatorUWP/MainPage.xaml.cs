using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        private readonly HttpClient client = new HttpClient();

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

        public MainPage()
        {
            InitializeComponent();
            // Hide stats and champ names
            foreach (TextBlock textBlock in GetAllTextBlocks())
            {
                if (textBlock.Name.StartsWith("Res") || textBlock.Name.EndsWith("Name"))
                {
                    textBlock.Visibility = Visibility.Collapsed;
                }
            }
            // Display app version
            Version.Text += GetVersion();
            GetPatch();
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            // Store champion names to display them later
            ch1 = FirstChampTxt.Text;
            ch2 = SecondChampTxt.Text;
            champ1 = ch1.ToLower().Contains("mundo") ? "DrMundo" : (ch1.ToLower().Contains("sol") ? "AurelionSol" : (ch1.ToLower().Contains("jarvan") ? "JarvanIV" : ((!ch1.ToLower().Equals("kai'sa") && !ch1.ToLower().Equals("kai sa")) ? (ch1.ToLower().Contains("kha") ? "Khazix" : (ch1.ToLower().Contains("kog") ? "KogMaw" : ((!ch1.ToLower().Equals("leesin") && !ch1.ToLower().Equals("lee sin")) ? (ch1.ToLower().Contains("master") ? "MasterYi" : (ch1.ToLower().Contains("miss") ? "MissFortune" : (ch1.ToLower().Contains("wukong") ? "MonkeyKing" : ((!ch1.ToLower().Equals("rek'sai") && !ch1.ToLower().Equals("reksai") && !ch1.ToLower().Equals("rek sai")) ? (ch1.ToLower().Contains("tahm") ? "TahmKench" : (ch1.ToLower().Contains("twisted") ? "TwistedFate" : ((ch1.ToLower().Equals("vel'koz") || ch1.ToLower().Equals("vel koz")) ? "Velkoz" : (ch1.ToLower().Contains("xin") ? "XinZhao" : (ch1.ToLower().Contains("pasquetto") ? "Shaco" : ((!ch1.Contains(" ")) ? (char.ToUpper(ch1[0]) + ch1.Substring(1).ToLower()) : (char.ToUpper(ch1[0]) + ch1.Substring(1).ToLower().TrimEnd(' ')))))))) : "RekSai")))) : "LeeSin"))) : "Kaisa")));
            champ2 = ch2.ToLower().Contains("mundo") ? "DrMundo" : (ch2.ToLower().Contains("sol") ? "AurelionSol" : (ch2.ToLower().Contains("jarvan") ? "JarvanIV" : ((!ch2.ToLower().Equals("kai'sa") && !ch2.ToLower().Equals("kai sa")) ? (ch2.ToLower().Contains("kha") ? "Khazix" : (ch2.ToLower().Contains("kog") ? "KogMaw" : ((!ch2.ToLower().Equals("leesin") && !ch2.ToLower().Equals("lee sin")) ? (ch2.ToLower().Contains("master") ? "MasterYi" : (ch2.ToLower().Contains("miss") ? "MissFortune" : (ch2.ToLower().Contains("wukong") ? "MonkeyKing" : ((!ch2.ToLower().Equals("rek'sai") && !ch2.ToLower().Equals("reksai") && !ch2.ToLower().Equals("rek sai")) ? (ch2.ToLower().Contains("tahm") ? "TahmKench" : (ch2.ToLower().Contains("twisted") ? "TwistedFate" : ((ch2.ToLower().Equals("vel'koz") || ch2.ToLower().Equals("vel koz")) ? "Velkoz" : (ch2.ToLower().Contains("xin") ? "XinZhao" : (ch2.ToLower().Contains("pasquetto") ? "Shaco" : ((!ch2.Contains(" ")) ? (char.ToUpper(ch2[0]) + ch2.Substring(1).ToLower()) : (char.ToUpper(ch2[0]) + ch2.Substring(1).ToLower().TrimEnd(' ')))))))) : "RekSai")))) : "LeeSin"))) : "Kaisa")));
            DisplayStats();
        }

        // Returns app version
        public static string GetVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;
            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        // Gets latest patch number and displays it
        public async void GetPatch()
        {
            HttpResponseMessage response = await client.GetAsync(new Uri("https://ddragon.leagueoflegends.com/api/versions.json"));
            string jsonString = await response.Content.ReadAsStringAsync();
            latestPatch = jsonString.Split(',')[0].TrimStart('[').TrimStart('"').TrimEnd('"');
            Patch.Text = "Game Patch: " + latestPatch;
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
                string[] stats = new string[46];
                stats[0] = champion1.stats.hp.ToString();
                stats[1] = champion2.stats.hp.ToString();
                stats[2] = $"+{champion1.stats.hpperlevel}";
                stats[3] = $"+{champion2.stats.hpperlevel}";
                stats[4] = champion1.stats.hpregen.ToString();
                stats[5] = champion2.stats.hpregen.ToString();
                stats[6] = $"+{champion1.stats.hpregenperlevel}";
                stats[7] = $"+{champion2.stats.hpregenperlevel}";
                stats[8] = champion1.partype;
                stats[9] = champion2.partype;
                stats[10] = champion1.stats.mp.ToString();
                stats[11] = champion2.stats.mp.ToString();
                stats[12] = $"+{champion1.stats.mpperlevel}";
                stats[13] = $"+{champion2.stats.mpperlevel}";
                stats[14] = champion1.stats.mpregen.ToString();
                stats[15] = champion2.stats.mpregen.ToString();
                stats[16] = $"+{champion1.stats.mpregenperlevel}";
                stats[17] = $"+{champion2.stats.mpregenperlevel}";
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

                // Set the colors of the most important stats. Netu died inside to do this.
                if (champion1.stats.hp > champion2.stats.hp)
                {
                    Res1.Foreground = new SolidColorBrush(Colors.Green);
                    Res2.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (champion1.stats.hp == champion2.stats.hp)
                {
                    Res1.Foreground = new SolidColorBrush(Colors.Blue);
                    Res2.Foreground = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    Res2.Foreground = new SolidColorBrush(Colors.Green);
                    Res1.Foreground = new SolidColorBrush(Colors.Red);
                }

                if (champion1.partype == champion2.partype)
                {
                    if (champion1.stats.mp > champion2.stats.mp)
                    {
                        Res11.Foreground = new SolidColorBrush(Colors.Green);
                        Res12.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else if (champion1.stats.mp == champion2.stats.mp)
                    {
                        Res11.Foreground = new SolidColorBrush(Colors.Blue);
                        Res12.Foreground = new SolidColorBrush(Colors.Blue);
                    }
                    else
                    {
                        Res12.Foreground = new SolidColorBrush(Colors.Green);
                        Res11.Foreground = new SolidColorBrush(Colors.Red);
                    }
                }
                else
                {
                    Res11.Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 167, 146, 221));
                    Res12.Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 167, 146, 221));
                }

                if (champion1.stats.attackdamage > champion2.stats.attackdamage)
                {
                    Res19.Foreground = new SolidColorBrush(Colors.Green);
                    Res20.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (champion1.stats.attackdamage == champion2.stats.attackdamage)
                {
                    Res19.Foreground = new SolidColorBrush(Colors.Blue);
                    Res20.Foreground = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    Res20.Foreground = new SolidColorBrush(Colors.Green);
                    Res19.Foreground = new SolidColorBrush(Colors.Red);
                }

                if (champion1.stats.attackrange > champion2.stats.attackrange)
                {
                    Res23.Foreground = new SolidColorBrush(Colors.Green);
                    Res24.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (champion1.stats.attackrange == champion2.stats.attackrange)
                {
                    Res23.Foreground = new SolidColorBrush(Colors.Blue);
                    Res24.Foreground = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    Res24.Foreground = new SolidColorBrush(Colors.Green);
                    Res23.Foreground = new SolidColorBrush(Colors.Red);
                }

                if (champion1.stats.attackspeed > champion2.stats.attackspeed)
                {
                    Res25.Foreground = new SolidColorBrush(Colors.Green);
                    Res26.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (champion1.stats.attackspeed == champion2.stats.attackspeed)
                {
                    Res25.Foreground = new SolidColorBrush(Colors.Blue);
                    Res26.Foreground = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    Res26.Foreground = new SolidColorBrush(Colors.Green);
                    Res25.Foreground = new SolidColorBrush(Colors.Red);
                }

                if (champion1.stats.armor > champion2.stats.armor)
                {
                    Res29.Foreground = new SolidColorBrush(Colors.Green);
                    Res30.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (champion1.stats.armor == champion2.stats.armor)
                {
                    Res29.Foreground = new SolidColorBrush(Colors.Blue);
                    Res30.Foreground = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    Res30.Foreground = new SolidColorBrush(Colors.Green);
                    Res29.Foreground = new SolidColorBrush(Colors.Red);
                }

                if (champion1.stats.spellblock > champion2.stats.spellblock)
                {
                    Res33.Foreground = new SolidColorBrush(Colors.Green);
                    Res34.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (champion1.stats.spellblock == champion2.stats.spellblock)
                {
                    Res33.Foreground = new SolidColorBrush(Colors.Blue);
                    Res34.Foreground = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    Res34.Foreground = new SolidColorBrush(Colors.Green);
                    Res33.Foreground = new SolidColorBrush(Colors.Red);
                }

                if (champion1.stats.movespeed > champion2.stats.movespeed)
                {
                    Res37.Foreground = new SolidColorBrush(Colors.Green);
                    Res38.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (champion1.stats.movespeed == champion2.stats.movespeed)
                {
                    Res37.Foreground = new SolidColorBrush(Colors.Blue);
                    Res38.Foreground = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    Res38.Foreground = new SolidColorBrush(Colors.Green);
                    Res37.Foreground = new SolidColorBrush(Colors.Red);
                }

                // Create an array to store stats labels and champ names
                TextBlock[] blocks = new TextBlock[46];

                // Set every stat label and show stats, champ names and pics
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
                FirstChampName.Text = champion1.name;
                SecondChampName.Text = champion2.name;
                FirstChampImage.Source = new BitmapImage(new Uri($@"ms-appx:///Assets/Champions/{ch1}.png"));
                SecondChampImage.Source = new BitmapImage(new Uri($@"ms-appx:///Assets/Champions/{ch2}.png"));

                // Clean textboxes
                FirstChampTxt.Text = "";
                SecondChampTxt.Text = "";
            }
        }
    }
}
