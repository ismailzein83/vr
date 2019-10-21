using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class UpdateOfficialAccountFlowManagement
    {
        const string startingProcess = "0F10B27F-4255-452E-BAC4-F792C73F72BB";
        const string update = "EA63090E-38A6-4337-B6E7-ED11800D4913";
        const string submitToOM = "9686ABD4-2550-4169-B665-02957E4B5833";

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
