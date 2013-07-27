using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInvoice.DataProcessing.ApprovalsProcessing.ByNomenclatureUpdating
    {
    /// <summary>
    /// Обработчик, вызываемый при удалении номенклатуры из разрешительных документов
    /// </summary>
    /// <param name="updates">Список изменений разрешительных</param>
    public delegate void ApprovalsUpdatedHandler(IEnumerable<ApprovalsUpdateResult> updates);
    }
