using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers.Zara
    {
    /// <summary>
    /// Получает штрих-код из загруженной номенклатуры или из фходящего файла
    /// </summary>
    public class BarCodeHandler : FromArticleHandlerBase
        {
        public BarCodeHandler( SystemInvoiceDBCache cachedData )
            : base( cachedData )
            {
            }

        protected override object ProcessExpression(NomenclatureCacheObject cachedObject)//Артикул.ШтрихКод[Артикул,Торговая марка] Артикул.ШтрихКод[Артикул,Торговая марка,1]
            {
            return cachedObject.BarCode;
            }
        }
    }
