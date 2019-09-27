using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class UpdateAccountFlowManagement
    {
        const string startingProcess = "294059CD-34BA-4056-9C84-FA5A35D8B630";
        const string update = "1A904D24-436C-4E82-BFA9-94551C9DE0F1";
        const string submitToOM = "83D06326-0959-4639-9A75-2AC63A7EB5A4 ";
        const string endProcess = "9129190F-6B33-43DC-B404-3198273D3962";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = update; break;
                case update: nextStepId = submitToOM; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
