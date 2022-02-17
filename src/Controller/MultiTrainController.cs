using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System.Threading;
using System.Windows.Forms;

namespace Kusaanko.Bvets.NumerousControllerInterface.Controller
{
    public class MultiTrainController : NCIController
    {
        private static int ATS_BUTTON = 0x1;
        private static int D_BUTTON = 0x2;
        private static int A_SHALLOW_BUTTON = 0x4;
        private static int A_DEEP_BUTTON = 0x8;
        private static int B_BUTTON = 0x10;
        private static int C_BUTTON = 0x20;
        private static int START_BUTTON = 0x1;
        private static int SELECT_BUTTON = 0x2;
        private static int UP_BUTTON = 0x4;
        private static int DOWN_BUTTON = 0x8;
        private static int LEFT_BUTTON = 0x10;
        private static int RIGHT_BUTTON = 0x20;
        private static List<MTCCartridge> _cartridges = new List<MTCCartridge>();
        private UsbDevice _device;
        private UsbEndpointReader _reader;
        private int _breakNotchCount;
        private int _powerNotchCount;
        private int _power;
        private int _break;
        private bool[] _buttons;
        private bool _loop;
        private bool _A_DEEP;
        private bool _A_SHALLOW;
        private long _A_milli;
        private Reverser _revPos;
        public static List<NCIController> Get()
        {
            if (_cartridges.Count == 0)
            {
                _cartridges.Add(new MTCCartridge(0x0AE4, 0x0101, 0400, 4, 6));//P4B6
                _cartridges.Add(new MTCCartridge(0x0AE4, 0x0101, 0300, 4, 7));//P4B7
                _cartridges.Add(new MTCCartridge(0x1C06, 0x77A7, 0202, 5, 5));//P5B5
                _cartridges.Add(new MTCCartridge(0x0AE4, 0x0101, 0800, 5, 7));//P5B7
                _cartridges.Add(new MTCCartridge(0x0AE4, 0x0101, 0000, 13, 7));//P13B7
            }
            List<NCIController> controllers = new List<NCIController>();
            foreach (UsbRegistry registry in UsbDevice.AllDevices)
            {
                foreach (MTCCartridge cartridge in _cartridges)
                {
                    if (registry.Vid == cartridge.Vid && registry.Pid == cartridge.Pid && registry.Rev == cartridge.Rev)
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
                            controllers.Add(new MultiTrainController(device, cartridge.PowerCount, cartridge.BreakCount));
                            break;
                        }
                    }
                }
            }
            return controllers;
        }
        public MultiTrainController(UsbDevice device, int powerNotchCount, int breakNotchCount)
        {
            this._device = device;
            _reader = device.OpenEndpointReader(ReadEndpointID.Ep01);
            _buttons = new bool[16];
            _loop = true;
            this._powerNotchCount = powerNotchCount;
            this._breakNotchCount = breakNotchCount;
            new Thread(() =>
            {
                while (_loop)
                {
                    byte[] buffer = new byte[8];
                    int len;
                    ErrorCode code = _reader.Read(buffer, 2000, out len);
                    if (code != ErrorCode.None) continue;
                    //ハンドル
                    int notch = buffer[1] & 0x0F;
                    if (_powerNotchCount + _breakNotchCount > 14) //ノッチ数が4ビットで表せないとき
                    {
                        notch += buffer[1] & 0b00110000;
                    }
                    // 一瞬0が入力される
                    if(notch == 0)
                    {
                        continue;
                    }
                    // EBから順に1,2,3,4,...
                    if (notch <= breakNotchCount + 1)
                    {
                        _power = 0;
                        _break = breakNotchCount - notch + 2;
                    }
                    else if (notch == breakNotchCount + 2)
                    {
                        _power = 0;
                        _break = 0;
                    }
                    else
                    {
                        _power = notch - breakNotchCount - 2;
                        _break = 0;
                    }
                    //ボタン
                    int button = buffer[2];
                    SetButton(button, ATS_BUTTON, 0);
                    SetButton(button, D_BUTTON, 1);
                    _A_SHALLOW = (button & A_SHALLOW_BUTTON) == A_SHALLOW_BUTTON;
                    _A_DEEP = (button & A_DEEP_BUTTON) == A_DEEP_BUTTON;
                    if(!_A_SHALLOW && !_A_DEEP)
                    {
                        _A_milli = 0;
                        _buttons[2] = _buttons[3] = false;
                    }
                    if(_A_milli == 0 && _A_SHALLOW && !_A_DEEP)
                    {
                        _A_milli = DateTime.Now.Ticks;
                        _buttons[2] = _buttons[3] = false;
                    }
                    if(_A_DEEP)
                    {
                        _A_milli = 0;
                        _buttons[2] = false;
                        _buttons[3] = true;
                    }
                    SetButton(button, B_BUTTON, 4);
                    SetButton(button, C_BUTTON, 5);
                    button = buffer[3];
                    SetButton(button, START_BUTTON, 6);
                    SetButton(button, SELECT_BUTTON, 7);
                    SetButton(button, UP_BUTTON, 8);
                    SetButton(button, DOWN_BUTTON, 9);
                    SetButton(button, LEFT_BUTTON, 10);
                    SetButton(button, RIGHT_BUTTON, 11);
                    //レバーサー
                    button = (buffer[1] & 0xF0) >> 4;
                    if (_powerNotchCount != 5 && _breakNotchCount != 5)//P5B5だけビット位置が違う
                    {
                        button >>= 2;
                    }
                    //前
                    if (button == 0x2)
                    {
                        _revPos = Reverser.FORWARD;
                    }
                    //切
                    if (button == 0x0)
                    {
                        _revPos = Reverser.CENTER;
                    }
                    //後
                    if (button == 0x1)
                    {
                        _revPos = Reverser.BACKWARD;
                    }

                }
                if (device.IsOpen)
                {
                    device.Close();
                }
            }).Start();
        }

        private void SetButton(int button, int bit, int index)
        {
            _buttons[index] = (button & bit) == bit;
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
            return _powerNotchCount;
        }

        public override int GetPower()
        {
            return _power;
        }

        public override int GetBreakCount()
        {
            return _breakNotchCount + 1;
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
            if(_A_milli != 0 && DateTime.Now.Ticks - _A_milli > 50 * TimeSpan.TicksPerMillisecond)//50ミリ秒以上Aボタンを浅く押したら浅く押した判定
            {
                _buttons[2] = true;
            }
            return _buttons;
        }

        public override string GetControllerType()
        {
            return "LibUsbDotNet VID:0x" + Convert.ToString(_device.UsbRegistryInfo.Vid, 16).ToUpper() + " PID:0x" + Convert.ToString(_device.UsbRegistryInfo.Pid, 16).ToUpper() + " Rev:" + _device.UsbRegistryInfo.Rev;
        }

        public override string GetName()
        {
            return "MultiTrainController P" + _powerNotchCount + "B" + _breakNotchCount;
        }

        public override int[] GetSliders()
        {
            return null;
        }
    }

    class MTCCartridge
    {
        public int Vid;
        public int Pid;
        public int Rev;
        public int PowerCount;
        public int BreakCount;
        
        public MTCCartridge(int vid, int pid, int rev, int powerCount, int breakCount)
        {
            this.Vid = vid;
            this.Pid = pid;
            this.Rev = rev;
            this.PowerCount = powerCount;
            this.BreakCount = breakCount;
        }
    }
}
