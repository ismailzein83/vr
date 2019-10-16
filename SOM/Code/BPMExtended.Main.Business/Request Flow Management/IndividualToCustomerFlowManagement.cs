using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class IndividualToCustomerFlowManagement
    {
        const string startingProcess = "23236E9C-3A43-4210-8387-1D2270387516";
        const string customerData = "0AE0C793-4829-495D-B069-D5ED16BB7C8B";
        const string submitToOM = "2B4B8AF1-6650-492A-A34E-3ABE69384B34";
        const string endProcess = "F83E52AF-00D4-4FDE-913A-A19A0E43C6CB";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = customerData; break;
                case customerData: nextStepId = submitToOM; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
