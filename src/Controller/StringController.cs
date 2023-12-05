using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace Kusaanko.Bvets.NumerousControllerInterface.Controller
{
    // 文字列操作によるコントローラー。COMポートなど。
    public abstract class StringController : NCIController
    {
        private StringControllerSettings _settings;
        private bool[] _buttons;
        private int _power;
        private int _break;
        private Reverser _reverser;
        private bool _isInitializationEnded;
        public StringController()
        {
            _isInitializationEnded = false;
        }

        public void SetSettings(StringControllerSettings settings)
        {
            _settings = settings;
        }

        public void ExecInitializationCommands(string commands)
        {
            commands = commands.Replace("\r\n", "\n");
            commands = commands.Replace("\r", "\n");
            string[] commandList = commands.Split('\n');
            foreach (string commandE in commandList)
            {
                string command = commandE;
                if (_settings != null)
                {
                    string replace = _settings.InputCommandReplaceDictionary[command];
                    if (replace != null)
                    {
                        command = replace;
                    }
                }
                string cmd = command.Substring(0, command.IndexOf(' '));
                string[] args = command.Substring(command.IndexOf(' ') + 1).Split(',');
                switch (cmd)
                {
                    case "NCIName":
                        if (args.Length > 0)
                        {
                            _settings.Name = args[0];
                        }
                        break;
                    case "NCIPowerCount":
                        if (args.Length > 0)
                        {
                            _settings.PowerCount = int.Parse(args[0]);
                        }
                        break;
                    case "NCIBreakCount":
                        if (args.Length > 0)
                        {
                            _settings.BreakCount = int.Parse(args[0]);
                        }
                        break;
                    case "NCIHasReverser":
                        if (args.Length > 0)
                        {
                            _settings.HasReverser = false;
                            if (args[0] == "True")
                            {
                                _settings.HasReverser = true;
                            }
                        }
                        break;
                    case "NCIButtons":
                        if (args.Length > 0)
                        {
                            _settings.ButtonCount = int.Parse(args[0]);
                            _settings.ButtonNames = new string[int.Parse(args[0])];
                        }
                        break;
                    case "NCIButtonNames":
                        for (int i = 0; i < args.Length; i++)
                        {
                            _settings.ButtonNames[i] = args[i].ToString();
                        }
                        break;
                    case "NCIButtonName":
                        if (args.Length > 1)
                        {
                            _settings.ButtonNames[int.Parse(args[0])] = args[1].ToString();
                        }
                        break;
                    case "NCIInitEnd":
                        if (args.Length > 0)
                        {
                            _isInitializationEnded = true;
                        }
                        break;
                }
            }
        }

        public void EnterInitializationMode()
        {
            _isInitializationEnded = true;
        }

        public bool IsInitializationEnded()
        {
            return _isInitializationEnded;
        }

        public void ExecCommands(string commands)
        {
            commands = commands.Replace("\r\n", "\n");
            commands = commands.Replace("\r", "\n");
            string[] commandList = commands.Split('\n');
            foreach (string commandE in commandList)
            {
                string command = commandE;
                // コマンドの置換
                if (_settings.InputCommandReplaceDictionary.ContainsKey(command))
                {
                    command = _settings.InputCommandReplaceDictionary[command];
                }
                string func = command;
                string argsString = "";
                if (func.Contains(" "))
                {
                    func = func.Substring(0, func.IndexOf(" "));
                    argsString = command.Substring(func.IndexOf(" ") + 1);
                }
                string[] args = argsString.Split(',');
                try
                {
                    if (func == "NCIPower")
                    {
                        _power = Convert.ToInt32(args[0]);
                        if (_power > _settings.PowerCount && args.Length == 1)
                        {
                            _power = _settings.PowerCount;
                        }
                    } else if (func == "NCIBreak" && args.Length == 1)
                    {
                        _break = Convert.ToInt32(args[0]);
                        if (_break > _settings.BreakCount)
                        {
                            _break = _settings.BreakCount;
                        }
                        if (_break < 0)
                        {
                            _break = 0;
                        }
                    } else if (func == "NCIButton" && args.Length == 2)
                    {
                        string name = args[0];
                        string value = args[1];
                        int button = GetButtonIndex(name);
                        _buttons[button] = value == "True";
                    } else if (func == "NCIReverser" && args.Length == 1)
                    {
                        if(args[0] == "F")
                        {
                            _reverser = Reverser.FORWARD;
                        } else if (args[0] == "C")
                        {
                            _reverser = Reverser.CENTER;
                        } else if (args[0] == "B")
                        {
                            _reverser = Reverser.BACKWARD;
                        }
                    }
                }
                catch
                {
                    // パース中に問題があった場合は無視
                }
            }
        }

        private string GetButtonName(int index)
        {
            string[] names = GetButtonNames();
            if (names.Length > index && names[index] != null && names[index] != "")
            {
                return names[index];
            }
            return index + "";
        }

        private int GetButtonIndex(string name)
        {
            string[] names = GetButtonNames();
            if (names.Length > 0)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    if (names[i] == name)
                    {
                        return i;
                    }
                }
            }
            try
            {
                return Convert.ToInt32(name);
            }
            catch
            {
                // 入力が整数値でない
                return -1;
            }
        }

        public override bool[] GetButtons()
        {
            return _buttons;
        }

        public override string GetName()
        {
            return _settings.Name;
        }

        public override int[] GetSliders()
        {
            return new int[0];
        }

        public override int GetPowerCount()
        {
            return _settings.PowerCount;
        }

        public override int GetBreakCount()
        {
            return _settings.BreakCount;
        }

        public override int GetPower()
        {
            return _power;
        }

        public override int GetBreak()
        {
            return _break;
        }

        public override Reverser GetReverser()
        {
            return _reverser;
        }

        public override bool HasReverser()
        {
            return _settings.HasReverser;
        }

        public override string[] GetButtonNames()
        {
            return _settings.ButtonNames;
        }
    }

    public class StringControllerSettings
    {
        public string Name;
        public int PowerCount;
        public int BreakCount;
        public bool HasReverser;
        public int ButtonCount;
        public string[] ButtonNames;
        public string OnInit;
        public Dictionary<string, string> InputCommandReplaceDictionary;
        public Dictionary<string, string> OutputCommandReplaceDictionary;
    }
}
