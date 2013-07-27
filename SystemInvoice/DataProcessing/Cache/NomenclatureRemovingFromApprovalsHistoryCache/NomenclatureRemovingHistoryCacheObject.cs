using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.NomenclatureRemovingFromApprovalsHistoryCache
    {
    /// <summary>
    /// Используется для кеширования истории о номенклатурах удаленных из табличных частей разрешительных документов
    /// </summary>
    public class NomenclatureRemovingHistoryCacheObject : CacheObject<NomenclatureRemovingHistoryCacheObject>
        {
        public long NomenclatureId { get; private set; }
        public long DocumentTypesId { get; private set; }
        public DateTime SearchedDate { get; private set; }

        public NomenclatureRemovingHistoryCacheObject(long nomenclatureId, long documentTypesId,
                                                      DateTime searchDate)
            {
            this.NomenclatureId = nomenclatureId;
            this.DocumentTypesId = documentTypesId;
            this.SearchedDate = searchDate;
            }


        protected override bool equals(NomenclatureRemovingHistoryCacheObject other)
            {
            return this.NomenclatureId.Equals(other.NomenclatureId) && this.DocumentTypesId.Equals(DocumentTypesId) && this.SearchedDate.Equals(other.SearchedDate);
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new object[] {NomenclatureId, DocumentTypesId, SearchedDate};
            }
        }
    }
