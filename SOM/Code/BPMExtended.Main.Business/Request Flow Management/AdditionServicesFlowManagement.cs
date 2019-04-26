using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class AdditionServicesFlowManagement
    {
        const string welcomeStep = "064F30F5-A84B-442F-9BE4-5D31D3D9D7D5";
        const string servicesStep = "A8370D39-12D8-412C-93C2-D54BC9913F94";
        const string printStep = "A68DBF9B-09DF-4365-A74D-484D6C0D750C";
        const string paymentStep = "C7CA9EF8-5F81-484C-BE4E-4CC7F7180761";
        const string completedStep = "64436D8D-547F-4B12-AFAA-B44425FE6EAF";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = servicesStep; break;
                case servicesStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = printStep; break;
                case printStep: nextStepId = completedStep; break;
            }
            return nextStepId;
        }

    }
}
