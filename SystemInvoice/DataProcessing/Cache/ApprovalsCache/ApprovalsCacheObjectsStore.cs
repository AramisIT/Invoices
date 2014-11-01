using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;

namespace SystemInvoice.DataProcessing.Cache.ApprovalsCache
    {
    /// <summary>
    /// Хранилище кешированных данных из табличных частей РД
    /// </summary>
    public class ApprovalsCacheObjectsStore : CacheObjectsStore<ApprovalsCacheObject>
        {
        private long currentTradeMark = 0;
        private long currentContractor = 0;
        private HashSet<DateTime> currentDates = null;// DateTime.MinValue;
        /// <summary>
        /// Индекс для поиска РД по шапке
        /// </summary>
        private readonly ApprovalsWithoutNomenclatureIndex searchForUpdateApprovalsIndex = new ApprovalsWithoutNomenclatureIndex();
        /// <summary>
        /// Индекс для поиска разрешительных в инвойсе по номенклатуре и дате инвойса
        /// </summary>
        private readonly ApprovalsSearchIndex approvalsSearchIndex = new ApprovalsSearchIndex();

        /// <summary>
        /// Используется для установки "Дата по" кешированного документа если возникает ошибка чтения этой даты. Это сделано для того что бы не стояло в случае ошибки значение DateTime.MinValue
        /// при котором система считает что срок действия РД до конца текущего года.
        /// </summary>
        private DateTime errorReadDateTimeTo = DateTime.MinValue.AddDays(1);

        #region Имена колонок для доступа к к результатам выполнения запроса

        private const string APPROVALS_ID_COLUMN_NAME = "approvalsId";
        private const string DOCUMENT_TYPE_CODE_COLUMN_NAME = "documentTypeCode";
        private const string DOCUMENT_TYPE_NAME_COLUMN_NAME = "documentType";
        private const string CONTRACTOR_NAME_COLUMN_NAME = "contractorName";
        private const string TRADEMARK_NAME_COLUMN_NAME = "trademarkName";
        private const string DOCUMENT_TYPE_ID_COLUMN_NAME = "documentTypeId";
        private const string TRADE_MARK_ID_COLUMN_NAME = "tradeMarkId";
        private const string CONTRACTOR_ID_COLUMN_NAME = "contractorId";
        private const string DATE_FROM_COLUMN_NAME = "dateFrom";
        private const string DATE_TO_COLUMN_NAME = "dateTo";
        private const string NOMENCLATURE_ID_COLUMN_NAME = "nomenclatureId";
        private const string DOCUMENT_NUMBER_COLUMN_NAME = "documentNumber";
        private const string DATE_FOR_COLUMN_NAME = "dateFor";
        private const string DOCUMENT_BASE_NUMBER_COLUMN_NAME = "documentBaseNumber";

        #endregion
        /// <summary>
        /// Возвращает текст запроса для загрузки РД для выбранного контрагента, выбранной торговой марки, и даты в течении которой РД является действительной.
        /// В качестве ID используется значение оконной функции поскольку нам все равно нужно вернуть колонку с именем ID, но у табличной части из которой мы выбираем данные поле
        /// которое можно было бы использовать в качестве айдишника отсутствует
        /// </summary>
        protected override string SelectQuery
            {
            get
                {
                string joinedPart = getDatesSelectPart();
                string selectColumnsPart = string.Format(@"select appr.Id as {0},dates.dateToCheck as {14},LTRIM(RTRIM(appr.DocumentNumber)) as {13},LTRIM(RTRIM(dt.QualifierCodeName)) as {1},LTRIM(RTRIM(dt.Description)) as {2},
LTRIM(RTRIM(contr.Description)) as {3},LTRIM(RTRIM(coalesce(tm.Description,'')))  as {4},
dt.Id as {5},coalesce(tm.Id,0) as {6},contr.Id as {7},appr.DateFrom as {8},appr.DateTo as {9}
,coalesce(nom.ItemNomenclature,-1) as {10},ROW_NUMBER() over(order by appr.Id,nom.LineNumber) as Id,
isnull(LTRIM(RTRIM(ApprovalsBase.DocumentNumber)), '') as {15}
from Approvals as appr
left join Approvals ApprovalsBase on ApprovalsBase.Id = appr.BaseApproval
join Contractor as contr on contr.Id = appr.Contractor and (contr.Don_tLoadCertToInvoice = 0 or appr.DocumentType <> {16})
left outer join TradeMark as tm on tm.Id = appr.TradeMark
join DocumentType as dt on dt.Id = appr.DocumentType
left outer join SubApprovalsNomenclatures as nom on nom.IdDoc = appr.Id
join dates on (dates.dateToCheck between cast(appr.DateFrom as DATE) and CAST(appr.DateTo as Date))
or (CAST(appr.DateTo as Date) = '0001.01.01' and (DATEPART(YEAR,dates.dateToCheck)) <= DATEPART(YEAR,CURRENT_TIMESTAMP) and cast(appr.DateFrom as DATE) <=dates.dateToCheck) 
where appr.MarkForDeleting = 0  and (appr.TradeMark = {11} or {11} = 0) and (appr.Contractor = {12} or {12} = 0);", APPROVALS_ID_COLUMN_NAME, DOCUMENT_TYPE_CODE_COLUMN_NAME, DOCUMENT_TYPE_NAME_COLUMN_NAME, CONTRACTOR_NAME_COLUMN_NAME,
                                TRADEMARK_NAME_COLUMN_NAME, DOCUMENT_TYPE_ID_COLUMN_NAME,
                                TRADE_MARK_ID_COLUMN_NAME, CONTRACTOR_ID_COLUMN_NAME, DATE_FROM_COLUMN_NAME, DATE_TO_COLUMN_NAME, NOMENCLATURE_ID_COLUMN_NAME,
                                 currentTradeMark, currentContractor, DOCUMENT_NUMBER_COLUMN_NAME, DATE_FOR_COLUMN_NAME, DOCUMENT_BASE_NUMBER_COLUMN_NAME, DocumentTypeHelper.GetCertificateType().Id);
                string constructedQuery = string.Concat(joinedPart, selectColumnsPart);
                return constructedQuery;
                }
            }

        private string getDatesSelectPart()
            {
            StringBuilder strBuilder = new StringBuilder();
            if (currentDates == null || currentDates.Count == 0)
                {
                strBuilder.Append(createDatePart(new DateTime(1, 1, 1)));
                }
            else
                {
                foreach (DateTime dt in currentDates)
                    {
                    if (strBuilder.Length == 0)
                        {
                        strBuilder.Append(createDatePart(dt));
                        }
                    else
                        {
                        strBuilder.Append(string.Concat(Environment.NewLine, " union", Environment.NewLine, createDatePart(dt)));
                        }
                    }
                }
            string finalStr = strBuilder.ToString();
            return string.Concat(@"with dates as (", Environment.NewLine, finalStr, Environment.NewLine, "  )");
            }

        private string createDatePart(DateTime dateTime)
            {
            return string.Format("  select CAST('{0}' as DATE) as dateToCheck", dateTime.ToString("yyyy.MM.dd"));
            }

        protected override string LatModifiedDateQuery
            {
            get
                {
                return "select CURRENT_TIMESTAMP;";// Max(LastModified) from Approvals as appr;"; 
                }
            }

        protected override string LastProcessedCountQuery
            {
            get
                {
                string joinedPart = getDatesSelectPart();
                string selectColumnsPart = string.Format(@"select Count(*) from Approvals as appr
join Contractor as contr on contr.Id = appr.Contractor
left outer join TradeMark as tm on tm.Id = appr.TradeMark
join DocumentType as dt on dt.Id = appr.DocumentType
left outer join SubApprovalsNomenclatures as nom on nom.IdDoc = appr.Id
join dates on (dates.dateToCheck between cast(appr.DateFrom as DATE) and CAST(appr.DateTo as Date))
or (CAST(appr.DateTo as Date) = '0001.01.01' and (DATEPART(YEAR,dates.dateToCheck)) <= DATEPART(YEAR,CURRENT_TIMESTAMP) and cast(appr.DateFrom as DATE) <=dates.dateToCheck) 
where appr.MarkForDeleting = 0  and (appr.TradeMark = {11} or {11} = 0) and (appr.Contractor = {12} or {12} = 0);", APPROVALS_ID_COLUMN_NAME, DOCUMENT_TYPE_CODE_COLUMN_NAME, DOCUMENT_TYPE_NAME_COLUMN_NAME, CONTRACTOR_NAME_COLUMN_NAME,
                                TRADEMARK_NAME_COLUMN_NAME, DOCUMENT_TYPE_ID_COLUMN_NAME,
                                TRADE_MARK_ID_COLUMN_NAME, CONTRACTOR_ID_COLUMN_NAME, DATE_FROM_COLUMN_NAME, DATE_TO_COLUMN_NAME, NOMENCLATURE_ID_COLUMN_NAME,
                                 currentTradeMark, currentContractor, DOCUMENT_NUMBER_COLUMN_NAME, DATE_FOR_COLUMN_NAME);
                string constructedQuery = string.Concat(joinedPart, selectColumnsPart);
                return constructedQuery;
                }
            }

        /// <summary>
        /// Выполянет очистку кеша (если необходимо) и загрузку в него данных по РД для заданной ТМ, контрагента, даты (если существующие данные потеряли актуальность)
        /// </summary>
        public void Refresh(long currentTradeMark, long currentContractor, HashSet<DateTime> currentDates)
            {
            this.currentTradeMark = currentTradeMark;
            this.currentDates = currentDates;
            this.currentContractor = currentContractor;
            base.Refresh();
            }

        protected override ApprovalsCacheObject createNew(System.Data.DataRow row)
            {
            long approvalsId = row.TrySafeGetColumnValue<long>(APPROVALS_ID_COLUMN_NAME, 0);
            string documentNumber = row.TrySafeGetColumnValue<string>(DOCUMENT_NUMBER_COLUMN_NAME, string.Empty);
            string documentBaseNumber = row.TrySafeGetColumnValue<string>(DOCUMENT_BASE_NUMBER_COLUMN_NAME, string.Empty);
            string documentTypeName = row.TrySafeGetColumnValue<string>(DOCUMENT_TYPE_NAME_COLUMN_NAME, string.Empty);
            string documentCodeName = row.TrySafeGetColumnValue<string>(DOCUMENT_TYPE_CODE_COLUMN_NAME, string.Empty);
            string contractorName = row.TrySafeGetColumnValue<string>(CONTRACTOR_NAME_COLUMN_NAME, string.Empty);
            string trademarkName = row.TrySafeGetColumnValue<string>(TRADEMARK_NAME_COLUMN_NAME, string.Empty);
            long doctypeID = row.TrySafeGetColumnValue<long>(DOCUMENT_TYPE_ID_COLUMN_NAME, 0);
            long contractorID = row.TrySafeGetColumnValue<long>(CONTRACTOR_ID_COLUMN_NAME, 0);
            long tradeMarkID = row.TrySafeGetColumnValue<long>(TRADE_MARK_ID_COLUMN_NAME, 0);
            DateTime dateFrom = row.TrySafeGetColumnValue<DateTime>(DATE_FROM_COLUMN_NAME, DateTime.MinValue);
            DateTime dateTo = row.TrySafeGetColumnValue<DateTime>(DATE_TO_COLUMN_NAME, errorReadDateTimeTo);
            DateTime searchedDate = row.TrySafeGetColumnValue<DateTime>(DATE_FOR_COLUMN_NAME, DateTime.MinValue);
            long nomenclatureId = row.TrySafeGetColumnValue<long>(NOMENCLATURE_ID_COLUMN_NAME, -1);

            var createdObject = new ApprovalsCacheObject(documentNumber, documentTypeName, documentCodeName, contractorName, trademarkName, doctypeID, contractorID, tradeMarkID,
                    dateFrom, dateTo, searchedDate, approvalsId, nomenclatureId, documentBaseNumber);

            return createdObject;
            }

        protected override void InitializeIndexes(List<IEqualityComparer<ApprovalsCacheObject>> indexes)
            {
            base.InitializeIndexes(indexes);
            indexes.Add(this.searchForUpdateApprovalsIndex);//индекс для поиска того, существует ли уже РД или нет
            }

        protected override void InitializeOneToManyIndexes(List<IEqualityComparer<ApprovalsCacheObject>> indexes)
            {
            base.InitializeOneToManyIndexes(indexes);
            indexes.Add(this.approvalsSearchIndex);//индекс для поиска разрешительных в инвойсе
            }

        /// <summary>
        /// Проверяет существует ли строка табличной части РД (поиск осуществляется для шапки и номенклатуры в табличной части)
        /// </summary>
        /// <param name="approvalsSearchObject">Содержит в себе данные из шапки РД и номенклатуру, используемые при поиске РД</param>
        /// <returns>Присутствует ли номенклатура в РД</returns>
        public long ContainsApprovals(ApprovalsCacheObject approvalsSearchObject)
            {
            return ContainsApprovals(approvalsSearchObject.DocumentNumber, approvalsSearchObject.DocumentTypeId, approvalsSearchObject.TradeMarkId,
                approvalsSearchObject.ContractorId, approvalsSearchObject.DateFrom, approvalsSearchObject.DateTo, approvalsSearchObject.NomenclatureId, approvalsSearchObject.DocumentBaseNumber);
            }

        /// <summary>
        /// Проверяет существует ли строка табличной части РД (поиск осуществляется для шапки и номенклатуры в табличной части)
        /// </summary>
        /// <returns>Присутствует ли номенклатура в РД</returns>
        public long ContainsApprovals(string documentNumber, long docType, long tradeMark, long contractor, DateTime from, DateTime to, long nomenclatureId, string baseDocumentNumber)
            {
            long founded = 0;
            ApprovalsCacheObject withTmSearchObject = new ApprovalsCacheObject(documentNumber, string.Empty, string.Empty, string.Empty, string.Empty,
                docType, contractor, tradeMark, from, to, DateTime.MinValue, 0, nomenclatureId, baseDocumentNumber);
            if ((founded = GetCachedObjectId(withTmSearchObject)) != 0)
                {
                return founded;
                }
            ApprovalsCacheObject withoutTmSearchObject = new ApprovalsCacheObject(documentNumber, string.Empty, string.Empty, string.Empty, string.Empty, docType, contractor, 0, from, to, DateTime.MinValue, 0, nomenclatureId, baseDocumentNumber);

            return GetCachedObjectId(withoutTmSearchObject);
            }

        /// <summary>
        /// Возвращает кешированный "ID" для РД, поиск которого осуществляется данными из шапки РД
        /// </summary>
        /// <param name="approvalsSearchObject">Содержит в себе данные из шапки РД, используемые при поиске РД</param>
        /// <returns>Псевдо Id см. описание SelectQuery</returns>
        public long GetApprovalsFakeId(ApprovalsCacheObject approvalsSearchObject)
            {
            return GetApprovalsFakeId(approvalsSearchObject.DocumentNumber, approvalsSearchObject.DocumentTypeId, approvalsSearchObject.TradeMarkId,
                approvalsSearchObject.ContractorId, approvalsSearchObject.DateFrom, approvalsSearchObject.DateTo, approvalsSearchObject.DocumentBaseNumber);
            }

        /// <summary>
        /// Возвращает кешированный "ID" для РД, поиск которого осуществляется данными из шапки РД
        /// </summary>
        /// <returns>Псевдо Id см. описание SelectQuery</returns>
        public long GetApprovalsFakeId(string documentNumber, long docType, long tradeMark, long contractor, DateTime from, DateTime to, string baseDocumentNumber)
            {
            long founded = 0;
            ApprovalsCacheObject withTmSearchObject = new ApprovalsCacheObject(documentNumber, string.Empty, string.Empty, string.Empty, string.Empty,
                docType, contractor, tradeMark, from, to, DateTime.MinValue, 0, 0, baseDocumentNumber);
            if ((founded = GetCachedObjectId(this.searchForUpdateApprovalsIndex, withTmSearchObject)) != 0)
                {
                return founded;
                }
            ApprovalsCacheObject withoutTmSearchObject = new ApprovalsCacheObject(documentNumber, string.Empty, string.Empty, string.Empty, string.Empty, docType, contractor, 0, from, to, DateTime.MinValue, 0, 0, baseDocumentNumber);
            return GetCachedObjectId(this.searchForUpdateApprovalsIndex, withoutTmSearchObject);
            }

        /// <summary>
        /// Возвращает информацию о разрешительных документах для номенклатуры и даты
        /// </summary>
        /// <param name="nomenclatureId">Айдишник номенклатуры</param>
        /// <param name="date">Дата инвойса</param>
        /// <returns>Список кешированных объектов с информацией о разрешительных док-тах</returns>
        public List<ApprovalsCacheObject> GetApprovals(long nomenclatureId, DateTime date)
            {
            ApprovalsCacheObject searchObject = new ApprovalsCacheObject(string.Empty, string.Empty, 0, 0, 0,
                                                                         DateTime.Now, DateTime.Now, date,
                                                                         nomenclatureId);
            List<ApprovalsCacheObject> cachedObjects = new List<ApprovalsCacheObject>();
            IEnumerable<long> ids = this.GetCachedObjectIds(this.approvalsSearchIndex, searchObject);
            if (ids != null)
                {
                foreach (long objectId in ids)
                    {
                    var cachedItem = GetFromOneToManyCachedObject(this.approvalsSearchIndex, objectId);
                    cachedObjects.Add(cachedItem);
                    }
                }
            return cachedObjects;
            }

        }
    }
