using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class UpdateCustomerFlowManagement
    {
        const string startingProcess = "808A82DD-C51D-4092-92E1-63668D489D6C";
        const string update = "C3DDA1CA-5018-4190-89CC-8DBC882FAC12";
        const string submitToOM = "769B1CB5-15BD-45C7-8B22-C69CEAF14623";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = update; break;
                case update: nextStepId = submitToOM; break;
            }
            return nextStepId;
        }
    }
}
