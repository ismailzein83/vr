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
        const string completedStep = "CEBCB883-84AA-4183-9938-817E711EB2BF";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = printStep; break;
                case printStep: nextStepId = reasonStep; break;
                case reasonStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = NBOfActiveContracts(id) == "1" ? billOnDemandStep : attachmentsStep; break;
                case billOnDemandStep: nextStepId = attachmentsStep; break;
                case attachmentsStep: nextStepId = technicalStep; break;
                case technicalStep: nextStepId = completedStep; break;
            }
            return nextStepId;
        }

        public string NBOfActiveContracts(string id)
        {

            //TODO : Get the count of active contracts for this customer (customerId)

            Random gen = new Random();
            int prob = gen.Next(10);
            return prob <= 6 ? "1" : "4";
        }

    }
}
