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

namespace SystemInvoice.Catalogs.Forms
    {
    [View( DBObjectGuid = "A8592D01-0B9C-4FED-9BE7-1C54F920003F", ViewType = ViewFormType.CatalogItem )]
    public partial class ApprovalsLoadFormatItemForm : DevExpress.XtraBars.Ribbon.RibbonForm,IItemForm
        {
        public ApprovalsLoadFormatItemForm()
            {
            InitializeComponent();
            }
        public DatabaseObject Item
            {
            get;
            set;
            }

        private void CancelBtn_ItemClick( object sender, ItemClickEventArgs e )
            {
            Close();
            }

        private void okBtn_ItemClick( object sender, ItemClickEventArgs e )
            {
            Item.Write();
            Close();
            }

        private void WriteBtn_ItemClick( object sender, ItemClickEventArgs e )
            {
            WritingResult result = Item.Write();
            if (result == WritingResult.Success)
                {
                Close();
                }
            }
        }
    }