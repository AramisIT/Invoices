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
    /// Справочник. Содержит описание валюты
    /// </summary>
    [Catalog( Description = "Валюта", GUID = "AAB8465F-3D7A-44D7-9CC6-B63FE9D7453B", DescriptionSize = 50, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false )]
    public class Currency : CatalogTable
        {
        #region (string) ShortName Сокращение
        [DataField( Description = "Сокращение", Size = 70, ShowInList = true )]
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


        #region (int) InternationalDigitCode Международный цифровой код
        [DataField( Description = "Международный цифровой код", Size = 50, DecimalPointsSummaryNumber = 0, DecimalPointsViewNumber = 0, DecimalPointsNumber = 0, ShowInList = true )]
        public int InternationalDigitCode
            {
            get
                {
                return z_InternationalDigitCode;
                }
            set
                {
                if (z_InternationalDigitCode == value)
                    {
                    return;
                    }

                z_InternationalDigitCode = value;
                NotifyPropertyChanged( "InternationalDigitCode" );
                }
            }
        private int z_InternationalDigitCode = 0;
        #endregion
        }
    }
