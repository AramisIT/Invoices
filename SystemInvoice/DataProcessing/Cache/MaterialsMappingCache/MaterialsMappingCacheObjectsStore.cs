using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.MaterialsMappingCache
    {
    /// <summary>
    /// Хранилище кэшированных типов материалов которое может использоватся для анализа материалов в инвойси и таможенном коде, при определении шапки графы 31
    /// </summary>
    public class MaterialsMappingCacheObjectsStore : CacheObjectsStore<MaterialsMappingCacheObject>
        {
        private const string materialKeyWordColumnName = "MaterialKeyWord";
        private const string customsCodeKeyWordColumnName = "CustomsCodeKeyWord";

        private readonly string[] emptyResult = new string[0];
        private readonly string[] splitterStr = new string[] { ",", " ", "\r", "\t", "\n" };
        /// <summary>
        /// Типы материалов которые могут встречатся в таможенном коде и могут быть обработанны
        /// </summary>
        private HashSet<string> materialsTypesNames = new HashSet<string>();

        protected override string SelectQuery
            {
            get
                {
                return @"select mapping.Id,mapping.MaterialKeyWord,material.CustomsCodeKeyWord from MaterialTypeMapping as mapping
join MaterialType as material on material.Id = mapping.MaterialType
where mapping.MarkForDeleting = 0 and material.MarkForDeleting = 0;";
                }
            }

        protected override string LatModifiedDateQuery
            {
            get
                {
                return "select CURRENT_TIMESTAMP;";
                //                return @"with dates as
                //                (
                //                select LastModified from MaterialType
                //                union 
                //                select LastModified from MaterialTypeMapping
                //                )
                //                select coalesce(MAX(LastModified),'0001-01-01') from dates;";
                }
            }

        protected override string LastProcessedCountQuery
            {
            get
                {
                return @"select Count(*) from MaterialTypeMapping as mapping
join MaterialType as material on material.Id = mapping.MaterialType
where mapping.MarkForDeleting = 0 and material.MarkForDeleting = 0;";
                }
            }

        protected override MaterialsMappingCacheObject createNew(DataRow row)
            {
            string materialKeyWord = row.TrySafeGetColumnValue(materialKeyWordColumnName, string.Empty).Trim();
            string customsCodeKeyWord = row.TrySafeGetColumnValue(customsCodeKeyWordColumnName, string.Empty).Trim();
            MaterialsMappingCacheObject materialsMapping = new MaterialsMappingCacheObject(materialKeyWord, customsCodeKeyWord);
            foreach (string part in customsCodeKeyWord.Split(splitterStr, StringSplitOptions.RemoveEmptyEntries))
                {
                if (!string.IsNullOrEmpty(part))
                    {
                    materialsTypesNames.Add(part.Trim().ToUpper());
                    }
                }
            return materialsMapping;
            }

        public override void Refresh()
            {
            materialsTypesNames.Clear();
            base.Refresh();
            }
        /// <summary>
        /// Возвращает типы метериалов которые могут встречатся в таможенных кодах для материала из инвойса
        /// </summary>
        /// <param name="material">Название материала из инвойса</param>
        /// <returns>Варианты наименования типов материала встречающиеся в таможенных кодах</returns>
        public string[] GetMaterialType(string material)
            {
            MaterialsMappingCacheObject searchObject = new MaterialsMappingCacheObject(material, string.Empty);
            MaterialsMappingCacheObject founded = GetCachedObject(searchObject);
            if (founded != null)
                {
                List<string> resultsList =
                    new List<string>(founded.CustomsCodeKeyWord.Split(splitterStr, StringSplitOptions.RemoveEmptyEntries));
                string[] result = new string[resultsList.Count];
                for (int i = 0; i < resultsList.Count; i++)
                    {
                    result[i] = resultsList[i].ToUpper();
                    }
                return result;
                }
            return emptyResult;
            }

        /// <summary>
        /// Является ли слово описанием типа матриала в таможенном коде
        /// </summary>
        /// <param name="materialType">Наименование материала</param>
        public bool IsMaterialType(string materialType)
            {
            return materialsTypesNames.Contains(materialType.Trim().ToUpper());
            }

        }

    }
