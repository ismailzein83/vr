using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class FriendAndFamilyFlowManagement
    {

        const string welcomeStep = "12A5DE33-6297-455C-88B5-5F793144B417";
        const string FAFNumbers = "A917CD0B-8A0E-493C-B7BD-A37DAC815EB2";
        const string printStep = "0523C75F-E667-4431-8D77-052AB1A5F60F";
        const string completedStep = "498A1C06-03A9-4EB7-A3BF-B0CC6554C40A";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = FAFNumbers; break;
                case FAFNumbers: nextStepId = printStep; break;
                case printStep: nextStepId = completedStep; break;
            }
            return nextStepId;
        }
    }
}
