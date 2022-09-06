using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    [JsonConverter(typeof(ButtonFeatureConverter))]
    public class ButtonFeature
    {
        public string Id { get; }
        [JsonIgnore]
        public int Axis { get; }
        [JsonIgnore]
        public int Value { get; }
        [JsonIgnore]
        public string Name { get; }

        public static Dictionary<string, ButtonFeature> Features = new Dictionary<string, ButtonFeature>();

        // ButtonFeatureには識別IDとしてランダムな文字列を持たせている。テキストや数字のIDではテキストの変更に対応できなかったり順番の入れ替え時に見栄えが悪くなるため
        public static ButtonFeature ReverserBackward = new ButtonFeature("3638c08f", "リバーサー後退", 0, -1);
        public static ButtonFeature ReverserCenter = new ButtonFeature("e979379c", "リバーサー切", 0, 0);
        public static ButtonFeature ReverserForward = new ButtonFeature("35b63bba", "リバーサー前進", 0, 1);
        public static ButtonFeature ElectricHorn = new ButtonFeature("3d444081", "電笛", -1, 0);
        public static ButtonFeature Horn = new ButtonFeature("cb8902d9", "空笛", -1, 1);
        public static ButtonFeature SlowDrive = new ButtonFeature("f1038bc4", "低速運転", -1, 2);
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

    class ButtonFeatureConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ButtonFeature).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object obj = serializer.Deserialize(reader);
            if(obj.GetType() == typeof(Newtonsoft.Json.Linq.JArray))
            {
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)obj;
                int[] values = new int[]{ (int) array[0], (int) array[1] };
                // リバーサーの値が間違っていたので修正する
                if (values[0] == 0) values[1]--;
                foreach(ButtonFeature feature in ButtonFeature.Features.Values)
                {
                    if(feature.Axis == values[0] && feature.Value == values[1])
                    {
                        return feature;
                    }
                }
            }
            if(obj.GetType() == typeof(string))
            {
                return ButtonFeature.Features[(string)obj];
            }
            return ButtonFeature.Ats0;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value.GetType() == typeof(ButtonFeature))
            {
                serializer.Serialize(writer, ((ButtonFeature)value).Id);
            }
        }
    }
}
