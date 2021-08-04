using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kusaanko.Bvets.NumerousControllerInterface
{
    public partial class NewNameDialog : Form
    {
        private Func<string, bool> ret;
        public NewNameDialog(string initName, Func<string, bool> ret)
        {
            InitializeComponent();
            this.ret = ret;
            textBox1.Text = initName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                MessageBox.Show("使用できない文字が含まれています。", "NumerousControllerInterface");
            }
            else
            {
                if (ret.Invoke(textBox1.Text))
                {
                    Close();
                }
            }
        }
    }
}
