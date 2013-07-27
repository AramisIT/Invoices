namespace SystemInvoice.Documents.Forms
    {
    partial class ApprovalsItemForm
        {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
            {
            if (disposing && (components != null))
                {
                components.Dispose();
                }
            base.Dispose( disposing );
            }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
            {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApprovalsItemForm));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btnOk = new DevExpress.XtraBars.BarButtonItem();
            this.btnWrite = new DevExpress.XtraBars.BarButtonItem();
            this.btnCancel = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.NomenclaturesButtonsBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.DocumentType = new Aramis.AramisSearchLookUpEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.TradeMark = new Aramis.AramisSearchLookUpEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.DocumentCode = new DevExpress.XtraEditors.TextEdit();
            this.desc = new DevExpress.XtraEditors.LabelControl();
            this.labelControl13 = new DevExpress.XtraEditors.LabelControl();
            this.NomenclaturesControl = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridView3 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.okBtn = new DevExpress.XtraBars.BarButtonItem();
            this.WriteBtn = new DevExpress.XtraBars.BarButtonItem();
            this.CancelBtn = new DevExpress.XtraBars.BarButtonItem();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.Contractor = new Aramis.AramisSearchLookUpEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.DateFrom = new DevExpress.XtraEditors.DateEdit();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.DateTo = new DevExpress.XtraEditors.DateEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.excelOpenPath = new DevExpress.XtraEditors.TextEdit();
            this.btnSelect = new DevExpress.XtraEditors.SimpleButton();
            this.btnOpen = new DevExpress.XtraEditors.SimpleButton();
            this.btnFill = new DevExpress.XtraEditors.SimpleButton();
            this.DocumentNumber = new DevExpress.XtraEditors.TextEdit();
            this.ApprovalsLoadFormat = new Aramis.AramisSearchLookUpEdit();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DocumentType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TradeMark.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DocumentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NomenclaturesControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Contractor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateFrom.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateTo.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.excelOpenPath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DocumentNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ApprovalsLoadFormat.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.ExpandCollapseItem.Name = "";
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.btnOk,
            this.btnWrite,
            this.btnCancel});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 4;
            this.ribbon.Name = "ribbon";
            this.ribbon.Size = new System.Drawing.Size(721, 54);
            this.ribbon.StatusBar = this.NomenclaturesButtonsBar;
            // 
            // btnOk
            // 
            this.btnOk.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnOk.Caption = "Ок";
            this.btnOk.Id = 1;
            this.btnOk.ImageIndex = 0;
            this.btnOk.Name = "btnOk";
            this.btnOk.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnOk_ItemClick);
            // 
            // btnWrite
            // 
            this.btnWrite.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnWrite.Caption = "Записать";
            this.btnWrite.Id = 2;
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnWrite_ItemClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnCancel.Caption = "Отмена";
            this.btnCancel.Id = 3;
            this.btnCancel.ImageIndex = 1;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCancel_ItemClick);
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.ItemLinks.Add(this.btnOk);
            this.ribbonStatusBar.ItemLinks.Add(this.btnWrite);
            this.ribbonStatusBar.ItemLinks.Add(this.btnCancel);
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 568);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(721, 23);
            // 
            // NomenclaturesButtonsBar
            // 
            this.NomenclaturesButtonsBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NomenclaturesButtonsBar.Dock = System.Windows.Forms.DockStyle.None;
            this.NomenclaturesButtonsBar.Location = new System.Drawing.Point(5, 301);
            this.NomenclaturesButtonsBar.Name = "NomenclaturesButtonsBar";
            this.NomenclaturesButtonsBar.Ribbon = this.ribbon;
            this.NomenclaturesButtonsBar.Size = new System.Drawing.Size(710, 23);
            // 
            // DocumentType
            // 
            this.DocumentType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DocumentType.BaseFilter = null;
            this.DocumentType.Location = new System.Drawing.Point(112, 96);
            this.DocumentType.MenuManager = this.ribbon;
            this.DocumentType.Name = "DocumentType";
            this.DocumentType.Properties.BaseFilter = null;
            this.DocumentType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.DocumentType.Properties.CloseUpKey = new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None);
            this.DocumentType.Properties.DisplayFormat.FormatString = "d";
            this.DocumentType.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.DocumentType.Properties.EditFormat.FormatString = "d";
            this.DocumentType.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.DocumentType.Properties.NullText = "";
            this.DocumentType.Size = new System.Drawing.Size(329, 20);
            this.DocumentType.TabIndex = 177;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(6, 99);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(76, 13);
            this.labelControl5.TabIndex = 176;
            this.labelControl5.Text = "Тип документа";
            // 
            // TradeMark
            // 
            this.TradeMark.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TradeMark.BaseFilter = null;
            this.TradeMark.Location = new System.Drawing.Point(112, 226);
            this.TradeMark.MenuManager = this.ribbon;
            this.TradeMark.Name = "TradeMark";
            this.TradeMark.Properties.BaseFilter = null;
            this.TradeMark.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.TradeMark.Properties.CloseUpKey = new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None);
            this.TradeMark.Properties.DisplayFormat.FormatString = "d";
            this.TradeMark.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.TradeMark.Properties.EditFormat.FormatString = "d";
            this.TradeMark.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.TradeMark.Properties.NullText = "";
            this.TradeMark.Size = new System.Drawing.Size(329, 20);
            this.TradeMark.TabIndex = 175;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(6, 229);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(80, 13);
            this.labelControl4.TabIndex = 174;
            this.labelControl4.Text = "Торговая марка";
            // 
            // DocumentCode
            // 
            this.DocumentCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DocumentCode.Location = new System.Drawing.Point(112, 122);
            this.DocumentCode.Name = "DocumentCode";
            this.DocumentCode.Size = new System.Drawing.Size(127, 20);
            this.DocumentCode.TabIndex = 181;
            // 
            // desc
            // 
            this.desc.Location = new System.Drawing.Point(7, 125);
            this.desc.Name = "desc";
            this.desc.Size = new System.Drawing.Size(20, 13);
            this.desc.TabIndex = 180;
            this.desc.Text = "Код";
            // 
            // labelControl13
            // 
            this.labelControl13.Location = new System.Drawing.Point(7, 151);
            this.labelControl13.Name = "labelControl13";
            this.labelControl13.Size = new System.Drawing.Size(89, 13);
            this.labelControl13.TabIndex = 188;
            this.labelControl13.Text = "Номер документа";
            // 
            // NomenclaturesControl
            // 
            this.NomenclaturesControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NomenclaturesControl.Location = new System.Drawing.Point(5, 324);
            this.NomenclaturesControl.MainView = this.gridView1;
            this.NomenclaturesControl.Name = "NomenclaturesControl";
            this.NomenclaturesControl.Size = new System.Drawing.Size(710, 238);
            this.NomenclaturesControl.TabIndex = 191;
            this.NomenclaturesControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1,
            this.gridView3});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.NomenclaturesControl;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsCustomization.AllowGroup = false;
            this.gridView1.OptionsCustomization.AllowSort = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            // 
            // gridView3
            // 
            this.gridView3.GridControl = this.NomenclaturesControl;
            this.gridView3.Name = "gridView3";
            // 
            // okBtn
            // 
            this.okBtn.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.okBtn.Caption = "Ok";
            this.okBtn.Id = 1;
            this.okBtn.ImageIndex = 0;
            this.okBtn.Name = "okBtn";
            // 
            // WriteBtn
            // 
            this.WriteBtn.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.WriteBtn.Caption = "Записать";
            this.WriteBtn.Id = 2;
            this.WriteBtn.Name = "WriteBtn";
            // 
            // CancelBtn
            // 
            this.CancelBtn.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.CancelBtn.Caption = "Отмена";
            this.CancelBtn.Id = 3;
            this.CancelBtn.ImageIndex = 1;
            this.CancelBtn.Name = "CancelBtn";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(6, 280);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(231, 13);
            this.labelControl1.TabIndex = 195;
            this.labelControl1.Text = "Партия товара на которую выдан документ:";
            // 
            // Contractor
            // 
            this.Contractor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Contractor.BaseFilter = null;
            this.Contractor.Location = new System.Drawing.Point(112, 200);
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
            this.Contractor.Properties.NullText = "";
            this.Contractor.Size = new System.Drawing.Size(329, 20);
            this.Contractor.TabIndex = 197;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(6, 203);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(60, 13);
            this.labelControl2.TabIndex = 196;
            this.labelControl2.Text = "Контрагент";
            // 
            // DateFrom
            // 
            this.DateFrom.EditValue = null;
            this.DateFrom.Location = new System.Drawing.Point(112, 174);
            this.DateFrom.Name = "DateFrom";
            this.DateFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DateFrom.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.DateFrom.Size = new System.Drawing.Size(107, 20);
            this.DateFrom.TabIndex = 199;
            // 
            // labelControl6
            // 
            this.labelControl6.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelControl6.Location = new System.Drawing.Point(6, 177);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(94, 14);
            this.labelControl6.TabIndex = 198;
            this.labelControl6.Text = "Срок действия с";
            // 
            // DateTo
            // 
            this.DateTo.EditValue = null;
            this.DateTo.Location = new System.Drawing.Point(245, 175);
            this.DateTo.Name = "DateTo";
            this.DateTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DateTo.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.DateTo.Size = new System.Drawing.Size(108, 20);
            this.DateTo.TabIndex = 201;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelControl3.Location = new System.Drawing.Point(225, 177);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(14, 14);
            this.labelControl3.TabIndex = 200;
            this.labelControl3.Text = "по";
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(7, 257);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(99, 13);
            this.labelControl7.TabIndex = 202;
            this.labelControl7.Text = "Загрузить из EXCEL";
            // 
            // excelOpenPath
            // 
            this.excelOpenPath.Location = new System.Drawing.Point(112, 254);
            this.excelOpenPath.Name = "excelOpenPath";
            this.excelOpenPath.Size = new System.Drawing.Size(227, 20);
            this.excelOpenPath.TabIndex = 203;
            // 
            // btnSelect
            // 
            this.btnSelect.Image = ((System.Drawing.Image)(resources.GetObject("btnSelect.Image")));
            this.btnSelect.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSelect.Location = new System.Drawing.Point(345, 252);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(25, 25);
            this.btnSelect.TabIndex = 204;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
            this.btnOpen.Location = new System.Drawing.Point(376, 252);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(30, 25);
            this.btnOpen.TabIndex = 205;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnFill
            // 
            this.btnFill.Image = ((System.Drawing.Image)(resources.GetObject("btnFill.Image")));
            this.btnFill.Location = new System.Drawing.Point(412, 252);
            this.btnFill.Name = "btnFill";
            this.btnFill.Size = new System.Drawing.Size(30, 25);
            this.btnFill.TabIndex = 206;
            this.btnFill.Click += new System.EventHandler(this.btnFill_Click);
            // 
            // DocumentNumber
            // 
            this.DocumentNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DocumentNumber.Location = new System.Drawing.Point(112, 148);
            this.DocumentNumber.Name = "DocumentNumber";
            this.DocumentNumber.Size = new System.Drawing.Size(329, 20);
            this.DocumentNumber.TabIndex = 207;
            // 
            // ApprovalsLoadFormat
            // 
            this.ApprovalsLoadFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ApprovalsLoadFormat.BaseFilter = null;
            this.ApprovalsLoadFormat.Location = new System.Drawing.Point(112, 70);
            this.ApprovalsLoadFormat.MenuManager = this.ribbon;
            this.ApprovalsLoadFormat.Name = "ApprovalsLoadFormat";
            this.ApprovalsLoadFormat.Properties.BaseFilter = null;
            this.ApprovalsLoadFormat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.ApprovalsLoadFormat.Properties.CloseUpKey = new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None);
            this.ApprovalsLoadFormat.Properties.DisplayFormat.FormatString = "d";
            this.ApprovalsLoadFormat.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.ApprovalsLoadFormat.Properties.EditFormat.FormatString = "d";
            this.ApprovalsLoadFormat.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.ApprovalsLoadFormat.Properties.NullText = "";
            this.ApprovalsLoadFormat.Size = new System.Drawing.Size(329, 20);
            this.ApprovalsLoadFormat.TabIndex = 212;
            // 
            // labelControl8
            // 
            this.labelControl8.Location = new System.Drawing.Point(6, 73);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(86, 13);
            this.labelControl8.TabIndex = 211;
            this.labelControl8.Text = "Формат загрузки";
            // 
            // ApprovalsItemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 591);
            this.Controls.Add(this.ApprovalsLoadFormat);
            this.Controls.Add(this.labelControl8);
            this.Controls.Add(this.DocumentNumber);
            this.Controls.Add(this.btnFill);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.excelOpenPath);
            this.Controls.Add(this.labelControl7);
            this.Controls.Add(this.DateTo);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.DateFrom);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.Contractor);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.NomenclaturesControl);
            this.Controls.Add(this.NomenclaturesButtonsBar);
            this.Controls.Add(this.labelControl13);
            this.Controls.Add(this.DocumentCode);
            this.Controls.Add(this.desc);
            this.Controls.Add(this.DocumentType);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.TradeMark);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.Name = "ApprovalsItemForm";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "Разрешительный документ";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DocumentType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TradeMark.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DocumentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NomenclaturesControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Contractor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateFrom.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateTo.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.excelOpenPath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DocumentNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ApprovalsLoadFormat.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.BarButtonItem btnOk;
        private DevExpress.XtraBars.BarButtonItem btnWrite;
        private DevExpress.XtraBars.BarButtonItem btnCancel;
        private Aramis.AramisSearchLookUpEdit DocumentType;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private Aramis.AramisSearchLookUpEdit TradeMark;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit DocumentCode;
        private DevExpress.XtraEditors.LabelControl desc;
        private DevExpress.XtraEditors.LabelControl labelControl13;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar NomenclaturesButtonsBar;
        private DevExpress.XtraGrid.GridControl NomenclaturesControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView3;
        private DevExpress.XtraBars.BarButtonItem okBtn;
        private DevExpress.XtraBars.BarButtonItem WriteBtn;
        private DevExpress.XtraBars.BarButtonItem CancelBtn;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private Aramis.AramisSearchLookUpEdit Contractor;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.DateEdit DateFrom;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.DateEdit DateTo;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.TextEdit excelOpenPath;
        private DevExpress.XtraEditors.SimpleButton btnSelect;
        private DevExpress.XtraEditors.SimpleButton btnOpen;
        private DevExpress.XtraEditors.SimpleButton btnFill;
        private DevExpress.XtraEditors.TextEdit DocumentNumber;
        private Aramis.AramisSearchLookUpEdit ApprovalsLoadFormat;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        }
    }