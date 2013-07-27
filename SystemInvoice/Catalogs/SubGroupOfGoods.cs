using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Core;
using Aramis.Attributes;
using Aramis.Enums;
using SystemInvoice.PropsSyncronization;

namespace SystemInvoice.Catalogs
    {
    /// <summary>
    /// Справочник. Содержит в себе описание группы товара (Торговая марка, производитель и т.д.)//Description - Название
    /// </summary>
    [Catalog( Description = "Подгруппы", GUID = "A9E91AB5-03BE-40B5-909C-A97442D40047", DescriptionSize = 40, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false )]
    public class SubGroupOfGoods : CatalogTable, ITradeMarkContractorManufacturerSource
        {
        private TradeMarkContractorManufacturerSyncronizer syncronizer = null;

        public SubGroupOfGoods()
            {
            syncronizer = new TradeMarkContractorManufacturerSyncronizer( this );
            }
        #region Свойства

        #region (Contractor) Contractor Производитель
        [DataField( Description = "Контрагент",ShowInList = true )]
        public Contractor Contractor
            {
            get
                {
                return (Contractor)GetValueForObjectProperty( "Contractor" );
                }
            set
                {
                SetValueForObjectProperty( "Contractor", value );
                }
            }
        #endregion


        #region (GroupOfGoods) GroupOfGoods Группа товара
        [DataField( Description = "Группа товара", ShowInList = true )]
        public GroupOfGoods GroupOfGoods
            {
            get
                {
                return (GroupOfGoods)GetValueForObjectProperty( "GroupOfGoods" );
                }
            set
                {
                SetValueForObjectProperty( "GroupOfGoods", value );
                }
            }
        #endregion

        #region (Contractor) Contractor Производитель
        [DataField( Description = "Производитель" )]
        public Manufacturer Manufacturer
            {
            get
                {
                return (Manufacturer)GetValueForObjectProperty( "Manufacturer" );
                }
            set
                {
                SetValueForObjectProperty( "Manufacturer", value );
                }
            }
        #endregion

        #region (TradeMark) TradeMark Торговая марка
        [DataField( Description = "Торговая марка", ShowInList = true )]
        public TradeMark TradeMark
            {
            get
                {
                return (TradeMark)GetValueForObjectProperty( "TradeMark" );
                }
            set
                {
                SetValueForObjectProperty( "TradeMark", value );
                }
            }
        #endregion

        #region (string) GroupCode Код Группы
        [DataField( Description = "Код Группы", NotEmpty = false, Size = 20, ShowInList = true )]
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
                NotifyPropertyChanged( "GroupCode" );
                }
            }
        private string z_GroupCode = "";
        #endregion

        //#region (string) GroupNameUKR Название группы укр.
        //[DataField( Description = "Название группы укр.", Size = 40 )]
        //public string GroupNameUKR
        //    {
        //    get
        //        {
        //        return z_GroupNameUKR;
        //        }
        //    set
        //        {
        //        if (z_GroupNameUKR == value)
        //            {
        //            return;
        //            }

        //        z_GroupNameUKR = value;
        //        NotifyPropertyChanged( "GroupNameUKR" );
        //        }
        //    }
        //private string z_GroupNameUKR = "";
        //#endregion

        #region (string) GroupNameEng Название группы англ.
        [DataField( Description = "Название группы англ.", Size = 40 )]
        public string GroupNameEng
            {
            get
                {
                return z_GroupNameEng;
                }
            set
                {
                if (z_GroupNameEng == value)
                    {
                    return;
                    }

                z_GroupNameEng = value;
                NotifyPropertyChanged( "GroupNameEng" );
                }
            }
        private string z_GroupNameEng = "";
        #endregion

        #region (string) ShortGroupName Сокращение
        [DataField( Description = "Сокращение", Size = 40 )]
        public string ShortGroupName
            {
            get
                {
                return z_ShortGroupName;
                }
            set
                {
                if (z_ShortGroupName == value)
                    {
                    return;
                    }

                z_ShortGroupName = value;
                NotifyPropertyChanged( "ShortGroupName" );
                }
            }
        private string z_ShortGroupName = "";
        #endregion

        #endregion

        public override GetListFilterDelegate GetFuncGetCustomFilter( string propertyName )
            {
            return syncronizer.GetFuncGetCustomFilter( propertyName );
            }
        }
    }
