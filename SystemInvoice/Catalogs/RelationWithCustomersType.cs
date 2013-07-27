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
    /// Справочник. Состоит из имени взаимоотношения с заказчиками.
    /// </summary>
    [Catalog( Description = "Типы взаимоотношений с заказчиками ", GUID = "A786B046-80AB-4DCD-ABBA-5EC0A6EF8AEF", DescriptionSize = 100, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false )]
    public class RelationWithCustomersType : CatalogTable
        {
        //Включает только одно поле Наименование(Description)
        }
    }
