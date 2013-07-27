using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.InvoiceProcessing.Syncronizers;
using SystemInvoice.Documents;

namespace SystemInvoice.DataProcessing.InvoiceProcessing
    {
    /// <summary>
    /// Осуществляет синхронизацию ячеек в строках табличной части инвойса при их изменении
    /// </summary>
    public class SyncronizationManager
        {
        List<ISyncronizer> syncronizers = new List<ISyncronizer>();
        private bool syncronizationDenied = false;
        private bool isInChangeMode = false;
        private Invoice invoice = null;
        public event Action<DataRow, string> OnSyncronized = null;

        private void raiseSyncronized(DataRow dataRow, string columnName)
            {
            if (OnSyncronized != null && !string.IsNullOrEmpty(columnName))
                {
                OnSyncronized(dataRow, columnName);
                }
            }

        public SyncronizationManager(Invoice invoice)
            {
            this.invoice = invoice;
            this.invoice.Goods.ColumnChanged += Goods_ColumnChanged;
            syncronizers.Add(new SummSyncronizer());
            syncronizers.Add(new WeightSyncronizer());
            }

        /// <summary>
        /// Включает синхронизацию которая осуществляется при изменении данных в таблице
        /// </summary>
        public void AllowSyncronization()
            {
            if (syncronizationDenied)
                {
                syncronizationDenied = false;
                this.invoice.Goods.ColumnChanged += Goods_ColumnChanged;
                }
            }

        /// <summary>
        /// Выключает синхронизацию которая осуществляется при изменении данных в таблице
        /// </summary>
        public void DenySyncronization()
            {
            if (!syncronizationDenied)
                {
                syncronizationDenied = true;
                this.invoice.Goods.ColumnChanged -= Goods_ColumnChanged;
                }
            }

        //public void NotifySyncronization(DataRow rowToSyncronize, string columnName)
        //    {
        //    syncronize(rowToSyncronize, columnName, RequestForSyncronizationSource.NotificationSource);
        //    }

        void Goods_ColumnChanged(object sender, System.Data.DataColumnChangeEventArgs e)
            {
            DataRow dataRow = e.Row;
            string columnName = e.Column.ColumnName;
            syncronize(dataRow, columnName);
            this.raiseSyncronized(dataRow, columnName);
            }

        /// <summary>
        /// Осуществляет синхронизацию инициированною изменением данных
        /// </summary>
        private void syncronize(DataRow dataRow, string columnName)
            {
            if (isInChangeMode)
                {
                return;
                }
            try
                {
                isInChangeMode = true;
                this.executeSyncronizedChanges(dataRow, columnName);
                }
            finally
                {
                isInChangeMode = false;
                }
            }

        private void executeSyncronizedChanges(System.Data.DataRow dataRow, string columnName)
            {
            foreach (ISyncronizer syncronizer in syncronizers)
                {
                if (syncronizer.NeedSyncronization(columnName))
                    {
                    syncronizer.Syncronize(dataRow, columnName, RequestForSyncronizationSource.DataCellChangedSource);
                    }
                }
            }

        /// <summary>
        /// Выполняет полную синхронизацию всех строк/колонок между собой
        /// </summary>
        public void RefreshAll()
            {
            bool isSyncronizationInitiallyDenied = syncronizationDenied;
            try
                {
                if (!isSyncronizationInitiallyDenied)
                    {
                    DenySyncronization();
                    }
                isInChangeMode = true;
                foreach (DataRow row in invoice.Goods.Rows)
                    {
                    this.refreshDefault(row);
                    }
                }
            finally
                {
                isInChangeMode = false;
                if (!isSyncronizationInitiallyDenied)
                    {
                    AllowSyncronization();
                    }
                }
            }

        private void refreshDefault(DataRow row)
            {
            RequestForSyncronizationSource source = RequestForSyncronizationSource.NotificationSource;
            foreach (ISyncronizer syncronizer in syncronizers)
                {
                syncronizer.Syncronize(row, string.Empty, source);
                }
            }
        }

    /// <summary>
    /// Тип источника кототрый вызвал синхронизацию
    /// </summary>
    public enum RequestForSyncronizationSource
        {
        /// <summary>
        /// Источником синхронизации - является уведомление о синхронизации
        /// </summary>
        NotificationSource,
        /// <summary>
        /// Источником синхронизации является обновление данных
        /// </summary>
        DataCellChangedSource
        };
    }
