using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Huawei.Entities;

namespace TOne.WhS.RouteSync.Huawei
{
    public partial class HuaweiSWSync : SwitchRouteSynchronizer
    {
        public int MinRNLength { get; set; }

        public override bool IsSwitchRouteSynchronizerValid(IIsSwitchRouteSynchronizerValidContext context)
        {
            if (this.CarrierMappings == null || this.CarrierMappings.Count == 0)
                return true;

            HashSet<int> allRSSNs = new HashSet<int>();
            HashSet<int> duplicatedRSSNs = new HashSet<int>();

            HashSet<string> allCSCNames = new HashSet<string>();
            HashSet<string> duplicatedCSCNames = new HashSet<string>();

            HashSet<string> allRouteNames = new HashSet<string>();
            HashSet<string> duplicatedRouteNames = new HashSet<string>();
            HashSet<string> shortRouteNames = new HashSet<string>();

            foreach (var carrierMapping in this.CarrierMappings)
            {
                CustomerMapping customerMapping = carrierMapping.Value.CustomerMapping;
                if (customerMapping != null && !string.IsNullOrEmpty(customerMapping.CSCName))
                {
                    var isCSCNameDuplicated = allCSCNames.Any(cscName => string.Compare(cscName, customerMapping.CSCName, true) == 0);
                    if (!isCSCNameDuplicated)
                        allCSCNames.Add(customerMapping.CSCName);
                    else
                        duplicatedCSCNames.Add(customerMapping.CSCName);


                    if (!allRSSNs.Contains(customerMapping.RSSN))
                        allRSSNs.Add(customerMapping.RSSN);
                    else
                        duplicatedRSSNs.Add(customerMapping.RSSN);
                }

                SupplierMapping supplierMapping = carrierMapping.Value.SupplierMapping;
                if (supplierMapping != null && !string.IsNullOrEmpty(supplierMapping.RouteName))
                {
                    string supplierRouteNameWithoutEmptySpaces = supplierMapping.RouteName.Replace(" ", "");

                    if (supplierRouteNameWithoutEmptySpaces.Length >= this.MinRNLength)
                    {
                        string modifiedSupplierRouteName = supplierRouteNameWithoutEmptySpaces.Substring(0, this.MinRNLength);
                        var isRouteNameDuplicated = allRouteNames.Any(routeName => !string.IsNullOrEmpty(routeName) && string.Compare(routeName.Replace(" ", "").Substring(0, this.MinRNLength), modifiedSupplierRouteName, true) == 0);
                        if (!isRouteNameDuplicated)
                            allRouteNames.Add(supplierMapping.RouteName);
                        else
                            duplicatedRouteNames.Add(supplierMapping.RouteName);
                    }
                    else
                    {
                        shortRouteNames.Add(supplierMapping.RouteName);
                    }
                }
            }

            List<string> validationMessages = new List<string>();
            if (duplicatedRSSNs.Count > 0)
                validationMessages.Add(string.Format("Duplicated RSSNs: {0}", string.Join(", ", duplicatedRSSNs)));

            if (duplicatedCSCNames.Count > 0)
                validationMessages.Add(string.Format("Duplicated CSC Names: {0}", string.Join(", ", duplicatedCSCNames)));

            if (duplicatedRouteNames.Count > 0)
                validationMessages.Add(string.Format("Duplicated Route Names: {0}", string.Join(", ", duplicatedRouteNames)));

            if (shortRouteNames.Count > 0)
                validationMessages.Add(string.Format("Invalid Route Names: {0}. Length should be greater than {1}", string.Join(", ", shortRouteNames), this.MinRNLength));

            context.ValidationMessages = validationMessages.Count > 0 ? validationMessages : null;
            return validationMessages.Count == 0;
        }
    }
}