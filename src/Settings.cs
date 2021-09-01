using System.IO;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Windows.Forms;
using Kusaanko.Bvets.NumerousControllerInterface.Controller;
using System.Text;
using System.Security.Cryptography;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public class Settings
    {
        private const string _filename = "Kusaanko.NumerousControllerInterface.json";
        private const string _profileDirectory = "Kusaanko.NumerousControllerInterface.Profiles\\";
        private string _directory = string.Empty;

        [JsonIgnore]
        public Dictionary<string, ControllerProfile> Profiles = new Dictionary<string, ControllerProfile>();
        public Dictionary<string, bool> IsEnabled = new Dictionary<string, bool>();
        public Dictionary<string, string> ProfileMap = new Dictionary<string, string>();
        [JsonIgnore]
        public List<string> removeProfilesList = new List<string>();
        public bool AlertNoControllerFound;
        public bool CheckUpdates;

        public Settings()
        {
            {
                ControllerProfile profile = new ControllerProfile();
                profile.IsTwoHandle = true;
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
                profile.KeyMap.Add(1, ButtonFeature.ReverserBackward);
                profile.KeyMap.Add(2, ButtonFeature.ReverserForward);
                profile.KeyMap.Add(3, ButtonFeature.Horn);
                profile.KeyMap.Add(9, ButtonFeature.Pause);
                profile.KeyMap.Add(8, ButtonFeature.Fastforward);
                Profiles.Add("JC-PS101U PS用電車でGO!コントローラー(ワン,ツーハンドル)", profile);
                ProfileMap.Add("JC-PS101U", "JC-PS101U PS用電車でGO!コントローラー(ワン,ツーハンドル)");
            }
            {
                ControllerProfile profile = new ControllerProfile();
                profile.IsTwoHandle = true;
                profile.CalcDuplicated();
                //力行
                profile.KeyMap.Add(128, ButtonFeature.BringPowerDown);
                profile.KeyMap.Add(130, ButtonFeature.BringPowerUp);
                //制動
                profile.KeyMap.Add(2, ButtonFeature.BringBreakDown);
                profile.KeyMap.Add(3, ButtonFeature.BringBreakUp);
                //非常
                profile.KeyMap.Add(0, ButtonFeature.SetNotchEB);
                //警笛
                profile.KeyMap.Add(1, ButtonFeature.Horn);
                profile.KeyMap.Add(6, ButtonFeature.SetNotchOff);
                profile.KeyMap.Add(7, ButtonFeature.SetNotchOff);
                profile.KeyMap.Add(9, ButtonFeature.Pause);
                profile.KeyMap.Add(8, ButtonFeature.Fastforward);
                Profiles.Add("JC-PS101U DualShock2", profile);
            }
            {
                ControllerProfile profile = new ControllerProfile();
                profile.IsTwoHandle = true;
                profile.CalcDuplicated();
                profile.KeyMap.Add(1, ButtonFeature.ReverserBackward);//C
                profile.KeyMap.Add(0, ButtonFeature.ReverserForward);//B
                profile.KeyMap.Add(2, ButtonFeature.Horn);//A
                profile.KeyMap.Add(3, ButtonFeature.Ats0);//D
                profile.KeyMap.Add(4, ButtonFeature.Pause);//SELECT
                profile.KeyMap.Add(5, ButtonFeature.Fastforward);//START
                Profiles.Add("TCPP-20009 PS2用電車でGO!コントローラー TYPE2", profile);
                ProfileMap.Add("PS2用電車でGO!コントローラー TYPE2", "TCPP-20009 PS2用電車でGO!コントローラー TYPE2");
            }
            {
                ControllerProfile profile = new ControllerProfile();
                profile.IsTwoHandle = false;
                profile.CalcDuplicated();
                profile.KeyMap.Add(1, ButtonFeature.ReverserBackward);//C
                profile.KeyMap.Add(0, ButtonFeature.ReverserForward);//B
                profile.KeyMap.Add(2, ButtonFeature.Horn);//A
                profile.KeyMap.Add(3, ButtonFeature.Ats0);//D
                profile.KeyMap.Add(4, ButtonFeature.Pause);//SELECT
                profile.KeyMap.Add(5, ButtonFeature.Fastforward);//START
                Profiles.Add("MultiTrainController P5B8", profile);
                ProfileMap.Add("MultiTrainController P5B8", "MultiTrainController P5B8");
            }
            {
                ControllerProfile profile = new ControllerProfile();
                profile.IsTwoHandle = false;
                profile.CalcDuplicated();
                profile.KeyMap.Add(4, ButtonFeature.Horn);//B
                profile.KeyMap.Add(0, ButtonFeature.Ats0);//ATS
                profile.KeyMap.Add(2, ButtonFeature.Ats1);//A Shallow
                profile.KeyMap.Add(3, ButtonFeature.Ats2);//A Deep
                profile.KeyMap.Add(5, ButtonFeature.Ats3);//C
                profile.KeyMap.Add(1, ButtonFeature.Ats4);//D
                profile.KeyMap.Add(7, ButtonFeature.Pause);//SELECT
                profile.KeyMap.Add(6, ButtonFeature.Fastforward);//START
                profile.KeyMap.Add(12, ButtonFeature.ReverserForward);//Reverser Front
                profile.KeyMap.Add(13, ButtonFeature.ReverserCenter);//Reverser Center
                profile.KeyMap.Add(14, ButtonFeature.ReverserBackward);//Reverser Back
                Profiles.Add("MultiTrainController P4B6", profile);
                ProfileMap.Add("MultiTrainController P4B6", "MultiTrainController P4B6");
            }
            {
                ControllerProfile profile = new ControllerProfile();
                profile.IsTwoHandle = false;
                profile.CalcDuplicated();
                profile.KeyMap.Add(4, ButtonFeature.Horn);//B
                profile.KeyMap.Add(0, ButtonFeature.Ats0);//ATS
                profile.KeyMap.Add(2, ButtonFeature.Ats1);//A Shallow
                profile.KeyMap.Add(3, ButtonFeature.Ats2);//A Deep
                profile.KeyMap.Add(5, ButtonFeature.Ats3);//C
                profile.KeyMap.Add(1, ButtonFeature.Ats4);//D
                profile.KeyMap.Add(7, ButtonFeature.Pause);//SELECT
                profile.KeyMap.Add(6, ButtonFeature.Fastforward);//START
                profile.KeyMap.Add(12, ButtonFeature.ReverserForward);//Reverser Front
                profile.KeyMap.Add(13, ButtonFeature.ReverserCenter);//Reverser Center
                profile.KeyMap.Add(14, ButtonFeature.ReverserBackward);//Reverser Back
                Profiles.Add("MultiTrainController P4B7", profile);
                ProfileMap.Add("MultiTrainController P4B7", "MultiTrainController P4B7");
            }
            {
                ControllerProfile profile = new ControllerProfile();
                profile.IsTwoHandle = false;
                profile.CalcDuplicated();
                profile.KeyMap.Add(4, ButtonFeature.Horn);//B
                profile.KeyMap.Add(0, ButtonFeature.Ats0);//ATS
                profile.KeyMap.Add(2, ButtonFeature.Ats1);//A Shallow
                profile.KeyMap.Add(3, ButtonFeature.Ats2);//A Deep
                profile.KeyMap.Add(5, ButtonFeature.Ats3);//C
                profile.KeyMap.Add(1, ButtonFeature.Ats4);//D
                profile.KeyMap.Add(7, ButtonFeature.Pause);//SELECT
                profile.KeyMap.Add(6, ButtonFeature.Fastforward);//START
                profile.KeyMap.Add(12, ButtonFeature.ReverserForward);//Reverser Front
                profile.KeyMap.Add(13, ButtonFeature.ReverserCenter);//Reverser Center
                profile.KeyMap.Add(14, ButtonFeature.ReverserBackward);//Reverser Back
                Profiles.Add("MultiTrainController P5B5", profile);
                ProfileMap.Add("MultiTrainController P5B5", "MultiTrainController P5B5");
            }
            AlertNoControllerFound = true;
            CheckUpdates = true;
        }

        public void SaveToXml()
        {
            if (!Directory.Exists(_directory)) Directory.CreateDirectory(_directory);

            string json = JsonConvert.SerializeObject(this);
            using (FileStream fs = new FileStream(Path.Combine(_directory, _filename), FileMode.Create)) // ファイルを開く
            {
                StreamWriter writer = new StreamWriter(fs);
                writer.Write(json);
                writer.Close();
            }
            if (!Directory.Exists(Path.Combine(_directory, _profileDirectory))) Directory.CreateDirectory(Path.Combine(_directory, _profileDirectory));
            foreach (string name in removeProfilesList)
            {
                File.Delete(Path.Combine(_directory, _profileDirectory + name + ".json"));
                File.Delete(Path.Combine(_directory, _profileDirectory + GetSHA256Hash(name) + ".json"));
            }
            foreach (string name in Profiles.Keys)
            {
                ControllerProfile profile = Profiles[name];
                SaveProfileToXml(profile);
            }
        }

        public void SaveProfileToXml(ControllerProfile profile)
        {
            string json = JsonConvert.SerializeObject(profile);

            using (FileStream fs = new FileStream(Path.Combine(_directory, _profileDirectory + GetSHA256Hash(profile.Name) + ".json"), FileMode.Create)) // ファイルを開く
            {
                StreamWriter writer = new StreamWriter(fs);
                writer.Write(json);
                writer.Close();
            }
        }

        public string GetProfileSavePath(ControllerProfile profile)
        {
            return Path.Combine(_directory, _profileDirectory + GetSHA256Hash(profile.Name) + ".json");
        }

        public string GetProfileDirectory()
        {
            return Path.Combine(_directory, _profileDirectory);
        }

        public static Settings LoadFromXml(string directory)
        {
            Settings settings;
            try
            {
                settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(Path.Combine(directory, _filename)), new JsonSerializerSettings());

            }
            catch
            {
                settings = new Settings();
            }

            settings._directory = directory;
            string key = "";
            if (!Directory.Exists(Path.Combine(directory, _profileDirectory))) Directory.CreateDirectory(Path.Combine(directory, _profileDirectory));
            foreach (string file in Directory.GetFiles(Path.Combine(directory, _profileDirectory)))
            {
                if(file.EndsWith(".json"))
                {
                    try
                    {
                        ControllerProfile profile = JsonConvert.DeserializeObject<ControllerProfile>(File.ReadAllText(file));
                        key = Path.GetFileNameWithoutExtension(file);
                        if (settings.Profiles.ContainsKey(key))
                        {
                            settings.Profiles.Remove(key);
                        }
                        if (profile.Name == null)
                        {
                            profile.Name = key;
                        }
                        if (!key.Equals(GetSHA256Hash(key)))
                        {
                            File.Delete(Path.Combine(directory, _profileDirectory + key + ".json"));
                            settings.SaveProfileToXml(profile);
                        }
                        if (settings.Profiles.ContainsKey(profile.Name))
                        {
                            settings.Profiles.Remove(profile.Name);
                        }
                        settings.Profiles.Add(profile.Name, profile);
                        profile.CalcDuplicated();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("プロファイルの読み込みに失敗しました。\n" + file + "\n" + e.Message, "NumerousControllerInterface");
                    }
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
                MessageBox.Show(name + " のプロファイル " + settings.ProfileMap[name] + " が見つからなかったため " + key + "に変更しました。", "NumerousControllerInterface");
                settings.ProfileMap[name] = key;
            }

            return settings;
        }

        public static string GetSHA256Hash(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            byte[] bs = sha256.ComputeHash(bytes);
            sha256.Clear();

            StringBuilder result = new StringBuilder();
            foreach (byte b in bs)
            {
                result.Append(b.ToString("x2"));
            }
            return result.ToString();
        }

        public ControllerProfile GetProfile(NCIController controller)
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
