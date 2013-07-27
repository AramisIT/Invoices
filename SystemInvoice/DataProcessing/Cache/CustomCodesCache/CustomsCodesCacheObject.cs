using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.CustomCodesCache
    {
    /// <summary>
    /// Используется для кеширования таможенных кодов, для быстрого поиска таможенного кода по коду
    /// </summary>
    public class CustomsCodesCacheObject : CacheObject<CustomsCodesCacheObject>, ICustomsCodeSearch
        {
        public string Code { get; private set; }
        public bool IsApprovalsRequired { get; private set; }
        /// <summary>
        /// Описание таможенного кода, может включать в себя вид продукции, материал и т.д.
        /// </summary>
        public string CodeDescription { get; private set; }

        public CustomsCodesCacheObject(string code, string codeDescription, bool isApprovalsRequired)
            {
            this.Code = code;
            this.CodeDescription = codeDescription;
            this.IsApprovalsRequired = isApprovalsRequired;
            }

        public CustomsCodesCacheObject(string name)
            : this(name, string.Empty, false)
            {
            }

        protected override bool equals(CustomsCodesCacheObject other)
            {
            return Code.Equals(other.Code);
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new[] { Code };
            }

        /// <summary>
        /// Переопределяем calcHash поскольку вычисление хэша в данном случае осуществляется на основании всего одного поля, и эта реализация будет работать быстрее чем
        /// та что по умолчанию, поскольку в ней используется boxing
        /// </summary>
        /// <returns>хэш</returns>
        protected override int calcHash()
            {
            return Code.GetHashCode();
            }

        #region реализация  ICustomsCodeSearch

        void ICustomsCodeSearch.SetSearchOptions(string codeName)
            {
            Code = codeName;
            refreshHash();
            }

        #endregion
        }
    }
