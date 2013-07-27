using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.RDChecking;
using SystemInvoice.Documents;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.Excel;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.Article;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.Country;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.CustomsCodeInternal;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.GroupOfGoods;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.Manufacturer;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.MarksAndSpenser;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NetWeight;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureDeclarationName;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureInvoiceName;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureOriginalName;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.Price;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.SubGroupOfGoods;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.TradeMark;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.ZaraContent;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.RowIsLoaded;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking
    {
    /// <summary>
    /// Инкапсулирует в себе всю логику проверки таблицы
    /// </summary>
    public class LoadedDocumentCheckerGlobal : LoadedDocumentCheckerBase
        {
        List<LoadedDocumentCheckerBase> processingInstances = new List<LoadedDocumentCheckerBase>();

        public LoadedDocumentCheckerGlobal(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            this.initializeProcessingInstances();
            }

        private void initializeProcessingInstances()
            {
            registerInstance(new MarksAndSpenserDocumentCheker(base.dbCache));
            registerInstance(new ZaraContentDocumentChecker(base.dbCache));
            registerInstance(new ArticleChecker(base.dbCache));
            registerInstance(new CountryChecker(base.dbCache));
            registerInstance(new CustomsCodeInternDocumentChecker(base.dbCache));
            registerInstance(new GroupOfGoodsChecker(base.dbCache));
            registerInstance(new ManufacturerDocumentChecker(base.dbCache));
            registerInstance(new NetWeightChecker(base.dbCache));
            registerInstance(new NomenclatureDeclarationNameChecker(base.dbCache));
            registerInstance(new NomenclatureInvoiceNameChecker(base.dbCache));
            registerInstance(new NomenclatureOriginalNameChecker(base.dbCache));
            registerInstance(new PriceChecker(base.dbCache));
            registerInstance(new SubGroupOfGoodsChecker(base.dbCache));
            registerInstance(new TradeMarkDocumentChecker(base.dbCache));
            registerInstance(new RowIsLoadedChecker(dbCache));
            registerInstance(new RDChecker(dbCache));
            }

        private void registerInstance(LoadedDocumentCheckerBase checker)
            {
            processingInstances.Add(checker);
            }

        protected override void CheckRow(DataRow rowToCheck, ExcelMapper mapper, bool isDocumentCurrentlyLoaded, string currentCheckedColumnName)
            {
            foreach (LoadedDocumentCheckerBase processor in processingInstances)
                {
                foreach (var errors in processor.GetRowErrors(rowToCheck, mapper, isDocumentCurrentlyLoaded, currentCheckedColumnName))
                    {
                    foreach (CellError error in errors.Value)
                        {
                        AddError(errors.Key, error);
                        }
                    }
                }
            }
        }
    }
