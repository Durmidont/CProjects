using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UPTF_Scope
{
    public partial class FormExit : Form
    {
        public FormExit()
        {
            InitializeComponent();
            buttonClose.Text = MainForm.ParameterName(9);
            buttonHide.Text = MainForm.ParameterNameMultiString(24);
            buttonTurnOff.Text = MainForm.ParameterNameMultiString(25);
            buttonExit.Text = MainForm.ParameterNameMultiString(3);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }

        private void buttonTurnOff_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void buttonHide_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
        }
    }
}
