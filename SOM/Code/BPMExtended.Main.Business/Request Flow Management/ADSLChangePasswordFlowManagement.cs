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
        const string paymentStep = "CD4C4BBC-A6BE-4746-8680-4AB859DA0E1F";
        const string printStep = "2D20C283-B0FE-430C-BC62-EF8887BA2FE3";
        const string technicalStep = "3F70A8B3-DAD2-4428-80C2-EABB8C19CD55";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = newPasswordStep; break;
                case newPasswordStep: nextStepId = new CommonManager().IsRequestAdvantageous(new CommonManager().GetEntityNameByRequestId(id), id) ?printStep: paymentStep; break;
                case paymentStep: nextStepId =  printStep; break;
                case printStep: nextStepId = technicalStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
