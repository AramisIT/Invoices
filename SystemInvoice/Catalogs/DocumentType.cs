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
    /// Справочник. Представляет собой описание и код документа
    /// </summary>
    [Catalog( Description = "Типы документов", GUID = "AAB49F34-FDD3-43C9-853F-E44568DAD9F2", DescriptionSize = 450, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false )]
    public class DocumentType : CatalogTable
        {
        #region (string) QualifierCodeName Код документа по классификатору
        [DataField( Description = "Код документа по классификатору", Size = 100, NotEmpty = true, ShowInList = true )]
        public string QualifierCodeName
            {
            get
                {
                return z_QualifierCodeName;
                }
            set
                {
                if (z_QualifierCodeName == value)
                    {
                    return;
                    }

                z_QualifierCodeName = value;
                NotifyPropertyChanged( "QualifierCodeName" );
                }
            }
        private string z_QualifierCodeName = "";
        #endregion

        #region (bool) DeleteApproval Удалять разрешительные при изменении номенклатуры
        [DataField(Description = "Удалять разрешительные при изменении номенклатуры")]
        public bool DeleteApproval
            {
            get
                {
                return z_DeleteApproval;
                }
            set
                {
                if (z_DeleteApproval == value)
                    {
                    return;
                    }

                z_DeleteApproval = value;
                NotifyPropertyChanged("DeleteApproval");
                }
            }
        private bool z_DeleteApproval = false;
        #endregion
        }
    }
