using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Idb;
using TOne.WhS.RouteSync.MVTSRadius;

namespace TOne.WhS.RouteSync.TelesIdb
{
    public class TelesIdbSWSync : SwitchRouteSynchronizer
    {
        public override Guid ConfigId { get { return new Guid("29135479-8150-4E23-9A0D-A42AF69A13AE"); } }

        public IIdbDataManager DataManager { get; set; }

        public string MappingSeparator { get; set; }

        public int NumberOfOptions { get; set; }

        public bool UseTwoMappingFormat { get; set; }

        public string SupplierOptionsSeparator { get; set; }
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

                    string concatenatedOptions = BuildOptions(route, invalidRoutes);

                    foreach (string customerMapping in carrierMapping.CustomerMapping)
                    {
                        if (string.IsNullOrEmpty(customerMapping))
                            continue;

                        TelesIdbConvertedRoute radiusRoute = new TelesIdbConvertedRoute()
                        {
                            Pref = string.Format("{0}{1}", customerMapping, route.Code),
                            Route = concatenatedOptions
                        };
                        idbRoutes.Add(radiusRoute);
                    }
                }
            }
            context.InvalidRoutes = invalidRoutes.Count > 0 ? invalidRoutes : null;
            context.ConvertedRoutes = idbRoutes;
        }

        private string BuildOptions(Route route, List<string> invalidRoutes)
        {
            return null;
            //int numberOfOptionsAdded = 0;
            //StringBuilder outputBuffer = new StringBuilder();
            //bool shouldStop = false;
            
            //if (route.Options == null || route.Options.Count == 0)
            //    return "BLK";

            //List<RouteOption> options = route.Options;

            //bool isPercentageOption = options.Sum(option => option.Percentage.HasValue ? option.Percentage.Value : 0) == 0 ? false : true;

            //for (int k = 0; k < options.Count; k++)
            //{
            //    var currentOption = options[k];

            //    if (options.Count > 1)
            //    {
            //        var option2 = k == options.Count - 1 || (isPercentageOption && currentOption.Percentage == 0) ? options[k - 1] : options[k + 1];
            //        if (isPercentageOption && currentOption.IsBlocked && option2.IsBlocked && currentOption.Percentage > 0 && option2.Percentage == 0)
            //            continue;
            //    }

            //    CarrierMapping supplierMapping;
            //    if (currentOption.IsValid && CarrierMappings.TryGetValue(currentOption.SupplierId, out supplierMapping))
            //    {
            //        string outputId = supplierMapping.SupplierMapping;
            //        if (isPercentageOption && outputId.Length == 4 && UseTwoMappingFormat && !currentOption.IsBlocked)
            //        {
            //            outputId = outputId + "XXXX";
            //        }
            //        if (currentOption.IsBlocked && isPercentageOption)
            //        {
            //            RouteOption otherOption = null;
            //            if (currentOption.Percentage > 0)
            //                otherOption = options[k + 1];
            //            else
            //                otherOption = k == 0 ? options[k] : options[k - 1];
            //            if (otherOption.IsValid && CarrierMappings.TryGetValue(otherOption.SupplierId, out supplierMapping))
            //            {
            //                outputId = supplierMapping.SupplierMapping;
            //                if (isPercentageOption && outputId.Length == 4 && UseTwoMappingFormat)
            //                {
            //                    outputId = outputId + "XXXX";
            //                }
            //            }
            //        }
            //        if ((isPercentageOption && currentOption.IsBlocked) || !currentOption.IsBlocked)
            //            for (int i = 0; i < currentOption.NumberOfTries; i++)
            //            {
            //                numberOfOptionsAdded++;
            //                if (numberOfOptionsAdded <= NumberOfOptions)
            //                {
            //                    string mappedOption = string.Format("{0}{1}{2}", isPercentageOption ? GetPercentage(currentOption.Percentage) : string.Empty, outputId, useSeperator);
            //                    outputBuffer.Append(mappedOption == ";" ? string.Empty : mappedOption);
            //                }
            //                else
            //                {
            //                    shouldStop = true;
            //                    break;
            //                }
            //            }
            //    }
            //    if (shouldStop) break;
            //}
            //char[] trims = { ';' };
            //return outputBuffer.Length == 0 ? "BLK" : outputBuffer.ToString().Trim(trims);
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

        public string SupplierMapping { get; set; }
    }
}
