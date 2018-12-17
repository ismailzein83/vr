using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class ADSLChangePasswordFlowManagement
    {
        const string welcomeStep = "25635B80-1A12-4424-9972-14A1296AC024";
        const string newPasswordStep = "00198C0A-81EC-4F04-9410-556CB31C490B";
        const string printStep = "2D20C283-B0FE-430C-BC62-EF8887BA2FE3";
        const string completedStep = "E326FAEA-C8F9-497E-85E0-3B92CFC32231";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = newPasswordStep; break;
                case newPasswordStep: nextStepId = printStep; break;
                case printStep: nextStepId = completedStep; break;
            }
            return nextStepId;
        }
    }
}
