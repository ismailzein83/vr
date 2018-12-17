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
        const string mdfTeamStep = "7530D937-9988-4B6E-A480-7D486ED40402";
        const string cabinetTeamStep = "A0D55D49-1544-48DD-9917-24F72F06BABB";
        const string dpTeamStep = "CA488A8D-A149-441F-AAB1-D7D150A9D99D";
        const string pdnTeamStep = "F66793E5-CCA5-4AD0-A3C6-EE28EA7DF54D";
        const string gshdslTeamStep = "565A5F16-B8A9-436C-806D-53B98E23B6DB";
        const string completedStep = "DAFFC209-2A67-44B2-8706-CCC0094D1D0D";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = printStep; break;
                case printStep: nextStepId = reasonStep; break;
                case reasonStep: nextStepId = NBOfActiveContracts(id) == "1" ? billOnDemandStep : mdfTeamStep; break;
                case billOnDemandStep: nextStepId = mdfTeamStep; break;
                case mdfTeamStep: nextStepId = cabinetTeamStep; break;
                case cabinetTeamStep: nextStepId = dpTeamStep; break;
                case dpTeamStep: nextStepId = ManualDSLAMForGSHDSL(id) ? pdnTeamStep : gshdslTeamStep; break;
                case pdnTeamStep: nextStepId = gshdslTeamStep; break;
                case gshdslTeamStep: nextStepId = completedStep; break;
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
