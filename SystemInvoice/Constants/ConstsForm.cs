using Aramis.Platform;
using Catalogs;
using DevExpress.XtraBars;
using DevExpress.XtraTab;
using System;
using System.Windows.Forms;

namespace SystemInvoice.Constants
    {
    public partial class ConstsForm : DevExpress.XtraBars.Ribbon.RibbonForm
        {
        public enum ConstsPages
            {
            TasmManagementTabPage,
            xtraTabPage2,
            xtraTabPage1,
            SalaryPage,
            catalogsPage,
            DiseasesPage,
            waybillPage,
            writeoff,
            lampsPage,
            systemPage
            }

        private readonly ConstsPages FirstPage;

        #region Constructor
        public ConstsForm()
            {
            InitializeComponent();

            lock (SystemConsts.locker)
                {
                // Если мы сюда попали, значит обновление не начнется пока мы не выйдем
                SystemConsts.СonstsAutoUpdating = false;
                }
            }

        public ConstsForm(ConstsPages firstPage)
            : this()
            {
            FirstPage = firstPage;
            }
        #endregion

        #region Open/Close
        private void Itemform_Load(object sender, EventArgs e)
            {
            SetVisibleTabs();
            xtraTabControl1.SelectedTabPageIndex = (int)FirstPage;
            }

        private void ConstsForm_FormClosed(object sender, FormClosedEventArgs e)
            {
            SystemConsts.СonstsAutoUpdating = true;
            }
        #endregion

        #region Navigation
        private void Itemform_KeyDown(object sender, KeyEventArgs e)
            {
            switch (e.KeyCode)
                {
                case Keys.Escape:
                    Close();
                    break;
                }
            }

        private void OKButton_ItemClick(object sender, ItemClickEventArgs e)
            {
            Close();
            }
        #endregion

        #region Options
        private void SetVisibleTabs()
            {
            if (SystemAramis.CurrentUserAdmin)
                {
                foreach (XtraTabPage tab in xtraTabControl1.TabPages)
                    {
                    tab.PageVisible = true;
                    }
                }
            else
                {
                //foreach (DataRow row in SystemAramis.CurrentUser.Roles.Rows)
                //{
                //    if ((long)row["Role"] == Roles.SalaryManager.Id)
                //    {
                //        SalaryPage.PageVisible = true;
                //    }
                //    else if ((long)row["Role"] == Roles.PlantDefenceManager.Id)
                //    {
                //        DiseasesPage.PageVisible = true;
                //    }
                //}
                }
            }
        #endregion

        private void NumberOfPlaces_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
            {

            }
        }
    }