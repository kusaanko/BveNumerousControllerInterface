using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace Kusaanko.Bvets.NumerousControllerInterface.Controller
{
    public class DenshadeGoShinkansen : NCIController
    {
        private UsbDevice _device;
        private UsbEndpointReader _reader;
        private int _power;
        private int _break;
        private bool[] _buttons;
        private bool _loop;
        private Reverser _revPos;
        private byte[] _SendData;
        public static List<NCIController> Get()
        {
            List<NCIController> controllers = new List<NCIController>();
            foreach (UsbRegistry registry in UsbDevice.AllDevices)
            {
                if (registry.Vid == 0x0AE4 && registry.Pid == 0x0005)
                {
                    UsbDevice device;
                    if(registry.Open(out device))
                    {
                        IUsbDevice wholeUsbDevice = device as IUsbDevice;
                        if (!ReferenceEquals(wholeUsbDevice, null))
                        {
                            wholeUsbDevice.SetConfiguration(1);
                            wholeUsbDevice.ClaimInterface(0);
                        }
                        controllers.Add(new DenshadeGoShinkansen(device));
                        break;
                    }
                }
            }
            return controllers;
        }
        public DenshadeGoShinkansen(UsbDevice device)
        {
            this._device = device;
            _SendData = new byte[8];
            _reader = device.OpenEndpointReader(ReadEndpointID.Ep01);
            _buttons = new bool[11];
            _loop = true;
            new Thread(() =>
            {
                while (_loop)
                {
                    byte[] buffer = new byte[6];
                    int len;
                    ErrorCode code = _reader.Read(buffer, 2000, out len);
                    if (code != ErrorCode.None) continue;
                    //ハンドル
                    int power = buffer[1];
                    if (power != 0xFF)
                    {
                        _power = (int)(Math.Round(power / 18d) - 1);
                    }
                    // EBから順に1,2,3,4,...
                    int brake = buffer[0];
                    if (brake != 0xFF)
                    {
                        _break = (int)(Math.Round(brake / 28d) - 1);
                    }
                    //ボタン
                    int button = buffer[4];
                    // D
                    _buttons[3] = (button & 0x1) > 0;
                    // C
                    _buttons[2] = (button & 0x2) > 0;
                    // B
                    _buttons[1] = (button & 0x4) > 0;
                    // A
                    _buttons[0] = (button & 0x8) > 0;
                    // SELECT
                   _buttons[5] = (button & 0x10) > 0;
                    // START
                   _buttons[4] = (button & 0x20) > 0;
                    // 十字キー
                    int hat = buffer[3];
                    if (hat == 0x00)
                    {
                        _buttons[6] = true;
                    }
                    else if (hat == 0x01)
                    {
                        _buttons[6] = true;
                        _buttons[7] = true;
                    }
                    else if (hat == 0x02)
                    {
                        _buttons[7] = true;
                    }
                    else if (hat == 0x03)
                    {
                        _buttons[7] = true;
                        _buttons[8] = true;
                    }
                    else if (hat == 0x04)
                    {
                        _buttons[8] = true;
                    }
                    else if (hat == 0x05)
                    {
                        _buttons[8] = true;
                        _buttons[9] = true;
                    }
                    else if (hat == 0x06)
                    {
                        _buttons[9] = true;
                    }
                    else if (hat == 0x07)
                    {
                        _buttons[9] = true;
                        _buttons[6] = true;
                    }
                    else
                    {
                        _buttons[6] = false;
                        _buttons[7] = false;
                        _buttons[8] = false;
                        _buttons[9] = false;
                    }
                    int horn = buffer[2];
                    _buttons[10] = horn == 0x00;
                }
                if (device.IsOpen)
                {
                    device.Close();
                }
            }).Start();
        }

        public override string[] GetButtonNames()
        {
            return new string[] { "A", "B", "C", "D", "START", "SELECT", "Up", "Right", "Down", "Left", "Horn" };
        }


        public override void Dispose()
        {
            _loop = false;
            int transferLength;
            var packet = new UsbSetupPacket(0x40, 0x09, 0x301, 0x0, 0x8);
            for (int i = 0; i < _SendData.Length; i++)
            {
                _SendData[i] = 0x00; // Reset the send data
            }
            _device.ControlTransfer(ref packet, new byte[_SendData.Length], _SendData.Length, out transferLength);
        }

        public override bool IsDisposed()
        {
            return !_loop;
        }

        public override int GetPowerCount()
        {
            return 13;
        }

        public override int GetPower()
        {
            return _power;
        }

        public override int GetBreakCount()
        {
            return 8;
        }

        public override int GetBreak()
        {
            return _break;
        }

        public override bool HasReverser()
        {
            return true;
        }

        public override Reverser GetReverser()
        {
            return _revPos;
        }

        public override bool[] GetButtons()
        {
            return _buttons;
        }

        public override string GetControllerType()
        {
            return "LibUsbDotNet VID:0x" + Convert.ToString(_device.UsbRegistryInfo.Vid, 16).ToUpper() + " PID:0x" + Convert.ToString(_device.UsbRegistryInfo.Pid, 16).ToUpper() + " Rev:" + _device.UsbRegistryInfo.Rev;
        }

        public override string GetName()
        {
            return "電車でGO!新幹線(PS2)";
        }

        public override int[] GetSliders()
        {
            return null;
        }
        public override bool HasOutputs()
        {
            return true;
        }

        public override Dictionary<string, OutputType> GetOutputs()
        {
            Dictionary<string, OutputType> outputs = new Dictionary<string, OutputType>
            {
                { "戸閉灯", OutputType.Bool },
                { "速度計", OutputType.Int },
                { "ATC", OutputType.Int },
                { "速度ゲージ", OutputType.Int },
                { "ATCゲージ", OutputType.Int }
            };
            return outputs;
        }

        public override Dictionary<string, OutputHint> GetOutputHints()
        {
            Dictionary<string, OutputHint> outputs = new Dictionary<string, OutputHint>
            {
                { "戸閉灯", OutputHint.DoorLamp },
                { "速度計", OutputHint.SpeedMeter },
                { "ATC", OutputHint.ATC },
            };
            return outputs;
        }

        public override void SetOutput(string key, object value)
        {
            switch (key)
            {
                case "戸閉灯":
                    _SendData[2] = (byte)((((bool)value) ? 0x80 : 0x00) | _SendData[2] & 0xF);
                    break;
                case "速度計":
                    int speed = (int)value;
                    int speed1 = (speed % 1000) / 100;
                    int speed2 = (speed % 100) / 10;
                    int speed3 = speed % 10;
                    _SendData[4] = (byte)(((speed2 << 4) | speed3) & 0xFF);
                    _SendData[5] = (byte)(speed1 & 0xFF);
                    break;
                case "ATC":
                    int atc = (int)value;
                    int atc1 = (atc % 1000) / 100;
                    int atc2 = (atc % 100) / 10;
                    int atc3 = atc % 10;
                    _SendData[6] = (byte)(((atc2 << 4) | atc3) & 0xFF);
                    _SendData[7] = (byte)(atc1 & 0xFF);
                    break;
                case "速度ゲージ":
                    int gauge = (int)Math.Ceiling((int)value / 15d);
                    if (gauge > 0x16) gauge = 0x16;
                    if (gauge < 0) gauge = 0;
                    _SendData[3] = (byte)gauge;
                    break;
                case "ATCゲージ":
                    int val = (int)value;
                    if (val > 10) val = 10;
                    if (val < 0) val = 0;
                    _SendData[2] = (byte)((_SendData[2] & 0xF0) | val);
                    break;
            }
        }

        public override void SendOutput()
        {
            int transferLength = 0;
            var packet = new UsbSetupPacket(0x40, 0x09, 0x301, 0x0, 0x8);
            _device.ControlTransfer(ref packet, _SendData, _SendData.Length, out transferLength);
        }
    }
}
