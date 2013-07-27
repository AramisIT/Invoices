using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.NomenclatureRemovingFromApprovalsHistoryCache
    {
    /// <summary>
    /// Индекс для получения типов документов встречающихся в РД из которых была удалена номенклатура, для заданной номенклатуры на определенную дату
    /// </summary>
    public class RequiredApprovalIndex : IEqualityComparer<NomenclatureRemovingHistoryCacheObject>
        {
        public bool Equals(NomenclatureRemovingHistoryCacheObject x, NomenclatureRemovingHistoryCacheObject y)
            {
            return x.SearchedDate.Equals(y.SearchedDate) && x.NomenclatureId.Equals(y.NomenclatureId);
            }

        public int GetHashCode(NomenclatureRemovingHistoryCacheObject obj)
            {
            return obj.NomenclatureId.GetHashCode() ^ obj.SearchedDate.GetHashCode();
            }
        }
    }
