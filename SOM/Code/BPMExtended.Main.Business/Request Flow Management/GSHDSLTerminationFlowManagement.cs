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
    public class GSHDSLTerminationFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "A1CD8C90-92FD-4DAF-B5EA-209236874992";
        const string printStep = "9B66A56E-A8CC-41ED-9C3A-E1FC8C4C38A2";
        const string reasonStep = "A42DE75F-E0B8-474D-AEF0-6EB17EA1DFDE";
        const string billOnDemandStep = "7814BBA5-9786-4ABD-AF23-A4881184FFA1";

        const string paymentStep = "18F30A6C-5B4E-44C8-927B-260708AE31DB";
        const string technicalStep = "56AAA40B-3D07-43CC-A6F5-541D0711633F";
        const string attachmentStep = "3DF0ABDE-EFFC-41B9-883A-C34E11D3E94D";

        public string GetNextStep(string id, string currentStepId , bool isAdvantageous = false)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = printStep; break;
                case printStep: nextStepId = reasonStep; break;
                case reasonStep: nextStepId = NBOfActiveContracts(id) == "1" ? billOnDemandStep : isAdvantageous ? attachmentStep : paymentStep; break;
                case billOnDemandStep: nextStepId = isAdvantageous ? attachmentStep : paymentStep; break;
                case paymentStep: nextStepId = attachmentStep; break;
                case attachmentStep: nextStepId = technicalStep; break;
                //case dpTeamStep: nextStepId = ManualDSLAMForGSHDSL(id) ? pdnTeamStep : gshdslTeamStep; break;
            }
            return nextStepId;
        }

        public string NBOfActiveContracts(string id)
        {
            
            //TODO : Get the count of active contracts for this customer (customerId)

            Random gen = new Random();
            int prob = gen.Next(10);
            return prob <= 6 ? "1" : "4";
        }

        public bool ManualDSLAMForGSHDSL(string contractId)
        {
            Random gen = new Random();
            int prob = gen.Next(10);
            return prob <= 5;

        }

    }
}
