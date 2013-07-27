using System;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.UIInteraction
    {
    /// <summary>
    /// Обеспечивает доступ к специальным событиям табличного контрола
    /// </summary>
    public class GridViewEventManager
        {
        //флаг который разрешает/запрещает вызывать события выбора ячейки
        private bool canCellSelect = false;
        //флаг который определяет что последнее событие выбора ячейки было осуществлено табуляцией
        private bool isTabSelect = false;

        /// <summary>
        /// Вызывается при нажатии на клавишу табуляции и наличии выбраной ячейки
        /// </summary>
        public event Action OnGridViewTabPressed;
        /// <summary>
        /// Вызывается при выборе новой ячейки мышкой
        /// </summary>
        public event Action OnSelectedCellNotByTabChanged;

        private GridView mainView = null;

        public GridViewEventManager(GridView mainView)
            {
            this.mainView = mainView;
            this.mainView.KeyDown += mainView_KeyDown;
            this.mainView.FocusedColumnChanged += mainView_FocusedColumnChanged;
            this.mainView.FocusedRowChanged += mainView_FocusedRowChanged;
            this.mainView.MouseDown += mainView_MouseDown;           
            }

        void mainView_MouseDown(object sender, MouseEventArgs e)
            {
            canCellSelect = true;
            }

        private void mainView_KeyDown(object sender, KeyEventArgs e)
            {
            if (((e.KeyCode & Keys.Back) != Keys.None) && ((e.KeyCode & Keys.LButton) != Keys.None) && ((e.KeyCode & Keys.MButton) == Keys.None))//хитрая комбинация клавиш которая означает что нажат именно таб!
                {
                canCellSelect = false;
                isTabSelect = true;
                }
            else
                {
                canCellSelect = true;
                }
            }

        void mainView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
            {
            raiseSelectionEvents();
            }

        void mainView_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
            {
            raiseSelectionEvents();
            }
        //Проверяет флаги и вызывает события
        private void raiseSelectionEvents()
            {
            checkTabSelect();
            raiseSelectedCellNotByTabChanged();
            }

        private void checkTabSelect()
            {
            if (isTabSelect)
                {
                raiseTabPressed();
                isTabSelect = false;
                }
            }

        private void raiseSelectedCellNotByTabChanged()
            {
            if (canCellSelect)
                {
                if (OnSelectedCellNotByTabChanged != null)
                    {
                    //Console.WriteLine("selected");
                    OnSelectedCellNotByTabChanged();
                    }
                }
            }

        private void raiseTabPressed()
            {
            if (OnGridViewTabPressed != null)
                {
                OnGridViewTabPressed();
                }
            }
        }
    }
