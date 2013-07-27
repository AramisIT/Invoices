using System;
using System.Data;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Syncronizers
    {
    /// <summary>
    /// Интерфейс описывающий синхронизацию разных ячеек строк групп строк между собой
    /// </summary>
    public interface ISyncronizer
        {
        /// <summary>
        /// Проверяет нуждается ли колонка в синхронизации
        /// </summary>
        /// <param name="columnName">Имя колонки в таблице</param>
        bool NeedSyncronization(string columnName);
        /// <summary>
        /// Осуществляет синхронизацию инициированную определенной строкой/колонкой
        /// </summary>
        /// <param name="row">Строка инициировавшая синхронизацию</param>
        /// <param name="columnName">Колонка инициировавшая синхронизацию</param>
        /// <param name="source">Источник синхронизации</param>
        void Syncronize(DataRow row, string columnName, RequestForSyncronizationSource source);
        }
    }
