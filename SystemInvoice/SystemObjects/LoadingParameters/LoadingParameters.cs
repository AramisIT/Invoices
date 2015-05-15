using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;
using SystemInvoice.DataProcessing.Cache.NomenclaturesCache;
using SystemInvoice.Documents;
using Aramis.Core;
using Aramis.DataBase;
using Aramis.DatabaseConnector;
using Aramis.SystemConfigurations;
using Aramis.UI;
using Aramis.UI.WinFormsDevXpress;
using ReportView;
using TableViewInterfaces;

namespace SystemInvoice.SystemObjects
    {
    public abstract class LoadingParameters
        {
        public abstract IContractor Contractor { get; }

        public abstract int ModelIndex { get; }

        public abstract int ArticleIndex { get; }

        public abstract int DateIndex { get; }

        public abstract bool ProcessExcelRow(LoadingEuroluxBehaviour.ExcelRow row,
            ILoadingWareRow docRow,
            List<LoadingEuroluxBehaviour.ApprovalDocumentInfo> approvalDocuments);

        public StringBuilder Warnings { get; private set; }

        protected void addWarning(string message, LoadingEuroluxBehaviour.ExcelRow row, string comment = "")
            {
            Warnings.AppendLine(
                string.Format("{0} Страница - {1} № стр. - {2}; {3}",
                message.PadRight(40), row.Sheet.Name.PadRight(25), row.RowNumber, comment));
            }

        internal void Init()
            {
            Warnings = new StringBuilder();
            }

        public List<ICatalog> NewCatalogItems = new List<ICatalog>();
        public List<IDocument> NewDocumentItems = new List<IDocument>();

        public StringCacheDictionary ApprovalsCertTypes { get; set; }
        public StringCacheDictionary CustomsCodes { get; set; }
        public StringCacheDictionary ApprovalsTypes { get; set; }
        public StringCacheDictionary Countries { get; set; }
        public StringCacheDictionary UnitMeasures { get; set; }
        public Dictionary<string, IManufacturer> Producers { get; set; }
        public Dictionary<string, ITradeMark> TradeMarks { get; set; }
        public Dictionary<DateTime, Dictionary<string, List<Approvals>>> ApprovalsCache { get; set; }

        public Dictionary<string, List<Approvals>> DeclarationsCache { get; set; }
        protected StringCacheDictionary NomnclatureCache { get; set; }
        public Dictionary<string, Approvals> CertificatesCache { get; set; }
        public IDocumentType DeclarationType { get; set; }
        public IDocumentType CertificateType { get; set; }

        public DateTime DefaultStartDate = new DateTime(2010, 1, 1);


        public long GetId(object value, StringCacheDictionary cache, string fieldName, bool throwException = true)
            {
            if (value == null) return 0;

            var strValue = value.ToString().Trim();
            if (string.IsNullOrEmpty(strValue))
                {
                return 0;
                }

            long id;
            if (cache.TryGetValue(strValue, out id))
                {
                return id;
                }
            if (throwException)
                {
                throw new Exception(string.Format("Не удалось получить поле {0}", fieldName));
                }
            return 0;
            }

        public T GetItem<T>(string strValue, Dictionary<string, T> cache) where T : IDatabaseObject
            {
            if (!string.IsNullOrEmpty(strValue))
                {
                T item;
                if (cache.TryGetValue(strValue, out item))
                    {
                    return item;
                    }
                else
                    {
                    item = A.New<T>();
                    item.SetRef("Contractor", Contractor.Id);
                    if (item is ICatalog)
                        {
                        ((ICatalog)item).Description = strValue;
                        NewCatalogItems.Add((ICatalog)item);
                        }
                    else
                        {
                        NewDocumentItems.Add((IDocument)item);
                        }
                    cache.Add(strValue, item);
                    return item;
                    }
                }

            throw new Exception(string.Format("Не удалось получить поле {0}",
                    SystemConfiguration.DBConfigurationTree[typeof(T).GetTableName()].Description));
            }

        public abstract bool TryLoadApprovals(System.Data.DataSet dataSet, Action<double> notifyProgress,
            int approvalDurationYears,
            out string errorDescription, out string errorHelpData);

        internal abstract void InitForApprovals();
        }
    }
