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
        const string completedStep = "1F05083D-6074-47DD-822D-ED30362E30BE";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = updateRatePlanStep; break;
                case updateRatePlanStep: nextStepId = completedStep; break;
            }
            return nextStepId;
        }
    }
}
