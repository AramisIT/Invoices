using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;

namespace SystemInvoice.DataProcessing.Cache.NomenclatureRemovingFromApprovalsHistoryCache
    {
    /// <summary>
    /// Используется для хранения информации об номенклатуре удаленной из табличных частей РД. Хранит информацию специфическую для 
    /// торговой марки/контрагента.
    /// </summary>
    public class NomenclatureRemovingHistoryCacheObjectsStore : CacheObjectsStore<NomenclatureRemovingHistoryCacheObject>
        {
        private long currentTradeMark = 0;
        private long currentContractor = 0;
        private HashSet<DateTime> currentDates = null;

        private readonly RequiredApprovalIndex requiredApprovalIndex = new RequiredApprovalIndex();

        protected override string SelectQuery
            {
            get
                {
                string joinedPart = getDatesSelectPart();//СТЕ, со списком дат для которых были актуальные РД, из которых была удаленна номенклатура
                //для теста
                string fakeStr = @"with dates as 
(
  select CAST('2013.05.17' as DATE) as dateToCheck
  union
  select CAST('2013.05.14' as DATE) as dateToCheck
),
removingHistory as
(
	select nom.Nomenclature,nom.DocumentType,checkDates.dateToCheck,
	Max(case when nom.NomenclatureRemovigTypeKind = 0 then nom.RemovingDate else '0001.01.01' end) as manualDeletedTime,
	Max(case when nom.NomenclatureRemovigTypeKind = 1 then nom.RemovingDate else '0001.01.01' end) as autoDeletedTime
	from NomenclatureApprovalsRemovingHistory as nom
	join Nomenclature as nomenclature on nomenclature.Id = nom.Nomenclature
	join dates as checkDates on checkDates.dateToCheck between CAST(nom.DateFrom as Date) and CAST(nom.DateTo as Date)
	where nom.MarkForDeleting = 0 and (nomenclature.TradeMark = 4 or 0 = 0) and (nomenclature.Contractor = 4 or 0 = 0)--set!!!
	group by nom.Nomenclature,nom.DocumentType,checkDates.dateToCheck
),
approvalsInfo as
(
	select distinct nom.ItemNomenclature as nomenclatureId,appr.DocumentType,dates.dateToCheck
	from Approvals as appr
	join Contractor as contr on contr.Id = appr.Contractor
	left outer join TradeMark as tm on tm.Id = appr.TradeMark
	join DocumentType as dt on dt.Id = appr.DocumentType
	left outer join SubApprovalsNomenclatures as nom on nom.IdDoc = appr.Id
	join dates on (dates.dateToCheck between cast(appr.DateFrom as DATE) and CAST(appr.DateTo as Date))
	or (CAST(appr.DateTo as Date) = '0001.01.01' and (DATEPART(YEAR,dates.dateToCheck)) <= DATEPART(YEAR,CURRENT_TIMESTAMP) and cast(appr.DateFrom as DATE) <=dates.dateToCheck) 
	where appr.MarkForDeleting = 0  and (appr.TradeMark = 0 or 0 = 0) and (appr.Contractor = 6 or 0 = 0)--set!!!
)
select ROW_NUMBER() over(order by removingHistory.Nomenclature) as Id, removingHistory.* from removingHistory 
left join approvalsInfo on removingHistory.DocumentType = approvalsInfo.DocumentType
 and removingHistory.Nomenclature = approvalsInfo.nomenclatureId
 and removingHistory.dateToCheck = approvalsInfo.dateToCheck
 where manualDeletedTime<autoDeletedTime and approvalsInfo.nomenclatureId is null;";
                //для продакшэна
                string queryStr =
                    string.Format(@",removingHistory as
(
	select nom.Nomenclature,nom.DocumentType,checkDates.dateToCheck,
	Max(case when nom.NomenclatureRemovigTypeKind = 0 then nom.RemovingDate else '0001.01.01' end) as manualDeletedTime,
	Max(case when nom.NomenclatureRemovigTypeKind = 1 then nom.RemovingDate else '0001.01.01' end) as autoDeletedTime
	from NomenclatureApprovalsRemovingHistory as nom
	join Nomenclature as nomenclature on nomenclature.Id = nom.Nomenclature
	join dates as checkDates on checkDates.dateToCheck between CAST(nom.DateFrom as Date) and CAST(nom.DateTo as Date)
	or (CAST(nom.DateTo as Date) = '0001.01.01' and (DATEPART(YEAR,checkDates.dateToCheck)) <= DATEPART(YEAR,CURRENT_TIMESTAMP) and cast(nom.DateFrom as DATE) <=checkDates.dateToCheck) 
	where nom.MarkForDeleting = 0 and (nomenclature.TradeMark = {0} or {0} = 0) and (nomenclature.Contractor = {1} or {1} = 0)
	group by nom.Nomenclature,nom.DocumentType,checkDates.dateToCheck
),
approvalsInfo as
(
	select distinct nom.ItemNomenclature as nomenclatureId,appr.DocumentType,dates.dateToCheck
	from Approvals as appr
	join Contractor as contr on contr.Id = appr.Contractor
	left outer join TradeMark as tm on tm.Id = appr.TradeMark
	join DocumentType as dt on dt.Id = appr.DocumentType
	left outer join SubApprovalsNomenclatures as nom on nom.IdDoc = appr.Id
	join dates on (dates.dateToCheck between cast(appr.DateFrom as DATE) and CAST(appr.DateTo as Date))
	or (CAST(appr.DateTo as Date) = '0001.01.01' and (DATEPART(YEAR,dates.dateToCheck)) <= DATEPART(YEAR,CURRENT_TIMESTAMP) and cast(appr.DateFrom as DATE) <=dates.dateToCheck) 
	where appr.MarkForDeleting = 0  and (appr.TradeMark =  {0} or {0} = 0) and (appr.Contractor = {1} or {1} = 0)
)
select ROW_NUMBER() over(order by removingHistory.Nomenclature) as Id, removingHistory.* from removingHistory 
left join approvalsInfo on removingHistory.DocumentType = approvalsInfo.DocumentType
 and removingHistory.Nomenclature = approvalsInfo.nomenclatureId
 and removingHistory.dateToCheck = approvalsInfo.dateToCheck
 where manualDeletedTime<autoDeletedTime and approvalsInfo.nomenclatureId is null;", currentTradeMark,
                                  currentContractor);
                string constructedQuery = string.Concat(joinedPart, queryStr);
                return constructedQuery;// fakeStr;
                }
            }
        /// <summary>
        /// Возвращает СТЕ со списоком дат для которых актуальные РД
        /// </summary>
        /// <returns></returns>
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
            get { return "select CURRENT_TIMESTAMP;"; }
            }

        protected override string LastProcessedCountQuery
            {
            get
                {
                string joinedPart = getDatesSelectPart();
                string queryStr =
                  string.Format(@",removingHistory as
(
	select nom.Nomenclature,nom.DocumentType,checkDates.dateToCheck,
	Max(case when nom.NomenclatureRemovigTypeKind = 0 then nom.RemovingDate else '0001.01.01' end) as manualDeletedTime,
	Max(case when nom.NomenclatureRemovigTypeKind = 1 then nom.RemovingDate else '0001.01.01' end) as autoDeletedTime
	from NomenclatureApprovalsRemovingHistory as nom
	join Nomenclature as nomenclature on nomenclature.Id = nom.Nomenclature
	join dates as checkDates on checkDates.dateToCheck between CAST(nom.DateFrom as Date) and CAST(nom.DateTo as Date)
	where nom.MarkForDeleting = 0 and (nomenclature.TradeMark = {0} or {0} = 0) and (nomenclature.Contractor = {1} or {1} = 0)
	group by nom.Nomenclature,nom.DocumentType,checkDates.dateToCheck
),
approvalsInfo as
(
	select distinct nom.ItemNomenclature as nomenclatureId,appr.DocumentType,dates.dateToCheck
	from Approvals as appr
	join Contractor as contr on contr.Id = appr.Contractor
	left outer join TradeMark as tm on tm.Id = appr.TradeMark
	join DocumentType as dt on dt.Id = appr.DocumentType
	left outer join SubApprovalsNomenclatures as nom on nom.IdDoc = appr.Id
	join dates on (dates.dateToCheck between cast(appr.DateFrom as DATE) and CAST(appr.DateTo as Date))
	or (CAST(appr.DateTo as Date) = '0001.01.01' and (DATEPART(YEAR,dates.dateToCheck)) <= DATEPART(YEAR,CURRENT_TIMESTAMP) and cast(appr.DateFrom as DATE) <=dates.dateToCheck) 
	where appr.MarkForDeleting = 0  and (appr.TradeMark =  {0} or {0} = 0) and (appr.Contractor = {1} or {1} = 0)
)
select Count(*) from removingHistory 
left join approvalsInfo on removingHistory.DocumentType = approvalsInfo.DocumentType
 and removingHistory.Nomenclature = approvalsInfo.nomenclatureId
 and removingHistory.dateToCheck = approvalsInfo.dateToCheck
 where manualDeletedTime<autoDeletedTime and approvalsInfo.nomenclatureId is null;", currentTradeMark,
                                currentContractor);
                string constructedQuery = string.Concat(joinedPart, queryStr);
                return constructedQuery;
                }
            }

        protected override NomenclatureRemovingHistoryCacheObject createNew(System.Data.DataRow row)
            {
            DateTime dateToCheck = row.TrySafeGetColumnValue<DateTime>("dateToCheck", DateTime.MinValue);
            long documentType = row.TrySafeGetColumnValue<long>("DocumentType", 0);
            long nomenclatureId = row.TrySafeGetColumnValue<long>("Nomenclature", 0);
            NomenclatureRemovingHistoryCacheObject cached = new NomenclatureRemovingHistoryCacheObject(nomenclatureId, documentType,dateToCheck);
            return cached;
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

        protected override void InitializeOneToManyIndexes(List<IEqualityComparer<NomenclatureRemovingHistoryCacheObject>> indexes)
            {
            indexes.Add(requiredApprovalIndex);//устанавливаем индекс для быстрого поиска по номенклатуре/дате
            base.InitializeOneToManyIndexes(indexes);
            }
        /// <summary>
        /// Возвращает список типов документов которые соответствуют разрешительным действующим на определенную дату, из которых была автоматически удаленна
        /// номенклатура при изменении ее таможенного кода
        /// </summary>
        /// <param name="nomenclatureId">Ай-ди номенклатуры</param>
        /// <param name="checkingDate">Дата действия РД, должна входить в саисок дат переданных в метод Refresh</param>
        /// <returns>Список типов документов</returns>
        public IEnumerable<long> RequiredApprovalTypes(long nomenclatureId, DateTime checkingDate)
            {
            //
            //nomenclatureId = 17955;
            //checkingDate = new DateTime(2013, 5, 17);
            //
            NomenclatureRemovingHistoryCacheObject searchObject =
                new NomenclatureRemovingHistoryCacheObject(nomenclatureId, 0, checkingDate);
            List<long> documentTypesIds = new List<long>();
            IEnumerable<long> ids = this.GetCachedObjectIds(this.requiredApprovalIndex, searchObject);
            if (ids != null)
                {
                foreach (long objectId in ids)
                    {
                    var cachedObject = GetCachedObject(objectId);
                    if (cachedObject != null)
                        {
                        documentTypesIds.Add(cachedObject.DocumentTypesId);
                        }
                    }
                }
            return documentTypesIds;
            }
        }
    }
