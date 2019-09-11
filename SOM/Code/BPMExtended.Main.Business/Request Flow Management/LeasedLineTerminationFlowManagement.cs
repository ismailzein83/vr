using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Entities;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class LeasedLineTerminationFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "E89F9048-3ADB-413F-BEA3-B3962A37BBA4";
        const string reasonStep = "C684A324-664E-4893-B03D-B5EDCD10B788";
        const string paymentStep = "7C9C8FA7-9BFB-4BC3-9196-1D69B26B633F";
        const string technicalStep = "352722D4-9AFC-4C93-8206-1C30434EFD63";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = reasonStep; break;
                case reasonStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = technicalStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));

            }
            return nextStepId;
        }

    }
}
