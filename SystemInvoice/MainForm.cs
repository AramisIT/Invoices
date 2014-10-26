using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SystemInvoice.Catalogs.Forms;
using SystemInvoice.DataProcessing.CatalogsProcessing;
using SystemInvoice.Documents;
using SystemInvoice.SystemObjects;
using Aramis.CommonForms;
using Aramis.Core;
using Aramis.DatabaseConnector;
using Aramis.Enums;
using Aramis.NET;
using Aramis.Platform;
using Aramis.Reports;
using Aramis.Reports.Forms;
using Aramis.SystemConfigurations;
using Aramis.UI;
using Aramis.UI.DBObjectsListFilter;
using Aramis.UI.WinFormsDevXpress.Forms;
using Aramis.UI.WinFormsDevXpress.Forms.Documents;
using Catalogs;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using Documents;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing;
using SystemInvoice.Constants;


namespace SystemInvoice
    {
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm, IMainForm
        {
        public static event Action LoadedForm = null;
        CatalogsFromExcelConverter excelConverter = new CatalogsFromExcelConverter();
        public MainForm()
            {
            InitializeComponent();
            this.Load += MainForm_Load;
            }

        void MainForm_Load(object sender, EventArgs e)
            {
            if (LoadedForm != null)
                {
                LoadedForm();
                }
            }

        #region IMainForm members

        public bool AutoStartMode
            {
            get;
            set;
            }

        public void OnAutoStart()
            {
            }

        public Action ShowConnectionTroublesForm
            {
            get;
            set;
            }

        public ImageCollection SmallImagesCollection
            {
            get
                {
                return smallImagesCollection;
                }
            }

        public ImageCollection LargeImagesCollection
            {
            get
                {
                return largeImagesCollection;
                }
            }


        new public UserLookAndFeel LookAndFeel
            {
            get
                {
                return defaultLookAndFeel.LookAndFeel;
                }
            }

        #endregion

        private void openCatalogsListBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowObjectSelectingView(AramisObjectType.Catalog);
            }

        private void openDocumentsListBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowObjectSelectingView(AramisObjectType.Document);
            }

        private void updateDBBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            Aramis.Platform.PlatformMethods.UpdateDB(true);
            }

        private void updateSolutionBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            PlatformMethods.UpdateSolution();
            }

        private void processFile(Action<string> processFileDelegate)
            {
            if (processFileDelegate == null)
                {
                return;
                }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel Files (.xls)|*.xls";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                processFileDelegate(ofd.FileName);
                }
            }

        private void newInvoiceBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            DateTime dateFrom = DateTime.Now;
            UserInterface.Current.ShowItem(new SystemInvoice.Documents.Invoice());
            Console.WriteLine((DateTime.Now - dateFrom).TotalMilliseconds);
            }

        private void invoiceListBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Documents.Invoice));
            }

        private void approvalsCreateBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowItem(new SystemInvoice.Documents.Approvals());
            }

        private void showApprovalsListBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Documents.Approvals));
            }


        private void helpBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            ShowHelp();
            }

        private void ShowHelp()
            {
            CatalogsViewer viewer = new CatalogsViewer("InterfacePages", Ribbon.SelectedPage.Text);
            var page = A.New<InterfacePages>(viewer.Id);
            new HelpForm().ShowPage(String.Format("{0}\\Help\\{1}", SystemAramis.APPLICATION_PATH, page.HelpFile.FileName));
            }

        private void propertyTypesBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            processFile((fileName) => excelConverter.LoadPropertyTypes(fileName));
            }

        private void countriesBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            processFile((fileName) => excelConverter.LoadCountries(fileName));
            }

        private void documentTypesBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            processFile((fileName) => excelConverter.LoadDocumentTypes(fileName));
            }

        private void custsomsCodeBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            processFile((fileName) => excelConverter.LoadCustomsCodes(fileName));
            }

        private void nomenclatureLoadBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            processFile((fileName) => excelConverter.LoadNomenclatures(fileName));
            }

        private void approvalsLoadFormatBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.ApprovalsLoadFormat));
            }

        private void contractorsBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.IContractor));
            }

        private void countryBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.Country));
            }

        private void currencyBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.Currency));
            }

        private void customsCodesBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.CustomsCode));
            }

        private void documentTypeBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.IDocumentType));
            }

        private void excelLoadingFormatBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.ExcelLoadingFormat));
            }

        private void groupOfGoodsBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.GroupOfGoods));
            }

        private void manufacturerBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.IManufacturer));
            }

        private void nomenclatureBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.Nomenclature));
            }

        private void propertyOfGoodsBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.PropertyOfGoods));
            }

        private void propertyTypeBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.PropertyType));
            }

        private void relationsWithCustomersBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.RelationWithCustomersType));
            }

        private void subGroupOfGoodsBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.SubGroupOfGoods));
            }

        private void tradeMarkBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.ITradeMark));
            }

        private void UnitOfMeasureBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.UnitOfMeasure));
            }

        private void accessControlBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            PlatformMethods.ObjectsPermissions();
            }

        private void MainForm_Load_1(object sender, EventArgs e)
            {
            CatalogUsers currentUser = SystemAramis.CurrentUser;
            if (currentUser != null)
                {
                UIConsts.Skin = currentUser.Skin;
                administratorPage.Visible = currentUser.Ref == CatalogUsers.Admin;
                }
            }

        private void btnSettings_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowSystemObject(SystemConsts.GetEntity(), new ConstsForm());
            }

        private void openMaterialTypesBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Catalogs.MaterialType));
            }

        private void openMaterialsTypesMappings_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(SystemInvoice.Documents.MaterialTypeMapping));
            }

        private void starterFilesUpload_ItemClick(object sender, ItemClickEventArgs e)
            {
            PlatformMethods.UploadLoaderFiles(true);
            }

        private void barButtonItem44_ItemClick(object sender, ItemClickEventArgs e)
            {
            SystemAramis.SystemStop();
            }

        private void barButtonItem45_ItemClick(object sender, ItemClickEventArgs e)
            {
            SystemAramis.SystemStop(true);
            }

        private void barButtonItem46_ItemClick(object sender, ItemClickEventArgs e)
            {
            PlatformMethods.UpdateDB(false);
            }

        private void barButtonItem47_ItemClick(object sender, ItemClickEventArgs e)
            {
            loadElectrolux(ElectroluxLoadingTypes.NomenclatureDatabase, new ElectroluxLoadingParameters());
            }

        private static void loadElectrolux(ElectroluxLoadingTypes loadingType, LoadingParameters loadingParameters)
            {
            var loadingEurolux = A.New<ILoadingEurolux>();
            loadingEurolux.GetBehaviour<LoadingEuroluxBehaviour>().SetLoadingParameters(loadingParameters);
            loadingEurolux.LoadingType = loadingType;


            var view = new LoadingEuroluxForm() { Item = loadingEurolux };
            UserInterface.Current.ShowSystemObject(loadingEurolux, view);
            }

        private void barButtonItem48_ItemClick(object sender, ItemClickEventArgs e)
            {
            loadElectrolux(ElectroluxLoadingTypes.Approvals, new ElectroluxLoadingParameters());
            }

        private void barButtonItem49_ItemClick(object sender, ItemClickEventArgs e)
            {
            loadElectrolux(ElectroluxLoadingTypes.Nomenclature, new ElectroluxLoadingParameters());
            }

        private void barButtonItem50_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowReport("Разрешительные документы");
            }

        private void barButtonItem51_ItemClick(object sender, ItemClickEventArgs e)
            {
            var info = SystemConfiguration.DBConfigurationTree[typeof(Invoice).GetTableName()];
            foreach (var subtableInfo in info.InfoSubTables.Values)
                {
                var length = 0;
                // if (subtableInfo.Name != "Goods") continue;

                foreach (var infoOfSubTableField in subtableInfo.SubtableFields.Values)
                    {
                    if (infoOfSubTableField.PropertyType == typeof(string)
                             && infoOfSubTableField.Attr.StorageType == StorageTypes.Database)
                        {
                        length += infoOfSubTableField.Attr.Size;
                        }
                    }

                string.Format("Width of {0} table = {1}", subtableInfo.Name, length).NotifyToUser();
                }
            }

        private void barButtonItem52_ItemClick(object sender, ItemClickEventArgs e)
            {
            loadElectrolux(ElectroluxLoadingTypes.NomenclatureDatabase, new WhirlpoolLoadingParameters());
            }

        private void barButtonItem53_ItemClick(object sender, ItemClickEventArgs e)
            {
            loadElectrolux(ElectroluxLoadingTypes.Approvals, new WhirlpoolLoadingParameters());
            }

        }
    }