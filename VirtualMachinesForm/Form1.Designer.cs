namespace VirtualMachinesForm
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.ribbon1 = new System.Windows.Forms.Ribbon();
            this.ribbonTab1 = new System.Windows.Forms.RibbonTab();
            this.ribbonPanel1 = new System.Windows.Forms.RibbonPanel();
            this.addVMButton = new System.Windows.Forms.RibbonButton();
            this.deleteVMButton = new System.Windows.Forms.RibbonButton();
            this.startVMButton = new System.Windows.Forms.RibbonButton();
            this.stopVMButton = new System.Windows.Forms.RibbonButton();
            this.stopAllButton = new System.Windows.Forms.RibbonButton();
            this.ribbonButton1 = new System.Windows.Forms.RibbonButton();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.ribbonPanel4 = new System.Windows.Forms.RibbonPanel();
            this.ribbonButton6 = new System.Windows.Forms.RibbonButton();
            this.ribbonButton5 = new System.Windows.Forms.RibbonButton();
            this.ribbonButton4 = new System.Windows.Forms.RibbonButton();
            this.ribbonTab2 = new System.Windows.Forms.RibbonTab();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ribbon1
            // 
            this.ribbon1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ribbon1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ribbon1.Location = new System.Drawing.Point(0, 0);
            this.ribbon1.Minimized = false;
            this.ribbon1.Name = "ribbon1";
            // 
            // 
            // 
            this.ribbon1.OrbDropDown.BorderRoundness = 8;
            this.ribbon1.OrbDropDown.Location = new System.Drawing.Point(0, 0);
            this.ribbon1.OrbDropDown.Name = "";
            this.ribbon1.OrbDropDown.Size = new System.Drawing.Size(527, 447);
            this.ribbon1.OrbDropDown.TabIndex = 0;
            this.ribbon1.OrbImage = null;
            this.ribbon1.OrbStyle = System.Windows.Forms.RibbonOrbStyle.Office_2013;
            this.ribbon1.OrbVisible = false;
            this.ribbon1.PanelCaptionHeight = 1;
            // 
            // 
            // 
            this.ribbon1.QuickAcessToolbar.DropDownButtonVisible = false;
            this.ribbon1.QuickAcessToolbar.Enabled = false;
            this.ribbon1.QuickAcessToolbar.Visible = false;
            this.ribbon1.RibbonTabFont = new System.Drawing.Font("Trebuchet MS", 9F);
            this.ribbon1.Size = new System.Drawing.Size(883, 111);
            this.ribbon1.TabIndex = 0;
            this.ribbon1.Tabs.Add(this.ribbonTab1);
            this.ribbon1.TabsMargin = new System.Windows.Forms.Padding(12, 2, 20, 0);
            this.ribbon1.Text = "ribbon1";
            this.ribbon1.ThemeColor = System.Windows.Forms.RibbonTheme.Blue;
            // 
            // ribbonTab1
            // 
            this.ribbonTab1.Panels.Add(this.ribbonPanel1);
            this.ribbonTab1.Text = "WM";
            // 
            // ribbonPanel1
            // 
            this.ribbonPanel1.ButtonMoreVisible = false;
            this.ribbonPanel1.Items.Add(this.addVMButton);
            this.ribbonPanel1.Items.Add(this.deleteVMButton);
            this.ribbonPanel1.Items.Add(this.startVMButton);
            this.ribbonPanel1.Items.Add(this.stopVMButton);
            this.ribbonPanel1.Items.Add(this.stopAllButton);
            this.ribbonPanel1.Items.Add(this.ribbonButton1);
            this.ribbonPanel1.Text = "";
            // 
            // addVMButton
            // 
            this.addVMButton.Image = ((System.Drawing.Image)(resources.GetObject("addVMButton.Image")));
            this.addVMButton.SmallImage = ((System.Drawing.Image)(resources.GetObject("addVMButton.SmallImage")));
            this.addVMButton.Text = "Добавить";
            this.addVMButton.Click += new System.EventHandler(this.addVMButton_Click);
            // 
            // deleteVMButton
            // 
            this.deleteVMButton.Image = ((System.Drawing.Image)(resources.GetObject("deleteVMButton.Image")));
            this.deleteVMButton.SmallImage = ((System.Drawing.Image)(resources.GetObject("deleteVMButton.SmallImage")));
            this.deleteVMButton.Text = "Удалить";
            this.deleteVMButton.Click += new System.EventHandler(this.deleteVMButton_Click);
            // 
            // startVMButton
            // 
            this.startVMButton.Image = ((System.Drawing.Image)(resources.GetObject("startVMButton.Image")));
            this.startVMButton.SmallImage = ((System.Drawing.Image)(resources.GetObject("startVMButton.SmallImage")));
            this.startVMButton.Text = "Запустить";
            this.startVMButton.Click += new System.EventHandler(this.startVMButton_Click);
            // 
            // stopVMButton
            // 
            this.stopVMButton.Image = ((System.Drawing.Image)(resources.GetObject("stopVMButton.Image")));
            this.stopVMButton.SmallImage = ((System.Drawing.Image)(resources.GetObject("stopVMButton.SmallImage")));
            this.stopVMButton.Text = "Остановить";
            this.stopVMButton.Click += new System.EventHandler(this.stopVMButton_Click);
            // 
            // stopAllButton
            // 
            this.stopAllButton.Image = ((System.Drawing.Image)(resources.GetObject("stopAllButton.Image")));
            this.stopAllButton.SmallImage = ((System.Drawing.Image)(resources.GetObject("stopAllButton.SmallImage")));
            this.stopAllButton.Text = "Остановить все";
            this.stopAllButton.Click += new System.EventHandler(this.stopAllButton_Click);
            // 
            // ribbonButton1
            // 
            this.ribbonButton1.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButton1.Image")));
            this.ribbonButton1.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton1.SmallImage")));
            this.ribbonButton1.Text = "Подключиться";
            this.ribbonButton1.Click += new System.EventHandler(this.rdpButton_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(875, 306);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "VM";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(869, 300);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView_DataBindingComplete);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 111);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(883, 332);
            this.tabControl1.TabIndex = 1;
            // 
            // ribbonPanel4
            // 
            this.ribbonPanel4.ButtonMoreVisible = false;
            this.ribbonPanel4.Text = "";
            // 
            // ribbonButton6
            // 
            this.ribbonButton6.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButton6.Image")));
            this.ribbonButton6.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton6.SmallImage")));
            this.ribbonButton6.Text = "Редактировать";
            // 
            // ribbonButton5
            // 
            this.ribbonButton5.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButton5.Image")));
            this.ribbonButton5.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton5.SmallImage")));
            this.ribbonButton5.Text = "Удалить";
            // 
            // ribbonButton4
            // 
            this.ribbonButton4.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButton4.Image")));
            this.ribbonButton4.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton4.SmallImage")));
            this.ribbonButton4.Text = "Добавить";
            // 
            // ribbonTab2
            // 
            this.ribbonTab2.Panels.Add(this.ribbonPanel4);
            this.ribbonTab2.Text = "Профиль";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 443);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.ribbon1);
            this.Name = "Form1";
            this.Text = "VMManager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Ribbon ribbon1;
        private System.Windows.Forms.RibbonTab ribbonTab1;
        private System.Windows.Forms.RibbonPanel ribbonPanel1;
        private System.Windows.Forms.RibbonButton addVMButton;
        private System.Windows.Forms.RibbonButton deleteVMButton;
        private System.Windows.Forms.RibbonButton startVMButton;
        private System.Windows.Forms.RibbonButton stopVMButton;
        private System.Windows.Forms.RibbonButton stopAllButton;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.RibbonButton ribbonButton1;
        private System.Windows.Forms.RibbonPanel ribbonPanel4;
        private System.Windows.Forms.RibbonButton ribbonButton6;
        private System.Windows.Forms.RibbonButton ribbonButton5;
        private System.Windows.Forms.RibbonButton ribbonButton4;
        private System.Windows.Forms.RibbonTab ribbonTab2;
    }
}

