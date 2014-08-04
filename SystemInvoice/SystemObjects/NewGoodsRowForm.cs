using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SystemInvoice.SystemObjects;
using Aramis.DatabaseConnector;
using DevExpress.XtraBars;
using Aramis.Attributes;
using Aramis.Core;
using Aramis.Enums;
using Aramis.UI.WinFormsDevXpress;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;

namespace SystemInvoice.Catalogs.Forms
    {
    public partial class NewGoodsRowForm : DevExpress.XtraBars.Ribbon.RibbonForm
        {
        public INewGoodsRow Item { get; set; }

        public NewGoodsRowForm()
            {
            InitializeComponent();
            splitContainer.Panel2Collapsed = true;
            }

        private void CancelBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            DialogResult = DialogResult.Cancel;
            }

        private void NewGoodsRowForm_Load(object sender, EventArgs e)
            {
            Item.AddPropertyChanged(Item.Article, () =>
            {
                gridView.Columns["Article"].FilterInfo = new ColumnFilterInfo(string.Format("StartsWith([Article], '{0}')", Item.Article));
                if (!Item.Nomenclature.IsNew && !Item.Nomenclature.Article.Equals(Item.Article))
                    {
                    Item.Nomenclature = new Nomenclature();
                    }
            });
            }

        private void Article_Enter(object sender, EventArgs e)
            {
            setTableVisibility(true);
            }

        private void setTableVisibility(bool isTableVisible)
            {
            splitContainer.Panel1Collapsed = isTableVisible;
            splitContainer.Panel2Collapsed = !isTableVisible;
            }

        private void Article_Leave(object sender, EventArgs e)
            {
            if (!SearchRowsControl.Focused)
                {
                setTableVisibility(false);

                checkNomenclature();
                }
            }

        private void checkNomenclature()
            {
            if (Item.Nomenclature.IsNew || !Item.Nomenclature.Article.Equals(Item.Article))
                {
                Item.Nomenclature = new Nomenclature();
                }
            }

        private void SearchRowsControl_DoubleClick(object sender, EventArgs e)
            {
            Point mousePosition = (sender as GridControl).PointToClient(GridControl.MousePosition);
            var hitInfo = gridView.CalcHitInfo(mousePosition);
            if (hitInfo.RowHandle >= 0)
                {
                setTableVisibility(false);
                var rowIndex = gridView.GetDataSourceRowIndex(hitInfo.RowHandle);
                fillFromNomenclature(Item.SearchRows[rowIndex].Id);
                }
            }

        private void fillFromNomenclature(long nomenclatureId)
            {
            Item.Nomenclature = A.New<Nomenclature>(nomenclatureId);
            Item.Article = Item.Nomenclature.Article;
            Item.Country = A.New<Country>(Item.Nomenclature.Country.Id);
            Item.UnitOfMeasure = A.New<UnitOfMeasure>(Item.Nomenclature.UnitOfMeasure.Id);
            Item.Price = Convert.ToDecimal(Item.Nomenclature.Price);
            Item.NameDecl = Item.Nomenclature.NameDecl;
            Item.NameInvoice = Item.Nomenclature.Description;
            Item.ItemProducer = A.New<Manufacturer>(Item.Nomenclature.Manufacturer.Id);
            Item.InternalCode = A.New<CustomsCode>(Item.Nomenclature.CustomsCodeInternal.Id);
            Item.TradeMark = A.New<ITradeMark>(Item.Nomenclature.TradeMark.Id);
            }

        private void SearchRowsControl_Leave(object sender, EventArgs e)
            {
            if (Article.Focused) return;

            setTableVisibility(false);

            checkNomenclature();
            }

        private void okBtn_ItemClick(object sender, ItemClickEventArgs e)
            {
            DialogResult = DialogResult.OK;
            }
        }
    }