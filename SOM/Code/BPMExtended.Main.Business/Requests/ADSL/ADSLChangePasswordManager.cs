using System;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ADSLChangePasswordManager
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
        public void PostADSLChangePasswordToOM(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLChangePassword");
            esq.AddColumn("StContractId");
            esq.AddColumn("StCustomerId");


            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractId");              
                var customerId = entities[0].GetColumnValue("StCustomerId");

                SOMRequestInput<ADSLChangePasswordRequestInput> somRequestInput = new SOMRequestInput<ADSLChangePasswordRequestInput>
                {

                    InputArguments = new ADSLChangePasswordRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            //ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                            //CustomerId = customerId.ToString()
                        }
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ADSLChangePasswordRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_ADSL_ResetPassword/StartProcess", somRequestInput);
                }

            }
        }

        #endregion
    }
}
