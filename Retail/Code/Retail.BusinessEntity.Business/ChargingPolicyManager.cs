using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class ChargingPolicyManager : BaseBusinessEntityManager
    {
        #region Fields

        ServiceTypeManager _serviceTypeManager = new ServiceTypeManager();

        #endregion
       
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<ChargingPolicyDetail> GetFilteredChargingPolicies(Vanrise.Entities.DataRetrievalInput<ChargingPolicyQuery> input)
        {
            Dictionary<int, ChargingPolicy> cachedChargingPolicies = this.GetCachedChargingPolicies();

            Func<ChargingPolicy, bool> filterExpression = (chargingPolicy) =>
                (input.Query.Name == null || chargingPolicy.Name.ToLower().Contains(input.Query.Name.ToLower())) &&
                (input.Query.ServiceTypeIds == null || input.Query.ServiceTypeIds.Contains(chargingPolicy.ServiceTypeId));


            var resultProcessingHandler = new ResultProcessingHandler<ChargingPolicyDetail>()
            {
                ExportExcelHandler = new ChargingPolicyDetailExportExcelHandler()
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedChargingPolicies.ToBigResult(input, filterExpression, ChargingPolicyDetailMapper), resultProcessingHandler);
        }
        public IEnumerable<ChargingPolicyInfo> GetChargingPoliciesInfo(ChargingPolicyInfoFilter filter)
        {

            Func<ChargingPolicy, bool> filterExpression = null;
            if (filter != null)
            {
                if (filter.ServiceTypeId.HasValue)
                    filterExpression = (x) => x.ServiceTypeId == filter.ServiceTypeId;
            }
            return this.GetCachedChargingPolicies().MapRecords(ChargingPolicyInfoMapper, filterExpression).OrderBy(x => x.Name);
        }
        public ChargingPolicy GetChargingPolicy(int chargingPolicyId, bool isViewedFromUI)
        {
            var chargingPolicy= this.GetCachedChargingPolicies().GetRecord(chargingPolicyId);
            if (chargingPolicy != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(new ChargingPolicyLoggableEntity(_serviceTypeManager.GetServiceType(chargingPolicy.ServiceTypeId).AccountBEDefinitionId), chargingPolicy);
            return chargingPolicy;
        }
        public ChargingPolicy GetChargingPolicy(int chargingPolicyId)
        {
            return GetChargingPolicy(chargingPolicyId,false);
        }

        public string GetChargingPolicyName(int chargingPolicyId)
        {
            var chargingPolicy = GetChargingPolicy(chargingPolicyId);
            return chargingPolicy != null ? chargingPolicy.Name : null;
        }

        public Vanrise.Entities.InsertOperationOutput<ChargingPolicyDetail> AddChargingPolicy(ChargingPolicy chargingPolicy)
        {
            ValidateChargingPolicyToAdd(chargingPolicy);

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<ChargingPolicyDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IChargingPolicyDataManager dataManager = BEDataManagerFactory.GetDataManager<IChargingPolicyDataManager>();
            int chargingPolicyId = -1;

            if (dataManager.Insert(chargingPolicy, out chargingPolicyId))
            {
                chargingPolicy.ChargingPolicyId = chargingPolicyId;
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(new ChargingPolicyLoggableEntity(_serviceTypeManager.GetServiceType(this.GetChargingPolicy(chargingPolicy.ChargingPolicyId).ServiceTypeId).AccountBEDefinitionId), this.GetChargingPolicy(chargingPolicy.ChargingPolicyId));
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = ChargingPolicyDetailMapper(chargingPolicy);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<ChargingPolicyDetail> UpdateChargingPolicy(ChargingPolicyToEdit chargingPolicy)
        {
            ValidateChargingPolicyToEdit(chargingPolicy);

            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ChargingPolicyDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IChargingPolicyDataManager dataManager = BEDataManagerFactory.GetDataManager<IChargingPolicyDataManager>();

            if (dataManager.Update(chargingPolicy))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(new ChargingPolicyLoggableEntity(_serviceTypeManager.GetServiceType(this.GetChargingPolicy(chargingPolicy.ChargingPolicyId).ServiceTypeId).AccountBEDefinitionId), this.GetChargingPolicy(chargingPolicy.ChargingPolicyId));
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ChargingPolicyDetailMapper(this.GetChargingPolicy(chargingPolicy.ChargingPolicyId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<ChargingPolicyPartTypeConfig> GetChargingPolicyPartTypeTemplateConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<ChargingPolicyPartTypeConfig>(ChargingPolicyPartTypeConfig.EXTENSION_TYPE);
        }
        public IEnumerable<ChargingPolicyDefinitionConfig> GetChargingPolicyTemplateConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<ChargingPolicyDefinitionConfig>(ChargingPolicyDefinitionConfig.EXTENSION_TYPE);
        }
        public IEnumerable<ChargingPolicyPartConfig> GetChargingPolicyPartTemplateConfigs(Guid partTypeConfigId)
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            var partType = GetChargingPolicyPartTypeTemplateConfigs().FindRecord(itm => itm.ExtensionConfigurationId == partTypeConfigId);
            if (partType == null)
                throw new NullReferenceException(string.Format("partTypeConfigId: {0} doesn't have partType", partTypeConfigId));
            return manager.GetExtensionConfigurations<ChargingPolicyPartConfig>(partType.PartTypeExtensionName);
        }
        #endregion

        #region Validation Methods

        private void ValidateChargingPolicyToAdd(ChargingPolicy chargingPolicy)
        {
            ValidateChargingPolicy(chargingPolicy.Name, chargingPolicy.ServiceTypeId, chargingPolicy.Settings);
        }

        private void ValidateChargingPolicyToEdit(ChargingPolicyToEdit chargingPolicy)
        {
            ChargingPolicy chargingPolicyEntity = this.GetChargingPolicy(chargingPolicy.ChargingPolicyId);

            if (chargingPolicyEntity == null)
                throw new DataIntegrityValidationException(String.Format("ChargingPolicy '{0}' does not exist", chargingPolicy.ChargingPolicyId));

            ValidateChargingPolicy(chargingPolicy.Name, chargingPolicyEntity.ServiceTypeId, chargingPolicy.Settings);
        }

        private void ValidateChargingPolicy(string name, Guid serviceTypeId, ChargingPolicySettings settings)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new MissingArgumentValidationException("ChargingPolicy.Name");

            var serviceTypeManager = new ServiceTypeManager();
            ServiceType serviceType = serviceTypeManager.GetServiceType(serviceTypeId);
            if (serviceType == null)
                throw new DataIntegrityValidationException(String.Format("ServiceType '{0}' does not exist", serviceTypeId));

            if (settings == null)
                throw new MissingArgumentValidationException("ChargingPolicy.Settings");
            if (settings.Parts == null)
                throw new MissingArgumentValidationException("ChargingPolicy.Settings.Parts");
        }

        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IChargingPolicyDataManager _dataManager = BEDataManagerFactory.GetDataManager<IChargingPolicyDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreChargingPoliciesUpdated(ref _updateHandle);
            }
        }

        private class ChargingPolicyLoggableEntity : VRLoggableEntityBase
        {
            static AccountBEDefinitionManager _accountBEDefintionManager = new AccountBEDefinitionManager();
           
            Guid _accountBEDefinitionId;

           
            static ChargingPolicyManager s_userManager = new ChargingPolicyManager();
            public ChargingPolicyLoggableEntity(Guid accountBEDefinitionId)
            {
                _accountBEDefinitionId = accountBEDefinitionId;
            }

           
            public override string EntityUniqueName
            {
                get { return String.Format("Retail_BusinessEntity_ChargingPolicy_{0}", _accountBEDefinitionId); }
            }

            public override string EntityDisplayName
            {
                get { return String.Format(_accountBEDefintionManager.GetAccountBEDefinitionName(_accountBEDefinitionId), "_ChargingPolicies"); }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "Retail_BusinessEntity_ChargingPolicy_ViewHistoryItem"; }
            }


            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                ChargingPolicy chargingPolicy = context.Object.CastWithValidate<ChargingPolicy>("context.Object");
                return chargingPolicy.ChargingPolicyId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                ChargingPolicy chargingPolicy = context.Object.CastWithValidate<ChargingPolicy>("context.Object");
                return s_userManager.GetChargingPolicyName(chargingPolicy.ChargingPolicyId);
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

           
        }

        private class ChargingPolicyDetailExportExcelHandler : ExcelExportHandler<ChargingPolicyDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<ChargingPolicyDetail> context)
            {
                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Charging Policies",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Name"});
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Service Type" });
                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.ChargingPolicyId });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Name });
                            row.Cells.Add(new ExportExcelCell() { Value = record.ServiceTypeName });
                            sheet.Rows.Add(row);
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, ChargingPolicy> GetCachedChargingPolicies()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetChargingPolicies", () =>
            {
                IChargingPolicyDataManager dataManager = BEDataManagerFactory.GetDataManager<IChargingPolicyDataManager>();
                IEnumerable<ChargingPolicy> chargingPolicies = dataManager.GetChargingPolicies();
                return chargingPolicies.ToDictionary(kvp => kvp.ChargingPolicyId, kvp => kvp);
            });
        }

        #endregion

        #region Mappers

        private ChargingPolicyDetail ChargingPolicyDetailMapper(ChargingPolicy chargingPolicy)
        {
            return new ChargingPolicyDetail()
            {
                Entity = chargingPolicy,
                ServiceTypeName = _serviceTypeManager.GetServiceTypeName(chargingPolicy.ServiceTypeId),
                RuleDefinitions = this.GetChargingPolicyRuleDefinitions(chargingPolicy.ServiceTypeId),
                AccountBEDefinitionId = _serviceTypeManager.GetServiceType(chargingPolicy.ServiceTypeId).AccountBEDefinitionId,
            };
        }
        private ChargingPolicyInfo ChargingPolicyInfoMapper(ChargingPolicy chargingPolicy)
        {
            return new ChargingPolicyInfo
            {
                ChargingPolicyId = chargingPolicy.ChargingPolicyId,
                Name = chargingPolicy.Name
            };
        }
        private IEnumerable<ChargingPolicyRuleDefinition> GetChargingPolicyRuleDefinitions(Guid serviceTypeId)
        {
            ServiceType serviceType = _serviceTypeManager.GetServiceType(serviceTypeId);
            if (serviceType == null)
                throw new DataIntegrityValidationException("serviceType");
            if (serviceType.Settings == null)
                throw new DataIntegrityValidationException("serviceType.Settings");
            if (serviceType.Settings.ChargingPolicyDefinitionSettings == null)
                throw new DataIntegrityValidationException("serviceType.Settings.ChargingPolicyDefinitionSettings");
            return serviceType.Settings.ChargingPolicyDefinitionSettings.RuleDefinitions;
        }

        #endregion

        #region IBusinessEntityManager

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var cachedChargingPolicies = GetCachedChargingPolicies();
            if (cachedChargingPolicies != null)
                return cachedChargingPolicies.Values.Select(itm => itm as dynamic).ToList();
            else
                return null;
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetChargingPolicy(context.EntityId);
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetChargingPolicyName(Convert.ToInt32(context.EntityId));
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var chargingPolicy = context.Entity as ChargingPolicy;
            return chargingPolicy.ChargingPolicyId;
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
