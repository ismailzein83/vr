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

        const string welcomeStep = "4BEAC3ED-8CAF-49AE-ADB6-AF9312AEEB82";
        const string paymentStep = "869A509F-425B-484C-A9A9-DA0B1340C6A7";
        const string attachmentstep = "159A9474-196B-46D1-B5FA-23BD0C3D27E6";
        const string submittedtoom = "EDA235A3-471F-48BA-94C2-9D3DA16B7C08";
        const string completedStep = "A7845FA8-8454-492E-908C-90B9E309C9FB";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = attachmentstep; break;
                case attachmentstep: nextStepId = submittedtoom; break;
                case submittedtoom: nextStepId = completedStep; break;
            }
            return nextStepId;
        }
    }
}
