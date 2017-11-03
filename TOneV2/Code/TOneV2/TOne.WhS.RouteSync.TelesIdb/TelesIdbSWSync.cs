﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Idb;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.TelesIdb
{
    public class TelesIdbSWSync : SwitchRouteSynchronizer
    {
        public const int SupplierMappingLength = 4;

        public override Guid ConfigId { get { return new Guid("29135479-8150-4E23-9A0D-A42AF69A13AE"); } }

        public IIdbDataManager DataManager { get; set; }

        public string MappingSeparator { get; set; }

        public int? NumberOfMappings { get; set; }

        public string SupplierOptionsSeparator { get; set; }

        public int NumberOfOptions { get; set; }

        /// <summary>
        /// Key = Carrier Account Id
        /// </summary>
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }


        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            this.DataManager.PrepareTables(context);
        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            if (context.Routes == null || CarrierMappings == null)
                return;

            List<string> invalidRoutes = new List<string>();
            CarrierMapping carrierMapping;
            List<ConvertedRoute> idbRoutes = new List<ConvertedRoute>();

            foreach (var route in context.Routes)
            {
                if (CarrierMappings.TryGetValue(route.CustomerId, out carrierMapping))
                {
                    if (carrierMapping.CustomerMapping == null)
                        continue;

                    string supplierOptionsSeparator = !string.IsNullOrEmpty(this.SupplierOptionsSeparator) ? this.SupplierOptionsSeparator : string.Empty;
                    string concatenatedOptions = BuildOptions(route, invalidRoutes, supplierOptionsSeparator);

                    foreach (string customerMapping in carrierMapping.CustomerMapping)
                    {
                        if (string.IsNullOrEmpty(customerMapping))
                            continue;

                        TelesIdbConvertedRoute idbRoute = new TelesIdbConvertedRoute()
                        {
                            Pref = string.Format("{0}{1}", customerMapping, route.Code),
                            Route = concatenatedOptions
                        };
                        idbRoutes.Add(idbRoute);
                    }
                }
            }

            context.InvalidRoutes = invalidRoutes.Count > 0 ? invalidRoutes : null;
            context.ConvertedRoutes = idbRoutes;
        }


        //Private Methods
        public string BuildOptions(Route route, List<string> invalidRoutes, string supplierOptionsSeparator)
        {
            if (route.Options == null || route.Options.Count == 0)
                return "BLK";

            StringBuilder strBuilder = new StringBuilder();

            List<RouteOption> routeOptions = route.Options;
            bool isPercentageOption = routeOptions.FirstOrDefault(itm => itm.Percentage.HasValue && itm.Percentage.Value > 0) != null;

            int numberOfAddedOptions = 0;
            CarrierMapping carrierMapping;

            if (!isPercentageOption)
            {
                foreach (RouteOption routeOption in routeOptions)
                {
                    if (numberOfAddedOptions == NumberOfOptions)
                        break;

                    if (routeOption.IsBlocked)
                        continue;

                    carrierMapping = CarrierMappings.GetRecord(routeOption.SupplierId);
                    if (carrierMapping == null || carrierMapping.SupplierMapping == null || carrierMapping.SupplierMapping.Count == 0)
                        continue;

                    for (int i = 1; i <= routeOption.NumberOfTries; i++)
                    {
                        if (numberOfAddedOptions == NumberOfOptions)
                            break;

                        numberOfAddedOptions++;
                        strBuilder.AppendFormat("{0}{1}", strBuilder.Length > 0 ? supplierOptionsSeparator : string.Empty, string.Join(string.Empty, carrierMapping.SupplierMapping));
                    }
                }
            }
            else
            {
                RouteOption nextRouteOption = null;

                for (var x = 0; x < routeOptions.Count; x++)
                {
                    if (numberOfAddedOptions == NumberOfOptions)
                        break;

                    RouteOption routeOption = routeOptions[x];
                    if (!routeOption.Percentage.HasValue || routeOption.Percentage.Value == 0 || !routeOption.IsValid)// routeOption is a backUp option
                        continue;

                    // routeOption is not a backUp option and valid
                    nextRouteOption = x < routeOptions.Count - 1 ? routeOptions[x + 1] : null;
                    bool nextOptionIsBackUp = nextRouteOption != null && (!nextRouteOption.Percentage.HasValue || nextRouteOption.Percentage.Value == 0);
                    bool shouldTakeNextRouteOption = nextOptionIsBackUp && !nextRouteOption.IsBlocked;

                    CarrierMapping backUpRouteOptionMapping = null;
                    if (nextOptionIsBackUp)
                    {
                        if (!shouldTakeNextRouteOption)
                        {
                            backUpRouteOptionMapping = CarrierMappings.GetRecord(routeOption.SupplierId);
                            if (backUpRouteOptionMapping == null || backUpRouteOptionMapping.SupplierMapping == null || backUpRouteOptionMapping.SupplierMapping.Count == 0)
                                continue;
                        }
                        else
                        {
                            backUpRouteOptionMapping = CarrierMappings.GetRecord(nextRouteOption.SupplierId);
                            if (backUpRouteOptionMapping == null || backUpRouteOptionMapping.SupplierMapping == null || backUpRouteOptionMapping.SupplierMapping.Count == 0)
                            {
                                backUpRouteOptionMapping = CarrierMappings.GetRecord(routeOption.SupplierId);
                                if (backUpRouteOptionMapping == null || backUpRouteOptionMapping.SupplierMapping == null || backUpRouteOptionMapping.SupplierMapping.Count == 0)
                                    continue;
                            }
                        }
                    }

                    CarrierMapping routeOptionMapping = null;
                    if (routeOption.IsBlocked)
                    {
                        if (!shouldTakeNextRouteOption)
                            continue;

                        routeOptionMapping = CarrierMappings.GetRecord(nextRouteOption.SupplierId);
                        if (routeOptionMapping == null || routeOptionMapping.SupplierMapping == null || routeOptionMapping.SupplierMapping.Count == 0)
                            continue;
                    }
                    else
                    {
                        routeOptionMapping = CarrierMappings.GetRecord(routeOption.SupplierId);
                        if (routeOptionMapping == null || routeOptionMapping.SupplierMapping == null || routeOptionMapping.SupplierMapping.Count == 0)
                        {
                            if (!shouldTakeNextRouteOption)
                                continue;

                            routeOptionMapping = CarrierMappings.GetRecord(nextRouteOption.SupplierId);
                            if (routeOptionMapping == null || routeOptionMapping.SupplierMapping == null || routeOptionMapping.SupplierMapping.Count == 0)
                                continue;
                        }
                    }

                    string concatSupplierMapping = this.ConcatenateSupplierMappings(routeOptionMapping.SupplierMapping);
                    concatSupplierMapping = GetPercentage(routeOption.Percentage) + concatSupplierMapping;
                    //string concatSupplierMapping = string.Join(string.Empty, routeOptionMapping.SupplierMapping);
                    //if (UseTwoSuppliersMapping && concatSupplierMapping.Length == SupplierMappingLength)
                    //    concatSupplierMapping = concatSupplierMapping + "XXXX";

                    string concatBackUpSupplierMapping = string.Empty;
                    if (backUpRouteOptionMapping != null)
                    {
                        concatBackUpSupplierMapping = this.ConcatenateSupplierMappings(backUpRouteOptionMapping.SupplierMapping);
                        concatBackUpSupplierMapping = GetPercentage(null) + concatBackUpSupplierMapping;
                        //concatBackUpSupplierMapping = string.Join(string.Empty, backUpRouteOptionMapping.SupplierMapping);
                        //if (UseTwoSuppliersMapping && concatBackUpSupplierMapping.Length == SupplierMappingLength)
                        //    concatBackUpSupplierMapping = concatBackUpSupplierMapping + "XXXX";
                    }

                    //for (int i = 1; i <= routeOption.NumberOfTries; i++)
                    //{
                    //    if (numberOfAddedOptions == NumberOfOptions)
                    //        break;

                    numberOfAddedOptions++;
                    strBuilder.AppendFormat("{0}{1}{2}", strBuilder.Length > 0 ? supplierOptionsSeparator : string.Empty, concatSupplierMapping, concatBackUpSupplierMapping);
                    //}
                }
            }
            return strBuilder.Length > 0 ? strBuilder.ToString() : "BLK";
        }

        private string ConcatenateSupplierMappings(List<string> supplierMappings)
        {
            if (!this.NumberOfMappings.HasValue)
                return string.Join(string.Empty, supplierMappings);

            string concatSupplierMapping = string.Join(string.Empty, supplierMappings.Take(this.NumberOfMappings.Value));

            int numberOfSupplierMappings = supplierMappings.Count;
            if (numberOfSupplierMappings < this.NumberOfMappings.Value)
            {
                int numberOfXToAdd = (this.NumberOfMappings.Value - numberOfSupplierMappings) * SupplierMappingLength;
                concatSupplierMapping = string.Format("{0}{1}", concatSupplierMapping, new StringBuilder(numberOfXToAdd).Insert(0, "X", numberOfXToAdd).ToString());
            }

            return concatSupplierMapping;
        }

        private string GetPercentage(int? percentage)
        {
            if (!percentage.HasValue)
                return "00";

            if (percentage == 0)
                return "00";

            if (percentage == 100)
                return "99";

            string percentageAsString = percentage.ToString();
            return percentageAsString.Length == 1 ? percentageAsString.Insert(0, "0") : percentageAsString;
        }


        public override Object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
        {
            return this.DataManager.PrepareDataForApply(context.ConvertedRoutes);
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

        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            this.DataManager.ApplySwitchRouteSyncRoutes(context);
        }

        public override bool TryBlockCustomer(ITryBlockCustomerContext context)
        {
            CarrierMapping customerMapping;
            if (!CarrierMappings.TryGetValue(context.CustomerId, out customerMapping))
                return false;

            if (customerMapping.CustomerMapping == null || customerMapping.CustomerMapping.Count == 0)
                return false;

            IdbBlockCustomerContext blockCustomerContext = new IdbBlockCustomerContext() { CustomerMappings = customerMapping.CustomerMapping, SwitchName = context.SwitchName };

            if (!this.DataManager.BlockCustomer(blockCustomerContext))
                throw new Exception(blockCustomerContext.ErrorMessage);

            return true;
        }

        public override bool IsSwitchRouteSynchronizerValid(IIsSwitchRouteSynchronizerValidContext context)
        {
            if (this.CarrierMappings == null || this.CarrierMappings.Count == 0)
                return true;

            HashSet<string> customerMappings = new HashSet<string>();
            HashSet<string> supplierMappings = new HashSet<string>();

            HashSet<string> duplicateCustomerMappings = new HashSet<string>();
            HashSet<string> duplicateSupplierMappings = new HashSet<string>();
            HashSet<int> invalidMappingSupplierIds = new HashSet<int>();

            foreach (var mapping in this.CarrierMappings.Values)
            {
                if (mapping.CustomerMapping != null)
                {
                    foreach (var customerMapping in mapping.CustomerMapping)
                    {
                        if (customerMappings.Contains(customerMapping))
                        {
                            duplicateCustomerMappings.Add(customerMapping);
                            continue;
                        }

                        customerMappings.Add(customerMapping);
                    }
                }

                if (mapping.SupplierMapping != null)
                {
                    foreach (var supplierMapping in mapping.SupplierMapping)
                    {
                        if (supplierMapping.Length != SupplierMappingLength)
                        {
                            invalidMappingSupplierIds.Add(mapping.CarrierId);
                            continue;
                        }

                        if (supplierMappings.Contains(supplierMapping))
                        {
                            duplicateSupplierMappings.Add(supplierMapping);
                            continue;
                        }

                        supplierMappings.Add(supplierMapping);
                    }
                }
            }

            if (duplicateCustomerMappings.Count == 0 && duplicateSupplierMappings.Count == 0 && invalidMappingSupplierIds.Count == 0)
                return true;

            List<string> validationMessages = new List<string>();

            if (duplicateCustomerMappings.Count > 0)
                validationMessages.Add(string.Format("Duplicate Customer Mappings: {0}", string.Join(", ", duplicateCustomerMappings)));

            if (duplicateSupplierMappings.Count > 0)
                validationMessages.Add(string.Format("Duplicate Supplier Mappings: {0}", string.Join(", ", duplicateSupplierMappings)));

            if (invalidMappingSupplierIds.Count > 0)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                List<string> invalidMappingSuppliers = new List<string>();

                foreach (var supplierId in invalidMappingSupplierIds)
                {
                    //CarrierAccount carrierAccount = carrierAccountManager.GetCarrierAccount(supplierId);
                    //string sourceIdDesc = carrierAccount != null && !string.IsNullOrEmpty(carrierAccount.SourceId) ? string.Format(" - SourceID: {0}", carrierAccount.SourceId) : string.Empty;

                    invalidMappingSuppliers.Add(string.Format("'{0}'", carrierAccountManager.GetCarrierAccountName(supplierId)));
                }

                validationMessages.Add(string.Format("Invalid Mappings for Suppliers: {0}", string.Join(", ", invalidMappingSuppliers)));
            }

            context.ValidationMessages = validationMessages;
            return false;
        }
    }

    public class CarrierMapping
    {
        public int CarrierId { get; set; }

        public List<string> CustomerMapping { get; set; }

        public List<string> SupplierMapping { get; set; }
    }
}
