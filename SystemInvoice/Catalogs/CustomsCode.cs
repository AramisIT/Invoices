﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Core;
using Aramis.Attributes;
using Aramis.Enums;
using System.Data;
using System.Drawing;

namespace SystemInvoice.Catalogs
    {
    /// <summary>
    /// Справочник. Содержит описание таможенного кода
    /// </summary>
    [Catalog(Description = "Таможенные коды", GUID = "AD8C0B21-B521-4CD0-8F98-5B6564B9A604", DescriptionSize = 50, DescriptionUnique = true, ShowCodeFieldInForm = false, HierarchicType = HierarchicTypes.Elements)]
    public class CustomsCode : CatalogTable
        {
        public const string emptyCodeStr = "Отсутствует";

        #region (string) CodeDescription Описание
        [DataField(Description = "Описание", Size = 1000, ShowInList = true)]
        public string CodeDescription
            {
            get
                {
                return z_CodeDescription;
                }
            set
                {
                if (z_CodeDescription == value)
                    {
                    return;
                    }

                z_CodeDescription = value;
                NotifyPropertyChanged("CodeDescription");
                }
            }
        private string z_CodeDescription = "";
        #endregion


        #region (bool) IsApprovalsRequired Необходим разрешительный
        [DataField(Description = "На все товары необходим разрешительный документ")]
        public bool IsApprovalsRequired
            {
            get
                {
                return z_IsApprovalsRequired;
                }
            set
                {
                if (z_IsApprovalsRequired == value)
                    {
                    return;
                    }

                z_IsApprovalsRequired = value;
                NotifyPropertyChanged("IsApprovalsRequired");
                }
            }
        private bool z_IsApprovalsRequired = false;
        #endregion

        public override WritingResult Write()
            {
            if (string.IsNullOrEmpty(this.Description))
                {
                this.Description = emptyCodeStr;
                }
            return base.Write();
            }

        public override Func<System.Data.DataRow, System.Drawing.Color> GetFuncGetRowColor()
            {
            return (row) =>
                {
                    if (row != null && row["Description"] != null && row["Description"] != DBNull.Value)
                        {
                        if (((string)row["Description"]).Equals(emptyCodeStr))
                            {
                            return Color.FromArgb(255, 139, 139);// Color.Red;
                            }
                        }
                    return Color.White;
                };
            }
        }
    }