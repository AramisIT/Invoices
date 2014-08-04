using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Core;
using Aramis.Attributes;
using Aramis.Enums;
using System.Data;
using SystemInvoice.PropsSyncronization;

namespace SystemInvoice.Catalogs
    {
    [Catalog( Description = "Формат загрузки разрешительного документа", GUID = "A8592D01-0B9C-4FED-9BE7-1C54F920003F", DescriptionSize = 50, HierarchicType = HierarchicTypes.None, ShowCodeFieldInForm = false )]
    public class ApprovalsLoadFormat : CatalogTable, ITradeMarkContractorSource
        {

        private TradeMarkContractorSyncronizer syncronizer = null;

        public static Dictionary<string, string> ApprovalsLoadFormatColumnNames = new Dictionary<string, string>();
        public static Dictionary<string, string> ApprovalsLoadFormatTranslated = new Dictionary<string, string>();

        static ApprovalsLoadFormat()
            {
            fillApprovalsLoadFormatColumNames();
            }

        private static void fillApprovalsLoadFormatColumNames()
            {
            ApprovalsLoadFormatColumnNames.Add( "FirstLoadRowIndex", "Номер первой строки" );
            ApprovalsLoadFormatColumnNames.Add( "Article", "Артикул" );
            ApprovalsLoadFormatColumnNames.Add( "OriginalName", "Наименование (исходное)" );
            ApprovalsLoadFormatColumnNames.Add( "CustomsCodeExtern", "Таможенный код внутренний" );
            ApprovalsLoadFormatColumnNames.Add( "CustomsCodeIntern", "Таможенный код внешний" );
            ApprovalsLoadFormatColumnNames.Add( "DeclarationName", "Наименование декларации" );
            ApprovalsLoadFormatColumnNames.Add( "Manufacturer", "Производитель" );
            foreach (KeyValuePair<string, string> pair in ApprovalsLoadFormatColumnNames)
                {
                ApprovalsLoadFormatTranslated.Add( pair.Value, pair.Key );
                }
            }

        public ApprovalsLoadFormat()
            {
            syncronizer = new TradeMarkContractorSyncronizer( this );
            }

        #region (Contractor) Contractor Контрагент
        [DataField( Description = "Контрагент", NotEmpty = true )]
        public IContractor Contractor
            {
            get
                {
                return (IContractor)GetValueForObjectProperty( "Contractor" );
                }
            set
                {
                SetValueForObjectProperty( "Contractor", value );
                }
            }
        #endregion

        #region (TradeMark) TradeMark Торговая Марка
        [DataField( Description = "Торговая Марка", NotEmpty = true )]
        public ITradeMark TradeMark
            {
            get
                {
                return (ITradeMark)GetValueForObjectProperty( "TradeMark" );
                }
            set
                {
                SetValueForObjectProperty( "TradeMark", value );
                }
            }
        #endregion

        #region Табличная часть ColumnsMappings (Колонки)
        [Table( Columns = "ColumnName,ColumnNumberInExcel,Constant" )]
        [DataField( Description = "Колонки" )]
        public DataTable ColumnsMappings
            {
            get
                {
                return GetSubtable( "ColumnsMappings" );
                }
            }

        [SubTableField( Description = "Название колонки", PropertyType = typeof( ApprovalsLoadColumnNames ), NotEmpty = true, Size = 50 )]
        public DataColumn ColumnName
            {
            get;
            set;
            }

        [SubTableField( Description = "№ Загружаемой колонки из ЕХЕL", PropertyType = typeof( string ), NotEmpty = false, Size = 50 )]
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

        #endregion

        protected override void InitNewBeforeShowing()
            {
            base.InitNewBeforeShowing();
            for (int lineNumber = 1; lineNumber <= ApprovalsLoadFormatColumnNames.Keys.Count; lineNumber++)
                {
                DataRow columnsMappingRow = ColumnsMappings.NewRow();
                columnsMappingRow["LineNumber"] = lineNumber;
                columnsMappingRow[ColumnName] = lineNumber;
                columnsMappingRow[ColumnNumberInExcel] = "";
                columnsMappingRow[Constant] = "";
                ColumnsMappings.Rows.Add( columnsMappingRow );
                }
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
            HashSet<ApprovalsLoadColumnNames> existingInvoice = new HashSet<ApprovalsLoadColumnNames>();
            SortedDictionary<int, string> names = Aramis.Core.FastInput.GetCashedData( typeof( ApprovalsLoadColumnNames ) );
            foreach (DataRow row in ColumnsMappings.Rows)
                {
                ApprovalsLoadColumnNames name = (ApprovalsLoadColumnNames)row[ColumnName];
                if (existingInvoice.Contains( name ))
                    {
                    string.Format( @"Колонка ""{0}"" описана более одного раза.", names[(int)name] ).AlertBox();
                    return false;
                    }
                existingInvoice.Add( name );
                }
            string messageFail = "";
            foreach (int key in names.Keys)
                {
                if (key == 0)
                    {
                    continue;
                    }
                if (!existingInvoice.Contains( (ApprovalsLoadColumnNames)key ))
                    {
                    if (string.IsNullOrEmpty( messageFail ))
                        {
                        messageFail = string.Concat( "Отсутствуют следующие колонки: ", Environment.NewLine );
                        }
                    messageFail += string.Concat( names[key], Environment.NewLine );
                    }
                }
            if (!string.IsNullOrEmpty( messageFail ))
                {
                messageFail.AlertBox();
                return false;
                }
            return true;
            }

        public enum ApprovalsLoadColumnNames
            {
            [DataField( Description = "Не заполнено" )]
            Empty,
            [DataField( Description = "Номер первой строки" )]
            FirstLoadRowIndex,
            [DataField( Description = "Артикул" )]
            Article,
            [DataField( Description = "Наименование (исходное)" )]
            OriginalName,
            [DataField( Description = "Таможенный код внутренний" )]
            CustomsCodeIntern,
            [DataField( Description = "Таможенный код внешний" )]
            CustomsCodeExtern,
            [DataField( Description = "Наименование декларации" )]
            DeclarationName,
            [DataField( Description = "Производитель" )]
            Manufacturer
            }

        public override GetListFilterDelegate GetFuncGetCustomFilter( string propertyName )
            {
            return syncronizer.GetFuncGetCustomFilter( propertyName );
            }

        //#region Табличная часть формата загрузки
        //[Table( Columns = "FirstLoadRowIndex,ItemArticle,OriginalName,CustomsCodeExtern,CustomsCodeIntern,DeclarationName,Manufacturer" )]
        //[DataField( Description = "Номенклатура" )]
        //public DataTable Nomenclatures
        //    {
        //    get
        //        {
        //        return GetSubtable( "Nomenclatures" );
        //        }
        //    }

        //[SubTableField( Description = "Номер первой строки", PropertyType = typeof( int ), NotEmpty = true )]
        //public DataColumn FirstLoadRowIndex
        //    {
        //    get;
        //    set;
        //    }


        //[SubTableField( Description = "Артикул", PropertyType = typeof( string ), NotEmpty = true )]
        //public DataColumn ItemArticle
        //    {
        //    get;
        //    set;
        //    }

        //[SubTableField( Description = "Наименование исходное", PropertyType = typeof( string ), NotEmpty = true )]
        //public DataColumn OriginalName
        //    {
        //    get;
        //    set;
        //    }


        //[SubTableField( Description = "Таможенный код внутренний", PropertyType = typeof( string ), NotEmpty = true )]
        //public DataColumn CustomsCodeExtern
        //    {
        //    get;
        //    set;
        //    }


        //[SubTableField( Description = "Таможенный код внешний", PropertyType = typeof( string ), NotEmpty = true )]
        //public DataColumn CustomsCodeIntern
        //    {
        //    get;
        //    set;
        //    }


        //[SubTableField( Description = "Наименование декларации", PropertyType = typeof( string ), NotEmpty = true )]
        //public DataColumn DeclarationName
        //    {
        //    get;
        //    set;
        //    }

        //[SubTableField( Description = "Производитель", PropertyType = typeof( string ), NotEmpty = true )]
        //public DataColumn Manufacturer
        //    {
        //    get;
        //    set;
        //    }
        //#endregion
        }
    }
