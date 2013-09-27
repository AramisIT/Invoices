using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking
    {
    /// <summary>
    /// Коллекция ошибок
    /// </summary>
    public class CellErrorsCollection : List<CellError>
        {
        /// <summary>
        /// Возвращает количество "информационных" ошибок, которые не свидетельствуют о несоответствии данных загруженных из БД и данных из документа,
        /// а свидетельствуют о некоректности самих данных из документа
        /// </summary>
        public int NonNotificationErrorsCount
            {
            get
                {
                int nonNotificationCount = 0;
                foreach (CellError error in this)
                    {
                    NotificationError notificationError = error as NotificationError;
                    if (notificationError == null)
                        {
                        nonNotificationCount++;
                        }
                    }
                return nonNotificationCount;
                }
            }

        /// <summary>
        /// Возвращает суммированный текст всех информационных ошибок
        /// </summary>
        public string CustomErrorsMessage
            {
            get
                {
                StringBuilder notificationErrorsBuilder = new StringBuilder();
                foreach (CellError error in this)
                    {
                    NotificationError notificationError = error as NotificationError;
                    if (notificationError != null)
                        {
                        notificationErrorsBuilder.Append(notificationError.NotificationMessage);
                        notificationErrorsBuilder.Append("  ");
                        }
                    }
                return notificationErrorsBuilder.ToString().Trim();
                }
            }
        }
    }
