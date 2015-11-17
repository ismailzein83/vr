using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Security.Business;
using Vanrise.Common;
namespace TOne.WhS.BusinessEntity.Business
{
    public class AccountManagerManager
    {
        public IEnumerable<AccountManagerCarrier> GetAssignedCarriersById(int userId)
        {
            var assignedCarriers = GetCachedAssignedCarriers();
             CarrierAccountManager carrierAccountManager=new CarrierAccountManager();
           IEnumerable<CarrierAccount> carriers= carrierAccountManager.GetAllCarriers();
             IEnumerable<AssignedCarrier> filteredAssignedCarriers=  assignedCarriers.Where(x => x.UserId == userId);

            return  carriers.MapRecords(AccountManagerCarrierMapper);

        }
        public IEnumerable<AssignedCarrier> GetAssignedCarriers()
        {
            return GetCachedAssignedCarriers();
        }
       
        public IEnumerable<AssignedCarrier> GetAssignedCarriers(int managerId, bool withDescendants, CarrierAccountType carrierType)
        {
            List<int> userIds = this.GetMemberIds(managerId, withDescendants);
            var assignedCarriers = GetCachedAssignedCarriers();
            Func<AssignedCarrier, bool> filterExpression = (prod) =>
              (userIds == null || userIds.Contains(prod.UserId)
              && (prod.RelationType == CarrierAccountType.Exchange || prod.RelationType == carrierType));

            return assignedCarriers.FindAllRecords(filterExpression);
        }
        public Vanrise.Entities.IDataRetrievalResult<AssignedCarrierDetail> GetFilteredAssignedCarriers(Vanrise.Entities.DataRetrievalInput<AssignedCarrierQuery> input)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var carrierAccounts = carrierAccountManager.GetAllCarriers();

            var assignedCarriers = GetCachedAssignedCarriers();
            List<int> userIds = this.GetMemberIds(input.Query.ManagerId, input.Query.WithDescendants);
            Func<AssignedCarrier, bool> filterExpression = (prod) =>
               (userIds == null || userIds.Contains(prod.UserId));
           var filteredAssignedCarriers= assignedCarriers.ToBigResult(input, filterExpression,AssignedCarrierDetailMapper);
           filteredAssignedCarriers.Data = FillNeededProperties(filteredAssignedCarriers.Data.ToList(), input.Query.ManagerId);
          
