namespace Exoskeleton
{
    partial class LoggerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoggerForm));
            this.imageListViewIcons = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listViewLog = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonClearLog = new System.Windows.Forms.ToolStripButton();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.textConsoleEval = new System.Windows.Forms.TextBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonClearConsole = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonToggleMultiline = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageListViewIcons
            // 
            this.imageListViewIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListViewIcons.ImageStream")));
            this.imageListViewIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListViewIcons.Images.SetKeyName(0, "console_info.png");
            this.imageListViewIcons.Images.SetKeyName(1, "console_warn.png");
            this.imageListViewIcons.Images.SetKeyName(2, "console_error.png");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewLog);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtConsole);
            this.splitContainer1.Panel2.Controls.Add(this.textConsoleEval);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip2);
            this.splitContainer1.Size = new System.Drawing.Size(919, 512);
            this.splitContainer1.SplitterDistance = 189;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 8;
            // 
            // listViewLog
            // 
            this.listViewLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader2,
            this.columnHeader4,
            this.columnHeader5});
            this.listViewLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewLog.FullRowSelect = true;
            this.listViewLog.Location = new System.Drawing.Point(0, 25);
            this.listViewLog.Margin = new System.Windows.Forms.Padding(4);
            this.listViewLog.Name = "listViewLog";
            this.listViewLog.Size = new System.Drawing.Size(919, 164);
            this.listViewLog.SmallImageList = this.imageListViewIcons;
            this.listViewLog.TabIndex = 2;
            this.listViewLog.UseCompatibleStateImageBehavior = false;
            this.listViewLog.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Type";
            this.columnHeader1.Width = 80;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Message";
            this.columnHeader3.Width = 260;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Source";
            this.columnHeader2.Width = 260;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Line";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Col";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonClearLog});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(919, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonClearLog
            // 
            this.toolStripButtonClearLog.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonClearLog.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonClearLog.Image")));
            this.toolStripButtonClearLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClearLog.Name = "toolStripButtonClearLog";
            this.toolStripButtonClearLog.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonClearLog.Text = "toolStripButton1";
            this.toolStripButtonClearLog.Click += new System.EventHandler(this.toolStripButtonClearLog_Click);
            // 
            // txtConsole
            // 
            this.txtConsole.BackColor = System.Drawing.Color.DimGray;
            this.txtConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConsole.Font = new System.Drawing.Font("Lucida Console", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsole.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtConsole.Location = new System.Drawing.Point(0, 39);
            this.txtConsole.Margin = new System.Windows.Forms.Padding(4);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConsole.Size = new System.Drawing.Size(919, 256);
            this.txtConsole.TabIndex = 3;
            // 
            // textConsoleEval
            // 
            this.textConsoleEval.BackColor = System.Drawing.Color.SeaShell;
            this.textConsoleEval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textConsoleEval.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textConsoleEval.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textConsoleEval.ForeColor = System.Drawing.Color.Black;
            this.textConsoleEval.Location = new System.Drawing.Point(0, 295);
            this.textConsoleEval.Margin = new System.Windows.Forms.Padding(4);
            this.textConsoleEval.Name = "textConsoleEval";
            this.textConsoleEval.Size = new System.Drawing.Size(919, 23);
            this.textConsoleEval.TabIndex = 2;
            this.textConsoleEval.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textConsoleEval_KeyDown);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonClearConsole,
            this.toolStripButtonToggleMultiline,
            this.toolStripLabel1});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(919, 39);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButtonClearConsole
            // 
            this.toolStripButtonClearConsole.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonClearConsole.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonClearConsole.Image")));
            this.toolStripButtonClearConsole.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClearConsole.Name = "toolStripButtonClearConsole";
            this.toolStripButtonClearConsole.Size = new System.Drawing.Size(23, 36);
            this.toolStripButtonClearConsole.Text = "Clear output";
            this.toolStripButtonClearConsole.ToolTipText = "Clear output";
            this.toolStripButtonClearConsole.Click += new System.EventHandler(this.toolStripButtonClearConsole_Click);
            // 
            // toolStripButtonToggleMultiline
            // 
            this.toolStripButtonToggleMultiline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonToggleMultiline.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonToggleMultiline.Image")));
            this.toolStripButtonToggleMultiline.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonToggleMultiline.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonToggleMultiline.Name = "toolStripButtonToggleMultiline";
            this.toolStripButtonToggleMultiline.Size = new System.Drawing.Size(36, 36);
            this.toolStripButtonToggleMultiline.Text = "Toggles multi-line input";
            this.toolStripButtonToggleMultiline.ToolTipText = "Toggles multi-line input";
            this.toolStripButtonToggleMultiline.Click += new System.EventHandler(this.toolStripButtonToggleMultiline_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripLabel1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic);
            this.toolStripLabel1.ForeColor = System.Drawing.Color.DarkGreen;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(335, 36);
            this.toolStripLabel1.Text = "* In multiline mode use CTRL+Enter to submit input.";
            // 
            // LoggerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 512);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "LoggerForm";
            this.Text = "LoggerForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList imageListViewIcons;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listViewLog;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonClearLog;
        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.TextBox textConsoleEval;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButtonClearConsole;
        private System.Windows.Forms.ToolStripButton toolStripButtonToggleMultiline;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
    }
}