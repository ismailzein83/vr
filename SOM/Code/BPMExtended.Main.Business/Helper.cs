using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using SOM.Main.Entities;

namespace BPMExtended.Main.Business
{
    public class Helper
    {
        public static CreateCustomerRequestOutput CreateSOMRequest(BPMCustomerType customerType, Guid accountOrContactId, string requestTitle, SOMRequestExtendedSettings requestSettings)
        {
            Guid requestId = Guid.NewGuid();
            CreateSOMRequestInput somRequestInput = new CreateSOMRequestInput
            {
                SOMRequestId = requestId,
                EntityId = BuildSOMEntityIdFromCustomer(customerType, accountOrContactId),
                RequestTitle = requestTitle,
                Settings = new SOMRequestSettings { ExtendedSettings = requestSettings }
            };

            CreateSOMRequestOutput output = null;

            using (var client = new SOMClient())
            {
                //s_dataManager.Insert(requestId, requestSettings.ConfigId, customerObjectType, accountOrContactId, requestTitle, CustomerRequestStatus.New);//insert request in BPM after making sure connection to SOM succeeds
                //try
                //{
                output = client.Post<CreateSOMRequestInput, CreateSOMRequestOutput>("api/SOM_Main/SOMRequest/CreateSOMRequest", somRequestInput);
                //}
                //catch
                //{
                //    s_dataManager.UpdateRequestStatus(requestId, CustomerRequestStatus.Aborted);
                //    throw;
                //}
            }
            return new CreateCustomerRequestOutput
            {
            };
        }

        private static string BuildSOMEntityIdFromCustomer(BPMCustomerType customerType, Guid accountOrContactId)
        {
            return String.Concat(customerType.ToString(), "_", accountOrContactId.ToString());
        }
    }
}
