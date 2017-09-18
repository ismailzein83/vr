using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Idb;


namespace TOne.WhS.RouteSync.TelesIdb
{
    public class TelesIdbSWSync : SwitchRouteSynchronizer
    {
        public override Guid ConfigId { get { return new Guid("29135479-8150-4E23-9A0D-A42AF69A13AE"); } }

        public IIdbDataManager DataManager { get; set; }

        public string MappingSeparator { get; set; }

        public int NumberOfOptions { get; set; }

        public string SupplierOptionsSeparator { get; set; }

        public bool UseTwoSuppliersMapping { get; set; }

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

                    string supplierOptionsSeparator = !string.IsNullOrEmpty(SupplierOptionsSeparator) ? SupplierOptionsSeparator : string.Empty;
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

        public string BuildOptions(Route route, List<string> invalidRoutes, string supplierOptionsSeparator)
        {
            if (route.Options == null || route.Options.Count == 0)
                return "BLK";

            List<RouteOption> routeOptions = route.Options;

            bool isPercentageOption = routeOptions.FirstOrDefault(itm => itm.Percentage.HasValue && itm.Percentage.Value > 0) != null;
            StringBuilder strBuilder = new StringBuilder();

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
                    RouteOption routeOption = routeOptions[x];
                    if (numberOfAddedOptions == NumberOfOptions)
                        break;

                    if (!routeOption.Percentage.HasValue || routeOption.Percentage.Value == 0 || !routeOption.IsValid)// routeOption is a backUp option
                    {
                        continue;
                    }
                    else// routeOption is not a backUp option
                    {
                        nextRouteOption = x < routeOptions.Count - 1 ? routeOptions[x + 1] : null;
                        bool nextOptionIsBackUp = nextRouteOption != null && (!nextRouteOption.Percentage.HasValue || nextRouteOption.Percentage.Value == 0);
                        bool shouldTakeNextRouteOption = nextOptionIsBackUp && !nextRouteOption.IsBlocked;

                        CarrierMapping backUpRouteOptionMapping = null;
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

                        CarrierMapping routeOptionMapping = null;
                        if (routeOption.IsBlocked)
                        {
                            if (!shouldTakeNextRouteOption)
                            {
                                continue;
                            }
                            else
                            {
                                routeOptionMapping = CarrierMappings.GetRecord(nextRouteOption.SupplierId);
                                if (routeOptionMapping == null || routeOptionMapping.SupplierMapping == null || routeOptionMapping.SupplierMapping.Count == 0)
                                    continue;
                            }
                        }
                        else
                        {
                            routeOptionMapping = CarrierMappings.GetRecord(routeOption.SupplierId);
                            if (routeOptionMapping == null || routeOptionMapping.SupplierMapping == null || routeOptionMapping.SupplierMapping.Count == 0)
                            {
                                if (!shouldTakeNextRouteOption)
                                {
                                    continue;
                                }
                                else
                                {
                                    routeOptionMapping = CarrierMappings.GetRecord(nextRouteOption.SupplierId);
                                    if (routeOptionMapping == null || routeOptionMapping.SupplierMapping == null || routeOptionMapping.SupplierMapping.Count == 0)
                                        continue;
                                }
                            }
                        }

                        string concatSupplierMapping = string.Join(string.Empty, routeOptionMapping.SupplierMapping);
                        if (UseTwoSuppliersMapping && concatSupplierMapping.Length == 4)
                            concatSupplierMapping = concatSupplierMapping + "XXXX";
                        concatSupplierMapping = GetPercentage(routeOption.Percentage) + concatSupplierMapping;

                        string concatBackUpSupplierMapping = string.Empty;
                        if (nextOptionIsBackUp)
                        {
                            concatBackUpSupplierMapping = string.Join(string.Empty, backUpRouteOptionMapping.SupplierMapping);
                            if (UseTwoSuppliersMapping && concatBackUpSupplierMapping.Length == 4)
                                concatBackUpSupplierMapping = concatBackUpSupplierMapping + "XXXX";
                            concatBackUpSupplierMapping = GetPercentage(null) + concatBackUpSupplierMapping;
                        }

                        for (int i = 1; i <= routeOption.NumberOfTries; i++)
                        {
                            if (numberOfAddedOptions == NumberOfOptions)
                                break;

                            numberOfAddedOptions++;
                            strBuilder.AppendFormat("{0}{1}{2}", strBuilder.Length > 0 ? supplierOptionsSeparator : string.Empty, concatSupplierMapping, concatBackUpSupplierMapping);
                        }
                    }
                }
            }
            return strBuilder.Length > 0 ? strBuilder.ToString() : "BLK";
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
    }

    public class CarrierMapping
    {
        public int CarrierId { get; set; }

        public List<string> CustomerMapping { get; set; }

        public List<string> SupplierMapping { get; set; }
    }
}
