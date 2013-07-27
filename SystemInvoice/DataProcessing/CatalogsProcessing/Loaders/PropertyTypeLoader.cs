using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing.Cache;
using AramisWpfComponents.Excel;

namespace SystemInvoice.DataProcessing.CatalogsProcessing.Loaders
    {
    /// <summary>
    /// Выполняет загрузку справочника виды свойств
    /// </summary>
    public class PropertyTypeLoader : FromExcelToDataBaseObjectsLoaderBase<PropertyType>
        {
        public PropertyTypeLoader(SystemInvoiceDBCache cachedData)
            : base(cachedData)
            {
            }

        protected override bool CheckItemBegoreCreate(PropertyType itemToCheck)
            {
            string propertyType = itemToCheck.PropertyOfGoods.Description.Trim();
            long groupId = itemToCheck.SubGroupOfGoods.Id;
            string propertyTypeName = propertyType.ToLower().Trim();
            if (propertyTypeName.ToLower().Equals("размер"))
                {
                if (string.IsNullOrEmpty(itemToCheck.UkrainianValue))
                    {
                    return false;
                    }
                if (cachedData.PropertyTypesCacheObjectsStore.ContainsSize(groupId, itemToCheck.UkrainianValue))
                    {
                    return false;
                    }
                }
            if (propertyTypeName.ToLower().Equals("пол"))
                {
                if (groupId == 0)
                    {
                    return false;
                    }
                if (cachedData.PropertyTypesCacheObjectsStore.ContainsGender(groupId))
                    {
                    return false;
                    }
                }
            long propertyId = itemToCheck.PropertyOfGoods.Id;
            string codeOfProperty = itemToCheck.CodeOfProperty;
            string property = itemToCheck.Value.Trim();
            if (cachedData.PropertyTypesCacheObjectsStore.ContainsProperty(0, property, groupId, propertyId, codeOfProperty))
                {
                return false;
                }
            return true;
            }

        protected override void InitializeMapping(Excel.ExcelMapper mapper)
            {
            base.AddCustomMapping("PropertyOfGoods", loadPropertyOfGoodsByString);
            base.AddCustomMapping("SubGroupOfGoods", loadSubGroupOfGoods);
            base.AddPropertyMapping("Description", 1);
            base.AddPropertyMapping("Value", 2);
            base.AddPropertyMapping("UkrainianValue", 3);
            base.AddPropertyMapping("InsoleLength", 4);
            base.AddCustomMapping("GroupOfGoods", loadGroupOfGoods);
            base.AddCustomMapping("GroupCode", loadSubGroupOfGoodsCode);
            base.AddPropertyMapping("CodeOfProperty", 8);
            }

        PropertyOfGoods loadPropertyOfGoodsByString(Row row)
            {
            string propertyName = row[0].Value.ToString().Trim();
            if (string.IsNullOrEmpty(propertyName))
                {
                return null;
                }
            long cachedID = cachedData.PropertyOfGoodsCacheObjectsStore.GetCachedObjectId(propertyName);
            if (cachedID > 0)
                {
                PropertyOfGoods existed = new PropertyOfGoods();
                existed.Id = cachedID;
                existed.Read();
                return existed;
                }
            PropertyOfGoods newProperty = new PropertyOfGoods();
            newProperty.Description = propertyName;
            newProperty.Write();
            cachedData.PropertyOfGoodsCacheObjectsStore.Refresh();
            return newProperty;
            }


        string loadGroupOfGoods(Row row)
            {
            SubGroupOfGoods subGroup = loadSubGroupOfGoods(row);
            return subGroup.GroupOfGoods.Description;
            }

        string loadSubGroupOfGoodsCode(Row row)
            {
            SubGroupOfGoods subGroup = loadSubGroupOfGoods(row);
            return subGroup.GroupCode;
            }

        SubGroupOfGoods loadSubGroupOfGoods(Row row)
            {
            string groupOfGoodsName = row[4].Value.ToString().Trim();
            string subGroupOfGoodsCode = row[5].Value.ToString().Trim();
            string subGroupOfGoodsName = row[6].Value.ToString().Trim();
            long subGroupId = 0;
            long groupOfGoods = 0;
            if (!string.IsNullOrEmpty(subGroupOfGoodsName))
                { 
                subGroupId = cachedData.GetSubGroupId(groupOfGoodsName, subGroupOfGoodsName, subGroupOfGoodsCode);
                if (!string.IsNullOrEmpty(groupOfGoodsName))
                    {
                    groupOfGoods = cachedData.GroupOfGoodsStore.GetGroupOfGoodsId(groupOfGoodsName);
                    }
                if (subGroupId == 0)
                    {
                    if (!string.IsNullOrEmpty(groupOfGoodsName))
                        {
                        if (groupOfGoods == 0)//создаем новую группу товара если такой еще нету
                            {
                            GroupOfGoods newGroup = new GroupOfGoods();
                            newGroup.Description = groupOfGoodsName;
                            newGroup.Write();
                            cachedData.GroupOfGoodsStore.Refresh();
                            groupOfGoods = newGroup.Id;
                            }
                        }
                    //создаем новую подгруппу товара если такой еще нету
                    SubGroupOfGoods newSubGroup = new SubGroupOfGoods();
                    GroupOfGoods group = new GroupOfGoods();
                    group.Id = groupOfGoods;
                    newSubGroup.Description = subGroupOfGoodsName;
                    newSubGroup.GroupCode = subGroupOfGoodsCode;
                    newSubGroup.GroupOfGoods = group;
                    newSubGroup.Write();
                    subGroupId = newSubGroup.Id;
                    cachedData.SubGroupOfGoodsCacheObjectsStore.Refresh();
                    }
                }
            //создаем новую подгруппу товара если такой еще нету
            SubGroupOfGoods subGroup = new SubGroupOfGoods();
            subGroup.Id = subGroupId;
            if (subGroupId != 0)
                {
                subGroup.GroupCode = subGroupOfGoodsCode;
                GroupOfGoods groupS = new GroupOfGoods();
                groupS.Id = groupOfGoods;
                groupS.Description = groupOfGoodsName;
                subGroup.GroupOfGoods = groupS;
                }
            return subGroup;
            }

        protected override bool OnLoadBegin()
            {
            cachedData.PropertyTypesCacheObjectsStore.Refresh();
            cachedData.GroupOfGoodsStore.Refresh();
            cachedData.SubGroupOfGoodsCacheObjectsStore.Refresh();
            return true;
            }
        }
    }
