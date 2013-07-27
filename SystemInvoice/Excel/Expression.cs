using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.Excel
    {
    /// <summary>
    /// Хранит выражение, описывающее преобразование строки в Excel - таблице в нужное значение
    /// </summary>
    public struct Expression
        {
        /// <summary>
        /// Тип выражение (константа, колонка, сумма и т.д.)
        /// </summary>
        public string ExpressionType;
        /// <summary>
        /// Тело выражения
        /// </summary>
        public string ExpressionBody;
        /// <summary>
        /// Значение по умолчанию, которое должно возвращатся если результат выражение null
        /// </summary>
        public object DefaultValue;
        /// <summary>
        /// Создает новый экземпляр выражения
        /// </summary>
        /// <param name="ExpressionType">тип выражения</param>
        /// <param name="ExpressionBody">тело выражения</param>
        public Expression( string ExpressionType, string ExpressionBody,object DefultValue )
            {
            this.ExpressionType = ExpressionType;
            this.ExpressionBody = ExpressionBody;
            this.DefaultValue = DefultValue;
            }
        }
    }
