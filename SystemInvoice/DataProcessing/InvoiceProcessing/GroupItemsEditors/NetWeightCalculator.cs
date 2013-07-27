using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Documents;
using System.Data;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.GroupItemsEditors
    {
    /// <summary>
    /// Распределяет все нетто, без учета верхней и нижней границы весов в номенклатуре
    /// </summary>
    public class NetWeightCalculator : NomenclatureBasedCalulator
        {
        private const string UNIT_NET_WEIGHT_COLUMN_NAME = "UnitWeight";

        public NetWeightCalculator(IEditableRowsSource editableRowsSource, SystemInvoiceDBCache dbCache)
            : base(editableRowsSource, dbCache)
            {
            }

        protected override void SetArrangedValue(System.Data.DataRow row, double arrangedValue)
            {
            double valueToSet = arrangedValue;
            int count = Helpers.InvoiceDataRetrieveHelper.GetNomenclaturesCount(row);
            if (count > 0)
                {
                string valueOld = row.TryGetColumnValue<string>(NOMENCLATURE_NET_WEIGHT_COLUMN_NAME, "");
                double oldValue = 0;
                double.TryParse(valueOld, out oldValue);
                valueToSet = Math.Round(arrangedValue + oldValue, 3);
                //устанавливаем значение для веса единицы товара
                double unitNetWeight = Math.Round(valueToSet / count, 3);
                row[UNIT_NET_WEIGHT_COLUMN_NAME] = unitNetWeight.ToString();
                }//устаначливаем значение для общего веса в строке
            row[NOMENCLATURE_NET_WEIGHT_COLUMN_NAME] = valueToSet.ToString();
            }
        /// <summary>
        /// Проверяем что б итоговое значение не вышло меньше нуля
        /// </summary>
        protected override bool CheckValue(DataRow row, double arrangedValue)
            {
            string valueOld = row.TryGetColumnValue<string>(NOMENCLATURE_NET_WEIGHT_COLUMN_NAME, "");
            double oldValue = 0;
            if (double.TryParse(valueOld, out oldValue))
                {
                return (oldValue + arrangedValue) > 0 || (oldValue == 0 && arrangedValue == 0);
                }
            return arrangedValue > 0;
            }

        public override bool ProcessLoadedOnly
            {
            get { return false; }
            }
        }
    }
