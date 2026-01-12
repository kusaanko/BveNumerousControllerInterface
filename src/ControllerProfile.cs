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
using System.IO.Ports;

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
        public static int s_Version = 1;

        [DataMember]
        public int Version;
        [DataMember]
        public string Name;
        // ボタンのマッピング
        [DataMember]
        public Dictionary<int, ButtonFeature> KeyMap;
        // ボタンを押し続けると繰り返し入力するかどうか
        [DataMember]
        public Dictionary<int, bool> HoldToRepeat;
        // ボタンを押し続けると繰り返し入力するまでの時間
        [DataMember]
        public Dictionary<int, float> HoldToRepeatTime;
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
        // それぞれノッチでどのボタンを組み合わせとして使うか
        [DataMember]
        public int[] BrakeButtons;
        // それぞれのノッチでボタンの組み合わせを保存している
        [DataMember]
        public List<List<bool>> BrakeButtonStatus;
        [DataMember]
        public int[] BrakeAxises;
        // それぞれのノッチで軸の値の組み合わせを保存している
        [DataMember]
        public List<List<int>> BrakeAxisStatus;
        [IgnoreDataMember]
        public ArrayList[] BrakeAxisUsedNumbers;
        [IgnoreDataMember]
        public bool[] BrakeDuplicated;
        [DataMember]
        public bool IsTwoHandle;
        [DataMember]
        public FlexibleNotchMode FlexiblePower;
        [DataMember]
        public FlexibleNotchMode FlexibleBrake;
        [DataMember]
        public bool InaccuracyModePower;
        [DataMember]
        public bool InaccuracyModeBrake;
        [DataMember]
        public int PowerCenterPosition;
        [DataMember]
        public Dictionary<string, string> BveExValue;
        // Brakeの誤記
        [DataMember(EmitDefaultValue =false)]
        public List<List<bool>> BreakButtonStatus;
        [DataMember(EmitDefaultValue = false)]
        public int[] BreakButtons;
        [DataMember(EmitDefaultValue = false)]
        public int[] BreakAxises;
        // それぞれのノッチで軸の値の組み合わせを保存している
        [DataMember(EmitDefaultValue = false)]
        public List<List<int>> BreakAxisStatus;
        [DataMember(EmitDefaultValue = false)]
        public FlexibleNotchMode FlexibleBreak;
        [DataMember(EmitDefaultValue = false)]
        public bool InaccuracyModeBreak;

        private int prePowerNotch;
        private int preBrakeNotch;
        private static int s_preDirectInputCount = -1;
        private static int s_preUsbCount = -1;
        private static int s_preComPortCount = -1;

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
            HoldToRepeat = new Dictionary<int, bool>();
            HoldToRepeatTime = new Dictionary<int, float>();
            PowerAxises = new int[0];
            PowerAxisStatus = new List<List<int>>();
            PowerButtons = new int[0];
            PowerButtonStatus = new List<List<bool>>();
            PowerDuplicated = new bool[0];
            BrakeAxises = new int[0];
            BrakeAxisStatus = new List<List<int>>();
            BrakeButtons = new int[0];
            BrakeButtonStatus = new List<List<bool>>();
            BrakeDuplicated = new bool[0];
            FlexiblePower = FlexibleNotchMode.LastMax;
            FlexibleBrake = FlexibleNotchMode.EBFixed;
            InaccuracyModePower = false;
            InaccuracyModeBrake = false;
            BveExValue = new Dictionary<string, string>();
            Version = s_Version;
        }

        public void InitializeNullVariables()
        {
            if (KeyMap == null) KeyMap = new Dictionary<int, ButtonFeature>();
            if (HoldToRepeat == null) HoldToRepeat = new Dictionary<int, bool>();
            if (HoldToRepeatTime == null) HoldToRepeatTime = new Dictionary<int, float>();
            if (PowerAxises == null) PowerAxises = new int[0];
            if (PowerAxisStatus == null) PowerAxisStatus = new List<List<int>>();
            if (PowerButtons == null) PowerButtons = new int[0];
            if (PowerButtonStatus == null) PowerButtonStatus = new List<List<bool>>();
            if (PowerDuplicated == null) PowerDuplicated = new bool[0];
            if (BrakeAxises == null)
            {
                if (BreakAxises != null)
                {
                    BrakeAxises = BreakAxises;
                }
                else
                {
                    BrakeAxises = new int[0];
                }
            }
            if (BrakeAxisStatus == null)
            {
                if (BreakAxisStatus != null)
                {
                    BrakeAxisStatus = BreakAxisStatus;
                }
                else
                {
                    BrakeAxisStatus = new List<List<int>>();
                }
            }
            if (BrakeButtons == null)
            {
                if (BreakButtons != null)
                {
                    BrakeButtons = BreakButtons;
                }
                else
                {
                    BrakeButtons = new int[0];
                }
            }
            if (BrakeButtonStatus == null)
            {
                if (BreakButtonStatus != null)
                {
                    BrakeButtonStatus = BreakButtonStatus;
                }
                else
                {
                    BrakeButtonStatus = new List<List<bool>>();
                }
            }
            if (BrakeDuplicated == null) BrakeDuplicated = new bool[0];
            if (Version == 0)
            {
                InaccuracyModeBrake = InaccuracyModeBreak;
                FlexibleBrake = FlexibleBreak;
            }
            Version = s_Version;
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

        public void ResetBrake()
        {
            BrakeAxises = new int[0];
            BrakeAxisStatus = new List<List<int>>();
            BrakeButtons = new int[0];
            BrakeButtonStatus = new List<List<bool>>();
            BrakeDuplicated = new bool[0];
            InaccuracyModeBrake = false;
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
            BrakeAxisStatus = new List<List<int>>();
            for (int x = 0; x < value.GetLength(0); x++)
            {
                List<int> valueSet = new List<int>();
                for (int y = 0; y < value.GetLength(1); y++)
                {
                    valueSet.Add(value[x, y]);
                }
                BrakeAxisStatus.Add(valueSet);
            }
        }

        public void SetBrakeButtonStatus(bool[,] value)
        {
            BrakeButtonStatus = new List<List<bool>>();
            for (int x = 0; x < value.GetLength(0); x++)
            {
                List<bool> valueSet = new List<bool>();
                for (int y = 0; y < value.GetLength(1); y++)
                {
                    valueSet.Add(value[x, y]);
                }
                BrakeButtonStatus.Add(valueSet);
            }
        }

        public bool HasPower(NCIController controller)
        {
            return GetPowerCount(controller) > 0;
        }

        public bool HasBrake(NCIController controller)
        {
            return GetBrakeCount(controller) > 0;
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
            return Math.Max(PowerButtonStatus.Count - 1, 0);
        }

        public int GetBrakeCount(NCIController controller)
        {
            if (controller.GetBrakeCount() > 0)
            {
                return controller.GetBrakeCount();
            }
            // 軸の値をそのまま使うモード
            foreach (List<int> status in BrakeAxisStatus)
            {
                if (BrakeAxises.Length != status.Count)
                {
                    return 9;
                }
            }
            return Math.Max(BrakeButtonStatus.Count - 1, 0);
        }

        public int GetPower(NCIController controller, int maxValue)
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
                prePowerNotch = (int)(((float)pos / range) * maxValue);
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
            int returnNotch = 0;
            if (FlexiblePower == FlexibleNotchMode.Flexible)
            {
                if (prePowerNotch >= GetPowerCount(controller))
                {
                    returnNotch = maxValue;
                }
                returnNotch = (int)Math.Floor(prePowerNotch * ((float) maxValue / (Math.Max(GetPowerCount(controller) - 1, 0))));
            }
            else if (FlexiblePower == FlexibleNotchMode.LastMax)
            {
                if (prePowerNotch >= GetPowerCount(controller))
                {
                    returnNotch = maxValue;
                }
                returnNotch = prePowerNotch;
            }
            else
            {
                returnNotch = prePowerNotch;
            }

            // 逆回し
            if (IsTwoHandle)
            {
                return returnNotch - PowerCenterPosition;
            } else
            {
                return returnNotch;
            }
        }

        public int GetBrake(NCIController controller, int maxValue)
        {
            if(controller.GetBrakeCount() > 0)
            {
                preBrakeNotch = controller.GetBrake();
                goto ret;
            }
            if (GetBrakeCount(controller) == 0) return 0;
            bool[] buttons = controller.GetButtonsSafe();
            int[] sliders = GetSliders(controller);
            if (buttons == null || sliders == null) return 0;
            // 軸の値をそのまま使うモード
            if (IsBrakeUseRawAxisValueMode())
            {
                int pos = -sliders[BrakeAxises[0]] + BrakeAxisStatus[0][0];
                int range = Math.Abs(BrakeAxisStatus[0][1] - BrakeAxisStatus[0][0]);
                if (BrakeAxisStatus[0][0] < BrakeAxisStatus[0][1])
                {
                    pos = sliders[BrakeAxises[0]] - BrakeAxisStatus[0][0];
                }
                preBrakeNotch = (int)(((float)pos / range) * maxValue);
                if (preBrakeNotch < 0) preBrakeNotch = 0;
                return preBrakeNotch;
            }
            else
            {
                for (int k = 0; k < BrakeButtonStatus.Count; k++)
                {
                    int match = 0;
                    for (int i = 0; i < BrakeButtons.Length; i++)
                    {
                        if (buttons[BrakeButtons[i]] == BrakeButtonStatus[k][i])
                        {
                            match++;
                        }
                    }
                    for (int i = 0; i < BrakeAxises.Length; i++)
                    {
                        //不正確モード
                        if (InaccuracyModeBrake)
                        {
                            int low = controller.GetSliderMinValue();
                            int high = controller.GetSliderMaxValue();
                            int nowBrakeAxis = BrakeAxisStatus[k][i];
                            int index = BrakeAxisUsedNumbers[i].IndexOf(nowBrakeAxis);
                            if (index > 0)
                            {
                                low = ((int)BrakeAxisUsedNumbers[i][index - 1] + (int)BrakeAxisUsedNumbers[i][index]) / 2;
                            }
                            if (index < BrakeAxisUsedNumbers[i].Count - 1)
                            {
                                high = ((int)BrakeAxisUsedNumbers[i][index + 1] + (int)BrakeAxisUsedNumbers[i][index]) / 2;
                            }
                            if (low <= sliders[BrakeAxises[i]] && sliders[BrakeAxises[i]] <= high)
                            {
                                match++;
                            }
                        }
                        else
                        {
                            if (sliders[BrakeAxises[i]] == BrakeAxisStatus[k][i])
                            {
                                match++;
                            }
                        }
                    }
                    if (match == BrakeButtons.Length + BrakeAxises.Length)
                    {
                        if (BrakeDuplicated[k])
                        {
                            if (preBrakeNotch - 1 == k || preBrakeNotch + 1 == k || preBrakeNotch == k)
                            {
                                preBrakeNotch = k;
                                goto ret;
                            }
                        }
                        else
                        {
                            preBrakeNotch = k;
                            goto ret;
                        }
                    }
                }
            }
        ret:
            if (FlexibleBrake == FlexibleNotchMode.Flexible)
            {
                if(preBrakeNotch >= GetBrakeCount(controller))
                {
                    return maxValue;
                }
                return (int)Math.Floor(preBrakeNotch * ((float) maxValue / Math.Max(GetBrakeCount(controller) - 1, 0)));
            }
            else if(FlexibleBrake == FlexibleNotchMode.EBFixed)
            {
                if(preBrakeNotch >= GetBrakeCount(controller))
                {
                    return maxValue;
                }
                else if(preBrakeNotch >= maxValue - 1)
                {
                    return maxValue - 1;
                }
                return preBrakeNotch;
            }
            else if (FlexibleBrake == FlexibleNotchMode.FlexibleWithoutEB)
            {
                if (preBrakeNotch >= GetBrakeCount(controller))
                {
                    return maxValue;
                }
                return (int)Math.Floor(preBrakeNotch * ((float)maxValue / Math.Max(GetBrakeCount(controller) - 1, 0)));
            }
            else
            {
                return preBrakeNotch;
            }
        }

        public List<int> GetButtons(NCIController controller)
        {
            List<int> list = new List<int>();
            bool[] buttons = controller.GetButtonsSafe();
            for(int i = 0;i < buttons.Length;i++)
            {
                if (buttons[i] && Array.IndexOf(PowerButtons, i) == -1 && Array.IndexOf(BrakeButtons, i) == -1)
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
            BrakeDuplicated = new bool[BrakeButtonStatus.Count];
            for (int l = 0; l < BrakeButtonStatus.Count; l++)
            {
                bool[] buttons = new bool[BrakeButtons.Length];
                for (int k = 0;k < buttons.Length;k++)
                {
                    buttons[k] = BrakeButtonStatus[l][k];
                }
                int[] sliders = new int[BrakeAxises.Length];
                for (int k = 0; k < sliders.Length; k++)
                {
                    sliders[k] = BrakeAxisStatus[l][k];
                }
                bool duplicated = false;
                for (int k = 0; k < BrakeButtonStatus[l].Count; k++)
                {
                    if (k == l) continue;
                    int match = 0;
                    for (int i = 0; i < BrakeButtons.Length; i++)
                    {
                        if (BrakeButtonStatus[k][i] == buttons[i])
                        {
                            match++;
                        }
                    }
                    for (int i = 0; i < BrakeAxises.Length; i++)
                    {
                        if (BrakeAxisStatus[k][i] == sliders[i])
                        {
                            match++;
                        }
                    }
                    if (match == BrakeButtons.Length + BrakeAxises.Length)
                    {
                        duplicated = true;
                        break;
                    }
                }
                BrakeDuplicated[l] = duplicated;
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
            BrakeAxisUsedNumbers = new ArrayList[BrakeAxises.Length];
            for (int i = 0; i < BrakeAxisUsedNumbers.Length; i++)
            {
                BrakeAxisUsedNumbers[i] = new ArrayList();
                for (int k = 0; k < BrakeAxisStatus.Count; k++)
                {
                    if (!BrakeAxisUsedNumbers[i].Contains(BrakeAxisStatus[k][i]))
                    {
                        BrakeAxisUsedNumbers[i].Add(BrakeAxisStatus[k][i]);
                    }
                }
                BrakeAxisUsedNumbers[i].Sort();
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

        private bool IsBrakeUseRawAxisValueMode()
        {
            return BrakeAxisStatus.Count > 0 && (BrakeAxises.Length != BrakeAxisStatus[0].Count);
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
                profile.PowerButtonStatus = CloneList(PowerButtonStatus);
            }
            if (profile.PowerAxises != null)
            {
                profile.PowerAxises = (int[])PowerAxises.Clone();
            }
            if (profile.PowerAxisStatus != null)
            {
                profile.PowerAxisStatus = CloneList(PowerAxisStatus);
            }
            if (profile.BrakeButtons != null)
            {
                profile.BrakeButtons = (int[])BrakeButtons.Clone();
            }
            if (profile.BrakeButtonStatus != null)
            {
                profile.BrakeButtonStatus = CloneList(BrakeButtonStatus);
            }
            if (profile.BrakeAxises != null)
            {
                profile.BrakeAxises = (int[])BrakeAxises.Clone();
            }
            if (profile.BrakeAxisStatus != null)
            {
                profile.BrakeAxisStatus = CloneList(BrakeAxisStatus);
            }
            if (profile.KeyMap != null)
            {
                profile.KeyMap = new Dictionary<int, ButtonFeature>(KeyMap);
            }
            if (profile.HoldToRepeat != null)
            {
                profile.HoldToRepeat = new Dictionary<int, bool>(HoldToRepeat);
            }
            if (profile.HoldToRepeatTime != null)
            {
                profile.HoldToRepeatTime = new Dictionary<int, float>(HoldToRepeatTime);
            }
            if (profile.BveExValue != null)
            {
                profile.BveExValue = new Dictionary<string, string>(BveExValue);
            }
            profile.CalcDuplicated();
            return profile;
        }
        
        private static List<List<T>> CloneList<T>(List<List<T>> list)
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
            int directInputCount = DIJoystick.GetControllerCount();
            if (directInputCount != s_preDirectInputCount || 
                UsbDevice.AllDevices.Count != s_preUsbCount ||
                COMController.GetCounterForUpdateControllerList() != s_preComPortCount) update = true;
            if (update)
            {
                foreach (NCIController controller in controllers)
                {
                    controller.Dispose();
                }
                controllers = new List<NCIController>();
                controllers.AddRange(COMController.Get());
                controllers.AddRange(DenshadeGoShinkansen.Get());
                controllers.AddRange(DIJoystick.Get());
                controllers.AddRange(MultiTrainController.Get());
                controllers.AddRange(PS2DenshadeGoType2.Get());
                foreach (NumerousControllerPlugin plugin in NumerousControllerInterface.Plugins)
                {
                    controllers.AddRange(plugin.GetAllControllers());
                }
            }
            s_preDirectInputCount = directInputCount;
            s_preUsbCount = UsbDevice.AllDevices.Count;
            s_preComPortCount = COMController.GetCounterForUpdateControllerList();
        }

        public static void DisposeAllControllers()
        {
            DIJoystick.DisposeDirectXInstance();
            COMController.DisposeAll();
            foreach (NCIController controller in controllers)
            {
                controller.Dispose();
            }
            controllers = new List<NCIController>();
            UsbDevice.Exit();
        }
    }
}
