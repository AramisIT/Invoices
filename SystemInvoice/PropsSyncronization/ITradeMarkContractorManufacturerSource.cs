using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;

namespace SystemInvoice.PropsSyncronization
    {
    public interface ITradeMarkContractorManufacturerSource : ITradeMarkContractorSource
        {
        IManufacturer Manufacturer { get; set; }
        }
    }
