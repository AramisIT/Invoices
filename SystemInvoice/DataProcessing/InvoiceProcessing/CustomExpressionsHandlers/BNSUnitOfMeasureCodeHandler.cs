using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Возвращает единицу измерения указанную для номенклатуры или, если нету - из входящего файла
    /// </summary>
    public class BNSUnitOfMeasureCodeHandler : CustomExpressionHandlerBase
        {
        public BNSUnitOfMeasureCodeHandler(SystemInvoiceDBCache cachedData)
            : base(cachedData)
            {
            }

        public override object ProcessRow(params object[] parameters)//BNS.ЕдИзм[Артикул,Торговая марка,Ед. изм.]
            {
            if (parameters.Length < 3)
                {
                return null;
                }
            string articleStr = parameters[0].ToString();
            string tradeMarkStr = parameters[1].ToString();
            string unitOfMeasureStr = parameters[2].ToString();
            string nomenclatureUnitOfMeasureCode = "";
            NomenclatureCacheObject cachedNomenclature = catalogsCachedData.GetCachedNomenclature(articleStr, tradeMarkStr);
            if (cachedNomenclature != null)
                {
                nomenclatureUnitOfMeasureCode = catalogsCachedData.GetNomenclatureUnitOfMeasureCode(cachedNomenclature);
                }
            string foundedUnitOfMeasureCode = catalogsCachedData.UnitOfMeasureCacheObjectsStore.GetUnitOfMeasureCodeFromShortName(unitOfMeasureStr);
            if (!string.IsNullOrEmpty(foundedUnitOfMeasureCode))
                {
                return foundedUnitOfMeasureCode;
                }
            return nomenclatureUnitOfMeasureCode;
            }
        }
    }
