using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.Excel.DataFormatting
    {
    /// <summary>
    /// Класс - хранилище генераторов преобразователей. Который на основании содержащихся в нем генераторов (конструкторов) преобразователей 
    /// формирует преобразователи из выражений для соответствующих типов данных
    /// </summary>
    public class FormattersGenerator
        {
        /// <summary>
        /// Хранилище генераторов (конструкторов) преобразователей
        /// </summary>
        private Dictionary<string, IFormatterConstructor> constructors = new Dictionary<string, IFormatterConstructor>();

        /// <summary>
        /// регистрирует новый конструктор
        /// </summary>
        /// <param name="key">ключевое слово используемое для данного конструктора (и как следствие соответствующего типа преобразователя)</param>
        /// <param name="contructor">Экземпляр конструкторв</param>
        /// <param name="overwrite">Перезаписывать конструктор</param>
        /// <returns>Результат (если такое ключевое слово уже зарезервировано, добавление невозможно)</returns>
        public bool Register( string key, IFormatterConstructor contructor, bool overwrite )
            {
            if (constructors.ContainsKey( key ))
                {
                if (!overwrite)
                    {
                    return false;
                    }
                constructors.Remove( key );
                }
            constructors.Add( key, contructor );
            return true;
            }
        /// <summary>
        /// Создает новый преобразователь
        /// </summary>
        /// <param name="expression">Выражение</param>
        /// <param name="dataType">Тип данных</param>
        /// <param name="formattersResolver">Делегат возвращающий вспомогательные преобразователи (не обязателен для большинства конструкторов)</param>
        /// <returns>Преобразователь</returns>
        public IDataFormatter CreateFormatter( Expression expression, Type dataType, Func<string, IDataFormatter> formattersResolver )
            {
            if (!constructors.ContainsKey( expression.ExpressionType ))
                {
                return null;
                }
            IFormatterConstructor construrctor = constructors[expression.ExpressionType];
            return construrctor.Create( expression.ExpressionBody, dataType, formattersResolver );
            }
        }
    }
