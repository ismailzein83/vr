using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;

namespace BPMExtended.Main.Business
{
    public class LineUnBlockingFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "8173B462-4BD2-40F0-8CBF-1955A937A2BB";
        const string attachmentstep = "5177CAE4-851C-4119-BFFE-A54986719DDF";
        const string completedStep = "6B225395-E14D-4768-AFF3-74A7D51395A5";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = attachmentstep; break;
                case attachmentstep: nextStepId = completedStep; break;
            }
            return nextStepId;
        }
    }
}
