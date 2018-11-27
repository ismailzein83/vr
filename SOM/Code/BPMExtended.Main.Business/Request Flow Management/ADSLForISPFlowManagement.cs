using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class ADSLForISPFlowManagement
    {
        string welcomeStep = "F7BCCABF-7970-4CBD-A040-AA0D0FD89C08";
        string requestISPStep = "B45BAFD9-769E-421B-9157-8086F51BD509";
        string printStep = "94F2A388-B758-4175-A022-BBD1E6ED3171";
        string portReservationStep = "C01F3AD2-B611-43CA-8F86-F628A8A22688";
        string paymentStep = "424131A8-9D04-4F9F-8D18-279BB00BCDDA";
        string mDFStep = "F7760F00-F66F-4C89-A779-CE74E0E93294";
        string completedStep = "72EE0965-4F1F-4A3E-A78F-02DBDD96F168";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId.ToLower())
            {
                case "f7bccabf-7970-4cbd-a040-aa0d0fd89c08": nextStepId = requestISPStep; break;
                case "b45bafd9-769e-421b-9157-8086f51bd509": nextStepId = printStep; break;
                case "94f2a388-b758-4175-a022-bbd1e6ed3171": nextStepId = portReservationStep; break;
                case "c01f3ad2-b611-43ca-8f86-f628a8a22688": nextStepId = paymentStep; break;
                case "424131a8-9d04-4f9f-8d18-279bb00bcdda": nextStepId = mDFStep; break;
                case "f7760f00-f66f-4c89-a779-ce74e0e93294": nextStepId = completedStep; break;
            }
            return nextStepId;
        }
    }
}
