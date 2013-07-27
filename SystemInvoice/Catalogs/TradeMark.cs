﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Core;
using Aramis.Attributes;
using Aramis.Enums;

namespace SystemInvoice.Catalogs
    {
    /// <summary>
    /// Справочник. Содержит описание торговой марки с указанием производителя
    /// </summary>
    [Catalog( Description = "Торговые марки", GUID = "ACEC45C5-16C3-4762-B535-25750EFE3273", DescriptionSize = 40, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false )]
    public class TradeMark : CatalogTable
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