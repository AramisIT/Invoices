using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.GroupItemsEditors
    {
    /// <summary>
    /// Распределяет вес нетто с учетом допустимых границ веса в справочнике номенклатура
    /// </summary>
    public class NetWeightWithBoundaryCheckingCalculator : NetWeightCalculator
        {


        public NetWeightWithBoundaryCheckingCalculator(IEditableRowsSource editableRowsSource, SystemInvoiceDBCache dbCache)
            : base(editableRowsSource, dbCache)
            {
            }
        /// <summary>
        /// Распределяются только по тем строкам по которым была найдена номенклатура
        /// </summary>
        public override bool ProcessLoadedOnly
            {
            get { return true; }
            }

        protected override Dictionary<long, double> Arrange(double totalAmount, Dictionary<long, NetWeightsInfo> netWeights, NetWeightsInfo totalRange)
            {
            double cannotUpdatePart = 0;
            bool isPlus = totalAmount > 0;
            double haveToUpdate = totalAmount * (isPlus ? 1 : -1);
            double avialableToUpdate = getAvialableToUpdateAbsoluteValue(totalRange, isPlus);
            if (avialableToUpdate <= 0)
                {
                cannotUpdatePart = totalAmount;
                return base.Arrange(0, netWeights, totalRange);
                }
            Dictionary<long, double> arranged = new Dictionary<long, double>();
            double ratio = totalAmount / totalRange.CurrentWeight;
            if (haveToUpdate > avialableToUpdate)
                {
                cannotUpdatePart = round(haveToUpdate - avialableToUpdate);
                arrangeAllToMaxOrMinValues(arranged, netWeights, isPlus);
                }
            else
                {
                arrangeWithCheckingForMinOrMax(arranged, netWeights, isPlus, ratio);
                }
            return arranged;
            }

        /// <summary>
        /// Рассчитывает распределение с учетом границ
        /// </summary>
        /// <param name="arranged">Словарь (номер строки - значение) в который записывается распределяемое значение</param>
        /// <param name="netWeights">Информация о допустимых границах и текущем значении в строках по которым нужно распределить вес</param>
        /// <param name="isPlus">Добавляется ли вес или отнимается</param>
        /// <param name="ratio">Относительное значение на которое нужно изменить общий вес</param>
        private void arrangeWithCheckingForMinOrMax(Dictionary<long, double> arranged, Dictionary<long, NetWeightsInfo> netWeights, bool isPlus, double ratio)
            {
            double totalProportional = 0;
            double setForNomenclatureIdValue = 0;
            double residual = 0;//остаток оторый дополнительно нужно распределять по весу нетто, возникает при округлении или при попытке установить значение выходящее за диапазон
            foreach (KeyValuePair<long, NetWeightsInfo> pair in netWeights)
                {
                long nomenclatureId = pair.Key;
                NetWeightsInfo netWeightsInfo = pair.Value;
                double proportionalUpdateValue = netWeightsInfo.CurrentWeight * ratio;
                totalProportional += proportionalUpdateValue;
                if (isPlus)
                    {
                    double allowedDiff = round(netWeightsInfo.MaxWeight - netWeightsInfo.CurrentWeight);
                    setForNomenclatureIdValue = round(Math.Min(allowedDiff, proportionalUpdateValue + residual));
                    if (setForNomenclatureIdValue < 0)
                        {
                        setForNomenclatureIdValue = 0;
                        }
                    }
                else
                    {
                    double allowedDiff = round(netWeightsInfo.MinWeight - netWeightsInfo.CurrentWeight);
                    setForNomenclatureIdValue = round(Math.Max(allowedDiff, proportionalUpdateValue + residual));
                    if (setForNomenclatureIdValue > 0)
                        {
                        setForNomenclatureIdValue = 0;
                        }
                    }
                double diffRes = netWeightsInfo.CalcResidual(setForNomenclatureIdValue);
                setForNomenclatureIdValue -= diffRes;
                residual = proportionalUpdateValue + residual - setForNomenclatureIdValue;//добавляем остаток который возникает при делении с точностью до трех знаков для веса и затем умножении на колличество
                arranged.Add(nomenclatureId, setForNomenclatureIdValue);
                }
            }
        /// <summary>
        /// Устанавливает для всех строк максимальное или минимальное допустимое значение
        /// </summary>
        private void arrangeAllToMaxOrMinValues(Dictionary<long, double> arranged, Dictionary<long, NetWeightsInfo> netWeights, bool isPlus)
            {
            foreach (KeyValuePair<long, NetWeightsInfo> pair in netWeights)
                {
                if (isPlus)
                    {
                    arranged.Add(pair.Key, round(pair.Value.MaxWeight - pair.Value.CurrentWeight));
                    }
                else
                    {
                    arranged.Add(pair.Key, round(pair.Value.MinWeight - pair.Value.CurrentWeight));
                    }
                }
            }
        /// <summary>
        /// Рассчитвает значение на которое можно изменить вес нетто
        /// </summary>
        /// <param name="totalRange">Допустимы диапазон с текущим значением</param>
        /// <param name="isPlus">Отнимаем или добавляем</param>
        /// <returns>Значение</returns>
        private double getAvialableToUpdateAbsoluteValue(NetWeightsInfo totalRange, bool isPlus)
            {
            double avialableToUpdate = 0;
            if (isPlus)
                {
                avialableToUpdate = round(totalRange.MaxWeight - totalRange.CurrentWeight);
                }
            else
                {
                avialableToUpdate = round(totalRange.CurrentWeight - totalRange.MinWeight);
                }
            return avialableToUpdate;
            }

        private double round(double val)
            {
            return Math.Round(val, 3);
            }
        /// <summary>
        /// Распределяет вес
        /// </summary>
        /// <param name="updateAmount">Велечина на которую нужно распределить</param>
        public void UpdateNetWeights(double updateAmount)
            {
            base.SetWeight(updateAmount);
            }
        }
    }
