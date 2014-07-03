using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Aramis.Attributes;
using Aramis.Core;
using Aramis.Enums;
using Aramis.UI.WinFormsDevXpress;
using SystemInvoice.Excel;
using SystemInvoice.Catalogs;
using Aramis.DatabaseConnector;
using SystemInvoice.Documents;

namespace SystemInvoice.Catalogs.Forms
    {
    [Aramis.Attributes.View(DBObjectGuid = "AAB86D95-95DF-4C81-92F2-D45C02769441", ViewType = ViewFormType.CatalogItem)]
    public partial class ExcelLoadingFormatItemForm : DevExpress.XtraBars.Ribbon.RibbonForm, IItemForm
        {
        TableLoader excelLoader = new TableLoader();

        public ExcelLoadingFormatItemForm()
            {
            InitializeComponent();
            }


        public IDatabaseObject Item
            {
            get;
            set;
            }

        public ExcelLoadingFormat ExcelLoadingFormat
            {
            get { return (ExcelLoadingFormat)Item; }
            }

        private void btnCancel_ItemClick( object sender, ItemClickEventArgs e )
            {
            Close();
            }

        private void btnWrite_ItemClick( object sender, ItemClickEventArgs e )
            {
            Item.Write();
            }

        private void btnOk_ItemClick( object sender, ItemClickEventArgs e )
            {
            WritingResult result = Item.Write();
            if (result == WritingResult.Success)
                {
                Close();
                }
            }

        private void btnLoad_Click( object sender, EventArgs e )
            {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel Files (.xls)|*.xls";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                this.LoadTablePart( ofd.FileName );
                }
            }

        public class DelegateFormatter : SystemInvoice.Excel.DataFormatting.ICustomExpressionHandler
            {
            public delegate object CustomExpressionDelegate( params object[] parameters );
            CustomExpressionDelegate thisDeleg = null;

            public DelegateFormatter( CustomExpressionDelegate deleg )
                {
                thisDeleg = deleg;
                }

            public object ProcessRow( params object[] parameters )
                {
                return thisDeleg( parameters );
                }
            }

        private void LoadTablePart( string fileName )
            {
            try
                {
                ExcelLoadingFormat.ColumnsMappings.Rows.Clear();
                ExcelMapper mapper = createMapper();
                excelLoader.RegisterFormatter( "enumFormatter", new SystemInvoice.Excel.DataFormatting.Formatters.CustomDelegateExpressionFormatterConstructor( new DelegateFormatter( ( obj ) =>
                    {
                        if (obj == null)
                            {
                            return 0;
                            }
                        string value = (string)obj[0];
                        try
                            {
                            if (Invoice.InvoiceColumnNamesTranslated.ContainsKey( value ))
                                {
                                value = Invoice.InvoiceColumnNamesTranslated[value];
                                }
                            InvoiceColumnNames name = (InvoiceColumnNames)Enum.Parse( typeof( InvoiceColumnNames ), value );
                            return (int)name;
                            }
                        catch
                            {
                            return 0;
                            }
                    } ) ) );
                excelLoader.RegisterFormatter( "translateFormatter", new SystemInvoice.Excel.DataFormatting.Formatters.CustomDelegateExpressionFormatterConstructor( new DelegateFormatter( ( obj ) =>
                    {
                        if (string.IsNullOrEmpty( obj[0].ToString() ))
                            {
                            return obj[0];
                            }
                        string valueStr = obj[0].ToString();
                        foreach (KeyValuePair<string, string> keyValuePair in SystemInvoice.Documents.Invoice.InvoiceColumnNames)
                            {
                            valueStr = valueStr.Replace( keyValuePair.Key, keyValuePair.Value );
                            }
                        return valueStr;
                    } ) ) );
                excelLoader.TryFill( ExcelLoadingFormat.ColumnsMappings, mapper, fileName, 1 );
                int lineNumber = 0;
                foreach (DataRow row in ExcelLoadingFormat.ColumnsMappings.Rows)
                    {
                    row["LineNumber"] = ++lineNumber;
                    }
                ExcelLoadingFormat.NotifyTableRowChanged( ExcelLoadingFormat.ColumnsMappings, ExcelLoadingFormat.ColumnName, null );
                }
            catch(Exception e)
                {
                string message = e.ToString();
                "Ошибка при попытке загрузить файл. Закройте файл если он открыт в другом приложении.".AlertBox();
                }
            }

        private ExcelMapper createMapper()
            {
            ExcelMapper mapper = new ExcelMapper();
            mapper.TryAddExpression( "ColumnName", "enumFormatter", "1", "" );
            mapper.TryAddExpression( "ColumnNumberInExcel", "translateFormatter", "2", "" );
            mapper.TryAddExpression( "Constant", ExcelMapper.IndexKey, "3", "" );
            mapper.TryAddExpression( "GraphContent", ExcelMapper.IndexKey, "4", "" );
            mapper.TryAddExpression( "ColumnNumberInGraph", ExcelMapper.IndexKey, "5", "" );
            mapper.TryAddExpression( "UnloadColumnNumber", ExcelMapper.IndexKey, "6", "" );
            mapper.TryAddExpression( "UnloadNewItemsColumnNumber", ExcelMapper.IndexKey, "7", "" );
            mapper.TryAddExpression( "GraphContentShoes", ExcelMapper.IndexKey, "8", "" ); ;
            mapper.TryAddExpression( "ColumnNumberInGraphShoes", ExcelMapper.IndexKey, "9", "" );
            return mapper;
            }

        private void btnUnload_Click( object sender, EventArgs e )
            {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Files (.xls)|*.xls";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                DataTable tableToSave = createSaveTable();
                TableUnloader unloader = new TableUnloader();
                ExcelMapper mapper = createMapper();
                checkMapper( mapper );
                unloader.OnInitializeRowData += unloader_OnInitializeRowData;
                unloader.Save( tableToSave, sfd.FileName, mapper, 1 );
                }
            }

        InitializedCellData unloader_OnInitializeRowData( string propertyName, int rowIndex )
            {
            string columnName = getUkrColumnName( propertyName );
            return new InitializedCellData( columnName, 1, 1, true );
            }

        private void checkMapper( ExcelMapper mapper )
            {
            List<string> expressions = new List<string>();
            foreach (string expr in mapper.Keys)
                {
                expressions.Add( expr );
                }
            foreach (var key in expressions)
                {
                Expression expression = mapper[key];
                expression.ExpressionType = ExcelMapper.IndexKey;
                mapper[key] = expression;
                }
            }

        private static string getUkrColumnName( string propertyName )
            {
            switch (propertyName.Trim())
                {
                case ("ColumnName"): return "Имя колонки";
                case ("ColumnNumberInExcel"): return "Номер колонки(выражение)";
                case ("Constant"): return "Константа";
                case ("GraphContent"): return "Состав графы 31";
                case ("ColumnNumberInGraph"): return "Номер в составе графы 31";
                case ("UnloadColumnNumber"): return "Номер колонки для обработанного файла";
                case ("UnloadNewItemsColumnNumber"): return "Номер колонки для новых элементов";
                case ("GraphContentShoes"): return "Состав гр. для обуви";
                case ("ColumnNumberInGraphShoes"): return "Номер колонки в составе для обуви";
                default: return propertyName;
                }
            }

        private DataTable createSaveTable()
            {
            DataTable table = new DataTable();
            table.Columns.Add( "ColumnName" );
            table.Columns.Add( "ColumnNumberInExcel" );
            table.Columns.Add( "Constant" );
            table.Columns.Add( "GraphContent" );
            table.Columns.Add( "ColumnNumberInGraph" );
            table.Columns.Add( "UnloadColumnNumber" );
            table.Columns.Add( "UnloadNewItemsColumnNumber" );
            table.Columns.Add( "GraphContentShoes" );
            table.Columns.Add( "ColumnNumberInGraphShoes" );
            foreach (DataRow row in ExcelLoadingFormat.ColumnsMappings.Rows)
                {
                string nameToUnload = "";
                string invoiceRuName = "";
                string nameEngInvoice = ((InvoiceColumnNames)row[1]).ToString();
                if (Invoice.InvoiceColumnNames.ContainsKey( nameEngInvoice ))
                    {
                    invoiceRuName = Invoice.InvoiceColumnNames[nameEngInvoice];
                    }
                nameToUnload = string.IsNullOrEmpty( invoiceRuName ) ? nameEngInvoice : invoiceRuName;
                DataRow newRow = table.NewRow();
                newRow["ColumnName"] = nameToUnload;
                newRow["ColumnNumberInExcel"] = row["ColumnNumberInExcel"];
                newRow["Constant"] = row["Constant"];
                newRow["GraphContent"] = row["GraphContent"];
                newRow["ColumnNumberInGraph"] = row["ColumnNumberInGraph"];
                newRow["UnloadColumnNumber"] = row["UnloadColumnNumber"];
                newRow["UnloadNewItemsColumnNumber"] = row["UnloadNewItemsColumnNumber"];
                newRow["GraphContentShoes"] = row["GraphContentShoes"];
                newRow["ColumnNumberInGraphShoes"] = row["ColumnNumberInGraphShoes"];
                table.Rows.Add( newRow );

                //table.Rows.Add( nameToUnload, row["ColumnNumberInExcel"], row["Constant"], row["GraphContent"], row["ColumnNumberInGraph"], row["UnloadColumnNumber"]
                //    , row["GraphContentShoes"], row["ColumnNumberInGraphShoes"] );
                }
            return table;
            }
        }
    }