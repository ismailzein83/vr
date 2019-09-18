using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class ChangeLeasedLineSpeedFlowManagement
    {
        const string welcomeStep = "FCEC824F-A8F8-440C-9BBD-78D4A3DA5952";
        const string changeRatePlanStep = "F0CF970B-EF90-4266-96F5-D580A0A85F35";
        const string technicalStep = "2497A8A4-E41C-4B56-A953-718107499FCE";
        const string completedStep = "61BDF20E-6B3B-400F-BD33-BE48D8A14278";

        public string GetNextStep(string id, string currentStepId)
        {
            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = changeRatePlanStep; break;
                case changeRatePlanStep: nextStepId = technicalStep; break;
                case technicalStep: nextStepId = completedStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
