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
    /// Рассчитывает вес брутто
    /// </summary>
    public class GrossWeightsCalculator : NomenclatureBasedCalulator
        {
        private const string NOMENCLATURE_GROSS_WEIGHT_COLUMN_NAME = "ItemGrossWeight";

        public GrossWeightsCalculator(IEditableRowsSource editableRowsSource, SystemInvoiceDBCache dbCache)
            : base(editableRowsSource, dbCache)
            {
            }

        protected override void SetArrangedValue(System.Data.DataRow row, double arrangedValue)
            {
            string valueOld = row.TryGetColumnValue<string>(NOMENCLATURE_GROSS_WEIGHT_COLUMN_NAME, "");
            double oldValue = 0;
            double.TryParse(valueOld, out oldValue);
            double valueToSet = Math.Round(arrangedValue + oldValue, 3);
            row[NOMENCLATURE_GROSS_WEIGHT_COLUMN_NAME] = valueToSet.ToString();
            }

        /// <summary>
        /// Проверяем что б итоговое значение не вышло меньше нуля
        /// </summary>
        protected override bool CheckValue(DataRow row, double arrangedValue)
            {
            string valueOld = row.TryGetColumnValue<string>(NOMENCLATURE_GROSS_WEIGHT_COLUMN_NAME, "");
            double oldValue = 0;
            if (double.TryParse(valueOld, out oldValue))
                {
                return (oldValue + arrangedValue) > 0 || (oldValue == 0 && arrangedValue == 0);
                }
            return arrangedValue >= 0;
            }

        /// <summary>
        /// Очищает все значения веса брутто для таблицы
        /// </summary>
        public void ClearGrossWeight()
            {
            List<DataRow> rowsToClear = this.EditableRowsSource.DisplayingRows.ToList();
            foreach (DataRow row in rowsToClear)
                {
                row[NOMENCLATURE_GROSS_WEIGHT_COLUMN_NAME] = "";
                }
            }

        public override bool ProcessLoadedOnly
            {
            get { return false; }
            }
        }
    }
