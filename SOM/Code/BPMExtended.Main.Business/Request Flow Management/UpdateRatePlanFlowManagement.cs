using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;

namespace BPMExtended.Main.Business
{
    public class UpdateRatePlanFlowManagement
    {

        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "97C86E0F-3009-4104-9487-17F86DA3CA28";
        const string updateRatePlanStep = "1552AB5D-44A0-4138-99D2-07523D62E11E";
        const string paymentStep = "132F24A7-8488-4E35-8B53-9E38D2A79B33";
        const string attachmentStep = "2129CEEE-BA8D-4A00-89A8-ED09BB1DCAD4";
        const string completedStep = "1F05083D-6074-47DD-822D-ED30362E30BE";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = updateRatePlanStep; break;
                case updateRatePlanStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = attachmentStep; break;
                case attachmentStep: nextStepId = completedStep; break;
            }
            return nextStepId;
        }
    }
}
