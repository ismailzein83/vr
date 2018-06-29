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
            var vrConnectionId = new Guid("dc097d1d-1195-4641-b364-deeb3a50190c");

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(vrConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            var checkVoucherAvailabilityOutput = connectionSettings.Get<CheckVoucherAvailabilityOutput>(string.Format("/api/VR_Voucher/VoucherCards/CheckVoucherAvailability?pinCode={0}&lockedBy={1}", input.PinCode, input.PhoneNumber));
            if (checkVoucherAvailabilityOutput != null && checkVoucherAvailabilityOutput.IsAvailable)
            {
                /// charge the voucher;
                var setVoucherUsed = connectionSettings.Post<SetVoucherUsedInput, SetVoucherUsedOutput>("/api/VR_Voucher/VoucherCards/SetVoucherUsed",
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
