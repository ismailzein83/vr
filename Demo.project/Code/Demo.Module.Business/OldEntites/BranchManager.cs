using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;
using Demo.Module.Data;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Caching;


namespace Demo.Module.Business
{
    public class BranchManager
    {
       
        #region Public Methods
        public InsertOperationOutput<BranchDetails> AddBranch(Branch branch)
        {
            IBranchDataManager branchDataManager = DemoModuleFactory.GetDataManager<IBranchDataManager>();
            InsertOperationOutput<BranchDetails> insertOperationOutput = new InsertOperationOutput<BranchDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int branchId = -1;
            bool insertActionSuccess = branchDataManager.Insert(branch, out branchId);
            if (insertActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                branch.BranchId = branchId;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = BranchDetailMapper(branch);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }



        public IDataRetrievalResult<BranchDetails> GetFilteredBranches(DataRetrievalInput<BranchQuery> input)
        {
            var allBranches = GetAllBranches();
            Func<Branch, bool> filterExpression = (branch) =>
            {
                if (input.Query.Name != null && !branch.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.CompanyIds != null && !input.Query.CompanyIds.Contains(branch.CompanyId))
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allBranches.ToBigResult(input, filterExpression, BranchDetailMapper));

        }

        public Branch GetBranchById(int branchId)
        {
            var allBranches = GetAllBranches();
            return allBranches.GetRecord(branchId);

        }

        public UpdateOperationOutput<BranchDetails> UpdateBranch(Branch branch)
        {
            IBranchDataManager branchDataManager = DemoModuleFactory.GetDataManager<IBranchDataManager>();
            UpdateOperationOutput<BranchDetails> updateOperationOutput = new UpdateOperationOutput<BranchDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = branchDataManager.Update(branch);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = BranchDetailMapper(branch);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        #endregion

        #region Private Methods
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBranchDataManager branchDataManager = DemoModuleFactory.GetDataManager<IBranchDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return branchDataManager.AreBranchesUpdated(ref _updateHandle);
            }
        }
        private Dictionary<int, Branch> GetAllBranches()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
                .GetOrCreateObject("GetCacheBranches", () =>
                {
                    IBranchDataManager branchDataManager = DemoModuleFactory.GetDataManager<IBranchDataManager>();
                    List<Branch> branches = branchDataManager.GetBranches();
                    return branches.ToDictionary(branch => branch.BranchId, branch => branch);
                });
        }
        #endregion

        #region Mappers
        public BranchDetails BranchDetailMapper(Branch branch)
        {
            BranchDetails branchDetails = new BranchDetails();
            branchDetails.BranchId = branch.BranchId;
            branchDetails.Name = branch.Name;
            branchDetails.CompanyId = branch.CompanyId;
            branchDetails.BranchSettingDescription = (branch.Setting!= null) ? branch.Setting.GetDescription() : null;

            CompanyManager companyManager = new CompanyManager();
            Company company = companyManager.GetCompanyById(branch.CompanyId);
            branchDetails.CompanyName = company.Name;

            return branchDetails;
        }
        #endregion

    }
    public class SettingConfigsManager
    {
        public IEnumerable<SettingConfig> GetSettingTypeTemplateConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<SettingConfig>(SettingConfig.EXTENSION_TYPE);
        }
    }
    public class DimensionsConfigsManager
    {
        public IEnumerable<DimensionsConfig> GetDimensionsTypeTemplateConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<DimensionsConfig>(DimensionsConfig.EXTENSION_TYPE);
        }
    }
}
