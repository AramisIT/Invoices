using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.PropertyTypesCache;
using SystemInvoice.Excel;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.RowIsLoaded
    {
    /// <summary>
    /// Проверяет была ли найдена номенклатурная позиция для определенной строки, в случае если номенклатура не найдена - добавляет ошибку для специальной
    /// служебной колонки, при обнаружении ошибки в которой, строка помечается как с новой/незагруженной номенклатурой
    /// </summary>
    public class RowIsLoadedChecker : LoadedDocumentCheckerBase
        {
        public RowIsLoadedChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            }

        protected override void CheckRow(System.Data.DataRow rowToCheck, ExcelMapper mapper, bool isDocumentCurrentlyLoaded, string currentCheckedColumnName)
            {
            long nomenclatureId = Helpers.InvoiceDataRetrieveHelper.GetRowNomenclatureId(rowToCheck);
            if (nomenclatureId == 0)
                {
                AddError(RowCheckError.ROW_IS_INVALID_FAKE_COLUMN_NAME, new RowCheckError());
                }
            }
        }
    }
