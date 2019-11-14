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
        const string paymentStep = "A1AC897D-D35D-4B2B-875A-FE80D340ACD6";
        const string billOnDemandStep = "7341F829-E511-4838-9324-7658122FD302";

        const string attachmentsStep = "D8285B77-3B08-47BC-8729-E52A86991C9E";

        const string technicalStep = "45363147-CC59-4632-B09E-EB850D2FD25F";
        const string submitToOM = "42A25FDE-19EB-407A-8640-1831B5FCC470";
        public string GetNextStep(string id, string currentStepId, bool isAdvantageous,string customerId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = printStep; break;
                case printStep: nextStepId = reasonStep; break;
                case reasonStep: nextStepId = new ContractManager().GetTelephonyContracts(customerId).Count>1 ? isAdvantageous? attachmentsStep : paymentStep : billOnDemandStep; break;
                case paymentStep: nextStepId = attachmentsStep; break;
                case billOnDemandStep: nextStepId = isAdvantageous ? attachmentsStep : paymentStep; break;
                case attachmentsStep: nextStepId = submitToOM; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }

        //public string NBOfActiveContracts(string id)
        //{

        //    //TODO : Get the count of active contracts for this customer (customerId)

        //    Random gen = new Random();
        //    int prob = gen.Next(10);
        //    return prob <= 6 ? "1" : "4";
        //}

    }
}
