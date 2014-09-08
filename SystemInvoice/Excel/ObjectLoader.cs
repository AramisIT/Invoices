using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using Aramis.Core;

namespace SystemInvoice.Excel
    {
    /// <summary>
    /// Загружает Ехеl в список объектов
    /// </summary>
    /// <typeparam name="T">Тип объекта в экземпляры которого загружаются результаты обработки строк Ехеl - таблицы</typeparam>
    public class ObjectLoader<T> : AbstractLoader where T : IDatabaseObject
        {
        /// <summary>
        /// Текущая коллекция в которую добавляются новые объекты в процессе обработки
        /// </summary>
        List<T> currentItemsCollection = null;
        /// <summary>
        /// Текущий экземпляр объекта свойствам которого присваиваются результаты обработки Ехеl - таблицы
        /// </summary>
        T currentItem = default(T);
        /// <summary>
        /// Словарь имя поля объекта - дескриптор свойства, используемый для присваивания свойствам объекта значения
        /// </summary>
        Dictionary<string, PropertyDescriptor> propertyDescriptors = new Dictionary<string, PropertyDescriptor>();

        /// <summary>
        /// Создает новый экземпляр класса, формирует дескрипторы свойств для типа в который загружаются результаты обработки
        /// </summary>
        public ObjectLoader()
            {
            fillDescriptors();
            }

        /// <summary>
        /// Формирует список объектов из Ехеl - таблицы на основании карты привязки свойств
        /// </summary>
        /// <param name="mapper">Набор выражений, соответствующий свойствам объекта</param>
        /// <param name="fileName">Путь к Ехеl - файлу</param>
        /// <param name="startIndex">Начальный индекс обрабатываемой строки</param>
        /// <param name="successed">Была ли операция выполнена без ошибок</param>
        /// <returns>Список сгенерированных объектов</returns>
        public List<T> Transform(ExcelMapper mapper, string fileName, int startIndex, out bool successed)
            {
            currentItemsCollection = new List<T>();
            successed = TryFill(currentItemsCollection, mapper, fileName, startIndex);
            return currentItemsCollection;
            }

        /// <summary>
        /// Формирует список объектов из Ехеl - таблицы на основании карты привязки свойств
        /// </summary>
        /// <param name="mapper">Набор выражений, соответствующий свойствам объекта</param>
        /// <param name="fileName">Путь к Ехеl - файлу</param>
        /// <param name="startIndex">Начальный индекс обрабатываемой строки</param>
        /// <returns>Список сгенерированных объектов</returns>
        public List<T> Transform(ExcelMapper mapper, string fileName, int startIndex)
            {
            bool successed = false;
            return Transform(mapper, fileName, startIndex, out successed);
            }

        /// <summary>
        /// Заполняет существующую коллекцию обектов новыми элементами сгенерированными в процессе обработки Ехеl - таблицы
        /// </summary>
        /// <param name="collectionToFill">Коллекция объектов</param>
        /// <param name="mapper">Набор выражений, соответствующий свойствам объекта</param>
        /// <param name="fileName">Путь к Ехеl - файлу</param>
        /// <param name="startIndex">Начальный индекс обрабатываемой строки</param>
        public bool TryFill(List<T> collectionToFill, ExcelMapper mapper, string fileName, int startIndex)
            {
            if (collectionToFill == null || mapper == null || string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                {
                return false;
                }
            currentItemsCollection = collectionToFill;
            return TryLoad(fileName, mapper, 0, startIndex, -1);
            }

        private void fillDescriptors()
            {
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(typeof(T)))
                {
                propertyDescriptors.Add(propertyDescriptor.Name, propertyDescriptor);
                }
            }

        protected override Type getFormatterType(string propertyName)
            {
            if (isPropertyExists(propertyName))
                {
                return propertyDescriptors[propertyName].PropertyType;
                }
            return null;
            }

        protected override bool isPropertyExists(string propertyName)
            {
            return propertyDescriptors.ContainsKey(propertyName);
            }

        protected override void OnRowProcessingBegin()
            {
            currentItem = A.New<T>();
            }

        protected override void OnRowProcessingComplete()
            {
            currentItemsCollection.Add(currentItem);
            }

        protected override void OnPropertySet(string propertyName, object value)
            {
            PropertyDescriptor descriptor = propertyDescriptors[propertyName];
            descriptor.SetValue(currentItem, value);
            }
        }
    }
