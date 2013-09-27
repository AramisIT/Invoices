using System;
using System.Data;
using SystemInvoice.Documents;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.GroupItemsEditors
    {

    public interface ITotalsState
        {
        double TotalCount { get; }
        double TotalGrossWeight { get; }
        double TotalNetWeight { get; }
        double TotalNumberOfPlaces { get; }
        double TotalPrice { get; }
        double VisibleTotalCount { get; }
        double VisibleTotalGrossWeight { get; }
        double VisibleTotalNetWeight { get; }
        double VisibleTotalNumberOfPlaces { get; }
        double VisibleTotalPrice { get; }
        }
    /// <summary>
    /// Выполняет расчет итогов в таблицы (отображаются в виде итоговых ячеек).
    /// </summary>
    public class BottomTotalsCalculator : ITotalsState
        {
        private int lastGetVisibleCount = 0;

        private Invoice invoice = null;
        double totalPrice = 0, totalCount = 0, totalNumberOfPlaces = 0,
            totalNetWeight = 0, totalGrossWeight = 0;

        double visibleTotalPrice = 0, visibleTotalCount = 0, visibleTotalNumberOfPlaces = 0,
            visibleTotalNetWeight = 0, visibleTotalGrossWeight = 0;

        private IEditableRowsSource editableRowsSource;

        private void checkStateIsActual()
            {
            int currentCount = editableRowsSource.DisplayingRows.Count;
            if (currentCount != lastGetVisibleCount)
                {
                lastGetVisibleCount = currentCount;
                RefreshTotals();
                }
            }

        public BottomTotalsCalculator(Invoice invoice, IEditableRowsSource editableRowsSourcce)
            {
            this.invoice = invoice;
            this.editableRowsSource = editableRowsSourcce;
            }
        /// <summary>
        /// Возвращает итоговую стоимость для таблицы
        /// </summary>
        public double TotalPrice
            {
            get
                {
                checkStateIsActual();
                return Math.Round(totalPrice, 2);
                }
            }

        /// <summary>
        /// Возвращает итоговую стоимость для отображаемых строк
        /// </summary>
        public double VisibleTotalPrice
            {
            get
                {
                checkStateIsActual();
                return Math.Round(visibleTotalPrice, 2);
                }
            }
        /// <summary>
        /// Возвращает итоговое количество для номенклатуры таблицы
        /// </summary>
        public double TotalCount
            {
            get
                {
                checkStateIsActual();
                return Math.Round(totalCount, 0);
                }
            }

        /// <summary>
        /// Возвращает итоговое количество для отображаемой номенклатуры таблицы
        /// </summary>
        public double VisibleTotalCount
            {
            get
                {
                checkStateIsActual();
                return Math.Round(visibleTotalCount, 0);
                }
            }


        /// <summary>
        /// Возвращает итоговое количество мест
        /// </summary>
        public double TotalNumberOfPlaces
            {
            get
                {
                checkStateIsActual();
                return Math.Round(totalNumberOfPlaces, 0);
                }
            }

        /// <summary>
        /// Возвращает итоговое количество мест для отображаемой номенклатуры
        /// </summary>
        public double VisibleTotalNumberOfPlaces
            {
            get
                {
                checkStateIsActual();
                return Math.Round(visibleTotalNumberOfPlaces, 0);
                }
            }
        /// <summary>
        /// Возвращает итоговый вес нетто
        /// </summary>
        public double TotalNetWeight
            {
            get
                {
                checkStateIsActual();
                return Math.Round(totalNetWeight, 3);
                }
            }
        /// <summary>
        /// Возвращает итоговый вес нетто для отображаемой номенклатуры
        /// </summary>
        public double VisibleTotalNetWeight
            {
            get
                {
                checkStateIsActual();
                return Math.Round(visibleTotalNetWeight, 3);
                }
            }
        /// <summary>
        /// Возвращает итоговый вес брутто
        /// </summary>
        public double TotalGrossWeight
            {
            get
                {
                checkStateIsActual();
                return Math.Round(totalGrossWeight, 3);
                }
            }

        /// <summary>
        /// Возвращает итоговый вес брутто для отображаемой номенклатуры
        /// </summary>
        public double VisibleTotalGrossWeight
            {
            get
                {
                checkStateIsActual();
                return Math.Round(visibleTotalGrossWeight, 3);
                }
            }



        /// <summary>
        /// Обновляет итоговые значения
        /// </summary>
        public void RefreshTotals()
            {
            setGlobalTotals();
            setVisibleTotals();
            //int maxCalcRange = 10000;
            //double realTotalSumm = 0;
            //DataTable table = tableToProcess;
            //for (int i = 0; i < table.Rows.Count ; i++)
            //    {
            //    DataRow row = table.Rows[i];
            //    long id = Helpers.InvoiceDataRetrieveHelper.GetRowNomenclatureId(row);
            //    if (id == 0 || maxCalcRange-- <= 0)
            //        {
            //        continue;
            //        }
            //    double netWeight = Helpers.InvoiceDataRetrieveHelper.GetRowNetWeight(row);
            //    realTotalSumm += netWeight;
            //    }
            }

        private void setVisibleTotals()
            {
            visibleTotalPrice = 0;
            visibleTotalCount = 0;
            visibleTotalNumberOfPlaces = 0;
            visibleTotalNetWeight = 0;
            visibleTotalGrossWeight = 0;
            foreach (DataRow row in editableRowsSource.DisplayingRows)
                {
                visibleTotalPrice += this.getNumericValue(row, "Sum");
                visibleTotalCount += this.getNumericValue(row, "Count");
                visibleTotalNumberOfPlaces += this.getNumericValue(row, "ItemNumberOfPlaces");
                visibleTotalNetWeight += this.getNumericValue(row, "NetWeight");
                visibleTotalGrossWeight += this.getNumericValue(row, "ItemGrossWeight");
                }
            }

        private void setGlobalTotals()
            {
            DataTable tableToProcess = this.invoice.Goods;
            totalPrice = 0;
            totalCount = 0;
            totalNumberOfPlaces = 0;
            totalNetWeight = 0;
            totalGrossWeight = 0;
            foreach (DataRow row in tableToProcess.Rows)
                {
                totalPrice += this.getNumericValue(row, "Sum");
                totalCount += this.getNumericValue(row, "Count");
                totalNumberOfPlaces += this.getNumericValue(row, "ItemNumberOfPlaces");
                totalNetWeight += this.getNumericValue(row, "NetWeight");
                totalGrossWeight += this.getNumericValue(row, "ItemGrossWeight");
                }
            }

        private double getNumericValue(DataRow row, string columnName)
            {
            string strValue = row.TryGetColumnValue<string>(columnName, "0");
            double doubleVal = 0;
            double.TryParse(strValue, out doubleVal);
            return doubleVal;
            }
        }
    }
