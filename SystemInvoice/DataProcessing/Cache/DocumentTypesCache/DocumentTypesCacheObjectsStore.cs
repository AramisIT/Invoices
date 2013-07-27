using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.DocumentTypesCache
    {
    /// <summary>
    /// Хранилище кэшированных типов документов используется к примеру для поиска айдишника документа по коду документа (5111,5112...)
    /// </summary>
    public class DocumentTypesCacheObjectsStore : CacheObjectsStore<DocumentTypeCacheObject>
        {

        #region Имена колонок для доступа к результатам выполнения запроса

        private const string DOCUMENT_TYPE_NAME_COLUMN_NAME = "documentTypeName";
        private const string DOCUMENT_TYPE_CODE_COLUMN_NAME = "codeName";
        
        #endregion

        protected override string SelectQuery
            {
            get
                {
                string constructedQuery = string.Format( @"
select Id,LTRIM(RTRIM(Description)) as {0},LTRIM(RTRIM(QualifierCodeName)) as {1} from DocumentType where MarkForDeleting = 0;",
                DOCUMENT_TYPE_NAME_COLUMN_NAME, DOCUMENT_TYPE_CODE_COLUMN_NAME );
                return constructedQuery;
                }
            }
        protected override string LatModifiedDateQuery
            {
            get { return "select Max(LastModified) from DocumentType;"; }
            }

        protected override string LastProcessedCountQuery
            {
            get { return "select Count(*) from DocumentType where MarkForDeleting = 0;"; }
            }

        protected override DocumentTypeCacheObject createNew( DataRow row )
            {
            string documentTypeName = row.TrySafeGetColumnValue<string>( DOCUMENT_TYPE_NAME_COLUMN_NAME, string.Empty );
            string documentCodeName = row.TrySafeGetColumnValue<string>( DOCUMENT_TYPE_CODE_COLUMN_NAME, string.Empty );
            DocumentTypeCacheObject createdObject = new DocumentTypeCacheObject( documentTypeName, documentCodeName );
            return createdObject;
            }

        /// <summary>
        /// Возвращает ID типа документа по его коду
        /// </summary>
        /// <param name="documentTypeCode">код типа документа</param>
        public long GetDocumentType( string documentTypeCode )
            {
            DocumentTypeCacheObject search = new DocumentTypeCacheObject( string.Empty, documentTypeCode );
            return base.GetCachedObjectId( search );
            }

        }
    }
