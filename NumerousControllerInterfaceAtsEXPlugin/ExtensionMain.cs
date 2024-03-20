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

namespace Kusaanko.Bvets.NumerousControllerInterface.AtsEXPlugin
{
    [PluginType(PluginType.Extension)]
    public class ExtensionMain : AssemblyPluginBase, IExtension
    {
        private Assembly NumerousControllerInterfaceAssembly;
        private Type NumerousControllerInterfaceType;
        private Dictionary<string, FastMethod> Methods;
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
                };
                SetMethods(methods);
                // NumerousController側にAtsExPluginの起動完了を通知
                Invoke("SetVersion", Assembly.GetExecutingAssembly().GetName().Version);
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
            return new ExtensionTickResult();
        }
    }
}
