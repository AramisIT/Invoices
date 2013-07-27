using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.CountryCache
    {
    /// <summary>
    /// Хранилище кэшированных стран, используется для поиска страны по короткому наименованию или другому наименованию
    /// </summary>
    public class CountryCahceObjectsStore : CacheObjectsStore<CountryCacheObject>
        {
        /// <summary>
        /// Индекс для поиска по русскому наименованию страны
        /// </summary>
        private IEqualityComparer<CountryCacheObject> countryRuIndex = new CountryNameRuIndex();
        /// <summary>
        /// Объект - запрос для поиска страны, при поиске мы просто меняем поле объекта, это позволяет нам не создавать каждый раз для поиска новой страны новый объект,
        /// что экономит память и увеличивает скорость
        /// </summary>
        CountryCacheObject searchObject = new CountryCacheObject( string.Empty, string.Empty, string.Empty, string.Empty );
        /// <summary>
        /// Интерфейс с помощью которого мы меняем поля объекта поиска, делаем это для того что бы нельзя напрямую изменить эти поля в кешированных объектах и тем самым нарушить целостность данных
        /// </summary>
        ICountrySearch searchState = null;

        public CountryCahceObjectsStore()
            {
            searchState = searchObject;
            }

        protected override string SelectQuery
            {
            get
                {
                return @"select Id, LTRIM(RTRIM(InternationalCode)) as shortName,LTRIM(RTRIM(Description)) as Name,LTRIM(RTRIM(NameEng)) as NameEng,
LTRIM(RTRIM(NameRus)) as NameRus,InternationalDigitCode from Country where MarkForDeleting = 0;";
                }
            }

        protected override CountryCacheObject createNew( System.Data.DataRow row )
            {
            string shortName = row.TryGetColumnValue<string>( "shortName", "" );
            string nameUkr = row.TrySafeGetColumnValue<string>( "Name", "" );
            string nameEn = row.TrySafeGetColumnValue<string>( "NameEng", "" );
            string nameRu = row.TrySafeGetColumnValue<string>( "NameRus", "" );
            return new CountryCacheObject( shortName, nameUkr, nameRu, nameEn );
            }

        public long GetIdForCountryShortName( string countryShortName )
            {
            searchState.SetSearchOptions( countryShortName.Trim() );
            return GetCachedObjectId( searchObject );
            }

        public CountryCacheObject GetCountryForShortName( string countryShortName )
            {
            searchState.SetSearchOptions( countryShortName.Trim() );
            return GetCachedObject( searchObject );
            }

        protected override void InitializeIndexes( List<IEqualityComparer<CountryCacheObject>> indexes )
            {
            indexes.Add( countryRuIndex );//добавляет индекс для поиска страны по русскому наименованию
            }
        /// <summary>
        /// Возвращает короткое имя страны на основании русского наименования
        /// </summary>
        public string GetShortNameForCountryRuName( string countryRuName )
            {
            //создаем объект для поиска страны по русскому наименованию
            CountryCacheObject searchObject = new CountryCacheObject( string.Empty, string.Empty, countryRuName.Trim(), string.Empty );
            //плучаем айдишник кешированного объекта
            long Id = GetCachedObjectId( countryRuIndex, searchObject );
            //возвращаем короткое имя (если объект в кеше найден)
            CountryCacheObject founded = null;
            if (Id == 0 || ((founded = GetCachedObject( Id )) == null))
                {
                return string.Empty;
                }
            return founded.CountryShortName;
            }

        protected override string LatModifiedDateQuery
            {
            get { return "select Max(LastModified) from Country;"; }
            }

        protected override string LastProcessedCountQuery
            {
            get { return "select Count(*) from Country where MarkForDeleting = 0;"; }
            }
        }
    }
