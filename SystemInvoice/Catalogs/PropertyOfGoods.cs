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
    /// Справочник. Описывает свойство товара
    /// </summary>
    [Catalog( Description = "Свойства товара", GUID = "AFCC5D07-40E6-4B78-979F-3A84645D5AD5", DescriptionSize = 30, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false )]
    public class PropertyOfGoods : CatalogTable
        {
        #region (string) ShortName Сокращение
        [DataField( Description = "Сокращение",Size = 30 )]
        public string ShortName
            {
            get
                {
                return z_ShortName;
                }
            set
                {
                if (z_ShortName == value)
                    {
                    return;
                    }

                z_ShortName = value;
                NotifyPropertyChanged( "ShortName" );
                }
            }
        private string z_ShortName = "";
        #endregion
        }
    }
