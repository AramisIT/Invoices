using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.NomenclaturesCache
    {
    /// <summary>
    /// Интерфейс для установки поля используемого в поиске, в объекте используемого для поиска. Explicity - реализация интерфейса скрывает его от пользователей кешированных объектов
    /// NomenclatureCacheObject, не позволяя им изменять его поля, что нарушило бы целостность данных в кеше
    /// </summary>
    interface INomenclatureSearch
        {
        void SetSearchOptions( string Article, long TradeMarkId, long ContractorId );
        }
    }
