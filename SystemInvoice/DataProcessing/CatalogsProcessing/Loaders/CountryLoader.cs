using System.Collections.Generic;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing.Cache;
using Aramis.DatabaseConnector;
using AramisWpfComponents.Excel;

namespace SystemInvoice.DataProcessing.CatalogsProcessing.Loaders
    {
    /// <summary>
    /// Загружает страны
    /// </summary>
    public class CountryLoader : FromExcelToDataBaseObjectsLoaderBase<Country>
        {
        HashSet<string> countriesExisted = new HashSet<string>();

        public CountryLoader(SystemInvoiceDBCache cachedData)
            : base(cachedData)
            {
            }

        /// <summary>
        /// Проверяем что бы были загруженны только новые страны и каждая страна была загруженна один раз и были заполнены необходимые поля
        /// </summary>
        protected override bool CheckItemBegoreCreate(Country itemToCheck)
            {
            if (countriesExisted.Contains(itemToCheck.InternationalCode) || string.IsNullOrEmpty(itemToCheck.InternationalCode.Trim()) ||
                string.IsNullOrEmpty(itemToCheck.Description.Trim()) || string.IsNullOrEmpty(itemToCheck.NameRus))
                {
                return false;
                }
            return true;
            }

        protected override void InitializeMapping(Excel.ExcelMapper mapper)
            {
            base.AddPropertyMapping("Description", 1);//учитываем что нумерация идет с 1
            base.AddPropertyMapping("NameRus", 2);
            base.AddPropertyMapping("NameEng", 3);
            base.AddPropertyMapping("InternationalCode", 4);
            base.AddCustomMapping("InternationalDigitCode", loadInternationalCountryCode);
            }
        /// <summary>
        /// Получаем цифрое значение международного кода.
        /// </summary>
        private object loadInternationalCountryCode(Row row)
            {
            object codeValue = row[4].Value;
            if (codeValue is int)
                {
                return codeValue;
                }
            string strVal = codeValue.ToString();
            int value = 0;
            if (int.TryParse(strVal, out value))
                {
                return value;
                }
            return 0;
            }

        protected override bool OnLoadBegin()
            {
            countriesExisted.Clear();
            string loadCountiresCodesStr = "select InternationalCode as code from Country where MarkForDeleting = 0;";
            DB.NewQuery(loadCountiresCodesStr).Foreach((result) =>
            {
                string codeDescription = ((string)result["code"]).Trim();
                if (!countriesExisted.Contains(codeDescription))
                    {
                    countriesExisted.Add(codeDescription);
                    }
            });
            return true;
            }
        }
    }
