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
        const string paymentStep = "D839D8C4-3A30-4816-A7FC-DBF661DCBD71";
        const string printStep = "0523C75F-E667-4431-8D77-052AB1A5F60F";
        const string attachmentStep = "C4F82323-23C3-47FF-BD50-A74EF2459E31";
        const string submitStep = "D475ADD0-42F7-491A-8B3A-FAFDC6CF655B";
        public string GetNextStep(string id, string currentStepId)
        {
            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = FAFNumbers; break;
                case FAFNumbers: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = printStep; break;
                case printStep: nextStepId = attachmentStep; break;
                case attachmentStep: nextStepId = submitStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));
            }
            return nextStepId;
        }
    }
}
