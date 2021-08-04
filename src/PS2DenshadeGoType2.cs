using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using LibUsbDotNet.Main;
using LibUsbDotNet;
using System.Threading;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public class PS2DenshadeGoType2 : IController
    {
        private static int B_BUTTON = 0x1;
        private static int A_BUTTON = 0x2;
        private static int C_BUTTON = 0x4;
        private static int D_BUTTON = 0x8;
        private static int SELECT_BUTTON = 0x10;
        private static int START_BUTTON = 0x20;
        public static UsbDeviceFinder usbDeviceFinder = new UsbDeviceFinder(0x0AE4, 0x0004);
        private UsbDevice device;
        private UsbEndpointReader reader;
        private int power;
        private int breakNotch;
        private bool[] buttons;
        private int[] sliders;
        private bool loop;
        public static List<IController> Get()
        {
            List<IController> controllers = new List<IController>();
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
            this.device = device;
            reader = device.OpenEndpointReader(ReadEndpointID.Ep01);
            sliders = new int[0];
            buttons = new bool[18];
            loop = true;
            new Thread(() =>
            {
                try
                {
                    while (loop)
                    {
                        byte[] buffer = new byte[8];
                        int len;
                        ErrorCode code = reader.Read(buffer, 2000, out len);
                        if (code != ErrorCode.None) continue;
                        //力行
                        switch (buffer[2])
                        {
                            case 0x81:
                                power = 0;
                                break;
                            case 0x6D:
                                power = 1;
                                break;
                            case 0x54:
                                power = 2;
                                break;
                            case 0x3D:
                                power = 3;
                                break;
                            case 0x21:
                                power = 4;
                                break;
                            case 0x00:
                                power = 5;
                                break;
                        }
                        buttons[11] = (power & 0x1) == 0x1;
                        buttons[12] = (power & 0x2) == 0x2;
                        buttons[13] = (power & 0x4) == 0x4;
                        //ブレーキ
                        switch (buffer[1])
                        {
                            case 0x79:
                                breakNotch = 0;
                                break;
                            case 0x8A:
                                breakNotch = 1;
                                break;
                            case 0x94:
                                breakNotch = 2;
                                break;
                            case 0x9A:
                                breakNotch = 3;
                                break;
                            case 0xA2:
                                breakNotch = 4;
                                break;
                            case 0xA8:
                                breakNotch = 5;
                                break;
                            case 0xAF:
                                breakNotch = 6;
                                break;
                            case 0xB2:
                                breakNotch = 7;
                                break;
                            case 0xB5:
                                breakNotch = 8;
                                break;
                            case 0xB9:
                                breakNotch = 9;
                                break;
                        }
                        buttons[14] = (breakNotch & 0x1) == 0x1;
                        buttons[15] = (breakNotch & 0x2) == 0x2;
                        buttons[16] = (breakNotch & 0x4) == 0x4;
                        buttons[17] = (breakNotch & 0x8) == 0x8;
                        //ボタン
                        int button = buffer[5];
                        if ((button & B_BUTTON) == B_BUTTON)
                        {
                            buttons[0] = true;
                        }
                        else
                        {
                            buttons[0] = false;
                        }
                        if ((button & A_BUTTON) == A_BUTTON)
                        {
                            buttons[1] = true;
                        }
                        else
                        {
                            buttons[1] = false;
                        }
                        if ((button & C_BUTTON) == C_BUTTON)
                        {
                            buttons[2] = true;
                        }
                        else
                        {
                            buttons[2] = false;
                        }
                        if ((button & D_BUTTON) == D_BUTTON)
                        {
                            buttons[3] = true;
                        }
                        else
                        {
                            buttons[3] = false;
                        }
                        if ((button & SELECT_BUTTON) == SELECT_BUTTON)
                        {
                            buttons[4] = true;
                        }
                        else
                        {
                            buttons[4] = false;
                        }
                        if ((button & START_BUTTON) == START_BUTTON)
                        {
                            buttons[5] = true;
                        }
                        else
                        {
                            buttons[5] = false;
                        }
                        //警笛
                        if (buffer[3] == 0x00)
                        {
                            buttons[6] = true;
                        }
                        else
                        {
                            buttons[6] = false;
                        }
                        //十字キー
                        int pov = buffer[4];
                        if (pov == 0 || pov == 1 || pov == 7)
                        {
                            buttons[7] = true;
                        }
                        else
                        {
                            buttons[7] = false;
                        }
                        if (pov >= 1 && pov <= 3)
                        {
                            buttons[8] = true;
                        }
                        else
                        {
                            buttons[8] = false;
                        }
                        if (pov >= 3 && pov <= 5)
                        {
                            buttons[9] = true;
                        }
                        else
                        {
                            buttons[9] = false;
                        }
                        if (pov >= 5 && pov <= 7)
                        {
                            buttons[10] = true;
                        }
                        else
                        {
                            buttons[10] = false;
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
            return buttons;
        }

        public string GetControllerType()
        {
            return "LibUsbDotNet VID:0x" + Convert.ToString(device.UsbRegistryInfo.Vid, 16).ToUpper() + " PID:0x" + Convert.ToString(device.UsbRegistryInfo.Pid, 16).ToUpper() + " Rev:" + device.UsbRegistryInfo.Rev;
        }

        public string GetName()
        {
            if (device.UsbRegistryInfo.Rev == 0102)
            {
                return "PS2用電車でGO!コントローラー TYPE2";
            }
            return "MultiTrainController P5B8";
        }

        public int[] GetSliders()
        {
            return sliders;
        }
    }
}
