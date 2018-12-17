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
    public class ManagePabxFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "15EA6BF0-DFFF-4BBB-BF12-2A5299BCF592";
        const string manageContractsStep = "F7D3AA83-6BAB-4A3C-8FF2-EFCBBA43ED3F";
        const string printStep = "6E8BC6D9-91C8-4B7C-9FBE-51E2565E3852";
        const string switchTeamStep = "9ADF25A4-C8CC-4383-8D82-0E37AAF8E895";
        const string completedStep = "4467A681-9D21-4577-8079-DA49D1B61616";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = manageContractsStep; break;
                case manageContractsStep: nextStepId = printStep; break;
                case printStep: nextStepId = ManualSwitch(id) ? switchTeamStep : completedStep; break;
                case switchTeamStep: nextStepId = completedStep; break;
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
