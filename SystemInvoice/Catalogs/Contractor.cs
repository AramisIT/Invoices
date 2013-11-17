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
    /// Справочник. Содержит описание контрагентов
    /// </summary>
    [Catalog(Description = "Контрагенты", GUID = "A72E0B94-F792-44F6-A60C-ECAC7AF8C41A", DescriptionSize = 120, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false)]
    public class Contractor : CatalogTable
        {
        #region (bool) UseComodityPrices Использовать биржевые цены
        [DataField(Description = "Использовать биржевые цены")]
        public bool UseComodityPrices
            {
            get
                {
                return z_UseComodityPrices;
                }
            set
                {
                if (z_UseComodityPrices == value)
                    {
                    return;
                    }

                z_UseComodityPrices = value;
                NotifyPropertyChanged("UseComodityPrices");
                }
            }
        private bool z_UseComodityPrices = false;
        #endregion

        #region (RelationWithCustomersType) RelationWithCustomersType Тип взаимоотношений
        [DataField(Description = "Тип взаимоотношений")]
        public RelationWithCustomersType RelationWithCustomersType
            {
            get
                {
                return (RelationWithCustomersType)GetValueForObjectProperty("RelationWithCustomersType");
                }
            set
                {
                SetValueForObjectProperty("RelationWithCustomersType", value);
                }
            }
        #endregion

        #region (Country) Country Страна
        [DataField(Description = "Страна")]//Размер в документе указан 15, но у самой страны может быть 20 поэтому тут не указывал
        public Country Country
            {
            get
                {
                return (Country)GetValueForObjectProperty("Country");
                }
            set
                {
                SetValueForObjectProperty("Country", value);
                }
            }
        #endregion

        #region (string) PhisicalAddress Физический адрес
        [DataField(Description = "Физический адрес", Size = 40)]
        public string PhisicalAddress
            {
            get
                {
                return z_PhisicalAddress;
                }
            set
                {
                if (z_PhisicalAddress == value)
                    {
                    return;
                    }

                z_PhisicalAddress = value;
                NotifyPropertyChanged("PhisicalAddress");
                }
            }
        private string z_PhisicalAddress = "";
        #endregion

        #region (string) JuridicalAddress Юридический адресс
        [DataField(Description = "Юридический адресс", Size = 40)]
        public string JuridicalAddress
            {
            get
                {
                return z_JuridicalAddress;
                }
            set
                {
                if (z_JuridicalAddress == value)
                    {
                    return;
                    }

                z_JuridicalAddress = value;
                NotifyPropertyChanged("JuridicalAddress");
                }
            }
        private string z_JuridicalAddress = "";
        #endregion

        #region (string) ContractorCode Код
        [DataField(Description = "Код", NotEmpty = true, Size = 50, ShowInList = true)]
        public string ContractorCode
            {
            get
                {
                return z_ContractorCode;
                }
            set
                {
                if (z_ContractorCode == value)
                    {
                    return;
                    }

                z_ContractorCode = value;
                NotifyPropertyChanged("ContractorCode");
                }
            }
        private string z_ContractorCode = "";
        #endregion

        #region (string) Phone Телефон
        [DataField(Description = "Телефон", Size = 50)]
        public string Phone
            {
            get
                {
                return z_Phone;
                }
            set
                {
                if (z_Phone == value)
                    {
                    return;
                    }

                z_Phone = value;
                NotifyPropertyChanged("Phone");
                }
            }
        private string z_Phone = "";
        #endregion
        
        }
    }
