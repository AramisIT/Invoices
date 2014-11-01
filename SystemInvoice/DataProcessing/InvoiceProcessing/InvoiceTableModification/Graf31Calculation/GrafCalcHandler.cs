using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using System.Data;
using SystemInvoice.Documents;
using SystemInvoice.Catalogs;
using SystemInvoice.Excel;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.InvoiceTableModification.Graf31Calculation
    {
    /// <summary>
    /// Формирует графу 31, вместе с шапкой.
    /// </summary>
    public class GrafCalcHandler
        {
        private static string contentPlaceHolderA = "(НАБОР)";
        private static string contentPlaceHolderB = "НАБОР";
        private string Graf31SwitchColumnName = "";
        private Invoice invoice = null;
        ExcelLoadingFormat format = null;
        private SystemInvoiceDBCache dbCache = null;
        public GrafCalcHandler(SystemInvoiceDBCache dbCache, Invoice invoice)
            {
            this.dbCache = dbCache;
            Graf31SwitchColumnName = "Graf31FilterColumn"; //InvoiceColumnNames.Graf31FilterColumn.ToString();
            this.invoice = invoice;
            }

        public void FillGrafCells(DataTable tableToProcess)
            {
            string grafContentColumnName = "GraphContent";
            string grafOrderColumnName = "ColumnNumberInGraph";
            string alterNativeColumnName = "GraphContentShoes";
            string alternaticeColumnOrder = "ColumnNumberInGraphShoes";
            ExcelLoadingFormat format = invoice.ExcelLoadingFormat;
            string filterColumnGraf31Value = format.GrafSwitchValue;
            string grafConstrStr = Invoice.GrafColumnReplaceString;
            Dictionary<string, string> graf31ColumnReplacer = new Dictionary<string, string>();
            graf31ColumnReplacer.Add(grafConstrStr, "");
            List<ResultBuilder> builders = getGrafBuilder(grafContentColumnName, grafOrderColumnName, format);
            List<ResultBuilder> alternativeBuilders = getGrafBuilder(alterNativeColumnName, alternaticeColumnOrder, format);
            DateTime from = DateTime.Now;
            processGraf31Values(builders, alternativeBuilders, filterColumnGraf31Value, tableToProcess);
            double period = (DateTime.Now - from).TotalMilliseconds;
            }

        private List<ResultBuilder> getGrafBuilder(string grafContentColumnName, string grafOrderColumnName, ExcelLoadingFormat format)
            {
            Dictionary<int, Graf31ExpressionInfo> graf31ContentDictionary = new Dictionary<int, Graf31ExpressionInfo>();
            List<int> indexes = new List<int>();
            foreach (DataRow row in format.ColumnsMappings.Rows)
                {
                string columnName, graf31content, graf31number;
                object columnNameValue = row[format.ColumnName];
                if (columnNameValue == DBNull.Value || columnNameValue == null)
                    {
                    continue;
                    }
                columnName = ((InvoiceColumnNames)columnNameValue).ToString();
                graf31content = row.TrySafeGetColumnValue<string>(grafContentColumnName, "");
                graf31number = row.TrySafeGetColumnValue<string>(grafOrderColumnName, "");
                int graf31IntNumber = -1;
                if (!string.IsNullOrEmpty(graf31content) && int.TryParse(graf31number, out graf31IntNumber)
                    && !graf31ContentDictionary.ContainsKey(graf31IntNumber))
                    {
                    graf31ContentDictionary.Add(graf31IntNumber, new Graf31ExpressionInfo(columnName, graf31content));
                    indexes.Add(graf31IntNumber);
                    }
                }
            List<ResultBuilder> builders = createResultBuilders(graf31ContentDictionary, indexes);
            return builders;
            }

        private void processGraf31Values(List<ResultBuilder> builders, List<ResultBuilder> alternativeBuilders, string filterColumnGraf31Value, DataTable tableToProcess)
            {
            Dictionary<DataRow, string> headers = this.getGrafRowHeaders(tableToProcess);
            Dictionary<DataRow, string> footers = this.getGrafRowHFooters(tableToProcess);
            List<ResultBuilder> currentBuilders = null;
            foreach (DataRow row in tableToProcess.Rows)
                {
                string filterGrafStrValue = row.TrySafeGetColumnValue<string>(Graf31SwitchColumnName, "");
                if (alternativeBuilders != null && alternativeBuilders.Count != 0 && !string.IsNullOrEmpty(filterGrafStrValue) && filterColumnGraf31Value.Equals(filterGrafStrValue))
                    {
                    currentBuilders = alternativeBuilders;
                    }
                else
                    {
                    currentBuilders = builders;
                    }
                string graf31Content = this.getGrafContent(footers, headers, currentBuilders, row);
                row[InvoiceColumnNames.Graf31.ToString()] = graf31Content;
                }
            }

        private Dictionary<DataRow, string> getGrafRowHeaders(DataTable tableToProcess)
            {
            Dictionary<DataRow, string> headers = new Dictionary<DataRow, string>();
            List<DataRow> currentRowsGroup = new List<DataRow>();
            DataRow previousRow = null;
            foreach (DataRow row in tableToProcess.Rows)
                {
                string rowToCompare = row.TrySafeGetColumnValue<string>(InvoiceColumnNames.Article.ToString(), string.Empty);
                if (rowToCompare.Equals("77E57D_BLK"))
                    {
                    int k = 0;
                    }
                if (isDifferentGroupingRows(row, previousRow))
                    {
                    string rowHeader = getRowsHeader(currentRowsGroup);
                    headers.Add(currentRowsGroup[0], rowHeader);
                    currentRowsGroup.Clear();
                    }
                previousRow = row;
                currentRowsGroup.Add(previousRow);
                }
            if (currentRowsGroup.Count > 0)
                {
                string rowHeaderLast = getRowsHeader(currentRowsGroup);
                headers.Add(currentRowsGroup[0], rowHeaderLast);
                currentRowsGroup.Clear();
                }
            return headers;
            }

        private Dictionary<DataRow, string> getGrafRowHFooters(DataTable tableToProcess)
            {
            Dictionary<DataRow, string> footers = new Dictionary<DataRow, string>();
            List<DataRow> currentRowsGroup = new List<DataRow>();
            DataRow previousRow = null;
            foreach (DataRow row in tableToProcess.Rows)
                {
                if (isDifferentGroupingRows(row, previousRow))
                    {
                    string rowFooter = getRowsFooter(currentRowsGroup);
                    footers.Add(currentRowsGroup[currentRowsGroup.Count - 1], rowFooter);
                    currentRowsGroup.Clear();
                    }
                previousRow = row;
                currentRowsGroup.Add(previousRow);
                }
            return footers;
            }

        private string getRowsHeader(List<DataRow> currentRowsGroup)
            {
            HashSet<string> existedWords = getExistedWords(currentRowsGroup);
            string customsCodeDescription = getCustomsCodeDescriptioin(currentRowsGroup);
            string result = getClearedHeader(customsCodeDescription, existedWords);

            bool containsMale = false, containsFemale = false;
            foreach (string existedWord in existedWords)
                {
                if (existedWord.ToLower().Contains("чол"))
                    {
                    containsMale = true;
                    }
                if (existedWord.ToLower().Contains("жін"))
                    {
                    containsFemale = true;
                    }
                }
            if (!containsMale && !containsFemale)
                {
                result = result.Replace("розміром 42 або більше", "");
                }
            return result;
            //throw new NotImplementedException();
            }

        private string getRowsFooter(List<DataRow> currentRowsGroup)
            {
            int acc = 0;
            int currentAcc = 0;
            foreach (DataRow row in currentRowsGroup)
                {
                string countStr = row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.COUNT_COLUMN_NAME, string.Empty);
                if (int.TryParse(countStr, out currentAcc))
                    {
                    acc += currentAcc;
                    }
                }
            string unitMeasure = string.Empty;
            if (currentRowsGroup.Count > 0)
                {
                string currentUnitOfMeasureStr = currentRowsGroup[0].TryGetColumnValue<string>("UnitOfMeasure", string.Empty).ToLower();
                unitMeasure = currentUnitOfMeasureStr.Length > 2 ? currentUnitOfMeasureStr.Substring(0, 3) : currentUnitOfMeasureStr;
                if (!unitMeasure.EndsWith("."))
                    {
                    unitMeasure = unitMeasure + ".";
                    }
                }
            return string.Concat(" Разом ", acc, " ", unitMeasure);
            }

        private string getClearedHeader(string customsCodeDescription, HashSet<string> existedWords)
            {
            string wordPrepared = customsCodeDescription.Replace(".", " .").Replace(",", " ,").Replace(":", " :").Replace(";", " ;").Replace("-", " -");
            string[] parts = wordPrepared.Split(new string[] { " ", "\t", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder resultBuilder = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
                {
                string part = parts[i];
                if (isInCheckingWords(part))
                    {
                    if (!existedWords.Contains(part.ToUpper()))
                        {
                        continue;
                        }
                    }
                if (part.ToUpper().Equals("З") || part.ToUpper().Equals("ІЗ"))
                    {
                    var newPartIndex = this.checkContentWords(i, parts, resultBuilder, existedWords);
                    if (newPartIndex == i && (i + 1) < parts.Length && !isWordContent(parts[i + 1]))
                        {
                        resultBuilder.Append(" ");
                        resultBuilder.Append(part);
                        }
                    i = newPartIndex;
                    continue;
                    }

                if (!(i == 0 || part.StartsWith(".") || part.StartsWith(",") || part.StartsWith(":") || part.StartsWith(";") || part.StartsWith("-")))
                    {
                    resultBuilder.Append(" ");
                    }
                resultBuilder.Append(part);
                }
            string upperCaseStr = resultBuilder.ToString().TrimStart();
            string upperPart = string.Empty;
            string lowerPart = string.Empty;
            if (upperCaseStr.Length >= 1)
                {
                upperPart = upperCaseStr.Substring(0, 1).ToUpper();
                }
            if (upperCaseStr.Length > 1)
                {
                lowerPart = upperCaseStr.Substring(1, upperCaseStr.Length - 1).ToLower();
                }
            return string.Concat(upperPart, lowerPart);
            }

        private int checkContentWords(int i, string[] parts, StringBuilder resultBuilder, HashSet<string> existedWords)
            {
            resultBuilder.Append(" ");

            int lastContentWordIndex = i;
            int contentWordsCount = 0;
            HashSet<string> exitedInCurrentCustomsCodeContentWords = new HashSet<string>();
            for (int wordIndex = i; wordIndex < parts.Length; wordIndex++)
                {
                string wordToCheck = parts[wordIndex].Trim().ToUpper(); ;
                if (isWordContent(wordToCheck))
                    {
                    if (existedWords.Contains(wordToCheck))
                        {
                        contentWordsCount++;
                        exitedInCurrentCustomsCodeContentWords.Add(wordToCheck);
                        }
                    lastContentWordIndex = wordIndex;
                    }
                }
            if (contentWordsCount == 0)
                {
                //resultBuilder.Append(parts[i]);
                //resultBuilder.Append(" ");
                //for (int k = i + 1; k <= lastContentWordIndex; k++)
                //    {
                //    resultBuilder.Append(" ");
                //    resultBuilder.Append(parts[k]);
                //    }
                }
            else
                {
                var contentWords = new string[lastContentWordIndex - i];
                for (int k = i + 1; k <= lastContentWordIndex; k++)
                    {
                    contentWords[k - i - 1] = parts[k].Trim();
                    }

                var content = getClearContent(contentWords, exitedInCurrentCustomsCodeContentWords);
                if (content.Length > 0)
                    {
                    resultBuilder.Append(" ");
                    resultBuilder.Append(parts[i]);
                    resultBuilder.Append(" ");
                    resultBuilder.Append(content);
                    }
                }
            return lastContentWordIndex;
            }

        private bool isSeparator(string word)
            {
            var result = word.Equals(",") || word.Equals("АБО") || word.Equals("ТА") || word.Equals("І");
            return result;
            }

        private StringBuilder getClearContent(string[] contentWords, HashSet<string> exitedInCurrentCustomsCodeContentWords)
            {
            var resultBuilder = new StringBuilder();

            var firstWordOfSetIndex = 0;
            var addCurrentWordsSet = false;
            var skipCurrentSet = false;

            for (int wordIndex = 0; wordIndex < contentWords.Length; wordIndex++)
                {
                string wordToCheck = contentWords[wordIndex].ToUpper();
                var lastWord = wordIndex + 1 == contentWords.Length;

                if (!addCurrentWordsSet && !skipCurrentSet)
                    {
                    if (isWordContent(wordToCheck))
                        {
                        addCurrentWordsSet = exitedInCurrentCustomsCodeContentWords.Contains(wordToCheck);
                        if (!addCurrentWordsSet)
                            {
                            skipCurrentSet = true;
                            }
                        }
                    }

                var isComma = isSeparator(wordToCheck);
                var endOfSet = isComma || lastWord;

                if (endOfSet)
                    {
                    if (addCurrentWordsSet)
                        {
                        for (int addedWordIndex = firstWordOfSetIndex; addedWordIndex <= (isComma ? wordIndex - 1 : wordIndex); addedWordIndex++)
                            {
                            var addingWord = contentWords[addedWordIndex];
                            if (resultBuilder.Length != 0 || !isSeparator(addingWord))
                                {
                                resultBuilder.Append(addingWord);
                                resultBuilder.Append(" ");
                                }
                            }
                        }
                    firstWordOfSetIndex = wordIndex;
                    addCurrentWordsSet = false;
                    skipCurrentSet = false;
                    }
                }

            return resultBuilder;

            //{
            //var resultBuilder = new StringBuilder();

            //for (int wordIndex = 0; wordIndex < contentWords.Length; wordIndex++)
            //    {
            //    var notLastWord = wordIndex + 1 != contentWords.Length;
            //    var nextWord = notLastWord ? contentWords[wordIndex + 1].ToUpper() : string.Empty;
            //    var isUnion = nextWord.Equals("АБО") || nextWord.Equals("ТА") || nextWord.Equals("І");

            //    string wordToCheck = contentWords[wordIndex].ToUpper();
            //    if (isWordContent(wordToCheck))
            //        {
            //        if (exitedInCurrentCustomsCodeContentWords.Contains(wordToCheck))
            //            {
            //            resultBuilder.Append(" ");
            //            resultBuilder.Append(wordToCheck);
            //            }
            //        else
            //            {
            //            if (notLastWord && isUnion)
            //                {
            //                wordIndex++;
            //                }
            //            }
            //        continue;
            //        }

            //    if (isUnion)
            //        {
            //        if (notLastWord)
            //            {
            //            resultBuilder.Append(" ");
            //            resultBuilder.Append(wordToCheck);
            //            continue;
            //            }
            //        }

            //    resultBuilder.Append(contentWords[wordIndex]);
            //    resultBuilder.Append(" ");
            //    }

            //return resultBuilder;
            //}
            }

        private bool isWordContent(string part)
            {
            return dbCache.MaterialsMappingCacheObjectsStore.IsMaterialType(part);
            }

        private bool isInCheckingWords(string part)
            {
            string checkingWord = part.Trim().ToUpper();
            return dbCache.PropertyTypesCacheObjectsStore.ContainsInCorrespondingValues(checkingWord);
            }

        private string getCustomsCodeDescriptioin(List<DataRow> currentRowsGroup)
            {
            string customsCodeIdStr = InvoiceColumnNames.CustomsCodeIntern.ToString();
            if (currentRowsGroup.Count > 0)
                {
                string customsCodeCodeName = currentRowsGroup[0].TrySafeGetColumnValue(customsCodeIdStr, string.Empty);
                return dbCache.CustomsCodesCacheStore.GetCustomsCodeDescription(customsCodeCodeName);
                }
            return string.Empty;
            }

        private HashSet<string> getExistedWords(List<DataRow> currentRowsGroup)
            {
            HashSet<string> existedWords = new HashSet<string>();
            foreach (DataRow row in currentRowsGroup)
                {
                this.appendWords(row, existedWords);
                }
            return existedWords;
            }


        private void appendWords(DataRow row, HashSet<string> existedWords)
            {
            this.appendGenderWords(row, existedWords);
            this.appendProductNameWords(row, existedWords);
            this.appendContentWords(row, existedWords);
            }

        private void appendContentWords(DataRow row, HashSet<string> existedWords)
            {
            IEnumerable<string> contentWords = this.getContentWord(row);
            foreach (string contentWord in contentWords)
                {
                if (!string.IsNullOrEmpty(contentWord))
                    {
                    existedWords.Add(contentWord);
                    }
                }
            }

        private IEnumerable<string> getContentWord(DataRow row)
            {
            Dictionary<string, double> materialTypesPercentage = new Dictionary<string, double>();
            double maxPercentage = 0;
            string contentDescription = row.TryGetColumnValue(ProcessingConsts.ColumnNames.CONTENT_COLUMN_NAME, string.Empty);
            string[] contentsWithPercentage = contentDescription.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < contentsWithPercentage.Length / 2; i++)
                {
                double percentageValue = getPercentageValue(contentsWithPercentage, i);
                foreach (string materialTypeKeyWord in getMaterialTypeKeyWords(contentsWithPercentage, i))
                    {
                    maxPercentage = updateTotalMaterialPercentage(materialTypesPercentage, materialTypeKeyWord, percentageValue, maxPercentage);
                    }
                }
            return getMaterialTypesForMaxPercentage(materialTypesPercentage, maxPercentage);
            }

        private double updateTotalMaterialPercentage(Dictionary<string, double> materialTypesPercentage, string materialTypeKeyWord,
                                                     double percentageValue, double maxPercentage)
            {
            double currentMaterialPercentage = getCurrentMaterialPercentage(materialTypesPercentage, materialTypeKeyWord);
            double currentValue = currentMaterialPercentage + percentageValue;
            materialTypesPercentage[materialTypeKeyWord] = currentValue;
            if (currentValue > maxPercentage)
                {
                maxPercentage = currentValue;
                }
            return maxPercentage;
            }

        private double getCurrentMaterialPercentage(Dictionary<string, double> materialTypesPercentage, string materialTypeKeyWord)
            {
            double existedValue;
            if (!materialTypesPercentage.TryGetValue(materialTypeKeyWord, out existedValue))
                {
                existedValue = 0;
                materialTypesPercentage.Add(materialTypeKeyWord, existedValue);
                }
            return existedValue;
            }

        private List<string> getMaterialTypesForMaxPercentage(Dictionary<string, double> materialTypesPercentage, double currentMax)
            {
            List<string> contentWords = new List<string>();
            foreach (var pair in materialTypesPercentage)
                {
                if (pair.Value.Equals(currentMax))
                    {
                    contentWords.Add(pair.Key);
                    }
                }
            return contentWords;
            }

        private string[] getMaterialTypeKeyWords(string[] contentParts, int i)
            {
            string name = contentParts[2 * i + 1];
            return dbCache.MaterialsMappingCacheObjectsStore.GetMaterialType(name);
            }

        private double getPercentageValue(string[] contentParts, int i)
            {
            double percentageValue = 0;
            string percentage = contentParts[2 * i].Trim();
            if (!percentage.EndsWith("%"))
                {
                return 0;
                }
            if (!double.TryParse(percentage.Replace("%", ""), out percentageValue))
                {
                return 0;
                }
            return percentageValue;
            }

        private void appendProductNameWords(DataRow row, HashSet<string> existedWords)
            {
            string declColumnName = InvoiceColumnNames.NomenclatureInvoice.ToString();
            string declaration = row.TrySafeGetColumnValue(declColumnName, string.Empty);
            if (string.IsNullOrEmpty(declaration))
                {
                return;
                }
            string[] declarationParts = new string[] { declaration }; //declaration.Split(new string[] { " ", "\r", "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string delcarationPart in declarationParts)
                {
                foreach (string item in dbCache.PropertyTypesCacheObjectsStore.GetCorrespondingElements(delcarationPart.Trim().ToUpper()))
                    {
                    if (!string.IsNullOrEmpty(item))
                        {
                        existedWords.Add(item);
                        }
                    }
                }
            }

        private void appendGenderWords(DataRow row, HashSet<string> existedWords)
            {
            string genderColumnName = InvoiceColumnNames.Gender.ToString();
            string genderDefStr = row.TrySafeGetColumnValue(genderColumnName, string.Empty);
            foreach (string item in dbCache.PropertyTypesCacheObjectsStore.GetCorrespondingElements(genderDefStr.ToUpper()))
                {
                if (!string.IsNullOrEmpty(item))
                    {
                    existedWords.Add(item);
                    }
                }
            }

        private bool isDifferentGroupingRows(DataRow row, DataRow previousRow)
            {
            if (previousRow == null || row == null)
                {
                return false;
                }
            string currentRowCustomCode = row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.CUSTOM_CODE_INTERNAL_COLUMN_NAME, string.Empty);
            string previousRowCustomCode = previousRow.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.CUSTOM_CODE_INTERNAL_COLUMN_NAME, string.Empty);
            string currentCountry = row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.COUNTRY_COLUMN_NAME, string.Empty);
            string previousCountry = previousRow.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.COUNTRY_COLUMN_NAME, string.Empty);
            string manufactuer = row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.MANUFACTURER_COLUMN_NAME, string.Empty);
            string previousManufacturer = previousRow.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.MANUFACTURER_COLUMN_NAME, string.Empty);
            string trademark = row.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.TRADEMARK_COLUMN_NAME, string.Empty);
            string preiousTradeMark = previousRow.TrySafeGetColumnValue(ProcessingConsts.ColumnNames.TRADEMARK_COLUMN_NAME, string.Empty);
            if (!currentRowCustomCode.Equals(previousRowCustomCode) ||
                !currentCountry.Equals(previousCountry) ||
                !manufactuer.Equals(previousManufacturer) ||
                !trademark.Equals(preiousTradeMark))
                {
                return true;
                }
            return false;
            }

        private List<ResultBuilder> createResultBuilders(Dictionary<int, Graf31ExpressionInfo> graf31ContentDictionary, List<int> indexes)
            {
            List<ResultBuilder> builders = new List<ResultBuilder>();
            //  string graf31ItemPlaceHolder = "A";
            foreach (int index in indexes.OrderBy(index => index))
                {
                Graf31ExpressionInfo additionalStr = graf31ContentDictionary[index];
                string currentColumnName = additionalStr.columnName;
                string[] expressionParts = ExcelHelper.DivideString(additionalStr.expression);
                foreach (string part in expressionParts)
                    {
                    appendBuilders(builders, currentColumnName, part);
                    }
                }
            return builders;
            }

        private void appendBuilders(List<ResultBuilder> builders, string currentColumnName, string part)
            {
            if (part.StartsWith(@"""") && part.EndsWith(@""""))
                {
                builders.Add(new ConstantBuilder(part.Substring(1, part.Length - 2)));
                return;
                }
            if (part.Contains(contentPlaceHolderA))
                {
                appendBuildersForContentWithBrakets(builders, currentColumnName, part);
                return;
                }
            if (part.Contains(contentPlaceHolderB))
                {
                appendBuildersForContentWithoutBrakets(builders, currentColumnName, part);
                return;
                }
            if (!part.Contains(Invoice.GrafColumnReplaceString))
                {
                builders.Add(new ConstantBuilder(part));
                return;
                }
            string[] replacements = part.Split(new string[] { Invoice.GrafColumnReplaceString }, StringSplitOptions.None);
            for (int i = 0; i < replacements.Length - 1; i++)
                {
                builders.Add(new ConstantBuilder(replacements[i]));
                builders.Add(new ColumnValueBuilder(currentColumnName));
                }
            builders.Add(new ConstantBuilder(replacements.Last()));
            }

        private void appendBuildersForContentWithoutBrakets(List<ResultBuilder> builders, string currentColumnName, string part)
            {
            ContentWithoutBraketsBuilder nCB = new ContentWithoutBraketsBuilder(this.dbCache);
            string[] subParts = part.Split(new string[] { contentPlaceHolderB }, StringSplitOptions.RemoveEmptyEntries);
            if (part.Length == 0)
                {
                builders.Add(nCB);
                }
            else
                {
                if (part.StartsWith(contentPlaceHolderB))
                    {
                    builders.Add(nCB);
                    }
                appendBuilders(builders, currentColumnName, subParts[0]);
                for (int i = 1; i < subParts.Length; i++)
                    {
                    builders.Add(nCB);
                    appendBuilders(builders, currentColumnName, subParts[i]);
                    }
                if (part.EndsWith(contentPlaceHolderB))
                    {
                    builders.Add(nCB);
                    }
                }
            }

        private void appendBuildersForContentWithBrakets(List<ResultBuilder> builders, string currentColumnName, string part)
            {
            ContentWithBraketsBuilder nCB = new ContentWithBraketsBuilder(this.dbCache);
            string[] subParts = part.Split(new string[] { contentPlaceHolderA }, StringSplitOptions.RemoveEmptyEntries);
            if (part.Length == 0)
                {
                builders.Add(nCB);
                }
            else
                {
                if (part.StartsWith(contentPlaceHolderA))
                    {
                    builders.Add(nCB);
                    }
                appendBuilders(builders, currentColumnName, subParts[0]);
                for (int i = 1; i < subParts.Length; i++)
                    {
                    builders.Add(nCB);
                    appendBuilders(builders, currentColumnName, subParts[i]);
                    }
                if (part.EndsWith(contentPlaceHolderA))
                    {
                    builders.Add(nCB);
                    }
                }
            }

        private string getGrafContent(Dictionary<DataRow, string> footers, Dictionary<DataRow, string> headers, List<ResultBuilder> builders, DataRow row)
            {
            string totalContent = string.Concat(builders.Select((builder) => builder.GetValuePart(row)));
            if (!this.invoice.SetGrafHeader)
                {
                return totalContent;
                }
            string headerContent;
            if (headers.TryGetValue(row, out headerContent))
                {
                totalContent = string.Concat(headerContent, totalContent);
                }
            string footerContent;
            if (footers.TryGetValue(row, out footerContent))
                {
                totalContent = string.Concat(totalContent, footerContent);
                }
            return totalContent;
            }

        private struct Graf31ExpressionInfo
            {
            public string columnName;
            public string expression;
            public Graf31ExpressionInfo(string columnName, string expression)
                {
                this.columnName = columnName;
                this.expression = expression;
                }
            }

        private abstract class ResultBuilder
            {
            public abstract string GetValuePart(DataRow row);
            }

        private abstract class ContentBuilderBase : ResultBuilder
            {
            private SystemInvoiceDBCache cache = null;

            protected ContentBuilderBase(SystemInvoiceDBCache cache)
                {
                this.cache = cache;
                }

            public override string GetValuePart(DataRow row)
                {
                string content = getContentStr(row);
                return GetFinalResultStr(content);
                }

            protected abstract string GetFinalResultStr(string content);

            private string getContentStr(DataRow row)
                {
                long nomenclatureId = row.TrySafeGetColumnValue<long>("FoundedNomenclature", 0);
                if (nomenclatureId > 0)
                    {
                    var cached = cache.NomenclatureCacheObjectsStore.GetCachedObject(nomenclatureId);
                    if (cached != null && cached.HasContent > 0)
                        {
                        try
                            {
                            string selectStr = @"
select coalesce(LTRIM(RTRIM(nom.Description)),'') as declName,cast(coalesce(cont.ItemCount,0) as nvarchar(10)) as itemsCount from SubNomenclatureSetContents as cont
join Nomenclature as nom on nom.Id = cont.ItemNomenclature
 where cont.IdDoc = @Id";
                            var query = Aramis.DatabaseConnector.DB.NewQuery(selectStr);
                            query.AddInputParameter("Id", nomenclatureId);
                            List<string> declarations = new List<string>();
                            List<string> counts = new List<string>();
                            fillItems(query, declarations, counts);
                            if (declarations.Count > 0)
                                {
                                string totalContent = string.Concat(" ", declarations[0], " ", counts[0], " шт.");
                                for (int i = 1; i < declarations.Count; i++)
                                    {
                                    totalContent += string.Concat(", ", declarations[i], " ", counts[i], " шт.");
                                    }
                                return totalContent;
                                }
                            }
                        catch
                            {
                            }
                        }
                    }
                return string.Empty;
                }

            private static void fillItems(Aramis.DatabaseConnector.Query query, List<string> declarations, List<string> counts)
                {
                query.Foreach((res) =>
                {
                    string declName = (string)res["declName"];
                    string count = (string)res["itemsCount"];
                    declarations.Add(declName);
                    counts.Add(count);
                });
                }
            }

        private class ContentWithBraketsBuilder : ContentBuilderBase
            {
            public ContentWithBraketsBuilder(SystemInvoiceDBCache cache)
                : base(cache)
                {
                }

            protected override string GetFinalResultStr(string content)
                {
                if (string.IsNullOrEmpty(content))
                    {
                    return string.Empty;
                    }
                return string.Concat("(", content, ")");
                }
            }

        private class ContentWithoutBraketsBuilder : ContentBuilderBase
            {
            public ContentWithoutBraketsBuilder(SystemInvoiceDBCache cache)
                : base(cache)
                {
                }

            protected override string GetFinalResultStr(string content)
                {
                return content ?? string.Empty;
                }
            }

        private class ConstantBuilder : ResultBuilder
            {
            private string constant = "";
            public ConstantBuilder(string constant)
                {
                this.constant = constant;
                }
            public override string GetValuePart(DataRow row)
                {
                return constant;
                }
            }

        private class ColumnValueBuilder : ResultBuilder
            {
            private string columnName = "";
            public ColumnValueBuilder(string columnName)
                {
                this.columnName = columnName;
                }
            public override string GetValuePart(DataRow row)
                {
                return row.TrySafeGetColumnValue<string>(columnName, "");
                }
            }
        }
    }
