using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;

namespace BPMExtended.Main.Business
{
    public class ADSLContractTakeOverFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "34F00F0E-4854-456B-9890-C6E5754C3164";
        const string targetCustomerStep = "A1943354-B768-45BD-8AF9-BBD3B075F422";
        const string chooseTelephonyContractStep = "503DE3E8-4542-4D0D-A065-90AF34D87C76";
        const string credentialsStep = "26A83FF2-5DD7-4CDC-A250-57D2882DFC95";
        const string paymentStep = "64297E54-548A-41EA-A7CA-2ED660C6BA6D";
        const string technicalStep = "FB364A70-F0C4-4B0F-AE4A-6A38A9189C79";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = targetCustomerStep; break;
                case targetCustomerStep: nextStepId = chooseTelephonyContractStep; break;
                case chooseTelephonyContractStep: nextStepId = credentialsStep; break;
                case credentialsStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = technicalStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
