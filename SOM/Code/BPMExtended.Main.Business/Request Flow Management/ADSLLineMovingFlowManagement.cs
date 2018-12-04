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
        string welcomeStep = "013B161F-FB2C-4072-82CC-60852FE53039";
        string chooseContractStep = "BE540720-169E-456B-B0D0-6D243D5B7F82";
        string printStep = "4F032C27-BBB9-420B-A064-6A88E7E532A0";
        string DSLAMPortStep = "0E26138A-89CA-4A06-9370-D4EB9060B639";
        string paymentStep = "ED21BA09-A2BB-4E4F-920A-26912132265A";
        string mDFStep = "6935960E-B310-49FF-835C-3333E2EEB73B";
        string destinationMDFStep = "ADB8BBEC-C23A-430E-BF3A-221444F0E589";
        string completedStep = "F3384A9D-A1D3-4F2C-8985-8987639522DE";

        public string GetNextStep(string id, string currentStepId)
        {
            string nextStepId = "";
            bool isonsameswitch = false;
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

            switch (currentStepId.ToLower())
            {
                ///welcome step
                case "013B161F-FB2C-4072-82CC-60852FE53039": nextStepId = chooseContractStep; break;
                ///choose contract step
                case "BE540720-169E-456B-B0D0-6D243D5B7F82": nextStepId = printStep; break;
                ///print step
                case "4F032C27-BBB9-420B-A064-6A88E7E532A0":
                    
                    nextStepId = isonsameswitch ? paymentStep :  DSLAMPortStep; break;
                /// DSLAM Step
                case "0E26138A-89CA-4A06-9370-D4EB9060B639": nextStepId = paymentStep; break;
                /*payment step*/
                case "ED21BA09-A2BB-4E4F-920A-26912132265A": nextStepId = isonsameswitch? mDFStep : destinationMDFStep ; break;
                /// MDF Step
                case "6935960E-B310-49FF-835C-3333E2EEB73B": nextStepId = destinationMDFStep; break;
                /// Destination MDF Step
                case "ADB8BBEC-C23A-430E-BF3A-221444F0E589": nextStepId = completedStep; break;
            }
            return nextStepId;
        }
    }
}
