using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache.TradeMarksCache
    {
    /// <summary>
    ///  Интерфейс для установки поля используемого в поиске торговой марки
    /// </summary>
    interface ITradeMarkSearch
        {
        void SetSearchOptions( string tradeMarkName, long contractorId );
        }
    }