           return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, filteredAssignedCarriers);
        }
        public IEnumerable<AssignedCarrierDetail> GetAssignedCarriersDetail(int managerId, bool withDescendants, CarrierAccountType carrierType)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var carrierAccounts = carrierAccountManager.GetAllCarriers();
            var assignedCarriers = GetCachedAssignedCarriers();
            List<int> userIds = this.GetMemberIds(managerId, withDescendants);
            Func<AssignedCarrier, bool> filterExpression = (prod) =>
               (userIds == null || userIds.Contains(prod.UserId)
            );
            var filteredAssignedCarriers = assignedCarriers.MapRecords(AssignedCarrierDetailMapper,filterExpression).ToList();
            FillNeededProperties( filteredAssignedCarriers,managerId);
            return filteredAssignedCarriers;
        }

        public List<AssignedCarrierDetail> FillNeededProperties(List<AssignedCarrierDetail> filteredAssignedCarriers, int managerId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var carrierAccounts = carrierAccountManager.GetAllCarriers();
            var assignedCarriers = GetCachedAssignedCarriers();
            List<AssignedCarrierDetail> listToRemove =new List<AssignedCarrierDetail>();
            foreach (var obj in filteredAssignedCarriers)
            {

                var carrierToRemove = filteredAssignedCarriers.FindRecord(x => x.Entity.CarrierAccountId == obj.Entity.CarrierAccountId  && x.Entity.RelationType != obj.Entity.RelationType);
                var assignCarrier = assignedCarriers.FindRecord(x => x.CarrierAccountId == obj.Entity.CarrierAccountId && x.RelationType != obj.Entity.RelationType);
                var carrierAccount = carrierAccounts.FindRecord(x => x.CarrierAccountId == obj.Entity.CarrierAccountId);
                if (carrierAccount.AccountType == CarrierAccountType.Exchange)
                {
                    obj.IsCustomerAssigned = (obj.Entity.RelationType == CarrierAccountType.Customer) || (carrierToRemove != null && carrierToRemove.Entity.RelationType == CarrierAccountType.Customer );
                    obj.IsSupplierAssigned = (obj.Entity.RelationType == CarrierAccountType.Supplier) || (carrierToRemove != null && carrierToRemove.Entity.RelationType == CarrierAccountType.Supplier );
                    obj.IsCustomerAvailable = (obj.Entity.RelationType == CarrierAccountType.Customer || (carrierToRemove != null && (carrierToRemove.Entity.RelationType == CarrierAccountType.Customer && obj.Entity.UserId == carrierToRemove.Entity.UserId)) || (assignCarrier == null && obj.Entity.UserId == managerId));
                    obj.IsSupplierAvailable = (obj.Entity.RelationType == CarrierAccountType.Supplier || (carrierToRemove != null && (carrierToRemove.Entity.RelationType == CarrierAccountType.Supplier && obj.Entity.UserId == carrierToRemove.Entity.UserId)) || (assignCarrier == null && obj.Entity.UserId == managerId));
                    obj.IsCustomerInDirect = (managerId != obj.Entity.UserId && (obj.Entity.RelationType == CarrierAccountType.Customer || (carrierToRemove != null && carrierToRemove.Entity.RelationType == CarrierAccountType.Customer && obj.Entity.UserId == carrierToRemove.Entity.UserId)));
                    obj.IsSupplierInDirect = (managerId != obj.Entity.UserId && (obj.Entity.RelationType == CarrierAccountType.Supplier || (carrierToRemove != null && carrierToRemove.Entity.RelationType == CarrierAccountType.Supplier && obj.Entity.UserId == carrierToRemove.Entity.UserId)));
                    if ((carrierToRemove != null && !listToRemove.Any(x => x.Entity.CarrierAccountId == carrierToRemove.Entity.CarrierAccountId)) )
                            listToRemove.Add(carrierToRemove);
                }
                else
                {
                      obj.IsCustomerAssigned = (obj.Entity.RelationType == CarrierAccountType.Customer);
                      obj.IsSupplierAssigned = (obj.Entity.RelationType == CarrierAccountType.Supplier);
                      obj.IsCustomerAvailable = (carrierAccount.AccountType == CarrierAccountType.Customer) && (obj.Entity.RelationType == CarrierAccountType.Customer);
                      obj.IsSupplierAvailable = (carrierAccount.AccountType == CarrierAccountType.Supplier) && (obj.Entity.RelationType == CarrierAccountType.Supplier);
                      obj.IsCustomerInDirect = (managerId != obj.Entity.UserId && obj.Entity.RelationType == CarrierAccountType.Customer);
                      obj.IsSupplierInDirect = (managerId != obj.Entity.UserId && obj.Entity.RelationType == CarrierAccountType.Supplier);
                }   
                
            }
            foreach(var obj in listToRemove)
                 filteredAssignedCarriers.Remove(obj);
            return filteredAssignedCarriers;
        }
        public Vanrise.Entities.UpdateOperationOutput<object> AssignCarriers(UpdatedAccountManagerCarrier[] updatedCarriers)
        {
            IAccountManagerCarrierDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountManagerCarrierDataManager>();
            Vanrise.Entities.UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            bool updateActionSucc = dataManager.AssignCarriers(updatedCarriers);
         
            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            }
           
            updateOperationOutput.UpdatedObject = null;
           
            return updateOperationOutput;
        }
        public int? GetLinkedOrgChartId()
        {
            OrgChartManager orgChartManager = new OrgChartManager();
            return orgChartManager.GetLinkedOrgChartId(new OrgChartAccountManagerInfo());
        }
        public Vanrise.Entities.UpdateOperationOutput<object> UpdateLinkedOrgChart(int orgChartId)
        {
            OrgChartManager orgChartManager = new OrgChartManager();
            return orgChartManager.UpdateOrgChartLinkedEntity(orgChartId, new OrgChartAccountManagerInfo());
        }
        private List<int> GetMemberIds(int orgChartId, int managerId)
        {
            OrgChartManager orgChartManager = new OrgChartManager();
            return orgChartManager.GetMemberIds(orgChartId, managerId);
        }
        private List<int> GetMemberIds(int managerId, bool withDescendants)
        {
            List<int> memberIds = new List<int>();

            if (withDescendants)
            {
                int? orgChartId = GetLinkedOrgChartId();

                if (orgChartId != null)
                    memberIds = GetMemberIds((int)orgChartId, managerId); // GetMemberIds will add managerId to the list
            }
            else
                memberIds.Add(managerId);

            return memberIds;
        }
        public IEnumerable<int> GetAssignedAccountIds(int managerId, CarrierAccountType carrierType)
        {
            IEnumerable<AssignedCarrier> assignedCarriers = GetAssignedCarriers(managerId, true, carrierType);
            List<int> accountIds = new List<int>();
            foreach (AssignedCarrier assignedCarrier in assignedCarriers)
            {
                accountIds.Add(assignedCarrier.CarrierAccountId);
            }
            return accountIds;
        }
        public IEnumerable<int> GetMyAssignedCustomerIds()
        {
            return GetAssignedAccountIds(SecurityContext.Current.GetLoggedInUserId(), CarrierAccountType.Customer);
        }
        public IEnumerable<int> GetMyAssignedSupplierIds()
        {
            return GetAssignedAccountIds(SecurityContext.Current.GetLoggedInUserId(), CarrierAccountType.Supplier);
        }

        #region Private Members

        IEnumerable<AssignedCarrier> GetCachedAssignedCarriers()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAssignedCarriers",
               () =>
               {
                   IAccountManagerCarrierDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountManagerCarrierDataManager>();
                   return dataManager.GetAssignedCarriers();
                    
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IAccountManagerCarrierDataManager _dataManager = BEDataManagerFactory.GetDataManager<IAccountManagerCarrierDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAccountManagerUpdated(ref _updateHandle);
            }
        }
        private AssignedCarrierDetail AssignedCarrierDetailMapper(AssignedCarrier assignedCarrier)
        {
            AssignedCarrierDetail assignedCarrierDetail = new AssignedCarrierDetail();
            CarrierAccountManager carrierAccountManager=new CarrierAccountManager();
            assignedCarrierDetail.Entity = assignedCarrier;
               CarrierAccount carrierAccount= carrierAccountManager.GetCarrierAccount(assignedCarrier.CarrierAccountId);
                if(carrierAccount!=null)
                     assignedCarrierDetail.CarrierName=carrierAccount.Name;
            return assignedCarrierDetail;

        }
        private AccountManagerCarrier AccountManagerCarrierMapper(CarrierAccount carrierAccount)
        {
            AccountManagerCarrier accountManagerCarrier = new AccountManagerCarrier();
            accountManagerCarrier.Name = carrierAccount.Name;
            accountManagerCarrier.CarrierAccountId = carrierAccount.CarrierAccountId;
            accountManagerCarrier.IsCustomerAvailable = (carrierAccount.AccountType == CarrierAccountType.Customer || carrierAccount.AccountType == CarrierAccountType.Exchange);
            accountManagerCarrier.IsSupplierAvailable = (carrierAccount.AccountType == CarrierAccountType.Supplier || carrierAccount.AccountType == CarrierAccountType.Exchange);
            return accountManagerCarrier;
        }
        #endregion


    }
}
