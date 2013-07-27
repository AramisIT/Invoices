using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.Catalogs;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.CustomsCodeInternal
    {
    public class CustomsCodeInternCheckError : CompareWithDBCellError
        {
        public CustomsCodeInternCheckError(string inDocumentValue, string inDBValue, string columnName)
            : base(inDocumentValue, inDBValue, columnName)
            {
            }

        public CustomsCodeInternCheckError()
            : base()
            {
            }

        public override void SetCurrentDBValueAsInCell(DataRow row, SystemInvoiceDBCache dbCache)
            {
            long inDocumentCustomsCode = dbCache.CustomsCodesCacheStore.GetCustomsCodeIdForCodeName(InDocumentValue);
            Nomenclature nomenclature = readNomenclature(row);
            if (nomenclature == null)
                {
                return;
                }
            if (inDocumentCustomsCode == 0)//попытка записать в базу несуществующее значение
                {
                string askMessage = string.Format("Код {0} отсутствует в справочнике таможенных кодов. Продолжить?", InDocumentValue);
                if (!askMessage.Ask())
                    {
                    throw new CannotWriteToDBException("Завершено с ошибкой.");
                    }
                return;
                }
            CustomsCode newCustomsCode = new CustomsCode();
            newCustomsCode.Id = inDocumentCustomsCode;
            nomenclature.CustomsCodeInternal = newCustomsCode;
            nomenclature.Write();
            }
        }
    }
