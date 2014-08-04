using Aramis.Attributes;
using Aramis.Core;
using Aramis.DatabaseConnector;
using System;
using SystemInvoice.Catalogs;
using SystemInvoice.Documents;

namespace SystemInvoice.SystemObjects
    {
    public interface INewGoodsRow : IAramisModel
        {
        [DataField(Size = Invoice.MAX_INVOICE_NUMBER_SIZE + 1)]
        string InvoiceNumber { get; set; }

        IContractor Contractor { get; set; }

        DateTime InvoiceDate { get; set; }

        ITradeMark TradeMark { get; set; }

        Manufacturer ItemProducer { get; set; }

        CustomsCode InternalCode { get; set; }

        Nomenclature Nomenclature { get; set; }

        string Article { get; set; }

        [DataField(Size = 600)]
        string NameDecl { get; set; }

        [DataField(Size = 400)]
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
        decimal NetPerUnit { get; set; }

        int PlacesCount { get; set; }

        Country Country { get; set; }

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

            }

        public override void InitItemBeforeShowing()
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
