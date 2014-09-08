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
    [Document(Description = "История удаления номенклатуры из РД", GUID = "097D3F4C-6B61-4E50-AEBB-25EEB65FBA0A", NumberType = NumberType.Int64, ShowLastModifiedDateInList = true)]
    public class NomenclatureApprovalsRemovingHistory : DocumentTable
        {
        #region (Nomenclature) Nomenclature Номенклатурная позиция
        [DataField(Description = "Номенклатурная позиция")]
        public Nomenclature Nomenclature
            {
            get
                {
                return (Nomenclature)GetValueForObjectProperty("Nomenclature");
                }
            set
                {
                SetValueForObjectProperty("Nomenclature", value);
                }
            }
        #endregion

        #region (NomenclatureRemovigTypeKind) NomenclatureRemovigTypeKind Метод удаления
        [DataField(Description = "Метод удаления")]
        public NomenclatureRemovigTypeKind NomenclatureRemovigTypeKind
            {
            get
                {
                return z_NomenclatureRemovigTypeKind;
                }
            set
                {
                if (z_NomenclatureRemovigTypeKind == value)
                    {
                    return;
                    }

                z_NomenclatureRemovigTypeKind = value;
                NotifyPropertyChanged("NomenclatureRemovigTypeKind");
                }
            }
        private NomenclatureRemovigTypeKind z_NomenclatureRemovigTypeKind = NomenclatureRemovigTypeKind.Auto;
        #endregion

        #region (DocumentType) DocumentType Тип документа
        [DataField(Description = "Тип документа")]
        public IDocumentType DocumentType
            {
            get
                {
                return (IDocumentType)GetValueForObjectProperty("DocumentType");
                }
            set
                {
                SetValueForObjectProperty("DocumentType", value);
                }
            }
        #endregion


        #region (DateTime) DateFrom Дата с
        [DataField(Description = "Дата с")]
        public DateTime DateFrom
            {
            get
                {
                return z_DateFrom;
                }
            set
                {
                if (z_DateFrom == value)
                    {
                    return;
                    }

                z_DateFrom = value;
                NotifyPropertyChanged("DateFrom");
                }
            }
        private DateTime z_DateFrom = DateTime.MinValue;
        #endregion

        #region (DateTime) DateTo Дата по
        [DataField(Description = "Дата по")]
        public DateTime DateTo
            {
            get
                {
                return z_DateTo;
                }
            set
                {
                if (z_DateTo == value)
                    {
                    return;
                    }

                z_DateTo = value;
                NotifyPropertyChanged("DateTo");
                }
            }
        private DateTime z_DateTo = DateTime.MinValue;
        #endregion


        #region (DateTime) RemovingDate Дата удаления
        [DataField(Description = "Дата удаления")]
        public DateTime RemovingDate
            {
            get
                {
                return z_RemovingDate;
                }
            set
                {
                if (z_RemovingDate == value)
                    {
                    return;
                    }

                z_RemovingDate = value;
                NotifyPropertyChanged("RemovingDate");
                }
            }
        private DateTime z_RemovingDate = DateTime.MinValue;
        #endregion
        }

    public enum NomenclatureRemovigTypeKind
        {
        [DataField(Description = "В ручную")]
        Manual,
        [DataField(Description = "Автоматически")]
        Auto
        }
    }
