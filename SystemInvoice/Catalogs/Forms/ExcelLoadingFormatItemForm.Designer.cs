namespace SystemInvoice.Catalogs.Forms
    {
    partial class ExcelLoadingFormatItemForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExcelLoadingFormatItemForm));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btnOk = new DevExpress.XtraBars.BarButtonItem();
            this.btnWrite = new DevExpress.XtraBars.BarButtonItem();
            this.btnCancel = new DevExpress.XtraBars.BarButtonItem();
            this.ColumnsMappingsButtonsBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.SubGroupOfGoods = new Aramis.AramisSearchLookUpEdit();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.Contractor = new Aramis.AramisSearchLookUpEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.TradeMark = new Aramis.AramisSearchLookUpEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.Description = new DevExpress.XtraEditors.TextEdit();
            this.desc = new DevExpress.XtraEditors.LabelControl();
            this.FirstRowNumber = new DevExpress.XtraEditors.CalcEdit();
            this.labelControl13 = new DevExpress.XtraEditors.LabelControl();
            this.ColumnsMappingsControl = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridView3 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.okBtn = new DevExpress.XtraBars.BarButtonItem();
            this.WriteBtn = new DevExpress.XtraBars.BarButtonItem();
            this.CancelBtn = new DevExpress.XtraBars.BarButtonItem();
            this.btnLoad = new DevExpress.XtraEditors.SimpleButton();
            this.btnUnload = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.GrafSwitchValue = new DevExpress.XtraEditors.TextEdit();
            this.ColumnIndexForGrafShoes = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.UseMaxBordersWithNomenclatureCreating = new DevExpress.XtraEditors.CheckEdit();
            this.OrderInvoiceColumns = new DevExpress.XtraEditors.CheckEdit();
            this.SaveOriginalRowsSet = new DevExpress.XtraEditors.CheckEdit();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.Graph31Prefix = new DevExpress.XtraEditors.TextEdit();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            this.DateFormatStr = new DevExpress.XtraEditors.TextEdit();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.EliminateRowsFooter = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SubGroupOfGoods.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Contractor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TradeMark.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Description.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FirstRowNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColumnsMappingsControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GrafSwitchValue.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColumnIndexForGrafShoes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UseMaxBordersWithNomenclatureCreating.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OrderInvoiceColumns.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SaveOriginalRowsSet.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Graph31Prefix.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateFormatStr.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EliminateRowsFooter.Properties)).BeginInit();
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
            this.ribbon.Size = new System.Drawing.Size(786, 27);
            this.ribbon.StatusBar = this.ribbonStatusBar;
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
            // ColumnsMappingsButtonsBar
            // 
            this.ColumnsMappingsButtonsBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.ColumnsMappingsButtonsBar.Location = new System.Drawing.Point(2, 2);
            this.ColumnsMappingsButtonsBar.Name = "ColumnsMappingsButtonsBar";
            this.ColumnsMappingsButtonsBar.Ribbon = this.ribbon;
            this.ColumnsMappingsButtonsBar.Size = new System.Drawing.Size(782, 27);
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.ItemLinks.Add(this.btnOk);
            this.ribbonStatusBar.ItemLinks.Add(this.btnWrite);
            this.ribbonStatusBar.ItemLinks.Add(this.btnCancel);
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 418);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(786, 31);
            // 
            // SubGroupOfGoods
            // 
            this.SubGroupOfGoods.BaseFilter = null;
            this.SubGroupOfGoods.Location = new System.Drawing.Point(156, 67);
            this.SubGroupOfGoods.MenuManager = this.ribbon;
            this.SubGroupOfGoods.Name = "SubGroupOfGoods";
            this.SubGroupOfGoods.Properties.BaseFilter = null;
            this.SubGroupOfGoods.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.SubGroupOfGoods.Properties.CloseUpKey = new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None);
            this.SubGroupOfGoods.Properties.DisplayFormat.FormatString = "d";
            this.SubGroupOfGoods.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.SubGroupOfGoods.Properties.EditFormat.FormatString = "d";
            this.SubGroupOfGoods.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.SubGroupOfGoods.Properties.FirstPopUp = null;
            this.SubGroupOfGoods.Properties.NullText = "";
            this.SubGroupOfGoods.Size = new System.Drawing.Size(391, 20);
            this.SubGroupOfGoods.TabIndex = 179;
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(8, 70);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(75, 13);
            this.labelControl6.TabIndex = 178;
            this.labelControl6.Text = "Группа товара";
            // 
            // Contractor
            // 
            this.Contractor.BaseFilter = null;
            this.Contractor.Location = new System.Drawing.Point(156, 15);
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
            this.Contractor.Size = new System.Drawing.Size(391, 20);
            this.Contractor.TabIndex = 177;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(8, 18);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(60, 13);
            this.labelControl5.TabIndex = 176;
            this.labelControl5.Text = "Контрагент";
            // 
            // TradeMark
            // 
            this.TradeMark.BaseFilter = null;
            this.TradeMark.Location = new System.Drawing.Point(156, 41);
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
            this.TradeMark.Size = new System.Drawing.Size(391, 20);
            this.TradeMark.TabIndex = 175;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(8, 44);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(80, 13);
            this.labelControl4.TabIndex = 174;
            this.labelControl4.Text = "Торговая марка";
            // 
            // Description
            // 
            this.Description.Location = new System.Drawing.Point(156, 93);
            this.Description.Name = "Description";
            this.Description.Size = new System.Drawing.Size(391, 20);
            this.Description.TabIndex = 181;
            // 
            // desc
            // 
            this.desc.Location = new System.Drawing.Point(9, 96);
            this.desc.Name = "desc";
            this.desc.Size = new System.Drawing.Size(73, 13);
            this.desc.TabIndex = 180;
            this.desc.Text = "Наименование";
            // 
            // FirstRowNumber
            // 
            this.FirstRowNumber.Location = new System.Drawing.Point(684, 68);
            this.FirstRowNumber.MenuManager = this.ribbon;
            this.FirstRowNumber.Name = "FirstRowNumber";
            this.FirstRowNumber.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.FirstRowNumber.Size = new System.Drawing.Size(92, 20);
            this.FirstRowNumber.TabIndex = 189;
            // 
            // labelControl13
            // 
            this.labelControl13.Location = new System.Drawing.Point(553, 71);
            this.labelControl13.Name = "labelControl13";
            this.labelControl13.Size = new System.Drawing.Size(108, 13);
            this.labelControl13.TabIndex = 188;
            this.labelControl13.Text = "Номер первой строки";
            // 
            // ColumnsMappingsControl
            // 
            this.ColumnsMappingsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ColumnsMappingsControl.Location = new System.Drawing.Point(2, 29);
            this.ColumnsMappingsControl.MainView = this.gridView1;
            this.ColumnsMappingsControl.Name = "ColumnsMappingsControl";
            this.ColumnsMappingsControl.Size = new System.Drawing.Size(782, 133);
            this.ColumnsMappingsControl.TabIndex = 191;
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
            // btnLoad
            // 
            this.btnLoad.Image = ((System.Drawing.Image)(resources.GetObject("btnLoad.Image")));
            this.btnLoad.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnLoad.Location = new System.Drawing.Point(749, 13);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(27, 25);
            this.btnLoad.TabIndex = 206;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnUnload
            // 
            this.btnUnload.Image = ((System.Drawing.Image)(resources.GetObject("btnUnload.Image")));
            this.btnUnload.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUnload.Location = new System.Drawing.Point(749, 39);
            this.btnUnload.Name = "btnUnload";
            this.btnUnload.Size = new System.Drawing.Size(27, 25);
            this.btnUnload.TabIndex = 211;
            this.btnUnload.Click += new System.EventHandler(this.btnUnload_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(553, 18);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(94, 13);
            this.labelControl1.TabIndex = 215;
            this.labelControl1.Text = "Загрузить из Excel";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(553, 44);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(91, 13);
            this.labelControl2.TabIndex = 216;
            this.labelControl2.Text = "Выгрузить в Excel";
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(292, 174);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(255, 13);
            this.labelControl7.TabIndex = 222;
            this.labelControl7.Text = "Значение в колонке фильтра графы 31 для обуви";
            // 
            // GrafSwitchValue
            // 
            this.GrafSwitchValue.Location = new System.Drawing.Point(553, 171);
            this.GrafSwitchValue.Name = "GrafSwitchValue";
            this.GrafSwitchValue.Size = new System.Drawing.Size(223, 20);
            this.GrafSwitchValue.TabIndex = 223;
            // 
            // ColumnIndexForGrafShoes
            // 
            this.ColumnIndexForGrafShoes.Location = new System.Drawing.Point(156, 172);
            this.ColumnIndexForGrafShoes.Name = "ColumnIndexForGrafShoes";
            this.ColumnIndexForGrafShoes.Size = new System.Drawing.Size(119, 20);
            this.ColumnIndexForGrafShoes.TabIndex = 228;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(9, 175);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(106, 13);
            this.labelControl3.TabIndex = 227;
            this.labelControl3.Text = "№  для обуви (Гр31)";
            // 
            // UseMaxBordersWithNomenclatureCreating
            // 
            this.UseMaxBordersWithNomenclatureCreating.Location = new System.Drawing.Point(8, 120);
            this.UseMaxBordersWithNomenclatureCreating.MenuManager = this.ribbon;
            this.UseMaxBordersWithNomenclatureCreating.Name = "UseMaxBordersWithNomenclatureCreating";
            this.UseMaxBordersWithNomenclatureCreating.Properties.AutoWidth = true;
            this.UseMaxBordersWithNomenclatureCreating.Properties.Caption = "Использовать максимальные рамки нетто при создании номенклатуры";
            this.UseMaxBordersWithNomenclatureCreating.Size = new System.Drawing.Size(384, 19);
            this.UseMaxBordersWithNomenclatureCreating.TabIndex = 232;
            // 
            // OrderInvoiceColumns
            // 
            this.OrderInvoiceColumns.Location = new System.Drawing.Point(553, 120);
            this.OrderInvoiceColumns.MenuManager = this.ribbon;
            this.OrderInvoiceColumns.Name = "OrderInvoiceColumns";
            this.OrderInvoiceColumns.Properties.AutoWidth = true;
            this.OrderInvoiceColumns.Properties.Caption = "Упорядочивать столбцы в документе";
            this.OrderInvoiceColumns.Size = new System.Drawing.Size(216, 19);
            this.OrderInvoiceColumns.TabIndex = 236;
            // 
            // SaveOriginalRowsSet
            // 
            this.SaveOriginalRowsSet.Location = new System.Drawing.Point(553, 95);
            this.SaveOriginalRowsSet.MenuManager = this.ribbon;
            this.SaveOriginalRowsSet.Name = "SaveOriginalRowsSet";
            this.SaveOriginalRowsSet.Properties.AutoWidth = true;
            this.SaveOriginalRowsSet.Properties.Caption = "Сохранять начальный набор строк";
            this.SaveOriginalRowsSet.Size = new System.Drawing.Size(202, 19);
            toolTipTitleItem1.Text = "Не группировать и не изменять порядок исходных строк Excel";
            superToolTip1.Items.Add(toolTipTitleItem1);
            this.SaveOriginalRowsSet.SuperTip = superToolTip1;
            this.SaveOriginalRowsSet.TabIndex = 240;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.EliminateRowsFooter);
            this.panelControl1.Controls.Add(this.Graph31Prefix);
            this.panelControl1.Controls.Add(this.labelControl9);
            this.panelControl1.Controls.Add(this.DateFormatStr);
            this.panelControl1.Controls.Add(this.labelControl8);
            this.panelControl1.Controls.Add(this.labelControl5);
            this.panelControl1.Controls.Add(this.SaveOriginalRowsSet);
            this.panelControl1.Controls.Add(this.labelControl4);
            this.panelControl1.Controls.Add(this.OrderInvoiceColumns);
            this.panelControl1.Controls.Add(this.TradeMark);
            this.panelControl1.Controls.Add(this.UseMaxBordersWithNomenclatureCreating);
            this.panelControl1.Controls.Add(this.Contractor);
            this.panelControl1.Controls.Add(this.ColumnIndexForGrafShoes);
            this.panelControl1.Controls.Add(this.labelControl6);
            this.panelControl1.Controls.Add(this.labelControl3);
            this.panelControl1.Controls.Add(this.SubGroupOfGoods);
            this.panelControl1.Controls.Add(this.GrafSwitchValue);
            this.panelControl1.Controls.Add(this.desc);
            this.panelControl1.Controls.Add(this.labelControl7);
            this.panelControl1.Controls.Add(this.Description);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Controls.Add(this.labelControl13);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.FirstRowNumber);
            this.panelControl1.Controls.Add(this.btnUnload);
            this.panelControl1.Controls.Add(this.btnLoad);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 27);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(786, 227);
            this.panelControl1.TabIndex = 244;
            // 
            // Graph31Prefix
            // 
            this.Graph31Prefix.Location = new System.Drawing.Point(156, 196);
            this.Graph31Prefix.Name = "Graph31Prefix";
            this.Graph31Prefix.Size = new System.Drawing.Size(119, 20);
            this.Graph31Prefix.TabIndex = 244;
            // 
            // labelControl9
            // 
            this.labelControl9.Location = new System.Drawing.Point(9, 200);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(139, 13);
            this.labelControl9.TabIndex = 243;
            this.labelControl9.Text = "Префикс в шапке графы 31";
            // 
            // DateFormatStr
            // 
            this.DateFormatStr.Location = new System.Drawing.Point(553, 196);
            this.DateFormatStr.Name = "DateFormatStr";
            this.DateFormatStr.Size = new System.Drawing.Size(223, 20);
            this.DateFormatStr.TabIndex = 242;
            // 
            // labelControl8
            // 
            this.labelControl8.Location = new System.Drawing.Point(292, 200);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(255, 13);
            this.labelControl8.TabIndex = 241;
            this.labelControl8.Text = "Формат даты при выгрузке в Excel (разр. док-ты)";
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.ColumnsMappingsControl);
            this.panelControl2.Controls.Add(this.ColumnsMappingsButtonsBar);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(0, 254);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(786, 164);
            this.panelControl2.TabIndex = 245;
            // 
            // EliminateRowsFooter
            // 
            this.EliminateRowsFooter.Location = new System.Drawing.Point(8, 145);
            this.EliminateRowsFooter.MenuManager = this.ribbon;
            this.EliminateRowsFooter.Name = "EliminateRowsFooter";
            this.EliminateRowsFooter.Properties.AutoWidth = true;
            this.EliminateRowsFooter.Properties.Caption = "Не добавлять приставку \"Разом\" в конце группы графы 31";
            this.EliminateRowsFooter.Size = new System.Drawing.Size(323, 19);
            this.EliminateRowsFooter.TabIndex = 245;
            // 
            // ExcelLoadingFormatItemForm
            // 
            this.AllowDisplayRibbon = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 449);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.Name = "ExcelLoadingFormatItemForm";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "Формат загрузки файлов Excel";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SubGroupOfGoods.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Contractor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TradeMark.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Description.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FirstRowNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColumnsMappingsControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GrafSwitchValue.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColumnIndexForGrafShoes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UseMaxBordersWithNomenclatureCreating.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OrderInvoiceColumns.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SaveOriginalRowsSet.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Graph31Prefix.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateFormatStr.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.EliminateRowsFooter.Properties)).EndInit();
            this.ResumeLayout(false);

            }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.BarButtonItem btnOk;
        private DevExpress.XtraBars.BarButtonItem btnWrite;
        private DevExpress.XtraBars.BarButtonItem btnCancel;
        private Aramis.AramisSearchLookUpEdit SubGroupOfGoods;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private Aramis.AramisSearchLookUpEdit Contractor;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private Aramis.AramisSearchLookUpEdit TradeMark;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit Description;
        private DevExpress.XtraEditors.LabelControl desc;
        private DevExpress.XtraEditors.CalcEdit FirstRowNumber;
        private DevExpress.XtraEditors.LabelControl labelControl13;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ColumnsMappingsButtonsBar;
        private DevExpress.XtraGrid.GridControl ColumnsMappingsControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView3;
        private DevExpress.XtraBars.BarButtonItem okBtn;
        private DevExpress.XtraBars.BarButtonItem WriteBtn;
        private DevExpress.XtraBars.BarButtonItem CancelBtn;
        private DevExpress.XtraEditors.SimpleButton btnLoad;
        private DevExpress.XtraEditors.SimpleButton btnUnload;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.TextEdit GrafSwitchValue;
        private DevExpress.XtraEditors.TextEdit ColumnIndexForGrafShoes;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.CheckEdit UseMaxBordersWithNomenclatureCreating;
        private DevExpress.XtraEditors.CheckEdit OrderInvoiceColumns;
        private DevExpress.XtraEditors.CheckEdit SaveOriginalRowsSet;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.TextEdit DateFormatStr;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.TextEdit Graph31Prefix;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private DevExpress.XtraEditors.CheckEdit EliminateRowsFooter;
        }
    }