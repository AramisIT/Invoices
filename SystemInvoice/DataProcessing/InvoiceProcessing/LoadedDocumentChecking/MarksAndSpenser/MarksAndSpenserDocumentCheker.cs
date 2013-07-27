using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.Excel;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.MarksAndSpenser
    {
    /// <summary>
    /// Проверяет колонеи состав и таможенный код для производителя MarksAndSpenser. Проверка осуществляется если в формате загрузки выбрана колонка  для хранения значения состава К/W
    /// </summary>
    public class MarksAndSpenserDocumentCheker : LoadedDocumentCheckerBase
        {
        private const string knitCustomsCodeGroupCode = "61";
        private const string wovenCustomsCodeGroupCode = "62";
        private const string knitKeyDescription = "k";
        private const string wovenKeyDescription = "w";
        private readonly string MSKnitWovenColumnNameName;
        private readonly string CustomsCodeInternColumnName;

        public MarksAndSpenserDocumentCheker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            MSKnitWovenColumnNameName = SystemInvoice.Documents.InvoiceColumnNames.MSKnitWovenColumnName.ToString();
            CustomsCodeInternColumnName = SystemInvoice.Documents.InvoiceColumnNames.CustomsCodeIntern.ToString();
            }

        protected override void CheckRow(DataRow rowToCheck, ExcelMapper mapper, bool isDocumentCurrentlyLoaded, string currentCheckedColumnName)
            {
            string columnName = MSKnitWovenColumnNameName;
            string customsCodeColumnName = CustomsCodeInternColumnName;
            if (!isDocumentCurrentlyLoaded || rowToCheck == null || !rowToCheck.Table.Columns.Contains(columnName) || !rowToCheck.Table.Columns.Contains(customsCodeColumnName)
                || !mapper.ContainsKey(columnName))//Если в формате загрузки не указана колонка с проверяемым составом - не проверяем
                {
                return;
                }
            string knitValue = rowToCheck.TrySafeGetColumnValue(columnName, string.Empty);
            string customsCode = rowToCheck.TrySafeGetColumnValue(customsCodeColumnName, string.Empty);
            if (knitValue.ToLower().StartsWith(knitKeyDescription) && customsCode.ToLower().StartsWith(knitCustomsCodeGroupCode))
                {
                return;
                }
            if (knitValue.ToLower().StartsWith(wovenKeyDescription) && customsCode.ToLower().StartsWith(wovenCustomsCodeGroupCode))
                {
                return;
                }
            AddError(customsCodeColumnName, new MarksAndSpenserContentError());
            AddError(columnName, new MarksAndSpenserContentError());
            }
        }
    }
