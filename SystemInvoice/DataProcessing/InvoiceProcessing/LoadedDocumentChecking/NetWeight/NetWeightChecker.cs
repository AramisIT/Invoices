using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.Excel;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;
using SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureCheck;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NetWeight
    {
    /// <summary>
    /// Проверяет вес нетто и вес нетто единицы товара для загруженной номенклатуры
    /// </summary>
    public class NetWeightChecker : LoadedDocumentCheckerBase
        {
        public NetWeightChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            }

        protected override void CheckRow(DataRow rowToCheck, ExcelMapper mapper, bool isDocumentCurrentlyLoaded, string currentCheckedColumnName)
            {
            bool containsNetWeight = mapper.ContainsKey(ProcessingConsts.ColumnNames.NET_WEIGHT_COLUMN_NAME);
            bool containsUnitNetWeight = mapper.ContainsKey(ProcessingConsts.ColumnNames.ITEM_NET_WEIGHT_COLUMN_NAME);
            bool checkBoth = containsNetWeight ^ containsUnitNetWeight;//если загружается одно значение а второе расчитывается автоматом - ошибки отображаются на обоих!
            if ((!containsNetWeight && !containsUnitNetWeight) || rowToCheck == null)
                {
                return;
                }
            //Получаем необходимые для сравнения данние
            long nomenclatureId = Helpers.InvoiceDataRetrieveHelper.GetRowNomenclatureId(rowToCheck);
            if (nomenclatureId == 0)
                {
                return;
                }
            double currentUnitNetWeight = Helpers.InvoiceDataRetrieveHelper.GetRowItemNetWeight(rowToCheck);
            double currentNetWeight = Helpers.InvoiceDataRetrieveHelper.GetRowNetWeight(rowToCheck);
            double weightFrom = Helpers.InvoiceDataRetrieveHelper.GetNomenclatureNetWeightFrom(dbCache, rowToCheck);
            double weightTo = Helpers.InvoiceDataRetrieveHelper.GetNomenclatureNetWeightTo(dbCache, rowToCheck);
            int count = Helpers.InvoiceDataRetrieveHelper.GetNomenclaturesCount(rowToCheck);
            double allowedNetWeightFrom = Math.Round(weightFrom * count, 3);
            double allowedWeightTo = Math.Round(weightTo * count, 3);
            //проверка!
            if ((containsNetWeight || checkBoth))
                {
                checkWeight(currentNetWeight, allowedNetWeightFrom, allowedWeightTo, ProcessingConsts.ColumnNames.NET_WEIGHT_COLUMN_NAME);
                }
            if ((containsUnitNetWeight || checkBoth))
                {
                checkWeight(currentUnitNetWeight, weightFrom, weightTo, ProcessingConsts.ColumnNames.ITEM_NET_WEIGHT_COLUMN_NAME);
                }
            }

        private void checkWeight(double currentWeight, double allowedFromWeight, double allowedToWeight, string columnToAddError)
            {
            if (currentWeight < allowedFromWeight)
                {
                AddError(columnToAddError, new NetWeightError(currentWeight.ToString(), allowedFromWeight.ToString(), columnToAddError));
                }
            if (currentWeight > allowedToWeight)
                {
                AddError(columnToAddError, new NetWeightError(currentWeight.ToString(), allowedToWeight.ToString(), columnToAddError));
                }
            }
        }
    }
