using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class RemovePublicDIFlowManagement
    {
        const string welcomeStep = "53D06636-197B-46FD-8336-E35FABD59113";
        const string paymentStep = "9EF1AF2C-539B-4E7C-87AD-A1F7A9321944";
        const string submitToOMStep = "9005B215-BB84-4FD7-83DF-4F40D5BEFBF7";
        const string completedStep = "9F693E6D-5E0C-4552-B301-A979008E7DF2";
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
