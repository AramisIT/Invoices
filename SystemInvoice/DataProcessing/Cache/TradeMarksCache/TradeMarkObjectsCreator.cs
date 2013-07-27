using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;

namespace SystemInvoice.DataProcessing.Cache.TradeMarksCache
    {
    /// <summary>
    /// Создает элементы справочника "Торговые марки"
    /// </summary>
    public class TradeMarkObjectsCreator : DbObjectCreator<TradeMark, TradeMarkCacheObject>
        {
        private TradeMarkCacheObjectsStore tradeMarksStore = null;

        public TradeMarkObjectsCreator( TradeMarkCacheObjectsStore tradeMarksStore )
            : base( tradeMarksStore )
            {
            this.tradeMarksStore = tradeMarksStore;
            }

        protected override TradeMark createDBObject( TradeMarkCacheObject cacheObject )
            {
            TradeMark tradeMark = new TradeMark();
            tradeMark.Description = cacheObject.TradeMarkName;
            Contractor contractor = new Contractor();
            contractor.Id = cacheObject.ContractorId;
            tradeMark.Contractor = contractor;
            return tradeMark;
            }

        protected override void deleteObject( TradeMark objectToDelete )
            {
            long id = objectToDelete.Id;
            if (id == 0)
                {
                return;
                }
            string query = string.Format( "delete from TradeMark where Id = {0}", id );
            ExceuteQuery( query );
            }
        /// <summary>
        /// Создает торговые марки для тех наименований которых нету в системе
        /// </summary>
        /// <param name="tradeMarksList">Список наименований</param>
        public bool TryCreateNewTradeMarks( HashSet<string> tradeMarksList )
            {
            HashSet<string> newItemsToCreateNames = new HashSet<string>();
            foreach (string tradeMarkName in tradeMarksList)
                {
                if (newItemsToCreateNames.Contains( tradeMarkName ))
                    {
                    continue;
                    }
                if (tradeMarksStore.GetTradeMarkIdOrCurrent( tradeMarkName ) > 0)
                    {
                    continue;
                    }
                TradeMarkCacheObject newTradeMark = tradeMarksStore.CreateTradeMark( tradeMarkName );
                if (newTradeMark.ContractorId == 0)
                    {
                    continue;
                    }
                if (base.TryAddToCreationList( newTradeMark ))
                    {
                    newItemsToCreateNames.Add( tradeMarkName );
                    }
                }
            return TryCreate();
            }

        protected override string failToCreateMessage( int failCount )
            {
            return string.Format( @"Создание {0} элементов справочника ""Торговые марки"" завершилось неудачей.", failCount );
            }
        }
    }
