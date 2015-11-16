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



        public AccountManagerCarrier AccountManagerCarrierMapper(CarrierAccount carrierAccount)
        {
            AccountManagerCarrier accountManagerCarrier = new AccountManagerCarrier();
            accountManagerCarrier.Name = carrierAccount.Name;
            accountManagerCarrier.CarrierAccountId = carrierAccount.CarrierAccountId;
            accountManagerCarrier.IsCustomerAvailable = (carrierAccount.AccountType == CarrierAccountType.Customer || carrierAccount.AccountType == CarrierAccountType.Exchange);
            accountManagerCarrier.IsSupplierAvailable = (carrierAccount.AccountType == CarrierAccountType.Supplier || carrierAccount.AccountType == CarrierAccountType.Exchange);
            return accountManagerCarrier;
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
            var assignedCarriers = GetCachedAssignedCarriers();
            List<int> userIds = this.GetMemberIds(input.Query.ManagerId, input.Query.WithDescendants);
            Func<AssignedCarrier, bool> filterExpression = (prod) =>
               (userIds == null || userIds.Contains(prod.UserId));
           var filteredAssignedCarriers= assignedCarriers.ToBigResult(input, filterExpression,AssignedCarrierDetailMapper);

            foreach(var obj in filteredAssignedCarriers.Data){
                obj.IsCustomerInDirect = (input.Query.ManagerId != obj.Entity.UserId && obj.Entity.RelationType == CarrierAccountType.Customer);
                obj.IsSupplierInDirect = (input.Query.ManagerId != obj.Entity.UserId && obj.Entity.RelationType == CarrierAccountType.Supplier);
                obj.IsCustomerAssigned = (obj.Entity.RelationType == CarrierAccountType.Customer || obj.Entity.RelationType == CarrierAccountType.Exchange);
                obj.IsSupplierAssigned = (obj.Entity.RelationType == CarrierAccountType.Supplier || obj.Entity.RelationType == CarrierAccountType.Exchange);
                obj.IsCustomerAvailable = true;
                obj.IsSupplierAvailable = true;
            }
           return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, filteredAssignedCarriers);
        }

        public IEnumerable<AssignedCarrierDetail> GetAssignedCarriersDetail(int managerId, bool withDescendants, CarrierAccountType carrierType)
        {
            var assignedCarriers = GetCachedAssignedCarriers();
            List<int> userIds = this.GetMemberIds(managerId, withDescendants);
            Func<AssignedCarrier, bool> filterExpression = (prod) =>
               (userIds == null || userIds.Contains(prod.UserId)
            );
            var filteredAssignedCarriers = assignedCarriers.MapRecords(AssignedCarrierDetailMapper,filterExpression).ToList();

            foreach (var obj in filteredAssignedCarriers)
            {
                obj.IsCustomerInDirect = (managerId != obj.Entity.UserId && obj.Entity.RelationType == CarrierAccountType.Customer);
                obj.IsSupplierInDirect = (managerId != obj.Entity.UserId && obj.Entity.RelationType == CarrierAccountType.Supplier);
                obj.IsCustomerAssigned = (obj.Entity.RelationType == CarrierAccountType.Customer || obj.Entity.RelationType == CarrierAccountType.Exchange);
                obj.IsSupplierAssigned = (obj.Entity.RelationType == CarrierAccountType.Supplier || obj.Entity.RelationType == CarrierAccountType.Exchange);
                obj.IsCustomerAvailable = true;
                obj.IsSupplierAvailable = true;
            }
            return filteredAssignedCarriers;
        }


        public Vanrise.Entities.UpdateOperationOutput<object> AssignCarriers(UpdatedAccountManagerCarrier[] updatedCarriers)
        {
            IAssignedCarrierDataManager dataManager = BEDataManagerFactory.GetDataManager<IAssignedCarrierDataManager>();
            bool updateActionSucc = dataManager.AssignCarriers(updatedCarriers);

            Vanrise.Entities.UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();

            updateOperationOutput.Result = updateActionSucc ? Vanrise.Entities.UpdateOperationResult.Succeeded : Vanrise.Entities.UpdateOperationResult.Failed;
            
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

        public IEnumerable<int> GetMyAssignedSuppliersAMU()
        {
            IEnumerable<AssignedCarrier> assignedCarriers = GetAssignedCarriers(SecurityContext.Current.GetLoggedInUserId(), true, CarrierAccountType.Supplier);
            List<int> suppliers = new List<int>();
            foreach (AssignedCarrier assignedCarrier in assignedCarriers)
            {
                suppliers.Add(assignedCarrier.CarrierAccountId);
            }
            return suppliers;
        }

        public IEnumerable<int> GetMyAssignedCustomerAMU()
        {
            IEnumerable<AssignedCarrier> assignedCarriers = GetAssignedCarriers(SecurityContext.Current.GetLoggedInUserId(), true, CarrierAccountType.Customer);
            List<int> cutomers = new List<int>();
            foreach (AssignedCarrier assignedCarrier in assignedCarriers)
            {
                cutomers.Add(assignedCarrier.CarrierAccountId);
            }
            return cutomers;
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
        #endregion


    }
}
