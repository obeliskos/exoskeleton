namespace Exoskeleton
{
    partial class HistorySelectorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistorySelectorForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ExitButton = new System.Windows.Forms.Button();
            this.SelectButton = new System.Windows.Forms.Button();
            this.AppHistoryListView = new System.Windows.Forms.ListView();
            this.IconImageList = new System.Windows.Forms.ImageList(this.components);
            this.RemoveSettingsButton = new System.Windows.Forms.Button();
            this.AddSettingsButton = new System.Windows.Forms.Button();
            this.SettingsSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AllowDrop = true;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(644, 68);
            this.panel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(277, 21);
            this.label1.TabIndex = 7;
            this.label1.Text = "Select an application from history :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(419, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(211, 19);
            this.label2.TabIndex = 1;
            this.label2.Text = "(Or Drag-and-Drop settings file)";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.RemoveSettingsButton);
            this.panel2.Controls.Add(this.AddSettingsButton);
            this.panel2.Controls.Add(this.ExitButton);
            this.panel2.Controls.Add(this.SelectButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 422);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(644, 62);
            this.panel2.TabIndex = 3;
            // 
            // ExitButton
            // 
            this.ExitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExitButton.ForeColor = System.Drawing.Color.Black;
            this.ExitButton.Location = new System.Drawing.Point(515, 10);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(110, 37);
            this.ExitButton.TabIndex = 3;
            this.ExitButton.Text = "Exit";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // SelectButton
            // 
            this.SelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectButton.ForeColor = System.Drawing.Color.Black;
            this.SelectButton.Location = new System.Drawing.Point(280, 10);
            this.SelectButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(216, 37);
            this.SelectButton.TabIndex = 2;
            this.SelectButton.Text = "Load selected Application";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // AppHistoryListView
            // 
            this.AppHistoryListView.AllowDrop = true;
            this.AppHistoryListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AppHistoryListView.HideSelection = false;
            this.AppHistoryListView.LargeImageList = this.IconImageList;
            this.AppHistoryListView.Location = new System.Drawing.Point(0, 68);
            this.AppHistoryListView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AppHistoryListView.Name = "AppHistoryListView";
            this.AppHistoryListView.Size = new System.Drawing.Size(644, 354);
            this.AppHistoryListView.SmallImageList = this.IconImageList;
            this.AppHistoryListView.TabIndex = 7;
            this.AppHistoryListView.UseCompatibleStateImageBehavior = false;
            this.AppHistoryListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.AppHistoryListView_DragDrop);
            this.AppHistoryListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.AppHistoryListView_DragEnter);
            this.AppHistoryListView.DoubleClick += new System.EventHandler(this.AppHistoryListView_DoubleClick);
            // 
            // IconImageList
            // 
            this.IconImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.IconImageList.ImageSize = new System.Drawing.Size(64, 64);
            this.IconImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // RemoveSettingsButton
            // 
            this.RemoveSettingsButton.Image = global::Exoskeleton.Properties.Resources.delete;
            this.RemoveSettingsButton.Location = new System.Drawing.Point(72, 10);
            this.RemoveSettingsButton.Name = "RemoveSettingsButton";
            this.RemoveSettingsButton.Size = new System.Drawing.Size(41, 41);
            this.RemoveSettingsButton.TabIndex = 5;
            this.RemoveSettingsButton.UseVisualStyleBackColor = true;
            this.RemoveSettingsButton.Click += new System.EventHandler(this.RemoveSettingsButton_Click);
            // 
            // AddSettingsButton
            // 
            this.AddSettingsButton.Image = global::Exoskeleton.Properties.Resources.add;
            this.AddSettingsButton.Location = new System.Drawing.Point(16, 10);
            this.AddSettingsButton.Name = "AddSettingsButton";
            this.AddSettingsButton.Size = new System.Drawing.Size(41, 41);
            this.AddSettingsButton.TabIndex = 4;
            this.AddSettingsButton.UseVisualStyleBackColor = true;
            this.AddSettingsButton.Click += new System.EventHandler(this.AddSettingsButton_Click);
            // 
            // SettingsSaveFileDialog
            // 
            this.SettingsSaveFileDialog.FileName = "settings.xos";
            this.SettingsSaveFileDialog.Filter = "Exoskeleton Settings|*.xos";
            // 
            // HistorySelectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(644, 484);
            this.Controls.Add(this.AppHistoryListView);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "HistorySelectorForm";
            this.Text = "Exoskeleton Launcher";
            this.Activated += new System.EventHandler(this.HistorySelectorForm_Activated);
            this.Load += new System.EventHandler(this.HistorySelectorForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button SelectButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView AppHistoryListView;
        private System.Windows.Forms.ImageList IconImageList;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Button RemoveSettingsButton;
        private System.Windows.Forms.Button AddSettingsButton;
        private System.Windows.Forms.SaveFileDialog SettingsSaveFileDialog;
    }
}