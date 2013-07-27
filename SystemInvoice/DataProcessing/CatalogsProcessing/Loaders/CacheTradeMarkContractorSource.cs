using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;
using SystemInvoice.PropsSyncronization;

namespace SystemInvoice.DataProcessing.CatalogsProcessing.Loaders
    {
    /// <summary>
    /// Содержи данные о контрагенте/торговой марке, которые нужно использовать при построении кеша
    /// </summary>
    class CacheTradeMarkContractorSource : ITradeMarkContractorSource
        {
        private Contractor contractor = null;
        private TradeMark tradeMark = null;
        public Contractor Contractor
            {
            get
                {
                if (contractor == null)
                    {
                    contractor = new Contractor() { Id = 0 };
                    }
                return contractor;
                }
            set { contractor = value; }
            }

        public TradeMark TradeMark
            {
            get
                {
                if (tradeMark == null)
                    {
                    tradeMark = new TradeMark() { Id = 0 };
                    }
                return tradeMark;
                }
            set { tradeMark = value; }
            }

        }
    }
