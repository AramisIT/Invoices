using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.ApprovalsCache
    {
    /// <summary>
    /// Индекс для поиска РД действующих для номенклатуры в определенную дату
    /// </summary>
    public class ApprovalsSearchIndex : IEqualityComparer<ApprovalsCacheObject>
        {
        public bool Equals(ApprovalsCacheObject x, ApprovalsCacheObject y)
            {
            return x.NomenclatureId.Equals(y.NomenclatureId) && x.SearchedDate.Equals(y.SearchedDate);
            }

        public int GetHashCode(ApprovalsCacheObject obj)
            {
            return obj.SearchedDate.GetHashCode() ^ obj.NomenclatureId.GetHashCode();
            }
        }
    }
