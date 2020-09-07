using MahApps.Metro.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlzEx.Theming;

namespace PokemonBWOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        readonly List<Pokemon> Pokedex = new List<Pokemon>();
        readonly string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).Trim();

        public MainWindow()
        {
            InitializeComponent();
            this.Icon = GetImageSource(path + "/sprites/none.gif");
            Pokedex = JsonConvert.DeserializeObject<List<Pokemon>>(File.ReadAllText(@"dex.json"));
            SetupItems();
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                ReadFile();
            }).Start();
            ThemeManager.Current.ChangeTheme(this, "Dark.Green");
        }
        public List<Pokemon> PossibleEvos(Pokemon pokemon)
        {
            var VanillaEvo = Pokedex.Find(x => x.Name.ToLower() == pokemon.VanillaEvo.ToLower());
            var LevelRate = Pokedex.FindAll(x => x.LevelRate == VanillaEvo.LevelRate);
            LevelRate.Remove(VanillaEvo);
            List<Pokemon> Typing = new List<Pokemon>();
            if (VanillaEvo.Id != 285 && VanillaEvo.Id != 426)
            {
                Typing = LevelRate.FindAll(x => x.Type1 == VanillaEvo.Type1 || x.Type1 == VanillaEvo.Type2 || x.Type2 == VanillaEvo.Type1 || x.Type2 == VanillaEvo.Type2);
            }
            else
            {
                Typing = LevelRate;
            }
            Double adjustrangeten = Convert.ToDouble(VanillaEvo.BST) * 0.1;
            Double adjustrangefif = Convert.ToDouble(VanillaEvo.BST) * 0.15;
            Double adjustrangetwen = Convert.ToDouble(VanillaEvo.BST) * 0.2;
            Double adjustrangetwenfi = Convert.ToDouble(VanillaEvo.BST) * 0.25;
            List<Pokemon> Possible = new List<Pokemon>();
            Possible = Typing.FindAll(x => x.BST <= (VanillaEvo.BST + adjustrangeten) && x.BST >= (VanillaEvo.BST - adjustrangeten));
            if (Possible.Count() >= 3)
            {
                return Possible.OrderByDescending(x => x.BST).ToList();
            }
            Possible = Typing.FindAll(x => x.BST <= (VanillaEvo.BST + adjustrangefif) && x.BST >= (VanillaEvo.BST - adjustrangefif));
            if (Possible.Count() >= 3)
            {
                return Possible.OrderByDescending(x => x.BST).ToList();
            }
            Possible = Typing.FindAll(x => x.BST <= (VanillaEvo.BST + adjustrangetwen) && x.BST >= (VanillaEvo.BST - adjustrangetwen));
            if (Possible.Count() >= 3)
            {
                return Possible.OrderByDescending(x => x.BST).ToList();
            }
            Possible = Typing.FindAll(x => x.BST <= (VanillaEvo.BST + adjustrangetwenfi) && x.BST >= (VanillaEvo.BST - adjustrangetwenfi));
            return Possible.OrderByDescending(x => x.BST).ToList();
        }
        public async void SetupItems()
        {
            await enemy.Poke1.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1.Text = "0"));
            await enemy.Poke1Gen.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Gen.Text = "0"));
            await enemy.Poke1Name.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Name.Text = "None"));
            await enemy.Poke1Type1.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Type1.Text = "None"));
            await enemy.Poke1Type2.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Type2.Text = "None"));
            await enemy.Poke1BST.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1BST.Text = "0"));
            await enemy.Poke1LevelRate.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1LevelRate.Text = "None"));
            await enemy.Poke1MoveLevels.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1MoveLevels.Text = "0"));
            await enemy.Poke1Legendary.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Legendary.Text = "False"));
            await enemy.Poke1EvoReq.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1EvoReq.Text = "None"));
            await enemy.Poke1Image.Dispatcher.BeginInvoke((Action)(() => WpfAnimatedGif.ImageBehavior.SetAnimatedSource(enemy.Poke1Image, GetImageSource(path + "/sprites/none.gif"))));
            await enemy.Poke1Evolutions.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Evolutions.Text = "None"));


            await player.Poke2.Dispatcher.BeginInvoke((Action)(() => player.Poke2.Text = "0"));
            await player.Poke2Gen.Dispatcher.BeginInvoke((Action)(() => player.Poke2Gen.Text = "0"));
            await player.Poke2Name.Dispatcher.BeginInvoke((Action)(() => player.Poke2Name.Text = "None"));
            await player.Poke2Type1.Dispatcher.BeginInvoke((Action)(() => player.Poke2Type1.Text = "None"));
            await player.Poke2Type2.Dispatcher.BeginInvoke((Action)(() => player.Poke2Type2.Text = "None"));
            await player.Poke2BST.Dispatcher.BeginInvoke((Action)(() => player.Poke2BST.Text = "0"));
            await player.Poke2LevelRate.Dispatcher.BeginInvoke((Action)(() => player.Poke2LevelRate.Text = "None"));
            await player.Poke2MoveLevels.Dispatcher.BeginInvoke((Action)(() => player.Poke2MoveLevels.Text = "0"));
            await player.Poke2Legendary.Dispatcher.BeginInvoke((Action)(() => player.Poke2Legendary.Text = "False"));
            await player.Poke2EvoReq.Dispatcher.BeginInvoke((Action)(() => player.Poke2EvoReq.Text = "None"));
            await player.Poke2Image.Dispatcher.BeginInvoke((Action)(() => WpfAnimatedGif.ImageBehavior.SetAnimatedSource(player.Poke2Image, GetImageSource(path + "/sprites/none.gif"))));
            await player.Poke2Evolutions.Dispatcher.BeginInvoke((Action)(() => player.Poke2Evolutions.Text = "None"));


        }
        private void ReadFile()
        {
            var wh = new AutoResetEvent(false);
            var fsw = new FileSystemWatcher(".");
            fsw.Filter = path + "/output";
            fsw.EnableRaisingEvents = true;
            fsw.Changed += (s, e) => wh.Set();

            var fs = new FileStream(path + "/output", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var sr = new StreamReader(fs))
            {
                var s = "";
                while (true)
                {
                    s = sr.ReadToEnd();
                    SearchForValues(s);
                    wh.WaitOne(200);
                    sr.DiscardBufferedData();
                    sr.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                }
            }
        }
        int type = 0;
        long GlobalStat = -1;
        long PreviousPokemon = 0;
        int counter = 0;
        bool changed = false;
        bool prechanged = false;

        private async void SearchForValues(string data)
        {
            JObject o = JObject.Parse(data);
            long value1 = Int64.Parse(o["value1"].ToString());
            long value2 = Int64.Parse(o["value2"].ToString());
            long value3 = Int64.Parse(o["value3"].ToString());
            long value4 = Int64.Parse(o["value4"].ToString());
            long PartyPoke = Int64.Parse(o["party1"].ToString());

            if (value1 == 1 && value2 == 0 && value3 == 1)
            {
                value1 = 0;
                value2 = 0;
                value3 = 0;
                value4 = 0;
            }
            if (GlobalStat != -1)
            {
                if (counter > 2)
                {
                    if (GlobalStat != value4 || (GlobalStat == 0 && value4 == 0))
                    {
                        changed = true;
                    }
                    counter = 0;
                }
                else
                {
                    counter++;
                }
            }
            Pokemon EnemyPokemon = null;
            Pokemon PlayersPokemon = null;
            // Type 0 is reset no battle
            // Type 1 is when we went into a wild battle
            // Type 2 is a trainer battle
            if (value1 == 0 && value2 == 0 && value3 == 0)
            {
                type = 0;
                PreviousPokemon = 0;
            }
            if (type == 0)
            {
                if (value1 != 0 && value2 == 0 && value3 != 0)
                {
                    type = 1;
                }
                else if (value1 == 0 && value2 != 0 && value3 != 0)
                {
                    type = 2;
                    GlobalStat = -1;
                    counter = 0;
                    changed = false;
                    PreviousPokemon = 00000;
                }
            }
            else if (type == 1)
            {
                // If its a wild pokemon do standard data
                EnemyPokemon = Pokedex.Find(x => x.Id == value1);
                long tracker = 0;
                if (value2 != 0)
                {
                    tracker = value2;
                }
                else
                {
                    tracker = value3;
                }
                PlayersPokemon = Pokedex.Find(x => x.Id == tracker);
            }
            else if (type == 2)
            {
                if (value1 == 0)
                {
                    // The battle just started and no pokemon has been swapped
                    EnemyPokemon = Pokedex.Find(x => x.Id == value2);
                    PreviousPokemon = value2;
                    PlayersPokemon = Pokedex.Find(x => x.Id == value3);
                    GlobalStat = value4;
                    counter = 0;
                    changed = false;
                }
                else
                {
                    // The Enemy pokemon died and we have not swapped
                    if (PreviousPokemon == value2 && changed == false)
                    {
                        EnemyPokemon = Pokedex.Find(x => x.Id == value1);
                        PlayersPokemon = Pokedex.Find(x => x.Id == value3);
                    }

                    // The Enemy pokemon fainted and we swapped
                    else if (PreviousPokemon != value2 && changed == true)
                    {
                        if (prechanged == false)
                        {
                            EnemyPokemon = Pokedex.Find(x => x.Id == value1);
                            PlayersPokemon = Pokedex.Find(x => x.Id == value2);
                        }
                        else
                        {
                            EnemyPokemon = Pokedex.Find(x => x.Id == value2);
                            PlayersPokemon = Pokedex.Find(x => x.Id == value1);
                        }
                    }
                    // The player changed their pokemon first
                    else if (PreviousPokemon == value2 && changed == true)
                    {
                        EnemyPokemon = Pokedex.Find(x => x.Id == value2);
                        PlayersPokemon = Pokedex.Find(x => x.Id == value1);
                        prechanged = true;
                    }
                    // Else default to assuming slot 1 is the enemy and we are still slot 3
                    else
                    {
                        EnemyPokemon = Pokedex.Find(x => x.Id == value1);
                        PlayersPokemon = Pokedex.Find(x => x.Id == value3);
                    }

                }

            }

            string CurrentEnemyPokemonId = (string)enemy.Poke1.Dispatcher.Invoke(new Func<string>(() => enemy.Poke1.Text));
            string CurrentPlayerPokemonId = (string)player.Poke2.Dispatcher.Invoke(new Func<string>(() => player.Poke2.Text));

            if (EnemyPokemon != null && PlayersPokemon != null)
            {
                if (counter == 0)
                {
                    if (CurrentEnemyPokemonId != EnemyPokemon.Id.ToString())
                    {
                        await enemy.Poke1.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1.Text = EnemyPokemon.Id.ToString()));
                        await enemy.Poke1Gen.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Gen.Text = EnemyPokemon.Gen.ToString()));
                        await enemy.Poke1Name.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Name.Text = EnemyPokemon.Name.ToString()));
                        await enemy.Poke1Type1.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Type1.Text = EnemyPokemon.Type1.ToString()));
                        await enemy.Poke1Type2.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Type2.Text = EnemyPokemon.Type2.ToString()));
                        await enemy.Poke1BST.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1BST.Text = EnemyPokemon.BST.ToString()));
                        await enemy.Poke1LevelRate.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1LevelRate.Text = EnemyPokemon.LevelRate.ToString()));
                        await enemy.Poke1MoveLevels.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1MoveLevels.Text = EnemyPokemon.MoveLevels.ToString()));
                        await enemy.Poke1Legendary.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Legendary.Text = EnemyPokemon.IsLegendary.ToString()));
                        await enemy.Poke1EvoReq.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1EvoReq.Text = EnemyPokemon.EvoReq.ToString()));
                        await enemy.Poke1Image.Dispatcher.BeginInvoke((Action)(() => WpfAnimatedGif.ImageBehavior.SetAnimatedSource(enemy.Poke1Image, GetImageSource(path + "/sprites/" + EnemyPokemon.Name.ToLower() + ".gif"))));
                        var Evos = PossibleEvos(EnemyPokemon);
                        string joined = string.Join(", ", Evos.Select(x => x.Name));
                        await enemy.Poke1Evolutions.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Evolutions.Text = joined));

                    }
                    if (CurrentPlayerPokemonId != PlayersPokemon.Id.ToString())
                    {
                        await player.Poke2.Dispatcher.BeginInvoke((Action)(() => player.Poke2.Text = PlayersPokemon.Id.ToString()));
                        await player.Poke2Gen.Dispatcher.BeginInvoke((Action)(() => player.Poke2Gen.Text = PlayersPokemon.Gen.ToString()));
                        await player.Poke2Name.Dispatcher.BeginInvoke((Action)(() => player.Poke2Name.Text = PlayersPokemon.Name.ToString()));
                        await player.Poke2Type1.Dispatcher.BeginInvoke((Action)(() => player.Poke2Type1.Text = PlayersPokemon.Type1.ToString()));
                        await player.Poke2Type2.Dispatcher.BeginInvoke((Action)(() => player.Poke2Type2.Text = PlayersPokemon.Type2.ToString()));
                        await player.Poke2BST.Dispatcher.BeginInvoke((Action)(() => player.Poke2BST.Text = PlayersPokemon.BST.ToString()));
                        await player.Poke2LevelRate.Dispatcher.BeginInvoke((Action)(() => player.Poke2LevelRate.Text = PlayersPokemon.LevelRate.ToString()));
                        await player.Poke2MoveLevels.Dispatcher.BeginInvoke((Action)(() => player.Poke2MoveLevels.Text = PlayersPokemon.MoveLevels.ToString()));
                        await player.Poke2Legendary.Dispatcher.BeginInvoke((Action)(() => player.Poke2Legendary.Text = PlayersPokemon.IsLegendary.ToString()));
                        await player.Poke2EvoReq.Dispatcher.BeginInvoke((Action)(() => player.Poke2EvoReq.Text = PlayersPokemon.EvoReq.ToString()));
                        await player.Poke2Image.Dispatcher.BeginInvoke((Action)(() => WpfAnimatedGif.ImageBehavior.SetAnimatedSource(player.Poke2Image, GetImageSource(path + "/sprites/" + PlayersPokemon.Name.ToLower() + ".gif"))));
                        var Evos = PossibleEvos(PlayersPokemon);
                        string joined = string.Join(", ", Evos.Select(x => x.Name));
                        await player.Poke2Evolutions.Dispatcher.BeginInvoke((Action)(() => player.Poke2Evolutions.Text = joined));
                    }
                }
            }
            else
            {
                if (AutoHide == true)
                {
                    await enemy.Dispatcher.BeginInvoke((Action)(() => enemy.Opacity = 0));
                    if (PartyView == true)
                    {
                        if (Transparent == true)
                        {
                            await player.Dispatcher.BeginInvoke((Action)(() => player.Opacity = 0.7));
                        }
                        else
                        {
                            await player.Dispatcher.BeginInvoke((Action)(() => player.Opacity = 1));
                        }
                    }
                    else
                    {
                        await player.Dispatcher.BeginInvoke((Action)(() => player.Opacity = 0));
                    }
                }
                else
                {
                    if (Transparent == true)
                    {
                        await enemy.Dispatcher.BeginInvoke((Action)(() => enemy.Opacity = 0.7));
                        await player.Dispatcher.BeginInvoke((Action)(() => player.Opacity = 0.7));
                    }
                    else
                    {
                        await enemy.Dispatcher.BeginInvoke((Action)(() => enemy.Opacity = 1));
                        await player.Dispatcher.BeginInvoke((Action)(() => player.Opacity = 1));
                    }
                }
                if (CurrentEnemyPokemonId != "0")
                {

                    await enemy.Poke1.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1.Text = "0"));
                    await enemy.Poke1Gen.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Gen.Text = "0"));
                    await enemy.Poke1Name.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Name.Text = "None"));
                    await enemy.Poke1Type1.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Type1.Text = "None"));
                    await enemy.Poke1Type2.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Type2.Text = "None"));
                    await enemy.Poke1BST.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1BST.Text = "0"));
                    await enemy.Poke1LevelRate.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1LevelRate.Text = "None"));
                    await enemy.Poke1MoveLevels.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1MoveLevels.Text = "0"));
                    await enemy.Poke1Legendary.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Legendary.Text = "False"));
                    await enemy.Poke1EvoReq.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1EvoReq.Text = "None"));
                    await enemy.Poke1Image.Dispatcher.BeginInvoke((Action)(() => WpfAnimatedGif.ImageBehavior.SetAnimatedSource(enemy.Poke1Image, GetImageSource(path + "/sprites/none.gif"))));
                    await enemy.Poke1Evolutions.Dispatcher.BeginInvoke((Action)(() => enemy.Poke1Evolutions.Text = "None"));
                }
                if (CurrentPlayerPokemonId != "0" || PartyView == true)
                {
                    if (PartyView == true)
                    {
                        PlayersPokemon = Pokedex.Find(x => x.Id == PartyPoke);
                        CurrentPlayerPokemonId = (string)player.Poke2.Dispatcher.Invoke(new Func<string>(() => player.Poke2.Text));
                        if (CurrentPlayerPokemonId != PlayersPokemon.Id.ToString())
                        {
                            await player.Poke2.Dispatcher.BeginInvoke((Action)(() => player.Poke2.Text = PlayersPokemon.Id.ToString()));
                            await player.Poke2Gen.Dispatcher.BeginInvoke((Action)(() => player.Poke2Gen.Text = PlayersPokemon.Gen.ToString()));
                            await player.Poke2Name.Dispatcher.BeginInvoke((Action)(() => player.Poke2Name.Text = PlayersPokemon.Name.ToString()));
                            await player.Poke2Type1.Dispatcher.BeginInvoke((Action)(() => player.Poke2Type1.Text = PlayersPokemon.Type1.ToString()));
                            await player.Poke2Type2.Dispatcher.BeginInvoke((Action)(() => player.Poke2Type2.Text = PlayersPokemon.Type2.ToString()));
                            await player.Poke2BST.Dispatcher.BeginInvoke((Action)(() => player.Poke2BST.Text = PlayersPokemon.BST.ToString()));
                            await player.Poke2LevelRate.Dispatcher.BeginInvoke((Action)(() => player.Poke2LevelRate.Text = PlayersPokemon.LevelRate.ToString()));
                            await player.Poke2MoveLevels.Dispatcher.BeginInvoke((Action)(() => player.Poke2MoveLevels.Text = PlayersPokemon.MoveLevels.ToString()));
                            await player.Poke2Legendary.Dispatcher.BeginInvoke((Action)(() => player.Poke2Legendary.Text = PlayersPokemon.IsLegendary.ToString()));
                            await player.Poke2EvoReq.Dispatcher.BeginInvoke((Action)(() => player.Poke2EvoReq.Text = PlayersPokemon.EvoReq.ToString()));
                            await player.Poke2Image.Dispatcher.BeginInvoke((Action)(() => WpfAnimatedGif.ImageBehavior.SetAnimatedSource(player.Poke2Image, GetImageSource(path + "/sprites/" + PlayersPokemon.Name.ToLower() + ".gif"))));
                            var Evos = PossibleEvos(PlayersPokemon);
                            string joined = string.Join(", ", Evos.Select(x => x.Name));
                            await player.Poke2Evolutions.Dispatcher.BeginInvoke((Action)(() => player.Poke2Evolutions.Text = joined));
                        }

                    }
                    else
                    {
                        if (CurrentPlayerPokemonId != "0")
                        {
                            await player.Poke2.Dispatcher.BeginInvoke((Action)(() => player.Poke2.Text = "0"));
                            await player.Poke2Gen.Dispatcher.BeginInvoke((Action)(() => player.Poke2Gen.Text = "0"));
                            await player.Poke2Name.Dispatcher.BeginInvoke((Action)(() => player.Poke2Name.Text = "None"));
                            await player.Poke2Type1.Dispatcher.BeginInvoke((Action)(() => player.Poke2Type1.Text = "None"));
                            await player.Poke2Type2.Dispatcher.BeginInvoke((Action)(() => player.Poke2Type2.Text = "None"));
                            await player.Poke2BST.Dispatcher.BeginInvoke((Action)(() => player.Poke2BST.Text = "0"));
                            await player.Poke2LevelRate.Dispatcher.BeginInvoke((Action)(() => player.Poke2LevelRate.Text = "None"));
                            await player.Poke2MoveLevels.Dispatcher.BeginInvoke((Action)(() => player.Poke2MoveLevels.Text = "0"));
                            await player.Poke2Legendary.Dispatcher.BeginInvoke((Action)(() => player.Poke2Legendary.Text = "False"));
                            await player.Poke2EvoReq.Dispatcher.BeginInvoke((Action)(() => player.Poke2EvoReq.Text = "None"));
                            await player.Poke2Image.Dispatcher.BeginInvoke((Action)(() => WpfAnimatedGif.ImageBehavior.SetAnimatedSource(player.Poke2Image, GetImageSource(path + "/sprites/none.gif"))));
                            await player.Poke2Evolutions.Dispatcher.BeginInvoke((Action)(() => player.Poke2Evolutions.Text = "None"));

                        }
                    }
                }
            }


        }
        public bool PartyView = false;
        public bool AutoHide = false;
        public void EnableParty_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    PartyView = true;
                }
                else
                {
                    PartyView = false;
                }
            }
        }
        public void AutoHide_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    AutoHide = true;
                }
                else
                {
                    AutoHide = false;
                }
            }
        }
        Player player = new Player();
        public void PlayerOpen_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                ThemeManager.Current.ChangeTheme(player, "Dark.Green");
                if (toggleSwitch.IsOn == true)
                {
                    player.Show();
                }
                else
                {
                    player.Hide();
                }
            }
        }
        Enemy enemy = new Enemy();

        public void EnemyOpen_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                ThemeManager.Current.ChangeTheme(enemy, "Dark.Green");

                if (toggleSwitch.IsOn == true)
                {
                    enemy.Show();
                }
                else
                {
                    enemy.Hide();

                }
            }
        }
        bool Transparent = false;
        public void Transparent_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    Transparent = true;
                }
                else
                {
                    Transparent = false;

                }
            }
        }
        public ImageSource GetImageSource(string filename)
        {
            string _fileName = filename;

            BitmapImage glowIcon = new BitmapImage();

            glowIcon.BeginInit();
            glowIcon.UriSource = new Uri(_fileName);
            glowIcon.EndInit();

            return glowIcon;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
