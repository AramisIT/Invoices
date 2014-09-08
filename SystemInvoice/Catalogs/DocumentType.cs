using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Core;
using Aramis.Attributes;
using Aramis.Enums;
using Aramis.DatabaseConnector;

namespace SystemInvoice.Catalogs
    {
    /// <summary>
    /// Справочник. Представляет собой описание и код документа
    /// </summary>
    [Catalog(Description = "Типы документов", GUID = "AAB49F34-FDD3-43C9-853F-E44568DAD9F2", DescriptionSize = 450, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false)]
    public interface IDocumentType : ICatalog
        {
        [DataField(Description = "Код документа по классификатору", Size = 100, NotEmpty = true, ShowInList = true)]
        string QualifierCodeName { get; set; }

        [DataField(Description = "Удалять разрешительные при изменении номенклатуры")]
        bool DeleteApproval { get; set; }
        }

    public class DocumentTypeHelper
        {
        private static IDocumentType certificateType;

        public static IDocumentType GetCertificateType()
            {
            if (certificateType == null)
                {
                var certificateTypeIdList = DB.NewQuery("select cap.Id from DocumentType cap where cap.QualifierCodeName = 5111 and cap.MarkForDeleting = 0").SelectToList<long>();
                certificateType = A.New<IDocumentType>(certificateTypeIdList.Count == 1 ? certificateTypeIdList.First() : 0);
                }
            return certificateType;
            }
        }
    }
