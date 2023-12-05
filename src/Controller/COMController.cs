using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kusaanko.Bvets.NumerousControllerInterface.Controller
{
    public class COMController : StringController
    {
        private SerialPort _serialPort;
        private COMControllerSettings _settings;
        private int _powerCount;
        private int _breakCount;
        private int _buttonCount;
        private string[] _buttonNames;
        private bool _hasReverser;
        private string _name;
        private bool _loop;
        private bool _loopInit;

        public COMController(SerialPort serialPort, COMControllerSettings settings)
        {
            _serialPort = serialPort;
            _settings = settings;
            _loopInit = true;
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
                if (_settings.onInit != null)
                {
                    initializationLines = _settings.onInit;
                    _loopInit = false;
                } else
                {
                    _loopInit = false;
                }
                _powerCount = _settings.powerCount;
                _breakCount = _settings.breakCount;
                _buttonCount = _settings.buttonCount;
                _hasReverser = _settings.hasReverser;
            }
            StringControllerSettings settings = new StringControllerSettings();
            if (_settings != null)
            {
                settings.OnInit = _settings.onInit;
                settings.InputCommandReplaceDictionary = _settings.inputCommandReplaceDictionary;
                settings.OutputCommandReplaceDictionary = _settings.outputCommandReplaceDictionary;
                settings.Name = _name;
                settings.ButtonCount = _settings.buttonCount;
                settings.PowerCount = _settings.powerCount;
                settings.BreakCount = _settings.breakCount;
                settings.HasReverser = _settings.hasReverser;
                settings.ButtonNames = _settings.buttonNames;
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
            _loop = true;
            Thread t = new Thread(new ThreadStart(RunFrame));
            t.Start();
        }

        public void RunFrame()
        {
            while (_loop)
            {
                string line = _serialPort.ReadLine();
                ExecCommands(line);
            }
        }

        public override void Dispose()
        {
            _serialPort.Write("NCIClose\n");
            _serialPort.Close();
            _loop = false;
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

    public class COMControllerSettings
    {
        public string name;
        public int powerCount;
        public int breakCount;
        public bool hasReverser;
        public int buttonCount;
        public string[] buttonNames;
        public string onInit;
        public Dictionary<string, string> inputCommandReplaceDictionary;
        public Dictionary<string, string> outputCommandReplaceDictionary;
    }
}
