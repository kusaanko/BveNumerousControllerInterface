using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System.Threading;
using System.Windows.Forms;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public class MultiTrainController : IController
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
        private static List<MTCCartridge> cartridges = new List<MTCCartridge>();
        private UsbDevice device;
        private UsbEndpointReader reader;
        private int breakNotchCount;
        private int powerNotchCount;
        private int power;
        private int breakNotch;
        private bool[] buttons;
        private int[] sliders;
        private bool loop;
        private bool A_DEEP;
        private bool A_SHALLOW;
        private long A_milli;
        private int RevPos;
        private long Rev_milli;
        public static List<IController> Get()
        {
            if (cartridges.Count == 0)
            {
                cartridges.Add(new MTCCartridge(0x0AE4, 0x0101, 0400, 4, 6));//P4B6
                cartridges.Add(new MTCCartridge(0x0AE4, 0x0101, 0300, 4, 7));//P4B7
                cartridges.Add(new MTCCartridge(0x1C06, 0x77A7, 0202, 5, 5));//P5B5
            }
            List<IController> controllers = new List<IController>();
            foreach (UsbRegistry registry in UsbDevice.AllDevices)
            {
                foreach (MTCCartridge cartridge in cartridges)
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
            this.device = device;
            reader = device.OpenEndpointReader(ReadEndpointID.Ep01);
            sliders = new int[0];
            buttons = new bool[23];
            loop = true;
            this.powerNotchCount = powerNotchCount;
            this.breakNotchCount = breakNotchCount;
            new Thread(() =>
            {
                while (loop)
                {
                    byte[] buffer = new byte[8];
                    int len;
                    ErrorCode code = reader.Read(buffer, 2000, out len);
                    if (code != ErrorCode.None) continue;
                    //ハンドル
                    int notch = buffer[1] & 0x0F;
                    if(notch <= breakNotchCount + 1)
                    {
                        power = 0;
                        breakNotch = breakNotchCount - notch + 2;
                    }else if(notch == breakNotchCount + 2)
                    {
                        power = 0;
                        breakNotch = 0;
                    }else
                    {
                        power = notch - breakNotchCount - 2;
                        breakNotch = 0;
                    }
                    buttons[15] = (power & 0x1) == 0x1;
                    buttons[16] = (power & 0x2) == 0x2;
                    buttons[17] = (power & 0x4) == 0x4;
                    buttons[18] = (breakNotch & 0x1) == 0x1;
                    buttons[19] = (breakNotch & 0x2) == 0x2;
                    buttons[20] = (breakNotch & 0x4) == 0x4;
                    buttons[21] = (breakNotch & 0x8) == 0x8;
                    //ボタン
                    int button = buffer[2];
                    SetButton(button, ATS_BUTTON, 0);
                    SetButton(button, D_BUTTON, 1);
                    A_SHALLOW = (button & A_SHALLOW_BUTTON) == A_SHALLOW_BUTTON;
                    A_DEEP = (button & A_DEEP_BUTTON) == A_DEEP_BUTTON;
                    if(!A_SHALLOW && !A_DEEP)
                    {
                        A_milli = 0;
                        buttons[2] = buttons[3] = false;
                    }
                    if(A_milli == 0 && A_SHALLOW && !A_DEEP)
                    {
                        A_milli = DateTime.Now.Ticks;
                        buttons[2] = buttons[3] = false;
                    }
                    if(A_DEEP)
                    {
                        A_milli = 0;
                        buttons[2] = false;
                        buttons[3] = true;
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
                    //前
                    buttons[12] = button == 0x8 || button == 0x2;
                    if (RevPos != 0 && buttons[12])
                    {
                        Rev_milli = DateTime.Now.Ticks;
                        RevPos = 0;
                    }
                    //切
                    buttons[13] = button == 0x0;
                    if (RevPos != 1 && buttons[13])
                    {
                        Rev_milli = DateTime.Now.Ticks;
                        RevPos = 1;
                    }
                    //後
                    buttons[14] = button == 0x4 || button == 0x1;
                    if (RevPos != 2 && buttons[14])
                    {
                        Rev_milli = DateTime.Now.Ticks;
                        RevPos = 2;
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
            buttons[index] = (button & bit) == bit;
        }
        public State Read()
        {
            return new State();
        }

        public void Dispose()
        {
            loop = false;
        }

        public bool[] GetButtons()
        {
            if(A_milli != 0 && DateTime.Now.Ticks - A_milli > 50 * TimeSpan.TicksPerMillisecond)//50ミリ秒以上Aボタンを浅く押したら浅く押した判定
            {
                buttons[2] = true;
            }
            if(DateTime.Now.Ticks - Rev_milli > 100 * TimeSpan.TicksPerMillisecond)//100ミリ以上経ったらリバーサーのボタンを離す
            {
                buttons[12] = buttons[13] = buttons[14] = false;
            }
            return buttons;
        }

        public string GetControllerType()
        {
            return "LibUsbDotNet VID:0x" + Convert.ToString(device.UsbRegistryInfo.Vid, 16).ToUpper() + " PID:0x" + Convert.ToString(device.UsbRegistryInfo.Pid, 16).ToUpper() + " Rev:" + device.UsbRegistryInfo.Rev;
        }

        public string GetName()
        {
            return "MultiTrainController P" + powerNotchCount + "B" + breakNotchCount;
        }

        public int[] GetSliders()
        {
            return sliders;
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
