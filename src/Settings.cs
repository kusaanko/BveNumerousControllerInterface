using System.IO;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public class Settings
    {
        private const string filename = "Kusaanko.NumerousControllerInterface.json";
        private const string profileDirectory = "Kusaanko.NumerousControllerInterface.Profiles/";
        private string directory = string.Empty;

        [JsonIgnore]
        public Dictionary<string, ControllerProfile> Profiles = new Dictionary<string, ControllerProfile>();
        public Dictionary<string, bool> IsEnabled = new Dictionary<string, bool>();
        public Dictionary<string, string> ProfileMap = new Dictionary<string, string>();
        [JsonIgnore]
        public List<string> removeProfilesList = new List<string>();
        public bool AlertNoControllerFound;

        public Settings()
        {
            {
                ControllerProfile profile = new ControllerProfile();
                profile.IsTwoHandle = true;
                profile.IsMasterController = true;
                profile.PowerAxises = new int[] { 21 };
                profile.PowerAxisStatus = new int[,] {
                    { -1000 },
                    { 1000 },
                    { 1000 },
                    { -1000 },
                    { -1000 },
                    { -8 }
                };
                profile.PowerButtons = new int[] { 0 };
                profile.PowerButtonStatus = new bool[,] {
                    { false },
                    { true },
                    { false },
                    { true },
                    { false },
                    { true }
                };
                profile.BreakButtons = new int[] { 4, 5, 6, 7};
                profile.BreakButtonStatus = new bool[,] { 
                    { true, true, false, true },
                    { false, true, true, true },
                    { false, true, false, true },
                    { true, true, true, false },
                    { true, true, false, false },
                    { false, true, true, false },
                    { false, true, false, false },
                    { true, false, true, true },
                    { true, false, false, true },
                    { false, false, false, false }
                };
                profile.CalcDuplicated();
                profile.KeyMap = new Dictionary<int, int[]>();
                profile.KeyMap.Add(1, new int[] { 0, 2});
                profile.KeyMap.Add(2, new int[] { 0, 0 });
                profile.KeyMap.Add(3, new int[] { -1, 1 });
                profile.KeyMap.Add(9, new int[] { -3, 12 });
                profile.KeyMap.Add(8, new int[] { -3, 11 });
                Profiles.Add("JC-PS101U PS用電車でGO!コントローラー(ワン,ツーハンドル)", profile);
                ProfileMap.Add("JC-PS101U", "JC-PS101U PS用電車でGO!コントローラー(ワン,ツーハンドル)");
            }
            {
                ControllerProfile profile = new ControllerProfile();
                profile.IsTwoHandle = true;
                profile.IsMasterController = true;
                profile.CalcDuplicated();
                profile.KeyMap = new Dictionary<int, int[]>();
                //力行
                profile.KeyMap.Add(128, new int[] { 99, 7 });
                profile.KeyMap.Add(130, new int[] { 99, 6 });
                //ブレーキ
                profile.KeyMap.Add(2, new int[] { 99, 4 });
                profile.KeyMap.Add(3, new int[] { 99, 3 });
                //非常
                profile.KeyMap.Add(0, new int[] { 99, 0 });
                //警笛
                profile.KeyMap.Add(1, new int[] { -1, 1 });
                profile.KeyMap.Add(6, new int[] { 99, 1 });
                profile.KeyMap.Add(7, new int[] { 99, 1 });
                profile.KeyMap.Add(9, new int[] { -3, 12 });
                profile.KeyMap.Add(8, new int[] { -3, 11 });
                Profiles.Add("JC-PS101U DualShock2", profile);
            }
            {
                ControllerProfile profile = new ControllerProfile();
                profile.IsTwoHandle = true;
                profile.IsMasterController = true;
                profile.PowerButtons = new int[] { 11, 12, 13 };
                profile.PowerButtonStatus = new bool[,] {
                    { false, false, false },
                    { true, false, false },
                    { false, true, false },
                    { true, true, false },
                    { false, false, true },
                    { true, false, true }
                };
                profile.BreakButtons = new int[] { 14, 15, 16, 17 };
                profile.BreakButtonStatus = new bool[,] {
                    { false, false, false, false },
                    { true, false, false, false },
                    { false, true, false, false },
                    { true, true, false, false },
                    { false, false, true, false },
                    { true, false, true, false },
                    { false, true, true, false },
                    { true, true, true, false },
                    { false, false, false, true},
                    { true, false, false, true }
                };
                profile.CalcDuplicated();
                profile.KeyMap = new Dictionary<int, int[]>();
                profile.KeyMap.Add(1, new int[] { 0, 2 });//C
                profile.KeyMap.Add(0, new int[] { 0, 0 });//B
                profile.KeyMap.Add(2, new int[] { -1, 1 });//A
                profile.KeyMap.Add(3, new int[] { -2, 0 });//D
                profile.KeyMap.Add(4, new int[] { -3, 12 });//SELECT
                profile.KeyMap.Add(5, new int[] { -3, 11 });//START
                Profiles.Add("TCPP-20009 PS2用電車でGO!コントローラー TYPE2", profile);
                ProfileMap.Add("PS2用電車でGO!コントローラー TYPE2", "TCPP-20009 PS2用電車でGO!コントローラー TYPE2");
            }
            {
                ControllerProfile profile = new ControllerProfile();
                profile.IsTwoHandle = false;
                profile.IsMasterController = true;
                profile.PowerButtons = new int[] { 11, 12, 13 };
                profile.PowerButtonStatus = new bool[,] {
                    { false, false, false },
                    { true, false, false },
                    { false, true, false },
                    { true, true, false },
                    { false, false, true },
                    { true, false, true }
                };
                profile.BreakButtons = new int[] { 14, 15, 16, 17 };
                profile.BreakButtonStatus = new bool[,] {
                    { false, false, false, false },
                    { true, false, false, false },
                    { false, true, false, false },
                    { true, true, false, false },
                    { false, false, true, false },
                    { true, false, true, false },
                    { false, true, true, false },
                    { true, true, true, false },
                    { false, false, false, true},
                    { true, false, false, true }
                };
                profile.CalcDuplicated();
                profile.KeyMap = new Dictionary<int, int[]>();
                profile.KeyMap.Add(1, new int[] { 0, 2 });//C
                profile.KeyMap.Add(0, new int[] { 0, 0 });//B
                profile.KeyMap.Add(2, new int[] { -1, 1 });//A
                profile.KeyMap.Add(3, new int[] { -2, 0 });//D
                profile.KeyMap.Add(4, new int[] { -3, 12 });//SELECT
                profile.KeyMap.Add(5, new int[] { -3, 11 });//START
                Profiles.Add("MultiTrainController P5B8", profile);
                ProfileMap.Add("MultiTrainController P5B8", "MultiTrainController P5B8");
            }
            {
                ControllerProfile profile = new ControllerProfile();
                profile.IsTwoHandle = false;
                profile.IsMasterController = true;
                profile.PowerButtons = new int[] { 15, 16, 17 };
                profile.PowerButtonStatus = new bool[,] {
                    { false, false, false },
                    { true, false, false },
                    { false, true, false },
                    { true, true, false },
                    { false, false, true }
                };
                profile.BreakButtons = new int[] { 18, 19, 20, 21 };
                profile.BreakButtonStatus = new bool[,] {
                    { false, false, false, false },
                    { true, false, false, false },
                    { false, true, false, false },
                    { true, true, false, false },
                    { false, false, true, false },
                    { true, false, true, false },
                    { false, true, true, false },
                    { true, true, true, false }
                };
                profile.CalcDuplicated();
                profile.KeyMap = new Dictionary<int, int[]>();
                profile.KeyMap.Add(4, new int[] { -1, 1 });//B
                profile.KeyMap.Add(0, new int[] { -2, 0 });//ATS
                profile.KeyMap.Add(2, new int[] { -2, 1 });//A Shallow
                profile.KeyMap.Add(3, new int[] { -2, 2 });//A Deep
                profile.KeyMap.Add(5, new int[] { -2, 3 });//C
                profile.KeyMap.Add(1, new int[] { -2, 4 });//D
                profile.KeyMap.Add(7, new int[] { -3, 12 });//SELECT
                profile.KeyMap.Add(6, new int[] { -3, 11 });//START
                profile.KeyMap.Add(12, new int[] { 0, 2 });//Reverser Front
                profile.KeyMap.Add(13, new int[] { 0, 1 });//Reverser Center
                profile.KeyMap.Add(14, new int[] { 0, 0 });//Reverser Back
                Profiles.Add("MultiTrainController P4B6", profile);
                ProfileMap.Add("MultiTrainController P4B6", "MultiTrainController P4B6");
            }
            {
                ControllerProfile profile = new ControllerProfile();
                profile.IsTwoHandle = false;
                profile.IsMasterController = true;
                profile.PowerButtons = new int[] { 15, 16, 17 };
                profile.PowerButtonStatus = new bool[,] {
                    { false, false, false },
                    { true, false, false },
                    { false, true, false },
                    { true, true, false },
                    { false, false, true }
                };
                profile.BreakButtons = new int[] { 18, 19, 20, 21 };
                profile.BreakButtonStatus = new bool[,] {
                    { false, false, false, false },
                    { true, false, false, false },
                    { false, true, false, false },
                    { true, true, false, false },
                    { false, false, true, false },
                    { true, false, true, false },
                    { false, true, true, false },
                    { true, true, true, false },
                    { false, false, false, true}
                };
                profile.CalcDuplicated();
                profile.KeyMap = new Dictionary<int, int[]>();
                profile.KeyMap.Add(4, new int[] { -1, 1 });//B
                profile.KeyMap.Add(0, new int[] { -2, 0 });//ATS
                profile.KeyMap.Add(2, new int[] { -2, 1 });//A Shallow
                profile.KeyMap.Add(3, new int[] { -2, 2 });//A Deep
                profile.KeyMap.Add(5, new int[] { -2, 3 });//C
                profile.KeyMap.Add(1, new int[] { -2, 4 });//D
                profile.KeyMap.Add(7, new int[] { -3, 12 });//SELECT
                profile.KeyMap.Add(6, new int[] { -3, 11 });//START
                profile.KeyMap.Add(12, new int[] { 0, 2 });//Reverser Front
                profile.KeyMap.Add(13, new int[] { 0, 1 });//Reverser Center
                profile.KeyMap.Add(14, new int[] { 0, 0 });//Reverser Back
                Profiles.Add("MultiTrainController P4B7", profile);
                ProfileMap.Add("MultiTrainController P4B7", "MultiTrainController P4B7");
            }
            {
                ControllerProfile profile = new ControllerProfile();
                profile.IsTwoHandle = false;
                profile.IsMasterController = true;
                profile.PowerButtons = new int[] { 15, 16, 17 };
                profile.PowerButtonStatus = new bool[,] {
                    { false, false, false },
                    { true, false, false },
                    { false, true, false },
                    { true, true, false },
                    { false, false, true },
                    { true, false, true }
                };
                profile.BreakButtons = new int[] { 18, 19, 20, 21 };
                profile.BreakButtonStatus = new bool[,] {
                    { false, false, false, false },
                    { true, false, false, false },
                    { false, true, false, false },
                    { true, true, false, false },
                    { false, false, true, false },
                    { true, false, true, false },
                    { false, true, true, false },
                };
                profile.CalcDuplicated();
                profile.KeyMap = new Dictionary<int, int[]>();
                profile.KeyMap.Add(4, new int[] { -1, 1 });//B
                profile.KeyMap.Add(0, new int[] { -2, 0 });//ATS
                profile.KeyMap.Add(2, new int[] { -2, 1 });//A Shallow
                profile.KeyMap.Add(3, new int[] { -2, 2 });//A Deep
                profile.KeyMap.Add(5, new int[] { -2, 3 });//C
                profile.KeyMap.Add(1, new int[] { -2, 4 });//D
                profile.KeyMap.Add(7, new int[] { -3, 12 });//SELECT
                profile.KeyMap.Add(6, new int[] { -3, 11 });//START
                profile.KeyMap.Add(12, new int[] { 0, 2 });//Reverser Front
                profile.KeyMap.Add(13, new int[] { 0, 1 });//Reverser Center
                profile.KeyMap.Add(14, new int[] { 0, 0 });//Reverser Back
                Profiles.Add("MultiTrainController P5B5", profile);
                ProfileMap.Add("MultiTrainController P5B5", "MultiTrainController P5B5");
            }
            AlertNoControllerFound = true;
        }
        public void SaveToXml()
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            string json = JsonConvert.SerializeObject(this);
            using (FileStream fs = new FileStream(Path.Combine(directory, filename), FileMode.Create)) // ファイルを開く
            {
                StreamWriter writer = new StreamWriter(fs);
                writer.Write(json);
                writer.Close();
            }
            if (!Directory.Exists(Path.Combine(directory, profileDirectory))) Directory.CreateDirectory(Path.Combine(directory, profileDirectory));
            foreach (string name in removeProfilesList)
            {
                File.Delete(Path.Combine(directory, profileDirectory + name + ".json"));
            }
            foreach (string name in Profiles.Keys)
            {
                ControllerProfile profile = Profiles[name];
                json = JsonConvert.SerializeObject(profile);

                using (FileStream fs = new FileStream(Path.Combine(directory, profileDirectory + name + ".json"), FileMode.Create)) // ファイルを開く
                {
                    StreamWriter writer = new StreamWriter(fs);
                    writer.Write(json);
                    writer.Close();
                }
            }
        }

        public static Settings LoadFromXml(string directory)
        {
            Settings settings;
            try
            {
                settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(Path.Combine(directory, filename)));

            }
            catch
            {
                settings = new Settings();
            }
            string key = "";
            if (!Directory.Exists(Path.Combine(directory, profileDirectory))) Directory.CreateDirectory(Path.Combine(directory, profileDirectory));
            foreach (string file in Directory.GetFiles(Path.Combine(directory, profileDirectory)))
            {
                try
                {
                    ControllerProfile profile = JsonConvert.DeserializeObject<ControllerProfile>(File.ReadAllText(file));
                    key = Path.GetFileNameWithoutExtension(file);
                    if (settings.Profiles.ContainsKey(key))
                    {
                        settings.Profiles.Remove(key);
                    }
                    settings.Profiles.Add(key, profile);
                    profile.CalcDuplicated();
                }
                catch(Exception e)
                {
                    MessageBox.Show("プロファイルの読み込みに失敗しました。\n" + file + "\n" + e.Message, "DenshadeGoInterface");
                }
            }
            List<string> changeList = new List<string>();
            foreach (string name in settings.ProfileMap.Keys)
            {
                if (!settings.Profiles.ContainsKey(settings.ProfileMap[name]))
                {
                    changeList.Add(name);
                }
            }
            foreach (string k in settings.Profiles.Keys)
            {
                key = k;
                break;
            }
            foreach (string name in changeList)
            {
                MessageBox.Show(name + " のプロファイル " + settings.ProfileMap[name] + " が見つからなかったため " + name + "に変更しました。", "DenshadeGoInterface");
                settings.ProfileMap[name] = key;
            }

            settings.directory = directory;

            return settings;
        }

        public ControllerProfile GetProfile(IController controller)
        {
            return Profiles[ProfileMap[controller.GetName()]];
        }

        public bool GetIsEnabled(string controller)
        {
            if (!IsEnabled.ContainsKey(controller))
            {
                IsEnabled.Add(controller, false);
            }
            return IsEnabled[controller];
        }

    }
}
