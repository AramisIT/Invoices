using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;
using Aramis.Attributes;
using Aramis.Core;
using Aramis.Enums;

namespace SystemInvoice.Documents
    {
    /// <summary>
    /// Системный документ. Содержит в себе в себе информацию об удаленной из разрешительных документов номенклатуре
    /// </summary>
    [Document(Description = "Соответствие типов материалов", GUID = "09D34802-A372-4DDE-938E-3CB37B8402EF", NumberType = NumberType.Int64, ShowLastModifiedDateInList = false, ShowResponsibleInList = false,
        ShowLastModifiedDate = false, ShowDateInForm = false, ShowNumberInForm = false,ShowCreationDate = false,ShowDateInList = false)]
    public class MaterialTypeMapping : DocumentTable
        {

        #region (MaterialType) MaterialType Тип материала
        [DataField(Description = "Тип материала", ShowInList = true)]
        public MaterialType MaterialType
            {
            get
                {
                return (MaterialType)GetValueForObjectProperty("MaterialType");
                }
            set
                {
                SetValueForObjectProperty("MaterialType", value);
                }
            }
        #endregion

        #region (string) MaterialKeyWord Ключевое слово в составе
        [DataField(Description = "Ключевое слово в составе", ShowInList = true)]
        public string MaterialKeyWord
            {
            get
                {
                return z_MaterialKeyWord;
                }
            set
                {
                if (z_MaterialKeyWord == value)
                    {
                    return;
                    }

                z_MaterialKeyWord = value;
                NotifyPropertyChanged("MaterialKeyWord");
                }
            }

        private string z_MaterialKeyWord = string.Empty;

        #endregion
        }
    }
