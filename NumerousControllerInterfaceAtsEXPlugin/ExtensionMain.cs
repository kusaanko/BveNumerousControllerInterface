using AtsEx.PluginHost.Plugins.Extensions;
using AtsEx.PluginHost.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using FastMember;
using AtsEx.PluginHost;
using MonoMod.Utils;

namespace Kusaanko.Bvets.NumerousControllerInterface.AtsEXPlugin
{
    [PluginType(PluginType.Extension)]
    public class ExtensionMain : AssemblyPluginBase, IExtension
    {
        private Assembly NumerousControllerInterfaceAssembly;
        private Type NumerousControllerInterfaceType;
        private Dictionary<string, FastMethod> Methods;
        private Dictionary<string, FastMethod> GetterMethods;
        private bool IsLoaded = false;
        private Dictionary<string, PropertyInfo> RetrievableValuesFromKey;
        private Dictionary<string, PropertyInfo> RetrievableValues;
        private Dictionary<string, PropertyInfo> Getters;
        private Dictionary<string, object> PreValue;
        private List<string> RetriveValues;
        public ExtensionMain(PluginBuilder builder) : base(builder)
        {
            // NumerousControllerInterfaceがなくてもエラーを出さないようにリフレクションを使用
            // ExtensionはどのBVE環境でも同じプラグインが読み込まれるため、BVE側にNumerosuControllerInterfaceが存在しなくても問題なく動くようにする
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // 対応バージョン0.12以上
                if (assembly.GetName().Name == "Kusaanko.NumerousControllerInterface" && (assembly.GetName().Version.Major > 0 ||  assembly.GetName().Version.Minor >= 12))
                {
                    NumerousControllerInterfaceAssembly = assembly;
                    NumerousControllerInterfaceType = NumerousControllerInterfaceAssembly.GetType("Kusaanko.Bvets.NumerousControllerInterface.NumerousControllerInterface");
                }
            }
            if (NumerousControllerInterfaceType != null)
            {
                // 今後使用する関数を取得
                string[] methods = {
                    "SetVersion",
                    "Disposed",
                    "ReportAvailableValues",
                    "ReportValueChanged",
                    "GetUseValueList",
                };
                SetMethods(methods);
            }
            if (IsLoaded)
            {
                // NumerousController側にAtsExPluginの起動完了を通知
                Invoke("SetVersion", Assembly.GetExecutingAssembly().GetName().Version);

                // 取得可能な値を列挙
                RetrievableValues = new Dictionary<string, PropertyInfo>();
                Getters = new Dictionary<string, PropertyInfo>();
                RetrievableValuesFromKey = new Dictionary<string, PropertyInfo>();
                GetterMethods = new Dictionary<string, FastMethod>();
                GetAllRetrievableProperties(typeof(IBveHacker), null);
                // 取得可能な値の一覧を通知
                // string, Type, string
                List<object[]> values = new List<object[]>
                {
                    { new object[] {"Standard.SpeedKmPerHour", typeof(double), "時速" } },
                    { new object[] {"Standard.DoorClosed", typeof(bool), "戸閉灯" } },
                    { new object[] {"Standard.BcPressure", typeof(double), "ブレーキシリンダ圧力[kPa]" } },
                    { new object[] { "Standard.BpPressure", typeof(double), "ブレーキ管圧力[kPa]" } },
                    { new object[] { "Standard.Current", typeof(double), "電流[A]" } },
                };
                foreach (var item in RetrievableValuesFromKey.OrderBy(pair => pair.Key))
                {
                    var info = item.Key;
                    values.Add(new object[] { info, item.Value.PropertyType, item.Value.DeclaringType.Name + "." + item.Value.Name });
                }
                Invoke("ReportAvailableValues", values);
            }
        }

