﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        public Guid APIConnectionId { get; set; }
        public ICataleyaDataManager DataManager { get; set; }
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }
        public int NodeID { get; set; }

        const string blockedTrunk = "999999";

        const char optionsSeparatorAsChar = '|';
        const string optionsSeparatorAsString = "|";

        const char trunkPercentageSeparatorAsChar = ',';
        const string trunkPercentageSeparatorAsString = ",";

        const char trunkBackupsSeparatorAsChar = ';';
        const string trunkBackupsSeparatorAsString = ";";

        const char backupsSeparatorAsChar = '$';
        const string backupsSeparatorAsString = "$";

        #region Public Methods

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            DataManager.InitializeCarrierAccountMappingTable();
            DataManager.InitializeCustomerIdentificationTable();

            var carrierAccountMappingByAccountId = new Dictionary<int, CarrierAccountMapping>();
            var customersIdentification = new List<CustomerIdentification>();

            var allActiveCustomers = new CarrierAccountManager().GetRoutingActiveCustomers();
            if (allActiveCustomers == null || !allActiveCustomers.Any())
                return;

            if (CarrierMappings == null || !CarrierMappings.Any())
                return;

            foreach (var customer in allActiveCustomers)
            {
                if (!CarrierMappings.TryGetValue(customer.CustomerId.ToString(), out CarrierMapping carrierMapping))
                    continue;

                if (carrierMapping.CustomerMappings == null || carrierMapping.CustomerMappings.InTrunks == null || carrierMapping.CustomerMappings.InTrunks.Count == 0)
                    continue;

                var carrierAccountMapping = new CarrierAccountMapping()
                {
                    CarrierId = carrierMapping.CarrierId,
                    ZoneID = carrierMapping.ZoneID,
                    Version = 0,
                    RouteTableName = Helper.BuildRouteTableName(carrierMapping.CarrierId, 0)
                    //Status = CarrierAccountStatus.Active
                };

                carrierAccountMappingByAccountId.Add(carrierAccountMapping.CarrierId, carrierAccountMapping);

                foreach (var trunk in carrierMapping.CustomerMappings.InTrunks)
                {
                    customersIdentification.Add(new CustomerIdentification()
                    {
                        CarrierId = carrierMapping.CarrierId,
                        Trunk = trunk.Trunk
                    });
                }
            }

            var existingCarrierAccountMappings = DataManager.GetCarrierAccountMappings(false);

            if (existingCarrierAccountMappings != null)
            {
                foreach (var existingCarrierAccount in existingCarrierAccountMappings)
                {
                    var carrierAccountMapping = carrierAccountMappingByAccountId.GetRecord(existingCarrierAccount.CarrierId);
                    if (carrierAccountMapping == null)
                        continue;

                    var newVersion = (existingCarrierAccount.Version + 1) % 3;
                    carrierAccountMapping.Version = newVersion;
                    carrierAccountMapping.RouteTableName = Helper.BuildRouteTableName(existingCarrierAccount.CarrierId, newVersion);
                }
            }
            DataManager.FillTempCarrierAccountMappingTable(carrierAccountMappingByAccountId.Values);
            DataManager.FillTempCustomerIdentificationTable(customersIdentification);
            DataManager.InitializeRouteTables(carrierAccountMappingByAccountId.Values);
        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            if (context.Routes == null || CarrierMappings == null)
                return;

            List<ConvertedRoute> routes = new List<ConvertedRoute>();

            foreach (var route in context.Routes)
            {
                if (!CarrierMappings.TryGetValue(route.CustomerId, out var carrierMapping))
                    continue;

                if (carrierMapping.CustomerMappings == null || carrierMapping.CustomerMappings.InTrunks == null || carrierMapping.CustomerMappings.InTrunks.Count == 0)
                    continue;

                bool isPercentage;
                string concatenatedOptions = null;
                if (route.Options == null || route.Options.Count == 0)
                {
                    concatenatedOptions = blockedTrunk;
                    isPercentage = false;
                }
                else
                {
                    isPercentage = route.Options.FirstOrDefault(itm => !itm.IsBlocked && itm.Percentage.HasValue && itm.Percentage.Value > 0) != null;
                    if (!isPercentage)
                    {
                        concatenatedOptions = BuildPriorityOptions(route.Options);
                    }
                    else
                    {
                        concatenatedOptions = BuildPercentageOptions(route.Options, out isPercentage);
                    }
                }

                routes.Add(new CataleyaConvertedRoute()
                {
                    CarrierID = carrierMapping.CarrierId,
                    Code = route.Code,
                    Options = concatenatedOptions,
                    IsPercentage = isPercentage
                });
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
            var finalCustomerDataByAccountId = new Dictionary<int, FinalCustomerData>();

            var oldCustomerIdentificationsDict = ConvertToDictionary(DataManager.GetCustomerIdentifications(false));
            var customerIdentificationsDict = ConvertToDictionary(DataManager.GetCustomerIdentifications(true));

            var routeTableNamesForIndexes = new List<String>();

            if (customerIdentificationsDict != null && customerIdentificationsDict.Count > 0)
            {
                foreach (var customerIdentificationKVP in customerIdentificationsDict)
                {
                    int customerId = customerIdentificationKVP.Key;
                    var customerIdentifications = customerIdentificationKVP.Value;

                    var finalCustomerData = finalCustomerDataByAccountId.GetOrCreateItem(customerId);

                    if (oldCustomerIdentificationsDict != null && oldCustomerIdentificationsDict.Count >= 0)
                    {
                        var oldCustomerIdentifications = oldCustomerIdentificationsDict.GetRecord(customerId);
                        if (oldCustomerIdentifications != null && oldCustomerIdentifications.Count > 0)
                        {
                            var itemsToAdd = GetUpdatedItems(customerIdentifications, oldCustomerIdentifications);
                            if (itemsToAdd != null && itemsToAdd.Count > 0)
                                finalCustomerData.CustomerIdentificationsToAdd.AddRange(itemsToAdd);

                            var itemsToDelete = GetUpdatedItems(oldCustomerIdentifications, customerIdentifications);
                            if (itemsToDelete != null && itemsToDelete.Count > 0)
                                finalCustomerData.CustomerIdentificationsToDelete.AddRange(itemsToDelete);
                        }
                        else
                        {
                            finalCustomerData.CustomerIdentificationsToAdd.AddRange(customerIdentifications);
                        }
                    }
                    else
                    {
                        finalCustomerData.CustomerIdentificationsToAdd.AddRange(customerIdentifications);
                    }
                }
            }
            else
            {
                if (oldCustomerIdentificationsDict != null && oldCustomerIdentificationsDict.Count >= 0)
                {
                    foreach (var oldCustomerIdentificationsKVP in oldCustomerIdentificationsDict)
                    {
                        int oldCustomerId = oldCustomerIdentificationsKVP.Key;
                        var oldCustomerIdentifications = oldCustomerIdentificationsKVP.Value;

                        var finalCustomerData = finalCustomerDataByAccountId.GetOrCreateItem(oldCustomerId);
                        finalCustomerData.CustomerIdentificationsToDelete.AddRange(oldCustomerIdentifications);
                    }
                }
            }

            Dictionary<int, CarrierAccountMapping> oldCarrierAccountMappingsDict = null;
            var oldCarrierAccountMappings = DataManager.GetCarrierAccountMappings(false);
            if (oldCarrierAccountMappings != null)
                oldCarrierAccountMappingsDict = oldCarrierAccountMappings.ToDictionary(itm => itm.CarrierId, itm => itm);


            Dictionary<int, CarrierAccountMapping> carrierAccountMappingsDict = null;
            var carrierAccountMappings = DataManager.GetCarrierAccountMappings(true);
            if (carrierAccountMappings != null)
                carrierAccountMappingsDict = carrierAccountMappings.ToDictionary(itm => itm.CarrierId, itm => itm);

            if (carrierAccountMappingsDict != null && carrierAccountMappingsDict.Count > 0)
            {
                foreach (var carrierAccountsMappingKVP in carrierAccountMappingsDict)
                {
                    int customerId = carrierAccountsMappingKVP.Key;
                    var carrierAccountMapping = carrierAccountsMappingKVP.Value;

                    var finalCustomerData = finalCustomerDataByAccountId.GetOrCreateItem(customerId);

                    if (oldCarrierAccountMappingsDict != null && oldCarrierAccountMappingsDict.Count >= 0)
                    {
                        var oldCarrierAccountMapping = oldCarrierAccountMappingsDict.GetRecord(customerId);
                        if (oldCarrierAccountMapping != null)
                        {
                            routeTableNamesForIndexes.Add(carrierAccountMapping.RouteTableName);
                            finalCustomerData.CarrierAccountMappingToUpdate = carrierAccountMapping;
                        }
                        else
                        {
                            routeTableNamesForIndexes.Add(carrierAccountMapping.RouteTableName);
                            finalCustomerData.CarrierAccountMappingToAdd = carrierAccountMapping;
                        }
                    }
                    else
                    {
                        routeTableNamesForIndexes.Add(carrierAccountMapping.RouteTableName);
                        finalCustomerData.CarrierAccountMappingToAdd = carrierAccountMapping;
                    }
                }
            }
            else
            {
                if (oldCarrierAccountMappingsDict != null && oldCarrierAccountMappingsDict.Count >= 0)
                {
                    foreach (var oldCarrierAccountMappingKVP in oldCarrierAccountMappingsDict)
                    {
                        int oldCustomerId = oldCarrierAccountMappingKVP.Key;
                        var oldCarrierAccountMapping = oldCarrierAccountMappingKVP.Value;

                        var finalCustomerData = finalCustomerDataByAccountId.GetOrCreateItem(oldCustomerId);
                        finalCustomerData.CarrierAccountMappingToUpdate = new CarrierAccountMapping()
                        {
                            ZoneID = oldCarrierAccountMapping.ZoneID,
                            CarrierId = oldCarrierAccountMapping.CarrierId,
                            RouteTableName = null,
                            Version = oldCarrierAccountMapping.Version
                        };
                    }
                }
            }

            DataManager.Finalize(new CataleyaFinalizeContext() { FinalCustomerDataByAccountId = finalCustomerDataByAccountId, RouteTableNamesForIndexes = routeTableNamesForIndexes });
        }

        private List<CustomerIdentification> GetUpdatedItems(List<CustomerIdentification> firstListItems, List<CustomerIdentification> secondListItems)
        {
            List<CustomerIdentification> result = new List<CustomerIdentification>();
            foreach (CustomerIdentification firstItem in firstListItems)
            {
                bool isMatching = false;
                foreach (CustomerIdentification secondItem in secondListItems)
                {
                    if (string.Compare(firstItem.Trunk, secondItem.Trunk, true) == 0)
                    {
                        isMatching = true;
                        break;
                    }
                }
                if (!isMatching)
                    result.Add(firstItem);
            }

            return result;
        }

        private Dictionary<int, List<CustomerIdentification>> ConvertToDictionary(List<CustomerIdentification> customerIdentifications)
        {
            if (customerIdentifications == null || customerIdentifications.Count == 0)
                return null;

            Dictionary<int, List<CustomerIdentification>> result = new Dictionary<int, List<CustomerIdentification>>();
            foreach (CustomerIdentification customerIdentification in customerIdentifications)
            {
                List<CustomerIdentification> tempList = result.GetOrCreateItem(customerIdentification.CarrierId);
                tempList.Add(customerIdentification);
            }

            return result;
        }

        public override void RemoveConnection(ISwitchRouteSynchronizerRemoveConnectionContext context)
        {
            DataManager = null;
        }

        public override bool TryBlockCustomer(ITryBlockCustomerContext context)
        {
            return ChangeAccountStatus(context.CustomerId, "Locked");
        }

        public override bool TryUnBlockCustomer(ITryUnBlockCustomerContext context)
        {
            return ChangeAccountStatus(context.CustomerId, "Unlocked");
        }

        public override bool TryBlockSupplier(ITryBlockSupplierContext context)
        {
            return ChangeAccountStatus(context.SupplierId, "Locked");
        }

        public override bool TryUnBlockSupplier(ITryUnBlockSupplierContext context)
        {
            return ChangeAccountStatus(context.SupplierId, "Unlocked");
        }

        public override bool TryDeactivate(ITryDeactivateContext context)
        {
            return ChangeAccountStatus(context.CarrierAccountId, "Locked");
        }

        public override bool TryReactivate(ITryReactivateContext context)
        {
            return ChangeAccountStatus(context.CarrierAccountId, "Unlocked");
        }

        public override bool IsSwitchRouteSynchronizerValid(IIsSwitchRouteSynchronizerValidContext context)
        {
            HashSet<int> allZoneIDs = new HashSet<int>();
            HashSet<int> duplicatedZoneIDs = new HashSet<int>();
            HashSet<string> allInTrunks = new HashSet<string>();
            HashSet<string> duplicatedInTrunks = new HashSet<string>();
            HashSet<string> allOutTrunks = new HashSet<string>();
            HashSet<string> duplicatedOutTrunks = new HashSet<string>();
            HashSet<string> invalidInTrunkNames = new HashSet<string>();
            HashSet<string> invalidOutTrunkNames = new HashSet<string>();
            char[] invalidCharacters = new char[] { trunkPercentageSeparatorAsChar, trunkBackupsSeparatorAsChar, backupsSeparatorAsChar, optionsSeparatorAsChar };

            if (CarrierMappings == null || CarrierMappings.Count == 0)
                return true;

            foreach (var kvp in CarrierMappings)
            {
                CarrierMapping carrierMapping = kvp.Value;
                if (carrierMapping == null)
                    continue;

                int zoneID = carrierMapping.ZoneID;
                if (allZoneIDs.Contains(zoneID))
                    duplicatedZoneIDs.Add(zoneID);

                allZoneIDs.Add(zoneID);

                CustomerMapping customerMapping = carrierMapping.CustomerMappings;
                if (customerMapping != null && customerMapping.InTrunks != null && customerMapping.InTrunks.Count > 0)
                {
                    foreach (var inTrunk in customerMapping.InTrunks)
                    {
                        if (inTrunk.Trunk.IndexOfAny(invalidCharacters) != -1)
                            invalidInTrunkNames.Add(inTrunk.Trunk);

                        if (allInTrunks.Contains(inTrunk.Trunk))
                            duplicatedInTrunks.Add(inTrunk.Trunk);

                        allInTrunks.Add(inTrunk.Trunk);
                    }
                }

                SupplierMapping supplierMapping = carrierMapping.SupplierMappings;
                if (supplierMapping != null && supplierMapping.OutTrunks != null && supplierMapping.OutTrunks.Count > 0)
                {
                    foreach (var outTrunk in supplierMapping.OutTrunks)
                    {
                        if (outTrunk.Trunk.IndexOfAny(invalidCharacters) != -1)
                            invalidOutTrunkNames.Add(outTrunk.Trunk);

                        if (allOutTrunks.Contains(outTrunk.Trunk))
                            duplicatedOutTrunks.Add(outTrunk.Trunk);

                        allOutTrunks.Add(outTrunk.Trunk);
                    }
                }
            }

            List<string> validationMessages = new List<string>();

            if (duplicatedZoneIDs.Count > 0)
                validationMessages.Add($"Following ZoneIDs are Duplicated: {string.Join(", ", duplicatedZoneIDs)}");

            if (duplicatedInTrunks.Count > 0)
                validationMessages.Add($"Following In Trunks are Duplicated: {string.Join(", ", duplicatedInTrunks)}");

            if (duplicatedOutTrunks.Count > 0)
                validationMessages.Add($"Following Out Trunks are Duplicated: {string.Join(", ", duplicatedOutTrunks)}");

            if (invalidInTrunkNames.Count > 0)
                validationMessages.Add($"Following In Trunks are Invalid: {string.Join(", ", invalidInTrunkNames)}");

            if (invalidOutTrunkNames.Count > 0)
                validationMessages.Add($"Following Out Trunks are Invalid: {string.Join(", ", invalidOutTrunkNames)}");

            context.ValidationMessages = validationMessages.Count > 0 ? validationMessages : null;
            return validationMessages.Count == 0;
        }

        #endregion

        #region Private Methods

        private string BuildPercentageOptions(List<RouteOption> routeOptions, out bool hasValidPercentageOptions)
        {
            hasValidPercentageOptions = true;

            if (routeOptions == null || routeOptions.Count == 0)
                return blockedTrunk;

            var concatenatedOptionsTrunksWithBackups = new List<string>();
            List<RouteOption> optionsWithoutPercentage = new List<RouteOption>();

            foreach (var routeOption in routeOptions)
            {
                if (routeOption.IsBlocked)
                    continue;

                if (!routeOption.Percentage.HasValue)
                {
                    optionsWithoutPercentage.Add(routeOption);
                    continue;
                }

                if (!CarrierMappings.TryGetValue(routeOption.SupplierId, out var carrierMapping))
                    continue;

                if (carrierMapping.SupplierMappings == null || carrierMapping.SupplierMappings.OutTrunks == null || carrierMapping.SupplierMappings.OutTrunks.Count == 0)
                    continue;

                List<string> trunksWithPercentage = new List<string>();
                List<string> trunksWithoutPercentage = new List<string>();

                List<OutTrunk> outTrunks = carrierMapping.SupplierMappings.OutTrunks;
                foreach (OutTrunk outTrunk in outTrunks)
                {
                    if (outTrunk.Percentage.HasValue)
                        trunksWithPercentage.Add($"{outTrunk.Trunk}{trunkPercentageSeparatorAsString}{outTrunk.Percentage.Value * routeOption.Percentage.Value}");
                    else
                        trunksWithoutPercentage.Add($"{outTrunk.Trunk}");
                }

                if (trunksWithPercentage.Count == 0)//In this case, we deal with the first trunk as its percentage is 100
                {
                    string firstTrunk = trunksWithoutPercentage.First();
                    trunksWithoutPercentage.Remove(firstTrunk);
                    trunksWithPercentage.Add($"{firstTrunk}{trunkPercentageSeparatorAsString}{routeOption.Percentage}");
                }

                if (routeOption.Backups != null && routeOption.Backups.Count > 0)
                {
                    foreach (var backupOption in routeOption.Backups)
                    {
                        if (!CarrierMappings.TryGetValue(backupOption.SupplierId, out var backupCarrierMapping))
                            continue;

                        if (backupCarrierMapping.SupplierMappings == null || backupCarrierMapping.SupplierMappings.OutTrunks == null || backupCarrierMapping.SupplierMappings.OutTrunks.Count == 0)
                            continue;

                        trunksWithoutPercentage.AddRange(backupCarrierMapping.SupplierMappings.OutTrunks.Select(outTrunk => outTrunk.Trunk));
                    }
                }

                var concatenatedOptionBackupsTrunks = "";
                if (trunksWithoutPercentage.Count > 0)
                    concatenatedOptionBackupsTrunks = string.Join(backupsSeparatorAsString, trunksWithoutPercentage);

                foreach (var optionMainTrunk in trunksWithPercentage)
                {
                    concatenatedOptionsTrunksWithBackups.Add($"{optionMainTrunk}{trunkBackupsSeparatorAsString}{concatenatedOptionBackupsTrunks}");
                }
            }

            if (concatenatedOptionsTrunksWithBackups.Count == 0)
            {
                if (optionsWithoutPercentage.Count == 0)
                {
                    return blockedTrunk;
                }
                else
                {
                    hasValidPercentageOptions = false;
                    BuildPriorityOptions(optionsWithoutPercentage);
                }
            };

            return string.Join(optionsSeparatorAsString, concatenatedOptionsTrunksWithBackups);
        }

        private string BuildPriorityOptions(List<RouteOption> routeOptions)
        {
            if (routeOptions == null || routeOptions.Count == 0)
                return blockedTrunk;

            var allOptionsTrunks = new List<string>();
            foreach (var routeOption in routeOptions)
            {
                if (routeOption.IsBlocked)
                    continue;

                if (!CarrierMappings.TryGetValue(routeOption.SupplierId, out var carrierMapping))
                    continue;

                if (carrierMapping.SupplierMappings == null || carrierMapping.SupplierMappings.OutTrunks == null || carrierMapping.SupplierMappings.OutTrunks.Count == 0)
                    continue;

                foreach (var trunk in carrierMapping.SupplierMappings.OutTrunks)
                {
                    allOptionsTrunks.Add(trunk.Trunk);
                }
            }

            if (allOptionsTrunks.Count == 0)
                return blockedTrunk;

            return string.Join(optionsSeparatorAsString, allOptionsTrunks);
        }

        private bool ChangeAccountStatus(string carrierAccountId, string status)
        {
            var vrConnection = new VRConnectionManager().GetVRConnection(APIConnectionId);
            VRHttpConnection httpConnection = vrConnection.Settings.CastWithValidate<VRHttpConnection>("connection.Settings", APIConnectionId);

            var apiInterceptor = httpConnection.Interceptor.CastWithValidate<VRAPIConnectionHttpConnectionCallInterceptor>(" httpConnection.Interceptor", APIConnectionId);

            if (!this.CarrierMappings.TryGetValue(carrierAccountId, out var carrierMapping))
                return false;

            return ChangeAccountStatus(NodeID, carrierMapping.ZoneID, status, apiInterceptor.Username, apiInterceptor.Password, httpConnection.BaseURL);
        }

        #endregion

        #region API Call

        private bool ChangeAccountStatus(int nodeID, int zoneID, string status, string username, string password, string baseURL)
        {
            var authURI = $"{baseURL}/cataleya/j_spring_security";
            var fullURI = $"{baseURL}/cataleya/j_spring_security_check?j_username={username}&j_password={password}";
            var requestURI = $"{baseURL}/cataleya/configuration/zone/{nodeID}/{zoneID}";

            var zone = APIRequest<ZoneData>("GET", requestURI, authURI, fullURI);

            var version = zone.data.ver;

            var lockingURI = $"{baseURL}/cataleya/configuration/zone/lock_unlock?id={zoneID}&nodeId={nodeID}&adminState={status}&ver={version}";
            var postResponse = APIRequest<PostCallResponse>("POST", lockingURI, authURI, fullURI);

            return postResponse.Success;
        }

        private T APIRequest<T>(string callType, string requestURI, string authURI, string fullURI)
        {
            var cookie = GetCookieAsString(authURI, fullURI);
            var request = HttpWebRequest.Create(requestURI) as HttpWebRequest;
            request.Method = callType;
            request.ContentType = "application/json";
            request.Headers.Add("Cookie", cookie);

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var a = response;
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    String responseString = reader.ReadToEnd();
                    return responseString != null ? Vanrise.Common.Serializer.Deserialize<T>(responseString) : default(T); ;
                }
            }
        }

        private string GetCookieAsString(string authURI, string fullURI)
        {
            var authRequest = HttpWebRequest.Create(fullURI) as HttpWebRequest;
            authRequest.Method = "POST";
            authRequest.ContentType = "application/json";
            CookieContainer authCookie = new CookieContainer();
            authRequest.CookieContainer = authCookie;

            using (var response = (HttpWebResponse)authRequest.GetResponse())
            {
                return GetCookiesAsString(authURI, authCookie);
            }
        }

        private string GetCookiesAsString(string fullAuthenticationServiceURI, CookieContainer cookiesContainer)
        {
            var cookies = cookiesContainer.GetCookies(new Uri(fullAuthenticationServiceURI));

            List<string> cookiesAsString = new List<string>();

            foreach (Cookie cookie in cookies)
            {
                var cookieAsString = string.Format("{0}={1}", cookie.Name, cookie.Value);
                cookiesAsString.Add(cookieAsString);
            }

            return string.Join("; ", cookiesAsString);
        }
        #endregion
    }
}