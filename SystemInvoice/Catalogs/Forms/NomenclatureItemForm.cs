using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SystemInvoice.Documents;
using Aramis.DatabaseConnector;
using Aramis.UI;
using DevExpress.XtraBars;
using Aramis.Attributes;
using Aramis.Core;
using Aramis.Enums;
using Aramis.UI.WinFormsDevXpress;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using MatrixBuilding.Totals;

namespace SystemInvoice.Catalogs.Forms
    {
    [global::Aramis.Attributes.View(DBObjectGuid = "AC3B7088-DDB8-49D7-9C7F-1063ACEF0619", ViewType = ViewFormType.CatalogItem)]
    public partial class NomenclatureItemForm : DevExpress.XtraBars.Ribbon.RibbonForm, IItemForm
        {
        public NomenclatureItemForm()
            {
            InitializeComponent();
            }

        public IDatabaseObject Item
            {
            get;
            set;
            }

        private void CancelBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            Close();
            }

        private void okBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            Item.Write();
            Close();
            }

        private void WriteBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            WritingResult result = Item.Write();
            if (result == WritingResult.Success)
                {
                Close();
                }
            }

        private void NomenclatureItemForm_Load(object sender, EventArgs e)
            {
            Nomenclature nom = Item as Nomenclature;
            if (nom != null)
                {
                // nom.FillApprovals();
                }
            }

        private void ApprovalsControl_DoubleClick(object sender, EventArgs e)
            {
            var hitInfo = (sender as GridControl).GetHitInfo();
            if (hitInfo.RowHandle < 0) return;

            var nom = Item as Nomenclature;
            var row = nom.Approvals.Rows[approvalsGridView.GetDataSourceRowIndex(hitInfo.RowHandle)];
            UserInterface.Current.ShowItem(typeof(Approvals).GetTableName(), (long)row[nom.ApprovalId]);
            }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
            {
            var q = DB.NewQuery(@"select Id, BaseApproval
	from Approvals
	where MarkForDeleting  = 0 and BaseApproval > 0");
            var wares = new HashSet<long>();
            var total = 0;

            using (var qResult = q.Select())
                {
                while (qResult.Next())
                    {
                    var id = qResult["Id"].ToInt64(true);
                    var approval = A.New<Approvals>(id);
                    var baseApproval = A.New<Approvals>(qResult["BaseApproval"]);
                    wares.Clear();

                    foreach (DataRow row in approval.Nomenclatures.Rows)
                        {
                        var wareId = row[approval.ItemNomenclature].ToInt64();
                        baseApproval.AddWareId(wareId);
                        }

                    if (baseApproval.IsModified)
                        {
                        baseApproval.Write();
                        total++;
                        }
                    }
                }

            string.Format("Обновлено {0} документов!", total).NotifyToUser();
            }
        }
    }