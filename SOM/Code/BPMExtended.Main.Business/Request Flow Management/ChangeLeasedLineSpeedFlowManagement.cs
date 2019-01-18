using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class ChangeLeasedLineSpeedFlowManagement
    {
        string welcomeStep = "FCEC824F-A8F8-440C-9BBD-78D4A3DA5952";
        string changeRatePlanStep = "F0CF970B-EF90-4266-96F5-D580A0A85F35";
        string technicalStep = "2497A8A4-E41C-4B56-A953-718107499FCE";
        string completedStep = "61BDF20E-6B3B-400F-BD33-BE48D8A14278";

        public string GetNextStep(string id, string currentStepId)
        {
            string nextStepId = "";
            switch (currentStepId.ToLower())
            {
                ///welcome step
                case "fcec824f-a8f8-440c-9bbd-78d4a3da5952": nextStepId = changeRatePlanStep; break;

                //changeRatePlanStep
                case "f0cf970b-ef90-4266-96f5-d580a0a85f35": nextStepId = technicalStep; break;

                /// Technical Step
                case "2497a8a4-e41c-4b56-a953-718107499fce": nextStepId = completedStep; break;
            }
            return nextStepId;
        }
    }
}
