using System;
using System.Collections.Generic;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ADSLForISPManager
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

        public bool ReservePortOnOtherISP(string port, string phoneNumber)
        {
            bool result;
            using (SOMClient client = new SOMClient())
            {
                result = client.Get<bool>(String.Format("api/SOM.ST/Inventory/ReserveDSLAMPort?phoneNumber={0}&dslamPort={1}", phoneNumber, port));
            }

            return result;
        }

        public List<DSLAMPortInfo> GetISPDSLAMPorts(string switchId, string ISP)
        {
            List<DSLAMPortInfo> items;
            using (SOMClient client = new SOMClient())
            {
                items = client.Get<List<DSLAMPortInfo>>(String.Format("api/SOM.ST/Inventory/GetISPDSLAMPorts?switchId={0}&ISP={1}", switchId, ISP));
            }

            return items;
        }

        public List<ISPInfo> GetISPs()
        {
            List<ISPInfo> items = new List<ISPInfo>();
            using (SOMClient client = new SOMClient())
            {
                items = client.Get<List<ISPInfo>>(String.Format("api/SOM.ST/Inventory/GetISPs"));
            }

            return items;

        }

        public void PostADSLForISPToOM(Guid requestId)
        {
            //Get Data from StLineSubscriptionRequest table
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            SOMRequestOutput output;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StISPADSL");
            esq.AddColumn("StContractId");
            esq.AddColumn("StISPName");
            esq.AddColumn("StISPId");
            esq.AddColumn("StPort");
            esq.AddColumn("StLinePathId");



            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var contractId = entities[0].GetColumnValue("StContractId");
                var ipsName = entities[0].GetColumnValue("StISPName");
                var ispId= entities[0].GetColumnValue("StISPId");
                var port = entities[0].GetColumnValue("StPort");
                var pathId = entities[0].GetColumnValue("StLinePathId");

                SOMRequestInput<ADSLForISPRequestInput> somRequestInput = new SOMRequestInput<ADSLForISPRequestInput>
                {

                    InputArguments = new ADSLForISPRequestInput
                    {
                        CommonInputArgument = new CommonInputArgument()
                        {
                            ContractId = contractId.ToString(),
                            RequestId = requestId.ToString(),
                        },
                        LinePathId = pathId.ToString()
                    }

                };

                //call api
                using (var client = new SOMClient())
                {
                    output = client.Post<SOMRequestInput<ADSLForISPRequestInput>, SOMRequestOutput>("api/DynamicBusinessProcess_BP/ST_Tel_ActivateADSLForISP/StartProcess", somRequestInput);
                }

            }

        }
        #endregion
    }
}
