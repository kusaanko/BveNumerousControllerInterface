using Kusaanko.Bvets.NumerousControllerInterface;
using Kusaanko.Bvets.NumerousControllerInterface.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NumerousControllerTestPlugin
{
    public class TestPlugin : NumerousControllerPlugin
    {
        public void Dispose()
        {

        }

        public List<NCIController> GetAllControllers()
        {
            List<NCIController> controllers = new List<NCIController>();
            controllers.Add(new Controller());
            return controllers;
        }

        public string GetName()
        {
            return "TestPlugin";
        }

        public string GetVersion()
        {
            return "1.0";
        }

        public void Load()
        {

        }

        public void LoadConfig(string directory)
        {
            MessageBox.Show(directory);
        }

        public void ShowConfigForm()
        {
            new Form1().Show();
        }
    }
}
