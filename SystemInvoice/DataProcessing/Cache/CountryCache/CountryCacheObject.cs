using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.CountryCache
    {
    /// <summary>
    /// Используется для кеширования стран для быстрого поиска по именам страны
    /// </summary>
    public class CountryCacheObject : CacheObject<CountryCacheObject>, ICountrySearch
        {
        public string CountryShortName { get; private set; }
        public readonly string CountryFullNameUkr;
        public readonly string CountryFullNameRu;
        public readonly string CountryFullNameEn;

        public CountryCacheObject(string countryShortName, string countryUkrName, string countryRuName, string countryEnName)
            {
            this.CountryShortName = countryShortName;
            this.CountryFullNameUkr = countryUkrName;
            this.CountryFullNameRu = countryRuName;
            this.CountryFullNameEn = countryEnName;
            }

        protected override bool equals(CountryCacheObject other)
            {
            return other.CountryShortName.Equals(CountryShortName);
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new[] { CountryShortName };
            }

        /// <summary>
        /// Переопределяем calcHash поскольку вычисление хэша в данном случае осуществляется на основании всего одного поля, и эта реализация будет работать быстрее чем
        /// та что по умолчанию, поскольку в ней используется boxing
        /// </summary>
        /// <returns>хэш</returns>
        protected override int calcHash()
            {
            return CountryShortName.GetHashCode();
            }

        #region реализация ICountrySerarch
        /// <summary>
        /// Изменяет поле CountryShortName, используется для объекта по которому осуществляется поиск
        /// </summary>
        /// <param name="countryShortName">Короткое имя страны (CN,UA...)</param>
        void ICountrySearch.SetSearchOptions(string countryShortName)
            {
            CountryShortName = countryShortName;
            refreshHash();
            }

        #endregion
        }
    }
