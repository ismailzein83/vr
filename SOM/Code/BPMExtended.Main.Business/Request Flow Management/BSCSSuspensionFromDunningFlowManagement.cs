using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class BSCSSuspensionFromDunningFlowManagement
    {
        const string suspension = "B0F39A64-6B4A-4350-BFEA-D6ED5461C691";
        const string technicalStep = "04AFF621-689F-47CD-8084-837B976D743E";
        const string submitToOM = "52E7F062-DBCE-457D-AD40-716C128AAD52";
        const string endProcess = "2F9ECCE9-DFD7-4F15-9FED-61483A30534A";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case suspension: nextStepId = submitToOM; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
