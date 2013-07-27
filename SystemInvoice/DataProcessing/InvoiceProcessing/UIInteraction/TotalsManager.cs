using SystemInvoice.DataProcessing.InvoiceProcessing.GroupItemsEditors;
using SystemInvoice.Documents;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.UIInteraction
    {
    /// <summary>
    /// Управляет отображением итоговых ячеек в табличном контроле
    /// </summary>
    public class TotalsManager
        {
        bool intitializationLoadPlacesTotal = false;
        bool initializationSumTotal = false;
        bool initializationGrossWeightTotal = false;
        bool initializationNetWeightTotal = false;
        bool initiazlizationCountTotal = false;

        private bool foouterInitialized = false;
        private BottomTotalsCalculator bottomTotalsCalculator = null;
        private Invoice Invoice = null;
        private GridView mainView = null;

        public TotalsManager(Invoice invoice, GridView mainView, IEditableRowsSource editableRowsSource)
            {
            this.mainView = mainView;
            this.Invoice = invoice;
            this.bottomTotalsCalculator = new BottomTotalsCalculator(invoice, editableRowsSource);
            initializeFooter();
            bottomTotalsCalculator.RefreshTotals();
            beginInitTotals();
            this.mainView.UpdateTotalSummary();
            endInitTotals();
            }

        public ITotalsState TotalsState
            {
            get
                {
                return bottomTotalsCalculator;
                }
            }

        /// <summary>
        /// Устанавливает итоговые ячейки для футера грида
        /// </summary>
        private void initializeFooter()
            {
            if (!foouterInitialized && mainView != null && mainView.Columns.Count > 0
                && mainView.Columns["ItemNumberOfPlaces"] != null
                && mainView.Columns["Sum"] != null
                && mainView.Columns["ItemGrossWeight"] != null
                && mainView.Columns["NetWeight"] != null
                && mainView.Columns["Count"] != null)
                {
                string placesCount = InvoiceColumnNames.ItemNumberOfPlaces.ToString();
                string summColumn = InvoiceColumnNames.Sum.ToString();
                string grossColumn = InvoiceColumnNames.ItemGrossWeight.ToString();
                string netWeightColumnName = InvoiceColumnNames.NetWeight.ToString();
                string countColumnName = InvoiceColumnNames.Count.ToString();
                GridColumn placesColumn = mainView.Columns[placesCount];
                placesColumn.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                placesColumn.SummaryItem.Tag = "Places";
                GridColumn totalSummColumn = mainView.Columns[summColumn];
                totalSummColumn.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                totalSummColumn.SummaryItem.Tag = "TotalSumm";
                GridColumn grossWeightColumn = mainView.Columns[grossColumn];
                grossWeightColumn.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                grossWeightColumn.SummaryItem.Tag = "GrossWeight";
                GridColumn netWeightColumn = mainView.Columns[netWeightColumnName];
                netWeightColumn.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                netWeightColumn.SummaryItem.Tag = "NetWeight";
                GridColumn countColumn = mainView.Columns[countColumnName];
                countColumn.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                countColumn.SummaryItem.Tag = "Count";
                mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
                foouterInitialized = true;
                }
            }

        private void beginInitTotals()
            {
            setInitializationState(true);
            }

        private void endInitTotals()
            {
            setInitializationState(false);
            }

        /// <summary>
        /// Устанавливает флаги которые задают что мы должны брать итоги не из рассчетов а из сохраненных значений в базе - 
        /// это необходимо что бы мы лишний раз не обновляли итоги которые хранятся в документе, иначе, даже если  мы установим то же значение итогов - 
        /// у нас все равно документ будет помечен как измененный и соответственно нам при закрытии будет предложено его сохранить, хотя мы реально ничего не меняли
        /// </summary>
        /// <param name="state"></param>
        private void setInitializationState(bool state)
            {
            intitializationLoadPlacesTotal = state;
            initializationSumTotal = state;
            initializationGrossWeightTotal = state;
            initializationNetWeightTotal = state;
            initiazlizationCountTotal = state;
            }

        void mainView_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
            {
            if (this.bottomTotalsCalculator == null || this.Invoice == null)
                {
                return;
                }
            GridColumnSummaryItem item = e.Item as GridColumnSummaryItem;
            GridView view = sender as GridView;
            if (Equals("Places", item.Tag))
                {
                setTotalPlaces(e);
                }
            if (Equals("TotalSumm", item.Tag))
                {
                setTotalSumm(e);
                }
            if (Equals("GrossWeight", item.Tag))
                {
                setTotalGrossWeight(e);
                }
            if (Equals("NetWeight", item.Tag))
                {
                setTotalNetWeight(e);
                }
            if (Equals("Count", item.Tag))
                {
                setTotalCount(e);
                }
            }

        private void setTotalCount(DevExpress.Data.CustomSummaryEventArgs e)
            {
            int visibleCurrent = (int)this.bottomTotalsCalculator.VisibleTotalCount;
            int countCurrent = (int)this.bottomTotalsCalculator.TotalCount;
            if (initiazlizationCountTotal)
                {
                countCurrent = this.Invoice.CountTotal;
                visibleCurrent = countCurrent;
                }
            else
                {
                this.Invoice.CountTotal = countCurrent;
                }
            e.TotalValue = visibleCurrent;// countCurrent;
            }

        private void setTotalNetWeight(DevExpress.Data.CustomSummaryEventArgs e)
            {
            double visibleNetWeightCurrent = this.bottomTotalsCalculator.VisibleTotalNetWeight;
            double netWeightCurrent = this.bottomTotalsCalculator.TotalNetWeight;
            if (initializationNetWeightTotal)
                {
                netWeightCurrent = this.Invoice.NetWeightTotal;
                visibleNetWeightCurrent = netWeightCurrent;
                }
            else
                {
                this.Invoice.NetWeightTotal = netWeightCurrent;
                }
            e.TotalValue = visibleNetWeightCurrent; // netWeightCurrent;
            }

        private void setTotalGrossWeight(DevExpress.Data.CustomSummaryEventArgs e)
            {
            double visibleGrossWeightCurrent = this.bottomTotalsCalculator.VisibleTotalGrossWeight;
            double grossWeightCurrent = this.bottomTotalsCalculator.TotalGrossWeight;
            if (initializationGrossWeightTotal)
                {
                grossWeightCurrent = this.Invoice.GrossWeightTotal;
                visibleGrossWeightCurrent = grossWeightCurrent;
                }
            else
                {
                this.Invoice.GrossWeightTotal = grossWeightCurrent;
                }
            e.TotalValue = visibleGrossWeightCurrent; //grossWeightCurrent;
            }

        private void setTotalSumm(DevExpress.Data.CustomSummaryEventArgs e)
            {
            double visibleSummCurrent = this.bottomTotalsCalculator.VisibleTotalPrice;
            double summCurrent = this.bottomTotalsCalculator.TotalPrice;
            if (initializationSumTotal)
                {
                summCurrent = this.Invoice.SumTotal;
                visibleSummCurrent = summCurrent;
                }
            else
                {
                this.Invoice.SumTotal = summCurrent;
                }
            e.TotalValue = visibleSummCurrent; //summCurrent;
            }

        private void setTotalPlaces(DevExpress.Data.CustomSummaryEventArgs e)
            {
            int visiblePlacesCurrent = (int)this.bottomTotalsCalculator.VisibleTotalNumberOfPlaces;
            int placesTotalsCurrent = (int)this.bottomTotalsCalculator.TotalNumberOfPlaces;
            if (intitializationLoadPlacesTotal)
                {
                placesTotalsCurrent = this.Invoice.PlacesTotal;
                visiblePlacesCurrent = placesTotalsCurrent;
                }
            else
                {
                this.Invoice.PlacesTotal = placesTotalsCurrent;
                }
            e.TotalValue = visiblePlacesCurrent;//placesTotalsCurrent;
            }

        public void RefreshTotals()
            {
            bottomTotalsCalculator.RefreshTotals();
            this.mainView.UpdateTotalSummary();
           // mainView.GridControl.
            }
        }
    }
