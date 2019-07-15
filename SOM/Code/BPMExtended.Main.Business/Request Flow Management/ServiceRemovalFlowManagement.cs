using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class ServiceRemovalFlowManagement
    {
        const string startingProcess = "1CA132AA-B104-4864-B5B4-8240CBA7E370";
        const string services = "1A7EDE59-D8A6-4BE9-BEF7-9253A4E68A91";
        const string payment = "BE514952-B291-4CF7-837C-FF49E80C7879";
        const string print = "BEA99786-3142-493A-BE56-3CC8411CDB5B";
        const string attachment = "E57851E1-C021-4B49-A675-5C4149F74593";
        const string technicalStep = "42CC62BA-A708-4097-A2B2-17CC2374AC13";
        const string submitToOM = "C68E5641-D066-45B5-B116-A9CD4F844947";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = services; break;
                case services: nextStepId = payment; break;
                case payment: nextStepId = print; break;
                case print: nextStepId = attachment; break;
                case attachment: nextStepId = submitToOM; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
