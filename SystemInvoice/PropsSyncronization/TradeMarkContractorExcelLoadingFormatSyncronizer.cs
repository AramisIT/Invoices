using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;
using Aramis.UI.DBObjectsListFilter;
using Aramis.Core;

namespace SystemInvoice.PropsSyncronization
    {
    public class TradeMarkContractorExcelLoadingFormatSyncronizer : TradeMarkContractorSyncronizer
        {
        ITradeMarkContractorExcelLoadFormatSource excelLoadingSource = null;

        public ExcelLoadingFormat ExcelLoadingFormat
            {
            get
                {
                if (excelLoadingSource == null) { return null; }
                return excelLoadingSource.ExcelLoadingFormat;
                }
            set
                {
                if (excelLoadingSource != null) { excelLoadingSource.ExcelLoadingFormat = value; }
                }
            }

        public TradeMarkContractorExcelLoadingFormatSyncronizer(DatabaseObject dbObject)
            : base(dbObject)
            {
            this.excelLoadingSource = dbObject as ITradeMarkContractorExcelLoadFormatSource;
            }

        public override void RefreshAll()
            {
            onExcelLoadingFormatChanged();
            base.RefreshAll();
            }

        protected override void onPropertyChanged(string propertyName)
            {
            base.onPropertyChanged(propertyName);
            if (propertyName.Equals("ExcelLoadingFormat"))
                {
                if (ExcelLoadingFormat.Id != 0)
                    {
                    onExcelLoadingFormatChanged();
                    }
                }
            }

        protected override void onContractorChanged()
            {
            base.onContractorChanged();
            if (ExcelLoadingFormat.Id != 0 && ExcelLoadingFormat.Contractor.Id != 0 && ExcelLoadingFormat.Contractor.Id != this.Contractor.Id)
                {
                this.ExcelLoadingFormat = new ExcelLoadingFormat();
                }
            }

        protected override void onTradeMarkChanged()
            {
            base.onTradeMarkChanged();
            if (ExcelLoadingFormat.Id != 0 && ExcelLoadingFormat.TradeMark.Id != this.TradeMark.Id)
                {
                this.ExcelLoadingFormat = new ExcelLoadingFormat();
                }
            }

        private void onExcelLoadingFormatChanged()
            {
            if (ExcelLoadingFormat.Contractor.Id != this.Contractor.Id && this.ExcelLoadingFormat.Contractor.Id != 0)
                {
                this.Contractor = new Contractor() { Id = ExcelLoadingFormat.Contractor.Id };
                }
            if (ExcelLoadingFormat.TradeMark.Id != TradeMark.Id)
                {
                this.TradeMark = new TradeMark() { Id = ExcelLoadingFormat.TradeMark.Id, Contractor = new Contractor() { Id = ExcelLoadingFormat.Contractor.Id } };
                }
            }

        protected override void setFilterForProperty(string propertyName, out GetListFilterDelegate filterDelegate)
            {
            base.setFilterForProperty(propertyName, out filterDelegate);
            if (propertyName.Equals("ExcelLoadingFormat"))
                {
                filterDelegate = excelLoadingFormatFilter;
                }
            }

        private ListFilter excelLoadingFormatFilter()
            {
            ListFilter result = new ListFilter(typeof(ExcelLoadingFormat));
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
