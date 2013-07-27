﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemInvoice.Documents;
using SystemInvoice.Catalogs;
using System.Data;

namespace SystemInvoice.DataProcessing.Cache.ApprovalsCache
    {
    /// <summary>
    /// Создает новые  РД или добавляет строки с Номенклатурой в табличную часть существующих РД.
    /// </summary>
    public class ApprovalsObjectCreator : DbObjectCreator<Approvals, ApprovalsCacheObject>
        {
        private ApprovalsCacheObjectsStore approvalsCacheObjectsStore = null;

        /// <summary>
        /// Словарь для поиска созданных айдишников номенклатуры для РД, которые будут удалены из табличной части РД, в случае если будет откат изменений
        /// </summary>
        private Dictionary<Approvals, List<long>> createdNomenclaturesRows = new Dictionary<Approvals, List<long>>();

        /// <summary>
        /// Список РД которые должны быть только обновлены, это значит, что в случае ошибки, когда пользователь решит откатить добавления РД, эти РД не будут удалены, 
        /// будут только удалены новые строки из табличной части для этих РД.
        /// </summary>
        private List<Approvals> updatedApprovals = new List<Approvals>();
        /// <summary>
        /// Хранит в себе список номенклатуры которая добавляется в существующий РД (ключь)
        /// </summary>
        Dictionary<ApprovalsCacheObject, List<long>> forExistedApprovalsNomenclatureLists = new Dictionary<ApprovalsCacheObject, List<long>>();
        /// <summary>
        /// Хранит в себе список номенклатуры которая создается с табличной частью нового РД (ключь)
        /// </summary>
        Dictionary<ApprovalsCacheObject, List<long>> forNewAprovalsNomenclatureLists = new Dictionary<ApprovalsCacheObject, List<long>>();

        public ApprovalsObjectCreator(ApprovalsCacheObjectsStore approvalsCacheObjectsStore)
            : base(approvalsCacheObjectsStore)
            {
            this.approvalsCacheObjectsStore = approvalsCacheObjectsStore;
            }


        protected override Approvals createDBObject(ApprovalsCacheObject cacheObject)
            {
            List<long> nomenclatures = null;//список номенклатуры которая в конечном итоге должна быть сформирована в табличной части РД
            Approvals approvals = new Approvals();
            if (cacheObject.ApprovalsId != 0)//добавляем строку в существующий РД
                {
                approvals.Id = cacheObject.ApprovalsId;
                approvals.Read();//считываем существующие строки в табличной части РД
                forExistedApprovalsNomenclatureLists.TryGetValue(cacheObject, out nomenclatures);//получаем список номенклатуры для существующего РД
                updatedApprovals.Add(approvals);
                }
            else//создаем новый РД
                {
                Contractor contractor = new Contractor();
                contractor.Id = cacheObject.ContractorId;
                DocumentType docType = new DocumentType();
                docType.Id = cacheObject.DocumentTypeId;
                approvals.Contractor = contractor;
                approvals.DocumentType = docType;
                approvals.DocumentNumber = cacheObject.DocumentNumber;
                approvals.DateFrom = cacheObject.DateFrom;
                approvals.DateTo = cacheObject.DateTo == DateTime.MinValue ? new DateTime(DateTime.Now.Year + 1, 1, 1) : cacheObject.DateTo;
                approvals.DocumentCode = cacheObject.DocumentTypeId == 0 ? string.Empty : cacheObject.DocumentCodeName;
                forNewAprovalsNomenclatureLists.TryGetValue(cacheObject, out nomenclatures);
                }
            if (nomenclatures != null && nomenclatures.Count > 0)
                {
                createdNomenclaturesRows.Add(approvals, nomenclatures);//добавляем в словарь для удаляемых строк в табличной части РД
                }
            //Создаем полностью табличную часть для РД которая затем будет записана в базу
            foreach (long nomenclatureId in nomenclatures)
                {
                Nomenclature nomenclature = new Nomenclature();
                nomenclature.Id = nomenclatureId;
                DataRow row = approvals.Nomenclatures.NewRow();
                row[approvals.ItemNomenclature] = nomenclature.Id;
                approvals.Nomenclatures.Rows.Add(row);
                approvals.NotifyTableRowAdded(approvals.Nomenclatures, row);
                }
           // approvals.UpdateLocalValuesOfTablePart();
            int startLineNumber = 1;
            foreach (DataRow row in approvals.Nomenclatures.Rows)
                {
                row["LineNumber"] = startLineNumber++;
                }
            return approvals;
            }

        protected override void deleteObject(Approvals objectToDelete)
            {
            string queryToDelete = "";
            if (createdNomenclaturesRows.ContainsKey(objectToDelete))//удаляем новые РД
                {
                queryToDelete = string.Concat(createdNomenclaturesRows[objectToDelete].
                    Select(id => string.Format("delete from SubApprovalsNomenclatures where IdDoc = {0} and  ItemNomenclature = {1} ; {2}", objectToDelete.Id, id, Environment.NewLine)));
                }
            if (!updatedApprovals.Contains(objectToDelete))//удаляем записи из табличной части РД
                {
                queryToDelete += string.Format("delete from Approvals where Id = {0};", objectToDelete.Id);
                }
            base.ExceuteQuery(queryToDelete);
            }

        protected override string failToCreateMessage(int failCount)
            {
            string failMessage = string.Format("Ошибка при создании/обновлении РД. Добавление {0} элементов завершилось неудачей. ", failCount);
            return failMessage;
            }

        /// <summary>
        /// Обновляет РД
        /// </summary>
        public bool TryUpdateApprovalsCatalog(HashSet<ApprovalsCacheObject> approvalsRows)
            {
            createdNomenclaturesRows.Clear();
            updatedApprovals.Clear();
            forExistedApprovalsNomenclatureLists.Clear();
            forNewAprovalsNomenclatureLists.Clear();
            //Заполняем словари которые хранят списки номенклатуры для каждого нового/существующего РД
            //при этом у нас в словарях каждый РД будет встречатся только один раз (поскольку мы сравниваем шапку РД), соответственно при записи в БД нескольких новых номенклатур
            //для одного РД, мы потом выполним запись только один раз заполнив табличную часть для нового/существующего РД
            foreach (ApprovalsCacheObject approvalRow in approvalsRows)
                {
                long fakeId = approvalsCacheObjectsStore.GetApprovalsFakeId(approvalRow);
                if (fakeId > 0)
                    {
                    addToUpdateExistedApprovalsList(approvalRow);
                    }
                else
                    {
                    addToCreateNewApprovalsList(approvalRow);
                    }
                }
            //Добавляем в список разрешительных для записи в БД уже существующие РД
            foreach (ApprovalsCacheObject toUpdate in forExistedApprovalsNomenclatureLists.Keys)
                {
                if (!TryAddToCreationList(toUpdate))
                    {
                    return false;
                    }
                }
            //Добавляем в список разрешительных для записи в БД новые РД
            foreach (ApprovalsCacheObject toCreate in forNewAprovalsNomenclatureLists.Keys)
                {
                if (!TryAddToCreationList(toCreate))
                    {
                    return false;
                    }
                }
            return this.TryCreate();
            }

        protected override bool CheckCanAddItem(ApprovalsCacheObject newItem)
            {
            return true;
            }

        /// <summary>
        /// Добавляет номенклатуру в словарь номенклатур для новых РД
        /// </summary>
        private void addToCreateNewApprovalsList(ApprovalsCacheObject approvalRow)
            {
            if (approvalRow.NomenclatureId == 0)
                {
                return;
                }
            ApprovalsCacheObject withoutNomenclatureAndTradeMarkObject = new ApprovalsCacheObject(approvalRow.DocumentNumber, approvalRow.DocumentCodeName, approvalRow.DocumentTypeId, approvalRow.ContractorId, 0, approvalRow.DateFrom, approvalRow.DateTo, DateTime.MinValue, 0);
            List<long> nomenclaturesList = null;
            if (!forNewAprovalsNomenclatureLists.TryGetValue(withoutNomenclatureAndTradeMarkObject, out nomenclaturesList))
                {
                nomenclaturesList = new List<long>();
                forNewAprovalsNomenclatureLists.Add(withoutNomenclatureAndTradeMarkObject, nomenclaturesList);
                }
            nomenclaturesList.Add(approvalRow.NomenclatureId);
            }

        /// <summary>
        /// Добавляет номенклатуру в словарь номенклатур для существующих РД
        /// </summary>
        private void addToUpdateExistedApprovalsList(ApprovalsCacheObject approvalRow)
            {
            if (approvalRow.NomenclatureId == 0)
                {
                return;
                }
            long fakeId = approvalsCacheObjectsStore.GetApprovalsFakeId(approvalRow);
            ApprovalsCacheObject existedApprovalDoc = approvalsCacheObjectsStore.GetCachedObject(fakeId);
            if (existedApprovalDoc != null)
                {
                List<long> nomenclaturesList = null;
                if (!forExistedApprovalsNomenclatureLists.TryGetValue(existedApprovalDoc, out nomenclaturesList))
                    {
                    nomenclaturesList = new List<long>();
                    forExistedApprovalsNomenclatureLists.Add(existedApprovalDoc, nomenclaturesList);
                    }
                nomenclaturesList.Add(approvalRow.NomenclatureId);
                }
            }
        }
    }
