using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class BillOnDemandRequestFlowManagement
    {
        const string welcomeStep = "650c4168-1dda-4fc1-82ac-4a119b5fba6d";
        const string attachementStep = "ea975beb-bcef-4ce6-aa33-5a63457143c1";
        const string simulateRequest = "9da1ee4a-70bc-4f13-a822-8d5b35c38c24";
        const string completedStep = "2c9774ae-a1e7-4744-abac-65211eb562d7";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = attachementStep; break;
                case attachementStep: nextStepId = simulateRequest; break;
                case simulateRequest: nextStepId = completedStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
