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
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.FilesBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.btnOk = new DevExpress.XtraBars.BarButtonItem();
            this.btnWrite = new DevExpress.XtraBars.BarButtonItem();
            this.btnCancel = new DevExpress.XtraBars.BarButtonItem();
            this.desc = new DevExpress.XtraEditors.LabelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.progressBar = new DevExpress.XtraEditors.ProgressBarControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.file = new DevExpress.XtraEditors.ButtonEdit();
            this.Contractor = new Aramis.AramisSearchLookUpEdit();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.Files = new DevExpress.XtraGrid.GridControl();
            this.filesGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.FindArticleAndModelRegEx = new DevExpress.XtraEditors.TextEdit();
            this.okButtonItem = new DevExpress.XtraBars.BarButtonItem();
            this.ApprovalDurationYears = new DevExpress.XtraEditors.CalcEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressBar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.file.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Contractor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtraTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Files)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filesGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FindArticleAndModelRegEx.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ApprovalDurationYears.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.ExpandCollapseItem.Name = "";
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.okBtn,
            this.CancelBtn,
            this.barButtonItem1});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 5;
            this.ribbon.Name = "ribbon";
            this.ribbon.Size = new System.Drawing.Size(679, 30);
            this.ribbon.StatusBar = this.FilesBar;
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
            // barButtonItem1
            // 
            this.barButtonItem1.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.barButtonItem1.Caption = "Загрузить в базу файл";
            this.barButtonItem1.Id = 4;
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.ItemLinks.Add(this.okBtn);
            this.ribbonStatusBar.ItemLinks.Add(this.CancelBtn);
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 489);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(679, 23);
            // 
            // FilesBar
            // 
            this.FilesBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.FilesBar.ItemLinks.Add(this.barButtonItem1);
            this.FilesBar.Location = new System.Drawing.Point(2, 21);
            this.FilesBar.Name = "FilesBar";
            this.FilesBar.Ribbon = this.ribbon;
            this.FilesBar.Size = new System.Drawing.Size(670, 23);
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
            this.panelControl1.Controls.Add(this.labelControl3);
            this.panelControl1.Controls.Add(this.ApprovalDurationYears);
            this.panelControl1.Controls.Add(this.progressBar);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.file);
            this.panelControl1.Controls.Add(this.Contractor);
            this.panelControl1.Controls.Add(this.desc);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 30);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(679, 129);
            this.panelControl1.TabIndex = 206;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(9, 47);
            this.progressBar.MenuManager = this.ribbon;
            this.progressBar.Name = "progressBar";
            this.progressBar.Properties.Step = 1;
            this.progressBar.Size = new System.Drawing.Size(658, 21);
            this.progressBar.TabIndex = 215;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(262, 21);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(119, 13);
            this.labelControl1.TabIndex = 214;
            this.labelControl1.Text = "Имя Excel файла, папки";
            // 
            // file
            // 
            this.file.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.file.Location = new System.Drawing.Point(387, 17);
            this.file.MenuManager = this.ribbon;
            this.file.Name = "file";
            this.file.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.file.Properties.ReadOnly = true;
            this.file.Size = new System.Drawing.Size(280, 20);
            this.file.TabIndex = 213;
            this.file.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.file_ButtonClick);
            // 
            // Contractor
            // 
            this.Contractor.BaseFilter = null;
            this.Contractor.Location = new System.Drawing.Point(75, 17);
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
            this.Contractor.Size = new System.Drawing.Size(173, 20);
            this.Contractor.TabIndex = 212;
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 159);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
            this.xtraTabControl1.Size = new System.Drawing.Size(679, 330);
            this.xtraTabControl1.TabIndex = 209;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage1});
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Controls.Add(this.groupControl1);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(674, 304);
            this.xtraTabPage1.Text = "Загрузка новых товаров";
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.Files);
            this.groupControl1.Controls.Add(this.panelControl2);
            this.groupControl1.Controls.Add(this.FilesBar);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(674, 304);
            this.groupControl1.TabIndex = 20;
            this.groupControl1.Text = "Список файлов, которые не удалось загрузить";
            // 
            // Files
            // 
            this.Files.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Files.Location = new System.Drawing.Point(2, 44);
            this.Files.MainView = this.filesGridView;
            this.Files.MenuManager = this.ribbon;
            this.Files.Name = "Files";
            this.Files.Size = new System.Drawing.Size(670, 221);
            this.Files.TabIndex = 19;
            this.Files.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.filesGridView});
            this.Files.DoubleClick += new System.EventHandler(this.Files_DoubleClick);
            // 
            // filesGridView
            // 
            this.filesGridView.GridControl = this.Files;
            this.filesGridView.Name = "filesGridView";
            this.filesGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.labelControl2);
            this.panelControl2.Controls.Add(this.FindArticleAndModelRegEx);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl2.Location = new System.Drawing.Point(2, 265);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(670, 37);
            this.panelControl2.TabIndex = 20;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(9, 12);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(195, 13);
            this.labelControl2.TabIndex = 200;
            this.labelControl2.Text = "Выражение поиска артикула и модели";
            // 
            // FindArticleAndModelRegEx
            // 
            this.FindArticleAndModelRegEx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FindArticleAndModelRegEx.Location = new System.Drawing.Point(210, 8);
            this.FindArticleAndModelRegEx.MenuManager = this.ribbon;
            this.FindArticleAndModelRegEx.Name = "FindArticleAndModelRegEx";
            this.FindArticleAndModelRegEx.Size = new System.Drawing.Size(455, 20);
            this.FindArticleAndModelRegEx.TabIndex = 0;
            // 
            // okButtonItem
            // 
            this.okButtonItem.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.okButtonItem.Caption = "OK";
            this.okButtonItem.Id = 10;
            this.okButtonItem.Name = "okButtonItem";
            // 
            // ApprovalDurationYears
            // 
            this.ApprovalDurationYears.Location = new System.Drawing.Point(270, 81);
            this.ApprovalDurationYears.MenuManager = this.ribbon;
            this.ApprovalDurationYears.Name = "ApprovalDurationYears";
            this.ApprovalDurationYears.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.ApprovalDurationYears.Size = new System.Drawing.Size(100, 20);
            this.ApprovalDurationYears.TabIndex = 216;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(9, 84);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(244, 13);
            this.labelControl3.TabIndex = 217;
            this.labelControl3.Text = "Длительность разр. док-та по умолчанию (лет)";
            // 
            // LoadingEuroluxForm
            // 
            this.AllowDisplayRibbon = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 512);
            this.Controls.Add(this.xtraTabControl1);
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
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtraTabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Files)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filesGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FindArticleAndModelRegEx.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ApprovalDurationYears.Properties)).EndInit();
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
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar FilesBar;
        private DevExpress.XtraGrid.GridControl Files;
        private DevExpress.XtraGrid.Views.Grid.GridView filesGridView;
        private DevExpress.XtraBars.BarButtonItem okButtonItem;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit FindArticleAndModelRegEx;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.CalcEdit ApprovalDurationYears;
        }
    }