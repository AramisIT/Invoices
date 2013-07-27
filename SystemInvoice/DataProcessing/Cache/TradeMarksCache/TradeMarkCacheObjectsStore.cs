using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.PropsSyncronization;

namespace SystemInvoice.DataProcessing.Cache.TradeMarksCache
    {
    /// <summary>
    /// Используется для хранения информации о торговых марках
    /// </summary>
    public class TradeMarkCacheObjectsStore : CacheObjectsStore<TradeMarkCacheObject>
        {
        /// <summary>
        /// Объект - запрос используемый при поиске торговой марки
        /// </summary>
        TradeMarkCacheObject searchObject = new TradeMarkCacheObject( string.Empty, 0 );
        /// <summary>
        /// используется для установки искомого значения
        /// </summary>
        ITradeMarkSearch searchState = null;
        /// <summary>
        /// Информакия о контрагенте/торговой марки используемых при поиске
        /// </summary>
        private ITradeMarkContractorSource tradeMarkContractorSource = null;

        public TradeMarkCacheObjectsStore( ITradeMarkContractorSource tradeMarkContractorSource )
            {
            this.tradeMarkContractorSource = tradeMarkContractorSource;
            searchState = searchObject;
            }

        public long CurrentContractor
            {
            get
                {
                return tradeMarkContractorSource == null ? 0 : tradeMarkContractorSource.Contractor.Id;
                }
            }

        protected override string SelectQuery
            {
            get { return "select Id,LTRIM(RTRIM(Description)) as name,Contractor as contractorID from TradeMark where MarkForDeleting = 0;"; }
            }

        protected override string LatModifiedDateQuery
            {
            get { return "select Max(LastModified) from TradeMark;"; }
            }

        protected override string LastProcessedCountQuery
            {
            get { return "select Count(*) from TradeMark where MarkForDeleting = 0;"; }
            }

        protected override TradeMarkCacheObject createNew( DataRow row )
            {
            string name = row.TryGetColumnValue<string>( "name", "" ).Trim();
            long contractorID = row.TryGetColumnValue<long>( "contractorID", 0 );
            return new TradeMarkCacheObject( name, contractorID );
            }
        /// <summary>
        /// Специальный метод выбирающий торговую марку из кэша или возвращающий значение из фильтра документа/справочника реализующего интерфейс ITradeMarkContractorSource
        /// Выполняет проверку на конфликты - если мы одновременно пытаемся выбрать торговую марку с определенным именем, при том что у нас уже выбрана в документе - 
        /// генерирует исключение. К примеру мы не можем выбрать любую торговую марку из загружаемого файла, если она уже задана в фильтре документа инвойс - поскольку
        /// мы должны обрабатывать именно ту торговую марку которая указанна в фильтре.
        /// </summary>
        /// <param name="tradeMarkName">Имя торговой марки. Если оно неизвестно (пустое значение) выбирает значение из фильтра документа/справочника</param>
        /// <returns>ID торговой марки. 0 - если не найдена.</returns>
        public long GetTradeMarkIdOrCurrent( string tradeMarkName )
            {
            tradeMarkName = tradeMarkName.Trim();
            long tradeMarkId = tradeMarkContractorSource.TradeMark.Id;
            if (tradeMarkId > 0)
                {
                if (string.IsNullOrEmpty( tradeMarkName ))
                    {
                    return tradeMarkId;
                    }
                string currentTradeMark = GetCachedObject( tradeMarkId ).TradeMarkName.Trim();
                //В данном случае получается конфликт - с одной стороны у нас задана номенклатура например в формате с другой стороны из файла выбираем другую
                if (!currentTradeMark.ToLower().Equals(tradeMarkName.Trim().ToLower())&&!tradeMarkName.Equals("ItemTradeMark"))//  string.IsNullOrEmpty( tradeMarkName ))
                    {
                    throw new TradeMarkConflictException();
                    }
                else
                    {
                    return tradeMarkId;
                    }
                }
            if (setSearchedTradeMark( tradeMarkName ))
                {
                return GetCachedObjectId( searchObject );
                }
            return 0;
            }
        /// <summary>
        /// Возвращает айдишник торговой марки
        /// </summary>
        /// <param name="tradeMarkName">Имя торговой марки</param>
        /// <returns>Айди</returns>
        public long GetTradeMarkId( string tradeMarkName )
            {
            if (setSearchedTradeMark( tradeMarkName.Trim() ))
                {
                return GetCachedObjectId( searchObject );
                }
            return 0;
            }

        public TradeMarkCacheObject GetTradeMarkForName( string tradeMarkName )
            {
            if (setSearchedTradeMark( tradeMarkName.Trim() ))
                {
                return GetCachedObject( searchObject );
                }
            return null;
            }

        private bool setSearchedTradeMark( string tradeMarkName )
            {
            if (tradeMarkContractorSource == null)
                {
                return false;
                }
            long contractorId = tradeMarkContractorSource.Contractor.Id;
            return setSearchedTradeMark(tradeMarkName, contractorId);
            }

        private bool setSearchedTradeMark(string tradeMarkName, long contractorId)
            {
            if (string.IsNullOrEmpty(tradeMarkName) || contractorId == 0)
                {
                return false;
                }
            searchState.SetSearchOptions(tradeMarkName, contractorId);
            return true;
            }

        public TradeMarkCacheObject CreateTradeMark( string tradeMarkName )
            {
            if(tradeMarkContractorSource == null)
                {
                return null;
                }
            TradeMarkCacheObject tMark = new TradeMarkCacheObject( tradeMarkName, tradeMarkContractorSource.Contractor.Id );
            return tMark;
            }

        public class TradeMarkConflictException : Exception
            {
            public override string Message
                {
                get
                    {
                    return "Торговая марка уже задана в документе/справочнике.";
                    }
                }
            }


        public long GetTradeMarkId(string tradeMarkName, long currentContractorId)
            {
            if (setSearchedTradeMark(tradeMarkName.Trim(), currentContractorId))
                {
                return GetCachedObjectId(searchObject);
                }
            return 0;
            }
        }
    }
