using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class AdministrativeComplaintFlowManagement
    {
        const string startingProcess = "80F01194-05E3-4D08-BB30-4BED014166E2";
        const string complaint = "7106E163-2939-4ABC-8C77-11D2D1487CB7";
        const string print = "74B623EA-8A65-4269-8B89-7920F8B46C10";
        const string technicalStep = "1AB2A86C-C696-41D3-8CF4-B77D9B804D18";
        const string completed = "E84E9E58-91EE-4BA0-A037-23A7E7C8AC52";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = complaint; break;
                case complaint: nextStepId = print; break;
                case print: nextStepId = technicalStep; break;
                case technicalStep: nextStepId = completed; break;
            }
            return nextStepId;
        }
    }
}
