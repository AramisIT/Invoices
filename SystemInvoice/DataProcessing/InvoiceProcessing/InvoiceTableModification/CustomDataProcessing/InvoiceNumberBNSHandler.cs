using System;
using System.Collections.Generic;
using System.Data;
using SystemInvoice.Documents;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.CustomDataProcessing
    {
    /// <summary>
    /// Генерирует номер инвойса на основании номера поставки. Генерирование происходит если в формате загрузки присутствует колонка, в которую записывается номер поставки
    /// из входящего файла.
    /// </summary>
    class InvoiceNumberBNSHandler
        {

        public void CreateInvoiceNumbersIfNeed(DataTable tableToProcess)
            {
            string bnsColumnName = InvoiceColumnNames.BNSInvoicePart.ToString();
            string invoiceNumberColumnName = InvoiceColumnNames.InvoiceNumber.ToString();
            if (tableToProcess.Columns.Contains(bnsColumnName))
                {
                string currentInvoiceNumber = "";
                //получаем диапазон для номера поставки
                Dictionary<string, Tuple<int, int>> diapasons = getDiapasons(tableToProcess, bnsColumnName);
                Dictionary<string, string> invoiceNumbers = new Dictionary<string, string>();
                foreach (DataRow row in tableToProcess.Rows)//формируем и записываем номер инвойса для каждой строки
                    {
                    string invoiceProcessPart = row[bnsColumnName].ToString().Trim();
                    string[] parts = invoiceProcessPart.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 2)
                        {
                        continue;
                        }
                    string key = parts[0];
                    if (!invoiceNumbers.ContainsKey(key))
                        {
                        string newInvoiceNumberPart = this.getFromDiapasons(diapasons, key);
                        if (string.IsNullOrEmpty(currentInvoiceNumber))
                            {
                            currentInvoiceNumber = newInvoiceNumberPart;
                            }
                        else
                            {
                            currentInvoiceNumber += string.Concat("/", newInvoiceNumberPart);
                            }
                        if (currentInvoiceNumber.Length > Documents.Invoice.MAX_INVOICE_NUMBER_SIZE)
                            {
                            currentInvoiceNumber = newInvoiceNumberPart;
                            }
                        invoiceNumbers.Add(key, currentInvoiceNumber);
                        }
                    row[invoiceNumberColumnName] = invoiceNumbers[key];//currentInvoiceNumber;
                    }
                }
            }

        private string getFromDiapasons(Dictionary<string, Tuple<int, int>> diapasons, string key)
            {
            if (!diapasons.ContainsKey(key))
                {
                return string.Empty;
                }
            Tuple<int, int> diapason = diapasons[key];
            if (diapason.Item1 != diapason.Item2)
                {
                return string.Format("{0}-{1}-{2}", key, diapason.Item1, diapason.Item2);
                }
            return string.Format("{0}-{1}", key, diapason.Item1);
            }

        //Получает диапазон значений для номера поставки
        private static Dictionary<string, Tuple<int, int>> getDiapasons(DataTable tableToProcess, string bnsColumnName)
            {
            Dictionary<string, Tuple<int, int>> range = new Dictionary<string, Tuple<int, int>>();
            foreach (DataRow row in tableToProcess.Rows)
                {
                string invoiceProcessPart = row[bnsColumnName].ToString().Trim();
                string[] parts = invoiceProcessPart.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                    {
                    continue;
                    }
                string key = parts[0];
                string numberStr = parts[1];
                int number;
                if (!int.TryParse(numberStr, out number))
                    {
                    continue;
                    }
                if (range.ContainsKey(key))
                    {
                    Tuple<int, int> diapason = range[key];
                    if (diapason.Item1 > number)
                        {
                        range[key] = new Tuple<int, int>(number, diapason.Item2);
                        }
                    if (diapason.Item2 < number)
                        {
                        range[key] = new Tuple<int, int>(diapason.Item1, number);
                        }
                    }
                else
                    {
                    range.Add(key, new Tuple<int, int>(number, number));
                    }
                }
            return range;
            }
        }
    }
