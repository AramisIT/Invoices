using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Core;
using Aramis.Attributes;
using Aramis.Enums;

namespace SystemInvoice.Catalogs
    {
    [Catalog(Description = "Контрагенты", GUID = "A72E0B94-F792-44F6-A60C-ECAC7AF8C41A", DescriptionSize = 120, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false)]
    public interface IContractor : ICatalog
        {
        [DataField(Description = "Использовать биржевые цены")]
        bool UseComodityPrices { get; set; }

        [DataField(Description = "Тип взаимоотношений")]
        RelationWithCustomersType RelationWithCustomersType { get; set; }

        [DataField(Description = "Страна")]//Размер в документе указан 15, но у самой страны может быть 20 поэтому тут не указывал
        Country Country { get; set; }

        [DataField(Description = "Физический адрес", Size = 40)]
        string PhisicalAddress { get; set; }

        [DataField(Description = "Юридический адресс", Size = 40)]
        string JuridicalAddress { get; set; }

        [DataField(Description = "Код", NotEmpty = true, Size = 50, ShowInList = true)]
        string ContractorCode { get; set; }

        [DataField(Description = "Телефон", Size = 50)]
        string Phone { get; set; }

        [DataField(Description = "Разрешить ручное заполнение")]
        bool AllowManualFilling { get; set; }

        [DataField(Description = "Не загружать сертификаты в инвойс")]
        bool Don_tLoadCertToInvoice { get; set; }
        }
    }
