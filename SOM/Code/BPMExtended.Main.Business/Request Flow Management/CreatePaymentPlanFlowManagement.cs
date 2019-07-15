﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class CreatePaymentPlanFlowManagement
    {
        const string startingProcess = "B2FBC219-6755-413C-9D81-0FFAB1C976DD";
        const string paymentTemplate = "119AC20E-C0B1-4699-8250-F057E6E070EF";
        const string print = "994DB20F-4F9F-4320-B730-538ACD72AC30";
        const string technicalStep = "D51B409B-85DE-40D3-A0D1-2D96B509306E";
        const string submitToOM = "E8B4AF7E-095B-4BF0-8B11-8C7FDBEE57F9";
        const string completed = "63130DA4-C53E-435D-8855-A4A715852D3A";

        public string GetNextStep(string id, string currentStepId)
        {
            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = paymentTemplate; break;
                case paymentTemplate: nextStepId = print; break;
                case print: nextStepId = technicalStep; break;
                case technicalStep: nextStepId = submitToOM; break;
                case submitToOM: nextStepId = completed; break;
            }
            return nextStepId;
        }
    }
}
