using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRTimeZoneManager
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

            VRTimeZoneExcelExportHandler vRTimeZoneExcel = new VRTimeZoneExcelExportHandler(input.Query);
            ResultProcessingHandler<VRTimeZoneDetail> handler = new ResultProcessingHandler<VRTimeZoneDetail>()
            {
                ExportExcelHandler = vRTimeZoneExcel
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allTimeZones.ToBigResult(input, filterExpression, VRTimeZoneDetailMapper), handler);
        }
        private class VRTimeZoneExcelExportHandler : ExcelExportHandler<VRTimeZoneDetail>
        {
            private VRTimeZoneQuery _query;
            public VRTimeZoneExcelExportHandler(VRTimeZoneQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<VRTimeZoneDetail> context)
            {
                if (context.BigResult == null)
                    throw new ArgumentNullException("context.BigResult");
                if (context.BigResult.Data == null)
                    throw new ArgumentNullException("context.BigResult.Data");
                ExportExcelSheet sheet = new ExportExcelSheet();
                sheet.Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Time Shift" });


                sheet.Rows = new List<ExportExcelRow>();
                foreach (var record in context.BigResult.Data)
                {
                    var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                    sheet.Rows.Add(row);
                    row.Cells.Add(new ExportExcelCell { Value = record.Entity.TimeZoneId });
                    row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                    row.Cells.Add(new ExportExcelCell { Value = record.Entity.Settings.Offset });
                }
                context.MainSheet = sheet;
            }
        }

        public IEnumerable<VRTimeZoneInfo> GetVRTimeZonesInfo()
        {
            return this.GetCachedVRTimeZones().MapRecords(VRTimeZoneInfoMapper).OrderBy(timeZone => timeZone.Name);
        }

       
        public VRTimeZone GetVRTimeZone(int timeZoneId)
        {
            var TimeZones = GetCachedVRTimeZones();
            return TimeZones.GetRecord(timeZoneId);
        }

        public Vanrise.Entities.InsertOperationOutput<VRTimeZoneDetail> AddVRTimeZone(VRTimeZone timeZone)
        {
            Vanrise.Entities.InsertOperationOutput<VRTimeZoneDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRTimeZoneDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int timeZoneId = -1;

            IVRTimeZoneDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRTimeZoneDataManager>();
            bool insertActionSucc = dataManager.Insert(timeZone, out timeZoneId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                timeZone.TimeZoneId = timeZoneId;
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

            bool updateActionSucc = dataManager.Update(timeZone);
            Vanrise.Entities.UpdateOperationOutput<VRTimeZoneDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRTimeZoneDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
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

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRTimeZoneDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRTimeZoneDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRTimeZonesUpdated(ref _updateHandle);
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
    }
}
