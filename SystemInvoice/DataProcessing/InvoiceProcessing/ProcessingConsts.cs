using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Documents;

namespace SystemInvoice.DataProcessing.InvoiceProcessing
    {
    /// <summary>
    /// Константы
    /// </summary>
    static class ProcessingConsts
        {
        public const string ROW_NOT_LOADED_COLOR = "#C1C1C1";
        public const string CELL_ERROR_COLOR = "#FF99EA";
        public const string EXCEL_UNLOADING_HEADER_COLOR = "#E0E0E0";
        public const string EXCEL_UNLOAD_AGGREGATE_ROW_COLOR = "#CACACA";
        public const string GRAF31_COLUMN_EN_NAME = "Graf31";
        public const string GRAF31_COLUMN_UK_NAME = "Наим. гр31";
        public const string EXCEL_LOAD_FORMAT_TARGET_COLUMN_COLUMN_NAME = "ColumnName";
        public const string EXCEL_UNLOAD_UNPROCESSED_COLUMNS_INDEX_MAPPING = "UnloadNewItemsColumnNumber";
        public const string EXCEL_UNLOAD_PROCESSED_COLUMN_NUMBER_NAME = "UnloadColumnNumber";
        public const string LOAD_FORMAT_NOT_FOUND_MESSAGE = "Oтсутствует формат загрузки";
        public const string NOT_EXISTS_MANUFACTURER_NAME = "немає даних";
        public const int CHECKING_APPROVALS_COUNT = 5;
        /// <summary>
        /// Колонки инвойса
        /// </summary>
        public static class ColumnNames
            {
            public const string FOUNDED_NOMENCLATURE_COLUMN_NAME = "FoundedNomenclature";
            public const string FOUNDED_SUB_GROUP_OF_GOODS = "FoundedSubGroupOfGoods";
            public const string LINE_NUMBER_COLUMN_NAME = "LineNumber";

            private static string customCodeInternalColumnName = InvoiceColumnNames.CustomsCodeIntern.ToString();
            private static string countryColumnName = InvoiceColumnNames.Country.ToString();
            private static string manufacturerColumnName = InvoiceColumnNames.ItemContractor.ToString();
            private static string trademarkColumnName = InvoiceColumnNames.ItemTradeMark.ToString();
            private static string aritecleColumnName = InvoiceColumnNames.Article.ToString();
            private static string sizeColumnName = InvoiceColumnNames.Size.ToString();
            private static string invoiceNumberColumnName = InvoiceColumnNames.InvoiceNumber.ToString();
            private static string invoiceDateColumnName = InvoiceColumnNames.InvoiceDate.ToString();
            private static string SubGroupOfGoodsColumnName = InvoiceColumnNames.SubGroupOfGoods.ToString();
            private static string groupCodeColumnName = InvoiceColumnNames.GroupCode.ToString();
            private static string priceColumnName = InvoiceColumnNames.Price.ToString();
            private static string grossWeightColumnName = InvoiceColumnNames.ItemGrossWeight.ToString();
            private static string countColumnName = InvoiceColumnNames.Count.ToString();
            private static string netWeightColumnName = InvoiceColumnNames.NetWeight.ToString();
            private static string sumColumnName = InvoiceColumnNames.Sum.ToString();
            private static string itemNetWeightColumnName = InvoiceColumnNames.UnitWeight.ToString();
            private static string contentColumnName = InvoiceColumnNames.Content.ToString();
            private static string marginPercentageColumnName = InvoiceColumnNames.MarginPrecentage.ToString();
            private static string number_of_places_column_name = InvoiceColumnNames.ItemNumberOfPlaces.ToString();
            private static string groupOfGoodsColumnName = SystemInvoice.Documents.InvoiceColumnNames.GroupOfGoods.ToString();
            private static string unitOfMeasureColumnName = SystemInvoice.Documents.InvoiceColumnNames.UnitOfMeasure.ToString();
            private static string customCodeExternalColumnName = SystemInvoice.Documents.InvoiceColumnNames.CustomsCodeExtern.ToString();
            private static string barCodeColumnName = SystemInvoice.Documents.InvoiceColumnNames.BarCode.ToString();
            private static string nomenclatureOrginalColumnName = SystemInvoice.Documents.InvoiceColumnNames.OriginalName.ToString();
            private static string nomenclatureDeclarationColumnName = SystemInvoice.Documents.InvoiceColumnNames.NomenclatureDeclaration.ToString();
            private static string nomenclatureInvoiceColumnName = SystemInvoice.Documents.InvoiceColumnNames.NomenclatureInvoice.ToString();
            private static string unitOfMeasureCodeColumnName = InvoiceColumnNames.UnitOfMeasureCode.ToString();
            private static string graf31ColumnName = InvoiceColumnNames.Graf31.ToString();
            public static string sizeOriginalColumnName = InvoiceColumnNames.SizeOriginal.ToString();
            private static string insoleLengthColumnName = InvoiceColumnNames.InsoleLength.ToString();
                                              
            public static string CUSTOM_CODE_INTERNAL_COLUMN_NAME
                {
                get { return customCodeInternalColumnName; }
                }

            public static string COUNTRY_COLUMN_NAME
                {
                get { return countryColumnName; }
                }

            public static string MANUFACTURER_COLUMN_NAME
                {
                get { return manufacturerColumnName; }
                }

            public static string TRADEMARK_COLUMN_NAME
                {
                get { return trademarkColumnName; }
                }

            public static string ARTICLE_COLUMN_NAME
                {
                get { return aritecleColumnName; }
                }

            public static string SIZE_COLUMN_NAME
                {
                get { return sizeColumnName; }
                }

            public static string INVOICE_NUMBER_COLUMN_NAME
                {
                get { return invoiceNumberColumnName; }
                }

            public static string INVOICE_DATE_COLUMN_NAME
                {
                get { return invoiceDateColumnName; }
                }

            public static string SUB_GROUP_OF_GOODS_COLUMN_NAME
                {
                get { return SubGroupOfGoodsColumnName; }
                }

            public static string GROUP_CODE_COLUMN_NAME
                {
                get { return groupCodeColumnName; }
                }

            public static string PRICE_COLUMN_NAME
                {
                get { return priceColumnName; }
                }

            public static string GROSS_WEIGHT_COLUMN_NAME
                {
                get { return grossWeightColumnName; }
                }

            public static string COUNT_COLUMN_NAME
                {
                get { return countColumnName; }
                }

            public static string NET_WEIGHT_COLUMN_NAME
                {
                get { return netWeightColumnName; }
                }

            public static string ITEM_NET_WEIGHT_COLUMN_NAME
                {
                get { return itemNetWeightColumnName; }
                }

            public static string SUM_COLUMN_NAME
                {
                get { return sumColumnName; }
                }

            public static string CONTENT_COLUMN_NAME
                {
                get { return contentColumnName; }
                }

            public static string MARGIN_PRECENTAGE_COLUMN_NAME
                {
                get { return marginPercentageColumnName; }
                }

            public static string NUMBER_OF_PLACES_COLUMN_NAME
                {
                get { return number_of_places_column_name; }
                }

            public static string GROUP_OF_GOODS_COLUMN_NAME
                {
                get { return groupOfGoodsColumnName; }
                }

            public static string UNIT_OF_MEASURE_COLUMN_NAME
                {
                get { return unitOfMeasureColumnName; }
                }

            public static string CUSTOM_CODE_EXTERNAL_COLUMN_NAME
                {
                get { return customCodeExternalColumnName; }
                }

            public static string BAR_CODE_COLUMN_NAME
                {
                get { return barCodeColumnName; }
                }

            public static string NOMENCLATURE_ORIGINAL_COLUMN_NAME
                {
                get { return nomenclatureOrginalColumnName; }
                }

            public static string NOMENCLATURE_DECLARATION_COLUMN_NAME
                {
                get { return nomenclatureDeclarationColumnName; }
                }

            public static string NOMENCLATURE_INVOICE_COLUM_NAME
                {
                get { return nomenclatureInvoiceColumnName; }
                }

            public static string UNIT_OF_MEASURE_CODE_COLUMN_NAME
                {
                get { return unitOfMeasureCodeColumnName; }
                }

            public static string GRAF31_COLUMN_NAME
                {
                get { return graf31ColumnName; }
                }

            public static string SIZE_ORIGINAL_COLUMN_NAME
                {
                get { return sizeOriginalColumnName; }
                }


            public static string INSOLE_LENGTH_COLUMN_NAME 
                {
                get { return insoleLengthColumnName; }
                }
            }
        }
    }
