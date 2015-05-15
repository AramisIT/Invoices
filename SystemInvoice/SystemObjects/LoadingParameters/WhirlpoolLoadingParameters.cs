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
    class WhirlpoolLoadingParameters : LoadingParameters
        {
        private static IContractor _WHIRLPOOL_CONTRACTOR;

        private int defaultApprovalDurationYears;

        public static IContractor WHIRLPOOL_CONTRACTOR
            {
            get
                {
                return _WHIRLPOOL_CONTRACTOR ??
                    (_WHIRLPOOL_CONTRACTOR = CatalogTable.FindByDescription(typeof(IContractor).GetTableName(), "Whirlpool") as IContractor);
                }
            }

        public override IContractor Contractor
            {
            get { return WHIRLPOOL_CONTRACTOR; }
            }

        public override int ModelIndex
            {
            get { return ArticleIndex; }
            }

        public override int ArticleIndex
            {
            get { return 4; }
            }

        public override int DateIndex
            {
            get { return 2; }
            }

        public override bool ProcessExcelRow(LoadingEuroluxBehaviour.ExcelRow row, ILoadingWareRow docRow, List<LoadingEuroluxBehaviour.ApprovalDocumentInfo> approvalDocuments)
            {
            const int customsCodeIndex = 3;
            const int producerIndex = 16;
            const int tradeMarkIndex = 15;
            const int countryIndex = 14;
            const int unitMeasureIndex = 17;

            const int netIndex = 10;
            const int grossIndex = 30;
            const int priceIndex = 8;

            const int nameInvoiceIndex = 5;
            const int nameDeclIndex = 6;

            docRow.Model = row.Model;

            docRow.NameDecl = row.Row.GetString(nameDeclIndex);
            docRow.NameInvoice = row.Row.GetString(nameInvoiceIndex);

            if (string.IsNullOrEmpty(docRow.NameDecl)
                && string.IsNullOrEmpty(docRow.NameInvoice))
                {
                addWarning("Не заполненно ни наим. декларации ни наим. инв.!", row);
                docRow.RemoveFromTable();
                return true;
                }

            if (string.IsNullOrEmpty(docRow.NameDecl))
                {
                docRow.NameDecl = docRow.NameInvoice;
                }
            else if (string.IsNullOrEmpty(docRow.NameInvoice))
                {
                docRow.NameInvoice = docRow.NameDecl;
                }

            var internalColde = row.Row.GetString(customsCodeIndex);
            docRow.CodeInternal.Id = GetId(internalColde, CustomsCodes, "УКТЗЕД", false);

            if (docRow.CodeInternal.Id == 0)
                {
                addWarning("Не найден УКТЗЕД!", row, internalColde);
                docRow.RemoveFromTable();
                return true;
                }

            docRow.Price = row.Row.GetDouble(priceIndex);
            docRow.NetOfOne = row.Row.GetDouble(netIndex);
            docRow.GrossOfOne = row.Row.GetDouble(grossIndex);

            try
                {
                docRow.Country.Id = GetId(row.Row.GetString(countryIndex), Countries, "Страна");
                docRow.MeasureUnit.Id = GetId(row.Row.GetString(unitMeasureIndex), UnitMeasures, "Един. изм.");

                var producer = row.Row.GetString(producerIndex);
                if (string.IsNullOrEmpty(producer))
                    {
                    addWarning("Не заполненно производитель!", row);
                    docRow.RemoveFromTable();
                    return true;
                    }

                docRow.Producer.Item = GetItem<IManufacturer>(producer, Producers);
                if (docRow.Producer.Item.Id == 0)
                    {
                    addWarning("Не заполненно производитель!", row);
                    docRow.RemoveFromTable();
                    return true;
                    }

                docRow.TradeMark.Item = GetItem<ITradeMark>(row.Row.GetString(tradeMarkIndex), TradeMarks);
                }
            catch (Exception exp)
                {
                string.Format("Ошибка в строке № {0}, на странице \"{1}\".\r\n{2}", row.RowNumber, row.Sheet.Name, exp.Message).WarningBox();
                return false;
                }

            return true;
            }

        public override bool TryLoadApprovals(DataSet dataSet, Action<double> notifyProgress, int approvalDurationYears, out string errorDescription, out string errorHelpData)
            {
            bool justUpdateWares = KeyBoard.CtrlAltShiftPressed() && "Только обновить необход. разрешительные?".Ask();

            errorDescription = string.Empty;
            errorHelpData = string.Empty;
            defaultApprovalDurationYears = approvalDurationYears;
            notifyProgress(0.0);

            var totalRows = 0;
            var currentRowIndex = 0;


            foreach (DataTable table in dataSet.Tables)
                {
                for (int rowIndex = 2; rowIndex < table.Rows.Count; rowIndex++) totalRows++;
                }

            foreach (DataTable table in dataSet.Tables)
                {
                for (int rowIndex = 2; rowIndex < table.Rows.Count; rowIndex++)
                    {
                    var row = table.Rows[rowIndex];

                    const int acticleIndex = 4;
                    var wareId = GetId(row[acticleIndex], NomnclatureCache, "ware", false);
                    if (wareId == 0)
                        {
                        currentRowIndex++;
                        continue;
                        }

                    if (justUpdateWares)
                        {
                        var ware = A.New<Nomenclature>(wareId);
                        ware.NotifyPropertyChanged("Code");
                        var writtenResult = ware.Write();
                        if (!writtenResult.IsSuccess())
                            {
                            string.Format("Не удалось обновить товар: {0}", ware).NotifyToUser(MessagesToUserTypes.Error);
                            return false;
                            }
                        }
                    else
                        {
                        if (!loadApprovals(wareId, row, out errorDescription))
                            {
                            errorHelpData = A.New<Nomenclature>(wareId).Article;
                            return false;
                            }
                        }
                    currentRowIndex++;
                    notifyProgress(((double)currentRowIndex) / totalRows);
                    }
                }

            return true;
            }

        private bool loadApprovals(long wareId, DataRow row, out string errorDescription)
            {
            const int offset = 7;
            const int docNumberIndex = 1;
            const int startDateIndex = 2;
            const int finishDateIndex = 3;

            var approvalsInfo = new List<KeyValuePair<ApprovalInfo, int>>();

            int currentOffset = offset;
            int blockNumber = 0;
            int maxBlockNumber = 5;
            while (blockNumber < maxBlockNumber)
                {
                blockNumber++;
                if (currentOffset >= row.Table.Columns.Count)
                    {
                    break;
                    }

                var docType = GetId(row[currentOffset], ApprovalsTypes, "Типы док-в", false);
                if (docType == 0)
                    {
                    currentOffset += 4;
                    continue;
                    }

                var approvalInfo = new ApprovalInfo(defaultApprovalDurationYears) { DocumentType = docType };
                var docnumber = row[currentOffset + docNumberIndex].ToString().Trim();

                approvalInfo.Number = docnumber;
                Trace.WriteLine(docnumber);

                approvalInfo.StartDate = getDate(row[currentOffset + startDateIndex]);

                var finishDateResultIndex = currentOffset + finishDateIndex;
                if (finishDateResultIndex < row.Table.Columns.Count)
                    {
                    var nextDocType = GetId(row[finishDateResultIndex], ApprovalsTypes, "Типы док-в", false);
                    if (nextDocType == 0)
                        {
                        approvalInfo.FinishDate = getDate(row[finishDateResultIndex]);
                        currentOffset += 4;
                        }
                    else
                        {
                        currentOffset += 3;
                        }
                    }

                if (!approvalInfo.Number.StartsWith("#") && !string.IsNullOrEmpty(approvalInfo.Number))
                    {
                    var added = false;
                    for (int itemIndex = 0; itemIndex < approvalsInfo.Count; itemIndex++)
                        {
                        var item = approvalsInfo[itemIndex];

                        if (item.Key.Equals(approvalInfo))
                            {
                            var number = item.Value + 1;
                            approvalsInfo.Remove(item);
                            approvalsInfo.Add(new KeyValuePair<ApprovalInfo, int>(approvalInfo, number));
                            added = true;
                            }
                        }
                    if (!added)
                        {
                        approvalsInfo.Add(new KeyValuePair<ApprovalInfo, int>(approvalInfo, 1));
                        }
                    }
                }

            return saveApprovalsToDatabase(wareId, approvalsInfo, out  errorDescription);
            }

        private DateTime getDate(object value)
            {
            if (value is DateTime)
                {
                return (DateTime)value;
                }

            return DateTime.MinValue;
            }

        private bool saveApprovalsToDatabase(long wareId, List<KeyValuePair<ApprovalInfo, int>> approvalsInfo, out string errorDescription)
            {
            foreach (var item in approvalsInfo)
                {
                var approval = item.Key;

                var q = DB.NewQuery(@"select Id, case when n.ItemNomenclature is null then 0 else 1 end ContainsWare
	from Approvals a
	left join SubApprovalsNomenclatures n on n.IdDoc = a.Id and n.ItemNomenclature = @Ware
	where a.Contractor = @Contractor and a.DocumentType=@DocumentType
		and a.Deleted = 0 and a.DateFrom = @StartDate and a.DateTo = @FinishDate
		and a.DocumentNumber = @Number
		
		order by ContainsWare desc");
                q.AddInputParameter("Contractor", Contractor.Id);
                q.AddInputParameter("Number", approval.Number);
                q.AddInputParameter("StartDate", approval.StartDate);
                q.AddInputParameter("FinishDate", approval.FinishDate);
                q.AddInputParameter("DocumentType", approval.DocumentType);
                q.AddInputParameter("Ware", wareId);

                var docsCount = item.Value;
                using (var qResult = q.Select())
                    {
                    while (qResult.Next())
                        {
                        var containsWare = qResult["ContainsWare"].ToBoolean();
                        if (!containsWare)
                            {
                            var doc = A.New<Approvals>(qResult["Id"]);
                            doc.AddWareId(wareId);
                            var writtenResult = doc.Write();

                            if (!writtenResult.IsSuccess())
                                {
                                errorDescription = doc.LastWrittingError;
                                return false;
                                }
                            }

                        docsCount--;
                        }
                    }

                for (int i = 0; i < docsCount; i++)
                    {
                    var newDoc = A.New<Approvals>();
                    newDoc.DateFrom = approval.StartDate;
                    newDoc.DateTo = approval.FinishDate;
                    newDoc.Date = DateTime.Now;
                    newDoc.DocumentNumber = approval.Number;
                    newDoc.Contractor = Contractor;
                    newDoc.DocumentType = A.New<IDocumentType>(approval.DocumentType);
                    newDoc.DocumentCode = newDoc.DocumentType.QualifierCodeName;

                    newDoc.AddWareId(wareId);

                    var writtenResult = newDoc.Write();

                    if (!writtenResult.IsSuccess())
                        {
                        errorDescription = newDoc.LastWrittingError;
                        return false;
                        }
                    }
                }

            errorDescription = null;
            return true;
            }

        class ApprovalInfo
            {
            private int defaultApprovalDurationYears;
            public ApprovalInfo(int defaultApprovalDurationYears)
                {
                this.defaultApprovalDurationYears = defaultApprovalDurationYears;
                }

            public string Number { get; set; }

            public DateTime StartDate { get; set; }

            private DateTime finishDate;

            public DateTime FinishDate
                {
                get
                    {
                    return finishDate.Equals(DateTime.MinValue) ?
                        StartDate.AddYears(defaultApprovalDurationYears)
                        : finishDate;
                    }

                set { finishDate = value; }
                }

            public long DocumentType { get; set; }

            public override bool Equals(object obj)
                {
                var item2 = obj as ApprovalInfo;
                if (item2.IsNull()) return false;

                return item2.DocumentType == DocumentType
                       && item2.Number.Equals(Number)
                       && item2.StartDate.Equals(StartDate)
                       && item2.FinishDate.Equals(FinishDate);
                }
            }

        internal override void InitForApprovals()
            {
            NomnclatureCache = new CatalogCacheCreator<Nomenclature>().GetDescriptionIdCache(new { Contractor = Contractor }, "Article");
            }
        }
    }