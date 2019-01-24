using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class ADSLComplaintsFlowManagement
    {
        const string welcome = "1108E818-F78F-42AB-BC63-4461592D5EFE";
        const string complaint = "3CCDD764-9700-46E6-91EB-5270DCBB1724";
        const string technicalStep = "836EDBD7-1761-440C-9D30-CCD0A47941B0";
        const string completed = "D1E41679-B952-40AC-96A0-4F2AA26A4968";

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
