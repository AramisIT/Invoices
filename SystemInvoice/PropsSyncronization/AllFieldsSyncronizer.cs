using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Core;
using SystemInvoice.Catalogs;

namespace SystemInvoice.PropsSyncronization
    {
    public class AllFieldsSyncronizer:TradeMarkContractorManufacturerSyncronizer
        {
        private TrademarkContractorSubGroupOfGoodsSyncronizer SubGroupOfGoodsSyncronizer = null;

        public AllFieldsSyncronizer( DatabaseObject dbObject )
            : base( dbObject )
            {
            SubGroupOfGoodsSyncronizer = new TrademarkContractorSubGroupOfGoodsSyncronizer( dbObject );
            }

        protected override void setFilterForProperty( string propertyName, out GetListFilterDelegate filterDelegate )
            {
            base.setFilterForProperty( propertyName, out filterDelegate );
            if (filterDelegate == null)
                {
                filterDelegate = SubGroupOfGoodsSyncronizer.GetFuncGetCustomFilter( propertyName );
                }
            }
        }
    }
