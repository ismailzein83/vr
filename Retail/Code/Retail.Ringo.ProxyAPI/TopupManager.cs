using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Voucher.Entities;

namespace Retail.Ringo.ProxyAPI
{
    public class TopupManager
    {
        public AddTopupOutput AddTopup(AddTopupInput input)
        {
            var vrConnectionId = new Guid("");

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(vrConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            var checkVoucherAvailabilityOutput = connectionSettings.Get<CheckVoucherAvailabilityOutput>(string.Format("/api/VR_Voucher/VRVoucherCards/CheckVoucherAvailability?pinCode={0}", input.PinCode));
            if (checkVoucherAvailabilityOutput != null && checkVoucherAvailabilityOutput.IsAvailable)
            {
                /// charge the voucher;
                var setVoucherUsed = connectionSettings.Put<SetVoucherUsedInput, SetVoucherUsedOutput>("/api/VR_Voucher/VRVoucherCards/SetVoucherUsed",
                    new SetVoucherUsedInput
                    {
                        PinCode = input.PinCode,
                        UsedBy = input.PhoneNumber
                    });
                
                if (setVoucherUsed != null)
                {
                    if (setVoucherUsed.Result == SetVoucherUsedResult.Succeeded)
                    {
                        return new AddTopupOutput
                        {
                            IsSucceeded = true,
                            FailureReason = FailureReason.InvalidPin
                        };
                    }
                    else
                    {
                        return new AddTopupOutput
                        {
                            IsSucceeded = false,
                        };
                    }
                }
            }
            
            return new AddTopupOutput
            {
                IsSucceeded = false,
                FailureReason = FailureReason.InvalidPin
            };
        }
    }
}
