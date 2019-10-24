using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ADSLAlterSpeedFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "1023C475-90F9-4329-B676-2C67DA8703AF";
        const string ratePlanStep = "BFD14429-929D-4968-9179-C3E8688105D1";
        const string paymentStep = "B93E26E9-FDD4-48E6-9AE5-97815A04F8B7";
        const string technicalStep = "B0DBAD29-F63F-41E3-98FE-8EA06B57B9A7";
        const string submitToOMStep = "C06ED9AB-332A-4D65-B397-4104548E9B83";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = ratePlanStep; break;
                case ratePlanStep: nextStepId = new CommonManager().IsRequestAdvantageous(new CommonManager().GetEntityNameByRequestId(id), id) ? submitToOMStep : paymentStep; break;
                case paymentStep: nextStepId = submitToOMStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
