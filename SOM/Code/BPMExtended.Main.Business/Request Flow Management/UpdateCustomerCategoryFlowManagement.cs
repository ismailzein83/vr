using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class UpdateCustomerCategoryFlowManagement
    {
        const string startingProcess = "4D8F183D-DA35-4D2B-9ABB-31366203A5BB";
        const string update = "57919F14-2759-413F-BCBA-8A82577B863A"; 
        const string payment = "921C2034-D89C-4A75-9B03-D10F59038CF3";
        const string technicalStep = "12A76A8F-F759-492A-9335-4FC01AEFF95E";
        const string submitToOM = "E07B3ABC-4713-46A7-AD13-11A6AFC079AA";
        const string endProcess = "9434699C-CA76-498F-A245-B993E3A3756F";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = update; break;
                case update: nextStepId = submitToOM; break;
               // case payment: nextStepId = technicalStep; break;
                //case technicalStep: nextStepId = submitToOM; break;
                case submitToOM: nextStepId = endProcess; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
