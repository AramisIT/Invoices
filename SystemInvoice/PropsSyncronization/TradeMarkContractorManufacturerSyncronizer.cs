﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.UI.DBObjectsListFilter;
using Aramis.Core;
using SystemInvoice.Catalogs;

namespace SystemInvoice.PropsSyncronization
    {
    public class TradeMarkContractorManufacturerSyncronizer : TradeMarkContractorSyncronizer
        {
        private ITradeMarkContractorManufacturerSource manufacturerDataSource = null;

        private IManufacturer Manufacturer
            {
            get
                {
                if (manufacturerDataSource == null) { return null; }
                return manufacturerDataSource.Manufacturer;
                }
            set
                {
                if (manufacturerDataSource != null) { manufacturerDataSource.Manufacturer = value; }
                }
            }

        public TradeMarkContractorManufacturerSyncronizer(IDatabaseObject dbObj)
            : base(dbObj)
            {
            this.manufacturerDataSource = dbObj as ITradeMarkContractorManufacturerSource;
            }

        protected override void onPropertyChanged(string propertyName)
            {
            base.onPropertyChanged(propertyName);
            if (propertyName.Equals("Manufacturer"))
                {
                if (this.Manufacturer.Id != 0)
                    {
                    onManufacturerChanged();
                    }
                }
            }

        private void onManufacturerChanged()
            {
            if (this.Manufacturer.Id != 0 && this.Contractor.Id != this.Manufacturer.Contractor.Id)
                {
                this.Contractor = A.New<IContractor>();
                this.Contractor.Id = this.Manufacturer.Contractor.Id;
                }
            }

        protected override void onContractorChanged()
            {
            base.onContractorChanged();
            if (this.Manufacturer.Contractor.Id != this.Contractor.Id)
                {
                this.Manufacturer = A.New<IManufacturer>();
                }
            }

        protected override void setFilterForProperty(string propertyName, out GetListFilterDelegate filterDelegate)
            {
            base.setFilterForProperty(propertyName, out filterDelegate);
            if (propertyName.Equals("Manufacturer"))
                {
                filterDelegate = manufacturerFilter;
                }
            }

        private ListFilter manufacturerFilter()
            {
            ListFilter result = new ListFilter(typeof(IManufacturer));
            if (this.Contractor.Id != 0)
                {
                result.Conditions["Contractor"].Active = true;
                (result.Conditions["Contractor"] as ConditionForNonHierarchyCatalog).Value.SingleValue = this.Contractor;
                }
            return result;
            }
        }
    }
