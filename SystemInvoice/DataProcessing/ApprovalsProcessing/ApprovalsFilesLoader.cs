using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Catalogs;
using SystemInvoice.Documents;
using SystemInvoice.Excel.DataFormatting;
using SystemInvoice.Excel;
using SystemInvoice.DataProcessing.Cache;
using System.Data;
using Aramis.Core;
using Aramis.Attributes;
using Aramis.Enums;
using Aramis.DatabaseConnector;

namespace SystemInvoice.DataProcessing.ApprovalsProcessing
    {
    /// <summary>
    /// Загружает табличную часть РД из екселевского файла, создает новую номенклатуру, если ее не было в системе
    /// </summary>
    public class ApprovalsFilesLoader
        {
        private TableLoader excelLoader = new TableLoader();
        private Approvals approvals = null;
        private SystemInvoiceDBCache cachedData = null;

        public ApprovalsFilesLoader(Approvals approvals, SystemInvoiceDBCache cachedData)
            {
            this.approvals = approvals;
            this.cachedData = cachedData;
            }

        public bool TryLoadApprovals(string path, out string error)
            {
            error = string.Empty;
            //формируем формат загрузки
            ExcelMapper mapper = this.createMapper();
            approvals.Nomenclatures.Rows.Clear();
            //получаем первый номер строки с которой нужно загружать файл
            string firstIndexKey = ApprovalsLoadFormat.ApprovalsLoadColumnNames.FirstLoadRowIndex.ToString();
            string firstRowIndex = mapper[firstIndexKey].ExpressionBody.Replace(@"""", "");
            mapper.Remove(firstIndexKey);
            //проверяем корректность данных
            int firstIndex = 0;
            if (!int.TryParse(firstRowIndex, out firstIndex))
                {
                error = "Не задан номер первой строки";
                return false;
                }
            if (approvals.Contractor.Id == 0 || approvals.TradeMark.Id == 0)
                {
                error = @"Необходимо заполнить поля ""Торговая марка"" и ""Контрагент""";
                return false;
                }
            if (approvals.ApprovalsLoadFormat.Id == 0)
                {
                error = "Необходимо выбрать формат загрузки.";
                return false;
                }
            //загружаем во временную таблицу
            DataTable sourceTable = excelLoader.Transform(mapper, path, firstIndex);
            //загружаем в сам разрешительный
            this.processTable(sourceTable);
            return true;
            }

        private ExcelMapper createMapper()
            {
            ExcelMapper mapper = new ExcelMapper();
            SortedDictionary<int, string> names = Aramis.Core.FastInput.GetCashedData(typeof(ApprovalsLoadFormat.ApprovalsLoadColumnNames));
            if (approvals.ApprovalsLoadFormat.Id > 0)
                {
                approvals.ApprovalsLoadFormat.Read();
                }
            foreach (DataRow row in approvals.ApprovalsLoadFormat.ColumnsMappings.Rows)
                {
                int columnNameIndex = row.TrySafeGetColumnValue<int>("ColumnName", -1);
                string expression = row.TrySafeGetColumnValue<string>("ColumnNumberInExcel", "");
                string constant = row.TrySafeGetColumnValue<string>("Constant", "");
                if (columnNameIndex <= 0 || (string.IsNullOrEmpty(expression) && string.IsNullOrEmpty(constant)))
                    {
                    continue;
                    }
                if (!string.IsNullOrEmpty(constant))
                    {
                    if (!constant.StartsWith(@"""") && !constant.EndsWith(@""""))
                        {
                        expression = string.Concat(@"""", constant, @"""");
                        }
                    else
                        {
                        expression = constant;
                        }
                    }
                else
                    {
                    expression = ExcelHelper.TransformExpression(expression, ApprovalsLoadFormat.ApprovalsLoadFormatColumnNames);
                    }
                mapper.TryAddExpression(((ApprovalsLoadFormat.ApprovalsLoadColumnNames)columnNameIndex).ToString(), ExcelMapper.SumKey, expression);
                }
            return mapper;
            }


        private void processTable(DataTable sourceTable)
            {
            HashSet<long> nomenclatures = null;
            nomenclatures = this.getNomenclatures(sourceTable);
            if (nomenclatures == null)
                {
                return;
                }
            long lineNumber = this.getMaxApprovalsLineNumber();
            foreach (long nomenclatureId in nomenclatures)
                {
                DataRow row = approvals.Nomenclatures.NewRow();
                row[approvals.ItemNomenclature] = nomenclatureId;
                row["LineNumber"] = ++lineNumber;
                approvals.Nomenclatures.Rows.Add(row);
                }
            approvals.NotifyTableRowChanged(approvals.Nomenclatures, approvals.ItemNomenclature, null);
            }

        private int getMaxApprovalsLineNumber()
            {
            int currentLineNumber = 0;
            foreach (DataRow row in approvals.Nomenclatures.Rows)
                {
                object lineNumberValue = row["LineNumber"];
                if (lineNumberValue != null && lineNumberValue != DBNull.Value)
                    {
                    int lineNumberInt = (int)lineNumberValue;
                    if (lineNumberInt > currentLineNumber)
                        {
                        currentLineNumber = lineNumberInt;
                        }
                    }
                }
            return currentLineNumber;
            }

        private HashSet<long> getNomenclatures(DataTable sourceTable)
            {
            int errorsCount = 0;
            HashSet<string> replacementsForAll = new HashSet<string>();
            HashSet<string> noreplacementsForAll = new HashSet<string>();
            cachedData.NomenclatureCacheObjectsStore.Refresh();//.RefreshNomenclaturesInfo();
            cachedData.CustomsCodesCacheStore.Refresh();//.RefreshCustomsCodesInfo();
            HashSet<long> nomenclatureSet = new HashSet<long>();
            List<DataRow> unfoundedRows = new List<DataRow>();
            Dictionary<long, string> nomenclatureManufacturersMaps = new Dictionary<long, string>();
            foreach (DataRow row in sourceTable.Rows)
                {
                long nomenclatureId = getNomenclatureIdForRow(row, replacementsForAll, noreplacementsForAll, nomenclatureManufacturersMaps);
                if (nomenclatureId == 0)
                    {
                    unfoundedRows.Add(row);
                    continue;
                    }
                nomenclatureSet.Add(nomenclatureId);
                }
            HashSet<string> newManufacturers = getProcessedManufacturersList(unfoundedRows, nomenclatureManufacturersMaps);
            if (!cachedData.ManufacturersCreator.TryCreateNewManufacturers(newManufacturers))
                {
                return null;
                }
            errorsCount = this.setNomenclatureManufacturersMapping(nomenclatureManufacturersMaps);
            if (!tryCreateNonExistedNomenclatures(unfoundedRows))
                {
                return null;
                }
            errorsCount = fillNewNomenclatures(nomenclatureSet, unfoundedRows);
            if (errorsCount > 0)
                {
                string errorMEssage = string.Format("Обработка {0} элементов завершилась с ошибкой", errorsCount);
                errorMEssage.AlertBox();
                }
            return nomenclatureSet;
            }

        private HashSet<string> getProcessedManufacturersList(List<DataRow> unfoundedRows, Dictionary<long, string> nomenclatureManufacturersMaps)
            {
            HashSet<string> newManufacturers = new HashSet<string>();
            //Manufacturers for existed nomenclatures
            foreach (string manufacturer in nomenclatureManufacturersMaps.Values)
                {
                newManufacturers.Add(manufacturer.Trim());
                }
            //Manufacturers for new nomenclatures
            foreach (DataRow unfounded in unfoundedRows)
                {
                string manufacturer = unfounded.TrySafeGetColumnValue<string>("Manufacturer", "").Trim();
                newManufacturers.Add(manufacturer);
                }
            return newManufacturers;
            }

        private bool tryCreateNonExistedNomenclatures(List<DataRow> unfoundedRows)
            {
            var nomenclatureCreator = cachedData.NomenclatureCreator;
            nomenclatureCreator.BeginCreation();
            foreach (DataRow row in unfoundedRows)
                {
                string manufacturer = row.TrySafeGetColumnValue<string>("Manufacturer", "").Trim();
                string article = row.TrySafeGetColumnValue<string>("Article", "").Trim();
                string originalName = row.TrySafeGetColumnValue<string>("OriginalName", "");
                string customsCodeExtern = row.TrySafeGetColumnValue<string>("CustomsCodeExtern", "");
                string customsCodeIntern = row.TrySafeGetColumnValue<string>("CustomsCodeIntern", "");
                string declarationName = row.TrySafeGetColumnValue<string>("DeclarationName", "");
                long subGroupId = 0;
                long CustomsCodeId = nomenclatureCreator.CustomsCodesStore.GetCustomsCodeIdForCodeName(customsCodeIntern);
                nomenclatureCreator.AddNomenclature(article, string.Empty, manufacturer, CustomsCodeId, declarationName, string.Empty,
                    string.Empty, customsCodeExtern, string.Empty, 0, 0, 0, 0, string.Empty, declarationName, subGroupId);
                }
            return nomenclatureCreator.TryCreate();
            }

        private int fillNewNomenclatures(HashSet<long> nomenclatureSet, List<DataRow> unfoundedRows)
            {
            int errorsCount = 0;
            foreach (DataRow row in unfoundedRows)
                {
                string article = row.TrySafeGetColumnValue<string>("Article", "").Trim();
                long nomenclatureCreatedID = cachedData.NomenclatureCacheObjectsStore.SelectNomenclatureIfExists(article);
                if (nomenclatureCreatedID == 0)
                    {
                    errorsCount++;
                    continue;
                    }
                nomenclatureSet.Add(nomenclatureCreatedID);
                }
            return errorsCount;
            }

        private int setNomenclatureManufacturersMapping(Dictionary<long, string> nomenclatureTradeMarksMaps)
            {
            int errorsCount = 0;
            foreach (long nomenclatureID in nomenclatureTradeMarksMaps.Keys)
                {
                string manufacturer = nomenclatureTradeMarksMaps[nomenclatureID];
                long manufacturerId = this.cachedData.ManufacturerCacheObjectsStore.GetManufcaturerId(manufacturer);
                if (manufacturerId == 0 && nomenclatureID == 0)
                    {
                    errorsCount++;
                    }
                Nomenclature nomenclature = new Nomenclature();
                nomenclature.Read(nomenclatureID);
                Manufacturer newManufacturer = new Manufacturer();
                newManufacturer.Contractor = nomenclature.Contractor;
                newManufacturer.Id = manufacturerId;
                nomenclature.Manufacturer = newManufacturer;
                if (nomenclature.Write() != WritingResult.Success)
                    {
                    errorsCount++;
                    }
                }
            return errorsCount;
            }

        private long getNomenclatureIdForRow(DataRow row, HashSet<string> replacementsForAll, HashSet<string> noreplacementsForAll, Dictionary<long, string> nomenclatureTradeMarksMaps)
            {
            string article = row.TrySafeGetColumnValue<string>("Article", "").Trim();
            string manufacturer = row.TrySafeGetColumnValue<string>("Manufacturer", "").Trim();
            long nomenclatureID = cachedData.NomenclatureCacheObjectsStore.SelectNomenclatureIfExists(article);
            if (nomenclatureID > 0)
                {
                var nomenclatureInfo = cachedData.GetCachedNomenclature(article, string.Empty);
                if (nomenclatureInfo == null)
                    {
                    return -1;
                    }
                string manufacturerName = cachedData.GetNomenclatureManufacturer(nomenclatureInfo);
                if (!manufacturerName.Equals(manufacturer))
                    {
                    string compareStr = string.Concat(manufacturerName, "{}", manufacturer);
                    if (replacementsForAll.Contains(compareStr))
                        {
                        nomenclatureTradeMarksMaps.Add(nomenclatureID, manufacturer);
                        return nomenclatureID;
                        }
                    if (noreplacementsForAll.Contains(compareStr))
                        {
                        return nomenclatureID;
                        }
                    string askMessage = string.Format("Для номенклатуры {0}\n уже существует производитель {1}\n  заменить его производителем {2} из файла?",
                        article, manufacturerName, manufacturer);
                    SystemInvoice.Documents.Forms.AprrovalsReplaceDialog dialog = new Documents.Forms.AprrovalsReplaceDialog(askMessage);
                    var result = dialog.ShowDialog();
                    switch (result)
                        {
                        case (System.Windows.Forms.DialogResult.Yes): nomenclatureTradeMarksMaps.Add(nomenclatureID, manufacturer); break;
                        case (System.Windows.Forms.DialogResult.No): break;
                        case (System.Windows.Forms.DialogResult.Retry):
                            replacementsForAll.Add(compareStr);
                            nomenclatureTradeMarksMaps.Add(nomenclatureID, manufacturer); break;
                        case (System.Windows.Forms.DialogResult.Ignore):
                            noreplacementsForAll.Add(compareStr);
                            break;
                        default: break;
                        }
                    }
                return nomenclatureID;
                }
            return nomenclatureID;
            }
        }
    }
