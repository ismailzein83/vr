using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.FreeRadius
{
    public class FreeRadiusSWSync : SwitchRouteSynchronizer
    {
        public const int CustomerMappingLength = 3;
        public const int SupplierMappingLength = 4;

        public override Guid ConfigId { get { return new Guid("99B59E02-1305-49E5-9342-1B4E08C91439"); } }

        public override bool SupportSyncWithinRouteBuild { get { return false; } }

        public IFreeRadiusDataManager DataManager { get; set; }

        public string MappingSeparator { get; set; }

        public string SupplierOptionsSeparator { get; set; }

        public int NumberOfOptions { get; set; }

        /// <summary>
        /// Key = Carrier Account Id
        /// </summary>
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

        #region Public Methods

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            this.DataManager.PrepareTables(context);
        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            List<FreeRadiusConvertedRoute> convertedRoutes = this.GetFreeRadiusConvertedRoute(context.Routes);
            if (convertedRoutes == null)
                return;

            FreeRadiusConvertedRoutesPayload freeRadiusConvertedRoutesPayload;
            if (context.ConvertedRoutesPayload != null)
                freeRadiusConvertedRoutesPayload = context.ConvertedRoutesPayload.CastWithValidate<FreeRadiusConvertedRoutesPayload>("context.ConvertedRoutesPayload");
            else
                freeRadiusConvertedRoutesPayload = new FreeRadiusConvertedRoutesPayload();

            Helper.AddToConvertedRoutesDict(freeRadiusConvertedRoutesPayload.ConvertedRoutesBuffer, convertedRoutes);
            context.ConvertedRoutesPayload = freeRadiusConvertedRoutesPayload;
        }

        public override void onAllRoutesConverted(ISwitchRouteSynchronizerOnAllRoutesConvertedContext context)
        {
            if (context.ConvertedRoutesPayload == null)
                return;

            FreeRadiusConvertedRoutesPayload payload = context.ConvertedRoutesPayload.CastWithValidate<FreeRadiusConvertedRoutesPayload>("context.ConvertedRoutesPayload");
            if (payload.ConvertedRoutesBuffer == null || payload.ConvertedRoutesBuffer.Count == 0)
                return;

            List<FreeRadiusConvertedRoute> freeRadiusConvertedRoutes = Helper.CompressConvertedRoutes(payload.ConvertedRoutesBuffer);
            context.ConvertedRoutes = freeRadiusConvertedRoutes.Select(itm => itm as ConvertedRoute).ToList();
        }

        public override object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
        {
            return this.DataManager.PrepareDataForApply(context.ConvertedRoutes);
        }

        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            this.DataManager.ApplySwitchRouteSyncRoutes(context);
        }

        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            SwapTableContext swapTableContext = new SwapTableContext()
            {
                WriteTrackingMessage = context.WriteTrackingMessage,
                SwitchName = context.SwitchName,
                IndexesCommandTimeoutInSeconds = context.IndexesCommandTimeoutInSeconds,
                SwitchId = context.SwitchId,
                PreviousSwitchSyncOutput = context.PreviousSwitchSyncOutput,
                WriteBusinessHandledException = context.WriteBusinessHandledException
            };
            this.DataManager.SwapTables(swapTableContext);
            context.SwitchSyncOutput = swapTableContext.SwitchSyncOutput;
        }

        public override bool IsSwitchRouteSynchronizerValid(IIsSwitchRouteSynchronizerValidContext context)
        {
            if (this.CarrierMappings == null || this.CarrierMappings.Count == 0)
                return true;

            HashSet<string> customerMappings = new HashSet<string>();
            HashSet<string> supplierMappings = new HashSet<string>();

            HashSet<string> duplicateCustomerMappings = new HashSet<string>();
            HashSet<string> duplicateSupplierMappings = new HashSet<string>();
            HashSet<int> invalidMappingCustomerIds = new HashSet<int>();
            HashSet<int> invalidMappingSupplierIds = new HashSet<int>();

            foreach (var mapping in this.CarrierMappings.Values)
            {
                if (mapping.CustomerMappings != null)
                {
                    foreach (var customerMapping in mapping.CustomerMappings)
                    {
                        if (customerMapping.Mapping.Length != CustomerMappingLength)
                        {
                            invalidMappingCustomerIds.Add(mapping.CarrierId);
                            continue;
                        }

                        if (customerMappings.Contains(customerMapping.Mapping))
                        {
                            duplicateCustomerMappings.Add(customerMapping.Mapping);
                            continue;
                        }

                        customerMappings.Add(customerMapping.Mapping);
                    }
                }

                if (mapping.SupplierMappings != null)
                {
                    foreach (var supplierMapping in mapping.SupplierMappings)
                    {
                        if (supplierMapping.Mapping.Length != SupplierMappingLength)
                        {
                            invalidMappingSupplierIds.Add(mapping.CarrierId);
                            continue;
                        }

                        if (supplierMappings.Contains(supplierMapping.Mapping))
                        {
                            duplicateSupplierMappings.Add(supplierMapping.Mapping);
                            continue;
                        }

                        supplierMappings.Add(supplierMapping.Mapping);
                    }
                }
            }

            if (duplicateCustomerMappings.Count == 0 && duplicateSupplierMappings.Count == 0 && invalidMappingCustomerIds.Count == 0 && invalidMappingSupplierIds.Count == 0)
                return true;

            List<string> validationMessages = new List<string>();

            if (duplicateCustomerMappings.Count > 0)
                validationMessages.Add(string.Format("Duplicate Customer Mappings: {0}", string.Join(", ", duplicateCustomerMappings)));

            if (duplicateSupplierMappings.Count > 0)
                validationMessages.Add(string.Format("Duplicate Supplier Mappings: {0}", string.Join(", ", duplicateSupplierMappings)));

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            if (invalidMappingCustomerIds.Count > 0)
            {
                List<string> invalidMappingCustomers = new List<string>();

                foreach (var customerId in invalidMappingCustomerIds)
                    invalidMappingCustomers.Add(string.Format("'{0}'", carrierAccountManager.GetCarrierAccountName(customerId)));

                validationMessages.Add(string.Format("Invalid Mappings for Customers: {0}", string.Join(", ", invalidMappingCustomers)));
            }

            if (invalidMappingSupplierIds.Count > 0)
            {
                List<string> invalidMappingSuppliers = new List<string>();

                foreach (var supplierId in invalidMappingSupplierIds)
                    invalidMappingSuppliers.Add(string.Format("'{0}'", carrierAccountManager.GetCarrierAccountName(supplierId)));

                validationMessages.Add(string.Format("Invalid Mappings for Suppliers: {0}", string.Join(", ", invalidMappingSuppliers)));
            }

            context.ValidationMessages = validationMessages;
            return false;
        }

        public override bool SupportPartialRouteSync { get { return true; } }

        public override void ApplyDifferentialRoutes(ISwitchRouteSynchronizerApplyDifferentialRoutesContext context)
        {
            if (context.UpdatedRoutes == null || context.UpdatedRoutes.Count == 0)
                return;

            int differentialRoutesPerTransaction = new TOne.WhS.RouteSync.Business.ConfigManager().GetRouteSyncProcessDifferentialRoutesPerTransaction();

            SwitchSyncOutput switchSyncOutput = null;
            List<Route> updatedRoutesBatch = new List<Route>();

            foreach (var updatedRoute in context.UpdatedRoutes)
            {
                updatedRoutesBatch.Add(updatedRoute);

                if (updatedRoutesBatch.Count >= differentialRoutesPerTransaction)
                {
                    switchSyncOutput = ApplyDifferentialRoutesBatch(updatedRoutesBatch, context.SwitchId, context.SwitchName, context.WriteBusinessHandledException, switchSyncOutput);
                    updatedRoutesBatch = new List<Route>();
                }
            }

            if (updatedRoutesBatch.Count > 0)
                switchSyncOutput = ApplyDifferentialRoutesBatch(updatedRoutesBatch, context.SwitchId, context.SwitchName, context.WriteBusinessHandledException, switchSyncOutput);

            context.SwitchSyncOutput = switchSyncOutput;
        }

        #endregion

        #region Private Methods

        private List<FreeRadiusConvertedRoute> GetFreeRadiusConvertedRoute(List<Route> routes)
        {
            if (routes == null || CarrierMappings == null)
                return null;

            CarrierMapping carrierMapping;
            List<FreeRadiusConvertedRoute> convertedRoutes = new List<FreeRadiusConvertedRoute>();

            foreach (var route in routes)
            {
                if (CarrierMappings.TryGetValue(route.CustomerId, out carrierMapping))
                {
                    if (carrierMapping.CustomerMappings == null)
                        continue;

                    string supplierOptionsSeparator = !string.IsNullOrEmpty(this.SupplierOptionsSeparator) ? this.SupplierOptionsSeparator : string.Empty;
                    List<FreeRadiusRouteOption> freeRadiusRouteOptions = BuildOptions(route, supplierOptionsSeparator);
                    RedistributePercentages(freeRadiusRouteOptions);

                    foreach (var customerMapping in carrierMapping.CustomerMappings)
                    {
                        if (string.IsNullOrEmpty(customerMapping.Mapping))
                            continue;

                        decimal currentPercentage = 0;

                        foreach (var convertedRouteOption in freeRadiusRouteOptions)
                        {
                            FreeRadiusConvertedRoute convertedRoute = new FreeRadiusConvertedRoute()
                            {
                                Customer_id = customerMapping.Mapping,
                                Clisis = "''",
                                Cldsid = route.Code,
                                Option = convertedRouteOption.Option,
                                Min_perc = currentPercentage,
                                Max_perc = currentPercentage + ((decimal)convertedRouteOption.Percentage / 100)
                            };
                            convertedRoutes.Add(convertedRoute);

                            currentPercentage = convertedRoute.Max_perc;
                        }
                    }
                }
            }

            return convertedRoutes.Count > 0 ? convertedRoutes : null;
        }

        private List<FreeRadiusRouteOption> BuildOptions(Route route, string supplierOptionsSeparator)
        {
            if (route.Options == null || route.Options.Count == 0)
                return new List<FreeRadiusRouteOption>() { new FreeRadiusRouteOption() { Option = "BLK", Percentage = 100 } };

            int numberOfAddedOptions = 0;
            CarrierMapping carrierMapping;
            List<FreeRadiusRouteOption> freeRadiusRouteOptions = new List<FreeRadiusRouteOption>();

            List<RouteOption> routeOptions = route.Options;
            bool isPercentageOption = routeOptions.FirstOrDefault(itm => itm.Percentage.HasValue && itm.Percentage.Value > 0) != null;

            if (!isPercentageOption)
            {
                StringBuilder strBuilder = new StringBuilder();

                foreach (RouteOption routeOption in routeOptions)
                {
                    if (numberOfAddedOptions == NumberOfOptions)
                        break;

                    if (routeOption.IsBlocked)
                        continue;

                    carrierMapping = CarrierMappings.GetRecord(routeOption.SupplierId);
                    if (carrierMapping == null || carrierMapping.SupplierMappings == null || carrierMapping.SupplierMappings.Count == 0)
                        continue;

                    for (int i = 1; i <= routeOption.NumberOfTries; i++)
                    {
                        if (numberOfAddedOptions == NumberOfOptions)
                            break;

                        numberOfAddedOptions++;
                        strBuilder.AppendFormat("{0}{1}", strBuilder.Length > 0 ? supplierOptionsSeparator : string.Empty, string.Join(string.Empty, carrierMapping.SupplierMappings.Select(itm => itm.Mapping)));
                    }
                }

                if (strBuilder.Length > 0)
                    freeRadiusRouteOptions.Add(new FreeRadiusRouteOption() { Option = strBuilder.ToString(), Percentage = 100 });
            }
            else
            {
                foreach (RouteOption routeOption in routeOptions)
                {
                    if (numberOfAddedOptions == NumberOfOptions)
                        break;

                    if (routeOption.IsBlocked || !routeOption.Percentage.HasValue)
                        continue;

                    StringBuilder backupOptionsBuilder = new StringBuilder(); 

                    if (routeOption.Backups != null && routeOption.Backups.Count > 0)
                    {
                        foreach (BackupRouteOption backup in routeOption.Backups)
                        {
                            if (backup.IsBlocked)
                                continue;

                            CarrierMapping backUpRouteOptionMapping = CarrierMappings.GetRecord(backup.SupplierId);
                            bool backupRouteOptionIsValid = backUpRouteOptionMapping != null && backUpRouteOptionMapping.SupplierMappings != null && backUpRouteOptionMapping.SupplierMappings.Count > 0;
                            if (backupRouteOptionIsValid)
                                backupOptionsBuilder.Append(this.ConcatenateSupplierMappings(backUpRouteOptionMapping.SupplierMappings));
                        }
                    }

                    CarrierMapping routeOptionMapping = CarrierMappings.GetRecord(routeOption.SupplierId);

                    bool routeOptionIsValid = routeOptionMapping != null && routeOptionMapping.SupplierMappings != null && routeOptionMapping.SupplierMappings.Count > 0;

                    if (!routeOptionIsValid && backupOptionsBuilder.Length == 0)
                        continue;

                    StringBuilder optionsBuilder = new StringBuilder();

                    if (routeOptionIsValid)
                        optionsBuilder.Append(this.ConcatenateSupplierMappings(routeOptionMapping.SupplierMappings));

                    if (backupOptionsBuilder.Length > 0)
                        optionsBuilder.Append(backupOptionsBuilder);

                    freeRadiusRouteOptions.Add(new FreeRadiusRouteOption() { Option = optionsBuilder.ToString(), Percentage = routeOption.Percentage.Value });
                    numberOfAddedOptions++;
                }
            }

            if (freeRadiusRouteOptions.Count == 0)
                freeRadiusRouteOptions.Add(new FreeRadiusRouteOption() { Option = "BLK", Percentage = 100 });

            return freeRadiusRouteOptions;
        }

        private string ConcatenateSupplierMappings(List<SupplierMapping> supplierMappings)
        {
            return string.Join(string.Empty, supplierMappings.Select(itm => itm.Mapping).ToList());
        }

        private void RedistributePercentages(List<FreeRadiusRouteOption> freeRadiusRouteOptions)
        {
            if (freeRadiusRouteOptions == null || freeRadiusRouteOptions.Count == 0)
                return;

            decimal sumOfPercentages = freeRadiusRouteOptions.Sum(itm => itm.Percentage);
            if (sumOfPercentages == 100)
                return;

            decimal remainingPercentage = 100;

            for (var index = 0; index < freeRadiusRouteOptions.Count; index++)
            {
                FreeRadiusRouteOption freeRadiusRouteOption = freeRadiusRouteOptions[index];

                if (index == freeRadiusRouteOptions.Count - 1)
                {
                    freeRadiusRouteOption.Percentage = remainingPercentage;
                }
                else
                {
                    decimal newPercentage = Math.Round((freeRadiusRouteOption.Percentage * 100) / sumOfPercentages, 2);
                    freeRadiusRouteOption.Percentage = newPercentage;
                    remainingPercentage -= newPercentage;
                }
            }
        }

        private SwitchSyncOutput ApplyDifferentialRoutesBatch(List<Route> updatedRoutesBatch, string switchId, string switchName, Action<Exception, bool> writeBusinessHandledException, SwitchSyncOutput previousSwitchSyncOutput)
        {
            List<FreeRadiusConvertedRoute> freeRadiusConvertedRoutes = this.GetFreeRadiusConvertedRoute(updatedRoutesBatch);
            if (freeRadiusConvertedRoutes == null)
                return previousSwitchSyncOutput;

            var freeRadiusApplyDifferentialRoutesContext = new FreeRadiusApplyDifferentialRoutesContext()
            {
                SwitchId = switchId,
                SwitchName = switchName,
                ConvertedUpdatedRoutes = freeRadiusConvertedRoutes,
                WriteBusinessHandledException = writeBusinessHandledException
            };
            this.DataManager.ApplyDifferentialRoutes(freeRadiusApplyDifferentialRoutesContext);

            return TOne.WhS.RouteSync.Business.Helper.MergeSwitchSyncOutputItems(previousSwitchSyncOutput, freeRadiusApplyDifferentialRoutesContext.SwitchSyncOutput);
        }

        #endregion
    }
}