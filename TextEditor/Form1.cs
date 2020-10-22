using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        private string path = "";
        private bool unsavedChanges = false;
        private Font defaultFont = new Font(FontFamily.GenericSansSerif.Name, 11, FontStyle.Regular);
        
        public Form1()
        {
            InitializeComponent();
        }

        // maintain the state of the file as
        // saved/unsaved in order to 
        // 1) display the asterisk next to the name
        //    so the user knows the file has unsaved changes
        // 2) prompt the user to save the file if it's unsaved 
        //    and they want to create/open another one
        void SetStateUnsaved()
        {
            unsavedChanges = true;
            unsavedStatusLabel.Visible = true;
        }

        void SetStateSaved()
        {
            unsavedChanges = false;
            unsavedStatusLabel.Visible = false;
        }

        // rich text box actions
        // all of them except copy change 
        // the text, so they set the state to 
        // unsaved
        private void RichCut()
        {
            richTextBox1.Cut();
            SetStateUnsaved();
        }

        private void RichCopy()
        {
            richTextBox1.Copy();
        }

        private void RichPaste()
        {
            richTextBox1.Paste();
            SetStateUnsaved();
        }

        private void RichUndo()
        {
            richTextBox1.Undo();
            SetStateUnsaved();
        }

        private void RichRedo()
        {
            richTextBox1.Redo();
            SetStateUnsaved();
        }

        private void RichAlignLeft()
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
        }

        private void RichAlignCenter()
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
        }

        private void RichAlignRight()
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
        }

        private void ShowSaveError()
        {
            MessageBox.Show("There occured an error while saving the file, please try again!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Save as... command
        // used when Ctrl  Shift + S is used
        private void SaveWithDialog()
        {
            saveFileDialog1.FileName = "";
            saveFileDialog1.Filter = "Text file|*.txt";
            saveFileDialog1.Title = "Save file...";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    path = saveFileDialog1.FileName;
                    richTextBox1.SaveFile(path, RichTextBoxStreamType.PlainText);
                    pathStatusLabel.Text = path;
                    SetStateSaved();
                }
                catch(ArgumentException e)
                {
                    ShowSaveError();
                }
                catch(IOException e)
                {
                    ShowSaveError();
                }
            }
        }

        // Save
        // used when Ctrl + S is used, but if the file is 
        // unsaved it behaves like Ctrl + Shift + S and 
        // opens the save dialog
        private void SaveWithoutDialog()
        {
            if(path != "")
            {
                try
                {
                    richTextBox1.SaveFile(path, RichTextBoxStreamType.PlainText);
                    SetStateSaved();
                }
                catch (ArgumentException e)
                {
                    ShowSaveError();
                }
                catch (IOException e)
                {
                    ShowSaveError();
                }
            }
            else
            {
                SaveWithDialog();
            }
        }

        // prompt the user to save the file if it has unsaved
        // changes and either New file or Open file is called
        private void AskToSave()
        {
            if (unsavedChanges == true)
            {
                var result = MessageBox.Show("You have unsaved changes, do you wish to save?", "Text Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SaveWithoutDialog();
                }
            }
        }

        // create a new document
        // prompt the user to save
        // if the previous file
        // has unsaved changes
        private void NewDocument()
        {
            AskToSave();

            richTextBox1.Clear();
            richTextBox1.ClearUndo();
            path = "";
            SetStateSaved();
            pathStatusLabel.Text = "Untitled.txt";
        }

        // open a document and
        // prompt the user to save
        // if the previous file
        // has unsaved changes
        private void OpenDocument()
        {
            AskToSave();

            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Text files|*.txt";
            openFileDialog1.Title = "Open file...";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog1.FileName;
                pathStatusLabel.Text = path;
                SetStateSaved();

                try
                {
                    StreamReader sr = new StreamReader(path);
                    richTextBox1.Text = sr.ReadToEnd();
                    sr.Close();
                }
                catch(OutOfMemoryException e)
                {
                    MessageBox.Show("Cannot open the file - it is either too long, or your machine does not have enough memory to load it!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch(IOException e)
                {
                    MessageBox.Show("There occured an error while opening the file, please make sure it is not open in another process!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ChangeFont()
        {
            var result = fontDialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                richTextBox1.SelectionFont = fontDialog.Font;
                SetStateUnsaved();
            }
        }

        private void ChangeColor()
        {
            var result = colorDialog1.ShowDialog();
            if(result == DialogResult.OK)
            {
                richTextBox1.SelectionColor = colorDialog1.Color;
                SetStateUnsaved();
            }
        }

        void ResetFormatting()
        {
            fontDialog.Font = defaultFont;
            colorDialog1.Reset();
            richTextBox1.SelectionFont = fontDialog.Font;
            richTextBox1.SelectionColor = colorDialog1.Color;
            richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
        }

        // EVENT HANDLERS
        // For reusability, each of the event handlers 
        // just calls a predefined method 


        // Whenever the user types anything in the textbox, the state is set 
        // to unsaved
        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            SetStateUnsaved();
        }
        // ---------------------------------------------------

        // TOOL STRIP
        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            RichCut();
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            RichCopy();
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            RichPaste();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            NewDocument();
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            RichUndo();
        }

        private void redoButton_Click(object sender, EventArgs e)
        {
            RichRedo();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenDocument();
        }

        private void leftToolStripButton_Click(object sender, EventArgs e)
        {
            RichAlignLeft();
        }

        private void centerToolStripButton_Click(object sender, EventArgs e)
        {
            RichAlignCenter();
        }

        private void rightToolStripButton_Click(object sender, EventArgs e)
        {
            RichAlignRight();
        }

        private void fontToolStripButton_Click(object sender, EventArgs e)
        {
            ChangeFont();
        }

        private void colourToolStripButton_Click(object sender, EventArgs e)
        {
            ChangeColor();
        }
        // ---------------------------------------------------

        // MENU STRIP
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichUndo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichRedo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichCut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichCopy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichPaste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveWithDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveWithoutDialog();
        }
        
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewDocument();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDocument();
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeFont();
        }

        private void resetAllFormattingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetFormatting();
        }

        private void colourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeColor();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // TODO
        // ---------------------------------------------------

        // FORM EVENTS
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            AskToSave();
        }

        private void insertCurrentDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.AppendText(dateTimePicker1.Value.ToString());
        }

        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fr = new FindAndReplace(this);
            fr.Show();
        }
        // --------------------------------------------------- 

        // CONTEXT MENU
        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RichCut();
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RichCopy();
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RichPaste();
        }

        private void fontToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ChangeFont();
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeColor();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var howtouse = new HowToUse();
            howtouse.ShowDialog();
        }
        
    }
}
