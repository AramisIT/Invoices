using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Documents;
using SystemInvoice.PropsSyncronization;

namespace SystemInvoice.DataProcessing.Cache
    {
    /// <summary>
    /// Прокси-объект для получения информации об используемых при построении кеша контрагенте и торговой марке
    /// </summary>
    public class InvoiceTrademarkContractorSource : ITradeMarkContractorSource
        {
        Invoice invoice = null;

        public InvoiceTrademarkContractorSource( Invoice invoice )
            {
            this.invoice = invoice;
            }

        public Catalogs.Contractor Contractor
            {
            get
                {
                return invoice.ExcelLoadingFormat.Contractor;
                }
            set
                {
                }
            }

        public Catalogs.TradeMark TradeMark
            {
            get
                {
                return invoice.ExcelLoadingFormat.TradeMark;
                }
            set
                {
                }
            }
        }
    }
