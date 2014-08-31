using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.ApprovalsCache
    {
    /// <summary>
    /// Используется для кеширования ТАБЛИЧНОЙ ЧАСТИ РД. Хранит в себе номенклатуру (из строки в табличной части РД) и данные из шапки РД. Поиск осуществляется по полям - 
    /// Тип документа, контрагент, торговая марка, дата с, дата по, номенклатура, номер документа. Непосредственно используется в механизме создания новых РД из отредактированного
    /// документа табличной части инвойса, при поиске существующих записей в РД, или новых РД
    /// </summary>
    public class ApprovalsCacheObject : CacheObject<ApprovalsCacheObject>
        {
        public long ApprovalsId { get; private set; }
        public long NomenclatureId { get; private set; }
        public string DocumentBaseNumber { get; set; }
        public string DocumentNumber { get; private set; }
        public string DocumentTypeName { get; private set; }
        public string DocumentCodeName { get; private set; }
        public string ContractorName { get; private set; }
        public string TradeMarkName { get; private set; }
        public long DocumentTypeId { get; private set; }
        public long ContractorId { get; private set; }
        public long TradeMarkId { get; private set; }
        public DateTime DateFrom { get; private set; }
        public DateTime DateTo { get; private set; }
        public DateTime SearchedDate { get; private set; }

        public ApprovalsCacheObject(string DocumentNumber, string DocumentTypeName, string DocumentCodeName, string ContractorName, string TradeMarkName, long DocumentTypeId, long ContractorId, long TradeMarkId, DateTime DateFrom, DateTime DateTo, DateTime searchedDate, long approvalsId, long nomenclatureId, string documentBaseNumber)
            {
            this.DocumentBaseNumber = documentBaseNumber;
            this.DocumentNumber = DocumentNumber;
            this.DocumentTypeName = DocumentTypeName;
            this.DocumentCodeName = DocumentCodeName;
            this.ContractorName = ContractorName;
            this.TradeMarkName = TradeMarkName;
            this.DocumentTypeId = DocumentTypeId;
            this.ContractorId = ContractorId;
            this.TradeMarkId = TradeMarkId;
            this.DateFrom = DateFrom;
            this.DateTo = DateTo;
            this.ApprovalsId = approvalsId;
            this.NomenclatureId = nomenclatureId;
            this.SearchedDate = searchedDate;
            }

        public ApprovalsCacheObject(string DocumentNumber, string documetCodeName, long DocumentTypeId, long ContractorId, long TradeMarkId, DateTime DateFrom, DateTime DateTo, DateTime searchedDate, long nomenclatureId)
            : this(DocumentNumber, string.Empty, documetCodeName, string.Empty, string.Empty, DocumentTypeId, ContractorId, TradeMarkId, DateFrom, DateTo, searchedDate, 0, nomenclatureId, string.Empty)
            {
            }

        protected override bool equals(ApprovalsCacheObject other)
            {
            return this.DocumentTypeId.Equals(other.DocumentTypeId) && this.ContractorId.Equals(other.ContractorId) && this.TradeMarkId.Equals(other.TradeMarkId) &&
                this.DateFrom.Equals(other.DateFrom) && this.DateTo.Equals(other.DateTo) && this.NomenclatureId.Equals(other.NomenclatureId) && this.DocumentNumber.Equals(other.DocumentNumber);
            }

        protected override object[] getForCacheCalculatedObjects()
            {
            return new object[] { DocumentTypeId, ContractorId, TradeMarkId, DateFrom, DateTo, NomenclatureId, DocumentNumber };
            }
        }
    }
