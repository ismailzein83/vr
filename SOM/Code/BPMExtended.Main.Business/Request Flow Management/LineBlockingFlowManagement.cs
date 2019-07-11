using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;

namespace BPMExtended.Main.Business
{
    public class LineBlockingFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "1109C15B-9A13-4CE0-AFCA-467DFAD399DD";
        const string paymentStep = "4BAF697B-E9E1-417A-8531-525EA4077902";
        const string attachmentstep = "A934D243-BEA2-42F1-947A-906A18F1BED6";
        const string submittedtoom = "D674711D-9AC1-4C6D-A9EC-1E0546DAADAF";
        const string completedStep = "9ACA0E2B-4D75-4F13-A486-C2674D2F8B0F";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = attachmentstep; break;
                case attachmentstep: nextStepId = submittedtoom; break;
                case submittedtoom: nextStepId = completedStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
