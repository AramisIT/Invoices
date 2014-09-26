using System.Collections.Generic;
using System.Data;
using SystemInvoice.DataProcessing.ApprovalsProcessing.ByNomenclatureUpdating;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.Documents;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.ApprovalsModification
    {
    /// <summary>
    /// Обноавляет табличную часть инвойса, удаляя записи о разрешительных рокументах, из табличных частей которых была удалена номенклатура в процессе обработки инвойса
    /// </summary>
    public class InvoiceAfterAprovalsUpdater
        {
        private const string replaceStr = " ";
        private Invoice invoice = null;
        /// <summary>
        /// Хранит информацию о РД в документе
        /// </summary>
        private ApprovalsDictionary approvalsDictionary = new ApprovalsDictionary();
        private ApprovalsSearcher searcher = null;

        public InvoiceAfterAprovalsUpdater(Invoice invoice, SystemInvoiceDBCache dbCache)
            {
            this.invoice = invoice;
            this.searcher = new ApprovalsSearcher(dbCache, invoice);
            }

        /// <summary>
        /// Обновляет информацию о текущем расположении разрешительных в документе
        /// </summary>
        public void RefreshApprovals()
            {
            DataTable tableToProcess = this.invoice.Goods;
            approvalsDictionary.Clear();
            List<ApprovalPosition> allPositions = new List<ApprovalPosition>();
            foreach (DataRow row in tableToProcess.Rows)
                {
                this.appendToAll(allPositions, row);
                }
            this.appendPositions(allPositions);
            }

        private void appendToAll(List<ApprovalPosition> allPositions, DataRow row)
            {
            for (int i = 1; i <= ProcessingConsts.CHECKING_APPROVALS_COUNT; i++)
                {
                ApprovalPosition position = getApprovalPosition(row, i);
                if (position != null)
                    {
                    allPositions.Add(position);
                    }
                }
            }

        private ApprovalPosition getApprovalPosition(DataRow row, int i)
            {
            string foundedApprovalColumnName = this.getFoundedApprovalColumnName(i);
            long approvalId = row.TrySafeGetColumnValue<long>(foundedApprovalColumnName, 0);
            if (approvalId != 0)
                {
                return new ApprovalPosition(row, i, approvalId);
                }
            return null;
            }

        private void appendPositions(List<ApprovalPosition> rowPositions)
            {
            foreach (ApprovalPosition position in rowPositions)
                {
                ApprovalsCollection approvalsCollection = null;
                if (!approvalsDictionary.TryGetValue(position.ApprovalId, out approvalsCollection))
                    {
                    approvalsCollection = new ApprovalsCollection(position.ApprovalId);
                    approvalsDictionary.Add(position.ApprovalId, approvalsCollection);
                    }
                approvalsCollection.Add(position);
                }
            }

        /// <summary>
        /// Обновляет (удаляет) информацию о РД из Табличной части инвойса
        /// </summary>
        /// <param name="updates">Информация об изменениях в рд</param>
        public void RefreshApprovals(IEnumerable<ApprovalsUpdateResult> updates)
            {
            //если не было в документе информации об РД - обновляем ее что бы значть что удалять
            if (approvalsDictionary.Count == 0)
                {
                RefreshApprovals();
                }
            //удаляем РД
            foreach (ApprovalsUpdateResult updateResult in updates)
                {
                this.refreshApproval(updateResult);
                }
            // Вообще это старый метод который изначально все обновлял, но его нужно протестить
            //поскольку он весьма медленный, и то что вызывается перед ним адекватно все обновляет - его нужно удалить
            searcher.FindApprovals(this.invoice.Goods);
            }

        private void refreshApproval(ApprovalsUpdateResult updateResult)
            {
            ApprovalsCollection approvalsCollection = null;
            if (approvalsDictionary.TryGetValue(updateResult.ApprovalId, out approvalsCollection))
                {
                this.deleteApprovalsFromDocument(approvalsCollection);
                }
            }

        private void deleteApprovalsFromDocument(ApprovalsCollection approvalsCollection)
            {
            foreach (ApprovalPosition position in approvalsCollection)
                {
                DataRow row = position.DataRow;
                int index = position.Index;
                this.removeApprovalFromRow(row, index);
                }
            }

        private void removeApprovalFromRow(DataRow row, int index)
            {
            string rdCodeColumnName = getRDCodeColumnName(index);
            string rdNumberColumnName = getRDNumberColumnName(index);
            string rdDateFromColumnName = getRDDateFromColumnName(index);
            string rdDateToColumnName = getRDDateToColumnName(index);
            string approvalIdColumnName = getFoundedApprovalColumnName(index);
            row[rdCodeColumnName] = replaceStr;
            row[getRDBaseNumberFromColumnName(index)] = replaceStr;
            row[rdNumberColumnName] = replaceStr;
            row[rdDateFromColumnName] = replaceStr;
            row[rdDateToColumnName] = replaceStr;
            row[approvalIdColumnName] = 0;
            }

        private string getRDCodeColumnName(int i)
            {
            return string.Concat("RDCode", i);
            }

        private string getRDNumberColumnName(int i)
            {
            return string.Concat("RDNumber", i);
            }

        private string getRDDateFromColumnName(int i)
            {
            return string.Concat("RDFromDate", i);
            }

        private string getRDBaseNumberFromColumnName(int i)
            {
            return string.Format("RD{0}BaseNumber", i);
            }

        private string getRDDateToColumnName(int i)
            {
            return string.Concat("RDToDate", i);
            }

        private string getFoundedApprovalColumnName(int i)
            {
            return string.Concat("FoundedApprovals", i);
            }
        }

    public class ApprovalPosition
        {
        public DataRow DataRow { get; private set; }
        public int Index { get; private set; }
        public long ApprovalId { get; private set; }
        public ApprovalPosition(DataRow dataRow, int index, long approvalId)
            {
            this.DataRow = dataRow;
            this.Index = index;
            this.ApprovalId = approvalId;
            }
        }

    public class ApprovalsCollection : List<ApprovalPosition>
        {
        public long ApprovalId { get; private set; }

        public ApprovalsCollection(long approvalId)
            {
            this.ApprovalId = approvalId;
            }
        }

    public class ApprovalsDictionary : Dictionary<long, ApprovalsCollection>
        {

        }
    }
