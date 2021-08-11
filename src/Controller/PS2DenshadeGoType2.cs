using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using LibUsbDotNet.Main;
using LibUsbDotNet;
using System.Threading;

namespace Kusaanko.Bvets.NumerousControllerInterface.Controller
{
    public class PS2DenshadeGoType2 : NCIController
    {
        private static int B_BUTTON = 0x1;
        private static int A_BUTTON = 0x2;
        private static int C_BUTTON = 0x4;
        private static int D_BUTTON = 0x8;
        private static int SELECT_BUTTON = 0x10;
        private static int START_BUTTON = 0x20;
        private UsbDevice _device;
        private UsbEndpointReader _reader;
        private int _power;
        private int _break;
        private bool[] _buttons;
        private bool _loop;
        public static List<NCIController> Get()
        {
            List<NCIController> controllers = new List<NCIController>();
            foreach (UsbRegistry registry in UsbDevice.AllDevices)
            {
                if (registry.Vid == 0x0AE4 && registry.Pid == 0x0004)
                {
                    UsbDevice device;
                    if (registry.Open(out device))
                    {
                        IUsbDevice wholeUsbDevice = device as IUsbDevice;
                        if (!ReferenceEquals(wholeUsbDevice, null))
                        {
                            wholeUsbDevice.SetConfiguration(1);
                            wholeUsbDevice.ClaimInterface(0);
                        }
                        controllers.Add(new PS2DenshadeGoType2(device));
                        break;
                    }
                }
            }
            return controllers;
        }
        public PS2DenshadeGoType2(UsbDevice device)
        {
            this._device = device;
            _reader = device.OpenEndpointReader(ReadEndpointID.Ep01);
            _buttons = new bool[11];
            _loop = true;
            new Thread(() =>
            {
                try
                {
                    while (_loop)
                    {
                        byte[] buffer = new byte[8];
                        int len;
                        ErrorCode code = _reader.Read(buffer, 2000, out len);
                        if (code != ErrorCode.None) continue;
                        //力行
                        switch (buffer[2])
                        {
                            case 0x81:
                                _power = 0;
                                break;
                            case 0x6D:
                                _power = 1;
                                break;
                            case 0x54:
                                _power = 2;
                                break;
                            case 0x3D:
                                _power = 3;
                                break;
                            case 0x21:
                                _power = 4;
                                break;
                            case 0x00:
                                _power = 5;
                                break;
                        }
                        //制動
                        switch (buffer[1])
                        {
                            case 0x79:
                                _break = 0;
                                break;
                            case 0x8A:
                                _break = 1;
                                break;
                            case 0x94:
                                _break = 2;
                                break;
                            case 0x9A:
                                _break = 3;
                                break;
                            case 0xA2:
                                _break = 4;
                                break;
                            case 0xA8:
                                _break = 5;
                                break;
                            case 0xAF:
                                _break = 6;
                                break;
                            case 0xB2:
                                _break = 7;
                                break;
                            case 0xB5:
                                _break = 8;
                                break;
                            case 0xB9:
                                _break = 9;
                                break;
                        }
                        //ボタン
                        int button = buffer[5];
                        if ((button & B_BUTTON) == B_BUTTON)
                        {
                            _buttons[0] = true;
                        }
                        else
                        {
                            _buttons[0] = false;
                        }
                        if ((button & A_BUTTON) == A_BUTTON)
                        {
                            _buttons[1] = true;
                        }
                        else
                        {
                            _buttons[1] = false;
                        }
                        if ((button & C_BUTTON) == C_BUTTON)
                        {
                            _buttons[2] = true;
                        }
                        else
                        {
                            _buttons[2] = false;
                        }
                        if ((button & D_BUTTON) == D_BUTTON)
                        {
                            _buttons[3] = true;
                        }
                        else
                        {
                            _buttons[3] = false;
                        }
                        if ((button & SELECT_BUTTON) == SELECT_BUTTON)
                        {
                            _buttons[4] = true;
                        }
                        else
                        {
                            _buttons[4] = false;
                        }
                        if ((button & START_BUTTON) == START_BUTTON)
                        {
                            _buttons[5] = true;
                        }
                        else
                        {
                            _buttons[5] = false;
                        }
                        //警笛
                        if (buffer[3] == 0x00)
                        {
                            _buttons[6] = true;
                        }
                        else
                        {
                            _buttons[6] = false;
                        }
                        //十字キー
                        int pov = buffer[4];
                        if (pov == 0 || pov == 1 || pov == 7)
                        {
                            _buttons[7] = true;
                        }
                        else
                        {
                            _buttons[7] = false;
                        }
                        if (pov >= 1 && pov <= 3)
                        {
                            _buttons[8] = true;
                        }
                        else
                        {
                            _buttons[8] = false;
                        }
                        if (pov >= 3 && pov <= 5)
                        {
                            _buttons[9] = true;
                        }
                        else
                        {
                            _buttons[9] = false;
                        }
                        if (pov >= 5 && pov <= 7)
                        {
                            _buttons[10] = true;
                        }
                        else
                        {
                            _buttons[10] = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "NumerousControllerInterface");
                }
                if (device.IsOpen)
                {
                    device.Close();
                }
            }).Start();
        }

        public override void Dispose()
        {
            _loop = false;
        }

        public override bool IsDisposed()
        {
            return !_loop;
        }

        public override int GetPowerCount()
        {
            return 5;
        }

        public override int GetPower()
        {
            return _power;
        }

        public override int GetBreakCount()
        {
            return 9;
        }

        public override int GetBreak()
        {
            return _break;
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
            if (_device.UsbRegistryInfo.Rev == 0102)
            {
                return "PS2用電車でGO!コントローラー TYPE2";
            }
            return "MultiTrainController P5B8";
        }

        public override int[] GetSliders()
        {
            return null;
        }
    }
}
