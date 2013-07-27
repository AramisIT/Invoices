using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Core;
using Aramis.Attributes;
using Aramis.Enums;

namespace SystemInvoice.Catalogs
    {
    /// <summary>
    /// Справочник хранит данные о заводе принадлежащем контрагенту
    /// </summary>
    [Catalog( Description = "Производитель", GUID = "BE79F0CB-DDB4-4FC6-B8D8-F86A7D808959", DescriptionSize = 200, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false )]
    public class Manufacturer : CatalogTable
        {
        #region (Contractor) Contractor Контрагент
        [DataField( Description = "Контрагент", UseForFastInput = UseFieldForFastInput.LoadButNotDisplay, ShowInList = true )]
        public Contractor Contractor
            {
            get
                {
                return (Contractor)GetValueForObjectProperty( "Contractor" );
                }
            set
                {
                SetValueForObjectProperty( "Contractor", value );
                }
            }
        #endregion
        }
    }
