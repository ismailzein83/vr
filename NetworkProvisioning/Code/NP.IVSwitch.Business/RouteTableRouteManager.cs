using NP.IVSwitch.Data;
using NP.IVSwitch.Entities.RouteTableRoute;
using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Rules;
using Vanrise.Caching;
using Vanrise.Common.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace NP.IVSwitch.Business
{
    public class RouteTableRouteManager
    {
        #region Public Methods
        public IDataRetrievalResult<RouteTableRouteDetail> GetFilteredRouteTableRoutes(DataRetrievalInput<RouteTableRouteQuery> input)
        {

            return BigDataManager.Instance.RetrieveData(input, new RouteTableRouteRequestHandler());

        }

        public InsertOperationOutput<RouteTableRouteDetails> AddRouteTableRoutes(RouteTableRoutesToAdd routeTableRouteItem)
        {
            IRouteTableRouteDataManager routeTableRTDataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableRouteDataManager>();
            Helper.SetSwitchConfig(routeTableRTDataManager);

            InsertOperationOutput<RouteTableRouteDetails> insertOperationOutput = new InsertOperationOutput<RouteTableRouteDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            string errorMessage;
            decimal percentage;
            Int16 preference = 0;
            if (routeTableRouteItem != null && routeTableRouteItem.RouteOptionsToAdd != null)
            {
                foreach (var item in routeTableRouteItem.RouteOptionsToAdd)
                    if (item != null && item.BackupOptions != null)
                        preference += (Int16)item.BackupOptions.Count;

                preference += (Int16)routeTableRouteItem.RouteOptionsToAdd.Count;
            }
            int blockedAccount;
            if (ValidateRouteOptionsForAdd(routeTableRouteItem.IsBlockedAccount, routeTableRouteItem.RouteOptionsToAdd, out percentage, out blockedAccount, out errorMessage))
            {
                List<RouteTableRoute> routeTableRoutes = new List<RouteTableRoute>();
                routeTableRouteItem.CodeListResolver.ThrowIfNull("routeTableRTItem.CodeListResolver");
                routeTableRouteItem.CodeListResolver.Settings.ThrowIfNull("routeTableRTItem.CodeListResolver.Settings");
                List<string> codes = routeTableRouteItem.CodeListResolver.Settings.GetCodeList(new CodeListResolverContext());
                if (codes != null)
                {
                    Int16 codePreference = preference;
                    Int16 codeMainPreference = preference;
                    foreach (var code in codes)
                    {
                        var routeTableRoute = new RouteTableRoute
                        {
                            RouteOptions = new List<RouteTableRouteOption>(),
                            Destination = code,
                            TechPrefix = routeTableRouteItem.TechPrefix,
                        };




                        if (routeTableRouteItem.IsBlockedAccount)
                        {
                            routeTableRoute.RouteOptions.Add(new RouteTableRouteOption { RouteId = blockedAccount });
                            routeTableRoutes.Add(routeTableRoute);
                        }
                        else
                        {
                            if (routeTableRouteItem != null && routeTableRouteItem.RouteOptionsToAdd != null)
                                foreach (var optionAdd in routeTableRouteItem.RouteOptionsToAdd)
                                {
                                    if (percentage != 0)
                                    {
                                        int bktSerial = codeMainPreference - codePreference;
                                        var evaluatedPercentage = Convert.ToInt32(Math.Round(optionAdd.Percentage.Value / 10));
                                        routeTableRoute.RouteOptions.Add(new RouteTableRouteOption
                                        {
                                            RouteId = optionAdd.RouteId,
                                            RoutingMode = 8,
                                            Preference = codePreference--,
                                            TotalBKTs = codeMainPreference,
                                            BKTSerial = codeMainPreference - codePreference,
                                            BKTCapacity = evaluatedPercentage,
                                            BKTTokens = evaluatedPercentage,
                                            Percentage = optionAdd.Percentage.Value,
                                            Huntstop = (optionAdd.BackupOptions == null) ? default(Int16?):0

                                        });
                                        if (optionAdd.BackupOptions != null)
                                        {
                                            foreach (var bacupOption in optionAdd.BackupOptions)
                                                routeTableRoute.RouteOptions.Add(new RouteTableRouteOption
                                                {
                                                    RouteId = bacupOption.BackupOptionRouteId,
                                                    RoutingMode = 8,
                                                    Preference = codePreference--,
                                                    TotalBKTs = codeMainPreference,
                                                    BKTSerial = codeMainPreference - codePreference,
                                                    BKTCapacity = evaluatedPercentage,
                                                    BKTTokens = evaluatedPercentage,
                                                    Percentage = optionAdd.Percentage.Value,
                                                    Huntstop = 0
                                                });

                                            routeTableRoute.RouteOptions[routeTableRoute.RouteOptions.Count - 1].Huntstop = 1;
                                        }


                                        
                                    }
                                    else
                                    {
                                        routeTableRoute.RouteOptions.Add(new RouteTableRouteOption
                                       {
                                           RouteId = optionAdd.RouteId,
                                           RoutingMode = 1,
                                           Preference = codePreference--,
                                           TotalBKTs = 1,
                                           BKTSerial = 1,
                                           BKTCapacity = 1,
                                           BKTTokens = 1,
                                       });
                                    }
                                }
                            routeTableRoutes.Add(routeTableRoute);
                        }
                    }

                    bool insertActionSuccess = routeTableRTDataManager.Insert(routeTableRoutes, routeTableRouteItem.RouteTableId, routeTableRouteItem.IsBlockedAccount);
                    if (insertActionSuccess)
                    {
                        insertOperationOutput.Result = InsertOperationResult.Succeeded;
                        insertOperationOutput.InsertedObject = RouteTableRouteDetailsToAddMapper(routeTableRouteItem);
                    }
                    else
                    {
                        insertOperationOutput.Result = InsertOperationResult.SameExists;
                    }
                }
                return insertOperationOutput;

            }
            else
            {
                insertOperationOutput.Message = errorMessage;
                insertOperationOutput.ShowExactMessage = true;
                return insertOperationOutput;
            }
        }
        public UpdateOperationOutput<RouteTableRouteDetails> UpdateRouteTableRoute(RouteTableRoutesToEdit routeTableItem)
        {

            IRouteTableRouteDataManager routeTableRouteDataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableRouteDataManager>();
            Helper.SetSwitchConfig(routeTableRouteDataManager);
            UpdateOperationOutput<RouteTableRouteDetails> updateOperationOutput = new UpdateOperationOutput<RouteTableRouteDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            string errorMessage;
            decimal percentage;
            Int16 preference = 0;
            Int16 mainPreference;
            if (routeTableItem != null && routeTableItem.RouteOptionsToEdit != null)
            {
                foreach (var item in routeTableItem.RouteOptionsToEdit)
                    if (item != null && item.BackupOptions != null)
                        preference += (Int16)item.BackupOptions.Count;

                preference += (Int16)routeTableItem.RouteOptionsToEdit.Count;
            }
            mainPreference = preference;

            int blockedAccount;
            if (ValidateRouteOptionsForUpdate(routeTableItem.IsBlockedAccount, routeTableItem.RouteOptionsToEdit, out percentage, out blockedAccount, out errorMessage))
            {
                RouteTableRoute routeTableRoute = new RouteTableRoute();
                routeTableRoute.RouteOptions = new List<RouteTableRouteOption>();
                if (routeTableItem.IsBlockedAccount)
                {
                    routeTableRoute.RouteOptions.Add(new RouteTableRouteOption { RouteId = blockedAccount });
                }
                else
                    foreach (var optionEdit in routeTableItem.RouteOptionsToEdit)
                    {
                        if (percentage != 0)
                        {
                            int bktSerial = routeTableItem.RouteOptionsToEdit.Count - optionEdit.Preference + 1;
                            var evaluatedPercentage = Convert.ToInt32(Math.Round(optionEdit.Percentage.Value / 10));
                            routeTableRoute.RouteOptions.Add(new RouteTableRouteOption
                            {
                                RouteId = optionEdit.RouteId,
                                RoutingMode = 8,
                                Preference = preference--,
                                TotalBKTs = mainPreference,
                                BKTSerial = mainPreference - preference,
                                BKTCapacity = evaluatedPercentage,
                                BKTTokens = evaluatedPercentage,
                                Percentage = optionEdit.Percentage.Value,
                                Huntstop = (optionEdit.BackupOptions == null) ? default(Int16?) : 0

                            });
                            if (optionEdit.BackupOptions != null)
                            {
                                foreach (var bacupOption in optionEdit.BackupOptions)
                                    routeTableRoute.RouteOptions.Add(new RouteTableRouteOption
                                    {
                                        RouteId = bacupOption.BackupOptionRouteId,
                                        RoutingMode = 8,
                                        Preference = preference--,
                                        TotalBKTs = mainPreference,
                                        BKTSerial = mainPreference - preference,
                                        BKTCapacity = evaluatedPercentage,
                                        BKTTokens = evaluatedPercentage,
                                        Percentage = optionEdit.Percentage.Value,
                                        Huntstop = 0
                                    });

                                routeTableRoute.RouteOptions[routeTableRoute.RouteOptions.Count - 1].Huntstop = 1;
                            }
                        }
                        else
                        {


                            routeTableRoute.RouteOptions.Add(new RouteTableRouteOption
                            {
                                RouteId = optionEdit.RouteId,
                                RoutingMode = 1,
                                Preference = preference--,
                                TotalBKTs = 1,
                                BKTSerial = 1,
                                BKTCapacity = 1,
                                BKTTokens = 1,
                            });

                        }
                    }
                routeTableRoute.Destination = routeTableItem.Destination;
                routeTableRoute.TechPrefix = routeTableItem.TechPrefix;
                updateOperationOutput.Result = UpdateOperationResult.Failed;
                updateOperationOutput.UpdatedObject = null;
                bool updateActionSuccess = routeTableRouteDataManager.Update(routeTableRoute, routeTableItem.RouteTableId, routeTableItem.IsBlockedAccount);
                if (updateActionSuccess)
                {
                    updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                    updateOperationOutput.UpdatedObject = RouteTableRouteDetailToEditMapper(routeTableItem);
                }
                else
                {
                    updateOperationOutput.Result = UpdateOperationResult.SameExists;
                }
                return updateOperationOutput;

            }
            else
            {
                updateOperationOutput.Message = errorMessage;
                updateOperationOutput.ShowExactMessage = true;
                return updateOperationOutput;
            }

        }
        public RouteTableRoutesRuntimeEditor GetRouteTableRoutesOptions(int routeTableId, string destination)
        {
            IRouteTableRouteDataManager routeTableRouteDataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableRouteDataManager>();
            Helper.SetSwitchConfig(routeTableRouteDataManager);
            RouteManager routeManager = new RouteManager();
            RouteTableRoutesRuntimeEditor routeTableRoutesRuntimeEditor = new RouteTableRoutesRuntimeEditor { 
            RouteOptionsToEdit=new List<RouteTableRouteOptionRuntimeEditor>(),            
            };

            RouteTableRoutesToEdit RouteTableRoutesToEdit = new RouteTableRoutesToEdit();
            RouteTableRoutesToEdit = routeTableRouteDataManager.GetRouteTableRoutesOptions(routeTableId, destination);
            routeTableRoutesRuntimeEditor.Destination = destination;
            routeTableRoutesRuntimeEditor.TechPrefix = RouteTableRoutesToEdit.TechPrefix;

            if (routeTableRoutesRuntimeEditor != null && routeTableRoutesRuntimeEditor.RouteOptionsToEdit != null)
                foreach (var item in RouteTableRoutesToEdit.RouteOptionsToEdit)
                {
                    int blockedAccount;
                    int.TryParse(Helper.GetIvSwitchSync().BlockedAccountMapping, out blockedAccount);

                    if (item.RouteId == blockedAccount)
                    {
                        routeTableRoutesRuntimeEditor.IsBlockedAccount = true;
                    }
                    else

                        routeTableRoutesRuntimeEditor.RouteOptionsToEdit.Add(new RouteTableRouteOptionRuntimeEditor
                        {
                            RouteId = item.RouteId,
                            SupplierId = routeManager.GetRouteCarrierAccountId(item.RouteId),
                            BackupOptions = (item.BackupOptions!=null)?item.BackupOptions.MapRecords(x => new BackupOption { BackupOptionRouteId = x.BackupOptionRouteId, BackupOptionSupplierId = routeManager.GetRouteCarrierAccountId(x.BackupOptionRouteId) }).ToList():null,
                            Percentage = item.Percentage,
                            Preference = item.Preference,
                            TechPrefix = item.TechPrefix,
                        });
                };
            if (routeTableRoutesRuntimeEditor.RouteOptionsToEdit!=null)
            routeTableRoutesRuntimeEditor.RouteOptionsToEdit = routeTableRoutesRuntimeEditor.RouteOptionsToEdit.OrderByDescending(item => item.Preference).ToList();

            return routeTableRoutesRuntimeEditor;
        }
        public void CreateRouteTableRoute(int routeTableId)
        {
            IRouteTableRouteDataManager routeTableRouteDataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableRouteDataManager>();
            Helper.SetSwitchConfig(routeTableRouteDataManager);
            routeTableRouteDataManager.CreateRouteTableRoute(routeTableId);
        }

        public bool DropRouteTableRoute(int routeTableId)
        {
            IRouteTableRouteDataManager routeTableRouteDataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableRouteDataManager>();
            Helper.SetSwitchConfig(routeTableRouteDataManager);
            return routeTableRouteDataManager.DropRouteTableRoute(routeTableId);

        }
        public DeleteOperationOutput<object> DeleteRouteTableRoutes(int routeTableId, string destination)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;
            IRouteTableRouteDataManager routeTableRouteDataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableRouteDataManager>();
            Helper.SetSwitchConfig(routeTableRouteDataManager);
            deleteOperationOutput.Result = DeleteOperationResult.Failed;
            bool deleted = routeTableRouteDataManager.DeleteRouteTableRoutes(routeTableId, destination);  //Delete the route table
            if (deleted)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;

            }
            return deleteOperationOutput;

        }
        #endregion



        #region Private Methods



        private bool ValidateRouteOptionsForAdd(bool isBlockedAccount, List<RouteTableRouteOptionsToAdd> routeOptions, out decimal percentage, out int blockedAccount, out string errorMessage)
        {
            errorMessage = null;
            blockedAccount = -1;
            percentage = 0;
            if (isBlockedAccount)
            {
                var ivSwitchSync = Helper.GetIvSwitchSync();
                ivSwitchSync.ThrowIfNull("ivSwitchSync");
                ivSwitchSync.BlockedAccountMapping.ThrowIfNull("ivSwitchSync.BlockedAccountMapping");
                if (!int.TryParse(ivSwitchSync.BlockedAccountMapping, out blockedAccount))
                {
                    errorMessage = "Error parsing blocked account";
                    return false;
                }
            }
            else
            {
                if (routeOptions.Count == 0)
                {
                    errorMessage = "No route options selected.";
                }
                foreach (var routeOption in routeOptions)
                {
                    if (routeOption.Percentage.HasValue)
                        percentage += routeOption.Percentage.Value;
                    else
                    {
                        if (percentage != 0)
                        {
                            errorMessage = "Not valid percentage with null value.";
                            return false;
                        }
                    }
                }
                if (percentage != 0 && percentage != 100)
                {
                    errorMessage = "Sum of percentages must be equals to 100";
                    return false;
                }
            }
            return true;
        }
        private bool ValidateRouteOptionsForUpdate(bool isBlockedAccount, List<RouteTableRouteOptionToEdit> routeOptions, out decimal percentage, out int blockedAccount, out string errorMessage)
        {
            errorMessage = null;
            blockedAccount = -1;
            percentage = 0;
            if (isBlockedAccount)
            {
                var ivSwitchSync = Helper.GetIvSwitchSync();
                ivSwitchSync.ThrowIfNull("ivSwitchSync");
                ivSwitchSync.BlockedAccountMapping.ThrowIfNull("ivSwitchSync.BlockedAccountMapping");
                if (!int.TryParse(ivSwitchSync.BlockedAccountMapping, out blockedAccount))
                {
                    errorMessage = "Error parsing blocked account";
                    return false;
                }
            }
            else
            {
                if (routeOptions.Count == 0)
                {
                    errorMessage = "No route options selected.";
                }
                foreach (var routeOption in routeOptions)
                {
                    if (routeOption.Percentage.HasValue)
                        percentage += routeOption.Percentage.Value;
                    else
                    {
                        if (percentage != 0)
                        {
                            errorMessage = "Not valid percentage with null value.";
                            return false;
                        }
                    }
                }
                if (percentage != 0 && percentage != 100)
                {
                    errorMessage = "Sum of percentages must be equals to 100";
                    return false;
                }
            }
            return true;
        }

        #endregion
        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool IsTimeExpirable { get { return true; } }
            protected override bool UseCentralizedCacheRefresher { get { return true; } }
        }

        private class RouteTableRouteRequestHandler : BigDataRequestHandler<RouteTableRouteQuery, RouteTableRoute, RouteTableRouteDetail>   
        {
            public override RouteTableRouteDetail EntityDetailMapper(RouteTableRoute routeTableRoute)
            {
                RouteManager _manager = new RouteManager();
                RouteTableRouteDetail routeTableRouteDetail = new RouteTableRouteDetail
                {
                    Destination = routeTableRoute.Destination,
                    TechPrefix = routeTableRoute.TechPrefix
                };
                int blockedAccount;
                int.TryParse(Helper.GetIvSwitchSync().BlockedAccountMapping, out blockedAccount);
                if (routeTableRoute.RouteOptions!=null)
                foreach (var routeOption in routeTableRoute.RouteOptions)
                {
                    if (routeOption.RouteId == blockedAccount)
                        break;

                    if (routeTableRouteDetail.RouteOptionsDetails == null)
                        routeTableRouteDetail.RouteOptionsDetails = new List<RouteTableRouteOptionDetails>();
                    routeTableRouteDetail.RouteOptionsDetails.Add(new RouteTableRouteOptionDetails
                    {
                        SupplierName = _manager.GetRouteCarrierAccountName(routeOption.RouteId),
                        RouteName = _manager.GetRouteDescription(routeOption.RouteId),
                        Percentage = routeOption.Percentage,
                        Preference = routeOption.Preference
                    });

                }
                if (routeTableRouteDetail.RouteOptionsDetails != null)
                    routeTableRouteDetail.RouteOptionsDetails = routeTableRouteDetail.RouteOptionsDetails.OrderByDescending(item => item.Preference).ToList();

                return routeTableRouteDetail;
            }
            public override IEnumerable<RouteTableRoute> RetrieveAllData(DataRetrievalInput<RouteTableRouteQuery> input)
            {
                IRouteTableRouteDataManager routeTableDataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableRouteDataManager>();
                Helper.SetSwitchConfig(routeTableDataManager);
                return  routeTableDataManager.GetRouteTablesRoutes(input.Query.RouteTableId, input.Query.Limit, input.Query.ANumber, input.Query.BNumber);
            }
        }

        #endregion
        #region Mappers
        public RouteTableRouteDetails RouteTableRouteDetailToEditMapper(RouteTableRoutesToEdit routeTableItem)
        {
            List<RouteTableRouteOptionDetails> options = new List<RouteTableRouteOptionDetails>();
            RouteManager _manager = new RouteManager();
            if (routeTableItem!=null && routeTableItem.RouteOptionsToEdit!=null)
            foreach (var item in routeTableItem.RouteOptionsToEdit)
            {
                RouteTableRouteOptionDetails option = new RouteTableRouteOptionDetails();
                option.Percentage = item.Percentage;
                option.RouteName = _manager.GetRouteDescription(item.RouteId);
                option.SupplierName = _manager.GetRouteCarrierAccountName(item.RouteId);
                options.Add(option);

            }

            List<RouteTableRouteDetail> routeTableRouteDetail = new List<RouteTableRouteDetail>();
            routeTableRouteDetail.Add(new RouteTableRouteDetail
            {

                Destination = routeTableItem.Destination,
                RouteOptionsDetails = options
            });
            return new RouteTableRouteDetails
            {
                Routes = routeTableRouteDetail
            };


        }
        public RouteTableRouteDetails RouteTableRouteDetailsToAddMapper(RouteTableRoutesToAdd routeTableItem)
        {
            RouteTableRouteDetails routeTableRouteDetailsToAdd = new RouteTableRouteDetails();
            routeTableRouteDetailsToAdd.Routes = new List<RouteTableRouteDetail>();
            foreach (var code in routeTableItem.CodeListResolver.Settings.GetCodeList(new CodeListResolverContext()))
            {
                RouteTableRouteDetail routeTableRouteDetail = new RouteTableRouteDetail();
                routeTableRouteDetail.Destination = code;
                List<RouteTableRouteOptionDetails> options = new List<RouteTableRouteOptionDetails>();
                RouteManager _manager = new RouteManager();
                if (routeTableItem.RouteOptionsToAdd!=null)
                foreach (var routePreference in routeTableItem.RouteOptionsToAdd)
                {
                    RouteTableRouteOptionDetails option = new RouteTableRouteOptionDetails();
                    option.Percentage = routePreference.Percentage;
                    option.RouteName = _manager.GetRouteDescription(routePreference.RouteId);
                    option.SupplierName = _manager.GetRouteCarrierAccountName(routePreference.RouteId);
                    options.Add(option);
                }
                routeTableRouteDetail.RouteOptionsDetails = options;
                routeTableRouteDetailsToAdd.Routes.Add(routeTableRouteDetail);
            }

            return routeTableRouteDetailsToAdd;
        }

        #endregion
    }
}
