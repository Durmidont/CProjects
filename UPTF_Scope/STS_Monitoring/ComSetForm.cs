using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;


namespace UPTF_Scope
{
    public partial class ComSetForm : Form
    {
        public ComSetForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ComsetClass.ComPortIndex = (byte)comPortComboBox.SelectedIndex;
            ComsetClass.SerialPortSpeedIndex = (byte)speedComboBox.SelectedIndex;
            ComsetClass.SerialPortParityIndex = (byte)parityComboBox.SelectedIndex;

            

            ComsetClass.SaveSettingsToFile();
            Close();
        }

        private void ComSetForm_Load(object sender, EventArgs e)
        {
            this.Text = MainForm.ParameterName(4);
            label1.Text = MainForm.ParameterName(5);
            label2.Text = MainForm.ParameterName(6);
            label3.Text = MainForm.ParameterName(7);
           

            textBox1.Text = ComsetClass.ArhivePath;

            button1.Text = MainForm.ParameterName(8);
            button2.Text = MainForm.ParameterName(9);

            comPortComboBox.Items.Clear();
            foreach (string st in ComsetClass.PortList)
            {
                comPortComboBox.Items.Add(st);
            }
            if (comPortComboBox.Items.Count != 0) comPortComboBox.SelectedIndex = 0;

            
            
        }

        private void ComSetForm_Shown(object sender, EventArgs e)
        {
            if (ComsetClass.ComPortIndex >= comPortComboBox.Items.Count)
            {
                try
                {
                    comPortComboBox.SelectedIndex = 0;
                }
                catch { }
            }
            else
            {
                comPortComboBox.SelectedIndex = ComsetClass.ComPortIndex;
            }


            parityComboBox.SelectedIndex = ComsetClass.SerialPortParityIndex;
            speedComboBox.SelectedIndex = ComsetClass.SerialPortSpeedIndex;


            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fb = new FolderBrowserDialog())
            {
                fb.Description = MainForm.ParameterName(11);
                //fb.ShowNewFolderButton = false;
                if (ComsetClass.ArhivePath != "")
                {
                    fb.SelectedPath = ComsetClass.ArhivePath;
                }
                else
                {
                    fb.RootFolder = Environment.SpecialFolder.MyComputer;
                }

                if (fb.ShowDialog() == DialogResult.OK)
                {
                    ComsetClass.ArhivePath = fb.SelectedPath;
                    textBox1.Text = ComsetClass.ArhivePath;
                }
                
            };

        }
    }
}
