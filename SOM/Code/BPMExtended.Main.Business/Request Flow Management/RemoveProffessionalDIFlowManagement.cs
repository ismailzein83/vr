using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class RemoveProffessionalDIFlowManagement
    {
        const string welcomeStep = "E6414051-9549-4506-9AB4-FB0735A57A33";
        const string paymentStep = "313A7C9D-AEEA-4A76-8F96-C7D8B82F4DC7";
        const string submitToOMStep = "DFF0F187-91E5-462B-9855-93328F31C766";
        const string completedStep = "290C7A1A-DC83-4F79-A337-C209E61CD8CA";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = submitToOMStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
