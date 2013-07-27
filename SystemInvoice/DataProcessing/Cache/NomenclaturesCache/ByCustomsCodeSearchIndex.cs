using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.NomenclaturesCache
    {
    /// <summary>
    /// Индекс, используемый для быстрого поиска любой номенклатуры соответствующей заданному таможенному коду
    /// </summary>
    public class ByCustomsCodeSearchIndex : IEqualityComparer<NomenclatureCacheObject>
        {
        public bool Equals(NomenclatureCacheObject x, NomenclatureCacheObject y)
            {
            return x.CustomsCodeId.Equals(y.CustomsCodeId);
            }

        public int GetHashCode(NomenclatureCacheObject obj)
            {
            return obj.CustomsCodeId.GetHashCode();
            }
        }
    }
