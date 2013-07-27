using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.DocumentTypesCache
    {
    /// <summary>
    /// Используется для кеширования типов документов
    /// </summary>
    public class DocumentTypeCacheObject : CacheObject<DocumentTypeCacheObject>
        {
        public string DocumentTypeName { get; private set; }
        public string DocumentTypeCode { get; private set; }

        public DocumentTypeCacheObject(string DocumentTypeName, string DocumentTypeCode)
            {
            this.DocumentTypeName = DocumentTypeName;
            this.DocumentTypeCode = DocumentTypeCode;
            }

        protected override bool equals(DocumentTypeCacheObject other)
            {
            return other.DocumentTypeCode.Equals(DocumentTypeCode);
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new[] { DocumentTypeCode };
            }
        }
    }
