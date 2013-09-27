using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Helpers
    {
    /// <summary>
    /// Класс - хэлпер получающий данные из табличной части инвойса
    /// </summary>
    public class InvoiceDataRetrieveHelper
        {
        /// <summary>
        /// Возвращает ID номенклатуры из строки табличной части Инвойса
        /// </summary>
        /// <param name="invoiceGoodsRow">Строка табличной части инвойса</param>
        public static long GetRowNomenclatureId(DataRow invoiceGoodsRow)
            {
            return invoiceGoodsRow.TrySafeGetColumnValue<long>(ProcessingConsts.ColumnNames.FOUNDED_NOMENCLATURE_COLUMN_NAME, 0);
            }
        
        /// <summary>
        /// Возвращает номер внутреннего таможенного кода из строки табличной части Инвойса
        /// </summary>
        /// <param name="invoiceGoodsRow">Строка табличной части инвойса</param>
        /// <returns></returns>
        public static string GetRowCustomsCode(DataRow invoiceGoodsRow)
            {
            return invoiceGoodsRow.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.CUSTOM_CODE_INTERNAL_COLUMN_NAME, string.Empty).Trim(); ;
            }

        /// <summary>
        /// Возвращает значение веса нетто единицы товара из строки табличной части Инвойса
        /// </summary>
        /// <param name="invoiceGoodsRow">Строка табличной части инвойса</param>
        /// </summary>
        public static double GetRowItemNetWeight(DataRow invoiceGoodsRow)
            {
            string itemNetWeightStr = invoiceGoodsRow.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.ITEM_NET_WEIGHT_COLUMN_NAME, string.Empty).Trim();
            double itemNetWeightVal;
            if (!double.TryParse(itemNetWeightStr, out itemNetWeightVal))
                {
                return double.NaN;
                }
            return Math.Round(itemNetWeightVal, 3);
            }

        /// <summary>
        /// Возвращает значение веса нетто из строки табличной части Инвойса
        /// </summary>
        /// <param name="invoiceGoodsRow">Строка табличной части инвойса</param>
        /// </summary>
        public static double GetRowNetWeight(DataRow invoiceGoodsRow)
            {
            string netWeightStr = invoiceGoodsRow.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.NET_WEIGHT_COLUMN_NAME, string.Empty).Trim(); 
            double netWeightVal;
            if (!double.TryParse(netWeightStr, out netWeightVal))
                {
                return double.NaN;
                }
            return Math.Round(netWeightVal, 3);
            }

        /// <summary>
        /// Возвращает значение веса нетто от, для номенклатуры в базе
        /// </summary>
        /// <param name="dbCache">Кэш</param>
        /// <param name="row">Строка табличной части инвойса</param>
        public static double GetNomenclatureNetWeightFrom(SystemInvoiceDBCache dbCache, DataRow row)
            {
            var cached = getNomenclatureCached(dbCache, row);
            return Math.Round(cached == null ? 0 : cached.NetWeightFrom, 3);
            }

        /// <summary>
        /// Возвращает значение веса нетто до, для номенклатуры в базе
        /// </summary>
        /// <param name="dbCache">Кэш</param>
        /// <param name="row">Строка табличной части инвойса</param>
        public static double GetNomenclatureNetWeightTo(SystemInvoiceDBCache dbCache, DataRow row)
            {
            var cached = getNomenclatureCached(dbCache, row);
            return Math.Round(cached == null ? 0 : cached.NetWeightTo, 3);
            }

        /// <summary>
        /// Возвращает соответствующее значение кешированной номенклатуры
        /// </summary>
        /// <param name="dbCache">Кэш</param>
        /// <param name="row">Табличная часть инвойса</param>
        private static NomenclatureCacheObject getNomenclatureCached(SystemInvoiceDBCache dbCache, DataRow row)
            {
            long nomenclatureId = GetRowNomenclatureId(row);
            if (nomenclatureId == 0)
                {
                return null;
                }
            return dbCache.NomenclatureCacheObjectsStore.GetCachedObject(nomenclatureId);
            }

        /// <summary>
        /// Возвращает количество единиц товара из табличной части инвойса
        /// </summary>
        /// <param name="row">Строка табличной части инвойса</param>
        public static int GetNomenclaturesCount(DataRow row)
            {
            int count = 0;
            string countStr = row.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.COUNT_COLUMN_NAME, "").Trim();
            int.TryParse(countStr, out count);
            return count;
            }

        /// <summary>
        /// Возвращает номер строки из табличной части инвойса
        /// </summary>
        /// <param name="row">Строка табличной части инвойса</param>
        public static long GetRowLineNumber(DataRow row)
            {
            long lineNumber = row.TrySafeGetColumnValue<long>(ProcessingConsts.ColumnNames.LINE_NUMBER_COLUMN_NAME, 0);
            return lineNumber;
            }

        /// <summary>
        /// Возвращает дату инвойса из табличной части
        /// </summary>
        /// <param name="row">строка табличной части инвойса</param>
        public static DateTime GetRowInvoiceDate(DataRow row)
            {
            string invoiceDateStr = row.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.INVOICE_DATE_COLUMN_NAME, string.Empty).Trim();
            DateTime dt;
            if (!DateTime.TryParse(invoiceDateStr, out dt))
                {
                dt = DateTime.MinValue;
                }
            return dt;
            }

        /// <summary>
        /// Возвращет наименование группы товара из табличной части инвойса
        /// </summary>
        /// <param name="row">Строка в табличной части инвойса</param>
        public static string GetRowGroupName(DataRow row)
            {
            return row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.GROUP_OF_GOODS_COLUMN_NAME, "").Trim();
            }

        /// <summary>
        /// Возвращает наименование подгруппы из табличной части инвойса
        /// </summary>
        /// <param name="row">Строка в табличной части инвойса</param>
        public static string GetRowSubGroupName(DataRow row)
            {
            return row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.SUB_GROUP_OF_GOODS_COLUMN_NAME, "").Trim();
            }

        /// <summary>
        /// Возвращает код подгруппы из табличной части инвойса
        /// </summary>
        /// <param name="row">Строка в табличной части инвойса</param>
        public static string GetRowSubGroupCode(DataRow row)
            {
            return row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.GROUP_CODE_COLUMN_NAME, "").Trim();
            }

        /// <summary>
        /// Возвращает имя торговой марки из табличной части инвойса
        /// </summary>
        /// <param name="row">Строка в табличной части инвойса</param>
        public static string GetRowTradeMark(DataRow row)
            {
            return row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.TRADEMARK_COLUMN_NAME, "").Trim();
            }

        /// <summary>
        /// Возвращает артикул из табличной части инвойса
        /// </summary>
        /// <param name="row">Строка табличной части инвойса</param>
        public static string GetRowArticle(DataRow row)
            {
            return row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.ARTICLE_COLUMN_NAME, "").Trim();
            }

        /// <summary>
        /// Возвращает имя производителя из табличной части инвойса
        /// </summary>
        /// <param name="row">Строка в табличной части инвойса</param>
        public static string GetRowManufacturerName(DataRow row)
            {
            return row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.MANUFACTURER_COLUMN_NAME, "").Trim();
            }

        /// <summary>
        /// Возвращает код страны из табличной части инвойса
        /// </summary>
        /// <param name="row">Строка из табличной части инвойса</param>
        public static string GetRowCountryCode(DataRow row)
            {
            return row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.COUNTRY_COLUMN_NAME, "").Trim();
            }
        /// <summary>
        /// Возвращает единицу измерения из табличной части инвойса
        /// </summary>
        /// <param name="row">Строка из табличной части инвойса</param>
        /// </summary>
        public static string GetRowUnitOfMeasure(DataRow row)
            {
            return row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.UNIT_OF_MEASURE_COLUMN_NAME, "").Trim();
            }

        /// <summary>
        /// Возвращает таможенный код внутренний из табличной части инвойса
        /// </summary>
        /// <param name="row">Строка из табличной части инвойса</param>
        public static string GetRowCustomsCodeExtern(DataRow row)
            {
            return row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.CUSTOM_CODE_EXTERNAL_COLUMN_NAME, "").Trim();
            }

        /// <summary>
        /// Возвращает штрих-код из табличной части инвойса
        /// </summary>
        /// <param name="row">Строка из табличной части инвойса</param>
        public static string GetRowBarCode(DataRow row)
            {
            return row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.BAR_CODE_COLUMN_NAME, "").Trim();
            }

        /// <summary>
        /// Возвращает вес брутто из табличной части инвойса с точностью до трех знаков
        /// </summary>
        /// <param name="row">Строка из табличной части инвойса</param>
        public static double GetRowGrossWeight(DataRow row)
            {
            string grossWeightStr = row.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.GROSS_WEIGHT_COLUMN_NAME, string.Empty).Trim();
            double grossWeight;
            if (!double.TryParse(grossWeightStr, out grossWeight))
                {
                return double.NaN;
                }
            return Math.Round(grossWeight, 3);
            }

        /// <summary>
        /// Возвращает цену товара из табличной части инвойса с точностью до двух знаков
        /// </summary>
        /// <param name="row">строка из табличной части инвойса</param>
        public static double GetRowPrice(DataRow row)
            {
            string priceStr = row.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME, string.Empty).Trim();
            double price;
            if (!double.TryParse(priceStr, out price))
                {
                return double.NaN;
                }
            return Math.Round(price, 2);
            }

        /// <summary>
        /// Возвращает наименование исходное из табличной части инвойса
        /// </summary>
        /// <param name="row">строка в табличной части инвойса</param>
        public static string GetRowOriginalName(DataRow row)
            {
            return row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.NOMENCLATURE_ORIGINAL_COLUMN_NAME, "").Trim();
            }

        /// <summary>
        /// Возвращает наименование декларации из табличной части инвойса
        /// </summary>
        /// <param name="row">Наименование декларации</param>
        public static string GetRowDeclarationName(DataRow row)
            {
            return row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.NOMENCLATURE_DECLARATION_COLUMN_NAME, "").Trim();
            }

        /// <summary>
        /// Возвращает наименование инвойса
        /// </summary>
        /// <param name="row">строка из табличной части инвойса</param>
        public static string GetRowInvoiceName(DataRow row)
            {
            return row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.NOMENCLATURE_INVOICE_COLUM_NAME, "").Trim();
            }

        /// <summary>
        /// Возвращает исходный размер
        /// </summary>
        /// <param name="dataRow">строка из табличной части инвойса</param>
        public static string GetOriginalSize(DataRow dataRow)
            {
            return dataRow.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.SIZE_ORIGINAL_COLUMN_NAME, "").Trim();
            }
        }
    }
