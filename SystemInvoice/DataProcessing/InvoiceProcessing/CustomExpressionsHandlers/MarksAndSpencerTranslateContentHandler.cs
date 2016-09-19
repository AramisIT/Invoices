using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SystemInvoice.DataProcessing.Cache;
using SystemInvoice.DataProcessing.Cache.PropertyTypesCache;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.CustomExpressionsHandlers
    {
    /// <summary>
    /// Делает перевод состава. Входящие данные - строка в которой полностью описан состав в формате - % наименование 
    /// </summary>
    public class TranslateContentHandler : CustomExpressionHandlerBase
        {
        long contentPropertyID = 0;
        public TranslateContentHandler(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            contentPropertyID = dbCache.PropertyOfGoodsCacheObjectsStore.GetCachedObjectId("Состав");
            }

        public override object ProcessRow(params object[] parameters)//СоставПеревод[1]
            {
            if (parameters.Length == 0)
                {
                return string.Empty;
                }
            string content = (string)parameters[0];
            string finalContent = "";
            string[] splitted = getContentParts(content);
            for (int i = 0; i < splitted.Length; i += 2)
                {
                //check!!!
                if (i + 1 >= splitted.Length)
                    {
                    finalContent += string.Concat(" ", splitted[i]);
                    break;
                    }
                int j = i + 2;
                while (j < splitted.Length)
                    {
                    bool isNextNumeric = true;
                    string next = splitted[j];
                    foreach (char c in next)
                        {
                        if (!Char.IsDigit(c) && !c.Equals('%') && !c.Equals('.'))
                            {
                            isNextNumeric = false;
                            break;
                            }
                        }
                    if (!isNextNumeric)
                        {
                        j++;
                        }
                    else
                        {
                        break;
                        }
                    }
                if (j > splitted.Length)
                    {
                    j = splitted.Length;
                    }

                string contentType = string.Concat(splitted.Skip(i + 1).Take(j - i - 1).Select(s => s + " ")).Trim();//[i + 1];
                string contentGet = contentType;
                long SubGroupOfGoodsId = 0;
                contentGet = catalogsCachedData.PropertyTypesCacheObjectsStore.GetPropertyUKValue(0, contentType, SubGroupOfGoodsId, contentPropertyID);// GetContentUkrValue( contentType, contentGet, SubGroupOfGoodsId );
                if (string.IsNullOrEmpty(contentGet))
                    {
                    contentGet = contentType;
                    }
                finalContent += string.Concat(" ", splitted[i], "% ", contentGet);
                i += j - i - 2;
                }
            return finalContent;
            }

        private static string checkSpaceBars(string input)
            {
            var result = new StringBuilder();
            const char spaceBar = ' ';
            const char comma = ',';

            char lastChar = spaceBar;
            foreach (var _char in input)
                {
                if (Char.IsDigit(_char))
                    {
                    if (Char.IsLetter(lastChar))
                        {
                        result.Append(comma);
                        result.Append(spaceBar);
                        }
                    }

                result.Append(_char);
                lastChar = _char;
                }

            return result.ToString();
            }

        private string[] getContentParts(string content)
            {
            content = checkSpaceBars(content);

            content = Regex.Replace(content, @"(\d+[,]\d+)", delegate(Match match)
            {
                string val = match.ToString();
                return val.Replace(',', '.');
            });

            string[] splitted = content.Split(new string[] { " ", "\r", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> parts = new List<string>();
            foreach (string splitPart in splitted)
                {
                var isPercent = splitPart.EndsWith("%");
                int i = 0;
                for (; i < splitPart.Length; i++)
                    {
                    if (!Char.IsDigit(splitPart[i])
                        && (!isPercent || (splitPart[i] != '.' && splitPart[i] != ',')))
                        {
                        break;
                        }
                    }
                string digitPart = splitPart.Substring(0, i);
                string contentPart = splitPart.Substring(i, splitPart.Length - i);
                if (!string.IsNullOrEmpty(digitPart))
                    {
                    parts.Add(digitPart.Replace(',', '.'));
                    }
                if (!string.IsNullOrEmpty(contentPart) && !contentPart.Equals("%"))
                    {
                    parts.Add(contentPart
                        .Replace(",", "")
                        .Replace(";", "").Replace(".", ""));
                    }
                }
            return parts.ToArray();
            }
        }
    }
