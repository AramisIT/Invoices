using System.Collections.Generic;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing.Cache;
using Aramis.DatabaseConnector;

namespace SystemInvoice.DataProcessing.CatalogsProcessing.Loaders
    {
    /// <summary>
    /// Загружает типы документов
    /// </summary>
    public class DocumentTypesLoader : FromExcelToDataBaseObjectsLoaderBase<IDocumentType>
        {
        HashSet<string> documentTypesExisted = new HashSet<string>();

        public DocumentTypesLoader(SystemInvoiceDBCache cachedData)
            : base(cachedData)
            {
            }

        protected override bool CheckItemBegoreCreate(IDocumentType itemToCheck)
            {
            if (string.IsNullOrEmpty(itemToCheck.Description.Trim()) || string.IsNullOrEmpty(itemToCheck.QualifierCodeName.Trim()) ||
                documentTypesExisted.Contains(itemToCheck.QualifierCodeName.Trim()))
                {
                return false;
                }
            return true;
            }

        protected override void InitializeMapping(Excel.ExcelMapper mapper)
            {
            base.AddPropertyMapping("QualifierCodeName", 1);
            base.AddPropertyMapping("Description", 2);
            }

        protected override bool OnLoadBegin()
            {
            documentTypesExisted.Clear();
            string loadDocumentsTypesStr = "select QualifierCodeName as codeName from DocumentType;";
            DB.NewQuery(loadDocumentsTypesStr).Foreach((result) =>
            {
                string codeName = ((string)result["codeName"]).Trim();
                if (!documentTypesExisted.Contains(codeName))
                    {
                    documentTypesExisted.Add(codeName);
                    }
            });
            return true;
            }
        }
    }
