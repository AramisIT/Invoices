using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.Excel;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.NomenclatureCheck
    {
    /// <summary>
    /// Базовый клас для проверки полей номенклатуры
    /// </summary>
    public abstract class NomenclatureChecker : LoadedDocumentCheckerBase
        {
        public NomenclatureChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            }

        protected override void CheckRow(DataRow rowToCheck, ExcelMapper mapper, bool isDocumentCurrentlyLoaded, string currentCheckedColumnName)
            {
            if (rowToCheck == null || !CheckThatHaveToProcess(mapper))
                {
                return;
                }
            OnCheckBegin(rowToCheck);
            long nomenclatureId = Helpers.InvoiceDataRetrieveHelper.GetRowNomenclatureId(rowToCheck);
            string expectededValue = rowToCheck.TrySafeGetColumnValue(ColumnToCheck, "").Trim();//значение в документе
            bool equalsAsExpected = true;
            NomenclatureCacheObject cacheObject = null;
            if (nomenclatureId != 0)
                {
                cacheObject = dbCache.NomenclatureCacheObjectsStore.GetCachedObject(nomenclatureId);
                if (cacheObject != null)
                    {
                    equalsAsExpected = this.CheckThatEquals(cacheObject, expectededValue);//проверяем равно ли значение загруженному в базе
                    }
                }
            if (!CheckExpectedValue(expectededValue, mapper) || !equalsAsExpected)//добавляем ошибку если проверка завершилась неудачей
                {
                CellError error = CreateError(expectededValue, cacheObject);
                if (error != null)
                    {
                    AddError(ColumnToCheck, error);
                    }
                }
            }
        /// <summary>
        /// Вызывается перед началом проверки
        /// </summary>
        /// <param name="rowToCheck"></param>
        protected virtual void OnCheckBegin(DataRow rowToCheck)
            {
            }
        /// <summary>
        /// Выполняет проверку самого значения загруженного из документа. К примеру может быть необходимо что бы это значение не было пустым
        /// </summary>
        /// <param name="expectedValue">Значение в документе</param>
        /// <param name="mapper">Маппер, по которому загружался документ</param>
        /// <returns>Результат проверки</returns>
        protected virtual bool CheckExpectedValue(string expectedValue, ExcelMapper mapper)
            {
            return !string.IsNullOrEmpty(expectedValue);
            }
        /// <summary>
        /// Вызывается для определения того нужна ли проверка вообеще. Например если некоторая колонка не была загружена, то ее проверять не нужно.
        /// </summary>
        /// <param name="mapper">Содержит набор загруженных колонок</param>
        protected virtual bool CheckThatHaveToProcess(ExcelMapper mapper)
            {
            return true;
            }
        /// <summary>
        /// Выполняет проверку значения загруженного из документа со значением в БД
        /// </summary>
        /// <param name="nomenclatureCacheObject">Кешированный объект по которому можно получить значение в БД</param>
        /// <param name="expectededValue">Значение в документе</param>
        /// <returns>Результат проверки</returns>
        protected abstract bool CheckThatEquals(NomenclatureCacheObject nomenclatureCacheObject, string expectededValue);
        /// <summary>
        /// Проверяемая колонка
        /// </summary>
        protected abstract string ColumnToCheck { get; }

        /// <summary>
        /// Создает екземпляр нужной ошибки при неуспешной проверке
        /// </summary>
        protected abstract CellError CreateError(string expectedValue, NomenclatureCacheObject nomenclatureCacheObject);
        }
    }
