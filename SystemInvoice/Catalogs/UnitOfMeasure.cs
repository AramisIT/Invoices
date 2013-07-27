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
    /// Справочник единиц измерения
    /// </summary>
    [Catalog( Description = "Единицы измерения", GUID = "AC310BB2-178A-4E97-BFAF-C926F030F0FF", DescriptionSize = 30, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false )]
    public class UnitOfMeasure : CatalogTable
        {
        #region Свойства

        #region (string) ShortName Сокращение
        [DataField( Description = "Сокращение", Size = 30, UseForFastInput = UseFieldForFastInput.LoadButNotDisplay, ShowInList = true )]
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

        #region (string) InternationalCode Международный код
        [DataField( Description = "Международный код", Size = 30, UseForFastInput = UseFieldForFastInput.LoadAndDisplay, ShowInList = true )]
        public string InternationalCode
            {
            get
                {
                return z_InternationalCode;
                }
            set
                {
                if (z_InternationalCode == value)
                    {
                    return;
                    }

                z_InternationalCode = value;
                NotifyPropertyChanged( "InternationalCode" );
                }
            }
        private string z_InternationalCode = "";
        #endregion

        #endregion
        }
    }
