using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.InvoiceProcessing.Helpers;
using SystemInvoice.Documents;
using System.Data;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.GroupItemsEditors
    {
    /// <summary>
    /// Выполняет расчет относительного значения веса нетто, используется для установки значений пропорциональных весу нетто
    /// </summary>
    public abstract class NomenclatureBasedCalulator
        {
        protected const string NOMENCLATURE_ID_COLUMN_NAME = "FoundedNomenclature";
        protected const string NOMENCLATURE_COUNT_COLUMN_NAME = "Count";
        protected const string NOMENCLATURE_NET_WEIGHT_COLUMN_NAME = "NetWeight";
        protected IEditableRowsSource EditableRowsSource = null;
        protected SystemInvoiceDBCache dbCache = null;

        public NomenclatureBasedCalulator(IEditableRowsSource editableRowsSource, SystemInvoiceDBCache dbCache)
            {
            this.dbCache = dbCache;
            this.EditableRowsSource = editableRowsSource;
            }
        /// <summary>
        /// Возвращант - нужно ли обрабатывать только загруженные строки или строки с ошибками также
        /// </summary>
        public abstract bool ProcessLoadedOnly { get; }

        /// <summary>
        /// Устанавливает значение веса
        /// </summary>
        /// <param name="totalNetWeight">Распределяемое значение</param>
        public void SetWeight(double totalNetWeight)
            {
            if (!updateValues(totalNetWeight))
                {
                "Устанавливаемое значение не может быть меньше нуля.".AlertBox();
                }
            }

        /// <summary>
        /// Возвращает данные о текущем значении веса нетто и о допустимых устанавливаемых значениях
        /// </summary>
        /// <returns></returns>
        private Dictionary<long, NetWeightsInfo> getNetWeightsInfo()
            {
            //double realTotalSumm = 0;
            Dictionary<long, NetWeightsInfo> netWeights = new Dictionary<long, NetWeightsInfo>();
            foreach (DataRow row in EditableRowsSource.DisplayingRows)
                {
                long id = InvoiceDataRetrieveHelper.GetRowLineNumber(row);
                long nomenclatureId = InvoiceDataRetrieveHelper.GetRowNomenclatureId(row);
                if (nomenclatureId == 0 && ProcessLoadedOnly)
                    {
                    continue;
                    }
                double netWeight = InvoiceDataRetrieveHelper.GetRowNetWeight(row);
                if (double.IsNaN(netWeight))
                    {
                    netWeight = 0.0;
                    }
                double netWeightFrom = InvoiceDataRetrieveHelper.GetNomenclatureNetWeightFrom(dbCache, row);
                double netWeightTo = InvoiceDataRetrieveHelper.GetNomenclatureNetWeightTo(dbCache, row);
                int count = InvoiceDataRetrieveHelper.GetNomenclaturesCount(row);
                NetWeightsInfo nwInfo = new NetWeightsInfo(netWeightFrom * count, netWeightTo * count, netWeight, count);
                if (netWeights.ContainsKey(id))
                    {
                    netWeights[id] += nwInfo;
                    }
                else
                    {
                    netWeights.Add(id, nwInfo);
                    }
                //realTotalSumm += netWeight;
                }
            return netWeights;
            }

        /// <summary>
        /// Возвращает данные о колличестве номенклатурных единиц для каждой строки.
        /// 
        /// </summary>
        private Dictionary<long, int> getNetWeightsCount()
            {
            Dictionary<long, int> nomenclaturesCount = new Dictionary<long, int>();
            foreach (DataRow row in EditableRowsSource.DisplayingRows)
                {
                long id = InvoiceDataRetrieveHelper.GetRowLineNumber(row);
                long nomenclatureId = InvoiceDataRetrieveHelper.GetRowNomenclatureId(row);
                if (nomenclatureId == 0 && ProcessLoadedOnly)
                    {
                    continue;
                    }
                int currentCount = InvoiceDataRetrieveHelper.GetNomenclaturesCount(row);
                if (nomenclaturesCount.ContainsKey(id))
                    {
                    nomenclaturesCount[id] += currentCount;
                    }
                else
                    {
                    nomenclaturesCount.Add(id, currentCount);
                    }
                }
            return nomenclaturesCount;
            }

        private bool updateValues(double distributedAmount)
            {
            Dictionary<long, NetWeightsInfo> netWeights = getNetWeightsInfo();
            Dictionary<long, double> arranged = getArrangedValues(distributedAmount, netWeights);
            return this.setArrangedValues(arranged);
            }
        /// <summary>
        /// Возвращает значения распределенные по строкам таблицы
        /// </summary>
        /// <param name="totalAmount">значения которое нужно распределить</param>
        /// <param name="netWeights">Текущие значения по весу нетто</param>
        /// <returns>Распределенные значения</returns>
        private Dictionary<long, double> getArrangedValues(double totalAmount, Dictionary<long, NetWeightsInfo> netWeights)
            {
            NetWeightsInfo totalRange = this.getNetWeightsTotalInfo(netWeights);
            if (totalRange == null)// || totalRange.CurrentWeight == 0)
                {
                return null;
                }
            return Arrange(totalAmount, netWeights, totalRange);
            }

        /// <summary>
        /// распределяет значение веса по строкам
        /// </summary>
        protected virtual Dictionary<long, double> Arrange(double totalAmount, Dictionary<long, NetWeightsInfo> netWeights, NetWeightsInfo totalRange)
            {
            bool isPlus = totalAmount > 0;
            Dictionary<long, double> arranged = new Dictionary<long, double>();
            double ratio = totalAmount / totalRange.CurrentWeight;
            double totalProportional = 0;
            double setForNomenclatureIdValue = 0;
            double residual = 0;//остаток оторый дополнительно нужно распределять по весу нетто, возникает при округлении или при попытке установить значение выходящее за диапазон
            foreach (KeyValuePair<long, NetWeightsInfo> pair in netWeights)
                {
                long lineNumber = pair.Key;
                NetWeightsInfo netWeightsInfo = pair.Value;
                double proportionalUpdateValue = netWeightsInfo.CurrentWeight * ratio;
                totalProportional += proportionalUpdateValue;
                setForNomenclatureIdValue = proportionalUpdateValue + residual;
                if (isPlus)
                    {
                    if (setForNomenclatureIdValue < 0)
                        {
                        setForNomenclatureIdValue = 0;
                        }
                    }
                else
                    {
                    if (setForNomenclatureIdValue > 0)
                        {
                        setForNomenclatureIdValue = 0;
                        }
                    }
                double diffRes = netWeightsInfo.CalcResidual(setForNomenclatureIdValue);
                setForNomenclatureIdValue -= diffRes;
                residual = proportionalUpdateValue + residual - setForNomenclatureIdValue;//добавляем остаток который возникает при делении с точностью до трех знаков для веса и затем умножении на количество
                arranged.Add(lineNumber, setForNomenclatureIdValue);
                }
            return arranged;
            }

        private NetWeightsInfo getNetWeightsTotalInfo(Dictionary<long, NetWeightsInfo> netWeights)
            {
            NetWeightsInfo total = new NetWeightsInfo(0, 0, 0, 0);
            foreach (NetWeightsInfo nwInfo in netWeights.Values)
                {
                total += nwInfo;
                }
            return total;
            }
        /// <summary>
        /// Записывает распределяемые значения в таблицу
        /// </summary>
        /// <param name="arranged">Распределенные значения</param>
        private bool setArrangedValues(Dictionary<long, double> arranged)
            {
            Dictionary<long, int> netWeights = getNetWeightsCount();
            List<Tuple<DataRow, double>> valuesToSet = new List<Tuple<DataRow, double>>();
            foreach (DataRow row in EditableRowsSource.DisplayingRows)
                {
                long lineNumber = InvoiceDataRetrieveHelper.GetRowLineNumber(row);
                int currentCount = InvoiceDataRetrieveHelper.GetNomenclaturesCount(row);
                int totalNomenclatureCount;
                if (lineNumber > 0 && arranged.ContainsKey(lineNumber) 
                    && netWeights.TryGetValue(lineNumber, out totalNomenclatureCount) && totalNomenclatureCount > 0)
                    {
                    double weight = currentCount * arranged[lineNumber] / totalNomenclatureCount;
                    valuesToSet.Add(new Tuple<DataRow, double>(row, weight));
                    }
                }
            foreach (var item in valuesToSet)
                {
                if (!CheckValue(item.Item1, item.Item2))
                    {
                    return false;
                    }
                }
            foreach (var item in valuesToSet)
                {
                this.SetArrangedValue(item.Item1, item.Item2);
                }
            return true;
            }
        /// <summary>
        /// Устанавливает распределяемое значение
        /// </summary>
        /// <param name="row">строка которой нужно установить значение</param>
        /// <param name="arrangedValue">Распределенное значение</param>
        protected abstract void SetArrangedValue(DataRow row, double arrangedValue);
        /// <summary>
        /// Проверяет возможность установки значения
        /// </summary>
        /// <param name="row">Строка которой нужно установить значение</param>
        /// <param name="arrangedValue">Устанавливаемое значение</param>
        /// <returns>Можно ли установить</returns>
        protected abstract bool CheckValue(DataRow row, double arrangedValue);
        }
    }
