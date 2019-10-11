using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Ericsson.Business
{
    public class EricssonManager
    {
        public bool IsSupplierTrunkUsed(int supplierId, Guid trunkId, string trunkName, Dictionary<int, CarrierMapping> carrierMappings, out List<UpdateAdditionalMessage> errorMessages)
        {
            bool usedByTrunkGroup = false;
            HashSet<int> supplierIds = new HashSet<int>();
            errorMessages = new List<UpdateAdditionalMessage>();

            foreach (var kvp in carrierMappings)
            {
                int carrierAccountId = kvp.Key;
                CarrierMapping carrierMapping = kvp.Value;
                SupplierMapping supplierMapping = carrierMapping.SupplierMapping;

                if (supplierMapping == null || supplierMapping.TrunkGroups == null)
                    continue;

                foreach (var trunkGroup in supplierMapping.TrunkGroups)
                {
                    if (trunkGroup.TrunkTrunkGroups == null)
                        continue;

                    foreach (var trunkTrunkGroup in trunkGroup.TrunkTrunkGroups)
                    {
                        if (carrierAccountId == supplierId && trunkId.Equals(trunkTrunkGroup.TrunkId))
                            usedByTrunkGroup = true;

                        if (trunkTrunkGroup.Backups == null)
                            continue;

                        foreach (var backup in trunkTrunkGroup.Backups)
                        {
                            if (backup.Trunks == null)
                                continue;

                            foreach (var backupTrunks in backup.Trunks)
                            {
                                if (backupTrunks != null && trunkId.Equals(backupTrunks.TrunkId))
                                    supplierIds.Add(carrierAccountId);
                            }
                        }
                    }
                }
            }

            if (supplierIds.Count > 0)
            {
                List<string> supplierNames = new List<string>();
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

                foreach (var currentSupplierId in supplierIds) 
                    supplierNames.Add(carrierAccountManager.GetCarrierAccountName(currentSupplierId));

                errorMessages.Add(new UpdateAdditionalMessage()
                {
                    Result = UpdateOperationResult.Failed,
                    Message = $"Trunk '{trunkName}' is used as backup by the following suppliers: {string.Join(", ", supplierNames)}"
                });
            }

            if (usedByTrunkGroup)
            {
                errorMessages.Add(new UpdateAdditionalMessage()
                {
                    Result = UpdateOperationResult.Failed,
                    Message = $"Trunk '{trunkName}' is used by current supplier trunk groups"
                });
            }

            return errorMessages.Count == 0;
        }
    }
}