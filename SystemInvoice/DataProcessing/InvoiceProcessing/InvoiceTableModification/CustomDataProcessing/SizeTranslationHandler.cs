using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.CustomDataProcessing
    {
    /// <summary>
    /// Осуществляет перевод размеров и получает длинну стельки для товаров.
    /// </summary>
    internal class SizeTranslationHandler
        {
        private SystemInvoiceDBCache dbCache = null;

        public SizeTranslationHandler( SystemInvoiceDBCache invoiceDBCache)
            {
            this.dbCache = invoiceDBCache;
            }

        public void MakeTranslation(DataTable dataTable)
            {
            if (this.needTranslateSize(dataTable))
                {
                this.translateSize(dataTable);
                }
            if(this.needGetInsoleLength(dataTable))
                {
                this.fillInsoleLengths(dataTable);
                }
            }

        private void fillInsoleLengths(DataTable dataTable)
            {
            foreach (DataRow dataRow in dataTable.Rows)
                {
                string groupOfGoodsName = Helpers.InvoiceDataRetrieveHelper.GetRowGroupName(dataRow);
                string subGroupOfGoodsName = Helpers.InvoiceDataRetrieveHelper.GetRowSubGroupName(dataRow);
                string subGroupOfGoodsCode = Helpers.InvoiceDataRetrieveHelper.GetRowSubGroupCode(dataRow);
                string article = Helpers.InvoiceDataRetrieveHelper.GetRowArticle(dataRow);
                string tradeMark = Helpers.InvoiceDataRetrieveHelper.GetRowTradeMark(dataRow);
                string size = dataRow.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.SIZE_COLUMN_NAME, "").Trim();//Helpers.InvoiceDataRetrieveHelper.GetRowSize(dataRow);
                if (string.IsNullOrEmpty(size))
                    {
                    continue;
                    }
                string insoleLength = dbCache.GetInsoleLength(groupOfGoodsName, subGroupOfGoodsName, subGroupOfGoodsCode,
                    article, tradeMark, size);

                dataRow[ProcessingConsts.ColumnNames.INSOLE_LENGTH_COLUMN_NAME] = insoleLength;
                }
            }

        /// <summary>
        /// Проверять нужно ли подтягивать длинну стельки из справочника виды свойств (просто проверяем что у нас есть колонка размер, на основании которой можно подтянуть длинну стельки
        /// и, собственно сама колонка длинна стельки
        /// </summary>
        /// <param name="dataTable">Обрабатываемая таблица</param>
        private bool needGetInsoleLength(DataTable dataTable)
            {
            return dataTable.Columns.Contains(ProcessingConsts.ColumnNames.SIZE_COLUMN_NAME) &&
                   dataTable.Columns.Contains(ProcessingConsts.ColumnNames.INSOLE_LENGTH_COLUMN_NAME);
            }


        private void translateSize(DataTable dataTable)
            {
            foreach (DataRow dataRow in dataTable.Rows)
                {
                string groupOfGoodsName = Helpers.InvoiceDataRetrieveHelper.GetRowGroupName(dataRow);
                string subGroupOfGoodsName = Helpers.InvoiceDataRetrieveHelper.GetRowSubGroupName(dataRow);
                string subGroupOfGoodsCode = Helpers.InvoiceDataRetrieveHelper.GetRowSubGroupCode(dataRow);
                string article = Helpers.InvoiceDataRetrieveHelper.GetRowArticle(dataRow);
                string tradeMark = Helpers.InvoiceDataRetrieveHelper.GetRowTradeMark(dataRow);
                string originalSize = Helpers.InvoiceDataRetrieveHelper.GetOriginalSize(dataRow);

                if (string.IsNullOrEmpty(originalSize))
                    {
                    continue;
                    }
                string translatedSize = dbCache.GetTranslatedSize(groupOfGoodsName, subGroupOfGoodsName, subGroupOfGoodsCode, originalSize,
                                                            tradeMark, article);

                dataRow[ProcessingConsts.ColumnNames.SIZE_COLUMN_NAME] = translatedSize;
                }
            }

        /// <summary>
        /// Провряет нужно ли осуществлять перевод размеров (есть ли колонка с исходными размерами и есть ли колонка куда записывать новые размеры)
        /// </summary>
        /// <param name="dataTable">Обрабатываемая таблица</param>
        private bool needTranslateSize(DataTable dataTable)
            {
            return dataTable.Columns.Contains(ProcessingConsts.ColumnNames.SIZE_ORIGINAL_COLUMN_NAME) &&
                   dataTable.Columns.Contains(ProcessingConsts.ColumnNames.SIZE_COLUMN_NAME);
            }
        }
    }