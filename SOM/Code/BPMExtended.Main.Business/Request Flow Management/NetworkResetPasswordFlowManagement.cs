using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;

namespace BPMExtended.Main.Business
{
    public class NetworkResetPasswordFlowManagement
    {

        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "0126F306-69C3-4954-B195-8B13091932DA";
        const string resetStep = "6E18BF0A-0AF4-409B-9594-C1C12AEE1914";
        const string completedStep = "9730D12A-23FB-45D9-AB4E-126B1028BE91";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = resetStep; break;
                 case resetStep: nextStepId = completedStep; break;
            }
            return nextStepId;
        }

    }
}
