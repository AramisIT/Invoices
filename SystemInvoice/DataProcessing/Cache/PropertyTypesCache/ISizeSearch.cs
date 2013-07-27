using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.PropertyTypesCache
    {
    /// <summary>
    /// Интерфейс для установки поля используемого в поиске украинского размера для размера в другой системе и группы товара
    /// </summary>
    interface ISizeSearch
        {
        void SetSearchOptions( long SubGroupOfGoodsId, string enSize);
        }
    }
