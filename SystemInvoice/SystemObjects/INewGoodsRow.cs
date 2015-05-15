using Aramis.Attributes;
using Aramis.Core;
using Aramis.DatabaseConnector;
using System;
using SystemInvoice.Catalogs;
using SystemInvoice.Documents;
using AramisInfrastructure.UI;

namespace SystemInvoice.SystemObjects
    {
    public interface INewGoodsRow : IAramisModel
        {
        [DataField(NotEmpty = true, Description = "Инвойс", Size = Invoice.MAX_INVOICE_NUMBER_SIZE + 1)]
        string InvoiceNumber { get; set; }

        [DataField(NotEmpty = true, Description = "Контрагент")]
        IContractor Contractor { get; set; }

        [DataField(NotEmpty = true, Description = "Дата")]
        DateTime InvoiceDate { get; set; }

        [DataField(NotEmpty = true, Description = "Торговая марка", SelectingType = TradeMarkListsGetter.TradeMarkListsTypes.FilteredByContractor)]
        ITradeMark TradeMark { get; set; }

        [DataField(NotEmpty = true, Description = "Производитель", SelectingType = ManufacturerListsGetter.ManufacturerListsTypes.FilteredByContractor)]
        IManufacturer ItemProducer { get; set; }

        [DataField(NotEmpty = true, Description = "УКТЗЕД")]
        CustomsCode InternalCode { get; set; }

        [DataField(Description = "Номенклатура")]
        Nomenclature Nomenclature { get; set; }

        [DataField(NotEmpty = true, Description = "Артикул")]
        string Article { get; set; }

        [DataField(Size = 600)]
        string NameDecl { get; set; }

        [DataField(NotEmpty = true, Description = "Назва", Size = 400)]
        string NameInvoice { get; set; }

        [DataField(DecimalPointsNumber = 3)]
        decimal Price { get; set; }

        [DataField(DecimalPointsNumber = 2)]
        decimal Amount { get; set; }

        [DataField(DecimalPointsNumber = 2)]
        decimal Sum { get; set; }

        [DataField(DecimalPointsNumber = 4)]
        decimal Net { get; set; }

        [DataField(DecimalPointsNumber = 4)]
        decimal Gross { get; set; }

        [DataField(DecimalPointsNumber = 4)]
        decimal GrossPerUnit { get; set; }

        [DataField(DecimalPointsNumber = 4)]
        decimal NetPerUnit { get; set; }

        int PlacesCount { get; set; }

        [DataField(NotEmpty = true, Description = "Страна")]
        Country Country { get; set; }

        [DataField(NotEmpty = true, Description = "Ед измерения")]
        UnitOfMeasure UnitOfMeasure { get; set; }

        Table<IArticleRow> SearchRows { get; }
        }

    public interface IArticleRow : ITableRow
        {
        [DataField(Description = "Артикул", ReadOnly = true)]
        string Article { get; set; }

        [DataField(Description = "Найменування", ReadOnly = true)]
        string Description { get; set; }

        [DataField(ShowInForm = false)]
        long Id { get; set; }
        }

    public class NewGoodsRowBehaviour : Behaviour<INewGoodsRow>
        {
        public NewGoodsRowBehaviour(INewGoodsRow item)
            : base(item)
            {
            O.AddPropertyChanged(O.Price, () => updateSum());
            O.AddPropertyChanged(O.Amount, () => updateSum());
            O.AddPropertyChanged(O.GrossPerUnit, () => updateSum());
            O.AddPropertyChanged(O.NetPerUnit, () => updateSum());
            }

        private void updateSum()
            {
            O.Sum = O.Price * O.Amount;
            O.Net = O.NetPerUnit * O.Amount;
            O.Gross = O.GrossPerUnit * O.Amount;
            O.PlacesCount = (int)O.Amount;
            }

        public override void InitItemBeforeShowing(IItemViewModeParameters viewModeParameters)
            {
            var q = DB.NewQuery(@"select rtrim(n.Article) Article, rtrim(n.Description) [Description], n.Id
	from Nomenclature n
	where n.Contractor = @Contractor	
	order by n.Article");
            q.AddInputParameter("Contractor", O.Contractor.Id);
            O.FillTable(O.SearchRows, q);
            }
        }


    }
