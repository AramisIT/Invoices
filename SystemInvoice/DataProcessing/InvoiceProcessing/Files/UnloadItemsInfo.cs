using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Files
    {
    /// <summary>
    /// Хранит информацию о соответствии строк выгруженного файла, строкам отображаемым в табличной части, с тем что бы обновлять нужные строки при
    /// загрузке отредактированного файла.
    /// </summary>
    public class UnloadItemsInfo
        {
        public readonly Dictionary<int, int> unloadRowsMappings = null;
        public readonly int TargetTableRowsCount;

        public int UnloadedRowsCount
            {
            get
                {
                return unloadRowsMappings == null ? 0 : unloadRowsMappings.Count;
                }
            }

        public UnloadItemsInfo( Dictionary<int, int> unloadRowsMappings, int targetTableRowsCount )
            {
            this.unloadRowsMappings = unloadRowsMappings;
            this.TargetTableRowsCount = targetTableRowsCount;
            }
        }
    }
