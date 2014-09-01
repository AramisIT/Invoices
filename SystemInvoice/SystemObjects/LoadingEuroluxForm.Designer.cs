namespace SystemInvoice.Catalogs.Forms
    {
    partial class LoadingEuroluxForm
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
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.okBtn = new DevExpress.XtraBars.BarButtonItem();
            this.CancelBtn = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.btnOk = new DevExpress.XtraBars.BarButtonItem();
            this.btnWrite = new DevExpress.XtraBars.BarButtonItem();
            this.btnCancel = new DevExpress.XtraBars.BarButtonItem();
            this.desc = new DevExpress.XtraEditors.LabelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.progressBar = new DevExpress.XtraEditors.ProgressBarControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.file = new DevExpress.XtraEditors.ButtonEdit();
            this.Contractor = new Aramis.AramisSearchLookUpEdit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressBar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.file.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Contractor.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.ExpandCollapseItem.Name = "";
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.okBtn,
            this.CancelBtn});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 4;
            this.ribbon.Name = "ribbon";
            this.ribbon.Size = new System.Drawing.Size(679, 30);
            this.ribbon.StatusBar = this.ribbonStatusBar;
            // 
            // okBtn
            // 
            this.okBtn.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.okBtn.Caption = "OK";
            this.okBtn.Id = 1;
            this.okBtn.ImageIndex = 0;
            this.okBtn.Name = "okBtn";
            this.okBtn.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.okBtn_ItemClick);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.CancelBtn.Caption = "Отмена";
            this.CancelBtn.Id = 3;
            this.CancelBtn.ImageIndex = 1;
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.CancelBtn_ItemClick);
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.ItemLinks.Add(this.okBtn);
            this.ribbonStatusBar.ItemLinks.Add(this.CancelBtn);
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 378);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(679, 23);
            // 
            // btnOk
            // 
            this.btnOk.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnOk.Caption = "Ок";
            this.btnOk.Id = 1;
            this.btnOk.ImageIndex = 0;
            this.btnOk.Name = "btnOk";
            // 
            // btnWrite
            // 
            this.btnWrite.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnWrite.Caption = "Записать";
            this.btnWrite.Id = 2;
            this.btnWrite.Name = "btnWrite";
            // 
            // btnCancel
            // 
            this.btnCancel.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnCancel.Caption = "Отмена";
            this.btnCancel.Id = 3;
            this.btnCancel.ImageIndex = 1;
            this.btnCancel.Name = "btnCancel";
            // 
            // desc
            // 
            this.desc.Location = new System.Drawing.Point(9, 21);
            this.desc.Name = "desc";
            this.desc.Size = new System.Drawing.Size(60, 13);
            this.desc.TabIndex = 199;
            this.desc.Text = "Контрагент";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.progressBar);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.file);
            this.panelControl1.Controls.Add(this.Contractor);
            this.panelControl1.Controls.Add(this.desc);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 30);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(679, 84);
            this.panelControl1.TabIndex = 206;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(9, 47);
            this.progressBar.MenuManager = this.ribbon;
            this.progressBar.Name = "progressBar";
            this.progressBar.Properties.Step = 1;
            this.progressBar.Size = new System.Drawing.Size(658, 21);
            this.progressBar.TabIndex = 215;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(267, 21);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(54, 13);
            this.labelControl1.TabIndex = 214;
            this.labelControl1.Text = "Excel файл";
            // 
            // file
            // 
            this.file.Location = new System.Drawing.Point(339, 17);
            this.file.MenuManager = this.ribbon;
            this.file.Name = "file";
            this.file.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.file.Properties.ReadOnly = true;
            this.file.Size = new System.Drawing.Size(328, 20);
            this.file.TabIndex = 213;
            this.file.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.file_ButtonClick);
            // 
            // Contractor
            // 
            this.Contractor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Contractor.BaseFilter = null;
            this.Contractor.Location = new System.Drawing.Point(80, 17);
            this.Contractor.MenuManager = this.ribbon;
            this.Contractor.Name = "Contractor";
            this.Contractor.Properties.BaseFilter = null;
            this.Contractor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.Contractor.Properties.CloseUpKey = new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None);
            this.Contractor.Properties.DisplayFormat.FormatString = "d";
            this.Contractor.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.Contractor.Properties.EditFormat.FormatString = "d";
            this.Contractor.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.Contractor.Properties.FirstPopUp = null;
            this.Contractor.Properties.NullText = "";
            this.Contractor.Size = new System.Drawing.Size(168, 20);
            this.Contractor.TabIndex = 212;
            // 
            // LoadingEuroluxForm
            // 
            this.AllowDisplayRibbon = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 401);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.Name = "LoadingEuroluxForm";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "Загрузка Excel";
            this.Load += new System.EventHandler(this.NewGoodsRowForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressBar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.file.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Contractor.Properties)).EndInit();
            this.ResumeLayout(false);

            }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.BarButtonItem btnOk;
        private DevExpress.XtraBars.BarButtonItem btnWrite;
        private DevExpress.XtraBars.BarButtonItem btnCancel;
        private DevExpress.XtraBars.BarButtonItem okBtn;
        private DevExpress.XtraBars.BarButtonItem CancelBtn;
        private DevExpress.XtraEditors.LabelControl desc;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private Aramis.AramisSearchLookUpEdit Contractor;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.ButtonEdit file;
        private DevExpress.XtraEditors.ProgressBarControl progressBar;
        }
    }