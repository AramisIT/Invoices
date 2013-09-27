using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.GroupItemsEditors
    {
    /// <summary>
    /// Содержит информацию о текущем общем весе номенклатурных позиций а так же о допустимом диапазоне значений
    /// </summary>
    public class NetWeightsInfo
        {
        public double MinWeight { get; private set; }
        public double MaxWeight { get; private set; }
        public double CurrentWeight { get; private set; }
        public int Count { get; private set; }

        public NetWeightsInfo(double MinWeight, double MaxWeight, double CurrentWeight, int Count)
            {
            this.MinWeight = Math.Round(MinWeight, 3);
            this.MaxWeight = Math.Round(MaxWeight, 3);
            this.CurrentWeight = Math.Round(CurrentWeight, 3);
            this.Count = Count;
            if (this.MinWeight == 0)
                {
                this.MinWeight = 0.001;
                }
            if (this.MaxWeight < this.MinWeight)
                {
                this.MaxWeight = this.MinWeight;
                }
            }

        public static NetWeightsInfo operator +(NetWeightsInfo first, NetWeightsInfo second)
            {
            if (first == null || second == null)
                {
                return null;
                }
            return new NetWeightsInfo(first.MinWeight + second.MinWeight, first.MaxWeight + second.MaxWeight,
                first.CurrentWeight + second.CurrentWeight, first.Count + second.Count);
            }
        /// <summary>
        /// Рассчитывает разницу между значением которое устанавливается для общего веса нетто и значением которое будет установлено в итого, обусловленную
        /// необходимостью соответствия общего веса - весу единицы умноженому на количество, а так же округлением
        /// </summary>
        /// <param name="totalAmount">Устанавливаемое значение</param>
        /// <returns></returns>
        public double CalcResidual(double totalAmount)
            {
            double unitAmount = Math.Round(totalAmount / Count, 3);
            return totalAmount - unitAmount * Count;
            }
        }
    }
