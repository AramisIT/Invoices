using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.GroupOfGoodsCache
    {
    /// <summary>
    /// Создает новые  Группы товаров.
    /// </summary>
    public class GroupOfGoodsObjectsCreator : DbObjectCreator<GroupOfGoods, GroupOfGoodsCacheObject>
        {
        /// <summary>
        /// Кешированные группы
        /// </summary>
        private GroupOfGoodsCacheObjectsStore groupOfGoodsStore = null;

        public GroupOfGoodsObjectsCreator(GroupOfGoodsCacheObjectsStore groupOfGoodsStore)
            : base(groupOfGoodsStore)
            {
            this.groupOfGoodsStore = groupOfGoodsStore;
            }

        protected override GroupOfGoods createDBObject(GroupOfGoodsCacheObject cacheObject)
            {
            GroupOfGoods groupOfGoods = new GroupOfGoods();
            groupOfGoods.Description = cacheObject.GroupName;
            return groupOfGoods;
            }

        protected override void deleteObject(GroupOfGoods objectToDelete)
            {
            long id = objectToDelete.Id;
            if (id == 0)
                {
                return;
                }
            string query = string.Format("delete from GroupOfGoods where Id = {0}", id);
            ExceuteQuery(query);
            }

        protected override string failToCreateMessage(int failCount)
            {
            return string.Format("Создание {0} групп товара завершилось ошибкой.", failCount);
            }

        /// <summary>
        /// Пытается создать новые группы товаров. Если группа с таким именем уже существует она уже не создается.
        /// </summary>
        /// <param name="groupsToCheckList">Набор  имен групп который может содержать в себе новые группы</param>
        /// <returns>Были ли успешно созданны новые группы для имен которые не были найдены в кэше</returns>
        public bool TryCreateGroupOfGoods(HashSet<string> groupsToCheckList)
            {
            HashSet<string> newItemsToCreateNames = new HashSet<string>();
            foreach (string groupOfGoodsName in groupsToCheckList)
                {
                if (string.IsNullOrEmpty(groupOfGoodsName) || groupOfGoodsStore.GetGroupOfGoodsId(groupOfGoodsName) > 0)
                    {
                    continue;
                    }
                GroupOfGoodsCacheObject newGroup = new GroupOfGoodsCacheObject(groupOfGoodsName);
                if (base.TryAddToCreationList(newGroup))
                    {
                    newItemsToCreateNames.Add(groupOfGoodsName);
                    }
                }
            return TryCreate();
            }
        }
    }
