using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.Documents;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.Excel;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking
    {
    /// <summary>
    /// Базовый клас осуществляющий проверку данных в таблице
    /// </summary>
    public abstract class LoadedDocumentCheckerBase
        {
        private RowColumnsErrors internalErrors = new RowColumnsErrors();

        protected SystemInvoiceDBCache dbCache = null;

        public LoadedDocumentCheckerBase(SystemInvoiceDBCache dbCache)
            {
            this.dbCache = dbCache;
            }
        /// <summary>
        /// Добавляет ошибку
        /// </summary>
        /// <param name="columnName">колонка к которой добавляется ошибка</param>
        /// <param name="error">екземпляр ошибки</param>
        protected void AddError(string columnName, CellError error)
            {
            CellErrorsCollection errorsCollection = null;
            if (!internalErrors.TryGetValue(columnName, out errorsCollection))
                {
                errorsCollection = new CellErrorsCollection();
                internalErrors.Add(columnName, errorsCollection);
                }
            errorsCollection.Add(error);
            }

        /// <summary>
        /// Выполняет проверку и возвращает найденные ошибки
        /// </summary>
        /// <param name="rowToCheck">Проверяемая строка</param>
        /// <param name="mapper">Данные о формате загрузки</param>
        /// <param name="isDocumentCurrentlyLoaded">Был ли загружен документ</param>
        /// <param name="currentCheckedColumnName">Колонка ячейки на которой сработал фокус, что проинициировало проверку</param>
        /// <returns>Ошибки</returns>
        public RowColumnsErrors GetRowErrors(DataRow rowToCheck, ExcelMapper mapper, bool isDocumentCurrentlyLoaded, string currentCheckedColumnName)
            {
            internalErrors = new RowColumnsErrors();
            this.CheckRow(rowToCheck, mapper, isDocumentCurrentlyLoaded, currentCheckedColumnName);
            return internalErrors;
            }

        /// <summary>
        /// Проверяет строку
        /// </summary>
        /// <param name="rowToCheck">Строка</param>
        /// <param name="mapper">Данные о формате загрузки</param>
        /// <param name="isDocumentCurrentlyLoaded">Был ли загружен документ</param>
        /// <param name="currentCheckedColumnName">Текущая колонка</param>
        protected abstract void CheckRow(DataRow rowToCheck, ExcelMapper mapper, bool isDocumentCurrentlyLoaded, string currentCheckedColumnName);
        }
    }
