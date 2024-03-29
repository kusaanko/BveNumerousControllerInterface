﻿using System;
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
        private Func<string, bool> _ret;
        public NewNameDialog(string initName, Func<string, bool> ret)
        {
            InitializeComponent();
            this._ret = ret;
            textBox1.Text = initName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_ret.Invoke(textBox1.Text))
            {
                Close();
            }
        }
    }
}
