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
    public class ADSLAlterSpeedFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "1023C475-90F9-4329-B676-2C67DA8703AF";
        const string adslSpeedStep = "5A4BB05E-6181-4293-952B-7B44F660AA1D";
        const string printStep = "9218440D-EAC0-426B-A7D8-8781EE259261";
        const string pdnTeamStep = "A3415BC5-32DB-4083-935A-7CC7F30D4FD6";
        const string completedStep = "33AF8121-E09C-4F57-AFA9-1FD79939580E";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = adslSpeedStep; break;
                case adslSpeedStep: nextStepId = printStep; break;
                case printStep: nextStepId = ManualSwitch(id) ? pdnTeamStep : completedStep; break;
                case pdnTeamStep: nextStepId = completedStep; break;
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
