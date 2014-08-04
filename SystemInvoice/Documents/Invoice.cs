using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Core;
using Aramis.Attributes;
using Aramis.Enums;
using SystemInvoice.Catalogs;
using System.Data;
using Aramis.DatabaseConnector;
using SystemInvoice.PropsSyncronization;
using Aramis.Platform;
using Catalogs;
using AramisCatalogs = Catalogs;

namespace SystemInvoice.Documents
    {
    /// <summary>
    /// Документ. Используется для обработки входящих файлов инвойса - их проверки, правки, присоединению информации о разрешительных документах
    /// </summary>
    [Document(Description = "Инвойс", GUID = "0ECCE2A0-F801-411B-AE06-E6E488720327", NumberType = NumberType.Int64,
        DateFieldDescription = "Дата создания", ShowResponsibleInList = true, ShowLastModifiedDateInList = true)]
    public class Invoice : DocumentTable, ITradeMarkContractorExcelLoadFormatSource
        {
        private TradeMarkContractorExcelLoadingFormatSyncronizer syncronizer = null;

        public const string CheckingRowNameToUnload = "HaveToBeUnloaded";
        public const string GrafColumnReplaceString = "A";
        public const int MAX_INVOICE_NUMBER_SIZE = 47;
        public const string FILTER_COLUMN_NAME = "TempFilterColumn";
        public static Dictionary<string, string> InvoiceColumnNames = new Dictionary<string, string>();
        public static Dictionary<string, string> InvoiceColumnNamesTranslated = new Dictionary<string, string>();

        static Invoice()
            {
            fillInvoiceColumnNames();
            }

        private static void fillInvoiceColumnNames()
            {
            InvoiceColumnNames.Add("SubGroupOfGoods", "Подгруппа товара");
            InvoiceColumnNames.Add("GroupOfGoods", "Группа товара");
            InvoiceColumnNames.Add("GroupCode", "Код подгруппы");
            InvoiceColumnNames.Add("NomenclatureDeclaration", "Товар(Декларация)");
            InvoiceColumnNames.Add("OriginalName", "Наименование (исходное)");
            InvoiceColumnNames.Add("ItemTradeMark", "Торговая марка");
            InvoiceColumnNames.Add("ItemContractor", "Производитель");
            InvoiceColumnNames.Add("BarCode", "Штрих-код");
            InvoiceColumnNames.Add("Count", "Кол-во");
            InvoiceColumnNames.Add("Price", "Цена");
            InvoiceColumnNames.Add("Margin", "Наценка");
            InvoiceColumnNames.Add("PriceWithMargin", "Цена с наценкой");
            InvoiceColumnNames.Add("MarginPrecentage", "% Наценки");
            InvoiceColumnNames.Add("Sum", "Сумма");
            InvoiceColumnNames.Add("UnitWeight", "Вес ед. товара");
            InvoiceColumnNames.Add("CustomsCodeExtern", "Таможенный код внешний");
            InvoiceColumnNames.Add("CustomsCodeIntern", "Таможенный код внутренний");
            InvoiceColumnNames.Add("Article", "Артикул");
            InvoiceColumnNames.Add("NetWeight", "Вес нетто");
            InvoiceColumnNames.Add("ItemGrossWeight", "Вес брутто");
            InvoiceColumnNames.Add("Gender", "Пол");
            InvoiceColumnNames.Add("ItemNumberOfPlaces", "Места");
            InvoiceColumnNames.Add("UnitOfMeasureCode", "Ед.(Код)");
            InvoiceColumnNames.Add("UnitOfMeasure", "Ед. изм.");
            InvoiceColumnNames.Add("Country", "Страна");
            InvoiceColumnNames.Add("Size", "Размер");
            InvoiceColumnNames.Add("InsoleLength", "Длинна стельки");
            InvoiceColumnNames.Add("ContentBottom", "Состав низ");
            InvoiceColumnNames.Add("Content", "Состав");
            InvoiceColumnNames.Add("InvoiceCode", "Код инвойса");
            InvoiceColumnNames.Add("InvoiceNumber", "№ инвойса");
            InvoiceColumnNames.Add("InvoiceDate", "Дата инвойса");
            InvoiceColumnNames.Add("NomenclatureInvoice", "Товар (инвойс)");
            InvoiceColumnNames.Add("Graf31", "Графа 31");
            InvoiceColumnNames.Add("RDCode1", "РД код");
            InvoiceColumnNames.Add("RDNumber1", "РД №");
            InvoiceColumnNames.Add("RDFromDate1", "РД Дата выдачи");
            InvoiceColumnNames.Add("RDToDate1", "РД Годен до");
            InvoiceColumnNames.Add("RDCode2", "РД 2 код");
            InvoiceColumnNames.Add("RDNumber2", "РД 2 №");
            InvoiceColumnNames.Add("RDFromDate2", "РД 2 Дата выдачи");
            InvoiceColumnNames.Add("RDToDate2", "РД 2 Годен до");
            InvoiceColumnNames.Add("RDCode3", "РД 3 код");
            InvoiceColumnNames.Add("RDNumber3", "РД 3 №");
            InvoiceColumnNames.Add("RDFromDate3", "РД 3 Дата выдачи");
            InvoiceColumnNames.Add("RDToDate3", "РД 3 Годен до");
            InvoiceColumnNames.Add("RDCode4", "РД 4 код");
            InvoiceColumnNames.Add("RDNumber4", "РД 4 №");
            InvoiceColumnNames.Add("RDFromDate4", "РД 4 Дата выдачи");
            InvoiceColumnNames.Add("RDToDate4", "РД 4 Годен до");
            InvoiceColumnNames.Add("RDCode5", "РД 5 код");
            InvoiceColumnNames.Add("RDNumber5", "РД 5 №");
            InvoiceColumnNames.Add("RDFromDate5", "РД 5 Дата выдачи");
            InvoiceColumnNames.Add("RDToDate5", "РД 5 Годен до");
            //Колонки для настройки проверки
            InvoiceColumnNames.Add("ZaraContent1Code", "Код состава (Зара)_1");
            InvoiceColumnNames.Add("ZaraContent1UkrName", "Имя состава укр. (Зара)_1");
            InvoiceColumnNames.Add("ZaraContent1EnName", "Имя состава англ. (Зара)_1");
            InvoiceColumnNames.Add("ZaraContent2Code", "Код состава (Зара)_2");
            InvoiceColumnNames.Add("ZaraContent2UkrName", "Имя состава укр. (Зара)_2");
            InvoiceColumnNames.Add("ZaraContent2EnName", "Имя состава англ. (Зара)_2");
            InvoiceColumnNames.Add("ZaraContent3Code", "Код состава (Зара)_3");
            InvoiceColumnNames.Add("ZaraContent3UkrName", "Имя состава укр. (Зара)_3");
            InvoiceColumnNames.Add("ZaraContent3EnName", "Имя состава англ. (Зара)_3");
            InvoiceColumnNames.Add("ZaraContent4Code", "Код состава (Зара)_4");
            InvoiceColumnNames.Add("ZaraContent4UkrName", "Имя состава укр. (Зара)_4");
            InvoiceColumnNames.Add("ZaraContent4EnName", "Имя состава англ. (Зара)_4");
            InvoiceColumnNames.Add("ZaraContent5Code", "Код состава (Зара)_5");
            InvoiceColumnNames.Add("ZaraContent5UkrName", "Имя состава укр. (Зара)_5");
            InvoiceColumnNames.Add("ZaraContent5EnName", "Имя состава англ. (Зара)_5");
            InvoiceColumnNames.Add("ZaraContent6Code", "Код состава (Зара)_6");
            InvoiceColumnNames.Add("ZaraContent6UkrName", "Имя состава укр. (Зара)_6");
            InvoiceColumnNames.Add("ZaraContent6EnName", "Имя состава англ. (Зара)_6");
            InvoiceColumnNames.Add("ZaraContent7Code", "Код состава (Зара)_7");
            InvoiceColumnNames.Add("ZaraContent7UkrName", "Имя состава укр. (Зара)_7");
            InvoiceColumnNames.Add("ZaraContent7EnName", "Имя состава англ. (Зара)_7");
            InvoiceColumnNames.Add("ZaraContent8Code", "Код состава (Зара)_8");
            InvoiceColumnNames.Add("ZaraContent8UkrName", "Имя состава укр. (Зара)_8");
            InvoiceColumnNames.Add("ZaraContent8EnName", "Имя состава англ. (Зара)_8");
            InvoiceColumnNames.Add("ZaraContent9Code", "Код состава (Зара)_9");
            InvoiceColumnNames.Add("ZaraContent9UkrName", "Имя состава укр. (Зара)_9");
            InvoiceColumnNames.Add("ZaraContent9EnName", "Имя состава англ. (Зара)_9");
            InvoiceColumnNames.Add("ZaraContent10Code", "Код состава (Зара)_10");
            InvoiceColumnNames.Add("ZaraContent10UkrName", "Имя состава укр. (Зара)_10");
            InvoiceColumnNames.Add("ZaraContent10EnName", "Имя состава англ. (Зара)_10");
            InvoiceColumnNames.Add("ZaraContent11Code", "Код состава (Зара)_11");
            InvoiceColumnNames.Add("ZaraContent11UkrName", "Имя состава укр. (Зара)_11");
            InvoiceColumnNames.Add("ZaraContent11EnName", "Имя состава англ. (Зара)_11");
            InvoiceColumnNames.Add("ZaraContent12Code", "Код состава (Зара)_12");
            InvoiceColumnNames.Add("ZaraContent12UkrName", "Имя состава укр. (Зара)_12");
            InvoiceColumnNames.Add("ZaraContent12EnName", "Имя состава англ. (Зара)_12");
            InvoiceColumnNames.Add("ZaraContent13Code", "Код состава (Зара)_13");
            InvoiceColumnNames.Add("ZaraContent13UkrName", "Имя состава укр. (Зара)_13");
            InvoiceColumnNames.Add("ZaraContent13EnName", "Имя состава англ. (Зара)_13");
            InvoiceColumnNames.Add("ZaraContent14Code", "Код состава (Зара)_14");
            InvoiceColumnNames.Add("ZaraContent14UkrName", "Имя состава укр. (Зара)_14");
            InvoiceColumnNames.Add("ZaraContent14EnName", "Имя состава англ. (Зара)_14");
            InvoiceColumnNames.Add("ZaraContent15Code", "Код состава (Зара)_15");
            InvoiceColumnNames.Add("ZaraContent15UkrName", "Имя состава укр. (Зара)_15");
            InvoiceColumnNames.Add("ZaraContent15EnName", "Имя состава англ. (Зара)_15");

            InvoiceColumnNames.Add("MSKnitWovenColumnName", "Состав KnitWoven .(Marks & Spenser)");
            InvoiceColumnNames.Add("BNSInvoicePart", "BNS Номер поставки");InvoiceColumnNames.Add("Graf31FilterColumn", "ФильтрГрафы31");
            InvoiceColumnNames.Add("SizeOriginal", "Размер исходный");
        //     [DataField(Description = "Размер исходный")]
        //SizeOriginal
            //    InvoiceColumnNames.Add( "ShortContent", "Короткий состав(BNS" );
            foreach (KeyValuePair<string, string> pair in InvoiceColumnNames)
                {
                InvoiceColumnNamesTranslated.Add(pair.Value, pair.Key);
                }
            }
        #region Свойства

        #region (DateTime) CurrentDate Текущая дата
        [DataField(Description = "Текущая дата", NotEmpty = true, ReadOnly = true)]
        public DateTime CurrentDate
            {
            get
                {
                return z_CurrentDate;
                }
            set
                {
                if (z_CurrentDate == value)
                    {
                    return;
                    }

                z_CurrentDate = value;
                NotifyPropertyChanged("CurrentDate");
                }
            }
        private DateTime z_CurrentDate = DateTime.Now;
        #endregion

        #region (string) ReplaceStr Автозамена в текущей колонке
        [DataField(Description = "Автозамена в текущей колонке", StorageType = StorageTypes.Local, Size = 100)]
        public string ReplaceStr
            {
            get
                {
                return z_ReplaceStr;
                }
            set
                {
                if (z_ReplaceStr == value)
                    {
                    return;
                    }

                z_ReplaceStr = value;
                NotifyPropertyChanged("ReplaceStr");
                }
            }
        private string z_ReplaceStr;
        #endregion

        #region (ExcelLoadingFormat) ExcelLoadingFormat Формат загрузки
        [DataField(Description = "Формат загрузки", NotEmpty = true, AllowOpenItem = true)]
        public ExcelLoadingFormat ExcelLoadingFormat
            {
            get
                {
                return (ExcelLoadingFormat)GetValueForObjectProperty("ExcelLoadingFormat");
                }
            set
                {
                SetValueForObjectProperty("ExcelLoadingFormat", value);
                }
            }
        #endregion

        #region (Contractor) Contractor Контрагент
        [DataField(Description = "Контрагент", NotEmpty = true, ShowInList = true)]
        public IContractor Contractor
            {
            get
                {
                return (IContractor)GetValueForObjectProperty("Contractor");
                }
            set
                {
                SetValueForObjectProperty("Contractor", value);
                }
            }
        #endregion

        #region (TradeMark) TradeMark Торговая марка
        [DataField(Description = "Торговая марка", ShowInList = true)]
        public ITradeMark TradeMark
            {
            get
                {
                return (ITradeMark)GetValueForObjectProperty("TradeMark");
                }
            set
                {
                SetValueForObjectProperty("TradeMark", value);
                }
            }
        #endregion

        #region (bool) SetGrafHeader Шапка графы 31
        [DataField(Description = "Шапка графы 31")]
        public bool SetGrafHeader
            {
            get
                {
                return z_SetGrafHeader;
                }
            set
                {
                if (z_SetGrafHeader == value)
                    {
                    return;
                    }

                z_SetGrafHeader = value;
                NotifyPropertyChanged("SetGrafHeader");
                }
            }

        private bool z_SetGrafHeader = true;
        #endregion

        #region (double) SumTotal Общая сумма
        [DataField(Description = "Общая сумма", ReadOnly = true, ShowInList = true, DecimalPointsNumber = 2)]
        public double SumTotal
            {
            get
                {
                return z_SumTotal;
                }
            set
                {
                if (z_SumTotal == value)
                    {
                    return;
                    }

                z_SumTotal = value;
                NotifyPropertyChanged("SumTotal");
                }
            }
        private double z_SumTotal = 0;
        #endregion

        #region (double) NetWeightTotal Общий вес нетто
        [DataField(Description = "Общий вес нетто", ReadOnly = true, ShowInList = true, DecimalPointsNumber = 3)]
        public double NetWeightTotal
            {
            get
                {
                return z_NetWeightTotal;
                }
            set
                {
                if (z_NetWeightTotal == value)
                    {
                    return;
                    }

                z_NetWeightTotal = value;
                NotifyPropertyChanged("NetWeightTotal");
                }
            }
        private double z_NetWeightTotal = 0;
        #endregion

        #region (double) GrossWeightTotal Общий вес брутто
        [DataField(Description = "Общий вес брутто", ReadOnly = true, ShowInList = true, DecimalPointsNumber = 3)]
        public double GrossWeightTotal
            {
            get
                {
                return z_GrossWeightTotal;
                }
            set
                {
                if (z_GrossWeightTotal == value)
                    {
                    return;
                    }

                z_GrossWeightTotal = value;
                NotifyPropertyChanged("GrossWeightTotal");
                }
            }
        private double z_GrossWeightTotal = 0;
        #endregion

        #region (int) PlacesTotal Места
        [DataField(Description = "Места", ReadOnly = true, ShowInList = true)]
        public int PlacesTotal
            {
            get
                {
                return z_PlacesTotal;
                }
            set
                {
                if (z_PlacesTotal == value)
                    {
                    return;
                    }

                z_PlacesTotal = value;
                NotifyPropertyChanged("PlacesTotal");
                }
            }
        private int z_PlacesTotal = 0;
        #endregion

        #region (int) CountTotal Количество общее
        [DataField(Description = "Количество общее")]
        public int CountTotal
            {
            get
                {
                return z_CountTotal;
                }
            set
                {
                if (z_CountTotal == value)
                    {
                    return;
                    }

                z_CountTotal = value;
                NotifyPropertyChanged("CountTotal");
                }
            }
        private int z_CountTotal = 0;
        #endregion

        #region (Currency) Currency Валюта
        [DataField(Description = "Валюта", NotEmpty = true, ShowInList = true)]
        public Currency Currency
            {
            get
                {
                return (Currency)GetValueForObjectProperty("Currency");
                }
            set
                {
                SetValueForObjectProperty("Currency", value);
                }
            }
        #endregion

        #region (int) NumberOfPlaces Количество мест
        [DataField(Description = "Количество мест", StorageType = StorageTypes.Local)]
        public int NumberOfPlaces
            {
            get
                {
                return z_NumberOfPlaces;
                }
            set
                {
                if (z_NumberOfPlaces == value)
                    {
                    return;
                    }

                z_NumberOfPlaces = value;
                NotifyPropertyChanged("NumberOfPlaces");
                }
            }
        private int z_NumberOfPlaces = 0;
        #endregion

        #region (double) NetWeightCalc Вес нетто
        [DataField(Description = "Вес нетто", StorageType = StorageTypes.Local, DecimalPointsNumber = 3)]
        public double NetWeightCalc
            {
            get
                {
                return z_NetWeightCalc;
                }
            set
                {
                if (z_NetWeightCalc == value)
                    {
                    return;
                    }

                z_NetWeightCalc = value;
                NotifyPropertyChanged("NetWeightCalc");
                }
            }
        private double z_NetWeightCalc = 0;
        #endregion

        #region (double) GrossWeight Вес брутто
        [DataField(Description = "Вес брутто", StorageType = StorageTypes.Local, DecimalPointsNumber = 3)]
        public double GrossWeight
            {
            get
                {
                return z_GrossWeight;
                }
            set
                {
                if (z_GrossWeight == value)
                    {
                    return;
                    }

                z_GrossWeight = value;
                NotifyPropertyChanged("GrossWeight");
                }
            }
        private double z_GrossWeight = 0;
        #endregion

        #region (string) CurrentNomenclature Текущий товар
        [DataField(Description = "Текущий товар", ReadOnly = true, StorageType = StorageTypes.Local)]
        public string CurrentNomenclature
            {
            get
                {
                return z_CurrentNomenclature;
                }
            set
                {
                if (z_CurrentNomenclature == value)
                    {
                    return;
                    }

                z_CurrentNomenclature = value;
                NotifyPropertyChanged("CurrentNomenclature");
                }
            }
        private string z_CurrentNomenclature = "";
        #endregion

        #region (string) SelectedCellValue Содержимое текущей ячейки
        [DataField(Description = "Содержимое текущей ячейки", StorageType = StorageTypes.Local, Size = 300)]
        public string SelectedCellValue
            {
            get
                {
                return z_SelectedCellValue;
                }
            set
                {
                if (z_SelectedCellValue == value)
                    {
                    return;
                    }

                z_SelectedCellValue = value;
                NotifyPropertyChanged("SelectedCellValue");
                }
            }
        private string z_SelectedCellValue = "";
        #endregion

        #region (string) NumberOfDelivery Номер поставки
        [DataField(Description = "Номер поставки", ShowInList = true)]
        public string NumberOfDelivery
            {
            get
                {
                return z_NumberOfDelivery;
                }
            set
                {
                if (z_NumberOfDelivery == value)
                    {
                    return;
                    }

                z_NumberOfDelivery = value;
                NotifyPropertyChanged("NumberOfDelivery");
                }
            }
        private string z_NumberOfDelivery = string.Empty;
        #endregion

        #endregion

        #region Табличная часть Товары

        [Table(Columns = "InvoiceNumber,InvoiceDate,OriginalName,CustomsCodeExtern,Country,Count,Price,MarginPrecentage,Margin,PriceWithMargin,Sum,UnitWeight,NetWeight,ItemGrossWeight,ItemNumberOfPlaces," +
            "CustomsCodeIntern,Article,NomenclatureDeclaration,NomenclatureInvoice,UnitOfMeasureCode,UnitOfMeasure,ItemTradeMark,ItemContractor,SubGroupOfGoods,GroupOfGoods,GroupCode,BarCode," +
            "Gender,Size,SizeOriginal,InsoleLength,Content,ContentBottom,InvoiceCode,Graf31,RDCode1,RDNumber1,RDFromDate1,RDToDate1,RDCode2,RDNumber2,RDFromDate2," +
            "RDToDate2,RDCode3,RDNumber3,RDFromDate3,RDToDate3,RDCode4,RDNumber4,RDFromDate4,RDToDate4,RDCode5,RDNumber5,RDFromDate5,RDToDate5," +
            "FoundedNomenclature,FoundedSubGroupOfGoods,FoundedApprovals1,FoundedApprovals2,FoundedApprovals3,FoundedApprovals4,FoundedApprovals5,HaveToBeUnloaded," +
            "ZaraContent1Code,ZaraContent1UkrName,ZaraContent1EnName,ZaraContent2Code,ZaraContent2UkrName,ZaraContent2EnName,ZaraContent3Code,ZaraContent3UkrName,ZaraContent3EnName," +
            "ZaraContent4Code,ZaraContent4UkrName,ZaraContent4EnName,ZaraContent5Code,ZaraContent5UkrName,ZaraContent5EnName,ZaraContent6Code,ZaraContent6UkrName,ZaraContent6EnName," +
            "ZaraContent7Code,ZaraContent7UkrName,ZaraContent7EnName,ZaraContent8Code,ZaraContent8UkrName,ZaraContent8EnName,ZaraContent9Code,ZaraContent9UkrName,ZaraContent9EnName," +
            "ZaraContent10Code,ZaraContent10UkrName,ZaraContent10EnName,ZaraContent11Code,ZaraContent11UkrName,ZaraContent11EnName,ZaraContent12Code,ZaraContent12UkrName,ZaraContent12EnName," +
            "ZaraContent13Code,ZaraContent13UkrName,ZaraContent13EnName,ZaraContent14Code,ZaraContent14UkrName,ZaraContent14EnName,ZaraContent15Code,ZaraContent15UkrName,ZaraContent15EnName," +
            "MSKnitWovenColumnName, ShowRow,BNSInvoicePart,Graf31FilterColumn,TempFilterColumn", AllowFiltering = true, AllowPopUpMenu = false, AllowCopyRow = false, ConfirmRowDeletion = true)]
        [DataField(Description = "Товары")]
        public DataTable Goods
            {
            get
                {
                return GetSubtable("Goods");
                }
            }

        [SubTableField(Description = "Подгруппа товара", PropertyType = typeof(string), Size = 40, AllowGroup = true)]
        public DataColumn SubGroupOfGoods
            {
            get;
            set;
            }

        [SubTableField(Description = "Группа товара", PropertyType = typeof(string), Size = 40, AllowGroup = true)]
        public DataColumn GroupOfGoods
            {
            get;
            set;
            }

        [SubTableField(Description = "Код подгруппы", PropertyType = typeof(string), Size = 20, AllowGroup = true)]
        public DataColumn GroupCode
            {
            get;
            set;
            }

        [SubTableField(Description = "Товар(Декларация)", PropertyType = typeof(string), Size = 600, AllowGroup = true, StorageType = StorageTypes.Local)]
        public DataColumn NomenclatureDeclaration
            {
            get;
            set;
            }

        [SubTableField(Description = "Наименование (исходное)", PropertyType = typeof(string), Size = 150, AllowGroup = true)]
        public DataColumn OriginalName
            {
            get;
            set;
            }

        [SubTableField(Description = "Торговая марка", PropertyType = typeof(string), Size = 30)]
        public DataColumn ItemTradeMark
            {
            get;
            set;
            }

        [SubTableField(Description = "Производитель", PropertyType = typeof(string), Size = 110)]
        public DataColumn ItemContractor
            {
            get;
            set;
            }

        [SubTableField(Description = "Штрих-код", PropertyType = typeof(string), Size = 20)]
        public DataColumn BarCode
            {
            get;
            set;
            }

        [SubTableField(Description = "Количество", PropertyType = typeof(string), Size = 10)]
        public DataColumn Count
            {
            get;
            set;
            }

        [SubTableField(Description = "Цена", PropertyType = typeof(string), Size = 10)]
        public DataColumn Price
            {
            get;
            set;
            }

        [SubTableField(Description = "Наценка", PropertyType = typeof(string), Size = 10)]
        public DataColumn Margin
            {
            get;
            set;
            }

        [SubTableField(Description = "Цена с наценкой", PropertyType = typeof(string), Size = 10)]
        public DataColumn PriceWithMargin
            {
            get;
            set;
            }

        [SubTableField(Description = "% Наценки", PropertyType = typeof(string), Size = 10)]
        public DataColumn MarginPrecentage
            {
            get;
            set;
            }

        [SubTableField(Description = "Сумма", PropertyType = typeof(string), Size = 10)]
        public DataColumn Sum
            {
            get;
            set;
            }

        [SubTableField(Description = "Вес ед. товара", PropertyType = typeof(string), Size = 20)]
        public DataColumn UnitWeight
            {
            get;
            set;
            }

        [SubTableField(Description = "Внешний ТК", PropertyType = typeof(string), Size = 10)]
        public DataColumn CustomsCodeExtern
            {
            get;
            set;
            }

        [SubTableField(Description = "УКТЗЕД", PropertyType = typeof(string), Size = 10)]
        public DataColumn CustomsCodeIntern
            {
            get;
            set;
            }

        [SubTableField(Description = "Артикул", PropertyType = typeof(string), Size = 20)]
        public DataColumn Article
            {
            get;
            set;
            }

        [SubTableField(Description = "Вес нетто", PropertyType = typeof(string), Size = 10)]
        public DataColumn NetWeight
            {
            get;
            set;
            }

        [SubTableField(Description = "Вес брутто", PropertyType = typeof(string), Size = 10)]
        public DataColumn ItemGrossWeight
            {
            get;
            set;
            }

        [SubTableField(Description = "Пол", PropertyType = typeof(string), Size = 25)]
        public DataColumn Gender
            {
            get;
            set;
            }

        [SubTableField(Description = "Места", PropertyType = typeof(string), Size = 10)]
        public DataColumn ItemNumberOfPlaces
            {
            get;
            set;
            }

        [SubTableField(Description = "Ед.(Код)", PropertyType = typeof(string), Size = 10)]
        public DataColumn UnitOfMeasureCode
            {
            get;
            set;
            }

        [SubTableField(Description = "Ед. изм.", PropertyType = typeof(string), Size = 10)]
        public DataColumn UnitOfMeasure
            {
            get;
            set;
            }

        [SubTableField(Description = "Страна", PropertyType = typeof(string), Size = 5)]
        public DataColumn Country
            {
            get;
            set;
            }

        [SubTableField(Description = "Размер", PropertyType = typeof(string), Size = 10)]
        public DataColumn Size
            {
            get;
            set;
            }
        
        [SubTableField(Description = "Размер исходный", PropertyType = typeof(string), Size = 10)]
        public DataColumn SizeOriginal
            {
            get;
            set;
            }

        [SubTableField(Description = "Длинна стельки", PropertyType = typeof(string), Size = 10)]
        public DataColumn InsoleLength
            {
            get;
            set;
            }

        [SubTableField(Description = "Состав", PropertyType = typeof(string), Size = 100)]
        public DataColumn Content
            {
            get;
            set;
            }


        [SubTableField(Description = "Состав низ", PropertyType = typeof(string), Size = 100)]
        public DataColumn ContentBottom
            {
            get;
            set;
            }

        [SubTableField(Description = "Код инвойса", PropertyType = typeof(string), Size = 12)]
        public DataColumn InvoiceCode
            {
            get;
            set;
            }

        [SubTableField(Description = "№ инвойса", PropertyType = typeof(string), Size = MAX_INVOICE_NUMBER_SIZE + 1)]
        public DataColumn InvoiceNumber
            {
            get;
            set;
            }

        [SubTableField(Description = "Дата инвойса", PropertyType = typeof(string), Size = 20)]
        public DataColumn InvoiceDate
            {
            get;
            set;
            }

        [SubTableField(Description = "Товар (инвойс)", PropertyType = typeof(string), Size = 400, StorageType = StorageTypes.Local)]
        public DataColumn NomenclatureInvoice
            {
            get;
            set;
            }

        [SubTableField(Description = "Графа 31", PropertyType = typeof(string), Size = 700, StorageType = StorageTypes.Local)]
        public DataColumn Graf31
            {
            get;
            set;
            }

        [SubTableField(Description = "РД код", PropertyType = typeof(string), Size = 6)]
        public DataColumn RDCode1
            {
            get;
            set;
            }

        [SubTableField(Description = "РД №", PropertyType = typeof(string), Size = 20)]
        public DataColumn RDNumber1
            {
            get;
            set;
            }

        [SubTableField(Description = "РД Дата выдачи", PropertyType = typeof(string), Size = 16)]
        public DataColumn RDFromDate1
            {
            get;
            set;
            }

        [SubTableField(Description = "РД Годен до", PropertyType = typeof(string), Size = 16)]
        public DataColumn RDToDate1
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 2 код", PropertyType = typeof(string), Size = 6)]
        public DataColumn RDCode2
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 2 №", PropertyType = typeof(string), Size = 20)]
        public DataColumn RDNumber2
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 2 Дата выдачи", PropertyType = typeof(string), Size = 16)]
        public DataColumn RDFromDate2
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 2 Годен до", PropertyType = typeof(string), Size = 16)]
        public DataColumn RDToDate2
            {
            get;
            set;
            }


        [SubTableField(Description = "РД 3 код", PropertyType = typeof(string), Size = 6)]
        public DataColumn RDCode3
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 3 №", PropertyType = typeof(string), Size = 20)]
        public DataColumn RDNumber3
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 3 Дата выдачи", PropertyType = typeof(string), Size = 16)]
        public DataColumn RDFromDate3
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 3 Годен до", PropertyType = typeof(string), Size = 16)]
        public DataColumn RDToDate3
            {
            get;
            set;
            }


        [SubTableField(Description = "РД 4 код", PropertyType = typeof(string), Size = 6)]
        public DataColumn RDCode4
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 4 №", PropertyType = typeof(string), Size = 20)]
        public DataColumn RDNumber4
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 4 Дата выдачи", PropertyType = typeof(string), Size = 16)]
        public DataColumn RDFromDate4
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 4 Годен до", PropertyType = typeof(string), Size = 16)]
        public DataColumn RDToDate4
            {
            get;
            set;
            }



        [SubTableField(Description = "РД 5 код", PropertyType = typeof(string), Size = 6)]
        public DataColumn RDCode5
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 5 №", PropertyType = typeof(string), Size = 20)]
        public DataColumn RDNumber5
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 5 Дата выдачи", PropertyType = typeof(string), Size = 16)]
        public DataColumn RDFromDate5
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 5 Годен до", PropertyType = typeof(string), Size = 16)]
        public DataColumn RDToDate5
            {
            get;
            set;
            }
        #region Колонки используемые для хранения найденых элементов документов/справочников
        [SubTableField(Description = "FoundedNomenclature", PropertyType = typeof(Nomenclature), ShowInForm = false)]
        public DataColumn FoundedNomenclature
            {
            get;
            set;
            }

        [SubTableField(Description = "FoundedSubGroupOfGoods", PropertyType = typeof(SubGroupOfGoods), ShowInForm = false)]
        public DataColumn FoundedSubGroupOfGoods
            {
            get;
            set;
            }

        [SubTableField(Description = "FoundedApprovals1", PropertyType = typeof(Approvals), ShowInForm = false)]
        public DataColumn FoundedApprovals1
            {
            get;
            set;
            }

        [SubTableField(Description = "FoundedApprovals2", PropertyType = typeof(Approvals), ShowInForm = false)]
        public DataColumn FoundedApprovals2
            {
            get;
            set;
            }

        [SubTableField(Description = "FoundedApprovals3", PropertyType = typeof(Approvals), ShowInForm = false)]
        public DataColumn FoundedApprovals3
            {
            get;
            set;
            }

        [SubTableField(Description = "FoundedApprovals4", PropertyType = typeof(Approvals), ShowInForm = false)]
        public DataColumn FoundedApprovals4
            {
            get;
            set;
            }

        [SubTableField(Description = "FoundedApprovals5", PropertyType = typeof(Approvals), ShowInForm = false)]
        public DataColumn FoundedApprovals5
            {
            get;
            set;
            }
        #endregion


        //Колонки используемые для проверки состава в Заре


        [SubTableField(Description = "Зара состав 1 код", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent1Code
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 1 имя укр.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent1UkrName
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 1 имя англ.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent1EnName
            {
            get;
            set;
            }


        [SubTableField(Description = "Зара состав 2 код", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent2Code
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 2 имя укр.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent2UkrName
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 2 имя англ.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent2EnName
            {
            get;
            set;
            }


        [SubTableField(Description = "Зара состав 3 код", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent3Code
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 3 имя укр.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent3UkrName
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 3 имя англ.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent3EnName
            {
            get;
            set;
            }


        [SubTableField(Description = "Зара состав 4 код", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent4Code
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 4 имя укр.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent4UkrName
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 4 имя англ.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent4EnName
            {
            get;
            set;
            }



        [SubTableField(Description = "Зара состав 5 код", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent5Code
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 5 имя укр.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent5UkrName
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 5 имя англ.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent5EnName
            {
            get;
            set;
            }


        [SubTableField(Description = "Зара состав 6 код", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent6Code
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 6 имя укр.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent6UkrName
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 6 имя англ.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent6EnName
            {
            get;
            set;
            }


        [SubTableField(Description = "Зара состав 7 код", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent7Code
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 7 имя укр.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent7UkrName
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 7 имя англ.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent7EnName
            {
            get;
            set;
            }


        [SubTableField(Description = "Зара состав 8 код", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent8Code
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 8 имя укр.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent8UkrName
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 8 имя англ.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent8EnName
            {
            get;
            set;
            }


        [SubTableField(Description = "Зара состав 9 код", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent9Code
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 9 имя укр.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent9UkrName
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 9 имя англ.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent9EnName
            {
            get;
            set;
            }


        [SubTableField(Description = "Зара состав 10 код", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent10Code
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 10 имя укр.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent10UkrName
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 10 имя англ.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent10EnName
            {
            get;
            set;
            }


        [SubTableField(Description = "Зара состав 11 код", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent11Code
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 11 имя укр.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent11UkrName
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 11 имя англ.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent11EnName
            {
            get;
            set;
            }


        [SubTableField(Description = "Зара состав 12 код", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent12Code
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 12 имя укр.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent12UkrName
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 12 имя англ.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent12EnName
            {
            get;
            set;
            }


        [SubTableField(Description = "Зара состав 13 код", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent13Code
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 13 имя укр.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent13UkrName
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 13 имя англ.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent13EnName
            {
            get;
            set;
            }


        [SubTableField(Description = "Зара состав 14 код", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent14Code
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 14 имя укр.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent14UkrName
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 14 имя англ.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent14EnName
            {
            get;
            set;
            }


        [SubTableField(Description = "Зара состав 15 код", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent15Code
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 15 имя укр.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent15UkrName
            {
            get;
            set;
            }

        [SubTableField(Description = "Зара состав 15 имя англ.", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn ZaraContent15EnName
            {
            get;
            set;
            }

        [SubTableField(Description = "Маркс энд спенсер (Knit/Woven)", PropertyType = typeof(string), ShowInForm = true, StorageType = StorageTypes.Local)]
        public DataColumn MSKnitWovenColumnName
            {
            get;
            set;
            }

        [SubTableField(Description = "HaveToBeUnloaded", PropertyType = typeof(int), ShowInForm = false, StorageType = StorageTypes.Local)]
        public DataColumn HaveToBeUnloaded
            {
            get;
            set;
            }

        [SubTableField(Description = "ShowRow", PropertyType = typeof(int), ShowInForm = false, StorageType = StorageTypes.Local)]
        public DataColumn ShowRow
            {
            get;
            set;
            }


        [SubTableField(Description = "BNS Номер поставки", PropertyType = typeof(string), ShowInForm = false, StorageType = StorageTypes.Local)]
        public DataColumn BNSInvoicePart
            {
            get;
            set;
            }

        [SubTableField(Description = "ФильтрГрафы31", PropertyType = typeof(string), ShowInForm = false)]
        public DataColumn Graf31FilterColumn
            {
            get;
            set;
            }

        [SubTableField(Description = "Фильтр", PropertyType = typeof(int), ShowInForm = false, StorageType = StorageTypes.Local)]
        public DataColumn TempFilterColumn
            {
            get;
            set;
            }

        //[SubTableField( Description = "Короткий состав(BNS)", PropertyType = typeof( string ), ShowInForm = false, Size = 25 )]
        //public DataColumn ShortContent
        //    {
        //    get;
        //    set;
        //    }

        #endregion

        #region Табличная часть Товары(часть полей) (для хранения части данных которые не помещаются в табличной части Товары)

        [Table(Columns = "Graf31Saved,NameDeclSaved,NameInvSaved", AllowFiltering = true, AllowPopUpMenu = false)]
        [DataField(Description = "Товары(часть полей)")]
        public DataTable GoodsPartialTable
            {
            get
                {
                return GetSubtable("GoodsPartialTable");
                }
            }

        [SubTableField(Description = "Графа31", PropertyType = typeof(string), Size = 700, AllowGroup = true)]
        public DataColumn Graf31Saved
            {
            get;
            set;
            }


        [SubTableField(Description = "Наим. Инвойс", PropertyType = typeof(string), Size = 600, AllowGroup = true)]
        public DataColumn NameDeclSaved
            {
            get;
            set;
            }

        [SubTableField(Description = "Наим. Декл", PropertyType = typeof(string), Size = 300, AllowGroup = true)]
        public DataColumn NameInvSaved
            {
            get;
            set;
            }

        #endregion

        public Invoice()
            {
            this.syncronizer = new TradeMarkContractorExcelLoadingFormatSyncronizer(this);
            this.ValueOfObjectPropertyChanged += Invoice_ValueOfObjectPropertyChanged;
            this.OnRead += Invoice_OnRead;
            this.BeforeWriting += Invoice_BeforeWriting;

            }

        #region Синхронизация локальных колонок в таблице инвойса с колонками таблицы для хранения не помещающихся колонок

        bool haveToBeWritten = false;

        /// <summary>
        /// Указывает на то, что необходимо вызвать уведомление об изменении таблицы из главной формы
        /// </summary>
        public bool HaveToBeWritten
            {
            get { return haveToBeWritten; }
            }

        void Invoice_BeforeWriting(DatabaseObject item, ref bool cancel)
            {
            saveFromMainToPartialTable();
            if (checkTableForAllowedSize())
                {
                this.Responsible = SystemAramis.CurrentUser;
                }
            else
                {
                cancel = true;
                }
            }

        private void saveFromMainToPartialTable()
            {
            GoodsPartialTable.Rows.Clear();
            foreach (DataRow row in Goods.Rows)
                {
                long lineNumber = row.TrySafeGetColumnValue<long>("LineNumber", 0);
                string declarationNmae = row.TrySafeGetColumnValue<string>("NomenclatureDeclaration", string.Empty);
                string invoiceName = row.TrySafeGetColumnValue<string>("NomenclatureInvoice", string.Empty);
                string graf31 = row[Graf31].ToString();
                // string nameDecl
                DataRow newRow = GoodsPartialTable.NewRow();
                newRow["LineNumber"] = lineNumber;
                newRow[Graf31Saved] = graf31;
                newRow[NameDeclSaved] = declarationNmae;
                newRow[NameInvSaved] = invoiceName;
                GoodsPartialTable.Rows.Add(newRow);
                }
            }

        protected override void InitItemBeforeShowing()
            {
            //            DateTime from = DateTime.Now;
            if (Goods.Rows.Count > GoodsPartialTable.Rows.Count)
                {
                haveToBeWritten = true;
                saveFromMainToPartialTable();
                }
            else
                {
                loadFromPartialToMainTable();
                }
            //            DateTime to = DateTime.Now;
            //            double duration = (to - from).TotalMilliseconds;
            //            Console.WriteLine("Duration: {0}", duration);
            base.InitItemBeforeShowing();
            }

        private void loadFromPartialToMainTable()
            {
            Dictionary<long, DataRow> rowsDict = getSavedByLineNumbersRowsDict();
            loadToMainTableLocalColumns(rowsDict);
            }

        private void loadToMainTableLocalColumns(Dictionary<long, DataRow> rowsDict)
            {
            foreach (DataRow dataRow in Goods.Rows)
                {
                long lineNumber = dataRow.TrySafeGetColumnValue<long>("LineNumber", 0);
                DataRow updateByRow = null;
                if (rowsDict.TryGetValue(lineNumber, out updateByRow))
                    {
                    string graf31 = updateByRow[Graf31Saved].ToString().Trim();
                    string oldGraf31 = dataRow[Graf31].ToString().Trim();
                    string invoiceNameSaved = updateByRow[NameInvSaved].ToString().Trim();
                    string nameDeclSaved = updateByRow[NameDeclSaved].ToString().Trim();
                    if (!graf31.Equals(oldGraf31))
                        {
                        haveToBeWritten = true;
                        dataRow[Graf31] = graf31;
                        }
                    if (string.IsNullOrEmpty(invoiceNameSaved))
                        {
                        invoiceNameSaved = " ";
                        }
                    if (string.IsNullOrEmpty(nameDeclSaved))
                        {
                        nameDeclSaved = " ";
                        }
                    dataRow[NomenclatureInvoice] = invoiceNameSaved;
                    dataRow[NomenclatureDeclaration] = nameDeclSaved;
                    }
                }
            }

        private Dictionary<long, DataRow> getSavedByLineNumbersRowsDict()
            {
            Dictionary<long, DataRow> rowsDict = new Dictionary<long, DataRow>();
            foreach (DataRow dataRow in GoodsPartialTable.Rows)
                {
                long lineNumber = dataRow.TrySafeGetColumnValue<long>("LineNumber", 0);
                if (!rowsDict.ContainsKey(lineNumber))
                    {
                    rowsDict.Add(lineNumber, dataRow);
                    }
                }
            return rowsDict;
            }
        #endregion

        void Invoice_OnRead()
            {
            syncronizer.RefreshAll();
            initExcelLoadFormat();
            }

        void Invoice_ValueOfObjectPropertyChanged(string propertyName)
            {
            if (propertyName.Equals("TradeMark") || propertyName.Equals("Contractor"))
                {
                initExcelLoadFormat();
                }
            }

        /// <summary>
        /// Выбирает формат загрузки с текущими значениями контрагента/торговой марки, если таковой существует в единственном числе
        /// </summary>
        private void initExcelLoadFormat()
            {
            if (this.Contractor.Id == 0 || this.ExcelLoadingFormat.Id != 0)
                {
                return;
                }
            string queryText = @"select max(ID) as id from ExcelLoadingFormat where TradeMark = @tradeMark and Contractor = @contractor having COUNT(*) = 1;";
            Query query = DB.NewQuery(queryText);
            query.AddInputParameter("contractor", this.Contractor.Id);
            query.AddInputParameter("tradeMark", this.TradeMark.Id);
            object value = query.SelectScalar();
            if (value != null && value != DBNull.Value)
                {
                long id = (long)value;
                IContractor contractorForApprovals = A.New<IContractor>();
                contractorForApprovals.Id = this.Contractor.Id;
                ITradeMark tradeMarkForApprovals = A.New<ITradeMark>();
                tradeMarkForApprovals.Contractor = contractorForApprovals;
                tradeMarkForApprovals.Id = this.TradeMark.Id;
                ExcelLoadingFormat loadFormat = new ExcelLoadingFormat();
                loadFormat.Id = id;
                loadFormat.Contractor = contractorForApprovals;
                loadFormat.TradeMark = tradeMarkForApprovals;
                loadFormat.Read();
                this.ExcelLoadingFormat = loadFormat;
                }
            }

        public override GetListFilterDelegate GetFuncGetCustomFilter(string propertyName)
            {
            return syncronizer.GetFuncGetCustomFilter(propertyName);
            }

        private bool checkTableForAllowedSize()
            {
            Dictionary<string, int> allowedSize = getAllowedColumnsSizes();
            Dictionary<DataColumn, int> currentSizes = getCurrentColumnsSizes();
            List<ErrorInfo> errors = getErrors(allowedSize, currentSizes);
            if (errors.Count == 0)
                {
                return true;
                }
            string errorMessage = "Найдены следующие ошибки в табличной части инвойса:" + Environment.NewLine +
                string.Concat(errors.Select(error => string.Format(@"Максимально допустимый размер текстового поля колонки ""{0}"" -  {1}, в то время как максимальное значение в таблице - {2}.{3}",
                    InvoiceColumnNames[error.ColumnName], error.AllowedSize, error.CurrentSize, Environment.NewLine))) + Environment.NewLine + "Документ не может быть сохранен.";
            errorMessage.AlertBox();
            return false;
            }

        private static List<ErrorInfo> getErrors(Dictionary<string, int> allowedSize, Dictionary<DataColumn, int> currentSizes)
            {
            List<ErrorInfo> errors = new List<ErrorInfo>();
            foreach (DataColumn column in currentSizes.Keys)
                {
                string columnName = column.ColumnName;
                int colAllowedSize = 0;
                int currentSize = currentSizes[column];
                if (allowedSize.ContainsKey(columnName))
                    {
                    colAllowedSize = allowedSize[columnName];
                    }
                else
                    {
                    continue;
                    }
                if (currentSize > colAllowedSize)
                    {
                    errors.Add(new ErrorInfo(columnName, colAllowedSize, currentSize));
                    }
                }
            return errors;
            }

        private Dictionary<DataColumn, int> getCurrentColumnsSizes()
            {
            Dictionary<DataColumn, int> dict = new Dictionary<DataColumn, int>();
            appendMaxColumnsSizes(this.GoodsPartialTable, dict);
            appendMaxColumnsSizes(this.Goods, dict);
            return dict;
            }

        private static void appendMaxColumnsSizes(DataTable tableToCheck, Dictionary<DataColumn, int> dict)
            {
            int maxSize = 0;
            for (int i = 0; i < tableToCheck.Rows.Count; i++)
                {
                foreach (DataColumn column in tableToCheck.Columns)
                    {
                    string strVal = tableToCheck.Rows[i][column].ToString();
                    int len = strVal.Length;

                    if (!dict.ContainsKey(column))
                        {
                        dict.Add(column, len);
                        }
                    else if (len > dict[column])
                        {
                        dict[column] = strVal.Length;
                        }
                    }
                }
            }

        private static Dictionary<string, int> getAllowedColumnsSizes()
            {
            Dictionary<string, int> allowedSize = new Dictionary<string, int>();
            foreach (var propItem in (typeof(Invoice)).GetProperties())
                {
                if (!propItem.PropertyType.ToString().Equals(typeof(System.Data.DataColumn).ToString()))
                    {
                    continue;
                    }
                var attribute = (Aramis.Attributes.SubTableFieldAttribute)propItem.GetCustomAttributes(typeof(Aramis.Attributes.SubTableFieldAttribute), false).FirstOrDefault();
                if (attribute.StorageType == StorageTypes.Local)
                    {
                    continue;
                    }
                int maxSizeC = attribute.Size;
                allowedSize.Add(propItem.Name, maxSizeC);
                }
            return allowedSize;
            }

        private struct ErrorInfo
            {
            public string ColumnName;
            public int AllowedSize;
            public int CurrentSize;

            public ErrorInfo(string columnName, int allowedSize, int currentSize)
                {
                this.ColumnName = columnName;
                this.AllowedSize = allowedSize;
                this.CurrentSize = currentSize;
                }
            }

        }

    public enum InvoiceColumnNames
        {
        [DataField(Description = "Не заполнено")]
        Empty,
        [DataField(Description = "Подгруппа товара")]
        SubGroupOfGoods,
        [DataField(Description = "Код подгруппы")]
        GroupCode,
        [DataField(Description = "Товар(Декларация)")]
        NomenclatureDeclaration,
        [DataField(Description = "Наименование (исходное)")]
        OriginalName,
        [DataField(Description = "Торговая марка")]
        ItemTradeMark,
        [DataField(Description = "Производитель")]
        ItemContractor,
        [DataField(Description = "Штрих-код")]
        BarCode,
        [DataField(Description = "Кол-во")]
        Count,
        [DataField(Description = "Цена")]
        Price,
        [DataField(Description = "Наценка")]
        Margin,
        [DataField(Description = "Сумма")]
        Sum,
        [DataField(Description = "Вес ед. товара")]
        UnitWeight,
        [DataField(Description = "Таможенный код внешний")]
        CustomsCodeExtern,
        [DataField(Description = "Таможенный код внутренний")]
        CustomsCodeIntern,
        [DataField(Description = "Артикул")]
        Article,
        [DataField(Description = "Вес нетто")]
        NetWeight,
        [DataField(Description = "Вес брутто")]
        ItemGrossWeight,
        [DataField(Description = "Пол")]
        Gender,
        [DataField(Description = "Места")]
        ItemNumberOfPlaces,
        [DataField(Description = "Ед.(Код)")]
        UnitOfMeasureCode,
        [DataField(Description = "Ед. изм.")]
        UnitOfMeasure,
        [DataField(Description = "Страна")]
        Country,
        [DataField(Description = "Размер")]
        Size,
        [DataField(Description = "Длинна стельки")]
        InsoleLength,
        [DataField(Description = "Состав низ")]
        ContentBottom,
        [DataField(Description = "Состав")]
        Content,
        [DataField(Description = "Код инвойса")]
        InvoiceCode,
        [DataField(Description = "№ инвойса")]
        InvoiceNumber,
        [DataField(Description = "Дата инвойса")]
        InvoiceDate,
        [DataField(Description = "Товар (инвойс)")]
        NomenclatureInvoice,
        [DataField(Description = "Графа 31")]
        Graf31,
        [DataField(Description = "РД код")]
        RDCode1,
        [DataField(Description = "РД №")]
        RDNumber1,
        [DataField(Description = "РД Дата выдачи")]
        RDFromDate1,
        [DataField(Description = "РД Годен до")]
        RDToDate1,
        [DataField(Description = "РД 2 код")]
        RDCode2,
        [DataField(Description = "РД 2 №")]
        RDNumber2,
        [DataField(Description = "РД 2 Дата выдачи")]
        RDFromDate2,
        [DataField(Description = "РД 2 Годен до")]
        RDToDate2,
        [DataField(Description = "РД 3 код")]
        RDCode3,
        [DataField(Description = "РД 3 №")]
        RDNumber3,
        [DataField(Description = "РД 3 Дата выдачи")]
        RDFromDate3,
        [DataField(Description = "РД 3 Годен до")]
        RDToDate3,
        [DataField(Description = "РД 4 код")]
        RDCode4,
        [DataField(Description = "РД 4 №")]
        RDNumber4,
        [DataField(Description = "РД 4 Дата выдачи")]
        RDFromDate4,
        [DataField(Description = "РД 4 Годен до")]
        RDToDate4,
        [DataField(Description = "РД 5 код")]
        RDCode5,
        [DataField(Description = "РД 5 №")]
        RDNumber5,
        [DataField(Description = "РД 5 Дата выдачи")]
        RDFromDate5,
        [DataField(Description = "РД 5 Годен до")]
        RDToDate5,

        [DataField(Description = "Код состава (Зара)_1")]
        ZaraContent1Code,
        [DataField(Description = "Имя состава укр. (Зара)_1")]
        ZaraContent1UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_1")]
        ZaraContent1EnName,
        [DataField(Description = "Код состава (Зара)_2")]
        ZaraContent2Code,
        [DataField(Description = "Имя состава укр. (Зара)_2")]
        ZaraContent2UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_2")]
        ZaraContent2EnName,
        [DataField(Description = "Код состава (Зара)_3")]
        ZaraContent3Code,
        [DataField(Description = "Имя состава укр. (Зара)_3")]
        ZaraContent3UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_3")]
        ZaraContent3EnName,
        [DataField(Description = "Код состава (Зара)_4")]
        ZaraContent4Code,
        [DataField(Description = "Имя состава укр. (Зара)_4")]
        ZaraContent4UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_4")]
        ZaraContent4EnName,
        [DataField(Description = "Код состава (Зара)_5")]
        ZaraContent5Code,
        [DataField(Description = "Имя состава укр. (Зара)_5")]
        ZaraContent5UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_5")]
        ZaraContent5EnName,
        [DataField(Description = "Код состава (Зара)_6")]
        ZaraContent6Code,
        [DataField(Description = "Имя состава укр. (Зара)_6")]
        ZaraContent6UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_6")]
        ZaraContent6EnName,
        [DataField(Description = "Код состава (Зара)_7")]
        ZaraContent7Code,
        [DataField(Description = "Имя состава укр. (Зара)_7")]
        ZaraContent7UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_7")]
        ZaraContent7EnName,
        [DataField(Description = "Код состава (Зара)_8")]
        ZaraContent8Code,
        [DataField(Description = "Имя состава укр. (Зара)_8")]
        ZaraContent8UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_8")]
        ZaraContent8EnName,
        [DataField(Description = "Код состава (Зара)_9")]
        ZaraContent9Code,
        [DataField(Description = "Имя состава укр. (Зара)_9")]
        ZaraContent9UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_9")]
        ZaraContent9EnName,
        [DataField(Description = "Код состава (Зара)_10")]
        ZaraContent10Code,
        [DataField(Description = "Имя состава укр. (Зара)_10")]
        ZaraContent10UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_10")]
        ZaraContent10EnName,
        [DataField(Description = "Код состава (Зара)_11")]
        ZaraContent11Code,
        [DataField(Description = "Имя состава укр. (Зара)_11")]
        ZaraContent11UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_11")]
        ZaraContent11EnName,
        [DataField(Description = "Код состава (Зара)_12")]
        ZaraContent12Code,
        [DataField(Description = "Имя состава укр. (Зара)_12")]
        ZaraContent12UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_12")]
        ZaraContent12EnName,
        [DataField(Description = "Код состава (Зара)_13")]
        ZaraContent13Code,
        [DataField(Description = "Имя состава укр. (Зара)_13")]
        ZaraContent13UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_13")]
        ZaraContent13EnName,
        [DataField(Description = "Код состава (Зара)_14")]
        ZaraContent14Code,
        [DataField(Description = "Имя состава укр. (Зара)_14")]
        ZaraContent14UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_14")]
        ZaraContent14EnName,
        [DataField(Description = "Код состава (Зара)_15")]
        ZaraContent15Code,
        [DataField(Description = "Имя состава укр. (Зара)_15")]
        ZaraContent15UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_15")]
        ZaraContent15EnName,

        [DataField(Description = "Состав KnitWoven .(Marks & Spenser)")]
        MSKnitWovenColumnName,
        [DataField(Description = "Группа товара")]
        GroupOfGoods,
        [DataField(Description = "BNS Номер поставки")]
        BNSInvoicePart,
        [DataField(Description = "Цена с наценкой")]
        PriceWithMargin,
        [DataField(Description = "% Наценки")]
        MarginPrecentage,
        [DataField(Description = "Размер исходный")]
        SizeOriginal
        //,
        //[DataField( Description = "Короткий состав(BNS)" )]
        //ShortContent
        //,
        //[DataField( Description = "ФильтрГрафы31" )]
        //Graf31FilterColumn
        }
    }
