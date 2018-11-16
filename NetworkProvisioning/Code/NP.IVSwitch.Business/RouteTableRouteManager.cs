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
using NP.IVSwitch.Entities;

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

                    foreach (var code in codes)
                    {
                        if (code.Length > 20)
                        {
                            insertOperationOutput.Message = "The length of code must be less than 20";
                            return insertOperationOutput;

                        }

                        Int16 codePreference = preference;
                        Int16 codeMainPreference = preference;
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
                                        if (evaluatedPercentage < 1)
                                            evaluatedPercentage = 1;
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
                                            Huntstop = (optionAdd.BackupOptions == null) ? (Int16)1 : (Int16)0,
                                            StateId = 1,
                                            Description = this.getDescription(optionAdd.RouteId)

										});
                                        if (optionAdd.BackupOptions != null)
                                        {
											foreach (var bacupOption in optionAdd.BackupOptions)
											{
												routeTableRoute.RouteOptions.Add(new RouteTableRouteOption
												{
													RouteId = bacupOption.BackupOptionRouteId,
													RoutingMode = 8,
													Preference = codePreference--,
													TotalBKTs = codeMainPreference,
													BKTSerial = codeMainPreference - codePreference,
													BKTCapacity = evaluatedPercentage,
													BKTTokens = evaluatedPercentage,
													Percentage = 0,
													Huntstop = 0,
													StateId = 1,
													Description = this.getDescription(bacupOption.BackupOptionRouteId)

												});
											}
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
                                           StateId = 1,
                                           Huntstop = 1,
                                           Description = this.getDescription(optionAdd.RouteId)

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
                            if (evaluatedPercentage < 1)
                                evaluatedPercentage = 1;
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
                                Huntstop = (optionEdit.BackupOptions == null) ? (Int16)1 : (Int16)0,
                                StateId = 1,
                                Description = this.getDescription(optionEdit.RouteId)



							});
                            if (optionEdit.BackupOptions != null)
                            {
								foreach (var bacupOption in optionEdit.BackupOptions)
								{
									routeTableRoute.RouteOptions.Add(new RouteTableRouteOption
									{
										RouteId = bacupOption.BackupOptionRouteId,
										RoutingMode = 8,
										Preference = preference--,
										TotalBKTs = mainPreference,
										BKTSerial = mainPreference - preference,
										BKTCapacity = evaluatedPercentage,
										BKTTokens = evaluatedPercentage,
										Percentage = 0,
										Huntstop = 0,
										StateId = 1,
										Description = this.getDescription(bacupOption.BackupOptionRouteId)

									});
								}
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
                                StateId = 1,
                                Huntstop = 1,
                                Description = this.getDescription(optionEdit.RouteId)

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
            RouteTableRoutesRuntimeEditor routeTableRoutesRuntimeEditor = new RouteTableRoutesRuntimeEditor
            {
                RouteOptionsToEdit = new List<RouteTableRouteOptionRuntimeEditor>(),
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
                            BackupOptions = (item.BackupOptions != null) ? item.BackupOptions.MapRecords(x => new BackupOption { BackupOptionRouteId = x.BackupOptionRouteId, BackupOptionSupplierId = routeManager.GetRouteCarrierAccountId(x.BackupOptionRouteId) }).ToList() : null,
                            Percentage = item.Percentage,
                            Preference = item.Preference,
                            TechPrefix = item.TechPrefix,
                        });
                };
            if (routeTableRoutesRuntimeEditor.RouteOptionsToEdit != null)
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
            else
            {
                deleteOperationOutput.Message = "Destination code doesn't exist";
                deleteOperationOutput.ShowExactMessage = true;
            }
            return deleteOperationOutput;

        }
        public bool CheckIfCodesExist(RouteTableRoutesToAdd routeTableRouteItems)
        {
            IRouteTableRouteDataManager routeTableRouteDataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableRouteDataManager>();
            Helper.SetSwitchConfig(routeTableRouteDataManager);
            List<string> codes = routeTableRouteItems.CodeListResolver.Settings.GetCodeList(new CodeListResolverContext());
             if (codes != null)
            return routeTableRouteDataManager.CheckIfCodesExist(codes, routeTableRouteItems.RouteTableId);
            return false;
        }
        #endregion



        #region Private Methods


        private string getDescription(int routeId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            RouteManager routeManager = new RouteManager();
            string description = "";
            string suffixName = "";
            int? carrierAccountId = routeManager.GetRouteCarrierAccountId(routeId);
            if (carrierAccountId.HasValue)
            {
                CarrierAccount carrierAccount = carrierAccountManager.GetCarrierAccount(carrierAccountId.Value);
                suffixName = carrierAccount.NameSuffix;
                description = string.Format("{0} {1}", carrierProfileManager.GetCarrierProfileName(carrierAccount.CarrierProfileId), suffixName != null ? string.Format("({0})", suffixName) : "");
            }
            return description;

        }
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
                    TechPrefix = routeTableRoute.TechPrefix,
                    RouteOptionsDetails = new List<RouteTableRouteOptionDetails>()
                };
                routeTableRoute.RouteOptions = routeTableRoute.RouteOptions.OrderByDescending(x => x.Preference).ToList();
                bool firstBackup = true;
                int blockedAccount;
                int.TryParse(Helper.GetIvSwitchSync().BlockedAccountMapping, out blockedAccount);
                RouteTableRouteOptionDetails routeTableRouteOptionDetails = new RouteTableRouteOptionDetails();

                if (routeTableRoute.RouteOptions != null)
                    foreach (var routeOption in routeTableRoute.RouteOptions)
                    {
                        if (routeOption.RouteId == blockedAccount)
                            break;

                        if (routeOption.Huntstop == 0 && firstBackup)
                        {
                            routeTableRouteOptionDetails = new RouteTableRouteOptionDetails();
                            routeTableRouteOptionDetails.SupplierName = _manager.GetRouteCarrierAccountName(routeOption.RouteId);
                            routeTableRouteOptionDetails.RouteName = _manager.GetRouteDescription(routeOption.RouteId);
                            routeTableRouteOptionDetails.Percentage = routeOption.Percentage;
                            routeTableRouteOptionDetails.Preference = routeOption.Preference;
                            routeTableRouteOptionDetails.BackupsOptionsDetails = new List<BackupRouteOptionDetail>();
                            firstBackup = false;
                        }
                        else
                            if (routeOption.Huntstop == 0)
                            {
                                routeTableRouteOptionDetails.BackupsOptionsDetails.Add(new BackupRouteOptionDetail
                                {

                                    SupplierName = _manager.GetRouteCarrierAccountName(routeOption.RouteId),
                                    RouteName = _manager.GetRouteDescription(routeOption.RouteId),
                                    Percentage = routeOption.Percentage,
                                    Preference = routeOption.Preference,

                                });

                            }

                        if (routeOption.Huntstop == 1 && firstBackup)
                        {
                            routeTableRouteOptionDetails = new RouteTableRouteOptionDetails {

                                SupplierName = _manager.GetRouteCarrierAccountName(routeOption.RouteId),
                                RouteName = _manager.GetRouteDescription(routeOption.RouteId),
                                Percentage = routeOption.Percentage,
                                Preference = routeOption.Preference,
                            };

                            routeTableRouteDetail.RouteOptionsDetails.Add(routeTableRouteOptionDetails);

                        }
                        else
                            if (routeOption.Huntstop == 1 && !firstBackup)
                            {
                                routeTableRouteOptionDetails.BackupsOptionsDetails.Add(new BackupRouteOptionDetail
                                {


                                    SupplierName = _manager.GetRouteCarrierAccountName(routeOption.RouteId),
                                    RouteName = _manager.GetRouteDescription(routeOption.RouteId),
                                    Percentage = routeOption.Percentage,
                                    Preference = routeOption.Preference,

                                });
                                routeTableRouteDetail.RouteOptionsDetails.Add(routeTableRouteOptionDetails);
                                firstBackup = true;
                            }






                    }
                if (routeTableRouteDetail.RouteOptionsDetails != null)
                    routeTableRouteDetail.RouteOptionsDetails = routeTableRouteDetail.RouteOptionsDetails.OrderByDescending(item => item.Preference).ToList();

                return routeTableRouteDetail;
            }
            public override IEnumerable<RouteTableRoute> RetrieveAllData(DataRetrievalInput<RouteTableRouteQuery> input)
            {
                IRouteTableRouteDataManager routeTableDataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableRouteDataManager>();
                Helper.SetSwitchConfig(routeTableDataManager);
                RouteManager routeManager = new RouteManager();
                List<int> routeIds = new List<int>();
                if (input.Query.SupplierIds != null && input.Query.SupplierIds.Count > 0 && (input.Query.RouteIds == null || input.Query.RouteIds.Count == 0))
                {
                    input.Query.RouteIds = new List<int>();
                    routeIds = routeManager.GetCarrierAccountsRouteIds(input.Query.SupplierIds);
                    if (routeIds.Count == 0)
                        return null;
                    input.Query.RouteIds = routeIds;
                }
                return routeTableDataManager.GetRouteTablesRoutes(input.Query.RouteTableViewType, input.Query.RouteTableId, input.Query.Limit, input.Query.ANumber, input.Query.BNumber, input.Query.Whitelist, input.Query.RouteIds);
            }

            protected override ResultProcessingHandler<RouteTableRouteDetail> GetResultProcessingHandler(DataRetrievalInput<RouteTableRouteQuery> input, BigResult<RouteTableRouteDetail> bigResult)
            {
                return new ResultProcessingHandler<RouteTableRouteDetail>
                {
                    ExportExcelHandler = new RouteTableRouteDetailExportExcelHandler()
                };
            }
            private class RouteTableRouteDetailExportExcelHandler : ExcelExportHandler<RouteTableRouteDetail>
            {
                public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<RouteTableRouteDetail> context)
                {
                    var dataRetrieval = context.Input.CastWithValidate<DataRetrievalInput<RouteTableRouteQuery>>("context.Input");
                    var query = dataRetrieval.Query;

                    var sheet = new ExportExcelSheet()
                    {
                        SheetName = "Route Table Routes",
                        Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                    };
                    switch (query.RouteTableViewType)
                    {
                        case RouteTableViewType.ANumber:
                            sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ANumber" });
                            sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "BNumber" });
                            sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Options" });
                            break;
                        case RouteTableViewType.BNumber:
                            sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "BNumber" });
                            sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Options" });
                            break;
                        case RouteTableViewType.Whitelist:
                            sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Whitelist" });
                            sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Options" });
                            break;



                    }



                    sheet.Rows = new List<ExportExcelRow>();
                    if (context.BigResult != null && context.BigResult.Data != null)
                    {
                        foreach (var record in context.BigResult.Data)
                        {
                            if (record != null)
                            {
                                var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                                StringBuilder options = new StringBuilder();
                                foreach (var option in record.RouteOptionsDetails)
                                {
                                    options.Append(option.SupplierName);
                                    options.Append(' ', 5);
                                    options.Append(option.RouteName);
                                    if(option.BackupsOptionsDetails!=null && option.BackupsOptionsDetails.Count>0)
                                    {
                                        options.Append("<");
                                        foreach(var backup in option.BackupsOptionsDetails)
                                        {
                                            options.Append(backup.SupplierName);
                                            options.Append(' ',5);
                                            options.Append(backup.RouteName);
                                            options.Append(",");

                                        }
                                        options.Append(">");
                                    }
                                    options.Append(' ', 10);
                                        

                                }
                                switch (query.RouteTableViewType)
                                {
                                    case RouteTableViewType.ANumber:
                                        row.Cells.Add(new ExportExcelCell() { Value = record.Destination });
                                        row.Cells.Add(new ExportExcelCell() { Value = record.TechPrefix });
                                        row.Cells.Add(new ExportExcelCell() { Value = options.ToString() });
                                        break;
                                    case RouteTableViewType.BNumber:
                                        row.Cells.Add(new ExportExcelCell() { Value = record.Destination });
                                        row.Cells.Add(new ExportExcelCell() { Value = options.ToString() });
                                        break;
                                    case RouteTableViewType.Whitelist:
                                        row.Cells.Add(new ExportExcelCell() { Value = record.Destination });
                                        row.Cells.Add(new ExportExcelCell() { Value = options.ToString() });
                                        break;



                                }
                                sheet.Rows.Add(row);
                            }
                        }
                    }

                    context.MainSheet = sheet;
                }
            }

        }

        #endregion
        #region Mappers
        public RouteTableRouteDetails RouteTableRouteDetailToEditMapper(RouteTableRoutesToEdit routeTableItem)
        {
            List<RouteTableRouteOptionDetails> options = new List<RouteTableRouteOptionDetails>();
            RouteManager _manager = new RouteManager();
            if (routeTableItem != null && routeTableItem.RouteOptionsToEdit != null)
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
                if (routeTableItem.RouteOptionsToAdd != null)
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
