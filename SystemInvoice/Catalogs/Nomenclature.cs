using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.ApprovalsProcessing;
using Aramis.Core;
using Aramis.Attributes;
using Aramis.Enums;
using System.Data;
using Aramis.DatabaseConnector;
using SystemInvoice.PropsSyncronization;

namespace SystemInvoice.Catalogs
    {
    /// <summary>
    /// Справочник. Содержит в себе описание товара и его состав 
    /// </summary>
    [Catalog(Description = "Номенклатура", GUID = "AC3B7088-DDB8-49D7-9C7F-1063ACEF0619", DescriptionSize = 400, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false)]
    public class Nomenclature : CatalogTable, IAllFieldsSource
        {
        private AllFieldsSyncronizer syncronizer = null;
        private ApprovalsByNomenclatureUpdater approvalsByNomenclatureUpdater = null;
        private long initCustomsCodeId = 0;

        #region Свойства

        #region (string) Article Артикул
        [DataField(Description = "Артикул", Size = 100, UseForFastInput = UseFieldForFastInput.LoadButNotDisplay, NotEmpty = true, ShowInList = true)]
        public string Article
            {
            get
                {
                return z_Article;
                }
            set
                {
                if (z_Article == value)
                    {
                    return;
                    }

                z_Article = value;
                NotifyPropertyChanged("Article");
                }
            }
        private string z_Article = "";
        #endregion

        #region (CustomsCode) UKTZCod Код УКТЗЕД
        [DataField(Description = "Код УКТЗЕД", UseForFastInput = UseFieldForFastInput.LoadAndDisplay, AllowOpenItem = true, NotEmpty = true, ShowInList = true)]
        public CustomsCode CustomsCodeInternal
            {
            get
                {
                return (CustomsCode)GetValueForObjectProperty("CustomsCodeInternal");
                }
            set
                {
                SetValueForObjectProperty("CustomsCodeInternal", value);
                }
            }
        #endregion

        #region (string) CustomsCodeExtern Таможенный код внешний
        [DataField(Description = "Таможенный код внешний", Size = 20, UseForFastInput = UseFieldForFastInput.LoadAndDisplay, ShowInList = true)]
        public string CustomsCodeExtern
            {
            get
                {
                return z_CustomsCodeExtern;
                }
            set
                {
                if (z_CustomsCodeExtern == value)
                    {
                    return;
                    }

                z_CustomsCodeExtern = value;
                NotifyPropertyChanged("CustomsCodeExtern");
                }
            }
        private string z_CustomsCodeExtern = "";
        #endregion

        #region (string) BarCode Штрих-Код
        [DataField(Description = "Штрих-Код", Size = 20)]
        public string BarCode
            {
            get
                {
                return z_BarCode;
                }
            set
                {
                if (z_BarCode == value)
                    {
                    return;
                    }

                z_BarCode = value;
                NotifyPropertyChanged("BarCode");
                }
            }
        private string z_BarCode = "";
        #endregion

        #region (Contractor) Contractor Производитель
        [DataField(Description = "Контрагент", UseForFastInput = UseFieldForFastInput.LoadButNotDisplay, ShowInList = true, NotEmpty = true)]
        public Contractor Contractor
            {
            get
                {
                return (Contractor)GetValueForObjectProperty("Contractor");
                }
            set
                {
                SetValueForObjectProperty("Contractor", value);
                }
            }
        #endregion

        #region (Manufacturer) Manufacturer Производитель
        [DataField(Description = "Производитель", UseForFastInput = UseFieldForFastInput.LoadButNotDisplay, NotEmpty = true)]
        public Manufacturer Manufacturer
            {
            get
                {
                return (Manufacturer)GetValueForObjectProperty("Manufacturer");
                }
            set
                {
                SetValueForObjectProperty("Manufacturer", value);
                }
            }
        #endregion

        #region (TradeMark) TradeMark Торговая марка
        [DataField(Description = "Торговая марка", UseForFastInput = UseFieldForFastInput.LoadButNotDisplay, NotEmpty = true)]
        public TradeMark TradeMark
            {
            get
                {
                return (TradeMark)GetValueForObjectProperty("TradeMark");
                }
            set
                {
                SetValueForObjectProperty("TradeMark", value);
                }
            }
        #endregion

        #region (Country) Country Страна происхождения
        [DataField(Description = "Страна происхождения", UseForFastInput = UseFieldForFastInput.LoadButNotDisplay, ShowInList = true)]
        public Country Country
            {
            get
                {
                return (Country)GetValueForObjectProperty("Country");
                }
            set
                {
                SetValueForObjectProperty("Country", value);
                }
            }
        #endregion

        #region (SubGroupOfGoods) SubGroupOfGoods Группа товара
        [DataField(Description = "Подгруппа товара", UseForFastInput = UseFieldForFastInput.LoadButNotDisplay)]
        public SubGroupOfGoods SubGroupOfGoods
            {
            get
                {
                return (SubGroupOfGoods)GetValueForObjectProperty("SubGroupOfGoods");
                }
            set
                {
                SetValueForObjectProperty("SubGroupOfGoods", value);
                }
            }
        #endregion

        #region (string) GroupOfGoods Группа товара
        [DataField(Description = "Группа товара", StorageType = StorageTypes.Local, ReadOnly = true)]
        public string GroupOfGoods
            {
            get
                {
                return z_GroupOfGoods;
                }
            set
                {
                if (z_GroupOfGoods == value)
                    {
                    return;
                    }
                z_GroupOfGoods = value;
                NotifyPropertyChanged("GroupOfGoods");
                }
            }
        private string z_GroupOfGoods;
        #endregion

        #region (UnitOfMeasure) UnitOfMeasure Единица измерения
        [DataField(Description = "Единица измерения", UseForFastInput = UseFieldForFastInput.LoadButNotDisplay)]
        public UnitOfMeasure UnitOfMeasure
            {
            get
                {
                return (UnitOfMeasure)GetValueForObjectProperty("UnitOfMeasure");
                }
            set
                {
                SetValueForObjectProperty("UnitOfMeasure", value);
                }
            }
        #endregion

        #region (double) NetWeightFrom Вес нетто от
        [DataField(Description = "Вес нетто от", DecimalPointsNumber = 3)]
        public double NetWeightFrom
            {
            get
                {
                return z_NetWeightFrom;
                }
            set
                {
                if (z_NetWeightFrom == value)
                    {
                    return;
                    }

                z_NetWeightFrom = value;
                NotifyPropertyChanged("NetWeightFrom");
                }
            }
        private double z_NetWeightFrom = 0;
        #endregion

        #region (double) NetWeightTo Вес нетто до
        [DataField(Description = "Вес нетто до", DecimalPointsNumber = 3)]
        public double NetWeightTo
            {
            get
                {
                return z_NetWeightTo;
                }
            set
                {
                if (z_NetWeightTo == value)
                    {
                    return;
                    }

                z_NetWeightTo = value;
                NotifyPropertyChanged("NetWeightTo");
                }
            }
        private double z_NetWeightTo = 0;
        #endregion

        #region (double) GrossWeightFrom Вес брутто от
        [DataField(Description = "Вес брутто от", DecimalPointsNumber = 3)]
        public double GrossWeightFrom
            {
            get
                {
                return z_GrossWeightFrom;
                }
            set
                {
                if (z_GrossWeightFrom == value)
                    {
                    return;
                    }

                z_GrossWeightFrom = value;
                NotifyPropertyChanged("GrossWeightFrom");
                }
            }
        private double z_GrossWeightFrom = 0;
        #endregion

        #region (double) GrossWeightTo Вес брутто до
        [DataField(Description = "Вес брутто до", DecimalPointsNumber = 3)]
        public double GrossWeightTo
            {
            get
                {
                return z_GrossWeightTo;
                }
            set
                {
                if (z_GrossWeightTo == value)
                    {
                    return;
                    }

                z_GrossWeightTo = value;
                NotifyPropertyChanged("GrossWeightTo");
                }
            }
        private double z_GrossWeightTo = 0;
        #endregion

        #region (double) Price Цена
        [DataField(Description = "Цена", DecimalPointsNumber = 2)]
        public double Price
            {
            get
                {
                return z_Price;
                }
            set
                {
                if (z_Price == value)
                    {
                    return;
                    }

                z_Price = value;
                NotifyPropertyChanged("Price");
                }
            }
        private double z_Price = 0;
        #endregion

        #region (Currency) Currency Валюта
        [DataField(Description = "Валюта")]
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

        #region (string) NameEng Наименование англ.
        [DataField(Description = "Наименование англ.", Size = 300)]
        public string NameOriginal
            {
            get
                {
                return z_NameOriginal;
                }
            set
                {
                if (z_NameOriginal == value)
                    {
                    return;
                    }

                z_NameOriginal = value;
                NotifyPropertyChanged("NameOriginal");
                }
            }
        private string z_NameOriginal = "";
        #endregion

        #region (string) NameDecl Наименование декл
        [DataField(Description = "Наименование декл", Size = 600)]
        public string NameDecl
            {
            get
                {
                return z_NameDecl;
                }
            set
                {
                if (z_NameDecl == value)
                    {
                    return;
                    }

                z_NameDecl = value;
                NotifyPropertyChanged("NameDecl");
                }
            }
        private string z_NameDecl = "";
        #endregion

        #region (string) NameInvoice Наименование инвойс
        public string NameInvoice
            {
            get
                {
                return Description;
                }
            set
                {
                Description = value;
                }
            }
        #endregion

        #endregion

        #region Табличная часть SetContents (Состав набора)
        /// <summary>
        /// Табличная часть справочника "Номенклатура"
        /// </summary>
        [Table(Columns = "ItemNomenclature,ItemArticle,ItemCount")]
        [DataField(Description = "Состав набора")]
        public DataTable SetContents
            {
            get
                {
                return GetSubtable("SetContents");
                }
            }

        [SubTableField(Description = "Товар", PropertyType = typeof(Nomenclature))]
        public DataColumn ItemNomenclature
            {
            get;
            set;
            }

        [SubTableField(Description = "Артикул", PropertyType = typeof(string), Size = 100, StorageType = StorageTypes.Local)]
        public DataColumn ItemArticle
            {
            get;
            set;
            }

        [SubTableField(Description = "Колличество", PropertyType = typeof(int))]
        public DataColumn ItemCount
            {
            get;
            set;
            }

        #endregion

        #region Табличная часть Разрешительные документы

        [Table(Columns = "ApprovalId,DateFrom,DateTo,DocumentNumber,DocumentCode,DocumentType", ShowLineNumberColumn = false)]
        [DataField(Description = "Разрешительные документы", StorageType = StorageTypes.Local)]
        public DataTable Approvals
            {
            get
                {
                return GetSubtable("Approvals");
                }
            }

        [SubTableField(Description = "Дата с", PropertyType = typeof(string))]
        public DataColumn DateFrom
            {
            get;
            set;
            }


        [SubTableField(Description = "Дата по", PropertyType = typeof(string))]
        public DataColumn DateTo
            {
            get;
            set;
            }


        [SubTableField(Description = "Номер документа", PropertyType = typeof(string))]
        public DataColumn DocumentNumber
            {
            get;
            set;
            }

        [SubTableField(Description = "Тип документа", PropertyType = typeof(string))]
        public DataColumn DocumentType
            {
            get;
            set;
            }

        [SubTableField(Description = "Id", PropertyType = typeof(long), ShowInForm = false)]
        public DataColumn ApprovalId
            {
            get;
            set;
            }

        [SubTableField(Description = "Код типа документа", PropertyType = typeof(string))]
        public DataColumn DocumentCode
            {
            get;
            set;
            }

        #endregion


        /// <summary>
        /// Создает новый экземпляр класса
        /// </summary>
        public Nomenclature()
            {
            this.OnRead += Nomenclature_OnRead;
            this.BeforeWriting += Nomenclature_BeforeWriting;
            this.TableRowChanged += Nomenclature_TableRowChanged;
            this.syncronizer = new AllFieldsSyncronizer(this);
            this.approvalsByNomenclatureUpdater = new ApprovalsByNomenclatureUpdater();
            }

        void Nomenclature_BeforeWriting(DatabaseObject item, ref bool cancel)
            {
            long customsCodeId = this.CustomsCodeInternal.Id;
            if (initCustomsCodeId != customsCodeId && initCustomsCodeId != 0)
                {
                approvalsByNomenclatureUpdater.RemoveNomenclatureFromSomeApprovals(this.Id);
                initCustomsCodeId = customsCodeId;
                }
            }

        /// <summary>
        /// Обрабатываем изменения в табличной части для синхронизации колонок номенклатура и артикул, которая берется из элемента номенклатура
        /// </summary>
        void Nomenclature_TableRowChanged(DataTable dataTable, DataColumn currentColumn, DataRow currentRow)
            {
            if (currentColumn == this.ItemNomenclature)
                {
                this.syncArticleColumn(currentRow, true);
                }
            else if (currentColumn == this.ItemArticle)
                {
                this.syncArticleColumn(currentRow, false);
                }
            }
        /// <summary>
        /// Синхронизирует колонки артикул и номенклатура
        /// </summary>
        /// <param name="currentRow">строка подлежащая синхронизации</param>
        /// <param name="directionToArticle">направление, либо записываем значение артикула из текущей номенклатуры в строке, либо записываем
        /// в артикул текущей номенклатуры значение из колонки артикул табличной части</param>
        private void syncArticleColumn(DataRow currentRow, bool directionToArticle)
            {
            if (currentRow[ItemNomenclature] != DBNull.Value && currentRow[ItemNomenclature] != null)
                {
                long itemId = (long)currentRow[ItemNomenclature];
                if (itemId == this.Id)//выполняем проверку не добавил ли пользователь в состав набора ту же номенклатуру в которую он и добавляет, если да - сбрасываем значение
                    {
                    currentRow[ItemNomenclature] = 0;
                    currentRow[ItemArticle] = "";
                    return;
                    }
                //выполняем синхронизацию
                Nomenclature itemNomenclature = new Nomenclature();
                itemNomenclature.Read(itemId);
                if (directionToArticle)
                    {
                    currentRow[ItemArticle] = itemNomenclature.Article;
                    }
                else
                    {
                    if (currentRow[ItemArticle] != null && currentRow[ItemArticle] != DBNull.Value)
                        {
                        itemNomenclature.Article = (string)currentRow[ItemArticle];
                        itemNomenclature.Write();
                        }
                    }
                }
            }
        /// <summary>
        /// Заполняет колонку "Артикул" табличной части, значением артикула у номенклатуры в колонке номенклатура
        /// </summary>
        void Nomenclature_OnRead()
            {
            if (SetContents != null)
                {
                foreach (DataRow row in SetContents.Rows)
                    {
                    syncArticleColumn(row, true);
                    }
                }
            this.initCustomsCodeId = this.CustomsCodeInternal.Id;
            FillApprovals();
            }

        public override GetListFilterDelegate GetFuncGetCustomFilter(string propertyName)
            {
            return syncronizer.GetFuncGetCustomFilter(propertyName);
            }

        private string fillApprovalsStr = @"
with dates as 
(
  select CAST(CURRENT_TIMESTAMP as DATE) as dateToCheck
)
select appr.Id as approvalsId,LTRIM(RTRIM(appr.DocumentNumber)) as documentNumber,LTRIM(RTRIM(dt.QualifierCodeName)) as documentTypeCode,
LTRIM(RTRIM(dt.Description)) as documentType,
appr.DateFrom as dateFrom,appr.DateTo as dateTo
from Approvals as appr
join Contractor as contr on contr.Id = appr.Contractor
left outer join TradeMark as tm on tm.Id = appr.TradeMark
join DocumentType as dt on dt.Id = appr.DocumentType
left outer join SubApprovalsNomenclatures as nom on nom.IdDoc = appr.Id
join dates on (dates.dateToCheck between cast(appr.DateFrom as DATE) and CAST(appr.DateTo as Date))
or (CAST(appr.DateTo as Date) = '0001.01.01' and 
(DATEPART(YEAR,dates.dateToCheck)) <= DATEPART(YEAR,CURRENT_TIMESTAMP) and 
cast(appr.DateFrom as DATE) <=dates.dateToCheck) 
where appr.MarkForDeleting = 0  and nom.ItemNomenclature = @nomenclatureId;";

        public void FillApprovals()
            {
            this.Approvals.Rows.Clear();
            Query q = DB.NewQuery(fillApprovalsStr);
            q.AddInputParameter("nomenclatureId", this.Id);
            DataTable resultsTable = q.SelectToTable();
            for (int rowNumber = 0; rowNumber < resultsTable.Rows.Count; rowNumber++)
                {
                DataRow targetRow = resultsTable.Rows[rowNumber];
                var dataRow = this.Approvals.NewRow();
                dataRow[DateFrom] = targetRow.TrySafeGetColumnValue<DateTime>("dateFrom", DateTime.MinValue).ToString("yyyy.MM.dd"); //DateTime.Now.AddDays(-10).ToString("yyyMMdd");
                dataRow[DateTo] = targetRow.TrySafeGetColumnValue<DateTime>("dateTo", DateTime.MinValue).ToString("yyyy.MM.dd"); // DateTime.Now.AddDays(10).ToString("yyyMMdd");
                dataRow[DocumentNumber] = targetRow.TrySafeGetColumnValue<string>("documentNumber", string.Empty);//"Number " + rowNumber.ToString();
                dataRow[DocumentType] = targetRow.TrySafeGetColumnValue<string>("documentType", string.Empty);//rowNumber % 2 == 0 ? "type A" : "type B";
                dataRow[ApprovalId] = targetRow.TrySafeGetColumnValue<long>("approvalsId", 0);
                dataRow[DocumentCode] = targetRow.TrySafeGetColumnValue<string>("documentTypeCode", string.Empty);
                this.Approvals.Rows.Add(dataRow);
                }
            }

        }
    }
