using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.CountryCache
    {
    /// <summary>
    /// Индекс для получения короткого имени страны (UA,CN...) на основании русского наименования (Украина, Китай...)
    /// </summary>
    public class CountryNameRuIndex : IEqualityComparer<CountryCacheObject>
        {
        public bool Equals( CountryCacheObject x, CountryCacheObject y )
            {
            return x.CountryFullNameRu.Equals( y.CountryFullNameRu );
            }

        public int GetHashCode( CountryCacheObject obj )
            {
            return obj.CountryFullNameRu.GetHashCode();
            }
        }
    }
