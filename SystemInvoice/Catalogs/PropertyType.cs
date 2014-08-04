using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Core;
using Aramis.Attributes;
using Aramis.Enums;
using Aramis.UI.DBObjectsListFilter;
using SystemInvoice.PropsSyncronization;

namespace SystemInvoice.Catalogs
    {
    /// <summary>
    /// Справочник. Содержит в себе описание свойства. Используется для перевода свойста в украинскую систему мер.//Description  - Свойство
    /// </summary>
    [Catalog(Description = "Виды свойств", GUID = "A23F1B69-5AA0-43C2-99E3-58AEEA45B89A", DescriptionSize = 50, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false)]
    public class PropertyType : CatalogTable, ITradeMarkContractorSubGroupOfGoodsSource
        {
        /*        with propGroupsBindings as
        (
            select propType.Id,groupOfGoods.Id as groupOfGoods from PropertyType as propType
            join GroupOfGoods as groupOfGoods on LTRIM(groupOfGoods.Description) = LTRIM(propType.GroupOfGoods)
        )
        update PropertyType set GroupOfGoodsRef = coalesce((select top 1 groupOfGoods from propGroupsBindings where Id = PropertyType.Id),0);*/
        private TrademarkContractorSubGroupOfGoodsSyncronizer syncronizer = null;
        #region Свойства

        #region (GoodsProperty) GoodsProperty Свойство
        [DataField(Description = "Свойство", ShowInList = true)]
        public PropertyOfGoods PropertyOfGoods
            {
            get
                {
                return (PropertyOfGoods)GetValueForObjectProperty("PropertyOfGoods");
                }
            set
                {
                SetValueForObjectProperty("PropertyOfGoods", value);
                }
            }
        #endregion

        #region (string) CodeOfProperty Код свойства
        [DataField(Description = "Код свойства", Size = 100, ShowInList = true)]
        public string CodeOfProperty
            {
            get
                {
                return z_CodeOfProperty;
                }
            set
                {
                if (z_CodeOfProperty == value)
                    {
                    return;
                    }

                z_CodeOfProperty = value;
                NotifyPropertyChanged("CodeOfProperty");
                }
            }
        private string z_CodeOfProperty = "";
        #endregion

        #region (string) Value Значение
        [DataField(Description = "Значение", Size = 100, ShowInList = true)]
        public string Value
            {
            get
                {
                return z_Value;
                }
            set
                {
                if (z_Value == value)
                    {
                    return;
                    }

                z_Value = value;
                NotifyPropertyChanged("Value");
                }
            }
        private string z_Value = "";
        #endregion

        #region (string) UkrainianValue Значение в украинской системе
        [DataField(Description = "Значение в украинской системе", Size = 100, ShowInList = true)]
        public string UkrainianValue
            {
            get
                {
                return z_UkrainianValue;
                }
            set
                {
                if (z_UkrainianValue == value)
                    {
                    return;
                    }

                z_UkrainianValue = value;
                NotifyPropertyChanged("UkrainianValue");
                }
            }
        private string z_UkrainianValue = "";
        #endregion

        #region (string) InsoleLength Длинна стельки
        [DataField(Description = "Длинна стельки", Size = 100, ShowInList = true)]
        public string InsoleLength
            {
            get
                {
                return z_InsoleLength;
                }
            set
                {
                if (z_InsoleLength == value)
                    {
                    return;
                    }

                z_InsoleLength = value;
                NotifyPropertyChanged("InsoleLength");
                }
            }
        private string z_InsoleLength = "";
        #endregion

        #region (Nomenclature) Nomenclature Номенклатура
        [DataField(Description = "Номенклатура")]
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

        #region (SubGroupOfGoods) SubGroupOfGoods Группа товара
        [DataField(Description = "Подгруппа товара", ShowInList = true)]
        public SubGroupOfGoods SubGroupOfGoods
            {
            get
                {
                return (SubGroupOfGoods)GetValueForObjectProperty("SubGroupOfGoods");
                }
            set
                {
                SetValueForObjectProperty("SubGroupOfGoods", value);
                }
            }
        #endregion


        #region (GroupOfGoods) GroupOfGoodsRef Группа товара
        [DataField(Description = "Группа товара", ShowInList = true)]
        public GroupOfGoods GroupOfGoodsRef
            {
            get
                {
                return (GroupOfGoods)GetValueForObjectProperty("GroupOfGoodsRef");
                }
            set
                {
                SetValueForObjectProperty("GroupOfGoodsRef", value);
                }
            }
        #endregion

        #region (string) GroupOfGoods Группа товара
        [DataField(Description = "Группа товара", Size = 100, ReadOnly = true, ShowInList = false,ShowInForm =  false)]
        public string GroupOfGoods
            {
            get
                {
                return z_GroupOfGoods;
                }
            set
                {
                if (z_GroupOfGoods == value)
                    {
                    return;
                    }
                z_GroupOfGoods = value;
                NotifyPropertyChanged("GroupOfGoods");
                }
            }
        private string z_GroupOfGoods = string.Empty;
        #endregion

        #region (TradeMark) TradeMark Торговая марка
        [DataField(Description = "Торговая марка")]
        public ITradeMark TradeMark
            {
            get
                {
                return (ITradeMark)GetValueForObjectProperty("TradeMark");
                }
            set
                {
                SetValueForObjectProperty("TradeMark", value);
                }
            }
        #endregion

        #region (Contractor) Contractor Контрагент
        [DataField(Description = "Контрагент", ShowInList = true)]
        public IContractor Contractor
            {
            get
                {
                return (IContractor)GetValueForObjectProperty("Contractor");
                }
            set
                {
                SetValueForObjectProperty("Contractor", value);
                }
            }
        #endregion

        #region (string) Article Артикул
        [DataField(Description = "Артикул", StorageType = StorageTypes.Local, ReadOnly = true)]
        public string Article
            {
            get
                {
                return z_Article;
                }
            set
                {
                if (z_Article == value)
                    {
                    return;
                    }
                z_Article = value;
                NotifyPropertyChanged("Article");
                }
            }
        private string z_Article = "";
        #endregion

        #region (string) GroupCode Код группы товара
        [DataField(Description = "Код подгруппы товара", Size = 100, ReadOnly = true, ShowInList = true)]
        public string GroupCode
            {
            get
                {
                return z_GroupCode;
                }
            set
                {
                if (z_GroupCode == value)
                    {
                    return;
                    }
                z_GroupCode = value;
                NotifyPropertyChanged("GroupCode");
                }
            }
        private string z_GroupCode = "";
        #endregion

        #endregion

        public PropertyType()
            {
            this.OnRead += PropertyType_OnRead;
            this.ValueOfObjectPropertyChanged += PropertyType_ValueOfObjectPropertyChanged;
            this.syncronizer = new TrademarkContractorSubGroupOfGoodsSyncronizer(this);
            }

        void PropertyType_OnRead()
            {
            setLocalFields();
            }

        private bool bySubGroupOfGoodsChanged = false;
        private bool byGroupOfGoodsChanged = false;

        void PropertyType_ValueOfObjectPropertyChanged(string propertyName)
            {
            if (propertyName.Equals("Nomenclature") || propertyName.Equals("SubGroupOfGoods") ||
                propertyName.Equals("PropertyOfGoods"))
                {
                setLocalFields();
                }
            if (propertyName.Equals("GroupOfGoodsRef") && !bySubGroupOfGoodsChanged) //Очищаем подгруппу товара
                {
                byGroupOfGoodsChanged = true;
                this.SubGroupOfGoods = new SubGroupOfGoods();
                bySubGroupOfGoodsChanged = false;
                }
            }

        private void setLocalFields()
            {
            bySubGroupOfGoodsChanged = true;
            GroupCode = SubGroupOfGoods.GroupCode;
            Article = Nomenclature.Article;
            Description = PropertyOfGoods.Description;
            if (!byGroupOfGoodsChanged && SubGroupOfGoods.Id!=0)
                {
                GroupOfGoodsRef = SubGroupOfGoods.GroupOfGoods;
                }
            bySubGroupOfGoodsChanged = false;
            }

        public override GetListFilterDelegate GetFuncGetCustomFilter(string propertyName)
            {
            return syncronizer.GetFuncGetCustomFilter(propertyName);
            }
        }
    }
