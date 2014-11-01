using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.InvoiceProcessing;
using Aramis.Core;
using Aramis.Attributes;
using Aramis.Enums;
using SystemInvoice.Catalogs;
using System.Data;
using Aramis.DatabaseConnector;
using SystemInvoice.PropsSyncronization;
using Aramis.Platform;
using Aramis.UI.WinFormsDevXpress;
using Catalogs;
using Documents;
using AramisCatalogs = Catalogs;
using SystemInvoice.SystemObjects;

namespace SystemInvoice.Documents
    {
    /// <summary>
    /// Документ. Содержит информацию о сроках действия сертификатов/документов для группы товаров
    /// </summary>
    [Document(Description = "Разрешительный документ", GUID = "0980DB9C-524B-43B6-91F4-424982EF9B99", NumberType = NumberType.Int64,
        DateFieldDescription = "Дата создания", ShowCreationDate = false, ShowLastModifiedDate = false,
        ShowResponsibleInList = true)]
    public class Approvals : DocumentTable, ITradeMarkContractorApprovalsLoadFormatSource
        {
        public const int NUMBER_MAX_LENGTH = 70;
        private Color errorColor = Color.White;
        private const string DATE_TO_COLUMN_NAME = "DateTo";
        private HashSet<long> whenLoadDocumentNomenclatures = new HashSet<long>();


        TradeMarkContractorAprovalsLoadFormatSyncronizer syncronizer = null;

        #region Свойства

        #region (DocumentType) DocumentType Тип документа
        [DataField(Description = "Тип документа", NotEmpty = true, ShowInList = true)]
        public IDocumentType DocumentType
            {
            get
                {
                return (IDocumentType)GetValueForObjectProperty("DocumentType");
                }
            set
                {
                SetValueForObjectProperty("DocumentType", value);
                }
            }
        #endregion

        #region (string) DocumentCode Код
        [DataField(Description = "Код", NotEmpty = true, ReadOnly = true, ShowInList = true)]
        public string DocumentCode
            {
            get
                {
                return z_DocumentCode;
                }
            set
                {
                if (z_DocumentCode == value)
                    {
                    return;
                    }
                z_DocumentCode = value;
                NotifyPropertyChanged("DocumentCode");
                }
            }
        private string z_DocumentCode = "";
        #endregion

        #region (string) DocumentNumber Номер документа
        [DataField(Description = "Номер документа", NotEmpty = true, Size = NUMBER_MAX_LENGTH, ShowInList = true)]
        public string DocumentNumber
            {
            get
                {
                return z_DocumentNumber;
                }
            set
                {
                if (z_DocumentNumber == value)
                    {
                    return;
                    }

                z_DocumentNumber = value;
                NotifyPropertyChanged("DocumentNumber");
                }
            }
        private string z_DocumentNumber = "";
        #endregion

        #region (DateTime) DateFrom Срок действия с
        [DataField(Description = "Срок действия с", NotEmpty = true, ShowInList = true)]
        public DateTime DateFrom
            {
            get
                {
                return z_DateFrom;
                }
            set
                {
                if (z_DateFrom == value)
                    {
                    return;
                    }

                z_DateFrom = value;
                NotifyPropertyChanged("DateFrom");
                }
            }
        private DateTime z_DateFrom;
        #endregion

        #region (DateTime) DateTo Срок действия по
        [DataField(Description = "Срок действия по", ShowInList = true)]
        public DateTime DateTo
            {
            get
                {
                return z_DateTo;
                }
            set
                {
                if (z_DateTo == value)
                    {
                    return;
                    }

                z_DateTo = value;
                NotifyPropertyChanged("DateTo");
                }
            }
        private DateTime z_DateTo;
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

        #region (ApprovalsLoadFormat) ApprovalsLoadFormat Формат загрузки
        [DataField(Description = "Формат загрузки")]
        public ApprovalsLoadFormat ApprovalsLoadFormat
            {
            get
                {
                return (ApprovalsLoadFormat)GetValueForObjectProperty("ApprovalsLoadFormat");
                }
            set
                {
                SetValueForObjectProperty("ApprovalsLoadFormat", value);
                }
            }
        #endregion

        [DataField(Description = "Основание")]
        public Approvals BaseApproval
            {
            get
                {
                return (Approvals)GetValueForObjectProperty("BaseApproval");
                }
            set
                {
                SetValueForObjectProperty("BaseApproval", value);
                }
            }


        #endregion

        #region Табличная часть Nomenclatures (Товары)
        [Table(Columns = "ItemNomenclature,ItemArticle,ItemCustomsCodeInternal,ItemManufacturer")]
        [DataField(Description = "Товары")]
        public DataTable Nomenclatures
            {
            get
                {
                return GetSubtable("Nomenclatures");
                }
            }

        [SubTableField(Description = "Товар", PropertyType = typeof(Nomenclature), NotEmpty = true, Unique = true)]
        public DataColumn ItemNomenclature
            {
            get;
            set;
            }

        [SubTableField(Description = "Артикул", PropertyType = typeof(string), NotEmpty = true, ReadOnly = true, StorageType = StorageTypes.Local)]
        public DataColumn ItemArticle
            {
            get;
            set;
            }

        [SubTableField(Description = "Там. Код внутренний", PropertyType = typeof(string), NotEmpty = true, ReadOnly = true, StorageType = StorageTypes.Local)]
        public DataColumn ItemCustomsCodeInternal
            {
            get;
            set;
            }

        [SubTableField(Description = "Производитель", PropertyType = typeof(string), NotEmpty = true, ReadOnly = true, StorageType = StorageTypes.Local)]
        public DataColumn ItemManufacturer
            {
            get;
            set;
            }

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает новый екземпляр класса
        /// </summary>
        public Approvals()
            {
            this.OnRead += Approvals_OnRead;
            this.TableRowChanged += Approvals_TableRowChanged;
            this.ValueOfObjectPropertyChanged += Approvals_ValueOfObjectPropertyChanged;
            this.syncronizer = new TradeMarkContractorAprovalsLoadFormatSyncronizer(this);
            ColorConverter colorConverter = new ColorConverter();
            errorColor = (Color)colorConverter.ConvertFromString(ProcessingConsts.CELL_ERROR_COLOR);
            }

        #endregion

        #region Методы, обработчики
        /// <summary>
        /// При изменении поля тип документа синхронизируем поле код с новым значением
        /// </summary>
        /// <param name="propertyName"></param>
        void Approvals_ValueOfObjectPropertyChanged(string propertyName)
            {
            if (propertyName.Equals("DocumentType"))
                {
                this.syncCode();
                }
            if (propertyName.Equals("TradeMark") || propertyName.Equals("Contractor"))
                {
                initApprovalsLoadFormat();
                }
            }

        /// <summary>
        /// Синхронизирует поле код с полем код по классификатору из поля тип документа
        /// </summary>
        private void syncCode()
            {
            if (this.DocumentType != null)
                {
                if (this.DocumentType.Id == 0)
                    {
                    DocumentNumber = "";
                    return;
                    }
                this.DocumentCode = DocumentType.QualifierCodeName;
                }
            }

        /// <summary>
        /// При изменении табличной части синхронизируем привязанные к номенклатуре колонки
        /// </summary>
        void Approvals_TableRowChanged(DataTable dataTable, DataColumn currentColumn, DataRow currentRow)
            {
            try
                {
                if (currentColumn == ItemNomenclature)
                    {
                    this.syncApprovalColumn(currentRow);
                    }
                }
            catch { }
            }

        /// <summary>
        /// При загрузке табличной части загружаем в локальные колонки значения из номенклатуры
        /// </summary>
        void Approvals_OnRead()
            {
            try
                {
                //выбираем формат загрузки если он не установлен
                this.initApprovalsLoadFormat();
                //записваем локальное поле код
                this.syncCode();
                //записываем локальные значения полей табличной части
                UpdateLocalValuesOfTablePart();
                //сохраняем номенклатуру которая была при загрузке файла
                refreshComparedNomenclatures();
                }
            catch { }
            }

        /// <summary>
        /// Выбирает формат загрузки с текущими значениями контрагента/торговой марки, если таковой существует в единственном числе
        /// </summary>
        private void initApprovalsLoadFormat()
            {
            if (this.Contractor.Id == 0 || this.ApprovalsLoadFormat.Id != 0)
                {
                return;
                }
            string queryText = @"select max(ID) as id from ApprovalsLoadFormat where TradeMark = @tradeMark and Contractor = @contractor having COUNT(*) = 1;";
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
                ApprovalsLoadFormat loadFormat = new ApprovalsLoadFormat();
                loadFormat.Id = id;
                loadFormat.Contractor = contractorForApprovals;
                loadFormat.TradeMark = tradeMarkForApprovals;
                loadFormat.Read();
                this.ApprovalsLoadFormat = loadFormat;
                }
            }

        /// <summary>
        /// 3аписывает локальные значения полей табличной части
        /// </summary>
        public void UpdateLocalValuesOfTablePart()
            {
            foreach (DataRow row in Nomenclatures.Rows)
                {
                this.syncApprovalColumn(row);
                }
            }

        /// <summary>
        /// Синхронизирует строку в табличной части, записывая в колонки артикул, таможенный код значения из номенклатуры колонки номенклатура
        /// </summary>
        /// <param name="currentRow">Строка которую нужно синхронизировать</param>
        private void syncApprovalColumn(DataRow currentRow)
            {
            if (currentRow == null)
                {
                foreach (DataRow row in Nomenclatures.Rows)
                    {
                    if (row != null)
                        {
                        syncApprovalColumn(row);
                        }
                    }
                }

            if (currentRow[ItemNomenclature] != null && currentRow[ItemNomenclature] != DBNull.Value)
                {
                var nomenclatureRow = A.New<Nomenclature>(currentRow[ItemNomenclature]);
                currentRow[ItemArticle] = nomenclatureRow.Article;
                currentRow[ItemCustomsCodeInternal] = nomenclatureRow.CustomsCodeInternal.Description;
                currentRow[ItemManufacturer] = nomenclatureRow.Manufacturer.Description;
                }
            }
        #endregion

        public override GetListFilterDelegate GetFuncGetCustomFilter(string propertyName)
            {
            return syncronizer.GetFuncGetCustomFilter(propertyName);
            }

        public override Func<DataRow, System.Drawing.Color> GetFuncGetRowColor()
            {
            return new Func<DataRow, System.Drawing.Color>((row) =>
                {
                    return getColorForRow(row);
                });
            }

        public override WritingResult Write()
            {
            removeSameRows();
            saveDeletedNomenclaturesHistory();
            refreshComparedNomenclatures();
            int lineNumber = 1;
            for (int i = 0; i < Nomenclatures.Rows.Count; i++)
                {
                Nomenclatures.Rows[i]["LineNumber"] = lineNumber++;
                }
            if (IsNew && Responsible.Empty)
                {
                SetRef("Responsible", SystemAramis.CurrentUserId);
                }
            return base.Write();
            }

        private void removeSameRows()
            {
            var wares = new HashSet<long>();

            for (int rowIndex = Nomenclatures.Rows.Count - 1; rowIndex >= 0; rowIndex--)
                {
                var row = Nomenclatures.Rows[rowIndex];
                var wareId = row[ItemNomenclature].ToInt64();
                if (wareId == 0 || wares.Contains(wareId))
                    {
                    Nomenclatures.Rows.RemoveAt(rowIndex);
                    }
                else
                    {
                    wares.Add(wareId);
                    }
                }
            }

        private System.Drawing.Color getColorForRow(DataRow row)
            {
            if (checkApprovalsToDateIsAfterCriticalDate(row))
                {
                return errorColor;
                }
            return System.Drawing.Color.White;
            }

        private bool checkApprovalsToDateIsAfterCriticalDate(DataRow row)
            {
            bool isRowContainsToDateValue = row.Table.Columns.Contains(DATE_TO_COLUMN_NAME) && row[DATE_TO_COLUMN_NAME] != DBNull.Value;
            if (isRowContainsToDateValue)
                {
                DateTime approvalsToDate = getApprovalsToDate(row);
                DateTime criticalDate = getCriticalCheckingDate();
                if (approvalsToDate < criticalDate)
                    {
                    return true;
                    }
                }
            return false;
            }

        private DateTime getCriticalCheckingDate()
            {
            int duration = getCheckingFromCurrentDateDuration();
            DateTime criticalTime = DateTime.Now.AddDays(duration);
            return criticalTime;
            }

        private DateTime getApprovalsToDate(DataRow row)
            {
            DateTime rowDateTime = (DateTime)row[DATE_TO_COLUMN_NAME];
            if (rowDateTime == DateTime.MinValue)
                {
                rowDateTime = DateTime.Today.AddMonths(1 - DateTime.Now.Month).AddDays(1 - DateTime.Now.Day).AddYears(1).AddMilliseconds(-1);
                }
            return rowDateTime;
            }

        private int getCheckingFromCurrentDateDuration()
            {
            int duration = 0;
            string durationStr = AramisCatalogs.SystemInvoiceConstants.AlarmForApprovalBecomeFailDays;
            int.TryParse(durationStr, out duration);
            return duration;
            }

        private void refreshComparedNomenclatures()
            {
            this.whenLoadDocumentNomenclatures = getCurrentNomenclaturesIds();
            if (this.MarkForDeleting)
                {
                whenLoadDocumentNomenclatures.Clear();
                }
            }

        private void saveDeletedNomenclaturesHistory()
            {
            HashSet<long> currentNomenclatures = getCurrentNomenclaturesIds();
            if (this.MarkForDeleting)
                {
                currentNomenclatures.Clear();
                }
            foreach (long nomenclatureId in this.whenLoadDocumentNomenclatures)
                {
                if (!currentNomenclatures.Contains(nomenclatureId))
                    {
                    NomenclatureApprovalsRemovingHistory removingHistory = new NomenclatureApprovalsRemovingHistory();
                    removingHistory.Nomenclature = new Nomenclature() { Id = nomenclatureId };
                    removingHistory.NomenclatureRemovigTypeKind = NomenclatureRemovigTypeKind.Manual;
                    removingHistory.RemovingDate = getServerTime();
                    removingHistory.DateFrom = this.DateFrom;
                    removingHistory.DateTo = this.DateTo;
                    removingHistory.DocumentType = this.DocumentType;
                    var result = removingHistory.Write();
                    }
                }
            }

        private DateTime getServerTime()
            {
            string selectQuery = "select CURRENT_TIMESTAMP;";
            object result = DB.NewQuery(selectQuery).SelectScalar();
            if (result != null && result != DBNull.Value && result is DateTime)
                {
                return (DateTime)result;
                }
            return DateTime.MinValue;
            }

        private HashSet<long> getCurrentNomenclaturesIds()
            {
            HashSet<long> nomenclatures = new HashSet<long>();

            foreach (DataRow dataRow in Nomenclatures.Rows)
                {
                long nomenclatureId = (long)dataRow[ItemNomenclature];
                if (nomenclatureId != 0)
                    {
                    nomenclatures.Add(nomenclatureId);
                    }
                }
            return nomenclatures;
            }

        internal void AddWareId(long wareId)
            {
            foreach (DataRow row in Nomenclatures.Rows)
                {
                var currentWareId = (long)row[ItemNomenclature];
                if (currentWareId == wareId) return;
                }

            var newRow = Nomenclatures.GetNewRow(this);
            newRow[ItemNomenclature] = wareId;
            newRow.AddRowToTable(this);
            SetSubtableModified("Nomenclatures");
            }
        }
    }
