using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Documents;
using System.Data;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.GroupItemsEditors;

namespace SystemInvoice.DataProcessing.InvoiceProcessing
    {
    /// <summary>
    /// Клас - фасад для групповой корректировки значений - веса нетто/брутто, расчет мест, обновления значения группы ячеек в колонке в отфильтрованных строках
    /// </summary>
    public class ItemsGroupEditor
        {
        private IEditableRowsSource editableRowsSource = null;
        //   private Invoice invoice = null;

        private NetWeightCalculator netWeigthsCalculator = null;
        private NetWeightWithBoundaryCheckingCalculator netWeightWithBoundaryCheckingCalculator = null;
        private GrossWeightsCalculator grossWeightCalculator = null;
        private SummCalculator summCalculator = null;
        private ReplaceEditor replaceEditor = null;
        private SystemInvoiceDBCache dbCache = null;


        public ItemsGroupEditor(IEditableRowsSource editableRowsSource, SystemInvoiceDBCache dbCache)
            {
            this.dbCache = dbCache;
            this.editableRowsSource = editableRowsSource;
            this.netWeigthsCalculator = new NetWeightCalculator(editableRowsSource, dbCache);
            this.netWeightWithBoundaryCheckingCalculator = new NetWeightWithBoundaryCheckingCalculator(editableRowsSource, dbCache);
            this.grossWeightCalculator = new GrossWeightsCalculator(editableRowsSource, dbCache);
            this.replaceEditor = new ReplaceEditor(editableRowsSource);
            this.summCalculator = new SummCalculator(editableRowsSource);
            }

        /// <summary>
        /// Корректирует занчение цены для товаров с тем что бы изменить сумму на нужное значение, при этом цена не может изменится более чем на определенное значение (на момент написания комментария - 5 коп.)
        /// </summary>
        /// <param name="arrangeDiff">Велечина на которую необходимо изменить сумму в выбранных ячейках</param>
        public void SetSumm(double arrangeDiff)
            {
            summCalculator.ArrangePrice(arrangeDiff);
            }

        /// <summary>
        /// Распределяет места по строкам табличной части инвойса пропорционально весу брутто
        /// </summary>
        /// <param name="totalPlacesCount">Общее колличество мест которое должно получится после распределения</param>
        public void SetPlaces(int totalPlacesCount)
            {
            ComputePlacesQuantity(totalPlacesCount);
            }

        /// <summary>
        /// Распределяет вес брутто по  выбранным строкам  табличной части инвойса, распределение осуществляется пропорционально весу нетто
        /// </summary>
        /// <param name="totalWeight">Велечина на которую необходимо изменить общий вес брутто</param>
        public void SetGrossWeight(double totalWeight)
            {
            grossWeightCalculator.SetWeight(totalWeight);
            }

        /// <summary>
        /// Производит замену значений в выбранных ячейках для выбранной колонки
        /// </summary>
        /// <param name="valueByWhichReplace">Значение которым заменяются значения в ячейках</param>
        /// <param name="columnToReplace">Колонка для которой заменяются значения</param>
        public void SetCurrentReplacement(string valueByWhichReplace, string columnToReplace)
            {
            this.replaceEditor.Replace(valueByWhichReplace.Trim(), columnToReplace);
            }

        /// <summary>
        /// Распределяет вес нетто по выбранным строкам табличной части инвойса меняя вес единицы товара. При этом изменение ограничивается 
        /// диапазоном веса нетто для номенклатурной единицы
        /// </summary>
        /// <param name="totalNetWeight">Велечина на которую необходимо изменить вес нетто</param>
        public void SetNetWeightIfCan(double totalNetWeight)
            {
            this.netWeightWithBoundaryCheckingCalculator.UpdateNetWeights(totalNetWeight);
            // netWeigthsCalculator.SetWeight(totalNetWeight);
            // arrangeNetWeight( totalNetWeight );
            }

        /// <summary>
        ///  Распределяет вес нетто по выбранным строкам табличной части инвойса меняя вес единицы товара. Измененный вес может выходить за допустимый 
        ///  диапазон веса нетто в номенклатуре
        /// </summary>
        /// <param name="totalNetWeight">Велечина на которую необходимо изменить вес нетто</param>
        public void SetNetWeightAnyWay(double totalNetWeight)
            {
            netWeigthsCalculator.SetWeight(totalNetWeight);
            }

        /// <summary>
        /// Распределяет места
        /// </summary>
        private void ComputePlacesQuantity(int placesQuantity)
            {
            #region init test source

            DataTable source = new DataTable();
            DataColumn grossColumn = new DataColumn("Gross", typeof(Double));
            DataColumn placesQuantityColumn = new DataColumn("PlacesQuantity", typeof(int));

            source.Columns.AddRange(new DataColumn[] { new DataColumn("Code", typeof(string)), 
                new DataColumn("Country", typeof(string)), grossColumn, placesQuantityColumn});

            //  таблицу нужно отсортировать в след. порядке: ТаможенныйКод, ВесБрутто по убыванию, Страна
            this.fillSource(source);

            #endregion

            double totalWeight = 0.0;
            foreach (DataRow row in source.Rows)
                {
                totalWeight += (double)row[grossColumn];
                }

            double onePlaceWeight = 0.0;
            int iterationNumber = 1;
            int currentPlacesQuantity = 0;

            while (Math.Abs(placesQuantity - currentPlacesQuantity) > 1 && iterationNumber <= 10)
                {
                iterationNumber++;

                if (currentPlacesQuantity == 0)
                    {
                    onePlaceWeight = totalWeight / placesQuantity;
                    }
                else
                    {
                    onePlaceWeight = (currentPlacesQuantity * onePlaceWeight) / placesQuantity;
                    }
                currentPlacesQuantity = 0;

                double placesQuantityResidual = 0;
                for (int rowIndex = 0; rowIndex < source.Rows.Count; rowIndex++)
                    {
                    DataRow row = source.Rows[rowIndex];
                    if (onePlaceWeight == 0)
                        {
                        row[placesQuantityColumn] = 0;
                        }
                    double doublePlacesQuntity = (double)row[grossColumn] / onePlaceWeight + placesQuantityResidual;
                    double placesQuntityRounded = Math.Round(doublePlacesQuntity, 0);
                    placesQuantityResidual = doublePlacesQuntity - placesQuntityRounded;
                    int currentRowPlacesQuantity = (int)placesQuntityRounded;
                    row[placesQuantityColumn] = currentRowPlacesQuantity < 0 ? 0 : currentRowPlacesQuantity;
                    currentPlacesQuantity += currentRowPlacesQuantity;
                    
                    if (currentPlacesQuantity == placesQuantity)
                        {
                        break;
                        }
                    }
                double totalAcc = double.NaN;
                //foreach (DataRow row in source.Rows)
                //    {
                //    if (onePlaceWeight == 0)
                //        {
                //        row[placesQuantityColumn] = 0;
                //        }
                //    int currentRowPlacesQuantity = (int)((double)row[grossColumn] / onePlaceWeight);
                //    row[placesQuantityColumn] = currentRowPlacesQuantity < 0 ? 0 : currentRowPlacesQuantity;
                //    currentPlacesQuantity += currentRowPlacesQuantity;
                //    }
                }
            string numberOfPlacesColumnName = InvoiceColumnNames.ItemNumberOfPlaces.ToString();
            DataRow currentRow = null;
            foreach (DataRow sourceRow in source.Rows)
                {
                if (rowsMappings.TryGetValue(sourceRow, out currentRow))
                    {
                    currentRow[numberOfPlacesColumnName] = sourceRow.TrySafeGetColumnValue<int>("PlacesQuantity", 0).ToString();
                    }
                }
            }

        Dictionary<DataRow, DataRow> rowsMappings = new Dictionary<DataRow, DataRow>();

        private void fillSource(DataTable source)
            {
            rowsMappings.Clear();
            string invoiceCountryColumnName = InvoiceColumnNames.Country.ToString();
            string invoiceCustsomsCodeColumnNAme = InvoiceColumnNames.CustomsCodeIntern.ToString();
            string invoiceGrossWeightColumnName = InvoiceColumnNames.ItemGrossWeight.ToString();
            List<DataRow> rowsToInsert = new List<DataRow>();
            foreach (DataRow row in editableRowsSource.DisplayingRows)
                {
                string countryCode = row.TrySafeGetColumnValue<string>(invoiceCountryColumnName, "");
                string customsCode = row.TrySafeGetColumnValue<string>(invoiceCustsomsCodeColumnNAme, "");
                string grossWeight = row.TrySafeGetColumnValue<string>(invoiceGrossWeightColumnName, "");
                if (string.IsNullOrEmpty(countryCode) || string.IsNullOrEmpty(customsCode))
                    {
                    continue;
                    }
                double grossWeightValue = 0;
                double.TryParse(grossWeight, out grossWeightValue);
                DataRow newRow = source.NewRow();
                newRow["Country"] = countryCode;
                newRow["Code"] = customsCode;
                newRow["Gross"] = grossWeightValue;
                rowsToInsert.Add(newRow);
                rowsMappings.Add(newRow, row);
                }
            foreach (DataRow newRow in rowsToInsert.OrderBy(row => (string)row["Country"]).OrderByDescending(row => (double)row["Gross"]).OrderBy(row => (string)row["Code"]))
                {
                source.Rows.Add(newRow);
                }
            }

        /// <summary>
        /// Зануляет значения веса брутто для всех строк в инвойсе
        /// </summary>
        public void ClearGrossWeight()
            {
            grossWeightCalculator.ClearGrossWeight();
            }


        }
    }
