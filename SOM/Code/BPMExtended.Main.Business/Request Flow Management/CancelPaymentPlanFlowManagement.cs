using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class CancelPaymentPlanFlowManagement
    {
        const string startingProcess = "4FAFDCB9-B414-4E58-9BBA-0EE01D861B36";
        const string print = "19F12ADD-3885-4B8D-BE50-8F9EF8EEFAB5";
        const string completed = "78901F89-A682-4367-85F7-5F4DE34A6641";

        public string GetNextStep(string id, string currentStepId)
        {
            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = print; break;
                case print: nextStepId = completed; break;
            }
            return nextStepId;
        }
    }
}
