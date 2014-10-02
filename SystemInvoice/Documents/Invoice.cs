using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Aramis.Core;
using Aramis.Attributes;
using Aramis.Core.ReflectionUtils;
using Aramis.Enums;
using SystemInvoice.Catalogs;
using System.Data;
using Aramis.DatabaseConnector;
using SystemInvoice.PropsSyncronization;
using Aramis.Platform;
using Aramis.SystemConfigurations;
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
        public static Dictionary<string, string> InvoiceColumnNames = new Dictionary<string, string>();
        public static Dictionary<string, string> InvoiceColumnNamesTranslated = new Dictionary<string, string>();

        static Invoice()
            {
            fillInvoiceColumnNames();
            }
        private static void fillInvoiceColumnNames()
            {
            var itemDescription = EnumReflector.ReflectEnumDescriptions(typeof(Documents.InvoiceColumnNames));
            foreach (var enumInfo in itemDescription)
                {
                InvoiceColumnNames.Add(enumInfo.ItemName, enumInfo.ItemDescription);
                }
            InvoiceColumnNames.Remove(Documents.InvoiceColumnNames.Empty.ToString());
            InvoiceColumnNames.Add("Graf31FilterColumn", "ФильтрГрафы31");

            foreach (KeyValuePair<string, string> pair in InvoiceColumnNames)
                {
                InvoiceColumnNamesTranslated.Add(pair.Value, pair.Key);
                }
            }

        private TradeMarkContractorExcelLoadingFormatSyncronizer syncronizer = null;

        public const string CheckingRowNameToUnload = "HaveToBeUnloaded";
        public const string GrafColumnReplaceString = "A";
        public const int MAX_INVOICE_NUMBER_SIZE = 47;
        public const string FILTER_COLUMN_NAME = "TempFilterColumn";

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
            "MSKnitWovenColumnName, ShowRow,BNSInvoicePart,Graf31FilterColumn,TempFilterColumn,Model,RD1BaseNumber,RD2BaseNumber,RD3BaseNumber,RD4BaseNumber,RD5BaseNumber,OneItemGross", AllowFiltering = true, AllowPopUpMenu = false, AllowCopyRow = false, ConfirmRowDeletion = true)]
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

        [SubTableField(Description = "РД №", PropertyType = typeof(string), Size = Approvals.NUMBER_MAX_LENGTH, StorageType = StorageTypes.Local)]
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

        [SubTableField(Description = "РД 2 №", PropertyType = typeof(string), Size = Approvals.NUMBER_MAX_LENGTH, StorageType = StorageTypes.Local)]
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

        [SubTableField(Description = "РД 3 №", PropertyType = typeof(string), Size = Approvals.NUMBER_MAX_LENGTH, StorageType = StorageTypes.Local)]
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

        [SubTableField(Description = "РД 4 №", PropertyType = typeof(string), Size = Approvals.NUMBER_MAX_LENGTH, StorageType = StorageTypes.Local)]
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

        [SubTableField(Description = "РД 5 №", PropertyType = typeof(string), Size = Approvals.NUMBER_MAX_LENGTH, StorageType = StorageTypes.Local)]
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

        [SubTableField(Description = "Модель", PropertyType = typeof(string), Size = Nomenclature.MODEL_SIZE)]
        public DataColumn Model
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 1 основание", PropertyType = typeof(string), Size = Approvals.NUMBER_MAX_LENGTH)]
        public DataColumn RD1BaseNumber
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 2 основание", PropertyType = typeof(string), Size = Approvals.NUMBER_MAX_LENGTH)]
        public DataColumn RD2BaseNumber
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 3 основание", PropertyType = typeof(string), Size = Approvals.NUMBER_MAX_LENGTH)]
        public DataColumn RD3BaseNumber
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 4 основание", PropertyType = typeof(string), Size = Approvals.NUMBER_MAX_LENGTH)]
        public DataColumn RD4BaseNumber
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 5 основание", PropertyType = typeof(string), Size = Approvals.NUMBER_MAX_LENGTH)]
        public DataColumn RD5BaseNumber
            {
            get;
            set;
            }

        [SubTableField(Description = "Брутто един.", PropertyType = typeof(string), Size = 10)]
        public DataColumn OneItemGross
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

        #region Табличная часть Товары(часть полей) (для хранения части данных которые не помещаются в табличной части Товары max=4026 (max 8061 bytes including 7 bytes of internal overhead))

        [Table(Columns = "Graf31Saved,NameDeclSaved,NameInvSaved,StoringRDNumber1,StoringRDNumber2,StoringRDNumber3,StoringRDNumber4,StoringRDNumber5", AllowFiltering = true, AllowPopUpMenu = false)]
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

        [SubTableField(Description = "РД 1 №", PropertyType = typeof(string), Size = Approvals.NUMBER_MAX_LENGTH)]
        public DataColumn StoringRDNumber1
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 2 №", PropertyType = typeof(string), Size = Approvals.NUMBER_MAX_LENGTH)]
        public DataColumn StoringRDNumber2
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 3 №", PropertyType = typeof(string), Size = Approvals.NUMBER_MAX_LENGTH)]
        public DataColumn StoringRDNumber3
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 4 №", PropertyType = typeof(string), Size = Approvals.NUMBER_MAX_LENGTH)]
        public DataColumn StoringRDNumber4
            {
            get;
            set;
            }

        [SubTableField(Description = "РД 5 №", PropertyType = typeof(string), Size = Approvals.NUMBER_MAX_LENGTH)]
        public DataColumn StoringRDNumber5
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
            //var totalSize = 0;
            //foreach (var kvp in this.SystemObjInfo.InfoSubTables["Goods"].SubtableFields)
            //    {
            //    var fieldName = kvp.Key;
            //    var fieldInfo = kvp.Value;
            //    if (fieldInfo.Attr.StorageType == StorageTypes.Local) continue;

            //    if (fieldInfo.PropertyType == typeof(string))
            //        {
            //        totalSize += fieldInfo.Attr.Size;
            //        }
            //    else
            //        {
            //        Trace.WriteLine(fieldInfo.PropertyType);
            //        }
            //    }

            //Trace.WriteLine(string.Format("Total row size: {0}", totalSize));
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
                newRow[StoringRDNumber1] = row[RDNumber1];
                newRow[StoringRDNumber2] = row[RDNumber2];
                newRow[StoringRDNumber3] = row[RDNumber3];
                newRow[StoringRDNumber4] = row[RDNumber4];
                newRow[StoringRDNumber5] = row[RDNumber5];
                GoodsPartialTable.Rows.Add(newRow);
                }
            }

        protected override void InitItemBeforeShowing()
            {
            if (Goods.Rows.Count > GoodsPartialTable.Rows.Count)
                {
                haveToBeWritten = true;
                saveFromMainToPartialTable();
                }
            else
                {
                loadFromPartialToMainTable();
                }
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

                    dataRow[RDNumber1] = updateByRow[StoringRDNumber1];
                    dataRow[RDNumber2] = updateByRow[StoringRDNumber2];
                    dataRow[RDNumber3] = updateByRow[StoringRDNumber3];
                    dataRow[RDNumber4] = updateByRow[StoringRDNumber4];
                    dataRow[RDNumber5] = updateByRow[StoringRDNumber5];
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
    }
