using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.Excel.DataFormatting
    {
    /// <summary>
    /// Описывает генератор преобразователей, который создает преобразователь определенного типа.
    /// </summary>
    public interface IFormatterConstructor
        {
        /// <summary>
        /// Возвращает новый экземпляр преобразователя
        /// </summary>
        /// <param name="expression">Выражение</param>
        /// <param name="targetType">Тип данных объекта который должен возвращать преобразователь</param>
        /// <param name="formattersResolver">Делегат возвращающий вспомогательные преобразователи на основании ключей которые должна содержать карта преобоазования (ExcelMapper)</param>
        /// <returns>Преобразователь</returns>
        IDataFormatter Create( string expression, Type targetType, Func<string, IDataFormatter> formattersResolver );
        }
    }
