using System;
using System.Collections.Generic;
using SystemInvoice.DataProcessing.Cache;
using System.Data;
using SystemInvoice.DataProcessing.Cache.ApprovalsCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.ApprovalsModification
    {
    /// <summary>
    /// Создает разрешительные документы из загруженного после ручной обработки екселевского документа инвойс. Если документ существует но в нем нету соответствующей
    /// номенклатуры - добавляет номенклатуру в табличную часть разрешительного докмента.
    /// </summary>
    public class ApprovalsFromInvoiceCreator
        {
        ApprovalsCacheObjectsStore approvalsCacheObjectStore = null;
        private SystemInvoiceDBCache dbCache = null;

        private List<string> RDDocTypeCodeColumnNames = new List<string>();
        private List<string> RDDateFromColumnNames = new List<string>();
        private List<string> RDDateToColumnNames = new List<string>();
        private List<string> RDDocNumberColumnNames = new List<string>();

        public ApprovalsFromInvoiceCreator(SystemInvoiceDBCache dbCache)
            {
            this.dbCache = dbCache;
            approvalsCacheObjectStore = dbCache.ApprovalsCacheObjectsStore;
            initColumnNames();
            }

        private void initColumnNames()
            {
            RDDocTypeCodeColumnNames.Clear();
            RDDateFromColumnNames.Clear();
            RDDateToColumnNames.Clear();
            RDDocTypeCodeColumnNames.Add(Documents.InvoiceColumnNames.RDCode1.ToString());
            RDDocTypeCodeColumnNames.Add(Documents.InvoiceColumnNames.RDCode2.ToString());
            RDDocTypeCodeColumnNames.Add(Documents.InvoiceColumnNames.RDCode3.ToString());
            RDDocTypeCodeColumnNames.Add(Documents.InvoiceColumnNames.RDCode4.ToString());
            RDDocTypeCodeColumnNames.Add(Documents.InvoiceColumnNames.RDCode5.ToString());
            RDDateFromColumnNames.Add(Documents.InvoiceColumnNames.RDFromDate1.ToString());
            RDDateFromColumnNames.Add(Documents.InvoiceColumnNames.RDFromDate2.ToString());
            RDDateFromColumnNames.Add(Documents.InvoiceColumnNames.RDFromDate3.ToString());
            RDDateFromColumnNames.Add(Documents.InvoiceColumnNames.RDFromDate4.ToString());
            RDDateFromColumnNames.Add(Documents.InvoiceColumnNames.RDFromDate5.ToString());
            RDDateToColumnNames.Add(Documents.InvoiceColumnNames.RDToDate1.ToString());
            RDDateToColumnNames.Add(Documents.InvoiceColumnNames.RDToDate2.ToString());
            RDDateToColumnNames.Add(Documents.InvoiceColumnNames.RDToDate3.ToString());
            RDDateToColumnNames.Add(Documents.InvoiceColumnNames.RDToDate4.ToString());
            RDDateToColumnNames.Add(Documents.InvoiceColumnNames.RDToDate5.ToString());
            RDDocNumberColumnNames.Add(Documents.InvoiceColumnNames.RDNumber1.ToString());
            RDDocNumberColumnNames.Add(Documents.InvoiceColumnNames.RDNumber2.ToString());
            RDDocNumberColumnNames.Add(Documents.InvoiceColumnNames.RDNumber3.ToString());
            RDDocNumberColumnNames.Add(Documents.InvoiceColumnNames.RDNumber4.ToString());
            RDDocNumberColumnNames.Add(Documents.InvoiceColumnNames.RDNumber5.ToString());
            }

        public void ModifyApprovalsCatalog(DataTable tableToProcess)
            {
            HashSet<DateTime> invoiceDates = getInvoiceDates(tableToProcess);
            //Вместо DateTime.MinValue можно подставить дату которая будет определять конечную дату по для РД-ов которые мы проверяем
            //Документы у которых дата по меньше (кроме тех у которых она не указанна - там по умолчанию 0001.01.01) не загружаются для проверки
            //это поможет избежать "тормозов" когда документов станет много.
            approvalsCacheObjectStore.Refresh(0, dbCache.TradeMarkContractorSource.Contractor.Id, invoiceDates);
            //выбираем новые записи с разрешительными для номенклатуры
            HashSet<ApprovalsCacheObject> newApprovalsRowsInDocument = getNewApprovalsRows(tableToProcess);
            //создаем новые разрещительные/обновляем существующие
            createOrUpdateApprovals(newApprovalsRowsInDocument);
            }

        /// <summary>
        /// Возвращает уникальные даты инвойса которые встречаются в таблице
        /// </summary>
        private HashSet<DateTime> getInvoiceDates(DataTable tableToProcess)
            {
            HashSet<DateTime> dates = new HashSet<DateTime>();
            foreach (DataRow row in tableToProcess.Rows)
                {
                var dt = getInvoiceDate(row);
                dates.Add(dt);
                }
            return dates;
            }

        private DateTime getInvoiceDate(DataRow row)
            {
            string invoiceDateStr = row.TrySafeGetColumnValue<string>(ProcessingConsts.ColumnNames.INVOICE_DATE_COLUMN_NAME,
                                                                      string.Empty);
            DateTime dt;
            if (!DateTime.TryParse(invoiceDateStr, out dt))
                {
                dt = DateTime.MinValue;
                }
            return dt;
            }


        private void createOrUpdateApprovals(HashSet<ApprovalsCacheObject> newApprovalsRowsInDocument)
            {
            dbCache.ApprovalsObjectCreator.TryUpdateApprovalsCatalog(newApprovalsRowsInDocument);
            }

        private HashSet<ApprovalsCacheObject> getNewApprovalsRows(DataTable tableToProcess)
            {
            HashSet<ApprovalsCacheObject> newApprovalsInDocument = new HashSet<ApprovalsCacheObject>();
            foreach (DataRow dataRow in tableToProcess.Rows)
                {
                //получаем все разрешительные для строки
                List<ApprovalsCacheObject> approvalsInfo = getApprovalsInfo(dataRow);
                if (approvalsInfo != null)
                    {
                    foreach (ApprovalsCacheObject approvalsRow in approvalsInfo)
                        {
                        //проверяем есть ли они уже в базе если нет - добавляем в наш список
                        if (approvalsRow != null && !isApprovalsExists(approvalsRow))
                            {
                            newApprovalsInDocument.Add(approvalsRow);
                            }
                        }
                    }
                }
            return newApprovalsInDocument;
            }

        private bool isApprovalsExists(ApprovalsCacheObject approvalsInfo)
            {
            return approvalsCacheObjectStore.ContainsApprovals(approvalsInfo);
            }

        private List<ApprovalsCacheObject> getApprovalsInfo(DataRow dataRow)
            {
            List<ApprovalsCacheObject> approvalsList = new List<ApprovalsCacheObject>();
            //собираем инфу о номенклатуре
            long nomenclatureId = Helpers.InvoiceDataRetrieveHelper.GetRowNomenclatureId(dataRow); ///this.getNomenclatureId(dataRow);
            if (nomenclatureId == 0)
                {
                return null;
                }
            long tradeMarkId = this.getTradeMark(dataRow);
            long contractorId = dbCache.TradeMarkContractorSource.Contractor.Id;
            DateTime invoiceDate = getInvoiceDate(dataRow);
            //выбираем разрешительные
            for (int i = 0; i < ProcessingConsts.CHECKING_APPROVALS_COUNT; i++)
                {
                string documentNumber = this.getNumber(dataRow, i);
                long documentTypeId = this.getDocumentType(dataRow, i);
                DateTime from = this.getFromDate(dataRow, i);
                DateTime to = this.getToDate(dataRow, i);
                string codeName = this.getDocumentCodeName(dataRow, i);
                if ((!string.IsNullOrEmpty(codeName) || !string.IsNullOrEmpty(documentNumber)) && (invoiceDate >= from && (invoiceDate <= to) || to == DateTime.MinValue))
                    {
                    ApprovalsCacheObject approvalsRow = new ApprovalsCacheObject(documentNumber, codeName, documentTypeId, contractorId, tradeMarkId, from, to, invoiceDate, nomenclatureId);
                    approvalsList.Add(approvalsRow);
                    }
                }
            return approvalsList;
            }

        /// <summary>
        /// Возвращает номер разрешительного для порядкового номера РД в табличной части
        /// </summary>
        private string getNumber(DataRow dataRow, int index)
            {
            List<string> columnNames = RDDocNumberColumnNames;
            if (index >= columnNames.Count)
                {
                return string.Empty;
                }
            string numberColumnName = columnNames[index];
            return dataRow.TrySafeGetColumnValue<string>(numberColumnName, string.Empty).Trim();
            }

        /// <summary>
        /// Возвращает дату по разрешительного для порядкового номера РД в табличной части
        /// </summary>
        private DateTime getToDate(DataRow dataRow, int index)
            {
            List<string> columnNames = RDDateToColumnNames;
            return getDateTimeFromRow(dataRow, index, columnNames);
            }

        /// <summary>
        /// Возвращает дату с разрешительного для порядкового номера РД в табличной части
        /// </summary>
        private DateTime getFromDate(DataRow dataRow, int index)
            {
            List<string> columnNames = RDDateFromColumnNames;
            return getDateTimeFromRow(dataRow, index, columnNames);
            }

        /// <summary>
        /// Возвращает дату для ячейки в табличной части инвойса
        /// </summary>
        /// <param name="dataRow">Строка</param>
        /// <param name="index">Индекс который указывает на колонку которую нужно взять для ячейки</param>
        /// <param name="columnNames">Список кколонок из которого нужно брать колонку для ячейки</param>
        private DateTime getDateTimeFromRow(DataRow dataRow, int index, List<string> columnNames)
            {
            DateTime result = DateTime.MinValue;
            if (index >= columnNames.Count)
                {
                return result;
                }
            string dateColumnName = columnNames[index];
            string dateTimeStr = dataRow.TrySafeGetColumnValue<string>(dateColumnName, string.Empty);
            DateTime.TryParse(dateTimeStr, out result);
            return result;
            }

        /// <summary>
        /// Возвращает айдишник типа документа для порядкового номера РД в табличной части
        /// </summary>
        private long getDocumentType(DataRow dataRow, int index)
            {
            string docType = getDocumentCodeName(dataRow, index);
            if (string.IsNullOrEmpty(docType))
                {
                return 0;
                }
            return dbCache.DocumentTypesCacheObjectsStore.GetDocumentType(docType);
            }

        /// <summary>
        /// Возвращает код документа для порядкового номера РД в табличной части
        /// </summary>
        private string getDocumentCodeName(DataRow dataRow, int index)
            {
            if (index >= RDDocTypeCodeColumnNames.Count)
                {
                return string.Empty;
                }
            string documentTypeCodeColumnName = RDDocTypeCodeColumnNames[index];
            string docType = dataRow.TrySafeGetColumnValue<string>(documentTypeCodeColumnName, string.Empty).Trim();
            return docType;
            }

        private long getTradeMark(DataRow dataRow)
            {
            string tradeMarkName = dataRow.TrySafeGetColumnValue<string>(Documents.InvoiceColumnNames.ItemTradeMark.ToString(), string.Empty);
            return dbCache.TradeMarkCacheObjectsStore.GetTradeMarkIdOrCurrent(tradeMarkName);
            }
        }
    }
