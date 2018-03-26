using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SwitchConnectivityManager : BaseBusinessEntityManager
    {
        #region Fields

        CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();
        SwitchManager _switchManager = new SwitchManager();

        #endregion

        #region Public Methods

        public SwitchConnectivity GetSwitchConnectivityHistoryDetailbyHistoryId(int switchConnectivityHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var switchConnectivity = s_vrObjectTrackingManager.GetObjectDetailById(switchConnectivityHistoryId);
            return switchConnectivity.CastWithValidate<SwitchConnectivity>("Switch Connectivity : historyId ", switchConnectivityHistoryId);
        }
        public Vanrise.Entities.IDataRetrievalResult<SwitchConnectivityDetail> GetFilteredSwitchConnectivities(Vanrise.Entities.DataRetrievalInput<SwitchConnectivityQuery> input)
        {
            Dictionary<int, SwitchConnectivity> cachedEntities = this.GetCachedSwitchConnectivities();

            Func<SwitchConnectivity, bool> filterExpression = (itm) =>
                (input.Query.Name == null || itm.Name.ToLower().Contains(input.Query.Name.ToLower())) &&
                (input.Query.CarrierAccountIds == null || input.Query.CarrierAccountIds.Contains(itm.CarrierAccountId)) &&
                (input.Query.SwitchIds == null || input.Query.SwitchIds.Contains(itm.SwitchId)) &&
                (input.Query.ConnectionTypes == null || input.Query.ConnectionTypes.Contains(itm.Settings.ConnectionType));

            var resultProcessingHandler = new ResultProcessingHandler<SwitchConnectivityDetail>()
            {
                ExportExcelHandler = new SwitchConnectivityExportExcelHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(SwitchConnectivityLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedEntities.ToBigResult(input, filterExpression, SwitchConnectivityDetailMapper), resultProcessingHandler);
        }

        public SwitchConnectivity GetSwitchConnectivity(int switchConnectivityId)
        {
            return GetSwitchConnectivity(switchConnectivityId, false);
        }

        public SwitchConnectivity GetSwitchConnectivity(int switchConnectivityId, bool isViewedFromUI)
        {
            Dictionary<int, SwitchConnectivity> cachedEntities = this.GetCachedSwitchConnectivities();
            var switchConnectivity = cachedEntities.GetRecord(switchConnectivityId);
            if (switchConnectivity != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(SwitchConnectivityLoggableEntity.Instance, switchConnectivity);
            return switchConnectivity;
        }

        public string GetSwitchConnectivityName(int switchConnectivityId)
        {
            SwitchConnectivity switchConnectivity = GetSwitchConnectivity(switchConnectivityId);
            if (switchConnectivity == null)
                return null;
            return switchConnectivity.Name;
        }

        public IEnumerable<SwitchConnectivityInfo> GetSwitcheConnectivitiesInfo()
        {
            return GetCachedSwitchConnectivities().MapRecords(SwitchConnectivityInfoMapper).OrderBy(x => x.Name);
        }

        public Vanrise.Entities.InsertOperationOutput<SwitchConnectivityDetail> AddSwitchConnectivity(SwitchConnectivity switchConnectivity)
        {
            ValidateSwitchConnectivityToAdd(switchConnectivity);

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SwitchConnectivityDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            ISwitchConnectivityDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchConnectivityDataManager>();
            int insertedId = -1;
            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();
            switchConnectivity.CreatedBy = loggedInUserId;
            switchConnectivity.LastModifiedBy = loggedInUserId;
            if (dataManager.Insert(switchConnectivity, out insertedId))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                switchConnectivity.SwitchConnectivityId = insertedId;
                VRActionLogger.Current.TrackAndLogObjectAdded(SwitchConnectivityLoggableEntity.Instance, switchConnectivity);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = SwitchConnectivityDetailMapper(switchConnectivity);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SwitchConnectivityDetail> UpdateSwitchConnectivity(SwitchConnectivityToEdit switchConnectivityToEdit)
        {
            ValidateSwitchConnectivityToEdit(switchConnectivityToEdit);

            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<SwitchConnectivityDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ISwitchConnectivityDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchConnectivityDataManager>();
            switchConnectivityToEdit.LastModifiedBy = SecurityContext.Current.GetLoggedInUserId();
            if (dataManager.Update(switchConnectivityToEdit))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var switchConnectivity = GetSwitchConnectivity(switchConnectivityToEdit.SwitchConnectivityId);
                VRActionLogger.Current.TrackAndLogObjectUpdated(SwitchConnectivityLoggableEntity.Instance, switchConnectivity);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SwitchConnectivityDetailMapper(this.GetSwitchConnectivity(switchConnectivityToEdit.SwitchConnectivityId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public SwitchConnectivity GetMatchConnectivity(int switchId, string port)
        {
            Dictionary<SwitchPortKey, SwitchConnectivity> switchConnectivityBySwitchPortKey = GetSwitchConnectivitiesBySwitchPortKey();
            if (switchConnectivityBySwitchPortKey == null)
                return null;

            SwitchPortKey switchPortKey = new SwitchPortKey() { SwitchId = switchId, Port = port };

            SwitchConnectivity connectivity;
            if (switchConnectivityBySwitchPortKey.TryGetValue(switchPortKey, out connectivity))
                return connectivity;
            else
                return null;
        }

        public string GetMatchConnectivityName(int switchId, string port)
        {
            var connectivity = GetMatchConnectivity(switchId, port);
            return connectivity != null ? connectivity.Name : null;
        }

        public Dictionary<SwitchPortKey, SwitchConnectivity> GetSwitchConnectivitiesBySwitchPortKey()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSwitchConnectivitiesBySwitchPort", () =>
            {
                Dictionary<SwitchPortKey, SwitchConnectivity> switchConnectivitiesByPort = new Dictionary<SwitchPortKey, SwitchConnectivity>();

                var switchConnectivitiesById = GetCachedSwitchConnectivities();
                if (switchConnectivitiesById != null)
                {
                    foreach (var switchConnectivity in switchConnectivitiesById.Values)
                    {
                        if (switchConnectivity.Settings != null && switchConnectivity.Settings.Trunks != null)
                        {
                            foreach (var trunk in switchConnectivity.Settings.Trunks)
                            {
                                SwitchPortKey switchPortKey = new SwitchPortKey() { SwitchId = switchConnectivity.SwitchId, Port = trunk.Name };
                                if (!switchConnectivitiesByPort.ContainsKey(switchPortKey))
                                    switchConnectivitiesByPort.Add(switchPortKey, switchConnectivity);
                            }
                        }
                    }
                }
                return switchConnectivitiesByPort;
            });
        }

        #endregion

        #region Validation Methods

        void ValidateSwitchConnectivityToAdd(SwitchConnectivity switchConnectivity)
        {
            ValidateSwitchConnectivity(switchConnectivity.Name, switchConnectivity.CarrierAccountId, switchConnectivity.SwitchId, switchConnectivity.Settings, switchConnectivity.BED);
        }

        void ValidateSwitchConnectivityToEdit(SwitchConnectivityToEdit switchConnectivity)
        {
            ValidateSwitchConnectivity(switchConnectivity.Name, switchConnectivity.CarrierAccountId, switchConnectivity.SwitchId, switchConnectivity.Settings, switchConnectivity.BED);
        }

        void ValidateSwitchConnectivity(string scName, int scCarrierAccountId, int scSwitchId, SwitchConnectivitySettings scSettings, DateTime scBED)
        {
            if (String.IsNullOrWhiteSpace(scName))
                throw new MissingArgumentValidationException("SwitchConnectivity.Name");

            var carrierAccountManager = new CarrierAccountManager();
            var carrierAccount = carrierAccountManager.GetCarrierAccount(scCarrierAccountId);
            if (carrierAccount == null)
                throw new DataIntegrityValidationException(String.Format("CarrierAccount '{0}' does not exist", scCarrierAccountId));

            var switchManager = new SwitchManager();
            var whsSwitch = switchManager.GetSwitch(scSwitchId);
            if (whsSwitch == null)
                throw new DataIntegrityValidationException(String.Format("Switch '{0}' does not exist", scSwitchId));

            if (scSettings == null)
                throw new MissingArgumentValidationException("SwitchConnectivity.Settings");

            if (scSettings.Trunks == null || scSettings.Trunks.Count == 0)
                throw new MissingArgumentValidationException("SwitchConnectivity.Settings.Trunks");

            var existingTrunkNames = new List<string>();
            for (int i = 0; i < scSettings.Trunks.Count; i++)
            {
                SwitchConnectivityTrunk trunk = scSettings.Trunks[i];
                if (trunk == null || String.IsNullOrWhiteSpace(trunk.Name))
                    throw new MissingArgumentValidationException(String.Format("SwitchConnectivityTrunk '{0}'", (i + 1)));
                string trunkName = trunk.Name.ToLower();
                string existingTrunkName = existingTrunkNames.FindRecord(itm => itm == trunkName);
                if (existingTrunkName != null)
                    throw new DataIntegrityValidationException(String.Format("Trunk '{0}' exists multiple times", trunk.Name));
                existingTrunkNames.Add(trunkName);
            }

            if (scBED == default(DateTime))
                throw new MissingArgumentValidationException("SwitchConnectivity.BED");
        }

        #endregion

        #region Private Classes

        private class SwitchConnectivityExportExcelHandler : ExcelExportHandler<SwitchConnectivityDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SwitchConnectivityDetail> context)
            {
                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Switch Connectivity",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Carrier Account Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Switch Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.SwitchConnectivityId });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Name });
                            row.Cells.Add(new ExportExcelCell() { Value = record.CarrierAccountName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.SwitchName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.BED });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.EED });
                            sheet.Rows.Add(row);
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }
        private class SwitchConnectivityLoggableEntity : VRLoggableEntityBase
        {
            public static SwitchConnectivityLoggableEntity Instance = new SwitchConnectivityLoggableEntity();

            private SwitchConnectivityLoggableEntity()
            {

            }

            static SwitchConnectivityManager s_switchConnectivityManager = new SwitchConnectivityManager();

            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_SwitchConnectivity"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Switch Connectivity"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_SwitchConnectivity_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                SwitchConnectivity switchConnectivity = context.Object.CastWithValidate<SwitchConnectivity>("context.Object");
                return switchConnectivity.SwitchConnectivityId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                SwitchConnectivity switchConnectivity = context.Object.CastWithValidate<SwitchConnectivity>("context.Object");
                return s_switchConnectivityManager.GetSwitchConnectivityName(switchConnectivity.SwitchConnectivityId);
            }
        }
        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            DateTime? _carrierAccountLastCheck;

            ISwitchConnectivityDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISwitchConnectivityDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSwitchConnectivitiesUpdated(ref _updateHandle)
                    | Vanrise.Caching.CacheManagerFactory.GetCacheManager<CarrierAccountManager.CacheManager>().IsCacheExpired(ref _carrierAccountLastCheck);
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<int, SwitchConnectivity> GetCachedSwitchConnectivities()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSwitchConnectivities", () =>
            {
                ISwitchConnectivityDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchConnectivityDataManager>();
                IEnumerable<SwitchConnectivity> switchConnectivities = dataManager.GetSwitchConnectivities();
                Dictionary<int, SwitchConnectivity> dic = new Dictionary<int, SwitchConnectivity>();
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

                foreach (SwitchConnectivity item in switchConnectivities)
                {
                    if (!carrierAccountManager.IsCarrierAccountDeleted(item.CarrierAccountId))
                        dic.Add(item.SwitchConnectivityId, item);
                }

                return dic;
            });
        }

        #endregion

        #region Mappers

        SwitchConnectivityDetail SwitchConnectivityDetailMapper(SwitchConnectivity switchConnectivity)
        {
            return new SwitchConnectivityDetail()
            {
                Entity = switchConnectivity,
                CarrierAccountName = _carrierAccountManager.GetCarrierAccountName(switchConnectivity.CarrierAccountId),
                SwitchName = _switchManager.GetSwitchName(switchConnectivity.SwitchId)
            };
        }
        private SwitchConnectivityInfo SwitchConnectivityInfoMapper(SwitchConnectivity switchConnectivity)
        {
            return new SwitchConnectivityInfo()
            {
                SwitchConnectivityId = switchConnectivity.SwitchConnectivityId,
                Name = switchConnectivity.Name,
            };
        }
        #endregion

        #region IBusinessEntityManager

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetSwitchConnectivity(context.EntityId);
        }

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var switchConnectivities = GetCachedSwitchConnectivities();
            if (switchConnectivities == null)
                return null;
            else
                return switchConnectivities.Select(itm => itm as dynamic).ToList();
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetSwitchConnectivityName(Convert.ToInt32(context.EntityId));
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var switchConnectivity = context.Entity as SwitchConnectivity;
            return switchConnectivity.SwitchConnectivityId;
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

    public struct SwitchPortKey
    {
        public int SwitchId { get; set; }

        public string Port { get; set; }
    }
}
