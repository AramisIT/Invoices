using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache
    {
    /// <summary>
    /// Базовый клас для кешированного объекта, определяет переопределяемый механизм сравнения объектов на равенство
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CacheObject<T> where T : class
        {
        private bool hashInitiated = false;
        /// <summary>
        /// Вычисляемое значение хэша, которое хранится в объекте после первого вычисления, предполагается что все поля объекта
        /// остаются неизменяемыми в процессе жизни объекта, поэтому для объектов в которых эти поля могут менятся (например объекты для поиска в кеше),
        /// необходимо обновлять значение хэша при изменении полей или переопределять сам механизм сравнения объектов
        /// </summary>
        private int hash = 0;

        public override bool Equals(object obj)
            {
            T other = obj as T;
            if (other != null)
                {
                return equals(other);
                }
            return false;
            }

        public override int GetHashCode()
            {
            if (!hashInitiated)
                {
                refreshHash();
                hashInitiated = true;
                }
            return hash;
            }
        /// <summary>
        /// Обновляет значение хэша. Необходимо явно вызывать при изменении значений полей/свойств объекта участвующих в сравнении объектов между собой
        /// </summary>
        protected void refreshHash()
            {
            hash = calcHash();
            }
        /// <summary>
        /// Расчитывает значение хэша, по умолчанию использует для расчета побитовое отрецательное или от значений хэша полученых из getForCacheCalculatedObjects
        /// </summary>
        /// <returns>Хэш</returns>
        protected virtual int calcHash()
            {
            int hash = 0;
            foreach (object item in getForCacheCalculatedObjects())
                {
                if (item != null)
                    {
                    hash ^= item.GetHashCode();
                    }
                }
            return hash;
            }
        /// <summary>
        /// Сравнивает текущий объект с другим объектом такого же типа
        /// </summary>
        /// <param name="other">Объект для сравнения</param>
        /// <returns>Результат сравнения</returns>
        protected abstract bool equals(T other);
        /// <summary>
        /// Возвращает список значений, по которым осуществляется сравнение объекта с другими объектами и расчет хэша
        /// </summary>
        /// <returns></returns>
        protected abstract object[] getForCacheCalculatedObjects();
        }
    }
