using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class DeactivateCptFlowManagement
    {
        const string startingProcessStep = "34419DBF-6E1E-45B3-AE01-7ED916FA4907";
        const string printStep = "06338F96-CEE8-432C-B33C-8FE0DC10F0F7";
        const string attachmentStep = "5FD61233-3779-4EB7-AEFE-239DF78FB760";
        const string technicalStep = "14209AD6-E05E-4B76-B7D0-6A416A95F975";

        public string GetNextStep(string id, string currentStepId)
        {
            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcessStep: nextStepId = printStep; break;
                case printStep: nextStepId = attachmentStep; break;
                case attachmentStep: nextStepId = technicalStep; break;
            }
            return nextStepId;
        }
    }
}
