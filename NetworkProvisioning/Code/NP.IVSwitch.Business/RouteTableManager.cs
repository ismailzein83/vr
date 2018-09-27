using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using NP.IVSwitch.Data;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;
using NP.IVSwitch.Entities.RouteTable;
using Vanrise.Rules;
using Vanrise.Caching;

namespace NP.IVSwitch.Business
{
    public class RouteTableManager
    {
        EndPointManager endPointManager = new EndPointManager();
        #region Public Methods
        public IDataRetrievalResult<RouteTableDetails> GetFilteredRouteTables(DataRetrievalInput<RouteTableQuery> input)
        {
            var allRouteTables = GetCachedRouteTables();
            Dictionary<int, RouteEndPoints> allRouteTableEndPoints=new Dictionary<int,RouteEndPoints>();
            switch (input.Query.RouteTableViewType)
            {
                case RouteTableViewType.ANumber:
                    allRouteTableEndPoints = GetCachedRouteTablesEndPointsANumber();
                    break;
                case RouteTableViewType.Whitelist:
                    allRouteTableEndPoints = GetCachedRouteTablesEndPointsWhitelist();
                    break;
                case RouteTableViewType.BNumber:
                    allRouteTableEndPoints = GetCachedRouteTablesEndPointsBNumber();
                    break;
            }



            Func<RouteTable, bool> filterExpression = (routeTable) =>
            {
                if (input.Query.Name != null && !routeTable.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                
                if (!allRouteTableEndPoints.ContainsKey(routeTable.RouteTableId))
                    return false;

                if (input.Query.CustomerIds != null && input.Query.CustomerIds.Count > 0)
                {
                    if (input.Query.EndPoints != null && input.Query.EndPoints.Count > 0)
                    {
                        var routTableEndPoints = allRouteTableEndPoints.GetRecord(routeTable.RouteTableId);
                        if (routTableEndPoints == null || !routTableEndPoints.EndPoints.Any(x => input.Query.EndPoints.Contains(x.EndPointId)))
                        {
                            return false;
                        }

                    }
                }
                return true;

            };
            return DataRetrievalManager.Instance.ProcessResult(input, allRouteTables.ToBigResult(input, filterExpression, RouteTableDetailMapper));
        }

        public InsertOperationOutput<RouteTableDetails> AddRouteTable(RouteTableInput routeTableItem)
        {
            IRouteTableDataManager routeTableDataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableDataManager>();
            Helper.SetSwitchConfig(routeTableDataManager);

            InsertOperationOutput<RouteTableDetails> insertOperationOutput = new InsertOperationOutput<RouteTableDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            if (routeTableItem.RouteTableViewType == RouteTableViewType.ANumber)
                routeTableItem.RouteTable.Name = routeTableItem.RouteTable.Name + "_ANumber";
            if (routeTableItem.RouteTableViewType == RouteTableViewType.Whitelist)
                routeTableItem.RouteTable.Name = routeTableItem.RouteTable.Name + "_Whitelist";
            int routeTableId = -1;
            if (routeTableItem.RouteTableViewType != RouteTableViewType.BNumber)
            {
                bool insertActionSuccess = routeTableDataManager.Insert(routeTableItem, out routeTableId);
                if (insertActionSuccess)
                {
                    RouteTableRouteManager routeTableRouteManager = new RouteTableRouteManager();
                    routeTableRouteManager.CreateRouteTableRoute(routeTableId);
                    endPointManager.EndPointAclUpdate(routeTableItem.EndPoints.MapRecords(x => x.EndPointId), routeTableId, routeTableItem.RouteTableViewType);
                    routeTableItem.RouteTable.RouteTableId = routeTableId;
                    CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    insertOperationOutput.Result = InsertOperationResult.Succeeded;
                    insertOperationOutput.InsertedObject = RouteTableDetailMapper(routeTableItem.RouteTable);
                }
                else
                {
                    insertOperationOutput.Result = InsertOperationResult.SameExists;
                }
            }
            else
            {
                insertOperationOutput.Message = "you cannot add a BNumber RouteTable";
                insertOperationOutput.ShowExactMessage = true;
                
            }
            return insertOperationOutput;
        }


        public UpdateOperationOutput<RouteTableDetails> UpdateRouteTable(RouteTableInput routeTableItem)
        {
            IRouteTableDataManager routeTableDataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableDataManager>();
            Helper.SetSwitchConfig(routeTableDataManager);

            UpdateOperationOutput<RouteTableDetails> updateOperationOutput = new UpdateOperationOutput<RouteTableDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            if (routeTableItem.RouteTableViewType != RouteTableViewType.BNumber)
            {
                List<RouteTableEndPoint> endPointsToLink = new List<RouteTableEndPoint>();
                List<RouteTableEndPoint> endPointsToUnlink = new List<RouteTableEndPoint>();
                RuntimeEditorEntity runtimeEditorEntity = this.GetRouteTableById(routeTableItem.RouteTable.RouteTableId, routeTableItem.RouteTableViewType);
                if (routeTableItem.EndPoints != null && runtimeEditorEntity.RouteTableInput.EndPoints != null)
                {
                    foreach (RouteTableEndPoint newItem in routeTableItem.EndPoints)
                    {
                        var endPoint = runtimeEditorEntity.RouteTableInput.EndPoints.FindRecord(x => x.EndPointId == newItem.EndPointId);
                        if (endPoint == null)
                            endPointsToLink.Add(newItem);
                    }

                    foreach (RouteTableEndPoint oldItem in runtimeEditorEntity.RouteTableInput.EndPoints)
                    {
                        var endPoint = routeTableItem.EndPoints.FindRecord(x => x.EndPointId == oldItem.EndPointId);
                        if (endPoint == null)
                            endPointsToUnlink.Add(oldItem);
                    }
                }
                if (endPointsToUnlink != null && endPointsToUnlink.Count > 0)
                    endPointManager.EndPointAclUpdate(endPointsToUnlink.MapRecords(x => x.EndPointId), 0, routeTableItem.RouteTableViewType);
                if (endPointsToLink != null && endPointsToLink.Count > 0)
                    endPointManager.EndPointAclUpdate(endPointsToLink.MapRecords(x => x.EndPointId), routeTableItem.RouteTable.RouteTableId, routeTableItem.RouteTableViewType);


                bool updateActionSuccess = routeTableDataManager.Update(routeTableItem);
                if (updateActionSuccess)
                {
                    CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                    updateOperationOutput.UpdatedObject = RouteTableDetailMapper(routeTableItem.RouteTable);
                }
                else
                    updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            else
            {
                updateOperationOutput.Message = "you cannot Edit a BNumber RouteTable";
                updateOperationOutput.ShowExactMessage = true;

            }
            return updateOperationOutput;
        }
        public DeleteOperationOutput<object> DeleteRouteTable(int routeTableId, RouteTableViewType routeTableViewType)
        {
            RouteTableRouteManager routeTableRouteManager = new RouteTableRouteManager();
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;
            IRouteTableDataManager routeTableDataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableDataManager>();
            Helper.SetSwitchConfig(routeTableDataManager);

            RuntimeEditorEntity runTimeEditorEntity = GetRouteTableById(routeTableId, routeTableViewType);
            endPointManager.EndPointAclUpdate(runTimeEditorEntity.RouteTableInput.EndPoints.MapRecords(x => x.EndPointId), 0, routeTableViewType); //update the access_list items related to this route table
            routeTableRouteManager.DropRouteTableRoute(routeTableId);          //Delete the routes of the route table
            bool deleted = routeTableDataManager.DeleteRouteTable(routeTableId);  //Delete the route table
            if (deleted)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }
            return deleteOperationOutput;
        }
        public RuntimeEditorEntity GetRouteTableById(int routeTableId, RouteTableViewType RouteTableViewType)
        {
            RuntimeEditorEntity runtimeEditorEntity = new RuntimeEditorEntity
            {
                EndPointCarrierAccount = new List<EndPointCarrierAccount>(),
                RouteTableInput = new RouteTableInput
                {
                    EndPoints = new List<RouteTableEndPoint>(),
                    RouteTable = GetCachedRouteTables().GetRecord(routeTableId),
                    RouteTableViewType = RouteTableViewType
                }
            };

            switch (RouteTableViewType)
            { 
                case RouteTableViewType.ANumber:
                var cachedRouteTablesEndPointsANumber = GetCachedRouteTablesEndPointsANumber();
                var routeTableEntityANumber = cachedRouteTablesEndPointsANumber.GetRecord(routeTableId);
                RuntimeEditorEntity(runtimeEditorEntity, routeTableEntityANumber);
                    break;
                case RouteTableViewType.Whitelist:
                var cachedRouteTablesEndPointsWhitelist = GetCachedRouteTablesEndPointsWhitelist();
                var routeTableEntityWhitelist = cachedRouteTablesEndPointsWhitelist.GetRecord(routeTableId);
                RuntimeEditorEntity(runtimeEditorEntity, routeTableEntityWhitelist);
                    break;
                case RouteTableViewType.BNumber:
                var cachedRouteTablesEndPointsBNumber = GetCachedRouteTablesEndPointsBNumber();
                var routeTableEntityBNumber = cachedRouteTablesEndPointsBNumber.GetRecord(routeTableId);
                RuntimeEditorEntity(runtimeEditorEntity, routeTableEntityBNumber);
                    break;
            
            
            
            }
             return runtimeEditorEntity;
        }
        private void RuntimeEditorEntity(RuntimeEditorEntity runtimeEditorEntity, RouteEndPoints routeTableEntity)
        {
            if (routeTableEntity != null && routeTableEntity.EndPoints != null)
            {
                foreach (var endPoint in routeTableEntity.EndPoints)
                {
                    runtimeEditorEntity.EndPointCarrierAccount.Add(new EndPointCarrierAccount { EndPointId = endPoint.EndPointId, CarrierAccount = endPointManager.GetEndPointCarrierAccountId(endPoint.EndPointId) });
                    runtimeEditorEntity.RouteTableInput.EndPoints.Add(new RouteTableEndPoint { EndPointId = endPoint.EndPointId });
                }
            }
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool IsTimeExpirable { get { return true; } }

        }
        #endregion

        #region Private Methods

        private Dictionary<int, RouteTable> GetCachedRouteTables()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedRouteTables", PrepareCachedRouteTables);
        }
        private Dictionary<int, RouteTable> PrepareCachedRouteTables()
        {
            IRouteTableDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableDataManager>();
            Helper.SetSwitchConfig(dataManager);

            Dictionary<int, RouteTable> routeTables = new Dictionary<int, RouteTable>();

            if (dataManager.IvSwitchSync == null) return routeTables;

            routeTables = dataManager.GetRouteTables().ToDictionary(x => x.RouteTableId, x => x);
            return routeTables;

        }
        private Dictionary<int, RouteEndPoints> GetCachedRouteTablesEndPointsANumber()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedRouteTablesEndPointsANumber", () =>
               {
                   IRouteTableDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableDataManager>();
                   Helper.SetSwitchConfig(dataManager);
                   Dictionary<int, RouteEndPoints> routeTablesEndPoints = new Dictionary<int, RouteEndPoints>();
                   Dictionary<int, RouteTable> allRouteTables = this.GetCachedRouteTables();
                   Dictionary<int, EndPoint> allEndPoints = endPointManager.GetAllEndPoints();
                   if(allRouteTables!=null)
                   foreach (var routeTable in allRouteTables)
                   {
                       if (allEndPoints!=null)
                       foreach (var endPoint in allEndPoints)
                       {
                           if (endPoint.Value.CliRouting == routeTable.Value.RouteTableId)
                           {
                               var routeEndPoints = routeTablesEndPoints.GetOrCreateItem(routeTable.Key, () =>
                               {
                                   return new RouteEndPoints
                                   {
                                       EndPoints = new List<EndPoint>(),
                                       RouteTable = routeTable.Value
                                   };
                               });
                               routeEndPoints.EndPoints.Add(endPoint.Value);
                           }
                       }
                   }
                   return routeTablesEndPoints;

               });
        }
        private Dictionary<int, RouteEndPoints> GetCachedRouteTablesEndPointsWhitelist()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedRouteTablesEndPointsWhitelist", () =>
               {
                   IRouteTableDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableDataManager>();
                   Helper.SetSwitchConfig(dataManager);
                   Dictionary<int, RouteEndPoints> routeTablesEndPoints = new Dictionary<int, RouteEndPoints>();
                   Dictionary<int, RouteTable> allRouteTables = this.GetCachedRouteTables();
                   Dictionary<int, EndPoint> allEndPoints = endPointManager.GetAllEndPoints();
                   if (allRouteTables != null)
                   foreach (var routeTable in allRouteTables)
                   {
                       if (allEndPoints!=null) 
                       foreach (var endPoint in allEndPoints)
                       {
                           if (endPoint.Value.DstRouting == routeTable.Value.RouteTableId)
                           {
                               var routeEndPoints = routeTablesEndPoints.GetOrCreateItem(routeTable.Key, () =>
                               {
                                   return new RouteEndPoints
                                   {
                                       EndPoints = new List<EndPoint>(),
                                       RouteTable = routeTable.Value
                                   };
                               });
                               routeEndPoints.EndPoints.Add(endPoint.Value);
                           }
                       }

                   }
                   return routeTablesEndPoints;
               });
        }

        private Dictionary<int, RouteEndPoints> GetCachedRouteTablesEndPointsBNumber()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedRouteTablesEndPointsBNumber", () =>
               {
                   IRouteTableDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableDataManager>();
                   Helper.SetSwitchConfig(dataManager);
                   Dictionary<int, RouteEndPoints> routeTablesEndPoints = new Dictionary<int, RouteEndPoints>();
                   Dictionary<int, RouteTable> allRouteTables = this.GetCachedRouteTables();
                   Dictionary<int, EndPoint> allEndPoints = endPointManager.GetAllEndPoints();
                   if (allRouteTables != null)
                       foreach (var routeTable in allRouteTables)
                       {
                           if (allEndPoints != null)
                               foreach (var endPoint in allEndPoints)
                               {
                                   if (endPoint.Value.RouteTableId.HasValue && endPoint.Value.RouteTableId.Value == routeTable.Value.RouteTableId)
                                   {
                                       var routeEndPoints = routeTablesEndPoints.GetOrCreateItem(routeTable.Key, () =>
                                       {
                                           return new RouteEndPoints
                                           {
                                               EndPoints = new List<EndPoint>(),
                                               RouteTable = routeTable.Value
                                           };
                                       });
                                       routeEndPoints.EndPoints.Add(endPoint.Value);
                                   }
                               }

                       }
                   return routeTablesEndPoints;
               });
        }


        #endregion

        #region Mappers
        public RouteTableDetails RouteTableDetailMapper(RouteTable routeTable)
        {
            return new RouteTableDetails
            {
                RouteTableId = routeTable.RouteTableId,
                Name = routeTable.Name,
                Description = routeTable.Description,
                PScore = routeTable.PScore
            };
        }

        #endregion
    }
}
