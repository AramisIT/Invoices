using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SystemInvoice.SystemObjects;
using Aramis;
using Aramis.DatabaseConnector;
using Aramis.IO;
using DevExpress.XtraBars;
using Aramis.Attributes;
using Aramis.Core;
using Aramis.Enums;
using Aramis.UI.WinFormsDevXpress;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using Excel;

namespace SystemInvoice.Catalogs.Forms
    {
    public partial class LoadingEuroluxForm : DevExpress.XtraBars.Ribbon.RibbonForm
        {
        private ILoadingCatalogsFromExcel item;

        private LoadingEuroluxBehaviour itemBehaviour
            {
            get { return Item.GetBehaviour<LoadingEuroluxBehaviour>(); }
            }

        public ILoadingCatalogsFromExcel Item
            {
            get { return item; }
            set
                {
                item = value;
                Text = string.Format("Загрузка Excel ({0})", item.LoadingType.GetEnumDescription());
                }
            }

        public LoadingEuroluxForm()
            {
            InitializeComponent();
            }

        private void CancelBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            DialogResult = DialogResult.Cancel;
            }

        private void NewGoodsRowForm_Load(object sender, EventArgs e)
            {

            }

        private void okBtn_ItemClick(object sender, ItemClickEventArgs e)
            {

            }

        private void file_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
            {
            switch (Item.LoadingType)
                {
                case CatalogBaseLoadingTypes.NomenclatureDatabase:
                    string fileName;
                    if (!AramisIO.ChooseExcel97_2003File(out fileName)) return;

                    progressBar.Position = 0;
                    (sender as ButtonEdit).Text = fileName;
                    itemBehaviour.LoadWaresDatabaseFromExcel(fileName, notifyPercentChanged);
                    break;

                case CatalogBaseLoadingTypes.Nomenclature:
                case CatalogBaseLoadingTypes.Approvals:
                    OpenFileFolderDialogResult selectingResult;
                    if (!AramisIO.ChooseFilesOrFolder(AramisIO.FilesTypesFilters.Excel, out selectingResult)) return;
                    var files = selectingResult.GetAllFiles();
                    if (selectingResult.SelectedFolder && files.Count == 0)
                        {
                        "В папке не найдено ни одного подходящего файла Excel".NotifyToUser(MessagesToUserTypes.Error);
                        return;
                        }

                    (sender as ButtonEdit).Text = selectingResult.SelectedFolder ? Path.GetDirectoryName(selectingResult.FolderName) :
                        (selectingResult.SelectedOneFile ? Path.GetDirectoryName(selectingResult.FileName) : string.Empty);

                    itemBehaviour.LoadExcelFiles(files, notifyPercentChanged);
                    break;
                }
            }

        private void notifyPercentChanged(double percent)
            {
            if (this.InvokeRequired)
                {
                this.Invoke(new Action<double>(notifyPercentChanged), new object[] { percent });
                }
            else
                {
                var currentValue = Convert.ToInt32(percent * 100.0);
                progressBar.Position = currentValue;
                progressBar.Update();
                }
            }

        private void Files_DoubleClick(object sender, EventArgs e)
            {
            var hitInfo = Files.GetHitInfo();
            if (hitInfo.RowHandle < 0) return;
            var row = Item.Files[filesGridView.GetDataSourceRowIndex(hitInfo.RowHandle)];
            if (row == null) return;

            try
                {
                Process.Start(row.FullFileName);
                }
            catch (Exception exp)
                {
                string.Format("На данном компьютере не удается автоматически открыть сохраненный файл:\r\n{0}", exp.Message).NotifyToUser(MessagesToUserTypes.Error);
                }
            }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
            {
            var sourceRowIndex = filesGridView.GetFocusedDataSourceRowIndex();
            if (sourceRowIndex < 0) return;
            var dataSource = Item.Files[sourceRowIndex];
            file.Text = dataSource.FullFileName;
            itemBehaviour.LoadExcelFiles(new List<string>() { dataSource.FullFileName }, notifyPercentChanged);
            }


        }
    }