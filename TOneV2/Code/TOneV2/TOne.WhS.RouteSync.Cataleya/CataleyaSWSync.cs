using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.RouteSync.Cataleya.Data;
using TOne.WhS.RouteSync.Cataleya.Entities;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Cataleya
{
    public class CataleyaSWSync : SwitchRouteSynchronizer
    {
        public override Guid ConfigId { get { return new Guid("D770F53B-057F-4BB8-BF20-883A2DBC510B"); } }
        public Guid APIConnectionId { get; set; }
        public ICataleyaDataManager DataManager { get; set; }
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

        #region Public Methods

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            var allActiveCarrierAccountsMappingByCarrier = new Dictionary<int, CarrierAccountMapping>();

            var customersIdentification = new List<CustomerIdentification>();
            var carrierAccountsMapping = new List<CarrierAccountMapping>();
            var routeTableNames = new List<string>();

            var allActiveCustomers = new CarrierAccountManager().GetRoutingActiveCustomers();
            var getCarrierAccountsPreviousVersionNumbersContext = new GetCarrierAccountsPreviousVersionNumbersContext() { CarrierAccountIds = new List<int>() };

            foreach (var customer in allActiveCustomers)
            {
                if (CarrierMappings.TryGetValue(customer.CustomerId.ToString(), out var carrierMapping))
                    if (carrierMapping.CustomerMappings != null && carrierMapping.CustomerMappings.InTrunks != null && carrierMapping.CustomerMappings.InTrunks.Count > 0)
                    {
                        var carrierAccountMapping = new CarrierAccountMapping()
                        {
                            CarrierId = carrierMapping.CarrierId,
                            ZoneID = carrierMapping.ZoneID,
                            Version = 0,
                            RouteTableName = $"Rt_{carrierMapping.CarrierId}_0"
                        };

                        allActiveCarrierAccountsMappingByCarrier.Add(carrierMapping.CarrierId, carrierAccountMapping);
                        carrierAccountsMapping.Add(carrierAccountMapping);

                        getCarrierAccountsPreviousVersionNumbersContext.CarrierAccountIds.Add(carrierMapping.CarrierId);

                        foreach (var trunk in carrierMapping.CustomerMappings.InTrunks)
                        {
                            customersIdentification.Add(new CustomerIdentification()
                            {
                                CarrierId = carrierMapping.CarrierId,
                                Trunk = trunk.Trunk
                            });
                        }
                    }
            }

            var carrierAccountsPreviousVersionInfo = DataManager.GetCarrierAccountsPreviousVersion(getCarrierAccountsPreviousVersionNumbersContext);

            if (carrierAccountsPreviousVersionInfo != null)
            {
                foreach (var carrierAccountVersionInfo in carrierAccountsPreviousVersionInfo)
                {
                    var carrierAccountMapping = allActiveCarrierAccountsMappingByCarrier.GetRecord(carrierAccountVersionInfo.CarrierId);
                    if (carrierAccountMapping == null)
                        continue;

                    var newVersion = (carrierAccountVersionInfo.Version + 1) % 2;
                    carrierAccountMapping.Version = newVersion;

                    carrierAccountMapping.RouteTableName = $"Rt_{carrierAccountMapping.CarrierId}_{newVersion}";
                }
            }

            foreach (var carrierMapping in carrierAccountsMapping)
            {
                routeTableNames.Add(carrierMapping.RouteTableName);
            }

            DataManager.PrepareTables(new RouteInitializeContext()
            {
                CustomersIdentification = customersIdentification,
                CarrierAccountsMapping = carrierAccountsMapping,
                RouteTableNames = routeTableNames
            });
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

        public override object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
        {
            return DataManager.PrepareDataForApply(context.ConvertedRoutes);
        }

        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            DataManager.ApplySwitchRouteSyncRoutes(context);
        }

        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            var carrierFinalizationDataByCustomer = new Dictionary<int, CarrierFinalizationData>();

            var oldCustomerIdentifications = DataManager.GetAllCustomerIdentifications(false);
            var customerIdentifications = DataManager.GetAllCustomerIdentifications(true);

            var oldCarrierAccountsMapping = DataManager.GetAllCarrierAccountsMapping(false);
            var carrierAccountsMapping = DataManager.GetAllCarrierAccountsMapping(true);
            var routeTableNames = new List<String>();

            if (customerIdentifications != null && customerIdentifications.Count > 0)
            {
                for (int i = 0; i < customerIdentifications.Count; i++)
                {
                    var customerIdentification = customerIdentifications[i];
                    var carrierFinalizationData = carrierFinalizationDataByCustomer.GetOrCreateItem(customerIdentification.CarrierId);

                    if (oldCustomerIdentifications != null && customerIdentifications.Count != 0)
                        for (int j = 0; j < oldCustomerIdentifications.Count; j++)
                        {
                            var oldCustomerIdentification = oldCustomerIdentifications[j];
                            if (oldCustomerIdentification.CarrierId == customerIdentification.CarrierId && oldCustomerIdentification.Trunk == customerIdentification.Trunk)
                            {
                                oldCustomerIdentifications.RemoveAt(j);
                                break;
                            }
                        }
                    else
                    {
                        if (carrierFinalizationData.CustomerIdentificationsToAdd == null)
                            carrierFinalizationData.CustomerIdentificationsToAdd = new List<CustomerIdentification>() { };

                        carrierFinalizationData.CustomerIdentificationsToAdd.Add(customerIdentification);
                    }
                }
            }

            if (oldCustomerIdentifications != null && customerIdentifications.Count != 0)
            {
                foreach (var oldCustomerIdentification in oldCustomerIdentifications)
                {
                    var carrierFinalizationData = carrierFinalizationDataByCustomer.GetOrCreateItem(oldCustomerIdentification.CarrierId);

                    if (carrierFinalizationData.CustomerIdentificationsToDelete == null)
                        carrierFinalizationData.CustomerIdentificationsToDelete = new List<CustomerIdentification>() { };

                    carrierFinalizationData.CustomerIdentificationsToAdd.Add(oldCustomerIdentification);
                }
            }

            if (carrierAccountsMapping != null && carrierAccountsMapping.Count != 0)
            {
                for (int i = 0; i < carrierAccountsMapping.Count; i++)
                {
                    var carrierAccountMapping = carrierAccountsMapping[i];
                    var carrierFinalizationData = carrierFinalizationDataByCustomer.GetOrCreateItem(carrierAccountMapping.CarrierId);
                    bool isItemToUpdate = false;

                    if (oldCarrierAccountsMapping != null && oldCarrierAccountsMapping.Count != 0)
                        for (int j = 0; j < oldCarrierAccountsMapping.Count; j++)
                        {
                            var oldcarrierAccount = oldCarrierAccountsMapping[j];

                            if (oldcarrierAccount.CarrierId == carrierAccountMapping.CarrierId)
                            {
                                isItemToUpdate = true;
                                carrierFinalizationData.CarrierAccountMappingToUpdate = carrierAccountMapping;
                                oldCustomerIdentifications.RemoveAt(j);
                                routeTableNames.Add(carrierAccountMapping.RouteTableName);
                                break;
                            }
                        }
                    if (!isItemToUpdate)
                    {
                        carrierFinalizationData.CarrierAccountMappingToAdd = carrierAccountMapping;
                        routeTableNames.Add(carrierAccountMapping.RouteTableName);
                    }
                }
            }

            if (oldCarrierAccountsMapping != null && oldCarrierAccountsMapping.Count != 0)
            {
                foreach (var oldCarrierAccountMapping in oldCarrierAccountsMapping)
                {
                    var carrierFinalizationData = carrierFinalizationDataByCustomer.GetOrCreateItem(oldCarrierAccountMapping.CarrierId);

                    carrierFinalizationData.CarrierAccountMappingToUpdate = new CarrierAccountMapping()
                    {
                        ZoneID = oldCarrierAccountMapping.ZoneID,
                        CarrierId = oldCarrierAccountMapping.CarrierId,
                        RouteTableName = null,
                        Version = (oldCarrierAccountMapping.Version + 1) % 2
                    };
                }
            }

            DataManager.Finalize(new CataleyaFinalizeContext() { CarrierFinalizationDataByCustomer = carrierFinalizationDataByCustomer });
        }

        public override void RemoveConnection(ISwitchRouteSynchronizerRemoveConnectionContext context)
        {
            DataManager = null;
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