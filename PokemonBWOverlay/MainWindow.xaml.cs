using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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

namespace PokemonBWOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly Process process;
        readonly Timer timer = new Timer(1000);
        readonly IntPtr hProc;
        readonly List<Pokemon> Pokedex = new List<Pokemon>();
        readonly string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public MainWindow()
        {
            InitializeComponent();
            process = Process.GetProcesses().Where(x => x.ProcessName.Contains("DeSmuME")).First();
            if(process == null)
            {
                MessageBox.Show("Please start DeSmuME before running this program");
                System.Environment.Exit(1);
            }
            hProc = OpenProcess(0x001F0FFF, false, process.Id);
            timer.Elapsed += SearchForValues;
            Pokedex = JsonConvert.DeserializeObject<List<Pokemon>>(File.ReadAllText(@"dex.json"));
            timer.Start();
            SetupItems();
        }
        public async void SetupItems()
        {
            await this.Poke1.Dispatcher.BeginInvoke((Action)(() => Poke1.Text = "0"));
            await this.Poke1Gen.Dispatcher.BeginInvoke((Action)(() => Poke1Gen.Text = "0"));
            await this.Poke1Name.Dispatcher.BeginInvoke((Action)(() => Poke1Name.Text = "None"));
            await this.Poke1Type1.Dispatcher.BeginInvoke((Action)(() => Poke1Type1.Text = "None"));
            await this.Poke1Type2.Dispatcher.BeginInvoke((Action)(() => Poke1Type2.Text = "None"));
            await this.Poke1BST.Dispatcher.BeginInvoke((Action)(() => Poke1BST.Text = "0"));
            await this.Poke1LevelRate.Dispatcher.BeginInvoke((Action)(() => Poke1LevelRate.Text = "None"));
            await this.Poke1MoveLevels.Dispatcher.BeginInvoke((Action)(() => Poke1MoveLevels.Text = "0"));
            await this.Poke1Legendary.Dispatcher.BeginInvoke((Action)(() => Poke1Legendary.Text = "False"));
            await this.Poke1EvoReq.Dispatcher.BeginInvoke((Action)(() => Poke1EvoReq.Text = "None"));
            await this.Poke1Image.Dispatcher.BeginInvoke((Action)(() => WpfAnimatedGif.ImageBehavior.SetAnimatedSource(Poke1Image, GetImageSource(path + "/sprites/none.gif"))));

            await this.Poke2.Dispatcher.BeginInvoke((Action)(() => Poke2.Text = "0"));
            await this.Poke2Gen.Dispatcher.BeginInvoke((Action)(() => Poke2Gen.Text = "0"));
            await this.Poke2Name.Dispatcher.BeginInvoke((Action)(() => Poke2Name.Text = "None"));
            await this.Poke2Type1.Dispatcher.BeginInvoke((Action)(() => Poke2Type1.Text = "None"));
            await this.Poke2Type2.Dispatcher.BeginInvoke((Action)(() => Poke2Type2.Text = "None"));
            await this.Poke2BST.Dispatcher.BeginInvoke((Action)(() => Poke2BST.Text = "0"));
            await this.Poke2LevelRate.Dispatcher.BeginInvoke((Action)(() => Poke2LevelRate.Text = "None"));
            await this.Poke2MoveLevels.Dispatcher.BeginInvoke((Action)(() => Poke2MoveLevels.Text = "0"));
            await this.Poke2Legendary.Dispatcher.BeginInvoke((Action)(() => Poke2Legendary.Text = "False"));
            await this.Poke2EvoReq.Dispatcher.BeginInvoke((Action)(() => Poke2EvoReq.Text = "None"));
            await this.Poke2Image.Dispatcher.BeginInvoke((Action)(() => WpfAnimatedGif.ImageBehavior.SetAnimatedSource(Poke2Image, GetImageSource(path + "/sprites/none.gif"))));
        }

        int type = 0;
        long GlobalStat = -1;
        long PreviousPokemon = 0;
        int counter = 0;
        bool changed = false;

        private async void SearchForValues(object sender, ElapsedEventArgs e)
        {

            // Wild - Has a value
            // Trainer Battle - Starts at 0
            var addr1 = FindDMAAddy(hProc, (IntPtr)(0x14A34740C), new int[] { 0x374, 0x14, 0 });
            long value1 = 0;
            try
            {
                value1 = int.Parse(addr1.ToString("X"), System.Globalization.NumberStyles.HexNumber);
                if (value1 > 5000)
                {
                    value1 = 0;
                }
            }
            catch { }

            // Wild Battle - Starts at 0
            // Trainer Battle - Starts as the enemy
            var addr2 = FindDMAAddy(hProc, (IntPtr)(0x14A347468), new int[] { 0x374, 0x14, 0 });
            long value2 = 0;
            try
            {
                value2 = int.Parse(addr2.ToString("X"), System.Globalization.NumberStyles.HexNumber);
                if (value2 > 5000)
                {
                    value2 = 0;
                }
            }
            catch { }

            // Wild Battle - Starts as player 
            // Trainer Battle - Starts as the players inital pokemon
            var addr3 = FindDMAAddy(hProc, (IntPtr)(0x14A3474C4), new int[] { 0x374, 0x14, 0 });
            long value3 = 0;
            try
            {
                value3 = int.Parse(addr3.ToString("X"), System.Globalization.NumberStyles.HexNumber);
                if (value3 > 5000)
                {
                    value3 = 0;
                }
            }
            catch { }

            // Only used for detecting a pokemon swap of the player
            var addr4 = FindDMAAddy(hProc, (IntPtr)(0x14A293A88), new int[] { 0x374, 0x14, 0 });
            long value4 = 0;
            try
            {
                value4 = Int64.Parse(addr4.ToString("X"), System.Globalization.NumberStyles.HexNumber);

                if (GlobalStat != -1)
                {
                    if (counter > 2)
                    {
                        if (GlobalStat != value4 || (GlobalStat == 0 && value4 == 0))
                        {
                            changed = true;
                        }
                    }
                    else
                    {
                        counter++;
                    }
                }


            }
            catch { }

            Pokemon EnemyPokemon = null;
            Pokemon PlayersPokemon = null;
            // Type 0 is reset no battle
            // Type 1 is when we went into a wild battle
            // Type 2 is a trainer battle
            if(value1 == 0 && value2 == 0 && value3 == 0)
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
                }
                else
                {
                    // The player changed their pokemon first
                    if(PreviousPokemon == value2 && changed == true)
                    {
                        EnemyPokemon = Pokedex.Find(x => x.Id == value2);
                        PlayersPokemon = Pokedex.Find(x => x.Id == value1);
                    }
                    // The Enemy pokemon died and we have not swapped
                    else if (PreviousPokemon == value2 && changed == false)
                    {
                        EnemyPokemon = Pokedex.Find(x => x.Id == value1);
                        PlayersPokemon = Pokedex.Find(x => x.Id == value3);
                    }
                    // The Enemy pokemon fainted and we swapped
                    else if (PreviousPokemon != value2 && changed == true)
                    {
                        EnemyPokemon = Pokedex.Find(x => x.Id == value1);
                        PlayersPokemon = Pokedex.Find(x => x.Id == value2);
                    }
                    // Else default to assuming slot 1 is the enemy and we are still slot 3
                    else
                    {
                        EnemyPokemon = Pokedex.Find(x => x.Id == value1);
                        PlayersPokemon = Pokedex.Find(x => x.Id == value3);
                    }

                }

            }
            string CurrentEnemyPokemonId = (string)Poke1.Dispatcher.Invoke(new Func<string>(() => Poke1.Text));
            string CurrentPlayerPokemonId = (string)Poke2.Dispatcher.Invoke(new Func<string>(() => Poke2.Text));

            if (EnemyPokemon != null && PlayersPokemon != null)
            {
                if (CurrentEnemyPokemonId != EnemyPokemon.Id.ToString())
                {
                    await this.Poke1.Dispatcher.BeginInvoke((Action)(() => Poke1.Text = EnemyPokemon.Id.ToString()));
                    await this.Poke1Gen.Dispatcher.BeginInvoke((Action)(() => Poke1Gen.Text = EnemyPokemon.Gen.ToString()));
                    await this.Poke1Name.Dispatcher.BeginInvoke((Action)(() => Poke1Name.Text = EnemyPokemon.Name.ToString()));
                    await this.Poke1Type1.Dispatcher.BeginInvoke((Action)(() => Poke1Type1.Text = EnemyPokemon.Type1.ToString()));
                    await this.Poke1Type2.Dispatcher.BeginInvoke((Action)(() => Poke1Type2.Text = EnemyPokemon.Type2.ToString()));
                    await this.Poke1BST.Dispatcher.BeginInvoke((Action)(() => Poke1BST.Text = EnemyPokemon.BST.ToString()));
                    await this.Poke1LevelRate.Dispatcher.BeginInvoke((Action)(() => Poke1LevelRate.Text = EnemyPokemon.LevelRate.ToString()));
                    await this.Poke1MoveLevels.Dispatcher.BeginInvoke((Action)(() => Poke1MoveLevels.Text = EnemyPokemon.MoveLevels.ToString()));
                    await this.Poke1Legendary.Dispatcher.BeginInvoke((Action)(() => Poke1Legendary.Text = EnemyPokemon.IsLegendary.ToString()));
                    await this.Poke1EvoReq.Dispatcher.BeginInvoke((Action)(() => Poke1EvoReq.Text = EnemyPokemon.EvoReq.ToString()));
                    await this.Poke1Image.Dispatcher.BeginInvoke((Action)(() => WpfAnimatedGif.ImageBehavior.SetAnimatedSource(Poke1Image, GetImageSource(path + "/sprites/" + EnemyPokemon.Name + ".gif"))));
                }
                if (CurrentPlayerPokemonId != PlayersPokemon.Id.ToString())
                {
                    await this.Poke2.Dispatcher.BeginInvoke((Action)(() => Poke2.Text = PlayersPokemon.Id.ToString()));
                    await this.Poke2Gen.Dispatcher.BeginInvoke((Action)(() => Poke2Gen.Text = PlayersPokemon.Gen.ToString()));
                    await this.Poke2Name.Dispatcher.BeginInvoke((Action)(() => Poke2Name.Text = PlayersPokemon.Name.ToString()));
                    await this.Poke2Type1.Dispatcher.BeginInvoke((Action)(() => Poke2Type1.Text = PlayersPokemon.Type1.ToString()));
                    await this.Poke2Type2.Dispatcher.BeginInvoke((Action)(() => Poke2Type2.Text = PlayersPokemon.Type2.ToString()));
                    await this.Poke2BST.Dispatcher.BeginInvoke((Action)(() => Poke2BST.Text = PlayersPokemon.BST.ToString()));
                    await this.Poke2LevelRate.Dispatcher.BeginInvoke((Action)(() => Poke2LevelRate.Text = PlayersPokemon.LevelRate.ToString()));
                    await this.Poke2MoveLevels.Dispatcher.BeginInvoke((Action)(() => Poke2MoveLevels.Text = PlayersPokemon.MoveLevels.ToString()));
                    await this.Poke2Legendary.Dispatcher.BeginInvoke((Action)(() => Poke2Legendary.Text = PlayersPokemon.IsLegendary.ToString()));
                    await this.Poke2EvoReq.Dispatcher.BeginInvoke((Action)(() => Poke2EvoReq.Text = PlayersPokemon.EvoReq.ToString()));
                    await this.Poke2Image.Dispatcher.BeginInvoke((Action)(() => WpfAnimatedGif.ImageBehavior.SetAnimatedSource(Poke2Image, GetImageSource(path + "/sprites/" + PlayersPokemon.Name + ".gif"))));
                }
            }
            else
            {
                if (CurrentEnemyPokemonId != "0")
                {
                    await this.Poke1.Dispatcher.BeginInvoke((Action)(() => Poke1.Text = "0"));
                    await this.Poke1Gen.Dispatcher.BeginInvoke((Action)(() => Poke1Gen.Text = "0"));
                    await this.Poke1Name.Dispatcher.BeginInvoke((Action)(() => Poke1Name.Text = "None"));
                    await this.Poke1Type1.Dispatcher.BeginInvoke((Action)(() => Poke1Type1.Text = "None"));
                    await this.Poke1Type2.Dispatcher.BeginInvoke((Action)(() => Poke1Type2.Text = "None"));
                    await this.Poke1BST.Dispatcher.BeginInvoke((Action)(() => Poke1BST.Text = "0"));
                    await this.Poke1LevelRate.Dispatcher.BeginInvoke((Action)(() => Poke1LevelRate.Text = "None"));
                    await this.Poke1MoveLevels.Dispatcher.BeginInvoke((Action)(() => Poke1MoveLevels.Text = "0"));
                    await this.Poke1Legendary.Dispatcher.BeginInvoke((Action)(() => Poke1Legendary.Text = "False"));
                    await this.Poke1EvoReq.Dispatcher.BeginInvoke((Action)(() => Poke1EvoReq.Text = "None"));
                    await this.Poke1Image.Dispatcher.BeginInvoke((Action)(() => WpfAnimatedGif.ImageBehavior.SetAnimatedSource(Poke1Image, GetImageSource(path + "/sprites/none.gif"))));
                }
                if (CurrentPlayerPokemonId != "0")
                {
                    await this.Poke2.Dispatcher.BeginInvoke((Action)(() => Poke2.Text = "0"));
                    await this.Poke2Gen.Dispatcher.BeginInvoke((Action)(() => Poke2Gen.Text = "0"));
                    await this.Poke2Name.Dispatcher.BeginInvoke((Action)(() => Poke2Name.Text = "None"));
                    await this.Poke2Type1.Dispatcher.BeginInvoke((Action)(() => Poke2Type1.Text = "None"));
                    await this.Poke2Type2.Dispatcher.BeginInvoke((Action)(() => Poke2Type2.Text = "None"));
                    await this.Poke2BST.Dispatcher.BeginInvoke((Action)(() => Poke2BST.Text = "0"));
                    await this.Poke2LevelRate.Dispatcher.BeginInvoke((Action)(() => Poke2LevelRate.Text = "None"));
                    await this.Poke2MoveLevels.Dispatcher.BeginInvoke((Action)(() => Poke2MoveLevels.Text = "0"));
                    await this.Poke2Legendary.Dispatcher.BeginInvoke((Action)(() => Poke2Legendary.Text = "False"));
                    await this.Poke2EvoReq.Dispatcher.BeginInvoke((Action)(() => Poke2EvoReq.Text = "None"));
                    await this.Poke2Image.Dispatcher.BeginInvoke((Action)(() => WpfAnimatedGif.ImageBehavior.SetAnimatedSource(Poke2Image, GetImageSource(path + "/sprites/none.gif"))));
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
        public IntPtr FindDMAAddy(IntPtr hProc, IntPtr ptr, int[] offsets)
        {
            var buffer = new byte[IntPtr.Size];
            foreach (int i in offsets)
            {
                ReadProcessMemory(hProc, ptr, buffer, buffer.Length, out var read);

                ptr = (IntPtr.Size == 4)
                ? IntPtr.Add(new IntPtr(BitConverter.ToInt32(buffer, 0)), i)
                : ptr = IntPtr.Add(new IntPtr(BitConverter.ToInt64(buffer, 0)), i);
            }
            return ptr;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Boolean bInheritHandle, Int32 dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess,
            IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);


        public IntPtr GetModuleBaseAddress(Process proc, string modName)
        {
            IntPtr addr = IntPtr.Zero;

            foreach (ProcessModule m in proc.Modules)
            {
                if (m.ModuleName == modName)
                {
                    addr = m.BaseAddress;
                    break;
                }
            }
            return addr;
        }
    }
}
