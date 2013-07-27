using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisWpfComponents.Excel;

namespace SystemInvoice.Excel.DataFormatting
    {
    /// <summary>
    /// Описывает преобразователь данных, который осуществляет непосредственное преобразование строки Excel - файла в результирующий объект 
    /// </summary>
    public interface IDataFormatter
        {
        /// <summary>
        /// Формирует результирующий объект из строки Excel - таблицы
        /// </summary>
        /// <param name="row">Строка таблицы</param>
        /// <returns>Результат преобразования</returns>
        object Format( Row row );
        }
    }
