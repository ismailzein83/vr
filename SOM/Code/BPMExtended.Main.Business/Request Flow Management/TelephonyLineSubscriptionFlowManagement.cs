using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class TelephonyLineSubscriptionFlowManagement
    {

        string welcomeStep = "26A7BD18-2E80-4901-9746-8061C7FF569D";
        string nearByNumberStep = "7C14E38A-F5CC-46DD-A94E-B49871B819E8";
        string printTemplateStep = "6895F711-7E76-4043-8777-9267D7CD27EC";
        string findFreeTechnicalStep = "EE7C2188-77A0-406C-BAA5-E29925509AE8";

        string waitingListStep = "E054EE92-661A-42D2-B0F6-9D78B12331B7";
        string networkTeamStep = "73F41594-A8DB-4967-ACB0-3B8B8E0694A8";
        string subscriptionAddressStep = "74FA6D04-804D-45D7-B3E3-887A98450F13";
        string servicesStep = "EEC0CF10-F7E2-45AD-953F-A35C1DAA4B27";

        string paymentStep = "6FD4F52A-310D-4E91-999D-F2D6FEC19AB0";
        string printContractStep = "1B93BB41-AA73-4430-9DB0-424243E1EE11";

        string technicalStep = "8D4B0B19-5F6B-4B52-A479-38DC7C4802B4";

        string completedStep = "EBACDCBA-0DB9-4582-999B-6317DA0094A7";

        public string GetNextStep(string id, string currentStepId , bool isWaitingOrNetwork)
        {

            string nextStepId = "";
            switch (currentStepId.ToLower())
            {   
                //welcome
                case "26a7bd18-2e80-4901-9746-8061c7ff569d": nextStepId = nearByNumberStep; break;
                //near by number
                case "7c14e38a-f5cc-46dd-a94e-b49871b819e8": nextStepId = printTemplateStep; break;
                //print template
                case "6895f711-7e76-4043-8777-9267d7cd27ec": nextStepId = findFreeTechnicalStep; break;
                //find free technical
                //case "ee7c2188-77a0-406c-baa5-e29925509ae8": !isWaitingOrNetwork? nextStepId = subscriptionAddressStep : ; break;

                case "424131a8-9d04-4f9f-8d18-279bb00bcdda": nextStepId = technicalStep; break;
                case "E3DB01E7-71B5-4658-9681-2DBC3DF301F9": nextStepId = completedStep; break;
            }
            return nextStepId;
        }

    }
}
