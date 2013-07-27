using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
//using SystemInvoice.DataProcessing.Cache.PropertyTypesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Получает перевод значения
    /// </summary>
    class TranslationHandler : CustomExpressionHandlerBase
        {
        public TranslationHandler(SystemInvoiceDBCache cachedData)
            : base(cachedData)
            {
            }

        public override object ProcessRow(params object[] parameters)//Перевод[Значение]
        {

        //string tradeMark = string.Empty;
        //string contractor = string.Empty;
        //string groupOfGoods = string.Empty;
        //string subGroupOfGoods = string.Empty;
        //string article = string.Empty;
            string valueToTranslate = "";
            if (parameters.Length < 1)
            {
                return string.Empty;
            }
            valueToTranslate = parameters[0].ToString();
            long propertyTypeId = catalogsCachedData.PropertyOfGoodsCacheObjectsStore.GetCachedObjectId("Перевод");
            string propertyTranslatedValue = catalogsCachedData.PropertyTypesCacheObjectsStore.GetPropertyUKValue(0,valueToTranslate,0,propertyTypeId);
            if (string.IsNullOrEmpty(propertyTranslatedValue))
                {
                return valueToTranslate;
                }
            return propertyTranslatedValue;
            }
        }
    }
