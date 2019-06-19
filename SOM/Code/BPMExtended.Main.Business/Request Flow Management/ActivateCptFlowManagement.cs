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
        const string payment = "5292113D-BC65-403F-8765-7B69B70DD443";
        const string print = "BF1F7C56-5219-4206-9834-C9813DB540B0";
        const string attachment = "F81E6A38-9763-47BD-89A9-2297062F0CCC";
        const string technicalStep = "A8F7C0E8-4903-4343-93E4-87295BF4D64E";
        const string submitToOM = "0F79F674-EAEA-4380-BB4F-648251A92D74";
        const string endProcess = "2A290E4F-A034-46A5-95BF-E0C8ED760805";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = reserveNumber; break;
                case reserveNumber: nextStepId = print; break;
                case print: nextStepId = payment; break;
                case payment: nextStepId = attachment; break;
                case attachment: nextStepId = submitToOM; break;
            }
            return nextStepId;
        }
    }
}
