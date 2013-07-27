using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.Excel;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureCheck;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureDeclarationName
    {
    /// <summary>
    /// Проверяет соответствие наименования декларации значению сохраненному в базе для номенклатуры
    /// </summary>
    public class NomenclatureDeclarationNameChecker : NomenclatureChecker
        {
        private readonly string nomenclatureDeclarationName;
        public NomenclatureDeclarationNameChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            nomenclatureDeclarationName = ProcessingConsts.ColumnNames.NOMENCLATURE_DECLARATION_COLUMN_NAME;
            }

        protected override bool CheckThatEquals(NomenclatureCacheObject nomenclatureCacheObject, string expectededValue)
            {
            return nomenclatureCacheObject.NameDecl.Equals(expectededValue);
            }

        protected override string ColumnToCheck
            {
            get { return nomenclatureDeclarationName; }
            }

        protected override bool CheckThatHaveToProcess(ExcelMapper mapper)
            {
            return mapper.ContainsKey(nomenclatureDeclarationName);
            }

        protected override bool CheckExpectedValue(string expectedValue, ExcelMapper mapper)
            {
            return true;
            }

        protected override CellError CreateError(string expectedValue, NomenclatureCacheObject nomenclatureCacheObject)
            {
            if (nomenclatureCacheObject == null)
                {
                return null;
                }
            return new NomenclatureDeclartionError(expectedValue, nomenclatureCacheObject.NameDecl, nomenclatureDeclarationName);
            }
        }
    }
