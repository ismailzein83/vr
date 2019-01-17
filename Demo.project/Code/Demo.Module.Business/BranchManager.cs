using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class BranchManager
    {
        CompanyManager companyManager = new CompanyManager();

        #region Public Methods
        public IDataRetrievalResult<BranchDetail> GetFilteredBranches(DataRetrievalInput<BranchQuery> input)
        {
            var allBranches = GetCachedBranches();
            Func<Branch, bool> filterExpression = (branch) =>
            {
                if (input.Query.Name != null && !branch.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                if (input.Query.CompanyIds != null && !input.Query.CompanyIds.Contains(branch.CompanyId))
                    return false;

                return true;
            };
            var result = DataRetrievalManager.Instance.ProcessResult(input, allBranches.ToBigResult(input, filterExpression, BranchDetailMapper));
            return DataRetrievalManager.Instance.ProcessResult(input, allBranches.ToBigResult(input, filterExpression, BranchDetailMapper));
        }

        public Branch GetBranchById(int branchId)
        {
            return GetCachedBranches().GetRecord(branchId);
        }

        public IEnumerable<BranchTypeConfig> GetBranchTypeConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<BranchTypeConfig>(BranchTypeConfig.EXTENSION_TYPE);
        }

        public IEnumerable<DepartmentSettingsConfig> GetDepartmentSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<DepartmentSettingsConfig>(DepartmentSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<EmployeeSettingsConfig> GetEmployeeSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<EmployeeSettingsConfig>(EmployeeSettingsConfig.EXTENSION_TYPE);
        }

        public InsertOperationOutput<BranchDetail> AddBranch(Branch branch)
        {
            InsertOperationOutput<BranchDetail> insertOperationOutput = new InsertOperationOutput<BranchDetail>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int branchId = -1;

            IBranchDataManager branchDataManager = DemoModuleFactory.GetDataManager<IBranchDataManager>();
            bool insertActionSuccess = branchDataManager.Insert(branch, out branchId);
            if (insertActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                branch.BranchId = branchId;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = this.BranchDetailMapper(branch);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }

        public UpdateOperationOutput<BranchDetail> UpdateBranch(Branch branch)
        {
            UpdateOperationOutput<BranchDetail> updateOperationOutput = new UpdateOperationOutput<BranchDetail>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IBranchDataManager branchDataManager = DemoModuleFactory.GetDataManager<IBranchDataManager>();
            bool updateActionSuccess = branchDataManager.Update(branch);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = this.BranchDetailMapper(branch);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        #endregion

        #region Private Methods
        private Dictionary<int, Branch> GetCachedBranches()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedBranches", () =>
            {
                IBranchDataManager branchDataManager = DemoModuleFactory.GetDataManager<IBranchDataManager>();
                List<Branch> branches = branchDataManager.GetBranches();
                return branches.ToDictionary(branch => branch.BranchId, branch => branch);
            });
        }
        #endregion

        #region Private Class
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBranchDataManager branchDataManager = DemoModuleFactory.GetDataManager<IBranchDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return branchDataManager.AreBranchesUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Mappers

        private BranchDetail BranchDetailMapper(Branch branch)
        {
            BranchDetail branchDetail = new BranchDetail();
            branchDetail.BranchId = branch.BranchId;
            branchDetail.Name = branch.Name;
            branchDetail.CompanyName = companyManager.GetCompanyName(branch.CompanyId);

            if (branch.Settings != null && branch.Settings.BranchType != null)
            {
                branchDetail.Address = branch.Settings.Address;
                branchDetail.BranchTypeDescription = branch.Settings.BranchType.GetDescription();
            }
            return branchDetail;
        }

        #endregion

    }
}
