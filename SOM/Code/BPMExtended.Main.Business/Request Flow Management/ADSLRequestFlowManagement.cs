using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Entities;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ADSLRequestFlowManagement
    {

        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "086480CE-D251-49B1-B9EC-6BE57A6D9772";
        const string chooseTelephonyContractStep = "B0C44AC6-1E43-4209-8A4B-BCF4EB3A0E65";
        const string printStep = "8D6F4638-31FD-46DD-96AA-478C1F4495ED";
        const string DSLAMStep = "86FC7101-B189-46AD-9CDD-65B35DF892E0";

        const string waitingListStep = "3F63A582-3E54-4469-83A1-5737535DCADE";
        const string servicesStep = "5961214F-D40A-4152-837A-9A85ED34AC70";
        const string paymentStep = "F04855B9-A46F-416F-84F3-7A51B621747C";
        const string adslCredentialsStep = "B1110D25-96F3-4FED-BC62-8C30623D929E";

        const string printConfigurationStep = "AA09C2F9-C844-4B13-BEFA-D9D4E00571FC";
        const string TechnicalStep = "6AB2B0E3-5201-4189-9838-CF4B4E4E17D9";

        const string completedStep = "8CD5BDC5-2551-4767-8166-5D334D5E0FD7";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = chooseTelephonyContractStep; break;
                case chooseTelephonyContractStep: nextStepId = printStep; break;
                case printStep: nextStepId = DSLAMStep; break;
                case DSLAMStep: nextStepId = FreeDSLAMPorts(id) !=null  ? servicesStep : waitingListStep; break;
                case waitingListStep: nextStepId = DSLAMStep; break;
                case servicesStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = adslCredentialsStep; break;
                case adslCredentialsStep: nextStepId = printConfigurationStep; break;
                case printConfigurationStep: nextStepId = TechnicalStep; break;
                case TechnicalStep: nextStepId = completedStep; break;
              
            }
            return nextStepId;
        }

        public List<DSLAMPortInfo> FreeDSLAMPorts(string id)
        {
            List<DSLAMPortInfo> ports = null;

            if (id != "")
            {
                Guid idd = new Guid(id.ToUpper());
                var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StADSL");
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

    }
}
