using System;
using System.Data;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.Helpers;
using SystemInvoice.Documents;
using DevExpress.XtraGrid.Views.Grid;
using System.Collections.Generic;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.UIInteraction
    {
    /// <summary>
    /// Заменяет таможенные коды в табличной части инвойса, и записывает изменения в справочник номенклатуры
    /// </summary>
    public class CustomsCodeUpdater
        {
        private SystemInvoiceDBCache systemInvoiceDBCache = null;
        private Invoice invoice = null;
        private GridView mainView = null;
        private FilteredRowsSource filteredRowsSource = null;
        /// <summary>
        /// Используется для хранения списка номенклатур в которых нужно обновить таможенный код
        /// </summary>
        private HashSet<long> updatedNomenclautesSet = new HashSet<long>();

        public CustomsCodeUpdater(SystemInvoiceDBCache systemInvoiceDBCache, Invoice invoice, GridView mainView, FilteredRowsSource filteredRowsSource)
            {
            this.filteredRowsSource = filteredRowsSource;
            this.systemInvoiceDBCache = systemInvoiceDBCache;
            this.invoice = invoice;
            this.mainView = mainView;
            }
        
        /// <summary>
        /// Меняет таможенные код в текущей строке и в связанных строках
        /// </summary>
        public void UpdateCurrentRowCustomsCode()
            {
            //очищаем список обновляемых номенклатур
            updatedNomenclautesSet.Clear();
            int selectedRowIndex = filteredRowsSource.getSourceRow(mainView.FocusedRowHandle);
            var customCodeId = getRowCustomCodeId(selectedRowIndex);
            //выбираем новый таможенный код для текущей строки
            long selectedID = Aramis.UI.UserInterface.Current.SelectItemFromList((new CustomsCode()).GUID, customCodeId);
            if (selectedID <= 0)
                {
                return;
                }
            //обновляем таможенные коды
            this.UpdateCustomsCodesGroup(selectedID, selectedRowIndex);
            }
        
        private long getRowCustomCodeId(int selectedRowIndex)
            {
            long customCodeId = 0;
            if (selectedRowIndex >= 0)
                {
                string customCode = this.getRowCustomsCode(selectedRowIndex);
                customCodeId = systemInvoiceDBCache.CustomsCodesCacheStore.GetCustomsCodeIdForCodeName(customCode);
                }
            return customCodeId;
            }

        private string getRowCustomsCode(int selectedRowIndex)
            {
            DataRow rowToGetInfo = this.invoice.Goods.Rows[selectedRowIndex];
            return InvoiceDataRetrieveHelper.GetRowCustomsCode(rowToGetInfo);
            }

        private long getRowNomenclature(int rowIndex)
            {
            DataRow rowForNomenclature = this.invoice.Goods.Rows[rowIndex];
            return InvoiceDataRetrieveHelper.GetRowNomenclatureId(rowForNomenclature);
            }

        /// <summary>
        /// Обновляет таможенный код в группе строк
        /// </summary>
        /// <param name="selectedId">Айдишник выбранного таможенного кода</param>
        /// <param name="selectedRowIndex">Выбраный номер строки</param>
        private void UpdateCustomsCodesGroup(long selectedId, int selectedRowIndex)
            {
            if (selectedRowIndex >= 0)
                {
                string customsCodeNew = getCustomsCodeCodeName(selectedId);
                string customsCodeOld = getRowCustomsCode(selectedRowIndex);
                long nomenclatureId = getRowNomenclature(selectedRowIndex);
                bool updateOtherRowsOnlyWhenNew = nomenclatureId == 0;
                long customsCodeOldId = getCustomsCodeId(customsCodeOld);
                //обновляем таможенный код в текущей строке
                updateCustomsCodeForRow(selectedRowIndex, customsCodeNew);
                if (nomenclatureId > 0 && customsCodeOldId > 0)
                    {
                    //заменяем ТК во всех строках с такой же номенклатурой
                    this.updateCustomsCodeForSameNomenclature(nomenclatureId, customsCodeNew, selectedId);
                    }
                //заменяем ТК во всех строках таким же значением старого таможенного кода
                this.updateCustomsCodeForSameOldCustomsCode(customsCodeOld, customsCodeNew, selectedId, updateOtherRowsOnlyWhenNew);
                }
            }

        private long getCustomsCodeId(string customsCodeOld)
            {
            long customsCodeOldId = systemInvoiceDBCache.CustomsCodesCacheStore.GetCustomsCodeIdForCodeName(customsCodeOld);
            return customsCodeOldId;
            }

        private void updateCustomsCodeForRow(DataRow row, string customsCodeNew)
            {
            if (!row.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.CUSTOM_CODE_INTERNAL_COLUMN_NAME, string.Empty)
                           .Trim().Equals(customsCodeNew))
                {
                row[ProcessingConsts.ColumnNames.CUSTOM_CODE_INTERNAL_COLUMN_NAME] = customsCodeNew;
                }
            }

        private void updateCustomsCodeForRow(int selectedRowIndex, string customsCodeNew)
            {
            DataRow rowToUpdate = this.invoice.Goods.Rows[selectedRowIndex];
            updateCustomsCodeForRow(rowToUpdate, customsCodeNew);
            }

        private string getCustomsCodeCodeName(long selectedId)
            {
            var cachedObject = systemInvoiceDBCache.CustomsCodesCacheStore.GetCachedObject(selectedId);
            if (cachedObject != null)
                {
                return cachedObject.Code;
                }
            return string.Empty;
            }

        private void updateCustomsCodeForSameNomenclature(long nomenclatureId, string customsCodeNew, long customsCodeNewId)
            {
            if (nomenclatureId == 0)
                {
                return;
                }
            if (updatedNomenclautesSet.Add(nomenclatureId))
                {
                writeNewCustomsCodeToDB(nomenclatureId, customsCodeNewId);
                foreach (DataRow row in this.invoice.Goods.Rows)
                    {
                    long nomenclature = InvoiceDataRetrieveHelper.GetRowNomenclatureId(row);
                    if (nomenclature == nomenclatureId)
                        {
                        updateCustomsCodeForRow(row, customsCodeNew);
                        }
                    }
                }
            }

        private static void writeNewCustomsCodeToDB(long nomenclatureId, long customsCodeNewId)
            {
            Nomenclature nomenclature = new Nomenclature();
            nomenclature.Read(nomenclatureId);
            nomenclature.CustomsCodeInternal = new CustomsCode() { Id = customsCodeNewId };
            nomenclature.Write();
            }

        private void updateCustomsCodeForSameOldCustomsCode(string customsCodeOld, string customsCodeNew, long customsCodeNewId, bool updateNewOnly)
            {
            updateNewOnly = false;//в будущем скорее всего для новых позиций скажут не заменять в загруженных строках, пока ставлю - заменять ТК как для загруженных
            //так и для незагруженных номенклатур независимо от того на какой строке мы клацнули что б поменять ТК
            string askMessage = updateNewOnly ?
                "Заменить таможенный  код для всех новых позиций?" :
                "Заменить таможенный  код для всех позиций с таким же таможенным кодом?";
            bool asked = false;
            foreach (DataRow row in this.invoice.Goods.Rows)
                {
                long nomenclatureId = InvoiceDataRetrieveHelper.GetRowNomenclatureId(row);
                string existedCustomsCode = InvoiceDataRetrieveHelper.GetRowCustomsCode(row);
                if ((!updateNewOnly || nomenclatureId == 0) && existedCustomsCode.Equals(customsCodeOld))
                    {
                    if (!asked)
                        {
                        if (!askMessage.Ask())
                            {
                            return;
                            }
                        asked = true;
                        }
                    updateCustomsCodeForRow(row, customsCodeNew);
                    if (!updateNewOnly)
                        {
                        this.updateCustomsCodeForSameNomenclature(nomenclatureId, customsCodeNew, customsCodeNewId);
                        }
                    }
                }
            }

        }
    }
