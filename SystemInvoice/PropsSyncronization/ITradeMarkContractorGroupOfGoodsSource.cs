using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;

namespace SystemInvoice.PropsSyncronization
    {
    public interface ITradeMarkContractorSubGroupOfGoodsSource:ITradeMarkContractorSource
        {
        SubGroupOfGoods SubGroupOfGoods { get; set; }

        string GroupOfGoods { get; set; }
        }
    }
