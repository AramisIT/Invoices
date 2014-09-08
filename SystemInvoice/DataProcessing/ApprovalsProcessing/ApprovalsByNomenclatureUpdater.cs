using System;
using System.Collections.Generic;
using SystemInvoice.Catalogs;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.ApprovalsProcessing.ByNomenclatureUpdating;
using SystemInvoice.Documents;
using Aramis.Core;
using Aramis.DatabaseConnector;

namespace SystemInvoice.DataProcessing.ApprovalsProcessing
    {
    /// <summary>
    /// Выполняет удаление номенклатуры из табличных частей разрешительных документов для соответствующих типов документов и удаление самих разрешительных если после удаления больше не остается 
    /// номенклатурных позиций в табличных частях
    /// </summary>
    public class ApprovalsByNomenclatureUpdater
        {
        private const string approvalIdInResultColumnName = "Id";
        private const string approvalUpdateKindColumnName = "IsDeleted";
        private const string documentTypeColumnName = "documentTypeId";
        private const string dateFromColumnName = "DateFrom";
        private const string dateToColumnName = "DateTo";
        private const string removeDateColumnName = "removeDate";

        private HashSet<long> updatedApprovals = new HashSet<long>();
        private HashSet<long> deletedApprovals = new HashSet<long>();

        public static event ApprovalsUpdatedHandler OnApprovalsUpdated;
        /// <summary>
        /// Текст запроса выполняющий удаление
        /// </summary>
        private readonly string queryText = @"Declare @updatedRows table(Id bigint,IsDeleted bit,documentTypeId bigint,DateFrom DateTime2,DateTo DateTime2,removeDate DateTime);
                                merge SubApprovalsNomenclatures as subApprovals
                                using  (	select appr.Id,nom.ItemNomenclature from Approvals as appr
	                                join DocumentType as docType on docType.Id = appr.DocumentType
	                                join SubApprovalsNomenclatures as nom on nom.IdDoc = appr.Id
	                                where docType.DeleteApproval = 1 and nom.ItemNomenclature = @nomenclatureId) as canDeleted (Id,ItemNomenclature)
                                on (subApprovals.IdDoc = canDeleted.Id and subApprovals.ItemNomenclature = canDeleted.ItemNomenclature)
                                when matched then 
                                delete
                                output deleted.IdDoc,0,0,current_timestamp,current_timestamp,current_timestamp into @updatedRows;
                                
                                with forUpdate as
                                (
									select rowsToUpdate.*,apr.DateFrom as aprDateFrom,apr.DateTo as aprDateTo,apr.DocumentType as aprDocType
									from @updatedRows as rowsToUpdate
									join Approvals as apr on apr.Id = rowsToUpdate.Id
                                )
                                update forUpdate set DateFrom = aprDateFrom,
													 DateTo = aprDateTo,
													 documentTypeId = aprDocType
                                from forUpdate;

                                with apprDeletePrepare as 
                                (
	                                select appr.Id from Approvals as appr
	                                join @updatedRows as urows on urows.Id = appr.Id
	                                left outer join SubApprovalsNomenclatures as nom on nom.IdDoc = appr.Id
	                                where nom.IdDoc  is null
                                )
                                delete from Approvals
                                output deleted.Id,1,deleted.DocumentType,deleted.DateFrom,deleted.DateTo,current_timestamp  into @updatedRows
                                where Id in (select Id from apprDeletePrepare);

                                select * from @updatedRows;";


        private void raiseApprovalsUpdated(IEnumerable<ApprovalsUpdateResult> updates)
            {
            if (updates != null && OnApprovalsUpdated != null)
                {
                OnApprovalsUpdated(updates);
                }
            }
        /// <summary>
        /// Выполняет удаление номенклатуры из разрешительных, которые имеют тип документа помеченный на автоматическое удаление номенклатуры при изменении ТК
        /// </summary>
        /// <param name="nomenclatureId"></param>
        public void RemoveNomenclatureFromSomeApprovals(long nomenclatureId)
            {
            bool isInCurrentTransaction = TransactionManager.TransactionManagerInstance.IsInTransaction();
            try
                {
                if (!isInCurrentTransaction)
                    {
                    TransactionManager.TransactionManagerInstance.BeginBusinessTransaction();
                    }
                executeRemove(nomenclatureId);
                }
            finally
                {
                if (!isInCurrentTransaction)
                    {
                    TransactionManager.TransactionManagerInstance.CompleteBusingessTransaction();
                    }
                }
            this.raiseApprovalsUpdated(this.makeResult());
            }

        /// <summary>
        /// Выполняет удаление
        /// </summary>
        private void executeRemove(long nomenclatureId)
            {
            updatedApprovals.Clear();
            deletedApprovals.Clear();
            Query query = DB.NewQuery(queryText);
            query.AddInputParameter("nomenclatureId", nomenclatureId);
            query.Foreach((row) => this.processRow(row, nomenclatureId));
            }

        /// <summary>
        /// Формирует результат об удалении номенклатуры из РД и удалении самих РД
        /// </summary>
        private IEnumerable<ApprovalsUpdateResult> makeResult()
            {
            List<ApprovalsUpdateResult> result = new List<ApprovalsUpdateResult>();
            foreach (long approval in updatedApprovals.Distinct())
                {
                ApprovalsUpdateKind updateKind = ApprovalsUpdateKind.RemovedRow;
                if (deletedApprovals.Contains(approval))
                    {
                    updateKind = ApprovalsUpdateKind.Deleted;
                    }
                result.Add(new ApprovalsUpdateResult(approval, updateKind));
                }
            return result;
            }

        /// <summary>
        /// Обрабатывает строку с результатом об удалении РД
        /// </summary>
        private void processRow(QueryResult rowResult, long nomenclatureId)
            {
            //сохраняем удаленные/измененные разрешительные
            long approvalId = (long)rowResult[approvalIdInResultColumnName];
            bool isDeleted = (bool)rowResult[approvalUpdateKindColumnName];
            if (isDeleted)
                {
                deletedApprovals.Add(approvalId);
                }
            else
                {
                updatedApprovals.Add(approvalId);
                }
            //сохраняем инфу в справочник
            this.addToRemoveHistory(rowResult, nomenclatureId);
            }

        /// <summary>
        /// Записывает в справочник об истории удаленных номенклатур информацию о разрешительном из которого была удалена номенклатура
        /// </summary>
        private void addToRemoveHistory(QueryResult rowResult, long nomenclatureId)
            {
            long documentTypeId = (long)rowResult[documentTypeColumnName];
            DateTime dateFrom = (DateTime)rowResult[dateFromColumnName];
            DateTime dateTo = (DateTime)rowResult[dateToColumnName];
            DateTime removingTime = (DateTime)rowResult[removeDateColumnName];
            NomenclatureApprovalsRemovingHistory removingHistory = new NomenclatureApprovalsRemovingHistory();
            removingHistory.Nomenclature = new Nomenclature() { Id = nomenclatureId };
            removingHistory.DocumentType = A.New<IDocumentType>(documentTypeId);
            removingHistory.DateFrom = dateFrom;
            removingHistory.DateTo = dateTo;
            removingHistory.RemovingDate = removingTime;
            var result = removingHistory.Write();
            }
        }
    }
