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
        // string mDFStep = "6935960E-B310-49FF-835C-3333E2EEB73B";
        // string destinationMDFStep = "ADB8BBEC-C23A-430E-BF3A-221444F0E589";
        string technicalStep = "9A9C358C-8A33-4580-A5BE-36126E805E3E";
        string completedStep = "F3384A9D-A1D3-4F2C-8985-8987639522DE";

        public string GetNextStep(string id, string currentStepId)
        {
            string nextStepId = "";
            switch (currentStepId.ToLower())
            {
                ///welcome step
                case "013b161f-fb2c-4072-82cc-60852fe53039": nextStepId = chooseContractStep; break;
                ///choose contract step
                case "be540720-169e-456b-b0d0-6d243d5b7f82": nextStepId = printStep; break;
                ///print step
                case "4f032c27-bbb9-420b-a064-6a88e7e532a0": nextStepId = SameSwitch(id) ? paymentStep :  DSLAMPortStep; break;
                /// DSLAM Step
                case "0e26138a-89ca-4a06-9370-d4eb9060b639": nextStepId = paymentStep; break;
                 /*payment step*/
               // case "ed21ba09-a2bb-4e4f-920a-26912132265a": nextStepId = SameSwitch(id) ? mDFStep : destinationMDFStep; break;
                /// MDF Step
               // case "6935960e-b310-49ff-835c-3333e2eeb73b": nextStepId = destinationMDFStep; break;
                /// Destination MDF Step
               // case "adb8bbec-c23a-430e-bf3a-221444f0e589": nextStepId = completedStep; break;
                    
                case "ed21ba09-a2bb-4e4f-920a-26912132265a": nextStepId = technicalStep; break;
               // case "9A9C358C-8A33-4580-A5BE-36126E805E3E": nextStepId = completedStep; break;
            }
            return nextStepId;
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
