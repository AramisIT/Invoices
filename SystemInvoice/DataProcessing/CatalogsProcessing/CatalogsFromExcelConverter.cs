using SystemInvoice.DataProcessing.CatalogsProcessing.Loaders;
using SystemInvoice.DataProcessing.Cache;

namespace SystemInvoice.DataProcessing.CatalogsProcessing
    {
    /// <summary>
    /// Клас - фасад, инкапсулирующий логику загрузки екселевских файлов
    /// </summary>
    public class CatalogsFromExcelConverter
        {
        private SystemInvoiceDBCache cachedData = null;
        private CustomsCodeLoader customsCodeLoader = null;
        private CountryLoader countryLoader = null;
        private DocumentTypesLoader documentTypeLoader = null;
        private PropertyTypeLoader propertyTypeLoader = null;
        private NomenclatureLoader nomenclatureLoader = null;

        public CatalogsFromExcelConverter()
            {
            CacheTradeMarkContractorSource tmcSource = new CacheTradeMarkContractorSource();
            cachedData = new SystemInvoiceDBCache(tmcSource);
            customsCodeLoader = new CustomsCodeLoader(cachedData);
            countryLoader = new CountryLoader(cachedData);
            documentTypeLoader = new DocumentTypesLoader(cachedData);
            propertyTypeLoader = new PropertyTypeLoader(cachedData);
            nomenclatureLoader = new NomenclatureLoader(tmcSource, cachedData);
            }

        public void LoadCustomsCodes(string fileName)
            {
            customsCodeLoader.Load(fileName);
            }

        public void LoadCountries(string fileName)
            {
            countryLoader.Load(fileName);
            }

        public void LoadDocumentTypes(string fileName)
            {
            documentTypeLoader.Load(fileName);
            }

        public void LoadPropertyTypes(string fileName)
            {
            propertyTypeLoader.Load(fileName);
            }

        public void LoadNomenclatures(string fileName)
            {
            nomenclatureLoader.Load(fileName);
            }
        }
    }
