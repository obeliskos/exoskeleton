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
            this.RefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.HostStatusStrip = new System.Windows.Forms.StatusStrip();
            this.HostToolStrip = new System.Windows.Forms.ToolStrip();
            this.HostWebBrowser = new System.Windows.Forms.WebBrowser();
            this.toolStripLeftLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripRightLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.HostStatusStrip.SuspendLayout();
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
            this.ExoskeletonNotification.Icon = ((System.Drawing.Icon)(resources.GetObject("ExoskeletonNotification.Icon")));
            this.ExoskeletonNotification.Text = "Exoskeleton";
            this.ExoskeletonNotification.Visible = true;
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
            // HostWebBrowser
            // 
            this.HostWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HostWebBrowser.Location = new System.Drawing.Point(0, 25);
            this.HostWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.HostWebBrowser.Name = "HostWebBrowser";
            this.HostWebBrowser.ScriptErrorsSuppressed = true;
            this.HostWebBrowser.Size = new System.Drawing.Size(864, 447);
            this.HostWebBrowser.TabIndex = 10;
            this.HostWebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.HostWebBrowser_DocumentCompleted);
            this.HostWebBrowser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.HostWebBrowser_PreviewKeyDown);
            // 
            // toolStripLeftLabel
            // 
            this.toolStripLeftLabel.Name = "toolStripLeftLabel";
            this.toolStripLeftLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripRightLabel
            // 
            this.toolStripRightLabel.Name = "toolStripRightLabel";
            this.toolStripRightLabel.Size = new System.Drawing.Size(818, 17);
            this.toolStripRightLabel.Spring = true;
            this.toolStripRightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 472);
            this.Controls.Add(this.HostWebBrowser);
            this.Controls.Add(this.HostToolStrip);
            this.Controls.Add(this.HostStatusStrip);
            this.Controls.Add(this.HostMenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.HostStatusStrip.ResumeLayout(false);
            this.HostStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip HostMenuStrip;
        private System.Windows.Forms.NotifyIcon ExoskeletonNotification;
        private System.Windows.Forms.Timer RefreshTimer;
        private System.Windows.Forms.StatusStrip HostStatusStrip;
        private System.Windows.Forms.ToolStrip HostToolStrip;
        private System.Windows.Forms.WebBrowser HostWebBrowser;
        private System.Windows.Forms.ToolStripStatusLabel toolStripLeftLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripRightLabel;
    }
}