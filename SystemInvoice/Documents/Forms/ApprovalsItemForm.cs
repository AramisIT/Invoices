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
using System.IO;
using System.Diagnostics;
using SystemInvoice.Excel;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.ApprovalsProcessing;
using Aramis.DatabaseConnector;

namespace SystemInvoice.Documents.Forms
    {
    [View( DBObjectGuid = "0980DB9C-524B-43B6-91F4-424982EF9B99", ViewType = ViewFormType.DocItem )]
    public partial class ApprovalsItemForm : DevExpress.XtraBars.Ribbon.RibbonForm, IItemForm
        {
        private SystemInvoiceDBCache cachedData = null;
        private ApprovalsFilesLoader filesLoader = null;
        private const int startRowIndex = 1;

        public ApprovalsItemForm()
            {
            InitializeComponent();
            }

        private DatabaseObject item = null;
        public DatabaseObject Item
            {
            get { return item; }
            set
                {
                item = value;
                cachedData = new SystemInvoiceDBCache( this.Approvals );
                this.filesLoader = new ApprovalsFilesLoader( this.Approvals, this.cachedData );
                }
            }

        private Approvals Approvals
            {
            get { return (Approvals)Item; }
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

        private void btnSelect_Click( object sender, EventArgs e )
            {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel Files (.xls)|*.xls";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                excelOpenPath.Text = ofd.FileName;
                }
            }

        private void btnOpen_Click( object sender, EventArgs e )
            {
            if (!string.IsNullOrEmpty( excelOpenPath.Text ) && File.Exists( excelOpenPath.Text ))
                {
                Process.Start( new ProcessStartInfo( excelOpenPath.Text ) );
                }
            else
                {
                "Файл не найден".AlertBox();
                }
            }

        private void btnFill_Click( object sender, EventArgs e )
            {
            try
                {
                if (!TransactionManager.TransactionManagerInstance.BeginBusinessTransaction())
                    {
                    return;
                    }
                string error = "";
                if (!filesLoader.TryLoadApprovals( excelOpenPath.Text, out error ))
                    {
                    error.AlertBox();
                    }
                }
            catch (Exception ex)
                {
                Console.WriteLine( ex.ToString() );
                "Ошибка при обработке Excel - файла".AlertBox();
                }
            finally
                {
                TransactionManager.TransactionManagerInstance.CompleteBusingessTransaction();
                }
            }
        }
    }