﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Entities;
using Retail.BusinessEntity.Data;
using Vanrise.Security.Business;

namespace Retail.BusinessEntity.Business
{
    public class PackageDefinitionManager
    {
        #region Public Methods

        public PackageDefinition GetPackageDefinitionById(Guid packageDefinitionId)
        {
            var packageDefinitions = GetCachedPackageDefinitionsWithHidden();
            return packageDefinitions.FindRecord(x => x.VRComponentTypeId == packageDefinitionId);
        }

        public string GetPackageDefinitionName(Guid packageDefinitionId)
        {
            var packageDefinition = GetPackageDefinitionById(packageDefinitionId);
            return packageDefinition != null ? packageDefinition.Name : null;
        }

        public IEnumerable<RecurringChargeEvaluatorConfig> GetRecurringChargeEvaluatorConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<RecurringChargeEvaluatorConfig>(RecurringChargeEvaluatorConfig.EXTENSION_TYPE);
        }

        public IEnumerable<PackageDefinitionInfo> GetPackageDefinitionsInfo(PackageDefinitionFilter filter)
        {
            Dictionary<Guid, PackageDefinition> cachedPackageDefinitions = null;

            Func<PackageDefinition, bool> filterExpression = null;
            if (filter != null)
            {
                if (filter.IncludeHiddenPackageDefinitions)
                    cachedPackageDefinitions = this.GetCachedPackageDefinitionsWithHidden();

                filterExpression = (packageDefinition) =>
                {
                    if (filter.AccountBEDefinitionId.HasValue && packageDefinition.Settings != null &&
                        filter.AccountBEDefinitionId.Value != packageDefinition.Settings.AccountBEDefinitionId)
                        return false;

                    if (filter.Filters != null && !CheckIfFilterIsMatch(packageDefinition, filter.Filters))
                        return false;

                    return true;
                };
            }

            if (cachedPackageDefinitions == null)
                cachedPackageDefinitions = this.GetCachedPackageDefinitions();

            return cachedPackageDefinitions.MapRecords(PackageDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }
     
        public IEnumerable<PackageDefinitionConfig> GetPackageDefinitionExtendedSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<PackageDefinitionConfig>(PackageDefinitionConfig.EXTENSION_TYPE);
        }

        public Guid GetPackageDefinitionAccountBEDefId(Guid packageDefinitionId)
        {
            PackageDefinitionManager packageDefinitionManager = new PackageDefinitionManager();
            PackageDefinition packageDefinition = packageDefinitionManager.GetPackageDefinitionById(packageDefinitionId);
            if (packageDefinition == null)
                throw new NullReferenceException(string.Format("packageDefinition of packageDefinitionId: {0}", packageDefinitionId));

            if (packageDefinition.Settings == null)
                throw new NullReferenceException(string.Format("packageDefinition.Settings of packageDefinitionId: {0}", packageDefinitionId));

            return packageDefinition.Settings.AccountBEDefinitionId;
        }

        #endregion

        #region Private Methods

