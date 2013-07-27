using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Core;
using SystemInvoice.Catalogs;
using Aramis.UI.DBObjectsListFilter;

namespace SystemInvoice.PropsSyncronization
    {
    public class TradeMarkContractorAprovalsLoadFormatSyncronizer : TradeMarkContractorSyncronizer
        {        
        ITradeMarkContractorApprovalsLoadFormatSource excelLoadingSource = null;

        public ApprovalsLoadFormat ApprovalsLoadFormat
            {
            get
                {
                if (excelLoadingSource == null) { return null; }
                return excelLoadingSource.ApprovalsLoadFormat;
                }
            set
                {
                if (excelLoadingSource != null) { excelLoadingSource.ApprovalsLoadFormat = value; }
                }
            }

        public TradeMarkContractorAprovalsLoadFormatSyncronizer( DatabaseObject dbObject )
            : base( dbObject )
            {
            this.excelLoadingSource = dbObject as ITradeMarkContractorApprovalsLoadFormatSource;
            }

        public override void RefreshAll()
            {
            onApprovalsLoadFormatChanged();
            base.RefreshAll();
            }

        protected override void onPropertyChanged( string propertyName )
            {
            base.onPropertyChanged( propertyName );
            if (propertyName.Equals( "ApprovalsLoadFormat" ))
                {
                if (ApprovalsLoadFormat.Id != 0)
                    {
                    onApprovalsLoadFormatChanged();
                    }
                }
            }

        protected override void onContractorChanged()
            {
            base.onContractorChanged();
            if (ApprovalsLoadFormat.Id != 0 && ApprovalsLoadFormat.Contractor.Id != 0 && ApprovalsLoadFormat.Contractor.Id != this.Contractor.Id)
                {
                this.ApprovalsLoadFormat = new ApprovalsLoadFormat();
                }
            }

        protected override void onTradeMarkChanged()
            {
            base.onTradeMarkChanged();
            if (ApprovalsLoadFormat.Id != 0 && ApprovalsLoadFormat.TradeMark.Id != this.TradeMark.Id)
                {
                this.ApprovalsLoadFormat = new ApprovalsLoadFormat();
                }
            }

        private void onApprovalsLoadFormatChanged()
            {
            if (ApprovalsLoadFormat.Contractor.Id != this.Contractor.Id && this.ApprovalsLoadFormat.Contractor.Id != 0)
                {
                this.Contractor = new Contractor() { Id = ApprovalsLoadFormat.Contractor.Id };
                }
            if (ApprovalsLoadFormat.TradeMark.Id != TradeMark.Id)
                {
                this.TradeMark = new TradeMark() { Id = ApprovalsLoadFormat.TradeMark.Id, Contractor = new Contractor() { Id = ApprovalsLoadFormat.Contractor.Id } };
                }
            }

        protected override void setFilterForProperty( string propertyName, out GetListFilterDelegate filterDelegate )
            {
            base.setFilterForProperty( propertyName, out filterDelegate );
            if (propertyName.Equals( "ApprovalsLoadFormat" ))
                {
                filterDelegate = approvalsLoadingFormatFilter;
                }
            }

        private ListFilter approvalsLoadingFormatFilter()
            {
            ListFilter result = new ListFilter( "ApprovalsLoadFormat" );
            if (this.Contractor.Id != 0)
                {
                result.Conditions["Contractor"].Active = true;
                (result.Conditions["Contractor"] as ConditionForNonHierarchyCatalog).Value.SingleValue = this.Contractor;
                }
            if (this.TradeMark.Id != 0)
                {
                result.Conditions["TradeMark"].Active = true;
                (result.Conditions["TradeMark"] as ConditionForNonHierarchyCatalog).Value.SingleValue = this.TradeMark;
                }
            return result;
            }
        }
    }
