using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;
using SystemInvoice.PropsSyncronization;
using Aramis.Core;

namespace SystemInvoice.DataProcessing.CatalogsProcessing.Loaders
    {
    /// <summary>
    /// Содержи данные о контрагенте/торговой марке, которые нужно использовать при построении кеша
    /// </summary>
    class CacheTradeMarkContractorSource : ITradeMarkContractorSource
        {
        private IContractor contractor = null;
        private ITradeMark tradeMark = null;
        public IContractor Contractor
            {
            get
                {
                if (contractor == null)
                    {
                    contractor = A.New<IContractor>();
                    }
                return contractor;
                }
            set { contractor = value; }
            }

        public ITradeMark TradeMark
            {
            get
                {
                if (tradeMark == null)
                    {
                    tradeMark = A.New<ITradeMark>();
                    }
                return tradeMark;
                }
            set { tradeMark = value; }
            }

        }
    }
