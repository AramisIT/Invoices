using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.MaterialsMappingCache
    {
    /// <summary>
    /// Используется для кеширования типов материалов
    /// </summary>
    public class MaterialsMappingCacheObject : CacheObject<MaterialsMappingCacheObject>
        {
        /// <summary>
        /// Ключевое имя материала в которое может встречатся в составе инвойса в входящем файле
        /// </summary>
        public string MaterialKeyWord { get; private set; }
        /// <summary>
        /// Ключевое имя типа материала которое может встречатся в таможенном коде
        /// </summary>
        public string CustomsCodeKeyWord { get; private set; }

        public MaterialsMappingCacheObject(string materialKeyWord, string customsCodeKeyWord)
            {
            MaterialKeyWord = materialKeyWord;
            CustomsCodeKeyWord = customsCodeKeyWord;
            }

        protected override bool equals(MaterialsMappingCacheObject other)
            {
            return MaterialKeyWord.ToUpper().Equals(other.MaterialKeyWord.ToUpper());
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new[] { MaterialKeyWord.ToUpper() };
            }
        }
    }
