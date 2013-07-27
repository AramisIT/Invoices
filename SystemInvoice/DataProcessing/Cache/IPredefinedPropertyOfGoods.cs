using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.Cache
    {
    /// <summary>
    /// Интерфейс предоставляющий доступ к элементам справочника виды свойств
    /// </summary>
    public interface IPredefinedPropertyOfGoods
        {
        void Refresh();
        long GenderPropertyTypeID { get; }
        long SizePropertyTypeID { get; }
        long CorrespondenceTypeId { get; }
        long NamePropertyTypeId { get; }
        }
    }
