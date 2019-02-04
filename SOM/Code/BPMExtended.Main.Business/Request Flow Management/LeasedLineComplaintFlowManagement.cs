using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class LeasedLineComplaintFlowManagement
    {
        const string welcome = "A69CFC3A-81D5-4360-A8B1-3D7A86D01390";
        const string complaint = "E07627C4-546A-4C37-BF0D-74D8769E907A";
        const string technicalStep = "1A3041F6-0E54-4A67-AC3C-19A6F8D5806C";
        const string completed = "19DDC748-EF03-441C-896E-4640FB802860";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcome: nextStepId = complaint; break;
                case complaint: nextStepId = technicalStep; break;
                case technicalStep: nextStepId = completed; break;
            }
            return nextStepId;
        }
    }
}
