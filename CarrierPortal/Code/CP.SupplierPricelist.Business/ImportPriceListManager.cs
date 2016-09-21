using CP.SupplierPricelist.Data;
using CP.SupplierPricelist.Entities;
using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.Common;
using System.Linq;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Entities;
using System;

namespace CP.SupplierPricelist.Business
{
    public class ImportPriceListManager
    {
        public InsertOperationOutput<PriceListDetail> Insert(PriceList inputPriceList)
        {
            InsertOperationOutput<PriceListDetail> insertOperationOutput = new InsertOperationOutput<PriceListDetail>
            {
                Result = InsertOperationResult.Failed,
                InsertedObject = null
            };
            IPriceListDataManager dataManager =
                ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            int priceListId;
            inputPriceList.UserId = SecurityContext.Current.GetLoggedInUserId();
            var customerSuppliers = new CustomerManager().GetCachedSupplierAccounts(inputPriceList.CustomerId);
            inputPriceList.CarrierAccountName = customerSuppliers.ContainsKey(inputPriceList.CarrierAccountId) ? customerSuppliers[inputPriceList.CarrierAccountId].SupplierName : "";
            bool insertActionSucc = dataManager.Insert(inputPriceList, (int)PriceListResult.NotCompleted, out priceListId);
            inputPriceList.PriceListId = priceListId;
            if (insertActionSucc)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = priceListDetailMapper(inputPriceList);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
                insertOperationOutput.ShowExactMessage = true;
                insertOperationOutput.Message = "Another pricelist for the same customer and carrier account is not yet completed";
            }
            return insertOperationOutput;
        }
        public List<PriceListDetail> GetBeforeId(GetBeforeIdInput input)
        {
            input.UserId = SecurityContext.Current.GetLoggedInUserId();
            IPriceListDataManager dataManager =
                ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            List<PriceList> listTestCalls = dataManager.GetBeforeId(input);
            List<PriceListDetail> listTestCallDetails = new List<PriceListDetail>();
            foreach (PriceList priceList in listTestCalls)
            {
                listTestCallDetails.Add(priceListDetailMapper(priceList));
            }
            return listTestCallDetails;
        }
        public PriceListlUpdateOutput GetUpdated(ref byte[] maxTimeStamp, int nbOfRows)
        {
            PriceListlUpdateOutput priceListUpdateOutputs = new PriceListlUpdateOutput();
            IPriceListDataManager dataManager =
             ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            List<PriceList> listPriceLists = dataManager.GetUpdated(ref maxTimeStamp, nbOfRows, SecurityContext.Current.GetLoggedInUserId());
            List<PriceListDetail> listPriceListDetails = listPriceLists.Select(priceListDetailMapper).ToList();
            priceListUpdateOutputs.ListPriceListDetails = listPriceListDetails;
            priceListUpdateOutputs.MaxTimeStamp = maxTimeStamp;
            return priceListUpdateOutputs;
        }
        public IDataRetrievalResult<PriceListDetail> GetFilterePriceLists(DataRetrievalInput<PriceListQuery> input)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            IPriceListDataManager dataManager = ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            BigResult<PriceList> priceListResult = dataManager.GetPriceListsFilteredFromTemp(input, userId);
        
            BigResult<PriceListDetail> priceListDetailResult = new BigResult<PriceListDetail>()
            {
                ResultKey = priceListResult.ResultKey,
                TotalCount = priceListResult.TotalCount,
                Data = priceListResult.Data.MapRecords(priceListDetailMapper)
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, priceListDetailResult);
        }
        PriceListDetail priceListDetailMapper(PriceList priceList)
        {
            UserManager userManager = new UserManager();
            User user = userManager.GetUserbyId(priceList.UserId);

            CustomerManager customerManager = new CustomerManager();
            var priceListDetail = new PriceListDetail
            {
                Entity = priceList,
                PriceListStatusDescription = Utilities.GetEnumDescription(priceList.Status),
                PriceListResultDescription = Utilities.GetEnumDescription(priceList.Result),
                PriceListTypeValue = Utilities.GetEnumDescription(priceList.PriceListType),
                UserName = user != null ? user.Name : ""
            };
            Customer customer = customerManager.GetCustomer(priceList.CustomerId);
            priceListDetail.CustomerName = customer != null ? customer.Name : "";

            return priceListDetail;
        }
       
        public List<PriceList> GetPriceLists(List<PriceListStatus> listPriceListStatuses)
        {
            IPriceListDataManager dataManager =
                ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            return dataManager.GetPriceLists(listPriceListStatuses);
        }
        public List<PriceList> GetPriceLists(List<PriceListStatus> listPriceListStatuses, int customerId)
        {
            IPriceListDataManager dataManager =
                ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            return dataManager.GetPriceLists(listPriceListStatuses, customerId);
        }

        public bool UpdatePriceListUpload(long id, int result, int status, object uploadInformation, int uploadRetryCount)
        {
            IPriceListDataManager dataManager =
                ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            return dataManager.UpdatePriceListUpload(id, result, status, uploadInformation, uploadRetryCount);
        }

        public IEnumerable<CustomerConnectorConfig> GetConnectorTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<CustomerConnectorConfig>(CustomerConnectorConfig.EXTENSION_TYPE);
        }

        public bool UpdatePriceListProgress(long id, int status, int result, int resultRetryCount, string alertMessage, long alertFileId)
        {
            IPriceListDataManager dataManager =
                   ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            return dataManager.UpdatePriceListProgress(id, status, result, resultRetryCount, alertMessage, alertFileId);

        }
    }
}
