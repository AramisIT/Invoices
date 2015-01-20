using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.SystemObjects;
using Aramis.Core;
using Aramis.Attributes;
using Aramis.DatabaseConnector;
using Aramis.Enums;
using AramisInfostructure.Queries;
using Core;

namespace SystemInvoice.Catalogs
    {
    /// <summary>
    /// Справочник. Содержит описание торговой марки с указанием производителя
    /// </summary>
    [Catalog(Description = "Торговые марки", GUID = "ACEC45C5-16C3-4762-B535-25750EFE3273", DescriptionSize = 40,
        HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false)]
    public interface ITradeMark : ICatalog
        {
        [DataField(Description = "Контрагент", UseForFastInput = UseFieldForFastInput.LoadButNotDisplay, ShowInList = true)]
        IContractor Contractor { get; set; }
        }

    public class TradeMarkListsGetter : FixedListsCreator<ITradeMark>
        {
        public enum TradeMarkListsTypes
            {
            FilteredByContractor
            }

        public override IQuery GetQuery(int listId, IAramisModel aramisObject)
            {
            var newGoodsRow = aramisObject as INewGoodsRow;
            if (newGoodsRow == null) return null;

            var listType = (TradeMarkListsTypes)listId;
            switch (listType)
                {
                case TradeMarkListsTypes.FilteredByContractor:
                    var q = DB.NewQuery(@"select rtrim(Description) [Description], Id 
from TradeMark
where Contractor = @Contractor
order by Description");
                    q.AddInputParameter("Contractor", newGoodsRow.Contractor.Id);
                    return q;

                default:
                    return null;
                }
            }
        }
    }
