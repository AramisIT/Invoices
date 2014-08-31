using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.DataProcessing.Cache;
using System.Data;
using SystemInvoice.Documents;
using SystemInvoice.Excel;

namespace SystemInvoice.DataProcessing.InvoiceProcessing.LoadedDocumentChecking.RDChecking
    {
    /// <summary>
    /// Проверяет номенклатуру на предмет необходимости наличия разрешительных документов
    /// </summary>
    public class RDChecker : LoadedDocumentCheckerBase
        {
        private const string foundedApprovalsPrefix = "FoundedApprovals";
        private const string compareGenderStrFirst = "ХЛОП";
        private const string compareGenderStrSecond = "ДІВЧ";
        private const string foundedApprovalsColumnNamePrefix = "FoundedApprovals";
        private readonly string nomenclatureFoundedColumnName;

        private List<string> RDDocTypeCodeColumnNames = new List<string>();
        private List<string> RDDateFromColumnNames = new List<string>();
        private List<string> RDDateToColumnNames = new List<string>();
        private List<string> RDDocNumberColumnNames = new List<string>();
        private List<string> RDBaseNumberToColumnNames = new List<string>();

        public static void InitColumnsNames(List<string> _RDDocTypeCodeColumnNames, List<string> _RDDateFromColumnNames,
            List<string> _RDDateToColumnNames, List<string> _RDDocNumberColumnNames, List<string> _RDBaseNumberToColumnNames)
            {
            _RDDocTypeCodeColumnNames.Clear();
            _RDDocTypeCodeColumnNames.Add(InvoiceColumnNames.RDCode1.ToString());
            _RDDocTypeCodeColumnNames.Add(InvoiceColumnNames.RDCode2.ToString());
            _RDDocTypeCodeColumnNames.Add(InvoiceColumnNames.RDCode3.ToString());
            _RDDocTypeCodeColumnNames.Add(InvoiceColumnNames.RDCode4.ToString());
            _RDDocTypeCodeColumnNames.Add(InvoiceColumnNames.RDCode5.ToString());

            _RDDateFromColumnNames.Clear();
            _RDDateFromColumnNames.Add(InvoiceColumnNames.RDFromDate1.ToString());
            _RDDateFromColumnNames.Add(InvoiceColumnNames.RDFromDate2.ToString());
            _RDDateFromColumnNames.Add(InvoiceColumnNames.RDFromDate3.ToString());
            _RDDateFromColumnNames.Add(InvoiceColumnNames.RDFromDate4.ToString());
            _RDDateFromColumnNames.Add(InvoiceColumnNames.RDFromDate5.ToString());

            _RDDateToColumnNames.Clear();
            _RDDateToColumnNames.Add(InvoiceColumnNames.RDToDate1.ToString());
            _RDDateToColumnNames.Add(InvoiceColumnNames.RDToDate2.ToString());
            _RDDateToColumnNames.Add(InvoiceColumnNames.RDToDate3.ToString());
            _RDDateToColumnNames.Add(InvoiceColumnNames.RDToDate4.ToString());
            _RDDateToColumnNames.Add(InvoiceColumnNames.RDToDate5.ToString());

            _RDDocNumberColumnNames.Clear();
            _RDDocNumberColumnNames.Add(InvoiceColumnNames.RDNumber1.ToString());
            _RDDocNumberColumnNames.Add(InvoiceColumnNames.RDNumber2.ToString());
            _RDDocNumberColumnNames.Add(InvoiceColumnNames.RDNumber3.ToString());
            _RDDocNumberColumnNames.Add(InvoiceColumnNames.RDNumber4.ToString());
            _RDDocNumberColumnNames.Add(InvoiceColumnNames.RDNumber5.ToString());

            _RDBaseNumberToColumnNames.Clear();
            _RDBaseNumberToColumnNames.Add(InvoiceColumnNames.RD1BaseNumber.ToString());
            _RDBaseNumberToColumnNames.Add(InvoiceColumnNames.RD2BaseNumber.ToString());
            _RDBaseNumberToColumnNames.Add(InvoiceColumnNames.RD3BaseNumber.ToString());
            _RDBaseNumberToColumnNames.Add(InvoiceColumnNames.RD4BaseNumber.ToString());
            _RDBaseNumberToColumnNames.Add(InvoiceColumnNames.RD5BaseNumber.ToString());
            }
        private void initColumnNames()
            {
            RDChecker.InitColumnsNames(RDDocTypeCodeColumnNames, RDDateFromColumnNames, RDDateToColumnNames, RDDocNumberColumnNames, RDBaseNumberToColumnNames);
            }

        public RDChecker(SystemInvoiceDBCache dbCache)
            : base(dbCache)
            {
            nomenclatureFoundedColumnName = ProcessingConsts.ColumnNames.FOUNDED_NOMENCLATURE_COLUMN_NAME;
            initColumnNames();
            }

        protected override void CheckRow(DataRow rowToCheck, ExcelMapper mapper, bool isDocumentCurrentlyLoaded, string currentCheckedColumnName)
            {
            if (rowToCheck == null)
                {
                return;
                }
            RDCheckError currentError = null;
            if ((currentError = this.checkCustomsCodes(rowToCheck)) != null)
                {
                this.addErrorToApprovalsColumns(currentError, 0);
                }
            if ((currentError = this.checkGender(rowToCheck)) != null)
                {
                this.addErrorToApprovalsColumns(currentError, 0);
                }
            this.checkDeletedApprovals(rowToCheck);
            }

        /// <summary>
        /// Добавляет ошибку ко всем ячейкам связанным с определенным РД
        /// </summary>
        /// <param name="currentError">Ошибка</param>
        /// <param name="approvalErrorsIndex">Индекс разрешительного в таблице инвойса куда нужно добавить ошибку</param>
        private void addErrorToApprovalsColumns(RDCheckError currentError, int approvalErrorsIndex)
            {
            this.AddError(RDDateFromColumnNames[approvalErrorsIndex], currentError);
            this.AddError(RDBaseNumberToColumnNames[approvalErrorsIndex], currentError);
            this.AddError(RDDateToColumnNames[approvalErrorsIndex], currentError);
            this.AddError(RDDocNumberColumnNames[approvalErrorsIndex], currentError);
            this.AddError(RDDocTypeCodeColumnNames[approvalErrorsIndex], currentError);
            }

        /// <summary>
        /// Проверяет необходимость нового создания разрешительных после автоматического удаления номенклатуры из РД
        /// </summary>
        private void checkDeletedApprovals(DataRow rowToCheck)
            {
            List<RDCheckError> requiredNomenclatureErrors = getApprovalsDeletedErrors(rowToCheck);
            fillApprovalsDeletedErrors(rowToCheck, requiredNomenclatureErrors);
            }

        private List<RDCheckError> getApprovalsDeletedErrors(DataRow rowToCheck)
            {
            List<RDCheckError> requiredNomenclatureErrors = new List<RDCheckError>();
            long nomenclatureId = Helpers.InvoiceDataRetrieveHelper.GetRowNomenclatureId(rowToCheck);
            DateTime invoiceDate = Helpers.InvoiceDataRetrieveHelper.GetRowInvoiceDate(rowToCheck);
            foreach (long documentTypeId in dbCache.NomenclatureRemovingHistoryCacheObjectsStore.RequiredApprovalTypes(nomenclatureId, invoiceDate))
                {
                var requredType = dbCache.DocumentTypesCacheObjectsStore.GetCachedObject(documentTypeId);
                if (requredType != null)
                    {
                    requiredNomenclatureErrors.Add(
                        new RDCheckError(string.Format("Необходимо создать РД с кодом:{0}", requredType.DocumentTypeCode)));
                    }
                }
            return requiredNomenclatureErrors;
            }

        private void fillApprovalsDeletedErrors(DataRow rowToCheck, List<RDCheckError> requiredNomenclatureErrors)
            {
            int currentErrorIndex = 0;
            for (int i = 0; i < ProcessingConsts.CHECKING_APPROVALS_COUNT && currentErrorIndex < requiredNomenclatureErrors.Count; i++)
                {
                if (!isApprovalExists(rowToCheck, i + 1))
                    {
                    this.addErrorToApprovalsColumns(requiredNomenclatureErrors[currentErrorIndex++], i);
                    }
                }
            }

        /// <summary>
        /// Проверяет необходмость наличия хотя бы одного РД для пола
        /// </summary>
        private RDCheckError checkGender(DataRow rowToCheck)
            {
            string genderValue = rowToCheck.TrySafeGetColumnValue<string>(Documents.InvoiceColumnNames.Gender.ToString(), string.Empty).ToUpper();
            if (genderValue.Contains(compareGenderStrFirst) || genderValue.Contains(compareGenderStrSecond) && !isApprovalsExists(rowToCheck))
                {
                return new RDCheckError("Необходим РД для пола.");
                }
            return null;
            }

        /// <summary>
        /// Проверяет необходимость наличия хотя бы одного РД для данного таможенного кода
        /// </summary>
        private RDCheckError checkCustomsCodes(DataRow rowToCheck)
            {
            if (dbCache.TradeMarkContractorSource == null)
                {
                return null;
                }
            string customsCode = Helpers.InvoiceDataRetrieveHelper.GetRowCustomsCode(rowToCheck);
            long nomenclatureId = Helpers.InvoiceDataRetrieveHelper.GetRowNomenclatureId(rowToCheck);
            var cachedObject = dbCache.CustomsCodesCacheStore.GetCustomsCodeForCodeName(customsCode);
            if (cachedObject != null && cachedObject.IsApprovalsRequired && nomenclatureId > 0 && !isApprovalsExists(rowToCheck))
                {
                return new RDCheckError("Необходим РД для данного таможенного кода.");
                }
            return null;
            }

        private bool isApprovalsExists(DataRow rowToCheck)
            {
            for (int i = 1; i <= ProcessingConsts.CHECKING_APPROVALS_COUNT; i++)
                {
                if (isApprovalExists(rowToCheck, i))
                    {
                    return true;
                    }
                }
            return false;
            }

        private bool isApprovalExists(DataRow rowToCheck, int i)
            {
            string foundedApprovalColumnName = string.Concat(foundedApprovalsColumnNamePrefix, i);
            long approvalId = rowToCheck.TrySafeGetColumnValue<long>(foundedApprovalColumnName, 0);
            if (approvalId > 0)
                {
                return true;
                }
            return false;
            }
        }
    }
