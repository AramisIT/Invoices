using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.ApprovalsCache
    {
    /// <summary>
    /// Используется для индексирования РД по шапке РД (без учета номенклатуры), для проверки того нужно ли создавать новый РД или добавлять запись в существующую табличную часть РД
    /// </summary>
    public class ApprovalsWithoutNomenclatureIndex : IEqualityComparer<ApprovalsCacheObject>
        {
        public bool Equals(ApprovalsCacheObject x, ApprovalsCacheObject y)
            {
            return x.DocumentTypeId.Equals(y.DocumentTypeId) && x.ContractorId.Equals(y.ContractorId) && x.TradeMarkId.Equals(y.TradeMarkId) &&
                 x.DateFrom.Equals(y.DateFrom) && x.DateTo.Equals(y.DateTo) && x.DocumentNumber.Equals(y.DocumentNumber);
            }

        public int GetHashCode(ApprovalsCacheObject obj)
            {
            return obj.DocumentTypeId.GetHashCode() ^ obj.ContractorId.GetHashCode() ^ obj.TradeMarkId.GetHashCode() ^ obj.DateFrom.GetHashCode() ^ obj.DateTo.GetHashCode() ^ obj.DocumentNumber.GetHashCode();
            }
        }
    }
