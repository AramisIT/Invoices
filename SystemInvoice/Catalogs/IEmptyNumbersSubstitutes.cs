using System;
using System.Collections.Generic;
using SystemInvoice.Catalogs;
using Aramis.Attributes;
using Aramis.DatabaseConnector;
using Aramis.Enums;
using Aramis.Core;
using Aramis.SystemConfigurations;
using Catalogs;
using NPOI.Util.Collections;

namespace Catalogs
    {
    [Catalog(Description = "Замены пустых номеров", GUID = "A2CD96AC-9A8D-4153-8318-460459B298AD", GrantAccessToEveryOne = true, HierarchicType = HierarchicTypes.None, DescriptionSize = 35, ShowCodeFieldInForm = false, ShowCreationDate = true, ShowLastModifiedDate = true)]
    public interface IEmptyNumbersSubstitutes : ICatalog
        {
        [DataField(Description = "Контрагент")]
        IContractor Contractor { get; set; }

        [DataField(Description = "Замена")]
        Table<IEmptyNumberSubstituteRow> Substitute { get; }

        [DataField(Description = "Перечень видов пустых номеров")]
        Table<IEmptyNumberRow> EmptyNumbers { get; }
        }

    public interface IEmptyNumberSubstituteRow : ITableRow
        {
        [DataField(Description = "Замена")]
        string Substitute { get; set; }
        }

    public interface IEmptyNumberRow : ITableRow
        {
        [DataField(Description = "Представление пустого номера")]
        string EmptyNumber { get; set; }
        }

    public class EmptyNumbersSubstitutesBehaviour : Behaviour<IEmptyNumbersSubstitutes>
        {
        public EmptyNumbersSubstitutesBehaviour(IEmptyNumbersSubstitutes item)
            : base(item)
            {
            O.AddPropertyChanged(O.Contractor, () => O.Description = O.Contractor.Description.Substring(0,
                Math.Min(O.Contractor.Description.Length,
                O.ObjInfo.FieldsDictionary[CONSTS.DESCRIPTION_FIELD_NAME].Attr.Size)));

            O.BeforeWriting += O_BeforeWriting;
            }

        void O_BeforeWriting(IDatabaseObject item, IDBObjectWritingOptions writingOptions, ref bool cancel)
            {
            O.GetEmptyNumbersHashSet();
            }
        }

    public static class EmptyNumbersSubstitutesExtentions
        {
        public static HashSet<string> GetEmptyNumbersHashSet(this IEmptyNumbersSubstitutes doc)
            {
            var hashSet = new HashSet<string>();
            for (int rowIndex = doc.EmptyNumbers.RowsCount - 1; rowIndex >= 0; rowIndex--)
                {
                var row = doc.EmptyNumbers[rowIndex];
                row.EmptyNumber = row.EmptyNumber.Trim().ToUpper();

                if (!hashSet.Contains(row.EmptyNumber))
                    {
                    hashSet.Add(row.EmptyNumber);
                    }
                else
                    {
                    row.RemoveFromTable();
                    }
                }

            return hashSet;
            }
        }
    }