        private void GetAllRetrievableProperties(Type findTarget, PropertyInfo instance)
        {
            // 取得可能な値を列挙
            foreach (var property in findTarget.GetProperties())
            {
                if ((property.PropertyType.Namespace == "BveTypes.ClassWrappers") && 
                    !property.PropertyType.IsArray && !property.PropertyType.Name.Contains("List") && !property.PropertyType.Name.Contains("Form"))
                {
                    string key = GetKey(property);
                    if (!Getters.ContainsKey(key))
                    {
                        Getters.Add(key, instance);
                        FastMethod fastMethod = FastMethod.Create(property.GetMethod);
                        GetterMethods[key] = fastMethod;
                    }
                    GetAllRetrievableProperties(property.PropertyType, property);
                } else
                {
                    if (property.GetMethod != null)
                    {
                        switch (Type.GetTypeCode(property.PropertyType))
                        {
                            case TypeCode.Boolean:
                            case TypeCode.Char:
                            case TypeCode.Single:
                            case TypeCode.Decimal:
                            case TypeCode.Double:
                            case TypeCode.String:
                            case TypeCode.Int32:
                                string key = GetKey(property);
                                if (!RetrievableValuesFromKey.ContainsKey(key))
                                {
                                    RetrievableValues.Add(key, instance);
                                    RetrievableValuesFromKey.Add(key, property);
                                    FastMethod fastMethod = FastMethod.Create(property.GetMethod);
                                    GetterMethods[key] = fastMethod;
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void SetMethods(string[] methodNames)
        {
            Methods = new Dictionary<string, FastMethod>();
            foreach (var name in methodNames)
            {
                MethodInfo method = NumerousControllerInterfaceType.GetMethod("AtsExPlugin" + name);
                if (method != null)
                {
                    Methods.Add(name, FastMethod.Create(method));
                } else
                {
                    Methods = null;
                    break;
                }
            }
            if (Methods != null)
            {
                IsLoaded = true;
            }
        }

        private void Invoke(string methodName, params object[] args)
        {
            if (Methods != null)
            {
                Methods[methodName].Invoke(null, args);
            }
        }

        public override void Dispose()
        {
            Invoke("Disposed");
        }

        public override TickResult Tick(TimeSpan elapsed)
        {
            if (IsLoaded)
            {
                if (RetriveValues == null)
                {
                    List<string> useValueList = (List<string>)Methods["GetUseValueList"].Invoke(null, new object[0]);
                    RetriveValues = new List<string>();
                    RetriveValues.AddRange(useValueList);
                    PreValue = new Dictionary<string, object>();
                }
                if (PreValue != null)
                {
                    foreach (string key in RetriveValues)
                    {
                        object value = GetValue(key);
                        if (value != null)
                        {
                            if (PreValue.ContainsKey(key))
                            {
                                if (PreValue[key] != value)
                                {
                                    Invoke("ReportValueChanged", key, value);
                                }
                                PreValue[key] = value;
                            }
                            else
                            {
                                Invoke("ReportValueChanged", key, value);
                                PreValue.Add(key, value);
                            }
                        }
                    }
                }
            }
            return new ExtensionTickResult();
        }

        private object GetInstance(PropertyInfo key, PropertyInfo target)
        {
            if (target == null)
            {
                if (key.DeclaringType.Namespace == "BveTypes.ClassWrappers")
                {
                    return BveHacker;
                }
            }
            if (Getters.ContainsKey(GetKey(target)))
            {
                object instance = GetInstance(key, Getters[GetKey(target)]);
                if (instance != null)
                {
                    return GetterMethods[GetKey(target)].Invoke(instance, null);
                }
            }
            return null;
        }

        private object GetValue(string key)
        {
            if (key.StartsWith("Standard"))
            {
                if (key == "Standard.SpeedKmPerHour")
                {
                    return BveHacker.Scenario.LocationManager.SpeedMeterPerSecond * 3600 / 1000;
                }
                if (key == "Standard.DoorClosed")
                {
                    return BveHacker.Scenario.Vehicle.Doors.AreAllClosingOrClosed;
                }
                if (key == "Standard.BcPressure")
                {
                    if (BveHacker.Scenario.Vehicle.Instruments.PluginLoader.StateStore.BcPressure.Length > 0)
                        return BveHacker.Scenario.Vehicle.Instruments.PluginLoader.StateStore.BcPressure[0];
                }
                if (key == "Standard.BpPressure")
                {
                    if (BveHacker.Scenario.Vehicle.Instruments.PluginLoader.StateStore.BpPressure.Length > 0)
                        return BveHacker.Scenario.Vehicle.Instruments.PluginLoader.StateStore.BpPressure[0];
                }
                if (key == "Standard.Current")
                {
                    if (BveHacker.Scenario.Vehicle.Instruments.PluginLoader.StateStore.Current.Length > 0)
                        return BveHacker.Scenario.Vehicle.Instruments.PluginLoader.StateStore.Current[0];
                }
            } else
            {
                PropertyInfo info = RetrievableValuesFromKey[key];
                PropertyInfo instanceInfo = RetrievableValues[key];
                object instance = GetInstance(info, instanceInfo);
                if (instance == null)
                {
                    return null;
                }
                return info.GetValue(instance);
            }
            return null;
        }

        private string GetKey(PropertyInfo property)
        {
            if (property == null) return "null";
            return property.DeclaringType.Namespace + "." + property.DeclaringType.Name + "." + property.Name;
        }
    }
}
