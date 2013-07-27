using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisWpfComponents.Excel;
using System.IO;

namespace SystemInvoice.Excel
    {
    public class ExcelHelper
        {
        public const char ConstantDelimeter = '"';
        public static string TransformExpression( string expression, Dictionary<string,string> allowedColumns )
            {
            List<string> parts = new List<string>();
            int inputLength = expression.Length;
            bool isCurrentDigit = false;
            bool isCurrentConstant = false;
            int digitToCheck = 0;
            List<char> buffer = new List<char>();
            for (int i = 0; i < inputLength; i++)
                {
                char current = expression[i];
                if (current == ConstantDelimeter)
                    {
                    if (!isCurrentConstant)
                        {
                        if (buffer.Count > 0)
                            {
                            parts.Add( new string( buffer.ToArray() ) );
                            buffer.Clear();
                            }
                        buffer.Add( current );
                        }
                    else
                        {
                        buffer.Add( current );
                        parts.Add( new string( buffer.ToArray() ) );
                        buffer.Clear();
                        }
                    isCurrentConstant = !isCurrentConstant;
                    continue;
                    }
                if (isCurrentConstant)
                    {
                    buffer.Add( current );
                    continue;
                    }
                bool isDigit = int.TryParse( current.ToString(), out digitToCheck );
                if ((isDigit && isCurrentDigit) || (!isDigit && !isCurrentDigit))
                    {
                    buffer.Add( current );
                    }
                else
                    {
                    if (buffer.Count > 0)
                        {
                        parts.Add( new string( buffer.ToArray() ) );
                        buffer.Clear();
                        }
                    buffer.Add( current );
                    isCurrentDigit = !isCurrentDigit;
                    }
                }
            if (buffer.Count > 0)
                {
                parts.Add( new string( buffer.ToArray() ) );
                buffer.Clear();
                }
            List<string> finalParts = new List<string>();
         //   List<string> orderedByMax = allowedColumns.Keys.Where( itm => expression.IndexOf( itm ) != -1 ).OrderBy( itm => itm.Length ).ToList();
            foreach (string part in parts)
                {
                if (part[0] == ConstantDelimeter || int.TryParse( part, out digitToCheck ))
                    {
                    finalParts.Add( part );
                    continue;
                    }
                finalParts.AddRange( getParts( part, allowedColumns ) );
                }
            string preparedExpression = string.Concat( finalParts.Select( ( part, i ) => i == 0 ? part : "+" + part ) );
            return preparedExpression;
            }

        private static string[] getParts( string part, IDictionary<string,string> delimeters )
            {
            part = string.Concat( ConstantDelimeter, part, ConstantDelimeter );
            foreach (KeyValuePair<string,string> delimeterPair in delimeters)
                {
                part = part.Replace( delimeterPair.Key, string.Concat( ConstantDelimeter, delimeterPair.Value, ConstantDelimeter ) );
                }
            return DivideString( part );
            }

        public static string[] DivideString( string toDivide )
            {
            List<string> parts = new List<string>();
            int inputLength = toDivide.Length;
            bool isCurrentConstant = false;
            List<char> buffer = new List<char>();
            for (int i = 0; i < inputLength; i++)
                {
                char current = toDivide[i];
                if (current == ConstantDelimeter)
                    {
                    if (!isCurrentConstant)
                        {
                        if (buffer.Count > 0)
                            {
                            parts.Add( new string( buffer.ToArray() ) );
                            buffer.Clear();
                            }
                        buffer.Add( current );
                        }
                    else
                        {
                        buffer.Add( current );
                        parts.Add( new string( buffer.ToArray() ) );
                        buffer.Clear();
                        }
                    isCurrentConstant = !isCurrentConstant;
                    continue;
                    }
                buffer.Add( current );
                }
            if (buffer.Count > 0)
                {
                parts.Add( new string( buffer.ToArray() ) );
                buffer.Clear();
                }
            return parts.ToArray();
            }

        public static bool tryLoad( string fileName, out ExcelXlsWorkbook book )
            {
            book = null;
            try
                {
                book = new ExcelXlsWorkbook( File.Open( fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) );
                }
            catch (NPOI.HSSF.OldExcelFormatException oldFormatException)
                {
                string messageToGet = @"The supplied spreadsheet seems to be Excel 5.0/7.0 (BIFF5) format. POI only supports BIFF8 format (from Excel versions 97/2000/XP/2003)";
                string message = oldFormatException.Message;
                string messageToShow = "Данный файл имеет устаревший формат Excel 5.0/7.0 (BIFF5), который не поддерживается. Поддерживаются формат BIFF8 (xls) (Excel 97/2000/XP/2003).";
                if (!message.Equals( messageToGet ))
                    {
                    messageToShow = message;
                    }
                messageToShow.AlertBox();
                return false;
                }
            catch (NPOI.POIFS.FileSystem.OfficeXmlFileException officeXMLException)
                {
                string messageToGet = "The supplied data appears to be in the Office 2007+ XML. POI only supports OLE2 Office documents";
                string message = officeXMLException.Message;
                string messageToShow = "Данный файл имеет формат Office 2007+ XML, который не поддерживается. Поддерживаются формат BIFF8 (xls) (Excel 97/2000/XP/2003).";
                if (!message.Equals( messageToGet ))
                    {
                    messageToShow = message;
                    }
                messageToShow.AlertBox();
                return false;
                }
            catch (System.IO.FileNotFoundException)
                {
                string message = "Файл не найден.";
                message.AlertBox();
                return false;
                }
            catch (System.IO.IOException)
                {
                string message = "Ошибка при чтении файла";
                message.AlertBox();
                return false;
                }
            if (book == null)
                {
                return false;
                }
            return true;
            }

        }
    }
