using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Core;
using Aramis.DatabaseConnector;
using System.Data;
using SystemInvoice.Excel;
using SystemInvoice.Excel.DataFormatting;
using SystemInvoice.Excel.DataFormatting.Formatters;
using SystemInvoice.Catalogs;
using SystemInvoice.Documents;
using SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers;
using SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers.Zara;
using System.IO;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers.Zepter;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.Files
    {
    /// <summary>
    /// Загружает входящий файл инвойса для обработки
    /// </summary>
    public class NewDocumentLoader
        {
        private SystemInvoiceDBCache cachedData = null;
        private const string graf31ColumnName = "Graf31";
        private Invoice invoice;
        private const char fromIndexChar = '[';
        private const char toIndexChar = ']';
        private TableLoader loader = new TableLoader();

        public NewDocumentLoader(Invoice invoice, SystemInvoiceDBCache cachedData)
            {
            loader = new TableLoader(true);
            this.cachedData = cachedData;
            this.invoice = invoice;
            register();
            }

        private bool tryAddAsCustomExpression(ExcelMapper mapper, string propertyName, string expression)
            {
            if (!expression.Contains(fromIndexChar) || !expression.Contains(toIndexChar))
                {
                return false;
                }
            int fromIndex = expression.IndexOf(fromIndexChar);
            int toIndex = expression.IndexOf(toIndexChar);
            if (toIndex <= fromIndex + 1)
                {
                return false;
                }
            string indexStr = expression.Substring(fromIndex + 1, toIndex - fromIndex - 1);
            string customPart = expression.Substring(0, fromIndex);
            return mapper.TryAddExpression(propertyName, customPart, indexStr);
            }
        /// <summary>
        /// Регистрирует классы - интерпретаторы, которые позволяют обрабатывать данные из входящего файла в функциональном виде. 
        /// Для получения данных обработанных нитерперетатором, необходимо в формате загрузки написать выражение вида: Функция[аргумент1,аргумент2,...]
        /// где Функция - код интерперетатора по которому он был зарегестрирован;аргумент1,аргумент2, - список аргументов, которые он может получать в обработчике,
        /// в список могут входить индексы колонок (в виде числа), имена колонок (наименование на русском в табличной части инвойса), либо константа
        /// </summary>
        private void register()
            {
            string customsCodeFromArticleKey = "Артикул.ТамКод";
            string declarationNameFromArticleKey = "Артикул.НДекл";
            string invoiceNameFromArticleKey = "Артикул.НИнв";
            //string contractorNameFromArticleKey = "Артикул.Контр";
            string convertFromUnitOfMeasureKey = "Ед.Код";
            //string itemWeightFromNetWeightAndCount = "Ед.Товара";
            string manufacturerFromArticleKey = "Артикул.Производ";
            string multiplyExpressionKey = "Произв";
            string articleZaraExpressionKey = "АртикулЗара";
            string customsCodeExternZara = "Тамож.Код.Вн.Зара";
            string genderShortNameZaraKey = "ПолЗара";
            string invoiceDataZaraKey = "ДатаИнвойса";
            string contentZaraKey = "Состав";
            string originalNameFromArticleKey = "Артикул.НИсх";
            string barCodeNameFromArticleKey = "Артикул.ШтрихКод";
            string insoleLengthZaraKey = "ДлиннаCтельки";
            string customsCodeExternFromArticleKey = "ТамКодВнешний";
            string genderMSKey = "Пол";
            string contentTranslatedKey = "СоставПеревод";
            string sizeTranslatedKey = "РазмерПеревод";
            string unitNetWeightKey = "ВесЕдТовара";
            string netWeightTotalKey = "ВесНетто";
            string codeFromCountryRu = "СтранаРусКод";
            string summCalc = "Сумма";
            string bnsSubGroupOfGoods = "BNSПодгруппа";
            string genderTranslate = "ПолПеревод";
            string bnsUnitOfMeasure = "BNS.ЕдИзм";
            string marginFromPercent = "Н_Процент";
            string withMargin = "С_Наценкой";
            string translate = "Перевод";
            string nameFromGroupStr = "Наименование";
            string zepterDeclKey = "Цептер.Декл";

            registerCustomHandler("Артикул.ТоргМарка", new TradeMarkFromArticleHandler(cachedData, () => invoice.Contractor));
            registerCustomHandler("Артикул.Модель", new ModelFromArticleHandler(cachedData));

            registerCustomHandler(customsCodeFromArticleKey, new CustomsCodeFromArticleHandler(cachedData));
            registerCustomHandler(declarationNameFromArticleKey, new DeclarationNameFromArticleHandler(cachedData));
            registerCustomHandler(invoiceNameFromArticleKey, new InvoiceNameFromArticleHandler(cachedData));
            //registerCustomHandler(contractorNameFromArticleKey, new ContractorNameFromArticleHandler(cachedData));
            registerCustomHandler(convertFromUnitOfMeasureKey, new GetUnitOfMeasureCodeFromNameHandler(cachedData));
            //registerCustomHandler(itemWeightFromNetWeightAndCount, new ItemWeightFromNetWeightHandler(cachedData));
            registerCustomHandler(manufacturerFromArticleKey, new ManufacturerFromArticleHandler(cachedData));
            registerCustomHandler(multiplyExpressionKey, new MultiplyExpressionHandler(cachedData));
            registerCustomHandler(articleZaraExpressionKey, new ArticleZaraExpressionHandler(cachedData));
            registerCustomHandler(customsCodeExternZara, new ExternCustomsCodeZaraHandler(cachedData));
            registerCustomHandler(genderShortNameZaraKey, new GenderZaraHandler(cachedData));
            registerCustomHandler(invoiceDataZaraKey, new InvoiceDateZaraHandler(cachedData));
            registerCustomHandler(contentZaraKey, new ContentZaraDressHandler(cachedData));
            registerCustomHandler(originalNameFromArticleKey, new OriginalNameFromArticleHandler(cachedData));
            registerCustomHandler(barCodeNameFromArticleKey, new BarCodeHandler(cachedData));
            registerCustomHandler(insoleLengthZaraKey, new InsoleLengthHandler(cachedData));
            registerCustomHandler(customsCodeExternFromArticleKey, new CustomsCodeExternFromArticleHandler(cachedData));
            registerCustomHandler(genderMSKey, new MarksAndSpenserGenderHandler(cachedData));
            registerCustomHandler(contentTranslatedKey, new TranslateContentHandler(cachedData));
            registerCustomHandler(sizeTranslatedKey, new SizeTranslateExpressionHandler(cachedData));
            registerCustomHandler(unitNetWeightKey, new UnitNWFromCountAndNWHandler(cachedData));
            registerCustomHandler(netWeightTotalKey, new NWeightFromCountAndUnitNWHandler(cachedData));
            registerCustomHandler(codeFromCountryRu, new CountryCodeFromRuNameHandler(cachedData));
            registerCustomHandler(summCalc, new SummCalculationHandler(cachedData));
            registerCustomHandler(bnsSubGroupOfGoods, new BNSSubGroupOfGoodsHandler(cachedData));//GenderTranslatehandler
            //   registerCustomHandler(genderTranslate, new GenderTranslatehandler(cachedData));
            registerCustomHandler(bnsUnitOfMeasure, new BNSUnitOfMeasureCodeHandler(cachedData));
            registerCustomHandler(marginFromPercent, new MarginFromPercentageAndPrice(cachedData));
            registerCustomHandler(withMargin, new PriceWithMargin(cachedData));
            registerCustomHandler(translate, new TranslationHandler(cachedData));
            registerCustomHandler(nameFromGroupStr, new NameFromGroupHandler(cachedData));
            registerCustomHandler(zepterDeclKey, new ZepterDeclarationHandler(cachedData));
            }

        private void registerCustomHandler(string expression, CustomExpressionHandlerBase handler)
            {
            loader.RegisterFormatter(expression, new CustomDelegateExpressionFormatterConstructor(handler), true);
            }


        public bool TryFillGoodsTable(string targetPath)
            {
            return tryFillGoodsTable(targetPath, this.invoice.Goods);
            }

        public bool TryCreateTable(string targetPath, out DataTable table)
            {
            table = this.invoice.Goods.Clone();
            return tryFillGoodsTable(targetPath, table);
            }

        private bool tryFillGoodsTable(string targetPath, DataTable table)
            {
            if (!canProcessFile(targetPath))
                {
                return false;
                }
            DataTable tableToFill = table;
            ExcelLoadingFormat loadingFormat = this.invoice.ExcelLoadingFormat;
            if (!string.IsNullOrEmpty(targetPath) && File.Exists(targetPath))
                {
                try
                    {
                    int startReadingINdex = loadingFormat.FirstRowNumber;
                    tableToFill.Rows.Clear();
                    ExcelMapper mapper = this.createMapper(loadingFormat);
                    cachedData.RefreshCache();
                    if (!loader.TryFill(tableToFill, mapper, targetPath, startReadingINdex))
                        {
                        return false;
                        }
                    }
                catch (SystemInvoice.DataProcessing.Cache.TradeMarksCache.TradeMarkCacheObjectsStore.TradeMarkConflictException conflictExceprion)
                    {
                    "Неправильный формат загрузки. Торговая марка не может быть в загружаемом файле, если она указана в самом формате.".AlertBox();
                    return false;
                    }
                finally
                    {
                    fillLinesNumbers(tableToFill);
                    //   checkTable( tableToFill );
                    }
                }
            else
                {
                "Файл не найден".AlertBox();
                return false;
                }
            return true;
            }

        private bool canProcessFile(string targetPath)
            {
            if (string.IsNullOrEmpty(targetPath))
                {
                "Необходимо выбрать файл".AlertBox();
                return false;
                }
            if (this.invoice.Contractor.Id == 0)
                {
                "Необходимо выбрать контрагента.".AlertBox();
                return false;
                }
            if (this.invoice.ExcelLoadingFormat.Id == 0)
                {
                "Необходимо выбрать формат загрузки".AlertBox();
                return false;
                }
            if (!this.checkTradeMark())
                {
                "Необходимо выбрать торговую марку.".AlertBox();
                return false;
                }
            return true;
            }

        private bool checkTradeMark()
            {
            ExcelLoadingFormat loadingFormat = this.invoice.ExcelLoadingFormat;
            if (loadingFormat.Id != 0)
                {
                foreach (DataRow format in loadingFormat.ColumnsMappings.Rows)
                    {
                    if ((InvoiceColumnNames)format[loadingFormat.ColumnName] == InvoiceColumnNames.ItemTradeMark)
                        {
                        string columnNumberInExcel = format.TrySafeGetColumnValue<string>("ColumnNumberInExcel", "").Trim();
                        string constantValue = format.TrySafeGetColumnValue<string>("Constant", "").Trim();
                        if (!string.IsNullOrEmpty(columnNumberInExcel) || !string.IsNullOrEmpty(constantValue))
                            {
                            return true;
                            }
                        }
                    }
                }
            return this.invoice.TradeMark.Id != 0;
            }

        /// <summary>
        /// Формирует привязку колонок из входящего файла в табличную часть инвойса
        /// </summary>
        /// <param name="format">Формат загрузки</param>
        private ExcelMapper createMapper(ExcelLoadingFormat format)
            {
            ExcelMapper mapper = new ExcelMapper();
            foreach (DataRow row in format.ColumnsMappings.Rows)
                {
                string columnName, expression, constant;
                object columnNameValue = row[format.ColumnName];
                if (columnNameValue == DBNull.Value || columnNameValue == null)
                    {
                    continue;
                    }
                columnName = ((InvoiceColumnNames)columnNameValue).ToString();
                expression = row.TrySafeGetColumnValue(format.ColumnNumberInExcel.ColumnName, "");// tryGetColumnValue<string>(row[format.ColumnNumberInExcel], "");
                constant = row.TrySafeGetColumnValue(format.Constant.ColumnName, ""); //tryGetColumnValue<string>(row[format.Constant], "");
                if (tryAddAsCustomExpression(mapper, columnName, expression))
                    {
                    continue;
                    }

                if (!string.IsNullOrEmpty(constant))
                    {
                    expression = string.Concat(ExcelHelper.ConstantDelimeter, constant, ExcelHelper.ConstantDelimeter);
                    }
                expression = ExcelHelper.TransformExpression(expression, Invoice.InvoiceColumnNamesTranslated);
                if (!mapper.TryAddExpression(columnName, ExcelMapper.SumKey, expression, ""))
                    {
                    string.Format("Некорректный формат загрузки. Колонка {0} содержится более одного раза.", columnName).AlertBox();
                    return null;
                    }
                }
            if (mapper.ContainsKey(graf31ColumnName))
                {
                mapper.Remove(graf31ColumnName);
                }
            if (!string.IsNullOrEmpty(format.ColumnIndexForGrafShoes))
                {
                mapper.TryAddExpression("Graf31FilterColumn", ExcelMapper.IndexKey, format.ColumnIndexForGrafShoes);
                }
            return mapper;
            }

        private void fillLinesNumbers(DataTable table)
            {
            int lineNumber = 0;
            foreach (DataRow row in table.Rows)
                {
                row["LineNumber"] = ++lineNumber;
                }
            }

        }
    }
