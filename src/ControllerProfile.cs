using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;
using System.Diagnostics;
using System.Runtime.Serialization;
using LibUsbDotNet;
using Kusaanko.Bvets.NumerousControllerInterface.Controller;
using static Kusaanko.Bvets.NumerousControllerInterface.Controller.NCIController;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public enum FlexibleNotchMode
    {
        None,
        EBFixed,
        FlexibleWithoutEB,
        Flexible,
        LastMax,
    }
    [DataContract]
    public class ControllerProfile
    {
        [DataMember]
        public string Name;
        [DataMember]
        public Dictionary<int, ButtonFeature> KeyMap;
        // それぞれノッチでどのボタンを組み合わせとして使うか
        [DataMember]
        public int[] PowerButtons;
        // それぞれのノッチでボタンの組み合わせを保存している
        [DataMember]
        public List<List<bool>> PowerButtonStatus;
        [DataMember]
        public int[] PowerAxises;
        // それぞれのノッチで軸の値の組み合わせを保存している
        [DataMember]
        public List<List<int>> PowerAxisStatus;
        [IgnoreDataMember]
        public ArrayList[] PowerAxisUsedNumbers;
        [IgnoreDataMember]
        public bool[] PowerDuplicated;
        [DataMember]
        // それぞれノッチでどのボタンを組み合わせとして使うか
        public int[] BreakButtons;
        // それぞれのノッチでボタンの組み合わせを保存している
        [DataMember]
        public List<List<bool>> BreakButtonStatus;
        [DataMember]
        public int[] BreakAxises;
        // それぞれのノッチで軸の値の組み合わせを保存している
        [DataMember]
        public List<List<int>> BreakAxisStatus;
        [IgnoreDataMember]
        public ArrayList[] BreakAxisUsedNumbers;
        [IgnoreDataMember]
        public bool[] BreakDuplicated;
        [DataMember]
        public bool IsTwoHandle;
        [DataMember]
        public FlexibleNotchMode FlexiblePower;
        [DataMember]
        public FlexibleNotchMode FlexibleBreak;
        [DataMember]
        public bool InaccuracyModePower;
        [DataMember]
        public bool InaccuracyModeBreak;
        private int prePowerNotch;
        private int preBreakNotch;
        private static int s_preDirectInputCount = -1;
        private static int s_preUsbCount = -1;

        public static List<NCIController> controllers = new List<NCIController>();

        public static Dictionary<int, string> FlexibleNotchModeStrings = new Dictionary<int, string>
        {
            { (int)FlexibleNotchMode.None, "無し" },
            { (int)FlexibleNotchMode.EBFixed, "非常のみ固定" },
            { (int)FlexibleNotchMode.FlexibleWithoutEB, "非常以外伸縮" },
            { (int)FlexibleNotchMode.Flexible, "すべて伸縮" },
            { (int)FlexibleNotchMode.LastMax, "最後のノッチを最大にする" },
        };

        public ControllerProfile() : this("")
        {
            
        }

        public ControllerProfile(string name)
        {
            Name = name;
            KeyMap = new Dictionary<int, ButtonFeature>();
            PowerAxises = new int[0];
            PowerAxisStatus = new List<List<int>>();
            PowerButtons = new int[0];
            PowerButtonStatus = new List<List<bool>>();
            PowerDuplicated = new bool[0];
            BreakAxises = new int[0];
            BreakAxisStatus = new List<List<int>>();
            BreakButtons = new int[0];
            BreakButtonStatus = new List<List<bool>>();
            BreakDuplicated = new bool[0];
            FlexiblePower = FlexibleNotchMode.LastMax;
            FlexibleBreak = FlexibleNotchMode.EBFixed;
            InaccuracyModePower = false;
            InaccuracyModeBreak = false;
        }

        public void ResetPower()
        {
            PowerAxises = new int[0];
            PowerAxisStatus = new List<List<int>>();
            PowerButtons = new int[0];
            PowerButtonStatus = new List<List<bool>>();
            PowerDuplicated = new bool[0];
            InaccuracyModePower = false;
        }

        public void ResetBreak()
        {
            BreakAxises = new int[0];
            BreakAxisStatus = new List<List<int>>();
            BreakButtons = new int[0];
            BreakButtonStatus = new List<List<bool>>();
            BreakDuplicated = new bool[0];
            InaccuracyModeBreak = false;
        }

        public void SetPowerAxisStatus(int[,] value)
        {
            PowerAxisStatus = new List<List<int>>();
            for (int x = 0;x < value.GetLength(0);x++)
            {
                List<int> valueSet = new List<int>();
                for (int y = 0;y < value.GetLength(1);y++)
                {
                    valueSet.Add(value[x, y]);
                }
                PowerAxisStatus.Add(valueSet);
            }
        }

        public void SetPowerButtonStatus(bool[,] value)
        {
            PowerButtonStatus = new List<List<bool>>();
            for (int x = 0; x < value.GetLength(0); x++)
            {
                List<bool> valueSet = new List<bool>();
                for (int y = 0; y < value.GetLength(1); y++)
                {
                    valueSet.Add(value[x, y]);
                }
                PowerButtonStatus.Add(valueSet);
            }
        }

        public void SetBreakAxisStatus(int[,] value)
        {
            BreakAxisStatus = new List<List<int>>();
            for (int x = 0; x < value.GetLength(0); x++)
            {
                List<int> valueSet = new List<int>();
                for (int y = 0; y < value.GetLength(1); y++)
                {
                    valueSet.Add(value[x, y]);
                }
                BreakAxisStatus.Add(valueSet);
            }
        }

        public void SetBreakButtonStatus(bool[,] value)
        {
            BreakButtonStatus = new List<List<bool>>();
            for (int x = 0; x < value.GetLength(0); x++)
            {
                List<bool> valueSet = new List<bool>();
                for (int y = 0; y < value.GetLength(1); y++)
                {
                    valueSet.Add(value[x, y]);
                }
                BreakButtonStatus.Add(valueSet);
            }
        }

        public bool HasPower(NCIController controller)
        {
            return GetPowerCount(controller) > 0;
        }

        public bool HasBreak(NCIController controller)
        {
            return GetBreakCount(controller) > 0;
        }

        public bool HasReverser(NCIController controller)
        {
            return controller.HasReverser();
        }

        public int[] GetSliders(NCIController controller)
        {
            return controller.GetSlidersSafe();
        }

        public int GetPowerCount(NCIController controller)
        {
            if (controller.GetPowerCount() > 0)
            {
                return controller.GetPowerCount();
            }
            // 軸の値をそのまま使うモード
            foreach (List<int> status in PowerAxisStatus)
            {
                if (PowerAxises.Length != status.Count)
                {
                    return 5;
                }
            }
            return PowerButtonStatus.Count - 1;
        }

        public int GetBreakCount(NCIController controller)
        {
            if (controller.GetBreakCount() > 0)
            {
                return controller.GetBreakCount();
            }
            // 軸の値をそのまま使うモード
            foreach (List<int> status in BreakAxisStatus)
            {
                if (BreakAxises.Length != status.Count)
                {
                    return 9;
                }
            }
            return BreakButtonStatus.Count - 1;
        }

        public int GetPower(NCIController controller, int maxStep)
        {
            if (controller.GetPowerCount() > 0)
            {
                prePowerNotch = controller.GetPower();
                goto ret;
            }
            if (GetPowerCount(controller) == 0) return 0;
            bool[] buttons = controller.GetButtonsSafe();
            int[] sliders = GetSliders(controller);
            if (buttons == null || sliders == null) return 0;
            // 軸の値をそのまま使うモード
            if (IsPowerUseRawAxisValueMode())
            {
                int pos = -sliders[PowerAxises[0]] + PowerAxisStatus[0][0];
                int range = Math.Abs(PowerAxisStatus[0][1] - PowerAxisStatus[0][0]);
                if (PowerAxisStatus[0][0] < PowerAxisStatus[0][1])
                {
                    pos = sliders[PowerAxises[0]] - PowerAxisStatus[0][0];
                }
                prePowerNotch = (int)(((float)pos / range) * (maxStep - 1));
                if (prePowerNotch < 0) prePowerNotch = 0;
                return prePowerNotch;
            }
            else
            {
                for (int k = 0; k < PowerButtonStatus.Count; k++)
                {
                    int match = 0;
                    for (int i = 0; i < PowerButtons.Length; i++)
                    {
                        if(buttons.Length <= PowerButtons[i])
                        {
                            return 0;
                        }
                        if (buttons[PowerButtons[i]] == PowerButtonStatus[k][i])
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
                            int low = controller.GetSliderMinValue();
                            int high = controller.GetSliderMaxValue();
                            int nowPowerAxis = PowerAxisStatus[k][i];
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
                            if (sliders[PowerAxises[i]] == PowerAxisStatus[k][i])
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
            if (FlexiblePower == FlexibleNotchMode.Flexible)
            {
                if (prePowerNotch == GetPowerCount(controller))
                {
                    return maxStep - 1;
                }
                return (int)Math.Floor(prePowerNotch * ((float) maxStep / GetPowerCount(controller)));
            }
            else if (FlexiblePower == FlexibleNotchMode.LastMax)
            {
                if (prePowerNotch == GetPowerCount(controller))
                {
                    return maxStep - 1;
                }
                return prePowerNotch;
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
            if (GetBreakCount(controller) == 0) return 0;
            bool[] buttons = controller.GetButtonsSafe();
            int[] sliders = GetSliders(controller);
            if (buttons == null || sliders == null) return 0;
            // 軸の値をそのまま使うモード
            if (IsBreakUseRawAxisValueMode())
            {
                int pos = -sliders[BreakAxises[0]] + BreakAxisStatus[0][0];
                int range = Math.Abs(BreakAxisStatus[0][1] - BreakAxisStatus[0][0]);
                if (BreakAxisStatus[0][0] < BreakAxisStatus[0][1])
                {
                    pos = sliders[BreakAxises[0]] - BreakAxisStatus[0][0];
                }
                preBreakNotch = (int)(((float)pos / range) * (maxStep - 1));
                if (preBreakNotch < 0) preBreakNotch = 0;
                return preBreakNotch;
            }
            else
            {
                for (int k = 0; k < BreakButtonStatus.Count; k++)
                {
                    int match = 0;
                    for (int i = 0; i < BreakButtons.Length; i++)
                    {
                        if (buttons[BreakButtons[i]] == BreakButtonStatus[k][i])
                        {
                            match++;
                        }
                    }
                    for (int i = 0; i < BreakAxises.Length; i++)
                    {
                        //不正確モード
                        if (InaccuracyModeBreak)
                        {
                            int low = controller.GetSliderMinValue();
                            int high = controller.GetSliderMaxValue();
                            int nowBreakAxis = BreakAxisStatus[k][i];
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
                            if (sliders[BreakAxises[i]] == BreakAxisStatus[k][i])
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
            if (FlexibleBreak == FlexibleNotchMode.Flexible)
            {
                if(preBreakNotch == GetBreakCount(controller))
                {
                    return maxStep - 1;
                }
                return (int)Math.Floor(preBreakNotch * ((float) maxStep / GetBreakCount(controller)));
            }
            else if(FlexibleBreak == FlexibleNotchMode.EBFixed)
            {
                if(preBreakNotch == GetBreakCount(controller))
                {
                    return maxStep - 1;
                }
                else if(preBreakNotch >= maxStep - 2)
                {
                    return maxStep - 2;
                }
                return preBreakNotch;
            }
            else if (FlexibleBreak == FlexibleNotchMode.FlexibleWithoutEB)
            {
                if (preBreakNotch == GetBreakCount(controller))
                {
                    return maxStep - 1;
                }
                return (int)Math.Floor(preBreakNotch * ((float)(maxStep - 1) / GetBreakCount(controller)));
            }
            else
            {
                return preBreakNotch;
            }
        }

        public List<int> GetButtons(NCIController controller)
        {
            List<int> list = new List<int>();
            bool[] buttons = controller.GetButtonsSafe();
            for(int i = 0;i < buttons.Length;i++)
            {
                if (buttons[i] && Array.IndexOf(PowerButtons, i) == -1 && Array.IndexOf(BreakButtons, i) == -1)
                {
                    list.Add(i);
                }
            }
            return list;
        }

        // コントローラーのボタンの重複を計算。重複時は前後のノッチから推測する
        public void CalcDuplicated()
        {
            PowerDuplicated = new bool[PowerButtonStatus.Count];
            for (int l = 0; l < PowerButtonStatus.Count; l++)
            {
                bool[] buttons = new bool[PowerButtons.Length];
                for (int k = 0; k < buttons.Length; k++)
                {
                    buttons[k] = PowerButtonStatus[l][k];
                }
                int[] sliders = new int[PowerAxises.Length];
                for (int k = 0; k < sliders.Length; k++)
                {
                    sliders[k] = PowerAxisStatus[l][k];
                }
                for (int k = 0; k < PowerButtonStatus.Count; k++)
                {
                    if (k == l) continue;
                    int match = 0;
                    for (int i = 0; i < PowerButtons.Length; i++)
                    {
                        if (PowerButtonStatus[k][i] == buttons[i])
                        {
                            match++;
                        }
                    }
                    for (int i = 0; i < PowerAxises.Length; i++)
                    {
                        if (PowerAxisStatus[k][i] == sliders[i])
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
            BreakDuplicated = new bool[BreakButtonStatus.Count];
            for (int l = 0; l < BreakButtonStatus.Count; l++)
            {
                bool[] buttons = new bool[BreakButtons.Length];
                for (int k = 0;k < buttons.Length;k++)
                {
                    buttons[k] = BreakButtonStatus[l][k];
                }
                int[] sliders = new int[BreakAxises.Length];
                for (int k = 0; k < sliders.Length; k++)
                {
                    sliders[k] = BreakAxisStatus[l][k];
                }
                bool duplicated = false;
                for (int k = 0; k < BreakButtonStatus[l].Count; k++)
                {
                    if (k == l) continue;
                    int match = 0;
                    for (int i = 0; i < BreakButtons.Length; i++)
                    {
                        if (BreakButtonStatus[k][i] == buttons[i])
                        {
                            match++;
                        }
                    }
                    for (int i = 0; i < BreakAxises.Length; i++)
                    {
                        if (BreakAxisStatus[k][i] == sliders[i])
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
            PowerAxisUsedNumbers = new ArrayList[PowerAxises.Length];
            for(int i = 0;i < PowerAxisUsedNumbers.Length; i++)
            {
                PowerAxisUsedNumbers[i] = new ArrayList();
                for(int k = 0;k < PowerAxisStatus.Count;k++)
                {
                    if (!PowerAxisUsedNumbers[i].Contains(PowerAxisStatus[k][i]))
                    {
                        PowerAxisUsedNumbers[i].Add(PowerAxisStatus[k][i]);
                    }
                }
                PowerAxisUsedNumbers[i].Sort();
            }
            BreakAxisUsedNumbers = new ArrayList[BreakAxises.Length];
            for (int i = 0; i < BreakAxisUsedNumbers.Length; i++)
            {
                BreakAxisUsedNumbers[i] = new ArrayList();
                for (int k = 0; k < BreakAxisStatus.Count; k++)
                {
                    if (!BreakAxisUsedNumbers[i].Contains(BreakAxisStatus[k][i]))
                    {
                        BreakAxisUsedNumbers[i].Add(BreakAxisStatus[k][i]);
                    }
                }
                BreakAxisUsedNumbers[i].Sort();
            }
        }

        public Reverser GetReverser(NCIController controller)
        {
            return controller.GetReverser();
        }

        private bool IsPowerUseRawAxisValueMode()
        {
            return PowerAxisStatus.Count > 0 && (PowerAxises.Length  != PowerAxisStatus[0].Count);
        }

        private bool IsBreakUseRawAxisValueMode()
        {
            return BreakAxisStatus.Count > 0 && (BreakAxises.Length != BreakAxisStatus[0].Count);
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
                profile.PowerButtonStatus = cloneList(PowerButtonStatus);
            }
            if (profile.PowerAxises != null)
            {
                profile.PowerAxises = (int[])PowerAxises.Clone();
            }
            if (profile.PowerAxisStatus != null)
            {
                profile.PowerAxisStatus = cloneList(PowerAxisStatus);
            }
            if (profile.BreakButtons != null)
            {
                profile.BreakButtons = (int[])BreakButtons.Clone();
            }
            if (profile.BreakButtonStatus != null)
            {
                profile.BreakButtonStatus = cloneList(BreakButtonStatus);
            }
            if (profile.BreakAxises != null)
            {
                profile.BreakAxises = (int[])BreakAxises.Clone();
            }
            if (profile.BreakAxisStatus != null)
            {
                profile.BreakAxisStatus = cloneList(BreakAxisStatus);
            }
            if (profile.KeyMap != null)
            {
                profile.KeyMap = new Dictionary<int, ButtonFeature>(KeyMap);
            }
            profile.CalcDuplicated();
            return profile;
        }
        
        private static List<List<T>> cloneList<T>(List<List<T>> list)
        {
            List<List<T>> result = new List<List<T>>(list.Count);
            foreach (var item in list)
            {
                List<T> values = new List<T>(item.Count);
                foreach (T value in item)
                {
                    values.Add(value);
                }
                result.Add(values);
            }
            return result;
        }

        public static void GetAllControllers()
        {
            bool update = false;
            int directInputCount = NumerousControllerInterface.Input.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly).Count;
            if (directInputCount != s_preDirectInputCount || 
                UsbDevice.AllDevices.Count != s_preUsbCount) update = true;
            if (update)
            {
                foreach (NCIController controller in controllers)
                {
                    controller.Dispose();
                }
                controllers = new List<NCIController>();
                controllers.AddRange(DIJoystick.Get());
                controllers.AddRange(PS2DenshadeGoType2.Get());
                controllers.AddRange(MultiTrainController.Get());
                foreach (NumerousControllerPlugin plugin in NumerousControllerInterface.Plugins)
                {
                    controllers.AddRange(plugin.GetAllControllers());
                }
            }
            s_preDirectInputCount = directInputCount;
            s_preUsbCount = UsbDevice.AllDevices.Count;
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
