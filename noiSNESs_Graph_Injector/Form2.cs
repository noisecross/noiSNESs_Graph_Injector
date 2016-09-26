using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace noiSNESs_Graph_Injector
{
    public partial class FormExportDialog : Form
    {
        public bool okToExport     = false;
        public int  pagesToExport  = 0;
        public int  nBytesToExport = 0;

        public FormExportDialog(int nPages)
        {
            InitializeComponent();

            if (nPages > 24)
                nPages = 24;

            for (int i = 1; i < nPages - 1; i++)
            {
                comboBoxPage.Items.Add(i.ToString("D2"));
            }
            comboBoxPage.SelectedIndex = 0;
        }



        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxPage.Enabled = false;
            numericUpDownOffset.Enabled = false;
        }



        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxPage.Enabled = true;
            numericUpDownOffset.Enabled = false;
        }



        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxPage.Enabled = false;
            numericUpDownOffset.Enabled = true;

        }



        private void buttonCancel_Click(object sender, EventArgs e)
        {
            okToExport = false;
            this.Close();
        }



        private void buttonOK_Click(object sender, EventArgs e)
        {
            okToExport = true;

            if (radioButton1.Checked)
            {
                pagesToExport = 1;
            }
            else if (radioButton2.Checked)
            {
                pagesToExport = comboBoxPage.SelectedIndex;
            }
            else if (radioButton3.Checked)
            {
                nBytesToExport = (int)numericUpDownOffset.Value;
            }
            else
            {
                /* WTF? */
                okToExport = true;
            }

            this.Close();
        }
    }
}
