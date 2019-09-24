using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Entities;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000
{
    public class HuaweiSoftX3000SWSync : SwitchRouteSynchronizer
    {
        public override Guid ConfigId { get { return new Guid("2574530B-B9FD-44E2-BD0C-D1F46DB80E68"); } }

        public override bool SupportSyncWithinRouteBuild { get { return false; } }

        public int NumberOfOptions { get; set; }

        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

        public List<Huawei.Entities.HuaweiSSHCommunication> SwitchCommunicationList { get; set; }

        public List<Huawei.Entities.SwitchLogger> SwitchLoggerList { get; set; }

        private Dictionary<int, CustomerMapping> _customerMappingsByRSSC;
        private Dictionary<int, CustomerMapping> CustomerMappingsByRSSC
        {
            get
            {
                if (_customerMappingsByRSSC != null)
                    return _customerMappingsByRSSC;

                if (CarrierMappings == null || CarrierMappings.Count == 0)
                    return null;

                _customerMappingsByRSSC = new Dictionary<int, CustomerMapping>();

                foreach (var carrierMappingKvp in CarrierMappings)
                {
                    var carrierMapping = carrierMappingKvp.Value;
                    if (carrierMapping.CustomerMapping != null)
                        _customerMappingsByRSSC.Add(carrierMapping.CustomerMapping.RSSC, carrierMapping.CustomerMapping);
                }

                if (_customerMappingsByRSSC.Count == 0)
                    _customerMappingsByRSSC = null;

                return _customerMappingsByRSSC;
            }
        }


        #region Public Methods

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            throw new NotImplementedException();
        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            throw new NotImplementedException();
        }

        public override void onAllRoutesConverted(ISwitchRouteSynchronizerOnAllRoutesConvertedContext context)
        {
            throw new NotImplementedException();
        }

        public override object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
        {
            throw new NotImplementedException();
        }

        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            throw new NotImplementedException();
        }

        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            throw new NotImplementedException();
        }

        public override void RemoveConnection(ISwitchRouteSynchronizerRemoveConnectionContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsSwitchRouteSynchronizerValid(IIsSwitchRouteSynchronizerValidContext context)
        {
            if (this.CarrierMappings == null || this.CarrierMappings.Count == 0)
                return true;

            HashSet<int> allRSSCs = new HashSet<int>();
            HashSet<int> duplicatedRSSCs = new HashSet<int>();

            HashSet<int> allSRTs = new HashSet<int>();
            HashSet<int> duplicatedSRTs = new HashSet<int>();
            
            foreach (var carrierMappingKvp in this.CarrierMappings)
            {
                CarrierMapping carrierMapping = carrierMappingKvp.Value;

                CustomerMapping customerMapping = carrierMapping.CustomerMapping;
                if (customerMapping != null)
                {
                    var isRSSCDuplicated = allRSSCs.Contains(customerMapping.RSSC);
                    if (!isRSSCDuplicated)
                        allRSSCs.Add(customerMapping.RSSC);
                    else
                        duplicatedRSSCs.Add(customerMapping.RSSC);
                }

                SupplierMapping supplierMapping = carrierMapping.SupplierMapping;
                if (supplierMapping != null)
                {
                    var isSRTDuplicated = allSRTs.Contains(supplierMapping.SRT);
                    if (!isSRTDuplicated)
                        allSRTs.Add(supplierMapping.SRT);
                    else
                        duplicatedSRTs.Add(supplierMapping.SRT);
                }
            }

            List<string> validationMessages = new List<string>();

            if (duplicatedRSSCs.Count > 0)
                validationMessages.Add(string.Format("Duplicated RSSCs: {0}", string.Join(", ", duplicatedRSSCs)));

            if (duplicatedSRTs.Count > 0)
                validationMessages.Add(string.Format("Duplicated SRTs: {0}", string.Join(", ", duplicatedSRTs)));

            context.ValidationMessages = validationMessages.Count > 0 ? validationMessages : null;
            return validationMessages.Count == 0;
        }

        #endregion
    }
}