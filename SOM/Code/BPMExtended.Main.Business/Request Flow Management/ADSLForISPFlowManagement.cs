using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class ADSLForISPFlowManagement
    {
        const string startingProcessStep = "F7BCCABF-7970-4CBD-A040-AA0D0FD89C08";
        const string portAndIspStep = "B45BAFD9-769E-421B-9157-8086F51BD509";
        const string printStep = "94F2A388-B758-4175-A022-BBD1E6ED3171";
        const string paymentStep = "424131A8-9D04-4F9F-8D18-279BB00BCDDA";
        const string attachmentStep = "A7B0D465-8D20-46DC-9C79-833C59F4EA09";
        const string technicalStep = "E3DB01E7-71B5-4658-9681-2DBC3DF301F9";
        const string submitToOM = "2B26EAC5-CEF8-47BF-9D01-9766DE951F7B";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcessStep: nextStepId = portAndIspStep; break;
                case portAndIspStep: nextStepId = new CommonManager().IsRequestAdvantageous(new CommonManager().GetEntityNameByRequestId(id), id)? printStep : paymentStep; break;
                case paymentStep: nextStepId = printStep; break;
                case printStep: nextStepId = attachmentStep; break;
                case attachmentStep: nextStepId = submitToOM; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
