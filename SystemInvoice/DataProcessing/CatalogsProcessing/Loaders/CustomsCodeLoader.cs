using System.Collections.Generic;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing.Cache;
using Aramis.DatabaseConnector;
using AramisWpfComponents.Excel;

namespace SystemInvoice.DataProcessing.CatalogsProcessing.Loaders
    {
    /// <summary>
    /// Загружает таможенные коды из входящего файла
    /// </summary>
    public class CustomsCodeLoader : FromExcelToDataBaseObjectsLoaderBase<CustomsCode>
        {
        private const string skipRowStr = "clear!!!";
        private const string replaceEmptyByAnotherCodeStr = "Отсутствуют";
        private bool mergedRegionsInitialized = false;
        private int skipRow = 0;
        private Dictionary<string, long> customsCodes = new Dictionary<string, long>();
        private Dictionary<string, long> customsCodesCodes = new Dictionary<string, long>();
        private CustomsCode lastCustomCodesItem = null;

        public CustomsCodeLoader(SystemInvoiceDBCache cachedData)
            : base(cachedData)
            {
            }

        protected override bool CheckItemBegoreCreate(CustomsCode itemToCheck)
            {
            if (string.IsNullOrEmpty(itemToCheck.CodeDescription.Trim()) || "!!!".Equals(itemToCheck.CodeDescription.Trim()))//Если таможенный код пустой заменяем наименование на константу
                {
                itemToCheck.CodeDescription = replaceEmptyByAnotherCodeStr;
                }
            if (itemToCheck == null || itemToCheck.CodeDescription.Equals(skipRowStr) || string.IsNullOrEmpty(itemToCheck.CodeDescription.Trim()) || "!!!".Equals(itemToCheck.CodeDescription.Trim()))
                {
                return false;
                }
            if (string.IsNullOrEmpty(itemToCheck.Description.Trim()))
                {
                return false;
                }
            //получаем родителя для текущего таможенного кода и записываем его. Таким образом выстраивается иерархия
            CustomsCode parentForCurrent = this.getParrent(lastCustomCodesItem, itemToCheck);
            if (parentForCurrent != null)
                {
                itemToCheck.ParentId = parentForCurrent;
                }
            itemToCheck.CodeDescription = getClearText(itemToCheck.CodeDescription);
            if (string.IsNullOrEmpty(itemToCheck.Description.Trim()))
                {
                itemToCheck.Description = CustomsCode.emptyCodeStr;
                }
            lastCustomCodesItem = itemToCheck;
            itemToCheck.Description = itemToCheck.Description.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "").Trim();
            string checkItemStr = string.Format("parentId:{0}code:{1}codeDesc:{2}", itemToCheck.ParentId.Id, itemToCheck.Description, itemToCheck.CodeDescription.Trim());
            if (customsCodes.ContainsKey(checkItemStr) || customsCodesCodes.ContainsKey(itemToCheck.Description.Trim()))//проверяем что такого кода не существует
                {
                long id = customsCodesCodes[itemToCheck.Description.Trim()];
                itemToCheck.ReadingId = customsCodesCodes;
                return false;
                }
            return true;
            }

        protected override void InitializeMapping(Excel.ExcelMapper mapper)
            {
            base.AddCustomMapping("CodeDescription", this.processCustomsCodeDescription);
            base.AddPropertyMapping("Description", 1);
            }
        /// <summary>
        /// Проверяем что мы еще не находимся в области с данными
        /// </summary>
        private void checkMergedRegion(Row row)
            {
            if (mergedRegionsInitialized)
                {
                return;
                }
            int currentMergedRegion = 0;
            Worksheet workSheet = row.ParentSheet;
            var sheet = row.ROW.Sheet;
            NPOI.SS.Util.CellRangeAddress address = null;
            while ((address = sheet.GetMergedRegion(currentMergedRegion++)) != null)
                {
                Cell cellFrom = workSheet[address.FirstRow][address.FirstColumn];
                cellFrom.ColumnSpan = address.LastColumn - address.FirstColumn + 1;
                cellFrom.RowSpan = address.LastRow - address.FirstRow + 1;
                }
            mergedRegionsInitialized = true;
            }

        /// <summary>
        /// "выдираем" описание кода из ячеек, учитывая то что часть ячеек может быть объеденина по строкам
        /// </summary>
        string processCustomsCodeDescription(AramisWpfComponents.Excel.Row row)
            {
            checkMergedRegion(row);
            if (skipRow-- > 0)
                {
                return skipRowStr;
                }
            Cell codeCell = row[0];
            skipRow = codeCell.RowSpan - 1;
            string descr = "";
            int currentRowIndex = row.RowIndex;
            Worksheet sheet = row.ParentSheet;
            for (int i = 0; i < codeCell.RowSpan; i++)
                {
                descr += (string)sheet[currentRowIndex + i][2].Value;
                }
            return descr;
            }

        private CustomsCode getParrent(CustomsCode lastItem, CustomsCode item)
            {
            if (lastItem == null)
                {
                return null;
                }
            CustomsCode selectedParent = lastItem;
            int lastItemHierarchyDeepness = 0;
            while ((selectedParent = selectedParent.ParentId as CustomsCode).Id != 0)
                {
                lastItemHierarchyDeepness++;
                }
            int currentDeepness = getDeepness(item.CodeDescription);
            if (currentDeepness == 0)
                {
                return null;
                }
            else
                {
                selectedParent = lastItem;
                }
            while (lastItemHierarchyDeepness >= currentDeepness && lastItemHierarchyDeepness >= 0)
                {
                if (selectedParent == null)
                    {
                    selectedParent = lastItem;
                    }
                lastItemHierarchyDeepness--;
                selectedParent = selectedParent.ParentId as CustomsCode;
                }
            return selectedParent;
            }

        private int getDeepness(string description)
            {
            int i = 0;
            for (; i + 1 < description.Length; i += 2)
                {
                if (description[i] != '-' || description[i + 1] != ' ')
                    {
                    break;
                    }
                }
            return i / 2;
            }

        private string getClearText(string text)
            {
            int index = getDeepness(text);
            return text.Substring(2 * index).Trim();
            }

        protected override bool OnLoadBegin()
            {
            mergedRegionsInitialized = false;
            customsCodes.Clear();
            customsCodesCodes.Clear();
            string loadAllCustsomsCodesStr = "select 'parentId:' + CAST(ParentID as nvarchar(20)) + 'code:' + LTRIM(RTRIM(Description)) + 'codeDesc:' + LTRIM(RTRIM(CodeDescription)) as description,Id,LTRIM(RTRIM(Description)) as code from CustomsCode where MarkForDeleting = 0;";
            DB.NewQuery(loadAllCustsomsCodesStr).Foreach((result) =>
            {
                string codeDescription = (string)result["description"];
                long itemId = (long)result["Id"];
                string code = (string)result["code"];
                if (!CustomsCode.emptyCodeStr.Equals(codeDescription) && !customsCodes.ContainsKey(codeDescription))
                    {
                    customsCodes.Add(codeDescription.Trim(), itemId);
                    }
                if (!customsCodesCodes.ContainsKey(code))
                    {
                    customsCodesCodes.Add(code, itemId);
                    }
            });
            return true;
            }

        protected override void OnLoadComplete()
            {
            customsCodes.Clear();
            lastCustomCodesItem = null;
            }

        protected override int StartRowIndex
            {
            get { return 4; }
            }
        }
    }
