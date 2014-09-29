using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisWpfComponents.Excel;
using System.IO;
using System.Threading.Tasks;
using SystemInvoice.Excel.DataFormatting;
using SystemInvoice.Excel.DataFormatting.Formatters;

namespace SystemInvoice.Excel
    {
    /// <summary>
    /// Выполняет загрузку Excel - файлов, содержит объявления методов используемых для дальнейшего преобразования данных
    /// инкапсулирует логику преобразования данных из Excel - в типы данных используемые целевыми объектами, а также
    /// формирует вычисляемые поля на основании выражений привязки
    /// </summary>
    public abstract class AbstractLoader
        {
        /// <summary>
        /// Структура которая хранит ссылку на преобразователь данных и соответствующее ключевое слово (может описывать колонку в таблице или поле объекта)
        /// </summary>
        private struct FormattersStore { public string PropertyName; public object DefaultValue; public IDataFormatter Formatter;}
        /// <summary>
        /// Генератор преобразователей
        /// </summary>
        private FormattersGenerator formattersGenerator = new FormattersGenerator();
        /// <summary>
        /// Возвращает тип данных который должен вернуть преобразователь данных для заданного ключевого слова (в зависимости от реализации может описывать к примеру колонку таблицы или поле объекта)
        /// </summary>
        /// <param name="propertyName">значение ключевого слова</param>
        /// <returns>Тип данных</returns>
        protected abstract Type getFormatterType(string propertyName);
        /// <summary>
        /// Возвращает - нужно ли выполнять обработку для данного ключевого слова (колонки таблицы, свойства ....)
        /// </summary>
        /// <param name="propertyName">значение</param>
        /// <returns>Результат проверки</returns>
        protected abstract bool isPropertyExists(string propertyName);
        /// <summary>
        /// Выполняется перед началом вызовов OnPropertySet для каждй строки.
        /// </summary>
        protected abstract void OnRowProcessingBegin();
        /// <summary>
        /// Выполняется после всех вызовов OnPropertySet для каждй строки.
        /// </summary>
        protected abstract void OnRowProcessingComplete();
        /// <summary>
        /// Выполняет обработку каждого элемента ExcelMapper для каждой строки таблицы
        /// </summary>
        /// <param name="propertyName">Ключевое поле</param>
        /// <param name="value">Результат выполнения преобразователя данных</param>
        protected abstract void OnPropertySet(string propertyName, object value);

        /// <summary>
        /// Создает новый экземпляр класса, регистрирует базовые типы выражений
        /// </summary>
        public AbstractLoader()
            {
            RegisterFormatter("index", new SimpleIndexFormatterConstructor());
            RegisterFormatter("summ", new SumExpressionDataFormatterConstructor());
            }
        /// <summary>
        /// Выполняет загрузку Excel - файла, и дальнейшую его обработку (которая реализуется в производных классах).
        /// </summary>
        /// <param name="fileName">Путь к Excel - файлу</param>
        /// <param name="mapper">Экземпляр класса описывающий привязку колонок Excel - файла к ключевым словам (которые к примеру могут описывать колонки таблицы или поля объекта)</param>
        /// <param name="workSheetIndex">Номер страницы в Excel - файле</param>
        /// <param name="startRowIndex">Начальная строка с которой начинается обработка файла</param>
        /// <param name="finishRowIndex">конечная строка (-1 если все строки начиная с начальной)</param>
        protected bool TryLoad(string fileName, ExcelMapper mapper, int workSheetIndex, int startRowIndex, int finishRowIndex)
            {
            // P001
            List<FormattersStore> formatters = createColumnFormatters(mapper);
            ExcelXlsWorkbook book = null;
            if (!ExcelHelper.tryLoad(fileName, out book))
                {
                return false;
                }
            Worksheet sheet = book[workSheetIndex];
            int propertiesCount = formatters.Count;
            for (int i = startRowIndex; i < sheet.RowCount && (i < finishRowIndex || finishRowIndex == -1); i++)
                {
                OnRowProcessingBegin();
                foreach (FormattersStore formatter in formatters)
                    {
                    object formattedValue = formatter.Formatter.Format(sheet[i]) ?? formatter.DefaultValue;
                    OnPropertySet(formatter.PropertyName, formattedValue);
                    }
                OnRowProcessingComplete();
                }
            if (book != null)
                {
                book.Dispose();
                book = null;
                GC.Collect(3);
                }
            return true;
            }

        /// <summary>
        /// Регистрирует класс создающий преобразователь данных на основании выражения преобразования
        /// </summary>
        /// <param name="parameterKeyWord">ключ преобразователя</param>
        /// <param name="formatterConstructor">экземпляр генератора преобразователя</param>
        /// <param name="overwrite">Перезаписывать конструктор</param>
        public void RegisterFormatter(string parameterKeyWord, IFormatterConstructor formatterConstructor, bool overwrite = false)
            {
            formattersGenerator.Register(parameterKeyWord, formatterConstructor, overwrite);
            }

        /// <summary>
        /// Формирует список преобразователей с связанными с ними полями/ключевыми словами/колонками
        /// </summary>
        /// <param name="mapper">Экземпляр класса описывающего привязку полей/... и преобразователей данных</param>
        /// <returns>Список преобразователей</returns>
        private List<FormattersStore> createColumnFormatters(ExcelMapper mapper)
            {
            Resolver resolver = new Resolver(mapper, formattersGenerator, getFormatterType);
            List<FormattersStore> formatters = new List<FormattersStore>();
            foreach (KeyValuePair<string, Expression> pair in mapper)
                {
                IDataFormatter formatter = null;
                string propertyName = pair.Key;
                Expression expression = pair.Value;
                if (!isPropertyExists(propertyName) || (formatter = formattersGenerator.CreateFormatter(expression, getFormatterType(propertyName), resolver.ResolveFormatter)) == null)
                    {
                    continue;
                    }
                formatters.Add(new FormattersStore() { PropertyName = propertyName, DefaultValue = expression.DefaultValue, Formatter = formatter });
                }
            return formatters;
            }
        /// <summary>
        /// Используется для передачи конструктору форматтера функции генерирующей вспомогательные форматтеры по имени поля для которого они генерируются.
        /// Используется именно класс поскольку при использовании делегата через лямбда-выражение с замыканием, невозможно сделать его рекурсивный вызов,
        /// поскольку в теле выражения он еще не будет проинициализирован.
        /// </summary>
        private class Resolver
            {
            ExcelMapper mapper;
            FormattersGenerator formattersGenerator;
            Func<string, Type> getFormatterTypeFunc = null;

            public Resolver(ExcelMapper mapper, FormattersGenerator formattersGenerator, Func<string, Type> getFormatterTypeFunc)
                {
                this.mapper = mapper;
                this.formattersGenerator = formattersGenerator;
                this.getFormatterTypeFunc = getFormatterTypeFunc;
                }

            public IDataFormatter ResolveFormatter(string propertyName)
                {
                if (!mapper.ContainsKey(propertyName))
                    {
                    return null;
                    }
                Expression expression = mapper[propertyName];
                return formattersGenerator.CreateFormatter(expression, getFormatterTypeFunc(propertyName), ResolveFormatter);
                }
            }
        }

    }
