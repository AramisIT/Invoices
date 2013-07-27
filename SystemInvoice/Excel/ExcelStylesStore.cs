using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisWpfComponents.Excel;

namespace SystemInvoice.Excel
    {
    /// <summary>
    /// Хранилище, используется для сохранения и повторного использования Excel - стилей соответствующих определенному цвету
    /// </summary>
    public class ExcelStylesStore
        {
        /// <summary>
        /// Книга Excel которой принадлежат стили
        /// </summary>
        ExcelXlsWorkbook workBook = null;
        /// <summary>
        /// Хранилище стилей
        /// </summary>
        private Dictionary<string, ExcelStyle> stylesDict = new Dictionary<string, ExcelStyle>();

        public ExcelStylesStore( ExcelXlsWorkbook workBook )
            {
            this.workBook = workBook;
            }
        /// <summary>
        /// Возвращает Excel - стиль, с фоном ячейки имеющим соответствующий цвет
        /// </summary>
        /// <param name="backColor">Цвет</param>
        /// <returns>Стиль</returns>
        public ExcelStyle GetStyle( string backColor )
            {
            if (stylesDict.ContainsKey( backColor ))
                {
                return stylesDict[backColor];
                }
            ExcelStyle newStyle = new ExcelStyle( workBook );
            newStyle.Border.Color = (System.Drawing.Color)(new System.Drawing.ColorConverter().ConvertFromString( "#A5A5A5" ));
            newStyle.Border.Sides = BorderSides.All;
            object colorVal = new System.Drawing.ColorConverter().ConvertFromString( backColor );
            if (colorVal != null)
                {
                newStyle.Interior.Color = (System.Drawing.Color)colorVal;
                }
            newStyle.Alignment.Horizontal = HorizontalAlignment.Left;
            stylesDict.Add( backColor, newStyle );
            return newStyle;
            }
        }
    }
