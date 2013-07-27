namespace SystemInvoice.DataProcessing.ApprovalsProcessing.ByNomenclatureUpdating
    {
    /// <summary>
    /// Хранит информацию об обновлении РЛ при удалении из него номенклатуры
    /// </summary>
    public class ApprovalsUpdateResult
        {
        public long ApprovalId { get; private set; }
        public ApprovalsUpdateKind UpdateKind { get; private set; }
        public ApprovalsUpdateResult(long approvalId, ApprovalsUpdateKind updateKind)
            {
            this.ApprovalId = approvalId;
            this.UpdateKind = updateKind;
            }
        }
    }
