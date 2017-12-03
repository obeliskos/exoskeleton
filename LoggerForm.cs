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
    public partial class LoggerForm : Form, ILogWindow
    {
        private string lastCommand = "";
        private string selectedWindowTitle {
            get
            {
                return HostWindowCombo.SelectedItem.ToString();
            }
        }

        private IHostWindow selectedHost {
            get {
                return hostDictionary[selectedWindowTitle];
            }
        }

        Dictionary<string, List<ListViewItem>> itemDictionary;
        Dictionary<string, StringBuilder> consoleDictionary;
        Dictionary<string, IHostWindow> hostDictionary;

        public LoggerForm()
        {
            InitializeComponent();

            itemDictionary = new Dictionary<string, List<ListViewItem>>();
            consoleDictionary = new Dictionary<string, StringBuilder>();
            hostDictionary = new Dictionary<string, IHostWindow>();

            HostWindowCombo.Items.Clear();
            HostWindowCombo.Items.Add("All");
            HostWindowCombo.SelectedItem = "All";
        }

        public void AttachHost(IHostWindow host, string title, Icon icon = null)
        {
            hostDictionary[title] = host;
            itemDictionary[title] = new List<ListViewItem>();
            consoleDictionary[title] = new StringBuilder();

            HostWindowCombo.Items.Add(title);

            if (icon != null)
            {
                this.Icon = icon;
            }
        }

        public void LogInfo(IHostWindow host, string source, string message)
        {
            ListViewItem lviMain = new ListViewItem();
            lviMain.Text = "Info";
            lviMain.ImageIndex = 0;

            ListViewItem.ListViewSubItem lviHostWindow = new ListViewItem.ListViewSubItem();
            lviHostWindow.Text = host.Title;

            ListViewItem.ListViewSubItem lviMessage = new ListViewItem.ListViewSubItem();
            lviMessage.Text = message;

            ListViewItem.ListViewSubItem lviSource = new ListViewItem.ListViewSubItem();
            lviSource.Text = source;

            lviMain.SubItems.Add(lviHostWindow);
            lviMain.SubItems.Add(lviMessage);
            lviMain.SubItems.Add(lviSource);

            itemDictionary[host.Title].Add(lviMain);
            if (selectedWindowTitle == "All" || selectedWindowTitle == host.Title)
            {
                listViewLog.Items.Add(lviMain);
            }

            if (WindowState == FormWindowState.Minimized && host.Settings.NotifyOnLoggedInfo)
            {
                host.ShowNotification(1000, "Console Info", message, ToolTipIcon.Info);
            }
        }

        public void logError(IHostWindow host, string msg, string url, string line, string col, string error)
        {
            ListViewItem lviMain = new ListViewItem();
            lviMain.Text = "Error";
            lviMain.ImageIndex = 2;

            ListViewItem.ListViewSubItem lviHostWindow = new ListViewItem.ListViewSubItem();
            lviHostWindow.Text = host.Title;

            ListViewItem.ListViewSubItem lviSource = new ListViewItem.ListViewSubItem();
            lviSource.Text = url;

            ListViewItem.ListViewSubItem lviLine = new ListViewItem.ListViewSubItem();
            lviLine.Text = line;

            ListViewItem.ListViewSubItem lviCol = new ListViewItem.ListViewSubItem();
            lviCol.Text = col;

            ListViewItem.ListViewSubItem lviMessage = new ListViewItem.ListViewSubItem();
            lviMessage.Text = msg;

            lviMain.SubItems.Add(lviHostWindow);
            lviMain.SubItems.Add(lviMessage);
            lviMain.SubItems.Add(lviSource);
            lviMain.SubItems.Add(lviLine);
            lviMain.SubItems.Add(lviCol);

            itemDictionary[host.Title].Add(lviMain);
            if (selectedWindowTitle == "All" || selectedWindowTitle == host.Title)
            {
                listViewLog.Items.Add(lviMain);
            }

            if (WindowState == FormWindowState.Minimized && host.Settings.NotifyOnLoggedErrors)
            {
                host.ShowNotification(1000, "Console Error", msg, ToolTipIcon.Error);
            }
        }

        public void logWarning(IHostWindow host, string source, string message)
        {
            ListViewItem lviMain = new ListViewItem();
            lviMain.Text = "Warning";
            lviMain.ImageIndex = 1;

            ListViewItem.ListViewSubItem lviHostWindow = new ListViewItem.ListViewSubItem();
            lviHostWindow.Text = host.Title;

            ListViewItem.ListViewSubItem lviMessage = new ListViewItem.ListViewSubItem();
            lviMessage.Text = message;

            ListViewItem.ListViewSubItem lviSource = new ListViewItem.ListViewSubItem();
            lviSource.Text = source;

            lviMain.SubItems.Add(lviHostWindow);
            lviMain.SubItems.Add(lviMessage);
            lviMain.SubItems.Add(lviSource);

            itemDictionary[host.Title].Add(lviMain);
            if (selectedWindowTitle == "All" || selectedWindowTitle == host.Title)
            {
                listViewLog.Items.Add(lviMain);
            }

            if (WindowState == FormWindowState.Minimized && host.Settings.NotifyOnLoggedWarnings)
            {
                host.ShowNotification(1000, "Console Warning", message, ToolTipIcon.Warning);
            }
        }

        public void logText(IHostWindow host, string message)
        {
            // Append to StringBuilder in dictionary
            consoleDictionary[host.Title].Append(message + Environment.NewLine);

            // If the window being logged to is selected in the dropdown filter, add to ui textbox
            if (selectedWindowTitle == "All" || selectedWindowTitle == host.Title)
            {
                txtConsole.AppendText(message + Environment.NewLine);
            }
        }

        private void toolStripButtonClearLog_Click(object sender, EventArgs e)
        {
            foreach (string title in itemDictionary.Keys)
            {
                if (selectedWindowTitle == "All" || selectedWindowTitle == title)
                {
                    // clear List<ListViewItem> in dictionary
                    itemDictionary[title].Clear();
                    // clear current listview ui element
                    listViewLog.Items.Clear();
                }
            }
        }

        private void toolStripButtonClearConsole_Click(object sender, EventArgs e)
        {
            foreach (string title in consoleDictionary.Keys)
            {
                if (selectedWindowTitle == "All" || selectedWindowTitle == title)
                {
                    // clear StringBuilder in dictionary
                    consoleDictionary[title].Clear();
                    // clear current ui element
                    txtConsole.Text = "";
                }
            }
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

                this.logText(this.selectedHost, cmd);

                object result = hostDictionary[selectedWindowTitle].InvokeScript("eval", new string[] { cmd });

                if (result != null)
                {
                    this.logText(hostDictionary[selectedWindowTitle], result.ToString());
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

        private void reloadLog()
        {
            listViewLog.Items.Clear();

            string sli = selectedWindowTitle;
            foreach(string key in itemDictionary.Keys)
            {
                if (sli == "All" || key == sli)
                {
                    List<ListViewItem> lvic = itemDictionary[key];
                    foreach (ListViewItem lvi in lvic)
                    {
                        listViewLog.Items.Add(lvi);
                    }
                }
            }
        }

        private void reloadConsole()
        {
            txtConsole.Text = "";

            string sli = selectedWindowTitle;
            foreach (string key in itemDictionary.Keys)
            {
                if (sli == "All" || key == sli)
                {
                    txtConsole.AppendText(consoleDictionary[key].ToString());
                }
            }
        }

        private void HostWindowCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            reloadLog();
            reloadConsole();

            textConsoleEval.Enabled = (selectedWindowTitle != "All");
            textConsoleEval.Text = (textConsoleEval.Enabled) ? "" : "[ Select a specific window before entering console commands ]";
        }

        private void LoggerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
        }
    }
}
