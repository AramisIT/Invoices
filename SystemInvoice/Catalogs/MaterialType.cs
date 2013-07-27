using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Attributes;
using Aramis.Core;
using Aramis.Enums;

namespace SystemInvoice.Catalogs
    {
    /// <summary>
    /// Справочник. Содержит в себе описание типов материалов ткани
    /// </summary>
    [Catalog(Description = "Типы материалов", GUID = "BBA8F5F9-247C-449C-986E-3C55233B84F8", DescriptionSize = 300, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false)]
    public class MaterialType : CatalogTable
        {
        #region (string) CustomsCodeKeyWord Описание в таможенном коде
        [DataField(Description = "Описание в таможенном коде")]
        public string CustomsCodeKeyWord
            {
            get
                {
                return z_CustomsCodeKeyWord;
                }
            set
                {
                if (z_CustomsCodeKeyWord == value)
                    {
                    return;
                    }

                z_CustomsCodeKeyWord = value;
                NotifyPropertyChanged("CustomsCodeKeyWord");
                }
            }

        private string z_CustomsCodeKeyWord = string.Empty;

        #endregion
        }
    }
