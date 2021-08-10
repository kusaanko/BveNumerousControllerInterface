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
        private Action<string> _action;
        public SelectMasterControllerForm(List<string> controllers, string type, Action<string> action)
        {
            InitializeComponent();
            controllerList.Items.AddRange(controllers.ToArray());
            controllerList.SelectedIndex = 0;
            this._action = action;
            this.Text = type + "を選択";
            label1.Text = type + "が複数検出されました。\n使用するマスコンを選択して下さい。";
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _action.Invoke(controllerList.SelectedItem.ToString());
            Close();
        }
    }
}
