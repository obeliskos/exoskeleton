using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exoskeleton
{
    public partial class LoggerForm : Form
    {
        private string lastCommand = "";

        public LoggerForm()
        {
            InitializeComponent();
        }

        public LoggerForm(string title) : this()
        {
            this.Text = title;
        }

        public void LogInfo(string source, string message)
        {
            ListViewItem lviMain = new ListViewItem();
            lviMain.Text = "Info";
            lviMain.ImageIndex = 0;

            ListViewItem.ListViewSubItem lviMessage = new ListViewItem.ListViewSubItem();
            lviMessage.Text = message;

            ListViewItem.ListViewSubItem lviSource = new ListViewItem.ListViewSubItem();
            lviSource.Text = source;

            lviMain.SubItems.Add(lviMessage);
            lviMain.SubItems.Add(lviSource);

            listViewLog.Items.Add(lviMain);
        }

        public void logError(string msg, string url, string line, string col, string error)
        {
            ListViewItem lviMain = new ListViewItem();
            lviMain.Text = "Error";
            lviMain.ImageIndex = 2;

            ListViewItem.ListViewSubItem lviSource = new ListViewItem.ListViewSubItem();
            lviSource.Text = url;

            ListViewItem.ListViewSubItem lviLine = new ListViewItem.ListViewSubItem();
            lviLine.Text = line;

            ListViewItem.ListViewSubItem lviCol = new ListViewItem.ListViewSubItem();
            lviCol.Text = col;

            ListViewItem.ListViewSubItem lviMessage = new ListViewItem.ListViewSubItem();
            lviMessage.Text = msg;

            lviMain.SubItems.Add(lviMessage);
            lviMain.SubItems.Add(lviSource);
            lviMain.SubItems.Add(lviLine);
            lviMain.SubItems.Add(lviCol);

            listViewLog.Items.Add(lviMain);
        }

        public void logWarning(string source, string message)
        {
            ListViewItem lviMain = new ListViewItem();
            lviMain.Text = "Warning";
            lviMain.ImageIndex = 1;

            ListViewItem.ListViewSubItem lviMessage = new ListViewItem.ListViewSubItem();
            lviMessage.Text = message;

            ListViewItem.ListViewSubItem lviSource = new ListViewItem.ListViewSubItem();
            lviSource.Text = source;

            lviMain.SubItems.Add(lviMessage);
            lviMain.SubItems.Add(lviSource);

            listViewLog.Items.Add(lviMain);
        }

        public void logText(string message)
        {
            txtConsole.AppendText(message + Environment.NewLine);
        }

        private void toolStripButtonClearLog_Click(object sender, EventArgs e)
        {
            listViewLog.Items.Clear();
        }

        private void toolStripButtonClearConsole_Click(object sender, EventArgs e)
        {
            txtConsole.Text = "";
        }

        private object WebInvokeScript(string name, params string[] args)
        {
            return MainForm.FormInstance.WebInvokeScript(name, args);
        }

        private void textConsoleEval_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Modifiers == Keys.Control || !textConsoleEval.Multiline) && e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                string cmd = textConsoleEval.Text;
                lastCommand = cmd;
                textConsoleEval.Text = "";
                this.logText(cmd);

                object result = WebInvokeScript("eval", cmd);

                if (result != null)
                {
                    this.logText(result.ToString());
                }
            }

            if ((e.Modifiers == Keys.Control || !textConsoleEval.Multiline) && e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                textConsoleEval.Text = lastCommand;
                textConsoleEval.SelectionStart = textConsoleEval.Text.Length;
                textConsoleEval.SelectionLength = 0;
            }
        }

        private void toolStripButtonToggleMultiline_Click(object sender, EventArgs e)
        {
            textConsoleEval.Multiline = !textConsoleEval.Multiline;
            if (textConsoleEval.Multiline)
            {
                textConsoleEval.Height = 120;
            }
        }
    }
}
