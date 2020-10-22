using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class FindAndReplace : Form
    {
        private int position = 0;
        private int next = -1;
        private Form1 f1;
        private string toFind = "";
        private string replaceWith = "";

        public FindAndReplace(Form1 frm1)
        {
            InitializeComponent();
            this.TopMost = true;
            f1 = frm1;
        }

        private void FindNext()
        {
            position = next + 1;
            next = f1.richTextBox1.Text.ToUpper().IndexOf(toFind.ToUpper(), position);
            
            if (next != -1)
            {
                f1.richTextBox1.DeselectAll();
                f1.richTextBox1.Select(next, toFind.Length);
            }
            else if(next == -1 && position == 0)
            {
                MessageBox.Show("No more entries found.");
            }
            else if(next == -1 && position != 0)
            {
                if( MessageBox.Show("Reached end of file, do you wish to reset the search from the beginning?", "Find and Replace", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    position = 0;
                    next = -1;
                    FindNext();
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void findTextBox_TextChanged(object sender, EventArgs e)
        {
            toFind = findTextBox.Text;
            f1.richTextBox1.DeselectAll();
            next = -1;
            position = 0;


            if (toFind == "")
            {
                findNextButton.Enabled = false;
                replaceButton.Enabled = false;
                replaceAllButton.Enabled = false;
            }
            else
            {
                findNextButton.Enabled = true;
                replaceButton.Enabled = true;
                replaceAllButton.Enabled = true;
            }
        }

        private void replacecTextBox_TextChanged(object sender, EventArgs e)
        {
            replaceWith = replacecTextBox.Text;
        }

        private void findNextButton_Click(object sender, EventArgs e)
        {
            FindNext();
        }

        private void replaceButton_Click(object sender, EventArgs e)
        {
            if(next != -1)
            {
                f1.richTextBox1.Text = f1.richTextBox1.Text.Remove(next, toFind.Length);
                f1.richTextBox1.Text = f1.richTextBox1.Text.Insert(next, replaceWith);
                FindNext();
            }
            else
            {
                FindNext();
            }
        }

        private void replaceAllButton_Click(object sender, EventArgs e)
        {
            f1.richTextBox1.Text = f1.richTextBox1.Text.Replace(toFind, replaceWith);
            MessageBox.Show("All matches replaced.","Replace all", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
