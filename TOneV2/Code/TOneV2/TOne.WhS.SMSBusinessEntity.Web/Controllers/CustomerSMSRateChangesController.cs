using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.SMSBusinessEntity.Business;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.SMSBusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerSMSRateChanges")]

    public class CustomerSMSRateChangesController : BaseAPIController
    {
        CustomerSMSRateDraftManager _customerSMSRateChangesManager = new CustomerSMSRateDraftManager();

        [HttpPost]
        [Route("GetFilteredChanges")]
        public Vanrise.Entities.IDataRetrievalResult<CustomerSMSRateChangesDetail> GetFilteredChanges(DataRetrievalInput<CustomerSMSRateChangesQuery> input)
        {
            return _customerSMSRateChangesManager.GetFilteredChanges(input);
        }

        [HttpPost]
        [Route("InsertOrUpdateChanges")]
        public DraftStateResult InsertOrUpdateChanges(CustomerSMSRateDraftToUpdate customerDraftToUpdate)
        {
            return _customerSMSRateChangesManager.InsertOrUpdateChanges(customerDraftToUpdate);
        }
     
        [HttpPost]
        [Route("UpdateSMSRateChangesStatus")]
        public bool UpdateSMSRateChangesStatus(DataRetrievalInput<UpdateCustomerSMSDraftStatusInput> input)
        {
            return _customerSMSRateChangesManager.UpdateSMSRateChangesStatus(input);
        }

        [HttpPost]
        [Route("GetDraftData")]
        public DraftData GetDraftData(DataRetrievalInput<CustomerDraftDataInput> input)
        {
            return _customerSMSRateChangesManager.GetDraftData(input);
        }

        [HttpGet]
        [Route("CheckIfDraftExist")]
        public DraftStateResult CheckIfDraftExist(int customerID)
        {
            return _customerSMSRateChangesManager.CheckIfDraftExist(customerID);
        }

    }
}