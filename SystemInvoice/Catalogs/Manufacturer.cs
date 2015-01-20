using Aramis.Attributes;
using Aramis.Core;
using Aramis.DatabaseConnector;
using Aramis.Enums;
using AramisInfostructure.Queries;
using Core;
using SystemInvoice.SystemObjects;

namespace SystemInvoice.Catalogs
    {
    /// <summary>
    /// Справочник хранит данные о заводе принадлежащем контрагенту
    /// </summary>
    [Catalog(Description = "Производитель", GUID = "BE79F0CB-DDB4-4FC6-B8D8-F86A7D808959", DescriptionSize = 200, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false)]
    public interface IManufacturer : ICatalog
        {
        [DataField(Description = "Контрагент", UseForFastInput = UseFieldForFastInput.LoadButNotDisplay, ShowInList = true)]
        IContractor Contractor { get; set; }
        }

    public class ManufacturerListsGetter : FixedListsCreator<IManufacturer>
        {
        public enum ManufacturerListsTypes
            {
            FilteredByContractor
            }

        public override IQuery GetQuery(int listId, IAramisModel aramisObject)
            {
            var newGoodsRow = aramisObject as INewGoodsRow;
            if (newGoodsRow == null) return null;

            var listType = (ManufacturerListsTypes)listId;
            switch (listType)
                {
                case ManufacturerListsTypes.FilteredByContractor:
                    var q = DB.NewQuery(@"select rtrim(Description) [Description], Id 
from Manufacturer
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
