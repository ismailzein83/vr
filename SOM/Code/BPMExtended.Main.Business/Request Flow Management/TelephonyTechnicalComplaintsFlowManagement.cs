using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class TelephonyTechnicalComplaintsFlowManagement
    {
        const string welcome = "EAD5BD2F-F20E-4F25-886E-6E9E13EAE238";
        const string complaint = "9846D108-7C3A-4829-A287-CDEDD1EDB48A";
        const string technicalStep = "54DF3EFD-0EFE-4974-BC72-307D1A2024D6";
        const string completed = "AAC06057-D8EB-46DA-B1E3-9DB21CC5FF94";

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
