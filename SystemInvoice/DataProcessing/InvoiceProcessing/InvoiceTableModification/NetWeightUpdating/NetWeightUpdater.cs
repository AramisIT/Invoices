using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.Helpers;
using SystemInvoice.Documents;

namespace SystemInvoice.DataProcessing.InvoiceProcessing
    {
    /// <summary>
    /// Устанавливает вес нетто таким образом что бы он был в диапазоне заданном в номенклатуре,
    /// или в диапазоне для первой попавшейся с таким же таможенным кодом номенклатуры, если это новая номенклатура
    /// </summary>
    public class NetWeightUpdater
    {
        private SystemInvoiceDBCache cachedData = null;

        public NetWeightUpdater(SystemInvoiceDBCache cachedData)
            {
            this.cachedData = cachedData;
            }

        public void MakeNetWeightReplacement(DataTable tableToUpdate)
            {
            foreach (DataRow row in tableToUpdate.Rows)
                {
                long nomenclatureId = InvoiceDataRetrieveHelper.GetRowNomenclatureId(row);
                double netWeight = InvoiceDataRetrieveHelper.GetRowItemNetWeight(row);
                Tuple<double, double> range = getRange(nomenclatureId, row);
                if (!isInRange(range, netWeight))
                    {
                    this.setInRange(range, netWeight, row);
                    }
                }
            }

        /// <summary>
        /// Ищет допустимый диапазон для веса нетто в номенклатуре или таможенном коде
        /// </summary>
        /// <param name="nomenclatureId">Айди номенклатуры</param>
        /// <param name="row">Строка</param>
        private Tuple<double, double> getRange(long nomenclatureId, DataRow row)
            {
            Tuple<double, double> range = null;
            if (nomenclatureId > 0)
                {
                range = getNetWeightRangeForNomenclature(nomenclatureId);
                }
            else
                {
                range = getNetWeightRangeForRowCustomsCode(row);
                }
            return range;
            }

        /// <summary>
        /// Возвращает диапазон веса нетто для номенклатуры
        /// </summary>
        private Tuple<double, double> getNetWeightRangeForNomenclature(long nomenclatureId)
            {
            return cachedData.NomenclatureCacheObjectsStore.GetNetWeightRangeForNomenclature(nomenclatureId);
            }

        /// <summary>
        /// Возвращает диапазон веса нетто для таможенного кода
        /// </summary>
        private Tuple<double, double> getNetWeightRangeForRowCustomsCode(DataRow row)
            {
            string customsCode = InvoiceDataRetrieveHelper.GetRowCustomsCode(row);
            long customsCodeId = cachedData.CustomsCodesCacheStore.GetCustomsCodeIdForCodeName(customsCode);
            return cachedData.NomenclatureCacheObjectsStore.GetNetWeightRangeForCustomsCode(customsCodeId);
            }

        /// <summary>
        /// Устанавливает значение веса нетто для номенклатуры, таким, которое попадает в допутимый диапазон
        /// </summary>
        /// <param name="range">Допустимый диапазон</param>
        /// <param name="netWeight">Текущий вес нетто</param>
        /// <param name="row">Строка</param>
        private void setInRange(Tuple<double, double> range, double netWeight, DataRow row)
            {
            if (double.IsNaN(netWeight) || range == null)
                {
                return;
                }
            double valueToSet = netWeight;
            if (range.Item1 > netWeight)
                {
                valueToSet = range.Item1;
                }
            if (range.Item2 < netWeight)
                {
                valueToSet = range.Item2;
                }
            string strValue = Math.Round(valueToSet, 3).ToString();
            if (!row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.ITEM_NET_WEIGHT_COLUMN_NAME, string.Empty).Equals(strValue))
                {
                row[ProcessingConsts.ColumnNames.ITEM_NET_WEIGHT_COLUMN_NAME] = strValue;
                }
            }

        private bool isInRange(Tuple<double, double> range, double netWeight)
            {
            if (double.IsNaN(netWeight) || range == null)
                {
                return true;
                }
            return range.Item1 <= netWeight && netWeight <= range.Item2;
            }
        }
    }
