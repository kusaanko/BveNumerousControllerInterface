using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace Kusaanko.Bvets.NumerousControllerInterface.Controller
{
    public class COMController : StringController
    {
        private static Dictionary<string, COMController> _controllers;
        private SerialPort _serialPort;
        private COMControllerSettings _settings;
        private bool _loop;
        private bool _loopInit;
        public static bool IsUpdateNeeded = false;

        public static List<NCIController> Get()
        {
            IsUpdateNeeded = false;
            if (_controllers == null)
            {
                _controllers = new Dictionary<string, COMController>();
            }
            List<NCIController> controllers = new List<NCIController>();
            List<string> enabled = new List<string>();
            foreach (string port in NumerousControllerInterface.SettingsInstance.EnabledComPorts)
            {
                enabled.Add(port);
                if (SerialPort.GetPortNames().Contains(port))
                {
                    if (NumerousControllerInterface.SettingsInstance.COMControllerSettingMap.ContainsKey(port))
                    {
                        if (!_controllers.ContainsKey(port))
                        {
                            if (NumerousControllerInterface.SettingsInstance.COMControllerSettings.ContainsKey(NumerousControllerInterface.SettingsInstance.COMControllerSettingMap[port])) {
                                _controllers.Add(port, new COMController(port, NumerousControllerInterface.SettingsInstance.COMControllerSettings[NumerousControllerInterface.SettingsInstance.COMControllerSettingMap[port]]));
                            } else
                            {
                                NumerousControllerInterface.SettingsInstance.COMControllerSettingMap.Remove(port);
                            }
                        }
                        else
                        {
                            COMController comController = _controllers[port];
                            if (!comController._loopInit)
                            {
                                controllers.Add(comController);
                            }
                        }
                    }
                }
            }
            foreach (string port in SerialPort.GetPortNames())
            {
                if (!enabled.Contains(port) && _controllers.ContainsKey(port))
                {
                    _controllers[port]._Dispose();
                    _controllers.Remove(port);
                }
            }
            return controllers;
        }

        public static void StopCOMPort(string port)
        {
            if (_controllers.ContainsKey(port))
            {
                _controllers[port]._Dispose();
            }
        }

        public static void DisposeAll()
        {
            List<COMController> controllers = new List<COMController>();
            foreach (COMController controller in _controllers.Values)
            {
                controllers.Add(controller);
            }
            foreach (COMController controller in controllers)
            {
                controller._Dispose();
            }
        }

        public static int GetCounterForUpdateControllerList()
        {
            int initializeEnded = 0;
            foreach (COMController controller in _controllers.Values)
            {
                if (!controller._loopInit) { initializeEnded++; }
            }
            return SerialPort.GetPortNames().Length + initializeEnded + (IsUpdateNeeded ? 100 : 0);
        }

        public COMController(string serialPortName, COMControllerSettings settings)
        {
            SerialPort serialPort = new SerialPort
            {
                PortName = serialPortName,
                BaudRate = settings.BaudRate,
                DtrEnable = settings.DtrEnable,
                RtsEnable = settings.RtsEnable
            };
            _serialPort = serialPort;
            _settings = settings;
            _loopInit = true;
            serialPort.Open();
            Thread t = new Thread(new ThreadStart(InitializationRunFrame));
            t.Start();
        }

        private void InitializationRunFrame()
        {
            string initializationLines = null;
            // 初期化
            if (_settings == null)
            {
                _serialPort.WriteLine("NCIInit");
            } else
            {
                if (_settings.IsNotSupported && _settings.OnInit != null && _settings.OnInit.Length > 0)
                {
                    initializationLines = _settings.OnInit;
                    _loopInit = false;
                } else
                {
                    _serialPort.WriteLine("NCIInit");
                }
            }
            StringControllerSettings settings = new StringControllerSettings();
            if (_settings != null)
            {
                settings.OnInit = _settings.OnInit;
                settings.InputCommandReplaceDictionary = _settings.InputCommandReplaceDictionary;
                settings.OutputCommandReplaceDictionary = _settings.OutputCommandReplaceDictionary;
                settings.Name = "COM Port Controller " + _serialPort.PortName;
                settings.ButtonCount = _settings.ButtonCount;
                settings.PowerCount = _settings.PowerCount;
                settings.BreakCount = _settings.BreakCount;
                settings.HasReverser = _settings.HasReverser;
                settings.ButtonNames = _settings.ButtonNames;
            }
            else
            {
                settings.OnInit = null;
                settings.InputCommandReplaceDictionary = new Dictionary<string, string>();
                settings.OutputCommandReplaceDictionary = new Dictionary<string, string>();
            }
            SetSettings(settings);
            if (initializationLines != null)
            {
                ExecInitializationCommands(initializationLines);
            } else
            {
                while (_loopInit && !IsInitializationEnded())
                {
                    string line = _serialPort.ReadLine();
                    ExecInitializationCommands(line);
                }
            }
            _loopInit = false;
            _loop = true;
            Thread t = new Thread(new ThreadStart(RunFrame));
            t.Start();
        }

        public void RunFrame()
        {
            try
            {
                while (_loop)
                {
                    if (IsNeedEmptyRequest())
                    {
                        _serialPort.WriteLine("NCINone");
                    }
                    string line = _serialPort.ReadLine();
                    try
                    {
                        ExecCommands(line);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            } catch (IOException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            _loop = false;
        }

        public override void Dispose()
        {
        }

        private void _Dispose()
        {
            _serialPort.Write("NCIClose\n");
            _serialPort.Close();
            _loop = false;
            _controllers.Remove(_serialPort.PortName);
        }

        public override string GetControllerType()
        {
            return "COM Port " + _serialPort.PortName;
        }

        public override bool IsDisposed()
        {
            return !_serialPort.IsOpen;
        }
    }

    [DataContract]
    public class COMControllerSettings
    {
        [DataMember]
        public string Name;
        [DataMember]
        public int PowerCount;
        [DataMember]
        public int BreakCount;
        [DataMember]
        public bool HasReverser;
        [DataMember]
        public int ButtonCount;
        [DataMember]
        public string[] ButtonNames;
        [DataMember]
        public string OnInit;
        [DataMember]
        public int BaudRate = 9600;
        [DataMember]
        public bool DtrEnable = true;
        [DataMember]
        public bool RtsEnable = true;
        [DataMember]
        public bool IsNotSupported = false;
        [DataMember]
        public Dictionary<string, string> InputCommandReplaceDictionary;
        [DataMember]
        public Dictionary<string, string> OutputCommandReplaceDictionary;

        internal COMControllerSettings Clone()
        {
            COMControllerSettings settings = new COMControllerSettings();
            settings.Name = Name;
            settings.PowerCount = PowerCount;
            settings.BreakCount = BreakCount;
            settings.ButtonCount = ButtonCount;
            if (ButtonNames != null)
            {
                settings.ButtonNames = (string[]) ButtonNames.Clone();
            }
            if (OnInit != null)
            {
                settings.OnInit = (string) OnInit.Clone();
            }
            settings.BaudRate = BaudRate;
            settings.DtrEnable = DtrEnable;
            settings.RtsEnable = RtsEnable;
            settings.IsNotSupported = IsNotSupported;
            settings.InputCommandReplaceDictionary = CloneDictionary(InputCommandReplaceDictionary);
            settings.OutputCommandReplaceDictionary = CloneDictionary(OutputCommandReplaceDictionary);
            return settings;
        }

        private Dictionary<string, string> CloneDictionary(Dictionary<string, string> from)
        {
            if (from == null)
            {
                return null;
            }
            Dictionary<string, string> ret = new Dictionary<string, string>();
            foreach (string key in from.Keys)
            {
                ret.Add(key, (string) from[key].Clone());
            }
            return ret;
        }
    }
}
