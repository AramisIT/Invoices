using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.Excel
    {
    /// <summary>
    /// Хранит описание того как стоит обрабатывать строку  Excel - таблицы. Содержит набор ключевых слов и соответствующих им выражений
    /// . При обработке строки каждое выражение формирует результат, который присваивается целевому объекту на основании ключевого поля.
    /// К примеру ключевым полем может быть колонка DataTable, а само выражение может содержать индекс колонки в Excel - таблице из которой для нее берутся данные.
    /// </summary>
    public class ExcelMapper : Dictionary<string, Expression>
        {
        //константы для базовых типов выражений
        public const string IndexKey = "index";
        public const string SumKey = "summ";
        /// <summary>
        /// Добавляет новое выражение
        /// </summary>
        /// <param name="propertyName">Ключевое значение</param>
        /// <param name="expressionType">Тип выражения (индексное, составное(сумма) ...)</param>
        /// <param name="expression">Тело выражения</param>
        /// <param name="defaultValue">Значение по умолчанию возвращаемое вместо null</param>
        /// <returns>Возвращает было ли добавлено выражение (если уже существует выражение для такого ключа - возвращает false)</returns>
        public bool TryAddExpression( string propertyName, string expressionType, string expression, object defaultValue = null )
            {
            Expression finalExpression = new Expression( expressionType, expression, defaultValue );
            if (!this.ContainsKey( propertyName ))
                {
                this.Add( propertyName, finalExpression );
                return true;
                }
            return false;
            }
        }
    }
