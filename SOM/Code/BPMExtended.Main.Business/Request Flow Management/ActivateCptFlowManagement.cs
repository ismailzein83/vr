﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class ActivateCptFlowManagement
    {
        const string startingProcess = "337976F9-2F42-4382-9111-817975698969";
        const string reserveNumber = "170E4B07-9263-4631-BF82-D647EC0D484E";
        const string print = "BF1F7C56-5219-4206-9834-C9813DB540B0";
        const string technicalStep = "A8F7C0E8-4903-4343-93E4-87295BF4D64E";
        //const string completed = "";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = reserveNumber; break;
                case reserveNumber: nextStepId = print; break;
                case print: nextStepId = technicalStep; break;
                //case technicalStep: nextStepId = completed; break;
            }
            return nextStepId;
        }
    }
}