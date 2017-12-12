using Exoskeleton.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exoskeleton
{
    public partial class HistorySelectorForm : Form
    {
        public static string SelectedSettingsFile = "";

        IPrimaryHostWindow primaryHost;

        public HistorySelectorForm(IPrimaryHostWindow primaryHost, GlobalSettings globalSettings)
        {
            InitializeComponent();

            this.primaryHost = primaryHost;

            IconImageList.Images.Clear();

            foreach (ExoAppLaunchInfo ali in globalSettings.AppHistory.OrderByDescending(ah => ah.LastRun))
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = ali.ShortName;

                if (ali.IconPath != "" && File.Exists(ali.IconPath))
                {
                    Icon appIcon = new Icon(ali.IconPath);
                    IconImageList.Images.Add(appIcon);
                    lvi.ImageIndex = IconImageList.Images.Count - 1;
                    lvi.Tag = ali.SettingsPath;
                }
                else
                {
                    IconImageList.Images.Add(this.Icon);
                    lvi.ImageIndex = IconImageList.Images.Count - 1;
                    lvi.Tag = ali.SettingsPath;
                }

                AppHistoryListView.Items.Add(lvi);
            }
        }

        private void HistorySelectorForm_Load(object sender, EventArgs e)
        {
            if (AppHistoryListView.Items.Count > 0)
            {
                AppHistoryListView.SelectedIndices.Clear();
                AppHistoryListView.SelectedIndices.Add(0);
            }

            AppHistoryListView.Focus();
        }

        private void HistorySelectorForm_Activated(object sender, EventArgs e)
        {
            AppHistoryListView.Focus();
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            SelectedSettingsFile = "";

            if (AppHistoryListView.SelectedItems.Count > 0)
            {
                SelectedSettingsFile = (string)AppHistoryListView.SelectedItems[0].Tag;

                if (!File.Exists(SelectedSettingsFile))
                {
                    SelectedSettingsFile = "";
                    MessageBox.Show("The selected app's settings file is not available", "Error resolving path to settings file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void AppHistoryListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }

        }

        private void AppHistoryListView_DragDrop(object sender, DragEventArgs e)
        {
            List<string> failedFiles = new List<string>();

            try
            {
                string[] settingsFilenames = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string settingsFilename in settingsFilenames)
                {
                    FileInfo fi = new FileInfo(settingsFilename);
                    if (!fi.Extension.ToLower().Equals(".xos") && !fi.Extension.ToLower().Equals(".xml"))
                    {
                        failedFiles.Add(fi.Name);
                    }
                    else
                    {
                        Settings settings = Settings.Load(settingsFilename, false);

                        ExoAppLaunchInfo ali = new ExoAppLaunchInfo();

                        ali.SettingsPath = settingsFilename;
                        ali.IconPath = settings.WindowIconPath;
                        ali.ShortName = settings.ApplicationShortName;
                        ali.Description = settings.ApplicationDescription;

                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = ali.ShortName;

                        string revisedIconPath = ali.IconPath
                            .Replace("{CurrentLocation}", Environment.CurrentDirectory)
                            .Replace("{SettingsLocation}", fi.DirectoryName);

                        if (revisedIconPath != "" && File.Exists(revisedIconPath))
                        {
                            // Haven't quite loaded the settings file so calling IPrimaryHost.ResolveExoUrlPath 
                            // won't work.  For now just substitute manually.

                            Icon appIcon = new Icon(revisedIconPath);
                            IconImageList.Images.Add(appIcon);
                            lvi.ImageIndex = IconImageList.Images.Count - 1;
                            lvi.Tag = ali.SettingsPath;
                        }
                        else
                        {
                            IconImageList.Images.Add(this.Icon);
                            lvi.ImageIndex = IconImageList.Images.Count - 1;
                            lvi.Tag = ali.SettingsPath;
                        }

                        // We can however make sure the global settings knows about this 'history' even 
                        // if we don't select it immediately.
                        this.primaryHost.AddLaunchHistory(
                            ali.SettingsPath,
                            ali.ShortName,
                            ali.Description,
                            ali.IconPath
                        );

                        AppHistoryListView.Items.Add(lvi);

                        AppHistoryListView.SelectedIndices.Clear();
                        AppHistoryListView.SelectedIndices.Add(AppHistoryListView.Items.Count - 1);
                        AppHistoryListView.Focus();
                    }
                }

                if (settingsFilenames.Count() > 0)
                {
                    string files = String.Join(Environment.NewLine, failedFiles.ToArray());
                    MessageBox.Show(
                        files, 
                        "The following files are not .xos or .xml files", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error processing dropped file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AppHistoryListView_DoubleClick(object sender, EventArgs e)
        {
            SelectedSettingsFile = "";

            if (AppHistoryListView.SelectedItems.Count > 0)
            {
                SelectedSettingsFile = (string)AppHistoryListView.SelectedItems[0].Tag;

                if (!File.Exists(SelectedSettingsFile))
                {
                    SelectedSettingsFile = "";
                    MessageBox.Show("The selected app's settings file is not available", "Error resolving path to settings file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (SelectedSettingsFile != "")
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void AddSettingsButton_Click(object sender, EventArgs e)
        {
            DialogResult dr = SettingsSaveFileDialog.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string filename = SettingsSaveFileDialog.FileName;

                Settings newSettings = new Settings();
                newSettings.Save(filename);

                ListViewItem lvi = new ListViewItem();
                lvi.Text = newSettings.ApplicationShortName;
                IconImageList.Images.Add(this.Icon);
                lvi.ImageIndex = IconImageList.Images.Count - 1;
                lvi.Tag = filename;

                this.primaryHost.AddLaunchHistory(
                    filename,
                    newSettings.ApplicationShortName,
                    newSettings.ApplicationDescription,
                    newSettings.WindowIconPath
                );

                AppHistoryListView.Items.Add(lvi);

                AppHistoryListView.SelectedIndices.Clear();
                AppHistoryListView.SelectedIndices.Add(AppHistoryListView.Items.Count - 1);
                AppHistoryListView.Focus();
            }
        }

        private void RemoveSettingsButton_Click(object sender, EventArgs e)
        {
            if (AppHistoryListView.SelectedItems.Count > 0)
            {
                ListViewItem lvi = AppHistoryListView.SelectedItems[0];

                DialogResult dr = MessageBox.Show(
                    "Remove launch info for '" + lvi.Text + "' ?",
                    "Confirm Removal of Launch History",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dr == DialogResult.Yes)
                {
                    string selectedFilepath = (string)AppHistoryListView.SelectedItems[0].Tag;
                    primaryHost.RemoveLaunchHistory(selectedFilepath);
                    AppHistoryListView.Items.Remove(lvi);
                }
            }
        }
    }
}
