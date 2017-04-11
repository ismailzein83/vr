﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Normalization;
using Vanrise.Rules.Normalization;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SwitchManager : IBusinessEntityManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<SwitchDetail> GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
        {
            var allSwitches = GetCachedSwitches();

            Func<Switch, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()));

            ResultProcessingHandler<SwitchDetail> handler = new ResultProcessingHandler<SwitchDetail>()
            {
                ExportExcelHandler = new SwitchExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(SwitchLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSwitches.ToBigResult(input, filterExpression, SwitchDetailMapper), handler);
        }
        private class SwitchExcelExportHandler : ExcelExportHandler<SwitchDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SwitchDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Switches",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };
                
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Switch Name" });
                
                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.SwitchId });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        public Switch GetSwitch(int switchId, bool isViewedFromUI)
        {
            var switchs = GetCachedSwitches();
           var swicth= switchs.GetRecord(switchId);
           if (swicth != null && isViewedFromUI)
               VRActionLogger.Current.LogObjectViewed(SwitchLoggableEntity.Instance, swicth);
           return swicth;
        }
        public Switch GetSwitch(int switchId)
        {
            return GetSwitch(switchId, false);
        }
        public List<Switch> GetAllSwitches()
        {
            var cachedSwitches = GetCachedSwitches();
            return cachedSwitches != null ? cachedSwitches.Values.ToList() : null;
        }

        public IEnumerable<SwitchInfo> GetSwitchesInfo()
        {
            return GetCachedSwitches().MapRecords(SwitchInfoMapper).OrderBy(x => x.Name);
        }

        public string GetSwitchName(int switchId)
        {
            Switch switchObj = GetSwitch(switchId);

            if (switchObj != null)
                return switchObj.Name;

            return null;
        }

        public Vanrise.Entities.InsertOperationOutput<SwitchDetail> AddSwitch(Switch whsSwitch)
        {
            ValidateSwitchToAdd(whsSwitch);

            Vanrise.Entities.InsertOperationOutput<SwitchDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SwitchDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int switchId = -1;

            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            bool insertActionSucc = dataManager.Insert(whsSwitch, out switchId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                whsSwitch.SwitchId = switchId;
                VRActionLogger.Current.TrackAndLogObjectAdded(SwitchLoggableEntity.Instance, whsSwitch);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = SwitchDetailMapper(whsSwitch);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SwitchDetail> UpdateSwitch(SwitchToEdit whsSwitch)
        {
            ValidateSwitchToUpdate(whsSwitch);

            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();

            bool updateActionSucc = dataManager.Update(whsSwitch);
            Vanrise.Entities.UpdateOperationOutput<SwitchDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<SwitchDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var switchItem = GetSwitch(whsSwitch.SwitchId);
                VRActionLogger.Current.TrackAndLogObjectUpdated(SwitchLoggableEntity.Instance, switchItem);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SwitchDetailMapper(this.GetSwitch(whsSwitch.SwitchId));
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }

        public Vanrise.Entities.DeleteOperationOutput<SwitchDetail> DeleteSwitch(int switchId)
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            Vanrise.Entities.DeleteOperationOutput<SwitchDetail> deleteOperationOutput = new Vanrise.Entities.DeleteOperationOutput<SwitchDetail>();
            bool updateActionSucc = dataManager.Delete(switchId);
            if (updateActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }
            else
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;
            return deleteOperationOutput;
        }

        public SwitchCDPNsForIdentification GetSwitchCDPNsForIdentification(int switchId, string inputCDPN, string cdpnIn, string cdpnOut)
        {
            Dictionary<SwitchCDPN, CDPNIdentification> mappingResults = GetCachedMappingSwitchCDPNs(switchId);

            string customerCDPN = GetCDPNValueForIdentification(mappingResults.GetRecord(SwitchCDPN.CustomerCDPN), inputCDPN, cdpnIn, cdpnOut);
            string supplierCDPN = GetCDPNValueForIdentification(mappingResults.GetRecord(SwitchCDPN.SupplierCDPN), inputCDPN, cdpnIn, cdpnOut);
            string outputCDPN = GetCDPNValueForIdentification(mappingResults.GetRecord(SwitchCDPN.CDPN), inputCDPN, cdpnIn, cdpnOut);

            return new SwitchCDPNsForIdentification
            {
                CustomerCDPN = customerCDPN,
                SupplierCDPN = supplierCDPN,
                OutputCDPN = outputCDPN
            };
        }

        public SwitchCDPNsForZoneMatch GetSwitchCDPNsForZoneMatch(int switchId, string cdpn, string cdpnIn, string cdpnOut, Guid normalizationRuleDefinitionId, DateTime effectiveTime)
        {
            Dictionary<SwitchCDPN, CDPNIdentification> mappingResults = GetCachedMappingSwitchCDPNs(switchId);

            string saleZoneCDPN = GetCDPNValueForZoneMatch(mappingResults.GetRecord(SwitchCDPN.SaleZoneCDPN), cdpn, cdpnIn, cdpnOut, normalizationRuleDefinitionId, effectiveTime, switchId);
            string supplierZoneCDPN = GetCDPNValueForZoneMatch(mappingResults.GetRecord(SwitchCDPN.SupplierZoneCDPN), cdpn, cdpnIn, cdpnOut, normalizationRuleDefinitionId, effectiveTime, switchId);

            return new SwitchCDPNsForZoneMatch
            {
                SaleZoneCDPN = saleZoneCDPN,
                SupplierZoneCDPN = supplierZoneCDPN,
            };
        }

        private Dictionary<SwitchCDPN, CDPNIdentification> GetCachedMappingSwitchCDPNs(int switchId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetMappingSwitchCDPNs",
                () =>
                {
                    ConfigManager configManager = new ConfigManager();
                    Switch currentSwitch = this.GetSwitch(switchId);
                    if (currentSwitch == null)
                        throw new NullReferenceException(string.Format("currentSwitch for ID: {0}", switchId));

                    SwitchCDRMappingConfiguration switchCDRMappingConfiguration = currentSwitch.Settings != null ? currentSwitch.Settings.SwitchCDRMappingConfiguration : null;

                    Dictionary<SwitchCDPN, CDPNIdentification> mappingResults = new Dictionary<SwitchCDPN, CDPNIdentification>();
                    if (switchCDRMappingConfiguration != null)
                    {
                        mappingResults.Add(SwitchCDPN.CDPN, GetCorrespondingCDPNIdentification(switchCDRMappingConfiguration.GeneralIdentification, configManager.GetGeneralCDPNIndentification()));
                        mappingResults.Add(SwitchCDPN.CustomerCDPN, GetCorrespondingCDPNIdentification(switchCDRMappingConfiguration.CustomerIdentification, configManager.GetCustomerCDPNIndentification()));
                        mappingResults.Add(SwitchCDPN.SupplierCDPN, GetCorrespondingCDPNIdentification(switchCDRMappingConfiguration.SupplierIdentification, configManager.GetSupplierCDPNIndentification()));
                        mappingResults.Add(SwitchCDPN.SaleZoneCDPN, GetCorrespondingCDPNIdentification(switchCDRMappingConfiguration.SaleZoneIdentification, configManager.GetSaleZoneCDPNIndentification()));
                        mappingResults.Add(SwitchCDPN.SupplierZoneCDPN, GetCorrespondingCDPNIdentification(switchCDRMappingConfiguration.SupplierZoneIdentification, configManager.GetSupplierZoneCDPNIndentification()));
                    }
                    else
                    {
                        mappingResults.Add(SwitchCDPN.CDPN, configManager.GetGeneralCDPNIndentification());
                        mappingResults.Add(SwitchCDPN.CustomerCDPN, configManager.GetCustomerCDPNIndentification());
                        mappingResults.Add(SwitchCDPN.SupplierCDPN, configManager.GetSupplierCDPNIndentification());
                        mappingResults.Add(SwitchCDPN.SaleZoneCDPN, configManager.GetSaleZoneCDPNIndentification());
                        mappingResults.Add(SwitchCDPN.SupplierZoneCDPN, configManager.GetSupplierZoneCDPNIndentification());
                    }
                    return mappingResults;
                });
        }

        #endregion

        #region Validation Methods

        void ValidateSwitchToAdd(Switch whsSwitch)
        {
            ValidateSwitch(whsSwitch.Name);
        }

        void ValidateSwitchToUpdate(SwitchToEdit whsSwitch)
        {
            ValidateSwitch(whsSwitch.Name);
        }

        void ValidateSwitch(string sName)
        {
            if (String.IsNullOrWhiteSpace(sName))
                throw new MissingArgumentValidationException("Switch.Name");
        }

        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            DateTime? _SettingsCacheLastCheck;

            ISwitchDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSwitchesUpdated(ref _updateHandle)
                    |
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<SettingManager.CacheManager>().IsCacheExpired(ref _SettingsCacheLastCheck);
            }
        }
        public class SwitchLoggableEntity : VRLoggableEntityBase
        {
            public static SwitchLoggableEntity Instance = new SwitchLoggableEntity();

            private SwitchLoggableEntity()
            {

            }

            static SwitchManager s_switchManager = new SwitchManager();

            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_Switch"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Switch"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_Switch_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                Switch switchItem = context.Object.CastWithValidate<Switch>("context.Object");
                return switchItem.SwitchId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                Switch switchItem = context.Object.CastWithValidate<Switch>("context.Object");
                return s_switchManager.GetSwitchName(switchItem.SwitchId);
            }
        }
        #endregion

        #region Private Methods

        Dictionary<int, Switch> GetCachedSwitches()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSwitches",
               () =>
               {
                   ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
                   IEnumerable<Switch> switchs = dataManager.GetSwitches();
                   return switchs.ToDictionary(cn => cn.SwitchId, cn => cn);
               });
        }

        private CDPNIdentification GetCorrespondingCDPNIdentification(CDPNIdentification? switchCDPNIdentification, CDPNIdentification defaultCDPNIdentification)
        {
            if (switchCDPNIdentification != null && switchCDPNIdentification.HasValue)
                return switchCDPNIdentification.Value;

            return defaultCDPNIdentification;
        }

        private string GetCDPNValueForIdentification(CDPNIdentification cdpnIdentification, string cdpn, string cdpnIn, string cdpnOut)
        {
            switch (cdpnIdentification)
            {
                case CDPNIdentification.CDPN: return cdpn;
                case CDPNIdentification.CDPNIn: return cdpnIn;
                case CDPNIdentification.CDPNOut: return cdpnOut;
                default: throw new NotSupportedException("cdpnIdentification");
            }
        }

        private string GetCDPNValueForZoneMatch(CDPNIdentification cdpnIdentification, string cdpn, string cdpnIn, string cdpnOut, Guid normalizationRuleDefinitionId,
            DateTime effectiveTime, int switchId)
        {
            switch (cdpnIdentification)
            {
                case CDPNIdentification.CDPN: return cdpn;
                case CDPNIdentification.CDPNIn: return cdpnIn;
                case CDPNIdentification.CDPNOut: return cdpnOut;
                case CDPNIdentification.NormalizedCDPN: return GetCDPNNormalizedValue(cdpn, normalizationRuleDefinitionId, effectiveTime, switchId);
                case CDPNIdentification.NormalizedCDPNIn: return GetCDPNNormalizedValue(cdpnIn, normalizationRuleDefinitionId, effectiveTime, switchId);
                case CDPNIdentification.NormalizedCDPNOut: return GetCDPNNormalizedValue(cdpnOut, normalizationRuleDefinitionId, effectiveTime, switchId);
                default: throw new NotSupportedException("cdpnIdentification");
            }
        }

        private string GetCDPNNormalizedValue(string cdpn, Guid normalizationRuleDefinitionId, DateTime effectiveTime, int switchId)
        {
            int cdpnChoiceValue = 1;

            var normalizeRuleContext = new Vanrise.GenericData.Normalization.NormalizeRuleContext();
            normalizeRuleContext.Value = cdpn;

            var genericRuleTarget = new Vanrise.GenericData.Entities.GenericRuleTarget();
            genericRuleTarget.EffectiveOn = effectiveTime;
            genericRuleTarget.TargetFieldValues = new Dictionary<string, object>();
            genericRuleTarget.TargetFieldValues.Add("NumberType", cdpnChoiceValue);
            genericRuleTarget.TargetFieldValues.Add("Switch", switchId);
            if (!string.IsNullOrEmpty(cdpn))
            {
                genericRuleTarget.TargetFieldValues.Add("NumberPrefix", cdpn);
                genericRuleTarget.TargetFieldValues.Add("NumberLength", cdpn.Length);
            }

            var normalizationRuleManager = new Vanrise.GenericData.Normalization.NormalizationRuleManager();
            normalizationRuleManager.ApplyNormalizationRule(normalizeRuleContext, normalizationRuleDefinitionId, genericRuleTarget);

            return normalizeRuleContext.NormalizedValue;
        }

        #endregion

        #region Mappers

        private SwitchInfo SwitchInfoMapper(Switch whsSwitch)
        {
            return new SwitchInfo()
            {
                SwitchId = whsSwitch.SwitchId,
                Name = whsSwitch.Name,
            };
        }

        private SwitchDetail SwitchDetailMapper(Switch whsSwitch)
        {
            SwitchDetail switchDetail = new SwitchDetail();
            switchDetail.Entity = whsSwitch;
            return switchDetail;
        }

        #endregion

        #region IBusinessEntityManager

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetSwitch(context.EntityId);
        }

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var switches = GetCachedSwitches();
            if (switches == null)
                return null;
            else
                return switches.Select(itm => itm as dynamic).ToList();
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetSwitchName(Convert.ToInt32(context.EntityId));
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var _switch = context.Entity as Switch;
            return _switch.SwitchId;
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}