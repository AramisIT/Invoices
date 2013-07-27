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
    /// Справочник. Классы товара.
    /// </summary>
    [Catalog( Description = "Группы", GUID = "A70CEFDB-5B25-4148-BE12-CEE220A95708", DescriptionSize = 40, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false )]
    public class GroupOfGoods : CatalogTable
        {
        //#region (string) ClassCode Код
        //[DataField( Description = "Код" )]
        //public string ClassCode
        //    {
        //    get
        //        {
        //        return z_ClassCode;
        //        }
        //    set
        //        {
        //        if (z_ClassCode == value)
        //            {
        //            return;
        //            }
        //        z_ClassCode = value;
        //        NotifyPropertyChanged( "ClassCode" );
        //        }
        //    }
        //private string z_ClassCode = "";
        //#endregion
        }
    }
