using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Data;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Business
{
    public class RouteSyncDefinitionManager : IRouteSyncDefinitionManager
    {
        #region Public Methods
        public RouteSyncBPDefinitionSettings GetRouteSyncBPDefinitionSettings(int routeSyncDefinitionId)
        {
            throw new NotImplementedException();
        }

        public RouteSyncDefinition GetRouteSyncDefinitionById(int routeSyncDefinitionId,bool isViewedFromUI)
        {
            RouteSyncDefinition routeSyncDefinition;
            var allRouteSyncDefinitions = GetCachedRouteSyncDefinitions();
            if (!allRouteSyncDefinitions.TryGetValue(routeSyncDefinitionId, out routeSyncDefinition))
                throw new NullReferenceException(string.Format("GetRouteSyncDefinitionById : {0}", routeSyncDefinitionId));

            var routeSyncBPDefinition =allRouteSyncDefinitions[routeSyncDefinitionId];

             if (routeSyncBPDefinition != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(RouteSyncDefinitionLoggableEntity.Instance, routeSyncBPDefinition);
            return routeSyncBPDefinition;
        }

        public RouteSyncDefinition GetRouteSyncDefinitionById(int routeSyncDefinitionId)
        {
          return GetRouteSyncDefinitionById(routeSyncDefinitionId,false);
        }

        public string GetRouteSyncDefinitionName(RouteSyncDefinition routeSyncDefinition)
        {

            return (routeSyncDefinition != null) ? routeSyncDefinition.Name : null;
        }

        public Vanrise.Entities.InsertOperationOutput<RouteSyncDefinitionDetail> AddRouteSyncDefinition(RouteSyncDefinition routeSyncDefinitionItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<RouteSyncDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int reprocessDefintionId = -1;

            IRouteSyncDefinitionDataManager dataManager = RouteSyncDataManagerFactory.GetDataManager<IRouteSyncDefinitionDataManager>();

            if (dataManager.Insert(routeSyncDefinitionItem, out reprocessDefintionId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                routeSyncDefinitionItem.RouteSyncDefinitionId = reprocessDefintionId;
                VRActionLogger.Current.TrackAndLogObjectAdded(RouteSyncDefinitionLoggableEntity.Instance, routeSyncDefinitionItem);
                insertOperationOutput.InsertedObject = RouteSyncDefinitionDetailMapper(routeSyncDefinitionItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<RouteSyncDefinitionDetail> UpdateRouteSyncDefinition(RouteSyncDefinition routeSyncDefinitionItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<RouteSyncDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IRouteSyncDefinitionDataManager dataManager = RouteSyncDataManagerFactory.GetDataManager<IRouteSyncDefinitionDataManager>();

            if (dataManager.Update(routeSyncDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(RouteSyncDefinitionLoggableEntity.Instance, routeSyncDefinitionItem);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = RouteSyncDefinitionDetailMapper(this.GetRouteSyncDefinitionById(routeSyncDefinitionItem.RouteSyncDefinitionId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IDataRetrievalResult<RouteSyncDefinitionDetail> GetFilteredRouteSyncDefinitions(DataRetrievalInput<RouteSyncDefinitionQuery> input)
        {
            var allRouteSyncDefinitions = GetCachedRouteSyncDefinitions();
            Func<RouteSyncDefinition, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));

            ResultProcessingHandler<RouteSyncDefinitionDetail> handler = new ResultProcessingHandler<RouteSyncDefinitionDetail>()
            {
                ExportExcelHandler = new RouteSyncDefinitionExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(RouteSyncDefinitionLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allRouteSyncDefinitions.ToBigResult(input, filterExpression, RouteSyncDefinitionDetailMapper), handler);
        }

        public IEnumerable<RouteReaderConfig> GetRouteReaderExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<RouteReaderConfig>(RouteReaderConfig.EXTENSION_TYPE);
        }

        public IEnumerable<RouteSyncDefinitionInfo> GetRouteSyncDefinitionsInfo()
        {
            return GetCachedRouteSyncDefinitions().MapRecords(RouteSyncDefinitionInfoMapper).OrderBy(d => d.Name);
        }
        #endregion
         

        #region Private Classes

        private class RouteSyncDefinitionExcelExportHandler : ExcelExportHandler<RouteSyncDefinitionDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<RouteSyncDefinitionDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Route Sync Definitions",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name", Width = 30 });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.RouteSyncDefinitionId });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }


        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRouteSyncDefinitionDataManager _dataManager = RouteSyncDataManagerFactory.GetDataManager<IRouteSyncDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreRouteSyncDefinitionsUpdated(ref _updateHandle);
            }
        }

        private class RouteSyncDefinitionLoggableEntity : VRLoggableEntityBase
        {
            public static RouteSyncDefinitionLoggableEntity Instance = new RouteSyncDefinitionLoggableEntity();

            private RouteSyncDefinitionLoggableEntity()
            {

            }

            static RouteSyncDefinitionManager s_routeSyncDefinitionManager = new RouteSyncDefinitionManager();

            public override string EntityUniqueName
            {
                get { return "WhS_RouteSync_RouteSyncDefinition"; }
            }

            public override string ModuleName
            {
                get { return "Route Sync"; }
            }

            public override string EntityDisplayName
            {
                get { return "Route Sync Definition"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_RouteSync_RouteSyncDefinition_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                RouteSyncDefinition routeSyncDefinition = context.Object.CastWithValidate<RouteSyncDefinition>("context.Object");
                return routeSyncDefinition.RouteSyncDefinitionId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                RouteSyncDefinition routeSyncDefinition = context.Object.CastWithValidate<RouteSyncDefinition>("context.Object");
                return s_routeSyncDefinitionManager.GetRouteSyncDefinitionName(routeSyncDefinition);

            }
        }

        #endregion


        #region Private Methods
        Dictionary<int, RouteSyncDefinition> GetCachedRouteSyncDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRouteSyncDefinitions",
               () =>
               {
                   IRouteSyncDefinitionDataManager dataManager = RouteSyncDataManagerFactory.GetDataManager<IRouteSyncDefinitionDataManager>();
                   IEnumerable<RouteSyncDefinition> routeSyncDefinitions = dataManager.GetRouteSyncDefinitions();
                   return routeSyncDefinitions.ToDictionary(rsd => rsd.RouteSyncDefinitionId, rsd => rsd);
               });
        }
        #endregion


        #region Mappers
        public RouteSyncDefinitionDetail RouteSyncDefinitionDetailMapper(RouteSyncDefinition routeSyncDefinition)
        {
            RouteSyncDefinitionDetail routeSyncDefinitionDetail = new RouteSyncDefinitionDetail()
            {
                Entity = routeSyncDefinition
            };
            return routeSyncDefinitionDetail;
        }

        RouteSyncDefinitionInfo RouteSyncDefinitionInfoMapper(RouteSyncDefinition routeSyncDefinition)
        {
            return new RouteSyncDefinitionInfo
            {
                Name = routeSyncDefinition.Name,
                RouteSyncDefinitionId = routeSyncDefinition.RouteSyncDefinitionId
            };
        }
        #endregion
    }
}
