using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrasoft.Core;
using Terrasoft.Core.Configuration;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;
using System.Web;
using BPMExtended.Main.SOMAPI;

namespace BPMExtended.Main.Business
{
    
    public class ADSLLineMovingFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "013B161F-FB2C-4072-82CC-60852FE53039";
        const string chooseContractStep = "BE540720-169E-456B-B0D0-6D243D5B7F82";
        const string printStep = "4F032C27-BBB9-420B-A064-6A88E7E532A0";
        const string DSLAMPortStep = "0E26138A-89CA-4A06-9370-D4EB9060B639";
        const string paymentValidationStep = "20CB8C91-E898-4935-9193-B0321289573C";
        const string waitingListStep = "979ED714-3DEE-4376-AE01-9C4F78702AE9";
        const string paymentStep = "ED21BA09-A2BB-4E4F-920A-26912132265A";

        const string attachmentsStep = "A72E1158-13C5-4A02-9589-E68D5D4A5E02";
        const string technicalStep = "9A9C358C-8A33-4580-A5BE-36126E805E3E";
        const string completedStep = "F3384A9D-A1D3-4F2C-8985-8987639522DE";

        public string GetNextStep(string id, string currentStepId)
        {
            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = chooseContractStep; break;
                case chooseContractStep: nextStepId = printStep; break;
                case printStep: nextStepId = SameSwitch(id) ? paymentStep :  DSLAMPortStep; break;
                case DSLAMPortStep: nextStepId = FreeDSLAMPorts(id) != null ? paymentStep : paymentValidationStep; break;
                case paymentValidationStep: nextStepId = waitingListStep; break;
                case waitingListStep: nextStepId = DSLAMPortStep; break;
                case paymentStep: nextStepId = attachmentsStep; break;
                case attachmentsStep: nextStepId = technicalStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }

        public List<DSLAMPortInfo> FreeDSLAMPorts(string id)
        {
            List<DSLAMPortInfo> ports = null;

            if (id != "")
            {
                Guid idd = new Guid(id.ToUpper());
                var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLLineMoving");
                esq.AddColumn("Id");
                esq.AddColumn("StPhoneNumber");
                // Creation of the first filter instance.
                var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);
                // Adding created filters to query collection. 
                esq.Filters.Add(esqFirstFilter);
                // Objects, i.e. query results, filtered by two filters, will be included into this collection.
                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var phoneNumber = entities[0].GetColumnValue("StPhoneNumber");
                    ports = new DslamManager().GetFreeDSLAMPorts(phoneNumber.ToString());
                }
            }

            return ports;

        }

        public bool SameSwitch(string id)
        {
            bool isonsameswitch = false;
            if (id != "")
            {
                //get request from bpm
                Guid idd = new Guid(id.ToUpper());
                // Creation of query instance with "City" root schema. 
                var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSLLineMoving");
                esq.AddColumn("Id");
                esq.AddColumn("StSelectedTelephonyContract");
                esq.AddColumn("StTelephonyContractId");
                // Creation of the first filter instance.
                var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);
                // Adding created filters to query collection. 
                esq.Filters.Add(esqFirstFilter);
                // Objects, i.e. query results, filtered by two filters, will be included into this collection.
                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var selectedcontract = entities[0].GetColumnValue("StSelectedTelephonyContract");
                    var pilotcontract = entities[0].GetColumnValue("StTelephonyContractId");
                    InventoryManager manager = new InventoryManager();
                    isonsameswitch = manager.IsNumbersOnSameSwitch(selectedcontract.ToString(), pilotcontract.ToString());
                }
            }
            return isonsameswitch;
        }
    }
}
