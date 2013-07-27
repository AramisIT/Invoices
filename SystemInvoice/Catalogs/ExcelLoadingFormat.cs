﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Core;
using Aramis.Attributes;
using Aramis.Enums;
using System.Data;
using SystemInvoice.Documents;
using SystemInvoice.PropsSyncronization;

namespace SystemInvoice.Catalogs
    {
    /// <summary>
    /// Справочник. Хранит в себе схему привязки документа/справочника к Excel - файлу
    /// </summary>
    [Catalog( Description = "Формат загрузки файлов Excel", GUID = "AAB86D95-95DF-4C81-92F2-D45C02769441", DescriptionSize = 100, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false )]
    public class ExcelLoadingFormat : CatalogTable, ITradeMarkContractorSubGroupOfGoodsSource
        {
        private TrademarkContractorSubGroupOfGoodsSyncronizer syncronizer = null;

        public ExcelLoadingFormat()
            {
            syncronizer = new TrademarkContractorSubGroupOfGoodsSyncronizer( this );
            this.PropertyChanged += ExcelLoadingFormat_PropertyChanged;
            }

        void ExcelLoadingFormat_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
            {
            if (e.PropertyName.Equals( "ColumnIndexForGrafShoes" ))
                {
                string valueToCheck = this.ColumnIndexForGrafShoes;
                if (string.IsNullOrEmpty( valueToCheck ))
                    {
                    return;
                    }
                int i = 0;
                if (!int.TryParse( valueToCheck, out i ))
                    {
                    this.ColumnIndexForGrafShoes = string.Empty;
                    }
                }
            }

        

        #region Свойства

        #region (Contractor) Contractor Контрагент
        [DataField( Description = "Контрагент", NotEmpty = true, ShowInList = true )]
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

        #region (SubGroupOfGoods) SubGroupOfGoods Группа товара
        [DataField( Description = "Подгруппа товара" )]
        public SubGroupOfGoods SubGroupOfGoods
            {
            get
                {
                return (SubGroupOfGoods)GetValueForObjectProperty( "SubGroupOfGoods" );
                }
            set
                {
                SetValueForObjectProperty( "SubGroupOfGoods", value );
                }
            }
        #endregion


        #region (string) GroupOfGoods Группа товара
        [DataField( Description = "Группа товара", StorageType = StorageTypes.Local, ReadOnly = true )]
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
                NotifyPropertyChanged( "GroupOfGoods" );
                }
            }
        private string z_GroupOfGoods;
        #endregion

        #region (int) FirstRowNumber № первой строки
        [DataField( Description = "№ первой строки" )]
        public int FirstRowNumber
            {
            get
                {
                return z_FirstRowNumber;
                }
            set
                {
                if (z_FirstRowNumber == value)
                    {
                    return;
                    }

                z_FirstRowNumber = value;
                NotifyPropertyChanged( "FirstRowNumber" );
                }
            }
        private int z_FirstRowNumber = 1;
        #endregion

        #region (string) ColumnIndexForGrafShoes № Колонки для переключения графы 31
        [DataField( Description = "№ Колонки для переключения графы 31", Size = 30 )]
        public string ColumnIndexForGrafShoes
            {
            get
                {
                return z_ColumnIndexForGrafShoes;
                }
            set
                {
                if (z_ColumnIndexForGrafShoes == value)
                    {
                    return;
                    }

                z_ColumnIndexForGrafShoes = value;
                NotifyPropertyChanged( "ColumnIndexForGrafShoes" );
                }
            }
        private string z_ColumnIndexForGrafShoes = "";
        #endregion

        #region (string) GrafSwitchValue Значение в колонке ColumnIndexForGrafShoes для переключения графы 31
        [DataField( Description = "Значение в колонке ColumnIndexForGrafShoes для переключения графы 31 ", Size = 30 )]
        public string GrafSwitchValue
            {
            get
                {
                return z_GrafSwitchValue;
                }
            set
                {
                if (z_GrafSwitchValue == value)
                    {
                    return;
                    }

                z_GrafSwitchValue = value;
                NotifyPropertyChanged( "GrafSwitchValue" );
                }
            }
        private string z_GrafSwitchValue = "";
        #endregion

        #endregion

        #region Табличная часть ColumnsMappings (Колонки)
        [Table( Columns = "ColumnName,ColumnNumberInExcel,Constant,GraphContent,ColumnNumberInGraph,GraphContentShoes,ColumnNumberInGraphShoes,UnloadColumnNumber,UnloadNewItemsColumnNumber" )]
        [DataField( Description = "Колонки" )]
        public DataTable ColumnsMappings
            {
            get
                {
                return GetSubtable( "ColumnsMappings" );
                }
            }

        //[SubTableField( Description = "EnumField", PropertyType = typeof( ColumnName ), NotEmpty = true, Size = 50 )]
        //public DataColumn EnumField
        //    {
        //    get;
        //    set;
        //    }

        [SubTableField( Description = "Название колонки", PropertyType = typeof( InvoiceColumnNames ), NotEmpty = true, Size = 50 )]
        public DataColumn ColumnName
            {
            get;
            set;
            }

        [SubTableField( Description = "№ Загружаемой колонки из ЕХЕL", PropertyType = typeof( string ), NotEmpty = false, Size = 250 )]
        public DataColumn ColumnNumberInExcel
            {
            get;
            set;
            }

        [SubTableField( Description = "Константа", PropertyType = typeof( string ), Size = 50 )]
        public DataColumn Constant
            {
            get;
            set;
            }

        [SubTableField( Description = "Состав Гр. 31(А)", PropertyType = typeof( string ), Size = 150 )]
        public DataColumn GraphContent
            {
            get;
            set;
            }
        //используется строковый ти поскольку для некоторых строк у нас должно быть пустое значение
        [SubTableField( Description = "№ Колонки в составе гр. 31", PropertyType = typeof( string ), Size = 50 )]
        public DataColumn ColumnNumberInGraph
            {
            get;
            set;
            }

        [SubTableField( Description = "Состав Гр. 31(А) (обувь)", PropertyType = typeof( string ), Size = 150 )]
        public DataColumn GraphContentShoes
            {
            get;
            set;
            }
        //используется строковый ти поскольку для некоторых строк у нас должно быть пустое значение
        [SubTableField( Description = "№ Колонки в составе гр. 31 (обувь)", PropertyType = typeof( string ), Size = 50 )]
        public DataColumn ColumnNumberInGraphShoes
            {
            get;
            set;
            }

        [SubTableField( Description = "№ Колонки при выгрузке обработанного файла в Excel", PropertyType = typeof( string ), Size = 50 )]
        public DataColumn UnloadColumnNumber
            {
            get;
            set;
            }


        [SubTableField( Description = "№ Колонки при выгрузке новых/необрабатываемых ел-тов в Excel", PropertyType = typeof( string ), Size = 50 )]
        public DataColumn UnloadNewItemsColumnNumber
            {
            get;
            set;
            }

        #endregion

        public override GetListFilterDelegate GetFuncGetCustomFilter( string propertyName )
            {
            return syncronizer.GetFuncGetCustomFilter( propertyName );
            }

        public override WritingResult Write()
            {
            if (checkTablePart())
                {
                return base.Write();
                }
            else
                {
                return WritingResult.Canceled;
                }
            }

        private bool checkTablePart()
            {
            HashSet<int> columnOutputNumbers = new HashSet<int>();
            HashSet<InvoiceColumnNames> existingInvoice = new HashSet<InvoiceColumnNames>();
            SortedDictionary<int, string> names = Aramis.Core.FastInput.GetCashedData( typeof( InvoiceColumnNames ) );
            int currentColumnIndex = -1;
            foreach (DataRow row in ColumnsMappings.Rows)
                {
                InvoiceColumnNames name = (InvoiceColumnNames)row[ColumnName];
                if (existingInvoice.Contains( name ))
                    {
                    string.Format( @"Колонка ""{0}"" описана более одного раза.", names[(int)name] ).AlertBox();
                    return false;
                    }
                string outputColumnIndex = row.TryGetColumnValue<string>( "UnloadColumnNumber", "" );
                if (!string.IsNullOrEmpty( outputColumnIndex ))
                    {
                    if (int.TryParse( outputColumnIndex, out currentColumnIndex ))
                        {
                        if (!columnOutputNumbers.Contains( currentColumnIndex ))
                            {
                            columnOutputNumbers.Add( currentColumnIndex );
                            }
                        else
                            {
                            string.Format( @"Номер для исходящего файла колонки ""{0}"" уже занят.", names[(int)name] ).AlertBox();
                            return false;
                            }
                        }
                    }
                existingInvoice.Add( name );
                }
            return true;
            }
        }
    }