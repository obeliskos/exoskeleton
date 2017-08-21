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
            this.ChildWebBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // ChildWebBrowser
            // 
            this.ChildWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChildWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.ChildWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.ChildWebBrowser.Name = "ChildWebBrowser";
            this.ChildWebBrowser.ScriptErrorsSuppressed = true;
            this.ChildWebBrowser.Size = new System.Drawing.Size(704, 441);
            this.ChildWebBrowser.TabIndex = 0;
            this.ChildWebBrowser.WebBrowserShortcutsEnabled = false;
            this.ChildWebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.ChildWebBrowser_DocumentCompleted);
            this.ChildWebBrowser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ChildWebBrowser_PreviewKeyDown);
            // 
            // ChildWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 441);
            this.Controls.Add(this.ChildWebBrowser);
            this.KeyPreview = true;
            this.Name = "ChildWindow";
            this.Text = "ChildWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChildWindow_FormClosing);
            this.Load += new System.EventHandler(this.ChildWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser ChildWebBrowser;
    }
}