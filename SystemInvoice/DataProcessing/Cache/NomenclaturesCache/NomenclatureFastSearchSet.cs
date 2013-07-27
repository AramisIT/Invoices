using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache.TradeMarksCache;

namespace SystemInvoice.DataProcessing.Cache.NomenclaturesCache
    {
    /// <summary>
    /// Используется для проверки уникальности последовательности элементов списка номенклатуры. Каждый элемент при проверке сверяется с предыдущими элементами
    /// с тем что бы не проделывать некоторые операции над одними и теми же элементами дважды
    /// </summary>
    public class NomenclatureFastSearchSet
        {
        /// <summary>
        /// Внутреннее хранилище объектов
        /// </summary>
        private HashSet<NomenclatureFastSearchResult> internalSet = new HashSet<NomenclatureFastSearchResult>();
        /// <summary>
        /// Объект для проверки нахождения объектов в внутренней структуре данных
        /// </summary>
        NomenclatureFastSearchResult searchObject = new NomenclatureFastSearchResult();

        /// <summary>
        /// Добавляет новый объект с полями описывающими торговую марку/артикул номенклатуры в список добавленных объектов, если такой еще не был добавлен.
        /// </summary>
        /// <returns>Был ли добавлен объект или он уже был добален ранее</returns>
        public bool AddIfNotContains( string article, long tradeMarkId )
            {
            searchObject.SetSearchState( article, tradeMarkId );
            if (this.internalSet.Contains( searchObject ))
                {
                return false;
                }
            this.internalSet.Add( new NomenclatureFastSearchResult( article, tradeMarkId ) );
            return true;
            }
        /// <summary>
        /// Очищает список добавленных объектов
        /// </summary>
        public void Clear()
            {
            internalSet.Clear();
            }

        /// <summary>
        /// Хранит в себе информацию об артикуле/торговой марке
        /// </summary>
        private class NomenclatureFastSearchResult
            {
            public string Article;
            public long TradeMarkId;
            private int hash = 0;

            public NomenclatureFastSearchResult() { }

            public NomenclatureFastSearchResult( string article, long tradeMarkId )
                {
                this.SetSearchState( article, tradeMarkId );
                }

            public void SetSearchState( string article, long tradeMarkId )
                {
                Article = article;
                TradeMarkId = tradeMarkId;
                hash = Article.GetHashCode() ^ tradeMarkId.GetHashCode();
                }

            public override int GetHashCode()
                {
                return hash;
                }

            public override bool Equals( object obj )
                {
                NomenclatureFastSearchResult fastSearchResult = (NomenclatureFastSearchResult)obj;
                return TradeMarkId == fastSearchResult.TradeMarkId && Article.Equals( fastSearchResult.Article );
                }
            }
        }

    }
