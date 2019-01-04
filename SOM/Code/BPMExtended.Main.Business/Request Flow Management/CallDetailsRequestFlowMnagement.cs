using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class CallDetailsRequestFlowMnagement
    {
        string welcomeStep = "63B482FA-23AC-4ACE-8B8B-14192FE17CBB";
        string selectOptionsStep = "B2EF53AA-FCE2-4F78-804D-16433A3B6F1C";
        string paymentStep = "504212A0-A4AD-470B-A32D-7773D536CC3C";
        string downloadStep = "E4078612-C8F1-428F-8F2A-31AA477C232F";
        string completedStep = "0E6F81C9-5F92-4FDF-ABAB-5C39B18E49E9";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId.ToLower())
            {
                case "63b482fa-23ac-4ace-8b8b-14192fe17cbb": nextStepId = selectOptionsStep; break;
                case "b2ef53aa-fce2-4f78-804d-16433a3b6f1c": nextStepId = paymentStep; break;
                case "504212a0-a4ad-470b-a32d-7773d536cc3c": nextStepId = downloadStep; break;
                case "e4078612-c8f1-428f-8f2a-31aa477c232f": nextStepId = completedStep; break;
            }
            return nextStepId;
        }
    }
}
