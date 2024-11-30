using System.IO;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Kusaanko.Bvets.NumerousControllerInterface.Controller;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public class Settings
    {
        private const string _filename = "Kusaanko.NumerousControllerInterface.xml";
        private const string _profileDirectory = "Kusaanko.NumerousControllerInterface.Profiles\\";
        private const string _COMSettingDirectory = "Kusaanko.NumerousControllerInterface.Profiles\\COMPort\\";
        private const string _pluginDirectry = "Kusaanko.NumerousControllerInterface.Plugins\\";
        private string _directory = string.Empty;

        [IgnoreDataMember]
        public Dictionary<string, ControllerProfile> Profiles = new Dictionary<string, ControllerProfile>();
        [DataMember]
        public Dictionary<string, bool> IsEnabled = new Dictionary<string, bool>();
        [DataMember]
        public Dictionary<string, string> ProfileMap = new Dictionary<string, string>();
        [IgnoreDataMember]
        public List<string> removeProfilesList = new List<string>();
        [IgnoreDataMember]
        public HashSet<string> removeCOMControllerProfilesList = new HashSet<string>();
        [DataMember]
        public bool AlertNoControllerFound;
        [DataMember]
        public bool CheckUpdates;
        [DataMember]
        public int IgnoreUpdate;
        [IgnoreDataMember]
        public Dictionary<string, COMControllerSettings> COMControllerSettings = new Dictionary<string, COMControllerSettings>();
        [IgnoreDataMember]
        public List<string> removeCOMSettingsList = new List<string>();
        [DataMember]
        public HashSet<string> EnabledComPorts = new HashSet<string>();
        [DataMember]
        public Dictionary<string, string> COMControllerSettingMap = new Dictionary<string, string>();

        public Settings()
        {
            {
                ControllerProfile profile = new ControllerProfile("JC-PS101U PS用電車でGO!コントローラー(ワン,ツーハンドル)");
                profile.IsTwoHandle = true;
                profile.PowerAxises = new int[] { 21 };
                profile.SetPowerAxisStatus(new int[,] {
                    { -1000 },
                    { 1000 },
                    { 1000 },
                    { -1000 },
                    { -1000 },
                    { -8 }
                });
                profile.PowerButtons = new int[] { 0 };
                profile.SetPowerButtonStatus(new bool[,] {
                    { false },
                    { true },
                    { false },
                    { true },
                    { false },
                    { true }
                });
                profile.BreakButtons = new int[] { 4, 5, 6, 7};
                profile.SetBreakButtonStatus(new bool[,] { 
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
                });
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
                ControllerProfile profile = new ControllerProfile("JC-PS101U DualShock2");
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
                ControllerProfile profile = new ControllerProfile("TCPP-20009 PS2用電車でGO!コントローラー TYPE2");
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
                ControllerProfile profile = new ControllerProfile("MultiTrainController P5B8");
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
                ControllerProfile profile = new ControllerProfile("MultiTrainController P4B6");
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
                Profiles.Add("MultiTrainController P4B6", profile);
                ProfileMap.Add("MultiTrainController P4B6", "MultiTrainController P4B6");
            }
            {
                ControllerProfile profile = new ControllerProfile("MultiTrainController P4B7");
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
                Profiles.Add("MultiTrainController P4B7", profile);
                ProfileMap.Add("MultiTrainController P4B7", "MultiTrainController P4B7");
            }
            {
                ControllerProfile profile = new ControllerProfile("MultiTrainController P5B5");
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
                Profiles.Add("MultiTrainController P5B5", profile);
                ProfileMap.Add("MultiTrainController P5B5", "MultiTrainController P5B5");
            }
            {
                ControllerProfile profile = new ControllerProfile("MultiTrainController P5B7");
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
                Profiles.Add("MultiTrainController P5B7", profile);
                ProfileMap.Add("MultiTrainController P5B7", "MultiTrainController P5B7");
            }
            AlertNoControllerFound = true;
            CheckUpdates = true;
            IgnoreUpdate = 0;
        }

        public void SaveToXml()
        {
            if (!Directory.Exists(_directory)) Directory.CreateDirectory(_directory);

            // XMLに設定を保存
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new UTF8Encoding(false)
            };

            DataContractSerializer serializer = new DataContractSerializer(typeof(Settings));
            using (XmlWriter writer = XmlWriter.Create(Path.Combine(_directory, _filename), settings))
            {
                serializer.WriteObject(writer, this);
            }
            if (!Directory.Exists(Path.Combine(_directory, _profileDirectory))) Directory.CreateDirectory(Path.Combine(_directory, _profileDirectory));
            foreach (string name in removeProfilesList)
            {
                File.Delete(Path.Combine(_directory, _profileDirectory + name + ".xml"));
                File.Delete(Path.Combine(_directory, _profileDirectory + GetSHA256Hash(name) + ".xml"));
            }
            foreach (string name in Profiles.Keys)
            {
                ControllerProfile profile = Profiles[name];
                SaveProfileToXml(profile);
            }
            if (!Directory.Exists(Path.Combine(_directory, _COMSettingDirectory))) Directory.CreateDirectory(Path.Combine(_directory, _COMSettingDirectory));
            foreach (string name in removeCOMSettingsList)
            {
                File.Delete(Path.Combine(_directory, _COMSettingDirectory + name + ".xml"));
                File.Delete(Path.Combine(_directory, _COMSettingDirectory + GetSHA256Hash(name) + ".xml"));
            }
            foreach (string name in COMControllerSettings.Keys)
            {
                COMControllerSettings setting = COMControllerSettings[name];
                SaveCOMSettingToXml(setting);
            }
        }

        public void SaveProfileToXml(ControllerProfile profile)
        {

            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new UTF8Encoding(false)
            };

            DataContractSerializer serializer = new DataContractSerializer(typeof(ControllerProfile));
            using (XmlWriter writer = XmlWriter.Create(Path.Combine(_directory, _profileDirectory + GetSHA256Hash(profile.Name) + ".xml"), settings))
            {
                serializer.WriteObject(writer, profile);
            }
        }

        public string GetProfileSavePath(ControllerProfile profile)
        {
            return Path.Combine(_directory, _profileDirectory + GetSHA256Hash(profile.Name) + ".xml");
        }

        public string GetProfileDirectory()
        {
            return Path.Combine(_directory, _profileDirectory);
        }

        public void SaveCOMSettingToXml(COMControllerSettings setting)
        {

            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new UTF8Encoding(false)
            };

            DataContractSerializer serializer = new DataContractSerializer(typeof(COMControllerSettings));
            using (XmlWriter writer = XmlWriter.Create(Path.Combine(_directory, _COMSettingDirectory + GetSHA256Hash(setting.Name) + ".xml"), settings))
            {
                serializer.WriteObject(writer, setting);
            }
        }

        public string GetCOMSettingSavePath(ControllerProfile profile)
        {
            return Path.Combine(_directory, _COMSettingDirectory + GetSHA256Hash(profile.Name) + ".xml");
        }

        public string GetCOMSettingDirectory()
        {
            return Path.Combine(_directory, _COMSettingDirectory);
        }

        public static Settings LoadFromXml(string directory)
        {
            var xmlReaderSettings = new XmlReaderSettings();
            
            Settings settings;
            try
            {
                DataContractSerializer settingsSerializer = new DataContractSerializer(typeof(Settings));
                using (XmlReader reader = XmlReader.Create(Path.Combine(directory, _filename), xmlReaderSettings))
                {
                    settings = (Settings) settingsSerializer.ReadObject(reader);
                }
            }
            catch
            {
                settings = new Settings();
            }
            settings._directory = directory;
            string key = "";
            if (!Directory.Exists(Path.Combine(directory, _profileDirectory))) Directory.CreateDirectory(Path.Combine(directory, _profileDirectory));
            if (!Directory.Exists(Path.Combine(directory, _COMSettingDirectory))) Directory.CreateDirectory(Path.Combine(directory, _COMSettingDirectory));

            // プロファイル
            DataContractSerializer controllerProfileSerializer = new DataContractSerializer(typeof(ControllerProfile));

            foreach (string file in Directory.GetFiles(Path.Combine(directory, _profileDirectory)))
            {
                if(file.EndsWith(".xml"))
                {
                    try
                    {
                        ControllerProfile profile;
                        using (XmlReader writer = XmlReader.Create(file, xmlReaderSettings))
                        {
                            profile = (ControllerProfile) controllerProfileSerializer.ReadObject(writer);
                        }
                        if (profile != null)
                        {
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
                                File.Delete(Path.Combine(directory, _profileDirectory + key + ".xml"));
                                settings.SaveProfileToXml(profile);
                            }
                            if (settings.Profiles.ContainsKey(profile.Name))
                            {
                                settings.Profiles.Remove(profile.Name);
                            }
                            // ButtonFeature
                            List<int> buttons = new List<int>(profile.KeyMap.Keys);
                            foreach (int button in buttons)
                            {
                                string featureId = profile.KeyMap[button].Id;
                                if (ButtonFeature.Features.ContainsKey(featureId))
                                {
                                    profile.KeyMap[button] = ButtonFeature.Features[featureId];
                                } else
                                {
                                    profile.KeyMap.Remove(button);
                                }
                            }
                            settings.Profiles.Add(profile.Name, profile);
                            profile.CalcDuplicated();
                        }
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
            foreach (string name in changeList)
            {
                MessageBox.Show(name + " のプロファイルが見つからなかったため無効化しました。", "NumerousControllerInterface");
                settings.ProfileMap.Remove(name);
            }
            // COMポート
            DataContractSerializer COMSettingSerializer = new DataContractSerializer(typeof(COMControllerSettings));

            foreach (string file in Directory.GetFiles(Path.Combine(directory, _COMSettingDirectory)))
            {
                if (file.EndsWith(".xml"))
                {
                    try
                    {
                        COMControllerSettings setting;
                        using (XmlReader writer = XmlReader.Create(file, xmlReaderSettings))
                        {
                            setting = (COMControllerSettings)COMSettingSerializer.ReadObject(writer);
                        }
                        if (setting != null)
                        {
                            key = Path.GetFileNameWithoutExtension(file);
                            if (settings.COMControllerSettings.ContainsKey(key))
                            {
                                settings.COMControllerSettings.Remove(key);
                            }
                            if (setting.Name == null)
                            {
                                setting.Name = key;
                            }
                            if (!key.Equals(GetSHA256Hash(key)))
                            {
                                File.Delete(Path.Combine(directory, _COMSettingDirectory + key + ".xml"));
                                settings.SaveCOMSettingToXml(setting);
                            }
                            if (settings.COMControllerSettings.ContainsKey(setting.Name))
                            {
                                settings.COMControllerSettings.Remove(setting.Name);
                            }
                            settings.COMControllerSettings.Add(setting.Name, setting);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("プロファイルの読み込みに失敗しました。\n" + file + "\n" + e.Message, "NumerousControllerInterface");
                    }
                }
            }
            changeList = new List<string>();
            foreach (string name in settings.COMControllerSettingMap.Keys)
            {
                if (!settings.COMControllerSettings.ContainsKey(settings.COMControllerSettingMap[name]))
                {
                    changeList.Add(name);
                }
            }
            foreach (string name in changeList)
            {
                MessageBox.Show(name + " のプロファイル " + settings.COMControllerSettingMap[name] + " が見つからなかったためプロファイルを無効化しました。", "NumerousControllerInterface");
                settings.COMControllerSettingMap.Remove(name);
            }
            // プラグイン
            if (NumerousControllerInterface.Plugins.Count == 0)
            {
                if (!Directory.Exists(Path.Combine(directory, _pluginDirectry))) Directory.CreateDirectory(Path.Combine(directory, _pluginDirectry));
                foreach (string file in Directory.GetFiles(Path.Combine(directory, _pluginDirectry), "*.dll"))
                {
                    Debug.WriteLine(file);
                    try
                    {
                        Assembly asm = Assembly.LoadFrom(file);
                        foreach (Type type in asm.GetTypes())
                        {
                            string pluginName = typeof(NumerousControllerPlugin).FullName;
                            if (type.IsClass && type.IsPublic && !type.IsAbstract && type.GetInterface(pluginName) != null)
                            {
                                NumerousControllerPlugin plugin = (NumerousControllerPlugin)asm.CreateInstance(type.FullName);
                                NumerousControllerInterface.Plugins.Add(plugin);
                                plugin.LoadConfig(directory);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
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
            if (!ProfileMap.ContainsKey(controller.GetName())) return null;
            if (!Profiles.ContainsKey(ProfileMap[controller.GetName()])) return null;
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
