using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class CallDetailsRequestFlowManagement
    {
        const string welcomeStep = "63B482FA-23AC-4ACE-8B8B-14192FE17CBB";
        const string selectOptionsStep = "B2EF53AA-FCE2-4F78-804D-16433A3B6F1C";
        const string paymentStep = "504212A0-A4AD-470B-A32D-7773D536CC3C";
        const string downloadStep = "E4078612-C8F1-428F-8F2A-31AA477C232F";
        const string submitToOMStep = "A1563C0C-60BE-4448-8D55-11BEB6BCD72D";
        const string completedStep = "0E6F81C9-5F92-4FDF-ABAB-5C39B18E49E9";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = selectOptionsStep; break;
                case selectOptionsStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = downloadStep; break;
                case downloadStep: nextStepId = completedStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
