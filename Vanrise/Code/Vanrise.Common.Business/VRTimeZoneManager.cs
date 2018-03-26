using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class VRTimeZoneManager : BaseBusinessEntityManager
    {
        #region Public Methods

        public IDataRetrievalResult<VRTimeZoneDetail> GetFilteredVRTimeZones(Vanrise.Entities.DataRetrievalInput<VRTimeZoneQuery> input)
        {
            var allTimeZones = GetCachedVRTimeZones();

            Func<VRTimeZone, bool> filterExpression = (prod) =>
            {
                if (input.Query.Name != null && !prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };

            ResultProcessingHandler<VRTimeZoneDetail> handler = new ResultProcessingHandler<VRTimeZoneDetail>()
            {
                ExportExcelHandler = new VRTimeZoneExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(VRTimeZoneLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allTimeZones.ToBigResult(input, filterExpression, VRTimeZoneDetailMapper), handler);
        }

        public IEnumerable<VRTimeZoneInfo> GetVRTimeZonesInfo()
        {
            return this.GetCachedVRTimeZones().MapRecords(VRTimeZoneInfoMapper).OrderBy(timeZone => timeZone.Name);
        }

        public string GetVRTimeZoneName(int timeZoneId)
        {
            var timeZone =  GetVRTimeZone(timeZoneId);
            return (timeZone != null) ? timeZone.Name : null ;
        }
        public VRTimeZone GetVRTimeZone(int timeZoneId, bool isViewedFromUI)
        {
            var timeZones = GetCachedVRTimeZones();
           var timeZone = timeZones.GetRecord(timeZoneId);
           if (timeZone != null && isViewedFromUI)
               VRActionLogger.Current.LogObjectViewed(VRTimeZoneLoggableEntity.Instance, timeZone);
           return timeZone;
        }
        public VRTimeZone GetVRTimeZone(int timeZoneId)
        {
           return GetVRTimeZone(timeZoneId,false);
        }
        public Vanrise.Entities.InsertOperationOutput<VRTimeZoneDetail> AddVRTimeZone(VRTimeZone timeZone)
        {
            Vanrise.Entities.InsertOperationOutput<VRTimeZoneDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRTimeZoneDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int timeZoneId = -1;

            IVRTimeZoneDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRTimeZoneDataManager>();
            
            int loggedInUserId = ContextFactory.GetContext().GetLoggedInUserId();
            timeZone.CreatedBy = loggedInUserId;
            timeZone.LastModifiedBy = loggedInUserId;

            bool insertActionSucc = dataManager.Insert(timeZone, out timeZoneId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                timeZone.TimeZoneId = timeZoneId;
                VRActionLogger.Current.TrackAndLogObjectAdded(VRTimeZoneLoggableEntity.Instance, timeZone);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRTimeZoneDetailMapper(timeZone);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<VRTimeZoneDetail> UpdateVRTimeZone(VRTimeZone timeZone)
        {
            IVRTimeZoneDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRTimeZoneDataManager>();

            timeZone.LastModifiedBy = ContextFactory.GetContext().GetLoggedInUserId();

            bool updateActionSucc = dataManager.Update(timeZone);
            Vanrise.Entities.UpdateOperationOutput<VRTimeZoneDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRTimeZoneDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(VRTimeZoneLoggableEntity.Instance, timeZone);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRTimeZoneDetailMapper(timeZone);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }


        public int GetVRTimeZoneTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetVRTimeZoneType());
        }

        public Type GetVRTimeZoneType()
        {
            return this.GetType();
        }

        #endregion

        #region Private Classes
        private class VRTimeZoneExcelExportHandler : ExcelExportHandler<VRTimeZoneDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<VRTimeZoneDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Time Zones",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Time Shift" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.TimeZoneId });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Settings.Offset });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRTimeZoneDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRTimeZoneDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRTimeZonesUpdated(ref _updateHandle);
            }
        }
        private class VRTimeZoneLoggableEntity : VRLoggableEntityBase
        {
            public static VRTimeZoneLoggableEntity Instance = new VRTimeZoneLoggableEntity();

            private VRTimeZoneLoggableEntity()
            {

            }

            static VRTimeZoneManager s_timeZoneManager = new VRTimeZoneManager();

            public override string EntityUniqueName
            {
                get { return "VR_Common_TimeZone"; }
            }

            public override string ModuleName
            {
                get { return "Common"; }
            }

            public override string EntityDisplayName
            {
                get { return "Time Zone"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Common_TimeZone_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                VRTimeZone vrtimeZone = context.Object.CastWithValidate<VRTimeZone>("context.Object");
                return vrtimeZone.TimeZoneId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                VRTimeZone vrtimeZone = context.Object.CastWithValidate<VRTimeZone>("context.Object");
                return s_timeZoneManager.GetVRTimeZoneName(vrtimeZone.TimeZoneId);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<int, VRTimeZone> GetCachedVRTimeZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetTimeZones",
              () =>
              {
                  IVRTimeZoneDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRTimeZoneDataManager>();
                  IEnumerable<VRTimeZone> TimeZones = dataManager.GetVRTimeZones();
                  return TimeZones.ToDictionary(c => c.TimeZoneId, c => c);
              });
        }

        #endregion

        #region Mappers

        public VRTimeZoneDetail VRTimeZoneDetailMapper(VRTimeZone timeZone)
        {
            VRTimeZoneDetail timeZoneDetail = new VRTimeZoneDetail();
            timeZoneDetail.Entity = timeZone;
            return timeZoneDetail;
        }

        public VRTimeZoneInfo VRTimeZoneInfoMapper(VRTimeZone timeZone)
        {
            VRTimeZoneInfo timeZoneInfo = new VRTimeZoneInfo();
            timeZoneInfo.TimeZoneId = timeZone.TimeZoneId;
            timeZoneInfo.Name = timeZone.Name;
            return timeZoneInfo;
        }

        #endregion


        #region IBusinessEntityManager
        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetCachedVRTimeZones().Select(itm => itm as dynamic).ToList();
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetVRTimeZone(context.EntityId);
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var timeZone = context.Entity as VRTimeZone;
            return timeZone.TimeZoneId;
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetVRTimeZoneName(Convert.ToInt32(context.EntityId));
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
