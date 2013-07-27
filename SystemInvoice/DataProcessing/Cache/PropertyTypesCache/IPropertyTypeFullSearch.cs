using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.PropertyTypesCache
    {
    /// <summary>
    /// Интерфейс для установки полей используемых про поиске по индексу FullCorrespondanceIndex
    /// </summary>
    interface IPropertyTypeFullSearch
        {
        void SetSearchOptions(long propertyTypeId, string propertyValue, long nomenclatureId, long subGroupOfGoodsId, long groupOfGoodsId, long tradeMarkId, long contractorId);
        }
    }
