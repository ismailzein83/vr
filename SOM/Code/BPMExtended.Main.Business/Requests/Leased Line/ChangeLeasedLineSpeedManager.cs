using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ChangeLeasedLineSpeedManager
    {
        #region User Connection
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        #endregion

        #region public 
        public void PostChangeLeasedLineSpeedToOM(Guid requestId)
        {

            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StChangeLeasedLineSpeed");
            esq.AddColumn("StContractId");
            esq.AddColumn("StCustomerId");
            esq.AddColumn("StContact");
            esq.AddColumn("StContact.Id");
            esq.AddColumn("StAccount");
            esq.AddColumn("StAccount.Id");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractId");
                var contactId = entities[0].GetColumnValue("StContactId");
                var accountId = entities[0].GetColumnValue("StAccountId");
                var customerId = entities[0].GetColumnValue("StCustomerId");

                SOMRequestInput<ChangeLeasedLineSpeedRequestInput> somRequestInput = new SOMRequestInput<ChangeLeasedLineSpeedRequestInput>
                {

                    InputArguments = new ChangeLeasedLineSpeedRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            //ContractId = contractId.ToString(),
                            //ContactId = contactId.ToString(),
                            //AccountId = null,
                            RequestId = requestId.ToString(),
                            //CustomerId = customerId.ToString()
                        }
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ChangeLeasedLineSpeedRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_LL_AlterSpeed/StartProcess", somRequestInput);
                }

            }


            //TODO:OM should deactivate the unavailable services in the new rateplan(if any based on the catalog).
            //TODO:OM should add the new core services in the new ratplan, if they were not activated before(this case is not exist based on the posted catalog).

        }
        #endregion
    }
}
