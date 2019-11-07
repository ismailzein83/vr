using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.RouteSync.Cataleya.Data;
using TOne.WhS.RouteSync.Cataleya.Entities;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.RouteSync.Cataleya
{
    public class CataleyaSWSync : SwitchRouteSynchronizer
    {
        public override Guid ConfigId { get { return new Guid("D770F53B-057F-4BB8-BF20-883A2DBC510B"); } }
        public DatabaseConnection DatabaseConnection { get; set; }
        public Guid APIConnectionId { get; set; }
        public ISyncDataManager DataManager { get; set; }

        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

        #region Public Methods

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            var allActiveCustomersTrunksByCustomer = new Dictionary<int, CustomerInfo>();
            var allActiveCustomers = new CarrierAccountManager().GetRoutingActiveCustomers();
            var getCarrierAccountsPreviousVersionNumbersContext = new GetCarrierAccountsPreviousVersionNumbersContext() { CarrierAccountIds = new List<int>() };

            foreach (var customer in allActiveCustomers)
            {
                if (CarrierMappings.TryGetValue(customer.CustomerId.ToString(), out var carrierMapping))
                    if (carrierMapping.CustomerMappings != null && carrierMapping.CustomerMappings.InTrunks != null && carrierMapping.CustomerMappings.InTrunks.Count > 0)
                    {
                        allActiveCustomersTrunksByCustomer.Add(carrierMapping.CarrierId, new CustomerInfo()
                        {
                            CarrierId = carrierMapping.CarrierId,
                            ZoneID = carrierMapping.ZoneID,
                            InTrunks = carrierMapping.CustomerMappings.InTrunks
                        });

                        getCarrierAccountsPreviousVersionNumbersContext.CarrierAccountIds.Add(carrierMapping.CarrierId);
                    }
            }

            var carrierAccountsVersionNumbers = DataManager.GetCarrierAccountsPreviousVersionNumbers(getCarrierAccountsPreviousVersionNumbersContext);

            foreach (var carrierAccountVersionNumber in carrierAccountsVersionNumbers)
            {
                var customerInfo = allActiveCustomersTrunksByCustomer.GetRecord(carrierAccountVersionNumber.CarrierAccountId);
                if (customerInfo != null)
                    customerInfo.Version = carrierAccountVersionNumber.Version;
            }

            this.DataManager.PrepareTables(new RouteInitializeContext()
            {
                CustomersInfoByCustomer = allActiveCustomersTrunksByCustomer
            });

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
            throw new NotImplementedException();
        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            if (context.Routes == null || CarrierMappings == null)
                return;

            List<ConvertedRoute> routes = new List<ConvertedRoute>();

            foreach (var route in context.Routes)
            {
                bool isPercentage;
                string concatenatedOptions = null;
                if (route.Options == null || route.Options.Count == 0)
                {
                    concatenatedOptions = "999999";
                    isPercentage = false;
                }
                else
                {
                    isPercentage = route.Options.FirstOrDefault(itm => itm.Percentage.HasValue && itm.Percentage.Value > 0) != null;
                    if (!isPercentage)
                    {
                        concatenatedOptions = BuildPriorityOptions(route.Options);
                    }
                    else
                    {
                        concatenatedOptions = BuildPercentageOptions(route.Options);
                    }
                }
                if (CarrierMappings.TryGetValue(route.CustomerId, out var carrierMapping))
                {
                    if (carrierMapping.CustomerMappings != null && carrierMapping.CustomerMappings.InTrunks != null && carrierMapping.CustomerMappings.InTrunks.Count > 0)
                    {
                        routes.Add(new CataleyaConvertedRoute()
                        {
                            CarrierID = carrierMapping.CarrierId,
                            Code = route.Code,
                            Options = concatenatedOptions,
                            IsPercentage = isPercentage
                        });
                    }
                }
            }
            context.ConvertedRoutes = routes;
        }

        public override void RemoveConnection(ISwitchRouteSynchronizerRemoveConnectionContext context)
        {
            this.DataManager = null;
        }

        public string GetConnectionString(Guid vrConnectionId)
        {
            var connection = new VRConnectionManager().GetVRConnection(vrConnectionId);
            var settings = connection.Settings as SQLConnection;

            return settings.ConnectionString;
        }

        #endregion

        #region Private Methods

        private string BuildPercentageOptions(List<RouteOption> routeOptions)
        {
            var concatenatedOptionsTrunksWithBackups = new List<string>();

            foreach (var routeOption in routeOptions)
            {
                if (routeOption.IsBlocked || !routeOption.Percentage.HasValue)
                    continue;

                if (CarrierMappings.TryGetValue(routeOption.SupplierId, out var carrierMapping))
                {
                    if (carrierMapping.SupplierMappings == null || carrierMapping.SupplierMappings.OutTrunks == null || carrierMapping.SupplierMappings.OutTrunks.Count == 0)
                        continue;

                    bool isTrunksPercentage = carrierMapping.SupplierMappings.OutTrunks.FirstOrDefault(itm => itm.Percentage.HasValue && itm.Percentage.Value > 0) != null;
                    var routeOptionMainTrunks = new List<string>();
                    var optionBackupsTrunks = new List<OutTrunk>();

                    if (!isTrunksPercentage)
                    {
                        var firstOutTrunk = carrierMapping.SupplierMappings.OutTrunks[0];
                        routeOptionMainTrunks.Add($"{firstOutTrunk.Trunk},{firstOutTrunk.Percentage}");

                        optionBackupsTrunks.AddRange(carrierMapping.SupplierMappings.OutTrunks.Skip(1));
                    }
                    else
                    {
                        foreach (var outTrunk in carrierMapping.SupplierMappings.OutTrunks)
                        {
                            routeOptionMainTrunks.Add($"{outTrunk.Trunk},{outTrunk.Percentage.Value * routeOption.Percentage.Value}");
                        }
                    };

                    foreach (var backupOption in routeOption.Backups)
                    {
                        if (CarrierMappings.TryGetValue(backupOption.SupplierId, out var backupCarrierMapping))
                        {
                            if (backupCarrierMapping.SupplierMappings == null || backupCarrierMapping.SupplierMappings.OutTrunks == null || backupCarrierMapping.SupplierMappings.OutTrunks.Count == 0)
                                continue;

                            optionBackupsTrunks.AddRange(backupCarrierMapping.SupplierMappings.OutTrunks);
                        }
                    }

                    var concatenatedOptionBackupsTrunks = "";
                    if (optionBackupsTrunks.Count > 0)
                        concatenatedOptionBackupsTrunks = $";{string.Join("$", optionBackupsTrunks)}";

                    foreach (var optionMainTrunk in routeOptionMainTrunks)
                    {
                        concatenatedOptionsTrunksWithBackups.Add($"{optionMainTrunk}{concatenatedOptionBackupsTrunks}");
                    }
                }
            }

            if (concatenatedOptionsTrunksWithBackups.Count == 0)
                return "999999";

            return string.Join("|", concatenatedOptionsTrunksWithBackups);
        }

        private string BuildPriorityOptions(List<RouteOption> routeOptions)
        {
            var allOptionsTrunks = new List<string>();
            foreach (var routeOption in routeOptions)
            {
                if (routeOption.IsBlocked)
                    continue;

                if (CarrierMappings.TryGetValue(routeOption.SupplierId, out var carrierMapping))
                {
                    if (carrierMapping.SupplierMappings == null || carrierMapping.SupplierMappings.OutTrunks == null || carrierMapping.SupplierMappings.OutTrunks.Count == 0)
                        continue;

                    foreach (var trunk in carrierMapping.SupplierMappings.OutTrunks)
                    {
                        allOptionsTrunks.Add(trunk.Trunk);
                    }
                }
            }

            if (allOptionsTrunks.Count == 0)
                return "999999";

            return string.Join("|", allOptionsTrunks);
        }

        #endregion
    }
}