using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class LineTerminationRequestFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "0BAC1212-9D90-4CAF-95C4-BE6CAFF6C0B1";
        const string printStep = "E98EA057-8D8F-4BD1-B14F-9DAECBAE9FB4";
        const string reasonStep = "309A5A35-395E-43FB-983D-22BAE925DAFC";
        const string generateInvoiceStep = "7341F829-E511-4838-9324-7658122FD302";
        const string validatePaymentStep = "A56EE0A5-A9D0-404A-B264-2517D0F3E5F7";
       // const string mdfTeamStep = "205FE9FF-FD14-4DB6-95A8-DA7BBE8D5D81";
       // const string cabinetTeamStep = "17A7EEF5-A226-4E08-8165-6986C755DEE1";
       // const string dpTeamStep = "8E6A058E-C805-4B77-AA95-D2F503E61BC7";
        const string technicalStep = "45363147-CC59-4632-B09E-EB850D2FD25F";
        const string completedStep = "CEBCB883-84AA-4183-9938-817E711EB2BF";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = printStep; break;
                case printStep: nextStepId = reasonStep; break;
                case reasonStep: nextStepId = generateInvoiceStep; break;
                case generateInvoiceStep: nextStepId = validatePaymentStep; break;
                case validatePaymentStep: nextStepId = technicalStep; break;
                case technicalStep: nextStepId = completedStep; break;
            }
            return nextStepId;
        }

        public bool ManualSwitch(string id)
        {
            bool ismanualswitch = false;
            if (id != "")
            {
                //get request from bpm
                Guid idd = new Guid(id.ToUpper());
                // Creation of query instance with "City" root schema. 
                var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StManagePabx");
                esq.AddColumn("Id");
                esq.AddColumn("StContractId");
                // Creation of the first filter instance.
                var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);
                // Adding created filters to query collection. 
                esq.Filters.Add(esqFirstFilter);
                // Objects, i.e. query results, filtered by two filters, will be included into this collection.
                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var contractId = entities[0].GetColumnValue("StContractId");
                    InventoryManager manager = new InventoryManager();
                    ismanualswitch = manager.IsManualSwitch(contractId.ToString());
                }
            }
            return ismanualswitch;
        }
    }
}
