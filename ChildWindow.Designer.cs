namespace Exoskeleton
{
    partial class ChildWindow
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
            this.HostMenuStrip = new System.Windows.Forms.MenuStrip();
            this.HostStatusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripLeftLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripRightLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.HostToolStrip = new System.Windows.Forms.ToolStrip();
            this.HostPanel = new System.Windows.Forms.Panel();
            this.HostWebBrowser = new System.Windows.Forms.WebBrowser();
            this.HostStatusStrip.SuspendLayout();
            this.HostPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // HostMenuStrip
            // 
            this.HostMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.HostMenuStrip.Name = "HostMenuStrip";
            this.HostMenuStrip.Size = new System.Drawing.Size(704, 24);
            this.HostMenuStrip.TabIndex = 1;
            this.HostMenuStrip.Text = "menuStrip1";
            this.HostMenuStrip.Visible = false;
            // 
            // HostStatusStrip
            // 
            this.HostStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLeftLabel,
            this.toolStripRightLabel});
            this.HostStatusStrip.Location = new System.Drawing.Point(0, 419);
            this.HostStatusStrip.Name = "HostStatusStrip";
            this.HostStatusStrip.Size = new System.Drawing.Size(704, 22);
            this.HostStatusStrip.TabIndex = 3;
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
            this.toolStripRightLabel.Size = new System.Drawing.Size(689, 17);
            this.toolStripRightLabel.Spring = true;
            this.toolStripRightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // HostToolStrip
            // 
            this.HostToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.HostToolStrip.Location = new System.Drawing.Point(0, 0);
            this.HostToolStrip.Name = "HostToolStrip";
            this.HostToolStrip.Size = new System.Drawing.Size(704, 25);
            this.HostToolStrip.TabIndex = 4;
            this.HostToolStrip.Text = "toolStrip1";
            this.HostToolStrip.Visible = false;
            // 
            // HostPanel
            // 
            this.HostPanel.Controls.Add(this.HostWebBrowser);
            this.HostPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HostPanel.Location = new System.Drawing.Point(0, 0);
            this.HostPanel.Name = "HostPanel";
            this.HostPanel.Size = new System.Drawing.Size(704, 441);
            this.HostPanel.TabIndex = 6;
            // 
            // HostWebBrowser
            // 
            this.HostWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HostWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.HostWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.HostWebBrowser.Name = "HostWebBrowser";
            this.HostWebBrowser.ScriptErrorsSuppressed = true;
            this.HostWebBrowser.Size = new System.Drawing.Size(704, 441);
            this.HostWebBrowser.TabIndex = 6;
            this.HostWebBrowser.WebBrowserShortcutsEnabled = false;
            this.HostWebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.ChildWebBrowser_DocumentCompleted);
            this.HostWebBrowser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ChildWebBrowser_PreviewKeyDown);
            // 
            // ChildWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 441);
            this.Controls.Add(this.HostPanel);
            this.Controls.Add(this.HostToolStrip);
            this.Controls.Add(this.HostStatusStrip);
            this.Controls.Add(this.HostMenuStrip);
            this.KeyPreview = true;
            this.MainMenuStrip = this.HostMenuStrip;
            this.Name = "ChildWindow";
            this.Text = "ChildWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChildWindow_FormClosing);
            this.Load += new System.EventHandler(this.ChildWindow_Load);
            this.HostStatusStrip.ResumeLayout(false);
            this.HostStatusStrip.PerformLayout();
            this.HostPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip HostMenuStrip;
        private System.Windows.Forms.StatusStrip HostStatusStrip;
        private System.Windows.Forms.ToolStrip HostToolStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripLeftLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripRightLabel;
        private System.Windows.Forms.Panel HostPanel;
        private System.Windows.Forms.WebBrowser HostWebBrowser;
    }
}