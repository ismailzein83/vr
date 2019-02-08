//using System.Collections.Generic;
//using System.Linq;
//using TOne.WhS.SMSBusinessEntity.Data.RDB;
//using TOne.WhS.SMSBusinessEntity.Entities;
//using TOne.WhS.SMSBusinessEntity.MainExtensions;
//using Vanrise.Common;

//namespace TOne.WhS.SMSBusinessEntity.Business
//{
//    public class SupplierSMSRateChangesManager
//    {
//        ProcessDraftDataManager _processDraftDataManager = new ProcessDraftDataManager();
//        public List<SupplierSMSRateDetail> GetChanges(SupplierSMSRateChanges supplierChanges)
//        {
//            SMSRateChanges rateChanges = _processDraftDataManager.GetChanges(ProcessEntityType.Supplier, supplierChanges.SupplierID.ToString(), supplierChanges.Status);
//            SupplierSMSRateChanges supplierSMSRateChanges = rateChanges.CastWithValidate<SupplierSMSRateChanges>("SupplierSMSRateChanges");
//            if (supplierSMSRateChanges == null)
//                return null;

//            var rateHistories = supplierSMSRateChanges.SMSRateHistories;
//            if (rateHistories == null)
//                return null;

//            return rateHistories.Select(item => SupplierSMSRateDetailMapper(item)).ToList();
//        }

//        public bool InsertOrUpdateChanges(SupplierSMSRateChanges supplierChanges)
//        {
//            return _processDraftDataManager.InsertOrUpdateChanges(ProcessEntityType.Supplier, supplierChanges, supplierChanges.SupplierID.ToString(), supplierChanges.Status);
//        }

//        public bool UpdateSMSRatePlanStatus(SupplierSMSRateChanges supplierChanges, SMSRatePlanStatus newSMSRatePlanStatus)
//        {
//            return _processDraftDataManager.UpdateSMSRatePlanStatus(ProcessEntityType.Supplier, supplierChanges.SupplierID.ToString(), supplierChanges.Status, newSMSRatePlanStatus);
//        }

//        public bool CancelSMSRatePlanChanges(int supplierID)
//        {
//            return _processDraftDataManager.CancelSMSRatePlanChanges(ProcessEntityType.Supplier, supplierID.ToString(), SMSRatePlanStatus.Draft);
//        }

//        private SupplierSMSRateDetail SupplierSMSRateDetailMapper(CustomerSMSRateHistorySMSRateHistory rateHistory)
//        {
//            return new SupplierSMSRateDetail()
//            {

//            };
//        }
//    }
//}
