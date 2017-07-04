using Aramis.CommonForms;
using Aramis.Core;
using Aramis.Enums;
using Aramis.Platform;
using Aramis.SystemConfigurations;
using Aramis.UI;
using Catalogs;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraBars;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SystemInvoice.Catalogs.Forms;
using SystemInvoice.Constants;
using SystemInvoice.DataProcessing.CatalogsProcessing;
using SystemInvoice.Documents;
using SystemInvoice.SystemObjects;
using Aramis;
using DevExpress.XtraEditors;

namespace SystemInvoice
    {
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm, IMainForm
        {
        public static event Action LoadedForm = null;
        private CatalogsFromExcelConverter excelConverter;
        public MainForm()
            {
            InitializeComponent();
            this.Load += MainForm_Load;
            if (!SystemAramis.DeployMode)
                {
                initExcelConverter();
                }
            }

        private void initExcelConverter()
            {
            excelConverter = new CatalogsFromExcelConverter();
            }

        void MainForm_Load(object sender, EventArgs e)
            {
            if (SystemAramis.DeployMode)
                {
                initExcelConverter();
                }

            if (LoadedForm != null)
                {
                LoadedForm();
                }

            if (SolutionRoles.ElectroluxLoader)
                {
                Ribbon.AddMenuItem("Загрузить базу товаров Электролюкс",
                    () => loadFromExcel(CatalogBaseLoadingTypes.NomenclatureDatabase, new ElectroluxLoadingParameters()));

                Ribbon.AddMenuItem("Удалить все данные (номенклатура и разрешительные документы) по Электролюкс", removeElectrolux);
                }

            Ribbon.AddMenuItem("Upload database from file", () =>
                {
                    string filePath;
                    if (!AramisIO.ChooseFile("Sql script |*.sql", out filePath)) return;

                    new Thread(uploadData).Start(filePath);
                });
            }

        private void setMessage(string message)
            {
            if (this.InvokeRequired)
                {
                this.Invoke(new Action<string>(setMessage), new object[] { message });
                }
            else
                {
                this.Text = message;
                }
            }
        private void uploadData(object filePathObj)
            {
            int lineNumber = 0;
            string nextLine;
            long totalLines = 0;
            string filePath = filePathObj as string;
            using (StreamReader sr = new StreamReader(filePath))
                {
                String line = "";
                while ((nextLine = sr.ReadLine()) != null)
                    {
                    totalLines++;
                    }
                }

            int percent = 0;
            long processedLines = 0, errorsCount = 0;
            using (StreamReader sr = new StreamReader(filePath))
                {
                String line = "";
                while ((nextLine = sr.ReadLine()) != null)
                    {
                    lineNumber++;
                    nextLine = nextLine.Trim();
                    processedLines++;

                    if (string.IsNullOrEmpty(nextLine)
                        || string.Equals("go", nextLine, StringComparison.OrdinalIgnoreCase)
                        || nextLine.StartsWith("use ", StringComparison.InvariantCultureIgnoreCase)) continue;

                    var success = uploadData(line, nextLine, lineNumber);
                    if (!success)
                        {
                        errorsCount++;
                        }
                    int currentPercent = (int)(100.0 * processedLines / totalLines);
                    if (percent != currentPercent)
                        {
                        percent = currentPercent;
                        setMessage($"Data uploading ... {currentPercent} %");
                        }
                    line = nextLine;
                    }

                uploadData(line, "SET IDENTITY_INSERT [dbo].[NonExistentTable] OFF", lineNumber);
                }

            setMessage($"Обработано {processedLines} строк, ошибок {errorsCount}!");
            }

        private string identityInsert = "";

        private bool setIdentityInsert(string str)
            {
            return str.StartsWith("SET IDENTITY_INSERT [dbo].[", StringComparison.InvariantCulture);
            }

        private bool isInsertLine(string str, out string tableName)
            {
            var result = str.StartsWith("INSERT ", StringComparison.OrdinalIgnoreCase);

            tableName = "";

            if (result)
                {
                tableName = str.Substring(str.IndexOf(".[") + 2);
                tableName = tableName.Substring(0, tableName.IndexOf("]"));
                }

            return result;
            }

        private string lastTruncatedTable = "";

        private bool uploadData(string sql, string nextLine, int lineNumber)
            {
            if (string.IsNullOrEmpty(sql)) return true;

            sql = sql.Replace("  ", " ");

            if (setIdentityInsert(sql))
                {
                if (sql.EndsWith(" ON", StringComparison.InvariantCulture))
                    {
                    identityInsert = sql;

                    var tableToUpdate = sql.Substring(sql.IndexOf(".[") + 2);
                    tableToUpdate = tableToUpdate.Substring(0, tableToUpdate.IndexOf("]"));

                    insertStarted = false;
                    sql = $"truncate table {tableToUpdate}";
                    lastTruncatedTable = tableToUpdate;
                    }
                else
                    {
                    return true;
                    }
                }

            if (insertStarted)
                {
                cmdBuilder.Append(sql);

                string t;
                if (isInsertLine(nextLine, out t) || setIdentityInsert(nextLine))
                    {
                    sql = cmdBuilder.ToString();
                    insertStarted = false;
                    }
                else
                    {
                    return true;
                    }
                }

            string insertToTableName, t2;
            var currentLineIsInsertCmd = isInsertLine(sql, out insertToTableName);
            if (currentLineIsInsertCmd)
                {
                if (!lastTruncatedTable.Equals(insertToTableName))
                    {
                    execute($"truncate table {insertToTableName}", -1);
                    lastTruncatedTable = insertToTableName;
                    identityInsert = "";
                    }
                }

            if (currentLineIsInsertCmd
                && !isInsertLine(nextLine, out t2)
                && !setIdentityInsert(nextLine))
                {
                insertStarted = true;
                cmdBuilder.Clear();
                cmdBuilder.Append(sql);
                }
            else
                {
                if (currentLineIsInsertCmd)
                    {
                    sql = (!string.IsNullOrEmpty(identityInsert) ? identityInsert + ";   " : "") + sql;
                    }
                return execute(sql, lineNumber);
                }

            return true;
            }

        private bool insertStarted;
        private StringBuilder cmdBuilder = new StringBuilder();

        private bool execute(string sqlCommand, int lineNumber)
            {
            var q = A.Query(sqlCommand);
            q.Execute();

            var success = q.ThrowedException == null;
            if (!success)
                {
                var message = q.ThrowedException.Message;
                if (!message.Contains("'dbo.EnumsInfo'")
                    && !message.Contains("'dbo.OneOffTickets'")
                    && !message.Contains("'dbo.QuickStart'"))
                    {
                    Debug.WriteLine($"Error line: {lineNumber} - {message}");
                    }
                else
                    {
                    success = true; // we can ignore such errors
                    }
                }
            return success;
            }

        private void removeElectrolux()
            {
            if (!"Все товары и все разрешительные документы по Электролюксу будут изъяты из системы. Продолжить?".Ask()) return;

            var q = A.Query(@"
DELETE SubApprovalsNomenclatures 
FROM   SubApprovalsNomenclatures n
INNER JOIN
Approvals a  on a.Id = n.IdDoc
and a.Contractor = @Contractor;

delete from Approvals where Contractor = @Contractor;

DELETE FROM [Nomenclature]    WHERE Contractor = @Contractor", new { Contractor = ElectroluxLoadingParameters.ELECTROLUX_CONTRACTOR });

            q.Execute();

            "Удаление завершено!".NotifyToUser(q.ThrowedException == null);
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

        private void MainForm_Load_1(object sender, EventArgs e)
            {
            var currentUser = SystemAramis.CurrentUser;
            if (currentUser != null)
                {
                UIConsts.NotifyUserSkinWasReviewed(currentUser.Skin);
                administratorPage.Visible = SystemAramis.CurrentUserAdmin;
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

        private void barButtonItem47_ItemClick(object sender, ItemClickEventArgs e)
            {
            loadFromExcel(CatalogBaseLoadingTypes.NomenclatureDatabase, new ElectroluxLoadingParameters());
            }

        private static void loadFromExcel(CatalogBaseLoadingTypes loadingType, LoadingParameters loadingParameters)
            {
            var loadingEurolux = A.New<ILoadingCatalogsFromExcel>();
            loadingEurolux.GetBehaviour<LoadingEuroluxBehaviour>().SetLoadingParameters(loadingParameters);
            loadingEurolux.LoadingType = loadingType;


            var view = new LoadingEuroluxForm() { Item = loadingEurolux };
            UserInterface.Current.ShowSystemObject(loadingEurolux, view);
            }

        private void barButtonItem48_ItemClick(object sender, ItemClickEventArgs e)
            {
            loadFromExcel(CatalogBaseLoadingTypes.Approvals, new ElectroluxLoadingParameters());
            }

        private void barButtonItem49_ItemClick(object sender, ItemClickEventArgs e)
            {
            loadFromExcel(CatalogBaseLoadingTypes.Nomenclature, new ElectroluxLoadingParameters());
            }

        private void barButtonItem50_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowReport("Разрешительные документы");
            }

        private void barButtonItem51_ItemClick(object sender, ItemClickEventArgs e)
            {
            var info = SystemConfiguration.Current[typeof(Invoice).GetTableName()];
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
            loadFromExcel(CatalogBaseLoadingTypes.NomenclatureDatabase, new WhirlpoolLoadingParameters());
            }

        private void barButtonItem53_ItemClick(object sender, ItemClickEventArgs e)
            {
            loadFromExcel(CatalogBaseLoadingTypes.Approvals, new WhirlpoolLoadingParameters());
            }

        private void barButtonItem54_ItemClick(object sender, ItemClickEventArgs e)
            {
            UserInterface.Current.ShowList(typeof(IEmptyNumbersSubstitutes));
            }

        }
    }