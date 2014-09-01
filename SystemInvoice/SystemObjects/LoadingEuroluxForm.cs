using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SystemInvoice.SystemObjects;
using Aramis;
using Aramis.DatabaseConnector;
using DevExpress.XtraBars;
using Aramis.Attributes;
using Aramis.Core;
using Aramis.Enums;
using Aramis.UI.WinFormsDevXpress;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;

namespace SystemInvoice.Catalogs.Forms
    {
    public partial class LoadingEuroluxForm : DevExpress.XtraBars.Ribbon.RibbonForm
        {
        public ILoadingEurolux Item { get; set; }

        private LoadingEuroluxBehaviour itemBehaviour
            {
            get { return Item.GetBehaviour<LoadingEuroluxBehaviour>(); }
            }

        public LoadingEuroluxForm()
            {
            InitializeComponent();
            }

        private void CancelBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            DialogResult = DialogResult.Cancel;
            }

        private void NewGoodsRowForm_Load(object sender, EventArgs e)
            {

            }

        private void okBtn_ItemClick(object sender, ItemClickEventArgs e)
            {

            }

        private void file_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
            {
            string fileName;
            if (!AramisIO.ChooseExcel97_2003File(out fileName)) return;

            progressBar.Position = 0;
            (sender as ButtonEdit).Text = fileName;
            itemBehaviour.LoadExcelFile(fileName, notifyPercentChanged);
            }

        private void notifyPercentChanged(double percent)
            {
            if (this.InvokeRequired)
                {
                this.Invoke(new Action<double>(notifyPercentChanged), new object[] { percent });
                }
            else
                {
                var currentValue = Convert.ToInt32(percent * 100.0);
                progressBar.Position = currentValue;
                progressBar.Update();
                }
            }


        }
    }