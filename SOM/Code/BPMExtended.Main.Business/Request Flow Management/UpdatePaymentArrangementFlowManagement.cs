using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
   public class UpdatePaymentArrangementFlowManagement
    {
        const string startingProcess = "001AF3DF-E838-4AB4-A56D-6B208498D459";
        const string update = "1DB22F4B-554F-4B0A-9DE5-D03168031552";
        const string submitToOM = "3A1CAE97-B20D-44E6-9446-1C1B6DE2500C";

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
