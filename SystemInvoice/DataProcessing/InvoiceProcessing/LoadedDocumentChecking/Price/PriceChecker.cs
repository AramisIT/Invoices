using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.Helpers;
using SystemInvoice.Excel;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureCheck;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;
using SystemInvoice.DataProcessing.Cache.ContractorsCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.Price
    {
    /// <summary>
    /// Проверяет соответствие цены значению сохраненному в базе для номенклатуры
    /// </summary>
    public class PriceChecker : LoadedDocumentCheckerBase
        {
        string priceColumnName = ProcessingConsts.ColumnNames.PRICE_COLUMN_NAME;

        public PriceChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            }

        protected override void CheckRow(DataRow rowToCheck, ExcelMapper mapper, bool isDocumentCurrentlyLoaded, string currentCheckedColumnName)
            {
            long nomenclatureId = Helpers.InvoiceDataRetrieveHelper.GetRowNomenclatureId(rowToCheck);
            NomenclatureCacheObject nomenclatureCacheObject = null;
            if (nomenclatureId != 0)
                {
                nomenclatureCacheObject = dbCache.NomenclatureCacheObjectsStore.GetCachedObject(nomenclatureId);
                if (nomenclatureCacheObject != null)
                    {
                    ContractorCacheObject contractorInfo = dbCache.ContractorCacheObjectsStore.GetCachedObject(nomenclatureCacheObject.ContractorId);
                    if (contractorInfo == null)
                        {
                        return;
                        }
                    double currentPrice = Helpers.InvoiceDataRetrieveHelper.GetRowPrice(rowToCheck);
                    double cachedPrice = nomenclatureCacheObject.Price;
                    if (contractorInfo.UseComodityPrices)
                        {
                        this.checkComodityPrices(currentPrice, cachedPrice);
                        }
                    else
                        {
                        this.checkEqualityPrices(currentPrice, cachedPrice);
                        }
                    }
                }
            }
        /// <summary>
        /// Проверяет равенство цен
        /// </summary>
        private void checkEqualityPrices(double currentPrice, double cachedPrice)
            {
            if (!currentPrice.Equals(cachedPrice))
                {
                this.AddError(priceColumnName, new PriceCheckError(currentPrice.ToString(), cachedPrice.ToString(), priceColumnName, false));
                }
            }
        /// <summary>
        /// Проверяет что цена не меньше цены в базе (необходимо для биржевых цен)
        /// </summary>
        private void checkComodityPrices(double currentPrice, double cachedPrice)
            {
            if (currentPrice < cachedPrice)
                {
                this.AddError(priceColumnName, new PriceCheckError(currentPrice.ToString(), cachedPrice.ToString(), priceColumnName, true));
                }
            }

        }
    }
