using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class ADSLPrintConfigurationFlowManagement
    {
        const string welcomeStep = "F031C238-2B2B-4F2E-B73C-C24EB5E2E605";
        const string printStep = "2595F010-0737-45FA-9F25-8ACD66F1A5A9";
        const string completedStep = "219E9C9D-A21C-40F3-90B3-F5672E79DE0B";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = printStep; break;
                case printStep: nextStepId = completedStep; break;
            }
            return nextStepId;
        }

    }
}
