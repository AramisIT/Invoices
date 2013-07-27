using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.CustomCodesCache
    {
    /// <summary>
    /// Интерфейс для установки поля используемого в поиске, в объекте используемого для поиска. Explicity - реализация интерфейса скрывает его от пользователей кешированных объектов
    /// CustomsCodesCacheObject, не позволяя им изменять его поля, что нарушило бы целостность данных в кеше
    /// </summary>
    interface ICustomsCodeSearch
        {
        void SetSearchOptions( string codeName );
        }
    }
