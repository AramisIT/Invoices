using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.UnitOfMeasureCache
    {
    /// <summary>
    /// Используется для хранения информации о единицах измерения
    /// </summary>
    public class UnitOfMeasureCacheObjectsStore : CacheObjectsStore<UnitOfMeasureCacheObject>
        {
        protected override string SelectQuery
            {
            get { return @"select Id,LTRIM(RTRIM(Description)) as name,LTRIM(RTRIM(InternationalCode)) as InternationalCode,LTRIM(RTRIM(ShortName)) as ShortName from UnitOfMeasure where MarkForDeleting = 0;"; }
            }

        protected override string LatModifiedDateQuery
            {
            get { return "select Max(LastModified) from UnitOfMeasure;"; }
            }

        protected override string LastProcessedCountQuery
            {
            get { return "select Count(*) from UnitOfMeasure where MarkForDeleting = 0;"; }
            }

        protected override UnitOfMeasureCacheObject createNew(System.Data.DataRow row)
            {
            string name = row.TryGetColumnValue<string>("ShortName", "").Trim();
            string internationalCode = row.TrySafeGetColumnValue<string>("InternationalCode", string.Empty).Trim();
            return new UnitOfMeasureCacheObject(name,internationalCode);
            }

        /// <summary>
        /// Возвращает полное имя единицы измерения по ее короткому имени
        /// </summary>
        public string GetUnitOfMeasureCodeFromShortName(string shortName)
            {
            UnitOfMeasureCacheObject searched = new UnitOfMeasureCacheObject(shortName, string.Empty);
            UnitOfMeasureCacheObject cached = GetCachedObject(searched);
            if (cached == null)
                {
                return string.Empty;
                }
            return cached.InternationalCode;//.GetObjectStringValue("InternationalCode");
            }

        /// <summary>
        /// Возвращает Id единицы измерения по ее короткому имени
        /// </summary>
        public long GetUnitOfMeasureIdForShortName(string unitOfMeasureShortName)
            {
            UnitOfMeasureCacheObject searched = new UnitOfMeasureCacheObject(unitOfMeasureShortName, string.Empty);
            long cachedId = GetCachedObjectId(searched);
            return cachedId;
            }
        }
    }
