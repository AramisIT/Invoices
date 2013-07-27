using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.ManufacturersCache
    {
    /// <summary>
    /// Используется для кеширования производителей
    /// </summary>
    public class ManufacturerCacheObject : CacheObject<ManufacturerCacheObject>, IManufacturerSearch
        {
        public string ManufacturerName { get; private set; }
        public long ContractorId { get; private set; }

        public ManufacturerCacheObject(string manufacturerName, long contractorId)
            {
            this.ManufacturerName = manufacturerName;
            this.ContractorId = contractorId;
            }

        protected override bool equals(ManufacturerCacheObject other)
            {
            return other.ContractorId == ContractorId && other.ManufacturerName.Equals(ManufacturerName);
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new object[] { ContractorId, ManufacturerName };
            }

        #region реализация  IManufacturerSearch
        void IManufacturerSearch.SetSearchOptions(string manufacturerName, long contractorId)
            {
            if (string.IsNullOrEmpty(manufacturerName))
                {
                return;
                }
            this.ManufacturerName = manufacturerName;
            this.ContractorId = contractorId;
            refreshHash();
            }
        #endregion


        /// <summary>
        /// Переопределяем calcHash поскольку вычисление хэша, эта реализация будет работать быстрее чем
        /// та что по умолчанию, поскольку в ней используется boxing
        /// </summary>
        /// <returns>хэш</returns>
        protected override int calcHash()
            {
            return ContractorId.GetHashCode() ^ ManufacturerName.GetHashCode();
            }
        }
    }