        public Dictionary<Guid, PackageDefinition> GetCachedPackageDefinitionsWithHidden()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetCachedComponentTypes<PackageDefinitionSettings, PackageDefinition>();
        }

        private Dictionary<Guid, PackageDefinition> GetCachedPackageDefinitions()
        {
            return new VRComponentTypeManager().GetCachedOrCreate("GetPackageDefinitions", () =>
            {
                var includedProductDefinitions = new Dictionary<Guid, PackageDefinition>();
                VRRetailBEVisibilityManager retailBEVisibilityManager = new VRRetailBEVisibilityManager();
                Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionPackageDefinition> visiblePackageDefinitionsById;

                var allPackageDefinitions = this.GetCachedPackageDefinitionsWithHidden();

                if (retailBEVisibilityManager.ShouldApplyPackageDefinitionsVisibility(out visiblePackageDefinitionsById))
                {
                    foreach (var packageDefinition in allPackageDefinitions)
                    {
                        if (visiblePackageDefinitionsById.ContainsKey(packageDefinition.Key))
                            includedProductDefinitions.Add(packageDefinition.Key, packageDefinition.Value);
                    }
                }
                else
                {
                    includedProductDefinitions = allPackageDefinitions;
                }

                return includedProductDefinitions;
            });
        }

        private bool CheckIfFilterIsMatch(PackageDefinition packageDefinition, List<IPackageDefinitionFilter> filters)
        {
            var context = new PackageDefinitionFilterContext { PakageDefinitionId = packageDefinition.VRComponentTypeId };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }


        #endregion

        #region Security
        public bool DoesUserHaveViewPackageDefinitions()
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return GetViewAllowedPackageDefinitions(userId).Count > 0;
        }
        public bool DoesUserHaveViewPackageDefinitions(int userId)
        {
            return GetViewAllowedPackageDefinitions(userId).Count > 0;
        }
        public HashSet<Guid> GetViewAllowedPackageDefinitions()
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return GetViewAllowedPackageDefinitions(userId);
        }

        public HashSet<Guid> GetViewAllowedPackageDefinitions(int userId)
        {
            HashSet<Guid> ids = new HashSet<Guid>();
            var allPackages = this.GetCachedPackageDefinitions();
            foreach (var p in allPackages)
            {
                if (DoesUserHaveViewAccess(userId, p.Key))
                    ids.Add(p.Key);
            }
            return ids;
        }
        public bool DoesUserHaveViewAccess(int UserId, Guid PackageDefinitionId)
        {
            var accountBEDefinitionId = GetPackageDefinitionAccountBEDefId(PackageDefinitionId);
            AccountPackageProvider accountPackageProvider = new AccountPackageProviderManager().GetAccountPackageProvider(accountBEDefinitionId);
            return accountPackageProvider.DoesUserHaveViewPackageAccess(new PackageDefinitionAccessContext
            {
                AccountBEDefinitionId = accountBEDefinitionId,
                PackageDefinitionId = PackageDefinitionId,
                UserId = UserId
            });
        }
        public bool DoesUserHaveAddPackageDefinitions()
        {
            return GetdAddAllowedPackageDefinitions().Count > 0;
        }
        public HashSet<Guid> GetdAddAllowedPackageDefinitions()
        {
            HashSet<Guid> ids = new HashSet<Guid>();
            int userId = SecurityContext.Current.GetLoggedInUserId();
            var allPackages = this.GetCachedPackageDefinitions();
            foreach (var p in allPackages)
            {
                if (DoesUserHaveAddPackageDefinitions(p.Key, userId))
                    ids.Add(p.Key);
            }
            return ids;
        }
        public bool DoesUserHaveAddPackageDefinitions(Guid packageDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAddPackageDefinitions(packageDefinitionId, userId);
        }

        public bool DoesUserHaveAddPackageDefinitions(Guid packageDefinitionId, int userId)
        {
            var accountBEDefinitionId = GetPackageDefinitionAccountBEDefId(packageDefinitionId);
            AccountPackageProvider accountPackageProvider = new AccountPackageProviderManager().GetAccountPackageProvider(accountBEDefinitionId);
            return accountPackageProvider.DoesUserHaveAddPackageAccess(new PackageDefinitionAccessContext
            {
                AccountBEDefinitionId = accountBEDefinitionId,
                PackageDefinitionId = packageDefinitionId,
                UserId = userId
            });
        }
        public bool DoesUserHaveEditPackageDefinitions(Guid packageDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveEditPackageDefinitions(packageDefinitionId, userId);
        }

        public bool DoesUserHaveEditPackageDefinitions(Guid packageDefinitionId, int userId)
        {
            var accountBEDefinitionId = GetPackageDefinitionAccountBEDefId(packageDefinitionId);
            AccountPackageProvider accountPackageProvider = new AccountPackageProviderManager().GetAccountPackageProvider(accountBEDefinitionId);
            return accountPackageProvider.DoesUserHaveEditPackageAccess(new PackageDefinitionAccessContext
            {
                AccountBEDefinitionId = accountBEDefinitionId,
                PackageDefinitionId = packageDefinitionId,
                UserId = userId
            });
        }

        #endregion

        #region Mapper

        public PackageDefinitionInfo PackageDefinitionInfoMapper(PackageDefinition packageDefinition)
        {
            return new PackageDefinitionInfo
            {
                Name = packageDefinition.Name,
                PackageDefinitionId = packageDefinition.VRComponentTypeId,
                RuntimeEditor = packageDefinition.Settings.ExtendedSettings.RuntimeEditor
            };
        }

        #endregion
    }
}
