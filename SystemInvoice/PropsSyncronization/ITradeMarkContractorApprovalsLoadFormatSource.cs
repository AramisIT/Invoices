using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;

namespace SystemInvoice.PropsSyncronization
    {
    interface ITradeMarkContractorApprovalsLoadFormatSource : ITradeMarkContractorSource
        {
        ApprovalsLoadFormat ApprovalsLoadFormat { get; set; }
        }
    }
