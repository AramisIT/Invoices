using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing.Cache.ManufacturersCache;
using SystemInvoice.DataProcessing.Cache.TradeMarksCache;
using SystemInvoice.DataProcessing.Cache.GroupOfGoodsCache;
using Aramis.Core;

namespace SystemInvoice.DataProcessing.Cache.SubGroupOfGoodsCache
    {
    /// <summary>
    /// Создает новые подгруппы товаров
    /// </summary>
    public class SubGroupOfGoodsObjectsCreator : DbObjectCreator<SubGroupOfGoods, SubGroupOfGoodsCacheObject>
        {
        private GroupOfGoodsCacheObjectsStore groupOfGoodsStore = null;
        private SubGroupOfGoodsCacheObjectsStore SubGroupOfGoodsObjectsStore = null;
        ManufacturerCacheObjectsStore manufacturersStore = null;
        TradeMarkCacheObjectsStore tradeMarksStore = null;

        public SubGroupOfGoodsObjectsCreator( ManufacturerCacheObjectsStore manufacturersStore,
            TradeMarkCacheObjectsStore tradeMarksStore, SubGroupOfGoodsCacheObjectsStore SubGroupOfGoodsObjectsStore, 
            GroupOfGoodsCacheObjectsStore groupOfGoodsStore )
            : base( SubGroupOfGoodsObjectsStore )
            {
            this.SubGroupOfGoodsObjectsStore = SubGroupOfGoodsObjectsStore;
            this.manufacturersStore = manufacturersStore;
            this.tradeMarksStore = tradeMarksStore;
            this.groupOfGoodsStore = groupOfGoodsStore; 
            }

        protected override SubGroupOfGoods createDBObject( SubGroupOfGoodsCacheObject cacheObject )
            {
            IContractor contractor = A.New<IContractor>();
            contractor.Id = cacheObject.Contractor;
            ITradeMark tm = A.New<ITradeMark>();
            tm.Id = cacheObject.TradeMark;
            tm.Contractor = contractor;
            IManufacturer mf = A.New<IManufacturer>();
            mf.Id = cacheObject.Manufacturer;
            mf.Contractor = contractor;
            GroupOfGoods groupOfGoods = new GroupOfGoods();
            groupOfGoods.Id = cacheObject.GroupId;
            SubGroupOfGoods newGroup = new SubGroupOfGoods();
            newGroup.GroupCode = cacheObject.Code;
            newGroup.Description = cacheObject.Name;
            newGroup.Contractor = contractor;
            newGroup.TradeMark = tm;
            newGroup.Manufacturer = mf;
            newGroup.GroupOfGoods = groupOfGoods;
            return newGroup;
            }

        protected override void deleteObject( SubGroupOfGoods objectToDelete )
            {
            long id = objectToDelete.Id;
            if (id == 0)
                {
                return;
                }
            string query = string.Format( "delete from SubGroupOfGoods where Id = {0}", id );
            ExceuteQuery( query );
            }

        protected override string failToCreateMessage( int failCount )
            {
            return string.Format( @"Создание {0} элементов справочника ""Подгруппы товаров"" завершилось неудачей.", failCount );
            }

        /// <summary>
        /// Добавляет в список объектов которые нужно создать информацию для новой подгруппы товара
        /// </summary>
        public void AddToCreationList( string groupName, string subGroupName, string groupCode, string manufacturer, string tradeMark )
            {
            long groupID = groupOfGoodsStore.GetGroupOfGoodsId( groupName );
            long tradeMarkId = tradeMarksStore.GetTradeMarkIdOrCurrent( tradeMark );
            long manufacturerId = manufacturersStore.GetManufcaturerId( manufacturer );
            SubGroupOfGoodsCacheObject newObject = SubGroupOfGoodsObjectsStore.CreateNew( groupID, subGroupName, groupCode, tradeMarkId, manufacturerId );
            if (newObject != null)
                {
                TryAddToCreationList( newObject );
                }
            }
        }
    }
