namespace Exoskeleton
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.HostMenuStrip = new System.Windows.Forms.MenuStrip();
            this.ExoskeletonNotification = new System.Windows.Forms.NotifyIcon(this.components);
            this.notificationMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.consoleLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notificationIconToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.HostStatusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripLeftLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripRightLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.HostToolStrip = new System.Windows.Forms.ToolStrip();
            this.HostPanel = new System.Windows.Forms.Panel();
            this.HostWebBrowser = new System.Windows.Forms.WebBrowser();
            this.notificationMenuStrip.SuspendLayout();
            this.HostStatusStrip.SuspendLayout();
            this.HostPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // HostMenuStrip
            // 
            this.HostMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.HostMenuStrip.Name = "HostMenuStrip";
            this.HostMenuStrip.Size = new System.Drawing.Size(864, 24);
            this.HostMenuStrip.TabIndex = 4;
            this.HostMenuStrip.Text = "HostMenuStrip";
            this.HostMenuStrip.Visible = false;
            // 
            // ExoskeletonNotification
            // 
            this.ExoskeletonNotification.ContextMenuStrip = this.notificationMenuStrip;
            this.ExoskeletonNotification.Icon = ((System.Drawing.Icon)(resources.GetObject("ExoskeletonNotification.Icon")));
            this.ExoskeletonNotification.Text = "Exoskeleton";
            this.ExoskeletonNotification.Visible = true;
            this.ExoskeletonNotification.BalloonTipClicked += new System.EventHandler(this.ExoskeletonNotification_BalloonTipClicked);
            // 
            // notificationMenuStrip
            // 
            this.notificationMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.consoleLogToolStripMenuItem,
            this.notificationIconToolStripSeparator,
            this.exitToolStripMenuItem});
            this.notificationMenuStrip.Name = "notificationMenuStrip";
            this.notificationMenuStrip.Size = new System.Drawing.Size(158, 76);
            // 
            // consoleLogToolStripMenuItem
            // 
            this.consoleLogToolStripMenuItem.Name = "consoleLogToolStripMenuItem";
            this.consoleLogToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.consoleLogToolStripMenuItem.Text = "Console Logger";
            this.consoleLogToolStripMenuItem.Click += new System.EventHandler(this.consoleLogToolStripMenuItem_Click);
            // 
            // notificationIconToolStripSeparator
            // 
            this.notificationIconToolStripSeparator.Name = "notificationIconToolStripSeparator";
            this.notificationIconToolStripSeparator.Size = new System.Drawing.Size(154, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // RefreshTimer
            // 
            this.RefreshTimer.Interval = 60000;
            this.RefreshTimer.Tick += new System.EventHandler(this.RefreshTimer_Tick);
            // 
            // HostStatusStrip
            // 
            this.HostStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLeftLabel,
            this.toolStripRightLabel});
            this.HostStatusStrip.Location = new System.Drawing.Point(0, 450);
            this.HostStatusStrip.Name = "HostStatusStrip";
            this.HostStatusStrip.Size = new System.Drawing.Size(864, 22);
            this.HostStatusStrip.TabIndex = 8;
            this.HostStatusStrip.Text = "statusStrip1";
            this.HostStatusStrip.Visible = false;
            // 
            // toolStripLeftLabel
            // 
            this.toolStripLeftLabel.Name = "toolStripLeftLabel";
            this.toolStripLeftLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripRightLabel
            // 
            this.toolStripRightLabel.Name = "toolStripRightLabel";
            this.toolStripRightLabel.Size = new System.Drawing.Size(849, 17);
            this.toolStripRightLabel.Spring = true;
            this.toolStripRightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // HostToolStrip
            // 
            this.HostToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.HostToolStrip.Location = new System.Drawing.Point(0, 0);
            this.HostToolStrip.Name = "HostToolStrip";
            this.HostToolStrip.Size = new System.Drawing.Size(864, 25);
            this.HostToolStrip.TabIndex = 9;
            this.HostToolStrip.Text = "toolStrip1";
            this.HostToolStrip.Visible = false;
            // 
            // HostPanel
            // 
            this.HostPanel.Controls.Add(this.HostWebBrowser);
            this.HostPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HostPanel.Location = new System.Drawing.Point(0, 0);
            this.HostPanel.Name = "HostPanel";
            this.HostPanel.Size = new System.Drawing.Size(864, 472);
            this.HostPanel.TabIndex = 11;
            // 
            // HostWebBrowser
            // 
            this.HostWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HostWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.HostWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.HostWebBrowser.Name = "HostWebBrowser";
            this.HostWebBrowser.ScriptErrorsSuppressed = true;
            this.HostWebBrowser.Size = new System.Drawing.Size(864, 472);
            this.HostWebBrowser.TabIndex = 11;
            this.HostWebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.HostWebBrowser_DocumentCompleted);
            this.HostWebBrowser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.HostWebBrowser_PreviewKeyDown);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(864, 472);
            this.Controls.Add(this.HostPanel);
            this.Controls.Add(this.HostStatusStrip);
            this.Controls.Add(this.HostToolStrip);
            this.Controls.Add(this.HostMenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.notificationMenuStrip.ResumeLayout(false);
            this.HostStatusStrip.ResumeLayout(false);
            this.HostStatusStrip.PerformLayout();
            this.HostPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip HostMenuStrip;
        private System.Windows.Forms.NotifyIcon ExoskeletonNotification;
        private System.Windows.Forms.Timer RefreshTimer;
        private System.Windows.Forms.StatusStrip HostStatusStrip;
        private System.Windows.Forms.ToolStrip HostToolStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripLeftLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripRightLabel;
        private System.Windows.Forms.Panel HostPanel;
        private System.Windows.Forms.WebBrowser HostWebBrowser;
        private System.Windows.Forms.ContextMenuStrip notificationMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem consoleLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator notificationIconToolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}