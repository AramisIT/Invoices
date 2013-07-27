using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.UnitOfMeasureCache
    {
    /// <summary>
    /// Используется для кеширования единиц измерения
    /// </summary>
    public class UnitOfMeasureCacheObject : CacheObject<UnitOfMeasureCacheObject>
        {
        public readonly string Name;
        public string InternationalCode { get; private set; }

        public UnitOfMeasureCacheObject(string unitOfMeasureName, string internationalCode)
            {
            this.Name = unitOfMeasureName;
            this.InternationalCode = internationalCode;
            }

        protected override bool equals(UnitOfMeasureCacheObject other)
            {
            return Name.ToLower().Equals(other.Name.ToLower());
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new[] { Name.ToLower() };
            }

        }
    }
