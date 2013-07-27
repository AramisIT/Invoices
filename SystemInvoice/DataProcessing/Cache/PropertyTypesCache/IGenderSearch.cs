using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.PropertyTypesCache
    {
    /// <summary>
    /// Интерфейс для установки поля используемого в поиске пола для группы товара
    /// </summary>
    interface IGenderSearch
        {
        void SetSearchOptions( long SubGroupOfGoodsId );
        }
    }
