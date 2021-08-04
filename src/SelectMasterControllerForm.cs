using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public partial class SelectMasterControllerForm : Form
    {
        private Action<string> action;
        public SelectMasterControllerForm(List<string> controllers, Action<string> action)
        {
            InitializeComponent();
            controllerList.Items.AddRange(controllers.ToArray());
            controllerList.SelectedIndex = 0;
            this.action = action;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            action.Invoke(controllerList.SelectedItem.ToString());
            Close();
        }
    }
}
