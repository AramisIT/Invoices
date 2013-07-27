using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.NomenclaturesCache
    {
    /// <summary>
    /// Использутется для кеширования информации о номенклатурной позиции
    /// </summary>
    public class NomenclatureCacheObject : CacheObject<NomenclatureCacheObject>, INomenclatureSearch
        {
        private string invoiceName;
        private long subGroupOfGoodsId;

        //Поля используемые для проверки уникальности номенклатуры
        public string Article { get; private set; }
        public long ContractorId { get; private set; }
        public long TradeMarkId { get; private set; }
        //Вместе с предыдущими полями формирует список необходимых полей для создания объекта БД
        public long ManufacturerId { get; private set; }
        public long CustomsCodeId { get; private set; }
        public string NameInvoice { get; private set; }
        //Вспомогательные поля
        public long CountryId { get; private set; }
        public long UnitOfMeasureId { get; private set; }
        public string CustomsCodeExtern { get; private set; }
        public string BarCode { get; private set; }
        public double NetWeightFrom { get; private set; }
        public double NetWeightTo { get; private set; }
        public double GrossWeight { get; private set; }
        public double Price { get; private set; }
        public string NameOriginal { get; private set; }
        public string NameDecl { get; private set; }
        public long SubGroupId { get; private set; }
        //Возвращает - имеет ли номенклатура заполненную табличную часть состава
        public int HasContent { get; private set; }

        public NomenclatureCacheObject(string Article, long TradeMarkId, long ContractorId, int hasContent)
            {
            HasContent = hasContent;
            this.Article = Article;
            this.ContractorId = ContractorId;
            this.TradeMarkId = TradeMarkId;
            }

        public NomenclatureCacheObject(string article, long tradeMarkId, long contractorId) :
            this(article, tradeMarkId, contractorId, 0)
            {
            }

        //public NomenclatureCacheObject(string Article, long TradeMarkId, long ContractorId, int hasContent)
        //    : this(Article, TradeMarkId, ContractorId, hasContent)
        //    {
        //    }

        public NomenclatureCacheObject(string Article, long TradeMarkId, long ContractorId, long ManufacturerId, long CustomsCodeId, string InvoiceName, int hasContent)
            : this(Article, TradeMarkId, ContractorId, hasContent)
            {
            this.ManufacturerId = ManufacturerId;
            this.CustomsCodeId = CustomsCodeId;
            this.NameInvoice = InvoiceName;
            }

        public NomenclatureCacheObject(string Article, long TradeMarkId, long ContractorId, long ManufacturerId, long CustomsCodeId, string InvoiceName,
            long CountryId, long UnitOfMeasureId, string CustomsCodeExtern, string BarCode, double NetWeightFrom, double NetWeightTo, double GrossWeight, double Price,
            string NameOriginal, string NameDecl, long SubGroupId, int hasContent)
            : this(Article, TradeMarkId, ContractorId, ManufacturerId, CustomsCodeId, InvoiceName, hasContent)
            {
            this.CountryId = CountryId;
            this.UnitOfMeasureId = UnitOfMeasureId;
            this.CustomsCodeExtern = CustomsCodeExtern;
            this.BarCode = BarCode;
            this.NetWeightFrom = NetWeightFrom;
            this.NetWeightTo = NetWeightTo;
            this.GrossWeight = GrossWeight;
            this.Price = Price;
            this.NameOriginal = NameOriginal;
            this.NameDecl = NameDecl;
            this.SubGroupId = SubGroupId;
            }


        //public NomenclatureCacheObject(string article, long tradeMarkId, long contractorId, long manufacturerId, long customsCodeId, string invoiceName,
        //    long countryId, long unitOfMeasureId, string customsCodeExtern, string barCode, double netWeightFrom, double netWeightTo, double grossWeight,
        //    double price, string nameOriginal, string nameDecl, long subGroupOfGoodsId)
        //    : this(article, tradeMarkId, contractorId, manufacturerId, customsCodeId, invoiceName, countryId, unitOfMeasureId, customsCodeExtern, barCode,
        //    netWeightFrom, netWeightTo, grossWeight, price, nameOriginal, nameDecl, subGroupOfGoodsId)
        //    {
        //    }

        public bool IsValidForCreation()
            {
            return !string.IsNullOrEmpty(Article) && !string.IsNullOrEmpty(NameInvoice) && TradeMarkId > 0 && ContractorId > 0 && ManufacturerId > 0 && CustomsCodeId > 0;
            }


        protected override bool equals(NomenclatureCacheObject other)
            {
            return ContractorId == other.ContractorId && TradeMarkId == other.TradeMarkId && Article.Equals(other.Article);
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new object[] { ContractorId, TradeMarkId, Article };
            }


        #region INomenclatureSearch реализация
        void INomenclatureSearch.SetSearchOptions(string Article, long TradeMarkId, long ContractorId)
            {
            this.Article = Article;
            this.TradeMarkId = TradeMarkId;
            this.ContractorId = ContractorId;
            refreshHash();
            }
        #endregion


        protected override int calcHash()
            {
            return ContractorId.GetHashCode() ^ TradeMarkId.GetHashCode() ^ Article.GetHashCode();
            }
        }
    }
