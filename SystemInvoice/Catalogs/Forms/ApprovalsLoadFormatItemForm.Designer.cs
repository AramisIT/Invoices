namespace SystemInvoice.Catalogs.Forms
    {
    partial class ApprovalsLoadFormatItemForm
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
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.okBtn = new DevExpress.XtraBars.BarButtonItem();
            this.WriteBtn = new DevExpress.XtraBars.BarButtonItem();
            this.CancelBtn = new DevExpress.XtraBars.BarButtonItem();
            this.ColumnsMappingsButtonsBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.ColumnsMappingsControl = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridView3 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnOk = new DevExpress.XtraBars.BarButtonItem();
            this.btnWrite = new DevExpress.XtraBars.BarButtonItem();
            this.btnCancel = new DevExpress.XtraBars.BarButtonItem();
            this.Contractor = new Aramis.AramisSearchLookUpEdit();
            this.contrDescr = new DevExpress.XtraEditors.LabelControl();
            this.TradeMark = new Aramis.AramisSearchLookUpEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.Description = new DevExpress.XtraEditors.TextEdit();
            this.desc = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColumnsMappingsControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Contractor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TradeMark.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Description.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.ExpandCollapseItem.Name = "";
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.okBtn,
            this.WriteBtn,
            this.CancelBtn});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 4;
            this.ribbon.Name = "ribbon";
            this.ribbon.Size = new System.Drawing.Size(679, 49);
            this.ribbon.StatusBar = this.ribbonStatusBar;
            // 
            // okBtn
            // 
            this.okBtn.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.okBtn.Caption = "Ok";
            this.okBtn.Id = 1;
            this.okBtn.Name = "okBtn";
            this.okBtn.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.okBtn_ItemClick);
            // 
            // WriteBtn
            // 
            this.WriteBtn.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.WriteBtn.Caption = "Записать";
            this.WriteBtn.Id = 2;
            this.WriteBtn.Name = "WriteBtn";
            this.WriteBtn.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.WriteBtn_ItemClick);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.CancelBtn.Caption = "Отмена";
            this.CancelBtn.Id = 3;
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.CancelBtn_ItemClick);
            // 
            // ColumnsMappingsButtonsBar
            // 
            this.ColumnsMappingsButtonsBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ColumnsMappingsButtonsBar.Dock = System.Windows.Forms.DockStyle.None;
            this.ColumnsMappingsButtonsBar.Location = new System.Drawing.Point(0, 152);
            this.ColumnsMappingsButtonsBar.Name = "ColumnsMappingsButtonsBar";
            this.ColumnsMappingsButtonsBar.Ribbon = this.ribbon;
            this.ColumnsMappingsButtonsBar.Size = new System.Drawing.Size(679, 27);
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.ItemLinks.Add(this.okBtn);
            this.ribbonStatusBar.ItemLinks.Add(this.WriteBtn);
            this.ribbonStatusBar.ItemLinks.Add(this.CancelBtn);
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 439);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(679, 31);
            // 
            // ColumnsMappingsControl
            // 
            this.ColumnsMappingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ColumnsMappingsControl.Location = new System.Drawing.Point(0, 179);
            this.ColumnsMappingsControl.MainView = this.gridView1;
            this.ColumnsMappingsControl.Name = "ColumnsMappingsControl";
            this.ColumnsMappingsControl.Size = new System.Drawing.Size(679, 265);
            this.ColumnsMappingsControl.TabIndex = 193;
            this.ColumnsMappingsControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1,
            this.gridView3});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.ColumnsMappingsControl;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsCustomization.AllowGroup = false;
            this.gridView1.OptionsCustomization.AllowSort = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            // 
            // gridView3
            // 
            this.gridView3.GridControl = this.ColumnsMappingsControl;
            this.gridView3.Name = "gridView3";
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
            // Contractor
            // 
            this.Contractor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Contractor.BaseFilter = null;
            this.Contractor.Location = new System.Drawing.Point(91, 98);
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
            this.Contractor.Size = new System.Drawing.Size(253, 20);
            this.Contractor.TabIndex = 198;
            // 
            // contrDescr
            // 
            this.contrDescr.Location = new System.Drawing.Point(5, 101);
            this.contrDescr.Name = "contrDescr";
            this.contrDescr.Size = new System.Drawing.Size(60, 13);
            this.contrDescr.TabIndex = 197;
            this.contrDescr.Text = "Контрагент";
            // 
            // TradeMark
            // 
            this.TradeMark.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TradeMark.BaseFilter = null;
            this.TradeMark.Location = new System.Drawing.Point(91, 124);
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
            this.TradeMark.Properties.FirstPopUp = null;
            this.TradeMark.Properties.NullText = "";
            this.TradeMark.Size = new System.Drawing.Size(253, 20);
            this.TradeMark.TabIndex = 196;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(5, 127);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(80, 13);
            this.labelControl4.TabIndex = 195;
            this.labelControl4.Text = "Торговая марка";
            // 
            // Description
            // 
            this.Description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Description.Location = new System.Drawing.Point(91, 72);
            this.Description.Name = "Description";
            this.Description.Size = new System.Drawing.Size(253, 20);
            this.Description.TabIndex = 200;
            // 
            // desc
            // 
            this.desc.Location = new System.Drawing.Point(5, 75);
            this.desc.Name = "desc";
            this.desc.Size = new System.Drawing.Size(73, 13);
            this.desc.TabIndex = 199;
            this.desc.Text = "Наименование";
            // 
            // ApprovalsLoadFormatItemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 470);
            this.Controls.Add(this.Description);
            this.Controls.Add(this.desc);
            this.Controls.Add(this.Contractor);
            this.Controls.Add(this.contrDescr);
            this.Controls.Add(this.TradeMark);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.ColumnsMappingsControl);
            this.Controls.Add(this.ColumnsMappingsButtonsBar);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.Name = "ApprovalsLoadFormatItemForm";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "Формат загрузки разрешительного документа";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColumnsMappingsControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Contractor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TradeMark.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Description.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ColumnsMappingsButtonsBar;
        private DevExpress.XtraGrid.GridControl ColumnsMappingsControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView3;
        private DevExpress.XtraBars.BarButtonItem btnOk;
        private DevExpress.XtraBars.BarButtonItem btnWrite;
        private DevExpress.XtraBars.BarButtonItem btnCancel;
        private Aramis.AramisSearchLookUpEdit Contractor;
        private DevExpress.XtraEditors.LabelControl contrDescr;
        private Aramis.AramisSearchLookUpEdit TradeMark;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraBars.BarButtonItem okBtn;
        private DevExpress.XtraBars.BarButtonItem WriteBtn;
        private DevExpress.XtraBars.BarButtonItem CancelBtn;
        private DevExpress.XtraEditors.TextEdit Description;
        private DevExpress.XtraEditors.LabelControl desc;
        }
    }