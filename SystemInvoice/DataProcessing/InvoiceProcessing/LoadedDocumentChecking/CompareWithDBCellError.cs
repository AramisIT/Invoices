using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.Catalogs;
using Aramis.Core;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking
    {
    /// <summary>
    /// Базовый клас для ошибок, которые могут быть исправлены пользователем прямо в таблице, используя контекстное меню
    /// </summary>
    public class CompareWithDBCellError : CellError
        {
        public readonly bool IsFailToGetDB;
        public readonly string InDocumentValue;
        public readonly string InDBValue;
        public readonly string ColumnName;
        /// <summary>
        /// Возвращает или устанавливает допускается ли только исправление ошибки путем копирования в ячейку данных из БД
        /// </summary>
        public bool CanCopyFromDBOnly { get; protected set; }

        public CompareWithDBCellError(string inDocumentValue, string inDbValue, string columnName)
            {
            this.InDocumentValue = inDocumentValue;
            this.InDBValue = inDbValue;
            this.IsFailToGetDB = false;
            this.ColumnName = columnName;
            this.CanCopyFromDBOnly = false;
            }

        public CompareWithDBCellError()
            : this(string.Empty, string.Empty, string.Empty)
            {
            this.IsFailToGetDB = true;
            }
        /// <summary>
        /// Устанавливает в ячейку значение из БД
        /// </summary>
        /// <param name="row">Строка содержащая исправляемую ячейку</param>
        /// <param name="dbCache">Кеш</param>
        public virtual void SetCurrentErrorCellAsInDB(DataRow row, SystemInvoiceDBCache dbCache)
            {
            if (row.Table.Columns.Contains(ColumnName))
                {
                row[ColumnName] = InDBValue;
                }
            }
        /// <summary>
        /// Записывает в базу значение из ячейки. Не имеет реализации по умолчанию.
        /// </summary>
        /// <param name="row">Строка содержащая ячейку с ошибкой</param>
        /// <param name="dbCache">Кэш</param>
        public virtual void SetCurrentDBValueAsInCell(DataRow row, SystemInvoiceDBCache dbCache)
            {
            }

        public override int GetHashCode()
            {
            return InDocumentValue.GetHashCode() ^ InDBValue.GetHashCode();
            }

        public override bool Equals(object obj)
            {
            CompareWithDBCellError other = obj as CompareWithDBCellError;
            if (other == null)
                {
                return false;
                }
            return other.IsFailToGetDB.Equals(IsFailToGetDB) && other.InDocumentValue.Equals(InDocumentValue) && other.GetType().Equals(GetType());
            }

        /// <summary>
        /// Возвращает екземпляр номенклатуры для загруженной строки
        /// </summary>
        protected Nomenclature readNomenclature(DataRow row)
            {
            string nomenclatureIdColumnName = ProcessingConsts.ColumnNames.FOUNDED_NOMENCLATURE_COLUMN_NAME;
            long nomenclatureId = row.TrySafeGetColumnValue<long>(nomenclatureIdColumnName, 0);
            if (nomenclatureId <= 0)
                {
                return null;
                }
            var nomenclature = A.New<Nomenclature>(nomenclatureId);
            return nomenclature;
            }

        /// <summary>
        /// Возвращает текст ошибки
        /// </summary>
        public string ErrorDescription
            {
            get
                {
                return FormattErrorMessage(InDocumentValue, InDBValue);
                }
            }
        /// <summary>
        /// Формирует текст ошибки
        /// </summary>
        protected virtual string FormattErrorMessage(string inDocVal, string inDBVal)
            {
            return string.Format(@"""{0}"" а должно быть ""{1}""", inDocVal, inDBVal);
            }

        }
    }
