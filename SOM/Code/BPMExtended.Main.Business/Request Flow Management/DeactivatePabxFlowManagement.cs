using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class DeactivatePabxFlowManagement
    {
        const string startingProcessStep = "DC8E5ACF-80A0-49AE-8EED-3B18A02AB2EC";
        const string deactivationStep = "F443F693-4DC8-46E1-9D9A-4613D44ACE30";
        const string printStep = "BD8E3019-7751-4CA6-9703-68989C9C377A";
        const string attachmentStep = "692336A3-BD7D-49D1-8991-5E832B3EAED7";
        const string technicalStep = "C666F283-43BB-470E-A104-A6AA5D25F42E";
        const string completed = "D81C2F66-3A3D-45E5-9BF1-3990C430F362";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcessStep: nextStepId = deactivationStep; break;
                case deactivationStep: nextStepId = printStep; break;
                case printStep: nextStepId = attachmentStep; break;
                case attachmentStep: nextStepId = technicalStep; break;
            }
            return nextStepId;
        }
    }
}
