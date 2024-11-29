using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    [DataContract]
    public class ButtonFeature
    {
        [DataMember]
        public string Id;
        public int Axis { get; }
        public int Value { get; }
        public string Name { get; }

        public static Dictionary<string, ButtonFeature> Features = new Dictionary<string, ButtonFeature>();

        // ButtonFeatureには識別IDとしてランダムな文字列を持たせている。テキストや数字のIDではテキストの変更に対応できなかったり順番の入れ替え時に見栄えが悪くなるため
        public static ButtonFeature ReverserBackward = new ButtonFeature("3638c08f", "リバーサー後退", 0, -1);
        public static ButtonFeature ReverserCenter = new ButtonFeature("e979379c", "リバーサー切", 0, 0);
        public static ButtonFeature ReverserForward = new ButtonFeature("35b63bba", "リバーサー前進", 0, 1);
        public static ButtonFeature ElectricHorn = new ButtonFeature("3d444081", "電笛", -1, 0);
        public static ButtonFeature Horn = new ButtonFeature("cb8902d9", "空笛", -1, 1);
        public static ButtonFeature FixedSpeedDrive = new ButtonFeature("f1038bc4", "定速運転", -1, 2);
        public static ButtonFeature BoardingAlightingPromotion = new ButtonFeature("74e90822", "乗降促進", -1, 3);
        public static ButtonFeature Ats0 = new ButtonFeature("6ab2c164", "ATS0(ATS確認)(S)(Space)", -2, 0);
        public static ButtonFeature Ats1 = new ButtonFeature("ed8ae4b4", "ATS1(警報維持)(A1)(Insert)", -2, 1);
        public static ButtonFeature Ats2 = new ButtonFeature("a4ed55ad", "ATS2(EBリセット)(A2)(Delete)", -2, 2);
        public static ButtonFeature Ats3 = new ButtonFeature("f8c5f318", "ATS3(復帰)(B1)(Home)", -2, 3);
        public static ButtonFeature Ats4 = new ButtonFeature("97cde4c1", "ATS4(ATS-P制動解放)(B2)(End)", -2, 4);
        public static ButtonFeature Ats5 = new ButtonFeature("beec7e6e", "ATS5(ATSに切り替え)(C1)(PageUp)", -2, 5);
        public static ButtonFeature Ats6 = new ButtonFeature("ca02115d", "ATS6(ATCに切り替え)(C2)(Next/PageDown)", -2, 6);
        public static ButtonFeature Ats7 = new ButtonFeature("2a8ac624", "ATS7(D)(2)", -2, 7);
        public static ButtonFeature Ats8 = new ButtonFeature("b09ad2fa", "ATS8(E)(3)", -2, 8);
        public static ButtonFeature Ats9 = new ButtonFeature("c6ddb615", "ATS9(F)(4)", -2, 9);
        public static ButtonFeature Ats10 = new ButtonFeature("66ed4902", "ATS10(G)(5)", -2, 10);
        public static ButtonFeature Ats11 = new ButtonFeature("11685658", "ATS11(H)(6)", -2, 11);
        public static ButtonFeature Ats12 = new ButtonFeature("001c1a61", "ATS12(I)(7)", -2, 12);
        public static ButtonFeature Ats13 = new ButtonFeature("ded78183", "ATS13(J)(8)", -2, 13);
        public static ButtonFeature Ats14 = new ButtonFeature("e1b97bc4", "ATS14(K)(9)", -2, 14);
        public static ButtonFeature Ats15 = new ButtonFeature("0b3f49a4", "ATS15(L)(0)", -2, 15);
        public static ButtonFeature MoveViewUp = new ButtonFeature("96d5af8b", "視点を上に移動", -3, 0);
        public static ButtonFeature MoveViewDown = new ButtonFeature("d37f2a0d", "視点を下に移動", -3, 1);
        public static ButtonFeature MoveViewLeft = new ButtonFeature("6fca414a", "視点を左に移動", -3, 2);
        public static ButtonFeature MoveViewRight = new ButtonFeature("850b9721", "視点を右に移動", -3, 3);
        public static ButtonFeature SetViewDefault = new ButtonFeature("4cec3a86", "視点をデフォルトに戻す", -3, 4);
        public static ButtonFeature ZoomViewIn = new ButtonFeature("9c4aaa4d", "視界をズームイン", -3, 5);
        public static ButtonFeature ZommViewOut = new ButtonFeature("243bba4e", "視界をズームアウト", -3, 6);
        public static ButtonFeature SwitchView = new ButtonFeature("9943368a", "視点を切り替え", -3, 7);
        public static ButtonFeature SwitchTimeDisplay = new ButtonFeature("55141a78", "時刻表示切り替え", -3, 8);
        public static ButtonFeature ReloadScinario = new ButtonFeature("226d5c2e", "シナリオ再読み込み", -3, 9);
        public static ButtonFeature ChangeTrainSpeed = new ButtonFeature("7c1100db", "列車速度の変更", -3, 10);
        public static ButtonFeature Fastforward = new ButtonFeature("988847ae", "早送り", -3, 11);
        public static ButtonFeature Pause = new ButtonFeature("98f3cd0b", "一時停止", -3, 12);
        public static ButtonFeature SetNotchEB = new ButtonFeature("75c2d2b2", "非常にする", 99, 0);
        public static ButtonFeature SetNotchOff = new ButtonFeature("6d3ce4ce", "全て切にする", 99, 1);
        public static ButtonFeature SetBreakOff = new ButtonFeature("b2aeefcd", "制動切", 99, 2);
        public static ButtonFeature BringBreakUp = new ButtonFeature("d59267da", "制動上げ", 99, 3);
        public static ButtonFeature BringBreakDown = new ButtonFeature("88f99f21", "制動下げ", 99, 4);
        public static ButtonFeature SetPowerOff = new ButtonFeature("f620a3af", "力行切", 99, 5);
        public static ButtonFeature BringPowerUp = new ButtonFeature("28f07705", "力行上げ", 99, 6);
        public static ButtonFeature BringPowerDown = new ButtonFeature("32f8feb0", "力行下げ", 99, 7);
        public static ButtonFeature BringNotchUp = new ButtonFeature("944ac26b", "ノッチ上げ", 99, 8);
        public static ButtonFeature BringNotchDown = new ButtonFeature("25d634b0", "ノッチ下げ", 99, 9);
        public static ButtonFeature Keyboard0 = new ButtonFeature("a4a8ee83", "0", 100, '0'); // 30
        public static ButtonFeature Keyboard1 = new ButtonFeature("979f4a33", "1", 100, '1');
        public static ButtonFeature Keyboard2 = new ButtonFeature("57610f35", "2", 100, '2');
        public static ButtonFeature Keyboard3 = new ButtonFeature("ceaeced5", "3", 100, '3');
        public static ButtonFeature Keyboard4 = new ButtonFeature("e2e1f0f0", "4", 100, '4');
        public static ButtonFeature Keyboard5 = new ButtonFeature("a9a3d649", "5", 100, '5');
        public static ButtonFeature Keyboard6 = new ButtonFeature("bcbabd50", "6", 100, '6');
        public static ButtonFeature Keyboard7 = new ButtonFeature("08fb7067", "7", 100, '7');
        public static ButtonFeature Keyboard8 = new ButtonFeature("62d706ed", "8", 100, '8');
        public static ButtonFeature Keyboard9 = new ButtonFeature("e0b605d3", "9", 100, '9'); // 39
        public static ButtonFeature KeyboardA = new ButtonFeature("6397fd44", "A", 100, 'A'); // 65
        public static ButtonFeature KeyboardB = new ButtonFeature("5c508219", "B", 100, 'B');
        public static ButtonFeature KeyboardC = new ButtonFeature("a050a097", "C", 100, 'C');
        public static ButtonFeature KeyboardD = new ButtonFeature("ef14b942", "D", 100, 'D');
        public static ButtonFeature KeyboardE = new ButtonFeature("e5cfef96", "E", 100, 'E');
        public static ButtonFeature KeyboardF = new ButtonFeature("ef8ddc5e", "F", 100, 'F');
        public static ButtonFeature KeyboardG = new ButtonFeature("568d36cb", "G", 100, 'G');
        public static ButtonFeature KeyboardH = new ButtonFeature("6c8c444f", "H", 100, 'H');
        public static ButtonFeature KeyboardI = new ButtonFeature("f49d55ac", "I", 100, 'I');
        public static ButtonFeature KeyboardJ = new ButtonFeature("8b9199cf", "J", 100, 'J');
        public static ButtonFeature KeyboardK = new ButtonFeature("b7b23aa0", "K", 100, 'K');
        public static ButtonFeature KeyboardL = new ButtonFeature("49f6dd96", "L", 100, 'L');
        public static ButtonFeature KeyboardM = new ButtonFeature("253183ca", "M", 100, 'M');
        public static ButtonFeature KeyboardN = new ButtonFeature("b033b8ce", "N", 100, 'N');
        public static ButtonFeature KeyboardO = new ButtonFeature("75e9f189", "O", 100, 'O');
        public static ButtonFeature KeyboardP = new ButtonFeature("bedaa93f", "P", 100, 'P');
        public static ButtonFeature KeyboardQ = new ButtonFeature("6313136e", "Q", 100, 'Q');
        public static ButtonFeature KeyboardR = new ButtonFeature("96e81156", "R", 100, 'R');
        public static ButtonFeature KeyboardS = new ButtonFeature("45c268d7", "S", 100, 'S');
        public static ButtonFeature KeyboardT = new ButtonFeature("038c81c8", "T", 100, 'T');
        public static ButtonFeature KeyboardU = new ButtonFeature("9f5fc008", "U", 100, 'U');
        public static ButtonFeature KeyboardV = new ButtonFeature("ca33b020", "V", 100, 'V');
        public static ButtonFeature KeyboardW = new ButtonFeature("09856432", "W", 100, 'W');
        public static ButtonFeature KeyboardX = new ButtonFeature("823c234d", "X", 100, 'X');
        public static ButtonFeature KeyboardY = new ButtonFeature("c9378bc7", "Y", 100, 'Y');
        public static ButtonFeature KeyboardZ = new ButtonFeature("853e0103", "Z", 100, 'Z'); // 90
        public static ButtonFeature KeyboardF1 = new ButtonFeature("4cba10d1", "F1", 100, 101);
        public static ButtonFeature KeyboardF2 = new ButtonFeature("cb2abece", "F2", 100, 102);
        public static ButtonFeature KeyboardF3 = new ButtonFeature("bb73caeb", "F3", 100, 103);
        public static ButtonFeature KeyboardF4 = new ButtonFeature("2f9fd371", "F4", 100, 104);
        public static ButtonFeature KeyboardF5 = new ButtonFeature("c982f633", "F5", 100, 105);
        public static ButtonFeature KeyboardF6 = new ButtonFeature("37bb5cd8", "F6", 100, 106);
        public static ButtonFeature KeyboardF7 = new ButtonFeature("977f2870", "F7", 100, 107);
        public static ButtonFeature KeyboardF8 = new ButtonFeature("2188e19e", "F8", 100, 108);
        public static ButtonFeature KeyboardF9 = new ButtonFeature("37bb6b55", "F9", 100, 109);
        public static ButtonFeature KeyboardF10 = new ButtonFeature("f30a5c0b", "F10", 100, 110);
        public static ButtonFeature KeyboardF11 = new ButtonFeature("c1370a4d", "F11", 100, 111);
        public static ButtonFeature KeyboardF12 = new ButtonFeature("99b68cf1", "F12", 100, 112);
        public static ButtonFeature KeyboardPageUp = new ButtonFeature("56b2a35b", "Page Up", 100, 150);
        public static ButtonFeature KeyboardPageDown = new ButtonFeature("403dd696", "Page Down", 100, 151);
        public static ButtonFeature KeyboardHome = new ButtonFeature("501d504d", "Home", 100, 152);
        public static ButtonFeature KeyboardEnd = new ButtonFeature("e91894f3", "End", 100, 153);
        public static ButtonFeature KeyboardInsert = new ButtonFeature("749c1f0c", "Insert", 100, 154);
        public static ButtonFeature KeyboardDelete = new ButtonFeature("5e539979", "Delete", 100, 155);
        public static ButtonFeature KeyboardEsc = new ButtonFeature("daa80fb6", "Esc", 100, 156);
        public static ButtonFeature KeyboardTab = new ButtonFeature("7eaa10f6", "Tab", 100, 157);
        public static ButtonFeature KeyboardBackspace = new ButtonFeature("c4f942ad", "Backspace", 100, 158);
        public static ButtonFeature KeyboardEnter = new ButtonFeature("71e60b39", "Enter", 100, 159);
        public static ButtonFeature KeyboardSpace = new ButtonFeature("0bf899e1", "Space", 100, 160);
        public static ButtonFeature KeyboardUpArrow = new ButtonFeature("dd9794b9", "Up Arrow", 100, 161);
        public static ButtonFeature KeyboardDownArrow = new ButtonFeature("bb85b161", "Down Arrow", 100, 162);
        public static ButtonFeature KeyboardLeftArrow = new ButtonFeature("b68f82e0", "Left Arrow", 100, 163);
        public static ButtonFeature KeyboardRightArrow = new ButtonFeature("e38c5400", "Right Arrow", 100, 164);
        public static ButtonFeature KeyboardRightKeypadAdd = new ButtonFeature("5eb8a11d", "テンキー +", 100, 165);
        public static ButtonFeature KeyboardRightKeypadSub = new ButtonFeature("985519a4", "テンキー -", 100, 166);
        public static ButtonFeature KeyboardRightKeypadMul = new ButtonFeature("8a39daf3", "テンキー *", 100, 167);
        public static ButtonFeature KeyboardRightKeypadDiv = new ButtonFeature("6d0ec535", "テンキー /", 100, 168);

        public static void Initialize()
        {
            if (Features.Count != 0) return;
            Type t = typeof(ButtonFeature);

            FieldInfo[] infos = t.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo info in infos)
            {
                if(info.FieldType == t)
                {
                    ButtonFeature feature = (ButtonFeature)info.GetValue(null);
                    if (Features.ContainsKey(feature.Id))
                    {
                        throw new ArgumentException("ButtonFeature " + info.Name + " Id " + feature.Id + " is exist. Please use UUID's first sector." );
                    }
                    if(feature.Id.Length != 8)
                    {
                        throw new ArgumentException("ButtonFeature " + info.Name + " Id " + feature.Id + " is wrong. Please use UUID's first sector.");
                    }
                    Features.Add(feature.Id, feature);
                }
            }
        }

        public ButtonFeature(string id, string name, int axis, int value)
        {
            Id = id;
            Axis = axis;
            Value = value;
            Name = name;
        }
    }
}
