using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.UI.DBObjectsListFilter;
using Aramis.Core;
using SystemInvoice.Catalogs;

namespace SystemInvoice.PropsSyncronization
    {
    public class TrademarkContractorSubGroupOfGoodsSyncronizer : TradeMarkContractorSyncronizer
        {
        private ITradeMarkContractorSubGroupOfGoodsSource goodsDataSource = null;

        private SubGroupOfGoods SubGroupOfGoods
            {
            get
                {
                if (goodsDataSource == null) { return null; }
                return goodsDataSource.SubGroupOfGoods;
                }
            set
                {
                if (goodsDataSource != null) { goodsDataSource.SubGroupOfGoods = value; }
                }
            }

        private string GropOfGoods
            {
            get
                {
                if (goodsDataSource == null) { return null; }
                return goodsDataSource.GroupOfGoods;
                }
            set
                {
                if (goodsDataSource != null) { goodsDataSource.GroupOfGoods = value; }
                }
            }

        public TrademarkContractorSubGroupOfGoodsSyncronizer(Aramis.Core.DatabaseObject dbObject)
            : base(dbObject)
            {
            this.goodsDataSource = dbObject as ITradeMarkContractorSubGroupOfGoodsSource;
            }

        protected override void onPropertyChanged(string propertyName)
            {
            base.onPropertyChanged(propertyName);
            if (propertyName.Equals("SubGroupOfGoods"))
                {
                if (this.SubGroupOfGoods.Id != 0)
                    {
                    onSubGroupOfGoodsChanged();
                    }
                }
            }

        protected virtual void onSubGroupOfGoodsChanged()
            {
            if (SubGroupOfGoods.Contractor.Id != this.Contractor.Id && this.SubGroupOfGoods.Contractor.Id != 0)
                {
                this.Contractor = new Contractor() { Id = SubGroupOfGoods.Contractor.Id };
                }
            if (SubGroupOfGoods.TradeMark.Contractor.Id == SubGroupOfGoods.Contractor.Id && SubGroupOfGoods.TradeMark.Id != 0 && SubGroupOfGoods.TradeMark.Id != TradeMark.Id)
                {
                this.TradeMark = new TradeMark() { Id = SubGroupOfGoods.TradeMark.Id, Contractor = new Contractor() { Id = SubGroupOfGoods.Contractor.Id } };
                }
            if (SubGroupOfGoods.GroupOfGoods.Id != 0)
                {
                this.GropOfGoods = SubGroupOfGoods.GroupOfGoods.Description;
                }
            }

        protected override void onContractorChanged()
            {
            base.onContractorChanged();
            if (SubGroupOfGoods.Id != 0 && SubGroupOfGoods.Contractor.Id != 0 && SubGroupOfGoods.Contractor.Id != this.Contractor.Id)
                {
                this.SubGroupOfGoods = new SubGroupOfGoods();
                }
            }

        protected override void setFilterForProperty(string propertyName, out Aramis.Core.GetListFilterDelegate filterDelegate)
            {
            base.setFilterForProperty(propertyName, out filterDelegate);
            if (propertyName.Equals("SubGroupOfGoods"))
                {
                filterDelegate = SubGroupOfGoodsFilter;
                }
            }

        private ListFilter SubGroupOfGoodsFilter()
            {
            ListFilter result = new ListFilter(typeof(SubGroupOfGoods));
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
