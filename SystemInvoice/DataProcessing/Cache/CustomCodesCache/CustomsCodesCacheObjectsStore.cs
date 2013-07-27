using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.CustomCodesCache
    {
    /// <summary>
    /// Хранилище кэшированных таможенных кодов, используется для поиска таможенного кода по коду.)
    /// </summary>
    public class CustomsCodesCacheObjectsStore : CacheObjectsStore<CustomsCodesCacheObject>
        {
        /// <summary>
        /// Объект - запрос для поиска таможенного кода, при поиске мы просто меняем поле объекта, это позволяет нам не создавать каждый раз для поиска новой страны новый объект,
        /// что экономит память и увеличивает скорость
        /// </summary>
        CustomsCodesCacheObject searchObject = new CustomsCodesCacheObject(string.Empty, string.Empty, false);
        /// <summary>
        /// Интерфейс с помощью которого мы меняем поля объекта поиска, делаем это для того что бы нельзя напрямую изменить эти поля в кешированных объектах и тем 
        /// самым нарушить целостность данных
        /// </summary>
        ICustomsCodeSearch searchState = null;

        public CustomsCodesCacheObjectsStore()
            {
            searchState = searchObject;
            }

        protected override string LatModifiedDateQuery
            {
            get { return "select Max(LastModified) from CustomsCode;"; }
            }

        protected override string LastProcessedCountQuery
            {
            get { return "select Count(*) from CustomsCode where MarkForDeleting = 0;"; }
            }

        protected override string SelectQuery
            {
            get { return "select Id,LTRIM(RTRIM(Description)) as code,LTRIM(RTRIM(CodeDescription)) as codeDescription,IsApprovalsRequired from CustomsCode where MarkForDeleting = 0;"; }
            }

        protected override CustomsCodesCacheObject createNew(System.Data.DataRow row)
            {
            string code = row.TryGetColumnValue<string>("code", "").Trim();
            string codeDescription = row.TryGetColumnValue<string>("codeDescription", "").Trim();
            bool isApprovalsRequired = row.TrySafeGetColumnValue<bool>("IsApprovalsRequired", false);
            //if (code.Trim().Equals("4202221000"))
            //    {
            //    int k = 0;
            //    }
            return new CustomsCodesCacheObject(code, codeDescription, isApprovalsRequired);
            }
        /// <summary>
        /// Возвращает айдишник таможенного кода по его коду
        /// </summary>
        /// <param codeName="codeName">код</param>
        public long GetCustomsCodeIdForCodeName(string codeName)
            {
            searchState.SetSearchOptions(codeName.Trim());
            return GetCachedObjectId(searchObject);
            }
        /// <summary>
        /// Возвращает кешированные данные таможенного кода по коду
        /// </summary>
        /// <param name="codeName">код</param>
        public CustomsCodesCacheObject GetCustomsCodeForCodeName(string codeName)
            {
            searchState.SetSearchOptions(codeName.Trim());
            return GetCachedObject(searchObject);
            }

        public string GetCustomsCodeDescription(string codeName)
            {
            CustomsCodesCacheObject foundedObject = GetCustomsCodeForCodeName(codeName);
            if (foundedObject != null)
                {
                return foundedObject.CodeDescription;
                }
            return string.Empty;
            }

        }
    }
