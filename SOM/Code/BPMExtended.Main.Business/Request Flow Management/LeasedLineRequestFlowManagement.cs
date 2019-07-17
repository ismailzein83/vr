using System;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class LeasedLineRequestFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string startingProcess = "59C93AF1-7B3E-4DBC-AB1A-770098F8204B";
        const string requestID = "3B9222AF-73F8-4F4F-BC3C-0BB0697DC795";
        const string address = "5237ECDF-8455-4720-A2CF-8CE3AC415921";
        const string technicalTeam = "53BD9DCE-7695-4007-97F0-0466CEAE356F";
        const string services = "AD7EB0E2-2B8D-4F3B-86E8-26FFEAF89409";
        const string createContractOnHolld = "CEA9B898-91CF-4CF4-BA11-78A9FA2A8F28";

        const string discount = "BF730CB5-396F-4720-A663-E16A8A09C7FB";
        const string paymentMethod = "16A296DF-6695-4FD7-AA11-4381BC4A9874";
        const string print = "E7C4431A-615D-44A6-8811-8DD731A378DB"; // to technical
        const string attachmentStep = "013C02E5-203A-424C-8F7A-F10A82B1466E";
        const string technicalStep = "0545B38D-F1BA-4CD6-BC96-1BEFD85C9B3A";

        public string GetNextStep(string id, string currentStepId)
        {
            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = requestID; break;
                case requestID: nextStepId = address; break;
                case address: nextStepId = technicalStep; break;
                case services: nextStepId = createContractOnHolld; break;
                case createContractOnHolld: nextStepId = paymentMethod; break;
                case paymentMethod: nextStepId = attachmentStep; break;
                case attachmentStep: nextStepId = technicalStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }

  /*      public bool IsFiberFounded(string id)
        {
            bool isFiberFounded = false;
            if (id != "")
            {
                Guid idd = new Guid(id.ToUpper());
                var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLeasedLine");

                esq.AddColumn("Id");
                esq.AddColumn("StIsFiberServiceSelected");

                var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);

                esq.Filters.Add(esqFirstFilter);

                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var fiber = entities[0].GetColumnValue("StIsFiberServiceSelected");
                    isFiberFounded = (bool)fiber==true ? true : false;
                }
            }
            return isFiberFounded;
        }

        public bool IsMicrowaveService(string id)
        {
            bool isMicrowaveService = false;
            if (id != "")
            {
                Guid idd = new Guid(id.ToUpper());
                var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLeasedLine");

                esq.AddColumn("Id");
                esq.AddColumn("StIsMicrowaveServiceSelected");

                var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);

                esq.Filters.Add(esqFirstFilter);

                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var microwaveService = entities[0].GetColumnValue("StIsMicrowaveServiceSelected");
                    isMicrowaveService = (bool)microwaveService== true ? true : false;
                }
            }
            return isMicrowaveService;
        }*/


    }
}
