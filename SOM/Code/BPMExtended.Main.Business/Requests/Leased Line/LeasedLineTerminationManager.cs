using System;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class LeasedLineTerminationManager
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

        public string GetConnectionType(Guid requestId)
        {
            Random random = new Random();
            return random.Next(10) > 5 ? "Fiber" : "Copper";
        }

        public void PostLeasedLineTerminationToOM(Guid requestId)
        {
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLeasedLineTermination");
            esq.AddColumn("StContractId");
            esq.AddColumn("StLinePathId");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractId");
                var pathId = entities[0].GetColumnValue("StLinePathId");

                SOMRequestInput<LeasedLineTerminationRequestInput> somRequestInput = new SOMRequestInput<LeasedLineTerminationRequestInput>
                {

                    InputArguments = new LeasedLineTerminationRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                        },
                         LinePathId= pathId.ToString()
                    }

                };


                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<LeasedLineTerminationRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_LL_TerminateContract/StartProcess", somRequestInput);
                }
                var manager = new BusinessEntityManager();
                manager.InsertSOMRequestToProcessInstancesLogs(requestId, output);

            }

        }
        #endregion
    }
}
