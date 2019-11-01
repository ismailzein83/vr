using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class AddPublicDIFlowManagement
    {
        const string welcomeStep = "368AF02F-8669-4F2E-87BE-8909A812F26A";
        const string paymentStep = "CA2579CE-2E52-4F24-9E7B-AFFC82BE9A66";
        const string submitToOMStep = "8DDA1D04-5737-4B63-93FF-874B88AFD3B9";
        const string completedStep = "657ADD47-AFC1-4BFE-BAB3-D3961517B710";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = paymentStep; break;// nextStepId = new CommonManager().IsRequestAdvantageous(new CommonManager().GetEntityNameByRequestId(id), id) ? submitToOMStep : paymentStep; break; 
                case paymentStep: nextStepId = submitToOMStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
