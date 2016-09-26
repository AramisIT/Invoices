using System;
using System.Collections.Generic;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using System.Data;
using SystemInvoice.Excel;
using SystemInvoice.Catalogs;
using SystemInvoice.Documents;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking
    {
    /// <summary>
    /// Выполняет проверку данных, управляет закраской строк, управляет разрешениями конфликтов в загруженных данных и данных которые хранятся в базе
    /// </summary>
    public class InvoiceChecker
        {
        private RowErrors errors = new RowErrors();
        private LoadedDocumentCheckerGlobal checker = null;
        private SystemInvoiceDBCache dbCache = null;
        private Invoice invoice = null;

        public event Action ErrorsCountChanged = null;

        private void raiseErorCountChanged()
            {
            if (ErrorsCountChanged != null)
                {
                ErrorsCountChanged();
                }
            }

        public InvoiceChecker(Invoice invoice, SystemInvoiceDBCache dbCache)
            {
            this.dbCache = dbCache;
            checker = new LoadedDocumentCheckerGlobal(dbCache);
            this.invoice = invoice;
            }

        /// <summary>
        /// Проверяет таблицу.
        /// </summary>
        /// <param name="isLoaded">Был ли документ загружен из файла или загружен из базы. Необходимо передавать в связи с тем что не все данные из таблицы инвойса при
        /// сохраненнии записываются в БД (поскольку не хватает места в таблице), и соответственно для тех колонок которые не сохранялись а потом были загруженны
        /// из БД, проверять бессмысленно
        /// </param>
        public void CheckTable(bool isLoaded)
            {
            if (invoice == null || invoice.ExcelLoadingFormat == null)
                {
                return;
                }
            errors.Clear();
            DataTable table = invoice.Goods;
            ExcelMapper mapper = this.createMapper(invoice.ExcelLoadingFormat);
            for (int i = 0; i < table.Rows.Count; i++)
                {
                RowColumnsErrors rowErrors = checker.GetRowErrors(table.Rows[i], mapper, isLoaded, string.Empty);
                if (rowErrors != null && rowErrors.Count > 0)
                    {
                    errors.Add(i, rowErrors);
                    }
                }
            raiseErorCountChanged();
            }

        private ExcelMapper createMapper(ExcelLoadingFormat loadingFormat)
            {
            ExcelMapper mapper = new ExcelMapper();
            foreach (DataRow row in loadingFormat.ColumnsMappings.Rows)
                {
                int columnIndex = 0;
                int columnNameIndex = row.TryGetColumnValue<int>(ProcessingConsts.EXCEL_LOAD_FORMAT_TARGET_COLUMN_COLUMN_NAME, -1);
                string unloadIndex = row.TryGetColumnValue<string>(ProcessingConsts.EXCEL_UNLOAD_UNPROCESSED_COLUMNS_INDEX_MAPPING, "");
                if (columnNameIndex >= 0 && !string.IsNullOrEmpty(unloadIndex))
                    {
                    string columnName = ((InvoiceColumnNames)columnNameIndex).ToString();
                    if (int.TryParse(unloadIndex, out columnIndex))
                        {
                        mapper.TryAddExpression(columnName, ExcelMapper.IndexKey, unloadIndex);
                        }
                    }
                }
            return mapper;
            }

        /// <summary>
        /// Проверяет, содержит ли строка ошибки. Ошибки с уведомлениями не учитываются
        /// </summary>
        /// <param name="rowIndex">Индекс строки</param>
        public bool IsRowValid(int rowIndex)
            {
            return errors.IsRowValid(rowIndex);
            }

        /// <summary>
        /// Проверяет нужно ли для данной ячейки отображать ошибку
        /// </summary>
        public bool HaveDisplayInvalid(int rowIndex, string columnName)
            {
            return errors.HaveDisplayError(rowIndex, columnName);
            }

        /// <summary>
        /// Возвращает текст - кодировку цвета, которым закрашивается ячейка
        /// </summary>
        public string GetCellCollor(int rowIndex, string columnName)
            {
            if (HaveDisplayInvalid(rowIndex, columnName))
                {
                return ProcessingConsts.CELL_ERROR_COLOR;
                }
            if (!isRowLoaded(rowIndex))
                {
                return ProcessingConsts.ROW_NOT_LOADED_COLOR;
                }
            return null;
            }

        /// <summary>
        /// Проверяет строку, и обновляет информацию об ошибках в этой строке
        /// </summary>
        /// <param name="rowIndex">Индекс строки</param>
        /// <param name="isDocumentCurrentlyLoaded">Был ли документ загружен из файла</param>
        /// <param name="checkInitializeSourceColumnName">Текущая колонка</param>
        public void CheckRow(int rowIndex, bool isDocumentCurrentlyLoaded, string checkInitializeSourceColumnName)
            {
            if (rowIndex < 0)
                {
                return;
                }
            DataTable table = invoice.Goods;
            if (table.Rows.Count <= rowIndex)
                {
                if (errors.ContainsKey(rowIndex))
                    {
                    errors.Remove(rowIndex);
                    }
                return;
                }
            RowColumnsErrors rowErrors = checker.GetRowErrors(table.Rows[rowIndex], createMapper(invoice.ExcelLoadingFormat), isDocumentCurrentlyLoaded, checkInitializeSourceColumnName);
            if (errors.ContainsKey(rowIndex))
                {
                errors.Remove(rowIndex);
                }
            if (rowErrors != null && rowErrors.Count > 0)
                {
                errors.Add(rowIndex, rowErrors);
                }
            raiseErorCountChanged();
            }


        private bool isRowLoaded(int rowIndex)
            {
            return errors.IsRowLoaded(rowIndex);
            }

        /// <summary>
        /// Возвращает первую ошибку в ячейке
        /// </summary>
        public CellError GetError(int rowIndex, string columnName)
            {
            return errors.GetError(rowIndex, columnName);
            }

        public string GetNotification(int rowIndex, string columnName)
            {
            return errors.GetNotification(rowIndex, columnName);
            }

        /// <summary>
        /// Возвращает текст с описанием общего колличества ошибок
        /// </summary>
        public string ErrorsDescription
            {
            get
                {
                int totalErrors = this.GetTotalErrorsCount();
                return string.Format("Количество ошибок: {0}", totalErrors);
                }
            }

        /// <summary>
        /// Ищет следующую ошибку
        /// </summary>
        /// <param name="currentColumnsOrder">Последовательность колонок, в которой нужно производить поиск</param>
        /// <param name="lastFocusedRow">Начальная строка с которой осуществлять поиск</param>
        /// <param name="lastFocusedColumn">Начальная колонка с которой осуществлять</param>
        public CellErrorPosition GetNextError(List<string> currentColumnsOrder, int lastFocusedRow, string lastFocusedColumn)
            {
            CellErrorPosition errorPosition = null;
            DataTable table = invoice.Goods;
            int columnSearchStartIndex = currentColumnsOrder.IndexOf(lastFocusedColumn);
            int rowSearchStartIndex = lastFocusedRow >= 0 ? lastFocusedRow : 0;
            int currentColumnIndex = lastFocusedRow >= 0 ? columnSearchStartIndex + 1 : 0;
            int fromSerarchRowIndex = rowSearchStartIndex;
            int toSearchRowIndex = table.Rows.Count;
            int toSearchColumnIndex = currentColumnsOrder.Count;
            bool startFromBegining = false;
            while (errorPosition == null)
                {
                for (int rowIndex = fromSerarchRowIndex; rowIndex < toSearchRowIndex; rowIndex++)
                    {
                    if (!errors.ContainsKey(rowIndex))
                        {
                        currentColumnIndex = 0;
                        continue;
                        }
                    RowColumnsErrors rowErrors = errors[rowIndex];
                    if (startFromBegining && rowIndex == rowSearchStartIndex)
                        {
                        toSearchColumnIndex = columnSearchStartIndex;
                        }
                    for (; currentColumnIndex < toSearchColumnIndex; currentColumnIndex++)
                        {
                        string columnToCheck = currentColumnsOrder[currentColumnIndex];
                        if (rowErrors.ContainsKey(columnToCheck))
                            {
                            CellErrorsCollection cellErrors = rowErrors[columnToCheck];
                            for (int cellErrorIndex = 0; cellErrorIndex < cellErrors.Count; cellErrorIndex++)
                                {
                                CellError error = cellErrors[cellErrorIndex];
                                if (error != null)
                                    {
                                    errorPosition = new CellErrorPosition(rowIndex, columnToCheck);
                                    return errorPosition;
                                    }
                                }
                            }
                        }
                    currentColumnIndex = 0;
                    }
                fromSerarchRowIndex = 0;
                toSearchRowIndex = rowSearchStartIndex + 1;
                toSearchColumnIndex = currentColumnsOrder.Count;
                if (startFromBegining)
                    {
                    return null;
                    }
                startFromBegining = true;
                }
            return null;
            }

        /// <summary>
        /// Возвращает общее количество ошибок в колонке таблицы
        /// </summary>
        public int GetColumnErrors(string columnName)
            {
            int columnErrorsCount = 0;
            foreach (RowColumnsErrors rowColumnError in errors.Values)
                {
                if (rowColumnError.ContainsKey(columnName))
                    {
                    columnErrorsCount += rowColumnError[columnName].NonNotificationErrorsCount;
                    }
                }
            return columnErrorsCount;
            }

        public bool PriceAndInternalCodeIsCorrect()
            {
            var priceColumnName = InvoiceColumnNames.Price.ToString();
            var wareCodeColumnName = InvoiceColumnNames.CustomsCodeIntern.ToString();

            var pricesErrors = new StringBuilder();
            foreach (var kvp in errors)
                {
                RowColumnsErrors rowColumnError = kvp.Value;
                var rowNumber = kvp.Key + 1;

                if (rowColumnError.ContainsKey(priceColumnName))
                    {
                    pricesErrors.AppendLine(string.Format(@"№ {0}", rowNumber));
                    }

                if (rowColumnError.ContainsKey(wareCodeColumnName))
                    {
                    string.Format(@"Ошибка в коде товара, стр. № {0}.
Данный формат загрузки запрещает выгрузку с неверным кодом товара!", rowNumber).ErrorBox();
                    return false;
                    }
                }

            if (pricesErrors.Length > 0)
                {
                var message = string.Format(@"Перечень строк с ошибками в поле ""Цена"":
{0}

Продолжить выгрузку?", pricesErrors);
                return message.Ask();
                }

            return true;
            }

        public int GetTotalErrorsCount()
            {
            int totalCount = 0;
            foreach (RowColumnsErrors rowColumnError in errors.Values)
                {
                foreach (CellErrorsCollection cellErrors in rowColumnError.Values)
                    {
                    totalCount += cellErrors.NonNotificationErrorsCount;
                    }
                if (!rowColumnError.IsRowLoaded)
                    {
                    totalCount--;
                    }
                }
            return totalCount;
            }

        public int NotLoadedRowsCount
            {
            get
                {
                if (errors != null)
                    {
                    return errors.NonLoadedRowsCount;
                    }
                return 0;
                }
            }

        public int LoadedRowsCount
            {
            get
                {
                if (this.invoice != null)
                    {
                    return this.invoice.Goods.Rows.Count - NotLoadedRowsCount;
                    }
                return 0;
                }
            }
        /// <summary>
        /// Копирует все значения из БД в колонку таблицы для колонки которая соответствует ошибке
        /// </summary>
        public void CopyToColumnFromDB(CellError currentError, int currentRowIndex, bool isDocumentCurrentlyLoaded)
            {
            CompareWithDBCellError compareError = currentError as CompareWithDBCellError;
            if (compareError == null || compareError.IsFailToGetDB || currentRowIndex < 0)
                {
                return;
                }
            string columnName = compareError.ColumnName;
            setTableValues(null, columnName, isDocumentCurrentlyLoaded);
            }

        /// <summary>
        /// Записывает в БД все данные из ошибочных ячеек для колонки которая соответствует ошибке
        /// </summary>
        public void CopyToDataBaseFromCurrentColumn(CellError currentError, int currentRowIndex, bool isDocumentCurrentlyLoaded)
            {
            CompareWithDBCellError compareError = currentError as CompareWithDBCellError;
            if (compareError == null || compareError.IsFailToGetDB || currentRowIndex < 0)
                {
                return;
                }
            string columnName = compareError.ColumnName;
            setDatabaseValues(null, columnName, isDocumentCurrentlyLoaded);
            }

        /// <summary>
        /// Копирует в таблицу значения из базы для всех ячеек у которых такое же ошибочное значение как и у исходной, для колонки соответствующей ошибке
        /// </summary>
        public void CopyToSameCellsFromDB(CellError currentError, int currentRowIndex, bool isDocumentCurrentlyLoaded)
            {
            CompareWithDBCellError compareError = currentError as CompareWithDBCellError;
            if (compareError == null || compareError.IsFailToGetDB || currentRowIndex < 0)
                {
                return;
                }
            string columnName = compareError.ColumnName;
            setTableValues(compareError, columnName, isDocumentCurrentlyLoaded);
            }

        /// <summary>
        /// Копирует в базу значения из таблицы для всех ячеек у которых такое же ошибочное значение как и у исходной, для колонки соответствующей ошибке
        /// </summary>
        public void CopyToCellFromDataBase(CellError currentError, int currentRowIndex, bool isDocumentCurrentlyLoaded)
            {
            CompareWithDBCellError compareError = currentError as CompareWithDBCellError;
            if (compareError == null || compareError.IsFailToGetDB || currentRowIndex < 0)
                {
                return;
                }
            try
                {
                DataRow row = getEditRow(currentRowIndex);
                if (row != null)
                    {
                    compareError.SetCurrentErrorCellAsInDB(row, this.dbCache);
                    }
                }
            catch (CannotWriteToDBException cannotWriteException)
                {
                cannotWriteException.Message.AlertBox();
                }
            finally
                {
                dbCache.RefreshCache();
                this.CheckRow(currentRowIndex, isDocumentCurrentlyLoaded, compareError.ColumnName);
                this.CheckTable(isDocumentCurrentlyLoaded);
                }
            }

        /// <summary>
        /// Записывает в базу значение из ошибочной ячейки
        /// </summary>
        public void CopyToDataBaseFromSameCells(CellError currentError, int currentRowIndex, bool isDocumentCurrentlyLoaded)
            {
            CompareWithDBCellError compareError = currentError as CompareWithDBCellError;
            if (compareError == null || compareError.IsFailToGetDB || currentRowIndex < 0)
                {
                return;
                }
            string columnName = compareError.ColumnName;
            setDatabaseValues(compareError, columnName, isDocumentCurrentlyLoaded);
            }

        /// <summary>
        /// Записывает в таблицу значения из ошибочной ячейки
        /// </summary>
        public void CopyToDataBaseFromCurrentCell(CellError currentError, int currentRowIndex, bool isDocumentCurrentlyLoaded)
            {
            CompareWithDBCellError compareError = currentError as CompareWithDBCellError;
            if (compareError == null || compareError.IsFailToGetDB || currentRowIndex < 0)
                {
                return;
                }
            try
                {
                DataRow row = getEditRow(currentRowIndex);
                if (row != null)
                    {
                    compareError.SetCurrentDBValueAsInCell(row, this.dbCache);
                    }
                }
            catch (CannotWriteToDBException cannotWriteException)
                {
                cannotWriteException.Message.AlertBox();
                }
            finally
                {
                dbCache.RefreshCache();
                this.CheckRow(currentRowIndex, isDocumentCurrentlyLoaded, compareError.ColumnName);
                this.CheckTable(isDocumentCurrentlyLoaded);
                }
            }

        private void setDatabaseValues(CompareWithDBCellError compareError, string columnName, bool isDocumentCurrentlyLoaded)
            {
            try
                {
                Dictionary<CellErrorPosition, List<CompareWithDBCellError>> allErrorsPositions = this.getSameErrors(compareError, columnName);
                foreach (CellErrorPosition position in allErrorsPositions.Keys)
                    {
                    List<CompareWithDBCellError> errorsToProcess = allErrorsPositions[position];
                    foreach (CompareWithDBCellError error in errorsToProcess)
                        {
                        DataRow row = getEditRow(position.RowIndex);
                        if (row == null)
                            {
                            continue;
                            }
                        error.SetCurrentDBValueAsInCell(row, this.dbCache);
                        }
                    }
                }
            catch (CannotWriteToDBException cannotWriteException)
                {
                cannotWriteException.Message.AlertBox();
                }
            finally
                {
                dbCache.RefreshCache();
                this.CheckTable(isDocumentCurrentlyLoaded);
                }
            }

        private void setTableValues(CompareWithDBCellError compareError, string columnName, bool isDocumentCurrentlyLoaded)
            {
            try
                {
                Dictionary<CellErrorPosition, List<CompareWithDBCellError>> allErrorsPositions = this.getSameErrors(compareError, columnName);
                foreach (CellErrorPosition position in allErrorsPositions.Keys)
                    {
                    List<CompareWithDBCellError> errorsToProcess = allErrorsPositions[position];
                    foreach (CompareWithDBCellError error in errorsToProcess)
                        {
                        DataRow row = getEditRow(position.RowIndex);
                        if (row == null)
                            {
                            continue;
                            }
                        error.SetCurrentErrorCellAsInDB(row, this.dbCache);
                        }
                    }
                }
            catch (CannotWriteToDBException cannotWriteException)
                {
                cannotWriteException.Message.AlertBox();
                }
            finally
                {
                dbCache.RefreshCache();
                this.CheckTable(isDocumentCurrentlyLoaded);
                }
            }

        private Dictionary<CellErrorPosition, List<CompareWithDBCellError>> getSameErrors(CompareWithDBCellError compareError, string columnNameForError)
            {
            Dictionary<CellErrorPosition, List<CompareWithDBCellError>> positions = new Dictionary<CellErrorPosition, List<CompareWithDBCellError>>();
            foreach (int rowIndex in errors.Keys)
                {
                CellErrorPosition currentPosition = new CellErrorPosition(rowIndex, columnNameForError);
                RowColumnsErrors columnErrors = errors[rowIndex];
                if (columnErrors.ContainsKey(columnNameForError))
                    {
                    CellErrorsCollection cellErors = columnErrors[columnNameForError];
                    foreach (CellError error in cellErors)
                        {
                        CompareWithDBCellError haveToAdd = (error.Equals(compareError) || compareError == null) ? error as CompareWithDBCellError : null;
                        if (haveToAdd != null)
                            {
                            if (positions.ContainsKey(currentPosition))
                                {
                                positions[currentPosition].Add(haveToAdd);
                                }
                            else
                                {
                                positions.Add(currentPosition, new List<CompareWithDBCellError>() { haveToAdd });
                                }
                            }
                        }
                    }
                }
            return positions;
            }

        private DataTable editTable
            {
            get { return this.invoice.Goods; }
            }

        private DataRow getEditRow(int index)
            {
            if (editTable.Rows.Count <= index || index < 0)
                {
                return null;
                }
            return editTable.Rows[index];
            }
        }
    }
