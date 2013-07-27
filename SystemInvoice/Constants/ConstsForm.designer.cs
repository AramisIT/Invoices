namespace SystemInvoice.Constants
    {
    partial class ConstsForm
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
            if ( disposing && ( components != null ) )
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
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.OKButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.barButtonGroup1 = new DevExpress.XtraBars.BarButtonGroup();
            this.DefaultPage = new DevExpress.XtraTab.XtraTabPage();
            this.AlarmForApprovalBecomeFailDays = new DevExpress.XtraEditors.TextEdit();
            this.labelControl14 = new DevExpress.XtraEditors.LabelControl();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            this.DefaultPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AlarmForApprovalBecomeFailDays.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.ItemLinks.Add(this.OKButton);
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 436);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(486, 23);
            // 
            // OKButton
            // 
            this.OKButton.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.OKButton.Caption = "OK";
            this.OKButton.Id = 6;
            this.OKButton.ImageIndex = 0;
            this.OKButton.Name = "OKButton";
            this.OKButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OKButton_ItemClick);
            // 
            // ribbon
            // 
            this.ribbon.ApplicationButtonText = null;
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.ExpandCollapseItem.Name = "";
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.barButtonGroup1,
            this.OKButton});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 9;
            this.ribbon.Name = "ribbon";
            this.ribbon.Size = new System.Drawing.Size(486, 54);
            this.ribbon.StatusBar = this.ribbonStatusBar;
            this.ribbon.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Above;
            // 
            // barButtonGroup1
            // 
            this.barButtonGroup1.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.barButtonGroup1.Caption = "barButtonGroup1";
            this.barButtonGroup1.Id = 4;
            this.barButtonGroup1.ItemLinks.Add(this.OKButton);
            this.barButtonGroup1.MenuAppearance.AppearanceMenu.Normal.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.barButtonGroup1.MenuAppearance.AppearanceMenu.Normal.Options.UseFont = true;
            this.barButtonGroup1.MenuAppearance.MenuBar.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.barButtonGroup1.MenuAppearance.MenuBar.Options.UseFont = true;
            this.barButtonGroup1.MenuAppearance.MenuCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.barButtonGroup1.MenuAppearance.MenuCaption.Options.UseFont = true;
            this.barButtonGroup1.MenuAppearance.SideStrip.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.barButtonGroup1.MenuAppearance.SideStrip.Options.UseFont = true;
            this.barButtonGroup1.Name = "barButtonGroup1";
            // 
            // DefaultPage
            // 
            this.DefaultPage.Controls.Add(this.AlarmForApprovalBecomeFailDays);
            this.DefaultPage.Controls.Add(this.labelControl14);
            this.DefaultPage.Name = "DefaultPage";
            this.DefaultPage.PageVisible = false;
            this.DefaultPage.Size = new System.Drawing.Size(460, 342);
            this.DefaultPage.Text = "Настройки";
            // 
            // AlarmForApprovalBecomeFailDays
            // 
            this.AlarmForApprovalBecomeFailDays.Location = new System.Drawing.Point(356, 30);
            this.AlarmForApprovalBecomeFailDays.MenuManager = this.ribbon;
            this.AlarmForApprovalBecomeFailDays.Name = "AlarmForApprovalBecomeFailDays";
            this.AlarmForApprovalBecomeFailDays.Properties.Mask.EditMask = "\\d{1,}";
            this.AlarmForApprovalBecomeFailDays.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.AlarmForApprovalBecomeFailDays.Size = new System.Drawing.Size(100, 20);
            this.AlarmForApprovalBecomeFailDays.TabIndex = 231;
            // 
            // labelControl14
            // 
            this.labelControl14.Location = new System.Drawing.Point(12, 33);
            this.labelControl14.Name = "labelControl14";
            this.labelControl14.Size = new System.Drawing.Size(338, 13);
            this.labelControl14.TabIndex = 228;
            this.labelControl14.Text = "Уведомлять за количество дней до окончания срока годности РД";
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.xtraTabControl1.Location = new System.Drawing.Point(12, 62);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.DefaultPage;
            this.xtraTabControl1.Size = new System.Drawing.Size(465, 368);
            this.xtraTabControl1.TabIndex = 2;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.DefaultPage});
            // 
            // ConstsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 459);
            this.Controls.Add(this.xtraTabControl1);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(299, 434);
            this.Name = "ConstsForm";
            this.Ribbon = this.ribbon;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "Константы системы";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConstsForm_FormClosed);
            this.Load += new System.EventHandler(this.Itemform_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Itemform_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            this.DefaultPage.ResumeLayout(false);
            this.DefaultPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AlarmForApprovalBecomeFailDays.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

            }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.BarButtonGroup barButtonGroup1;
        private DevExpress.XtraBars.BarButtonItem OKButton;
        private DevExpress.XtraTab.XtraTabPage DefaultPage;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraEditors.LabelControl labelControl14;
        private DevExpress.XtraEditors.TextEdit AlarmForApprovalBecomeFailDays;
        }
    }