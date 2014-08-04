using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;
using Aramis.Core;

namespace SystemInvoice.DataProcessing.Cache.ManufacturersCache
    {
    /// <summary>
    /// Создает новых производителей.
    /// </summary>
    public class ManufacturersObjectsCreator : DbObjectCreator<Manufacturer, ManufacturerCacheObject>
        {
        /// <summary>
        /// Кешированные производители
        /// </summary>
        private ManufacturerCacheObjectsStore manufacturersStore = null;

        public ManufacturersObjectsCreator(ManufacturerCacheObjectsStore manufacturerCacheObjectsStore)
            : base(manufacturerCacheObjectsStore)
            {
            this.manufacturersStore = manufacturerCacheObjectsStore;
            }

        protected override Manufacturer createDBObject(ManufacturerCacheObject cacheObject)
            {
            Manufacturer manufacturer = new Manufacturer();
            manufacturer.Description = cacheObject.ManufacturerName;
            IContractor contractor = A.New<IContractor>();
            contractor.Id = cacheObject.ContractorId;
            manufacturer.Contractor = contractor;
            return manufacturer;
            }

        protected override void deleteObject(Manufacturer objectToDelete)
            {
            long id = objectToDelete.Id;
            if (id == 0)
                {
                return;
                }
            string query = string.Format("delete from Manufacturer where Id = {0}", id);
            ExceuteQuery(query);
            }

        /// <summary>
        /// Пытается создать новые производители. Если производитель с таким именем для текущего контрагента уже существует она уже не создается.
        /// </summary>
        /// <param name="groupsToCheckList">Набор  имен производителей который может включать в себя новых прооизводителей</param>
        /// <returns>Были ли успешно созданны производители для имен которые не были найдены в кэше для контрагента который использовался при построении кеша</returns>
        public bool TryCreateNewManufacturers(HashSet<string> manufacturersList)
            {
            HashSet<string> newItemsToCreateNames = new HashSet<string>();
            foreach (string manufacturerName in manufacturersList)
                {
                if (string.IsNullOrEmpty(manufacturerName) || manufacturersStore.GetManufcaturerId(manufacturerName) > 0)
                    {
                    continue;
                    }
                ManufacturerCacheObject newManufacturer = manufacturersStore.CreateManufacturer(manufacturerName);
                if (newManufacturer.ContractorId == 0)
                    {
                    continue;
                    }
                if (base.TryAddToCreationList(newManufacturer))
                    {
                    newItemsToCreateNames.Add(manufacturerName);
                    }
                }
            return TryCreate();
            }

        protected override string failToCreateMessage(int failCount)
            {
            return string.Format(@"Создание {0} элементов справочника ""Производители"" завершилось неудачей.", failCount);
            }
        }
    }
