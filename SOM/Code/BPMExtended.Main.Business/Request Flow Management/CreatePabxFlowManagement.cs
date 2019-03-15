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
    public class CreatePabxFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "AEB4AA8D-5371-4B09-8862-4D3F5111EE48";
        const string reserveNumbersStep = "2883FC66-8A4C-4D2F-81DD-3109D47D010B";
        const string printStep = "65BB4FBB-121E-4BB8-A4ED-A01F9C2F7803";
        const string attachmentStep = "5A456A6D-47BE-49B8-BF4A-E0B27CEA2DCA";
        //const string switchTeamStep = "3AE7FEA6-3CEE-4C4D-91D6-015571946B2D";
        const string technicalStep = "4AD46645-AD4A-4F53-83DC-EB38831867FA";
        const string completedStep = "1386C350-017D-400B-855C-04FC427868BF";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = reserveNumbersStep; break;
                case reserveNumbersStep: nextStepId = printStep; break;
                case printStep: nextStepId = attachmentStep; break;
                case attachmentStep: nextStepId = technicalStep; break;
            }
            return nextStepId;
        }

    }
}
