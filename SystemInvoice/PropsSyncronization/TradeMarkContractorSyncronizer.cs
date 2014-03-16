using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.UI.DBObjectsListFilter;
using Aramis.Core;
using SystemInvoice.Catalogs;

namespace SystemInvoice.PropsSyncronization
    {
    public class TradeMarkContractorSyncronizer
        {
        ITradeMarkContractorSource tradeMarkContractorSource = null;
        Aramis.Core.DatabaseObject dbObject = null;

        protected Contractor Contractor
            {
            get
                {
                if (tradeMarkContractorSource != null) { return tradeMarkContractorSource.Contractor; }
                return null;
                }
            set
                {
                if (tradeMarkContractorSource != null) { tradeMarkContractorSource.Contractor = value; }
                }
            }

        public TradeMark TradeMark
            {
            get
                {
                if (tradeMarkContractorSource != null) { return tradeMarkContractorSource.TradeMark; }
                return null;
                }
            set
                {
                if (tradeMarkContractorSource != null) { tradeMarkContractorSource.TradeMark = value; }
                }
            }

        public TradeMarkContractorSyncronizer(Aramis.Core.DatabaseObject dbObject)
            {
            tradeMarkContractorSource = dbObject as ITradeMarkContractorSource;
            this.dbObject = dbObject;
            dbObject.ValueOfObjectPropertyChanged += dbObject_ValueOfObjectPropertyChanged;
            }

        public virtual void RefreshAll()
            {
            onTradeMarkChanged();
            onContractorChanged();
            }

        void dbObject_ValueOfObjectPropertyChanged(string propertyName)
            {
            if (dbObject != null && tradeMarkContractorSource != null)
                {
                onPropertyChanged(propertyName);
                }
            }

        protected virtual void onPropertyChanged(string propertyName)
            {
            switch (propertyName)
                {
                case ("TradeMark"):
                        {
                        if (this.TradeMark.Id != 0)
                            {
                            onTradeMarkChanged();
                            }
                        break;
                        }
                case ("Contractor"):
                        {
                        if (this.Contractor.Id != 0)
                            {
                            onContractorChanged();
                            }
                        break;
                        }
                default: break;
                }
            }

        protected virtual void onContractorChanged()
            {
            if (TradeMark.Contractor.Id != this.Contractor.Id)
                {
                this.TradeMark = new TradeMark();
                }
            }

        protected virtual void onTradeMarkChanged()
            {
            if (this.TradeMark.Id != 0 && this.Contractor.Id != TradeMark.Contractor.Id)
                {
                this.Contractor = new Contractor() { Id = TradeMark.Contractor.Id };
                }
            }

        public GetListFilterDelegate GetFuncGetCustomFilter(string propertyName)
            {
            GetListFilterDelegate listFilter = null;
            setFilterForProperty(propertyName, out listFilter);
            return listFilter;
            }

        protected virtual void setFilterForProperty(string propertyName, out  GetListFilterDelegate filterDelegate)
            {
            filterDelegate = null;
            if (propertyName.Equals("TradeMark"))
                {
                filterDelegate = tradeMarkFilter;
                }
            }

        private ListFilter tradeMarkFilter()
            {
            ListFilter result = new ListFilter(typeof(TradeMark));
            if (this.Contractor.Id != 0)
                {
                result.Conditions["Contractor"].Active = true;
                (result.Conditions["Contractor"] as ConditionForNonHierarchyCatalog).Value.SingleValue = this.Contractor;
                }
            return result;
            }
        }
    }
