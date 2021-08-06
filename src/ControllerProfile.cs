using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;
using System.Diagnostics;
using Newtonsoft.Json;
using LibUsbDotNet;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public class ControllerProfile
    {
        public Dictionary<int, int[]> KeyMap;
        public int[] PowerButtons;
        public bool[,] PowerButtonStatus;
        public int[] PowerAxises;
        public int[,] PowerAxisStatus;
        [JsonIgnore]
        public ArrayList[] PowerAxisUsedNumbers;
        [JsonIgnore]
        public bool[] PowerDuplicated;
        public int[] BreakButtons;
        public bool[,] BreakButtonStatus;
        public int[] BreakAxises;
        public int[,] BreakAxisStatus;
        [JsonIgnore]
        public ArrayList[] BreakAxisUsedNumbers;
        [JsonIgnore]
        public bool[] BreakDuplicated;
        public bool IsTwoHandle;
        public bool IsMasterController;
        public bool IsFlexibleNotch;
        public bool InaccuracyModePower;
        public bool InaccuracyModeBreak;
        private int prePowerNotch;
        private int preBreakNotch;

        public static List<NCIController> controllers = new List<NCIController>();

        public ControllerProfile()
        {
            KeyMap = new Dictionary<int, int[]>();
            PowerAxises = new int[0];
            PowerAxisStatus = new int[0, 0];
            PowerButtons = new int[0];
            PowerButtonStatus = new bool[0, 0];
            PowerDuplicated = new bool[0];
            BreakAxises = new int[0];
            BreakAxisStatus = new int[0, 0];
            BreakButtons = new int[0];
            BreakButtonStatus = new bool[0, 0];
            BreakDuplicated = new bool[0];
            IsFlexibleNotch = false;
            InaccuracyModePower = false;
            InaccuracyModeBreak = false;
        }

        public int[] GetSliders(NCIController state)
        {
            return state.GetSliders();
        }

        public int GetPowerCount(NCIController controller)
        {
            if (controller.GetPowerCount() > 0)
            {
                return controller.GetPowerCount();
            }
            if (PowerAxises.Length != PowerAxisStatus.GetLength(1))
            {
                return 6;
            }
            return PowerButtonStatus.GetLength(0);
        }

        public int GetBreakCount(NCIController controller)
        {
            if (controller.GetBreakCount() > 0)
            {
                return controller.GetBreakCount();
            }
            if (BreakAxises.Length != BreakAxisStatus.GetLength(1))
            {
                return 10;
            }
            return BreakButtonStatus.GetLength(0);
        }

        public int GetPower(NCIController controller, int maxStep)
        {
            if (controller.GetPowerCount() > 0)
            {
                prePowerNotch = controller.GetPower();
                goto ret;
            }
            bool[] buttons = controller.GetButtons();
            int[] sliders = GetSliders(controller);
            if (PowerAxises.Length != PowerAxisStatus.GetLength(1))
            {
                int pos = -sliders[PowerAxises[0]] + PowerAxisStatus[0, 0];
                int range = Math.Abs(PowerAxisStatus[0, 1] - PowerAxisStatus[0, 0]);
                if (PowerAxisStatus[0, 0] < PowerAxisStatus[0, 1])
                {
                    pos = sliders[PowerAxises[0]] - PowerAxisStatus[0, 0];
                }
                prePowerNotch = (int)(((float)pos / range) * (maxStep - 1));
                if (prePowerNotch < 0) prePowerNotch = 0;
                return prePowerNotch;
            }
            else
            {
                for (int k = 0; k < PowerButtonStatus.GetLength(0); k++)
                {
                    int match = 0;
                    for (int i = 0; i < PowerButtons.Length; i++)
                    {
                        if(buttons.Length <= PowerButtons[i])
                        {
                            return 0;
                        }
                        if (buttons[PowerButtons[i]] == PowerButtonStatus[k, i])
                        {
                            match++;
                        }
                    }
                    for (int i = 0; i < PowerAxises.Length; i++)
                    {
                        if (sliders.Length <= PowerAxises[i])
                        {
                            return 0;
                        }
                        //不正確モード
                        if (InaccuracyModePower)
                        {
                            int low = -1000;
                            int high = 1000;
                            int nowPowerAxis = PowerAxisStatus[k, i];
                            int index = PowerAxisUsedNumbers[i].IndexOf(nowPowerAxis);
                            if(index > 0)
                            {
                                low = ((int)PowerAxisUsedNumbers[i][index - 1] + (int)PowerAxisUsedNumbers[i][index]) / 2;
                            }
                            if (index < PowerAxisUsedNumbers[i].Count - 1)
                            {
                                high = ((int)PowerAxisUsedNumbers[i][index + 1] + (int)PowerAxisUsedNumbers[i][index]) / 2;
                            }
                            if (low <= sliders[PowerAxises[i]] && sliders[PowerAxises[i]] <= high)
                            {
                                match++;
                            }
                        }
                        else
                        {
                            if (sliders[PowerAxises[i]] == PowerAxisStatus[k, i])
                            {
                                match++;
                            }
                        }
                    }
                    if (match == PowerButtons.Length + PowerAxises.Length)
                    {
                        if (PowerDuplicated[k])
                        {
                            if (prePowerNotch - 1 == k || prePowerNotch + 1 == k || prePowerNotch == k)
                            {
                                prePowerNotch = k;
                                goto ret;
                            }
                        }
                        else
                        {
                            prePowerNotch = k;
                            goto ret;
                        }
                    }
                }
            }
        ret:
            if (IsFlexibleNotch)
            {
                if (prePowerNotch + 1 == GetPowerCount(controller))
                {
                    return maxStep - 1;
                }
                return (int)Math.Floor(prePowerNotch * ((float) maxStep / GetPowerCount(controller)));
            }
            else
            {
                return prePowerNotch;
            }
        }

        public int GetBreak(NCIController controller, int maxStep)
        {
            if(controller.GetBreakCount() > 0)
            {
                preBreakNotch = controller.GetBreak();
                goto ret;
            }
            bool[] buttons = controller.GetButtons();
            int[] sliders = GetSliders(controller);
            if (BreakAxises.Length != BreakAxisStatus.GetLength(1))
            {
                int pos = -sliders[BreakAxises[0]] + BreakAxisStatus[0, 0];
                int range = Math.Abs(BreakAxisStatus[0, 1] - BreakAxisStatus[0, 0]);
                if (BreakAxisStatus[0, 0] < BreakAxisStatus[0, 1])
                {
                    pos = sliders[BreakAxises[0]] - BreakAxisStatus[0, 0];
                }
                preBreakNotch = (int)(((float)pos / range) * (maxStep - 1));
                if (preBreakNotch < 0) preBreakNotch = 0;
                return preBreakNotch;
            }
            else
            {
                for (int k = 0; k < BreakButtonStatus.GetLength(0); k++)
                {
                    int match = 0;
                    for (int i = 0; i < BreakButtons.Length; i++)
                    {
                        if (buttons[BreakButtons[i]] == BreakButtonStatus[k, i])
                        {
                            match++;
                        }
                    }
                    for (int i = 0; i < BreakAxises.Length; i++)
                    {
                        //不正確モード
                        if (InaccuracyModeBreak)
                        {
                            int low = -1000;
                            int high = 1000;
                            int nowBreakAxis = BreakAxisStatus[k, i];
                            int index = BreakAxisUsedNumbers[i].IndexOf(nowBreakAxis);
                            if (index > 0)
                            {
                                low = ((int)BreakAxisUsedNumbers[i][index - 1] + (int)BreakAxisUsedNumbers[i][index]) / 2;
                            }
                            if (index < BreakAxisUsedNumbers[i].Count - 1)
                            {
                                high = ((int)BreakAxisUsedNumbers[i][index + 1] + (int)BreakAxisUsedNumbers[i][index]) / 2;
                            }
                            if (low <= sliders[BreakAxises[i]] && sliders[BreakAxises[i]] <= high)
                            {
                                match++;
                            }
                        }
                        else
                        {
                            if (sliders[BreakAxises[i]] == BreakAxisStatus[k, i])
                            {
                                match++;
                            }
                        }
                    }
                    if (match == BreakButtons.Length + BreakAxises.Length)
                    {
                        if (BreakDuplicated[k])
                        {
                            if (preBreakNotch - 1 == k || preBreakNotch + 1 == k || preBreakNotch == k)
                            {
                                preBreakNotch = k;
                                goto ret;
                            }
                        }
                        else
                        {
                            preBreakNotch = k;
                            goto ret;
                        }
                    }
                }
            }
        ret:
            if (IsFlexibleNotch)
            {
                if(preBreakNotch + 1 == GetBreakCount(controller))
                {
                    return maxStep - 1;
                }
                return (int)Math.Floor(preBreakNotch * ((float) maxStep / GetBreakCount(controller)));
            }
            else
            {
                return preBreakNotch;
            }
        }

        public List<int> GetButtons(NCIController state)
        {
            List<int> list = new List<int>();
            bool[] buttons = state.GetButtons();
            for(int i = 0;i < buttons.Length;i++)
            {
                if (buttons[i] && Array.IndexOf(PowerButtons, i) == -1 && Array.IndexOf(BreakButtons, i) == -1)
                {
                    list.Add(i);
                }
            }
            return list;
        }

        public void CalcDuplicated()
        {
            PowerDuplicated = new bool[PowerButtonStatus.GetLength(0)];
            for (int l = 0; l < PowerButtonStatus.GetLength(0); l++)
            {
                bool[] buttons = new bool[PowerButtonStatus.GetLength(1)];
                for (int k = 0; k < buttons.Length; k++)
                {
                    buttons[k] = PowerButtonStatus[l, k];
                }
                int[] sliders = new int[PowerAxises.Length];
                for (int k = 0; k < sliders.Length; k++)
                {
                    sliders[k] = PowerAxisStatus[l, k];
                }
                for (int k = 0; k < PowerButtonStatus.GetLength(0); k++)
                {
                    if (k == l) continue;
                    int match = 0;
                    for (int i = 0; i < PowerButtons.Length; i++)
                    {
                        if (PowerButtonStatus[k, i] == buttons[i])
                        {
                            match++;
                        }
                    }
                    for (int i = 0; i < PowerAxises.Length; i++)
                    {
                        if (PowerAxisStatus[k, i] == sliders[i])
                        {
                            match++;
                        }
                    }
                    if (match == PowerButtons.Length + PowerAxises.Length)
                    {
                        PowerDuplicated[k] = true;
                        break;
                    }
                }
            }
            BreakDuplicated = new bool[BreakButtonStatus.GetLength(0)];
            for (int l = 0; l < BreakButtonStatus.GetLength(0); l++)
            {
                bool[] buttons = new bool[BreakButtonStatus.GetLength(1)];
                for (int k = 0;k < buttons.Length;k++)
                {
                    buttons[k] = BreakButtonStatus[l, k];
                }
                int[] sliders = new int[BreakAxisStatus.GetLength(1)];
                for (int k = 0; k < sliders.Length; k++)
                {
                    sliders[k] = BreakAxisStatus[l, k];
                }
                bool duplicated = false;
                for (int k = 0; k < BreakButtonStatus.GetLength(0); k++)
                {
                    if (k == l) continue;
                    int match = 0;
                    for (int i = 0; i < BreakButtons.Length; i++)
                    {
                        if (BreakButtonStatus[k, i] == buttons[i])
                        {
                            match++;
                        }
                    }
                    for (int i = 0; i < BreakAxises.Length; i++)
                    {
                        if (BreakAxisStatus[k, i] == sliders[BreakAxises[i]])
                        {
                            match++;
                        }
                    }
                    if (match == BreakButtons.Length + BreakAxises.Length)
                    {
                        duplicated = true;
                        break;
                    }
                }
                BreakDuplicated[l] = duplicated;
            }
            //使用済みの軸の値を取得
            //不正確モードで使用します
            PowerAxisUsedNumbers = new ArrayList[PowerAxisStatus.GetLength(1)];
            for(int i = 0;i < PowerAxisStatus.GetLength(1); i++)
            {
                PowerAxisUsedNumbers[i] = new ArrayList();
                for(int k = 0;k < PowerAxisStatus.GetLength(0);k++)
                {
                    if (!PowerAxisUsedNumbers[i].Contains(PowerAxisStatus[k, i]))
                    {
                        PowerAxisUsedNumbers[i].Add(PowerAxisStatus[k, i]);
                    }
                }
                PowerAxisUsedNumbers[i].Sort();
            }
            BreakAxisUsedNumbers = new ArrayList[BreakAxisStatus.GetLength(1)];
            for (int i = 0; i < BreakAxisStatus.GetLength(1); i++)
            {
                BreakAxisUsedNumbers[i] = new ArrayList();
                for (int k = 0; k < BreakAxisStatus.GetLength(0); k++)
                {
                    if (!BreakAxisUsedNumbers[i].Contains(BreakAxisStatus[k, i]))
                    {
                        BreakAxisUsedNumbers[i].Add(BreakAxisStatus[k, i]);
                    }
                }
                BreakAxisUsedNumbers[i].Sort();
            }
        }

        public ControllerProfile Clone()
        {
            ControllerProfile profile = (ControllerProfile)MemberwiseClone();
            if(profile.PowerButtons != null)
            {
                profile.PowerButtons = (int[])PowerButtons.Clone();
            }
            if (profile.PowerButtonStatus != null)
            {
                profile.PowerButtonStatus = (bool[,])PowerButtonStatus.Clone();
            }
            if (profile.PowerAxises != null)
            {
                profile.PowerAxises = (int[])PowerAxises.Clone();
            }
            if (profile.PowerAxisStatus != null)
            {
                profile.PowerAxisStatus = (int[,])PowerAxisStatus.Clone();
            }
            if (profile.BreakButtons != null)
            {
                profile.BreakButtons = (int[])BreakButtons.Clone();
            }
            if (profile.BreakButtonStatus != null)
            {
                profile.BreakButtonStatus = (bool[,])BreakButtonStatus.Clone();
            }
            if (profile.BreakAxises != null)
            {
                profile.BreakAxises = (int[])BreakAxises.Clone();
            }
            if (profile.BreakAxisStatus != null)
            {
                profile.BreakAxisStatus = (int[,])BreakAxisStatus.Clone();
            }
            if (profile.KeyMap != null)
            {
                profile.KeyMap = new Dictionary<int, int[]>(KeyMap);
            }
            profile.CalcDuplicated();
            return profile;
        }

        public static void GetAllControllers()
        {
            if (NumerousControllerInterface.Input.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly).Count != controllers.Count)
            {
                foreach (NCIController controller in controllers)
                {
                    controller.Dispose();
                }
                controllers = new List<NCIController>();
                List<string> addedControllerName = new List<string>();
                foreach (DeviceInstance device in NumerousControllerInterface.Input.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
                {
                    try
                    {
                        Joystick stick = new Joystick(NumerousControllerInterface.Input, device.InstanceGuid);
                        if (!addedControllerName.Contains(device.ProductName))
                        {
                            NCIController controller = new DIJoystick(stick, device.ProductName);
                            controllers.Add(controller);
                            addedControllerName.Add(controller.GetName());
                        }
                    }
                    catch (DirectInputException)
                    {
                    }
                }
                controllers.AddRange(PS2DenshadeGoType2.Get());
                controllers.AddRange(MultiTrainController.Get());
            }
        }

        public static void DisposeAllControllers()
        {
            foreach (NCIController controller in controllers)
            {
                controller.Dispose();
            }
            controllers = new List<NCIController>();
            UsbDevice.Exit();
        }
    }
}
