using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class RevokeFlowManagement
    {
        const string welcomeStep = "d6eeffac-8533-47f7-8a32-32f63223277f";
        const string paymentStep = "b7129d59-96a0-4e95-ad9e-59ed425d0a4a";
        const string attachmentstep = "b95e0860-c03a-443a-af09-28d9a1d3d9cd";
        const string submittedtoom = "a4f00097-11cf-4b54-897d-6fde80bf2e1b";
        const string completedStep = "5e803dda-f776-4ddd-a971-345d87a99d96";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = attachmentstep; break;
                case attachmentstep: nextStepId = submittedtoom; break;
                case submittedtoom: nextStepId = completedStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
