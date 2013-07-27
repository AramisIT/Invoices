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
    /// Справочник. Содержит названия страны и ее междкнародный код
    /// </summary>
    [Catalog( Description = "Страны", GUID = "A3A1E536-CA5A-479E-A79D-A0EF598FEAC9", DescriptionSize = 50, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false )]
    public class Country : CatalogTable
        {
        #region Свойства

        #region (string) NameRus Наименование рос.
        [DataField( Description = "Наименование рос.", Size = 50, NotEmpty = true )]
        public string NameRus
            {
            get
                {
                return z_NameRus;
                }
            set
                {
                if (z_NameRus == value)
                    {
                    return;
                    }

                z_NameRus = value;
                NotifyPropertyChanged( "NameRus" );
                }
            }
        private string z_NameRus = "";
        #endregion

        #region (string) NameEng Наименование англ.
        [DataField( Description = "Наименование англ.", Size = 50 )]
        public string NameEng
            {
            get
                {
                return z_NameEng;
                }
            set
                {
                if (z_NameEng == value)
                    {
                    return;
                    }
                z_NameEng = value;
                NotifyPropertyChanged( "NameEng" );
                }
            }
        private string z_NameEng = "";
        #endregion

        #region (string) InternationalCode Код международный
        [DataField( Description = "Код международный", Size = 25, NotEmpty = true, UseForFastInput = UseFieldForFastInput.LoadButNotDisplay, ShowInList = true )]
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

        #region (int) InternationalDigitCode Международный цифровой код
        [DataField( Description = "Международный цифровой код", Size = 3 )]
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

        #endregion
        }
    }
