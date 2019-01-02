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
        const string switchTeamStep = "B5FF3B58-133E-4C62-9EE8-1BC579B18F88";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcessStep: nextStepId = deactivationStep; break;
                case deactivationStep: nextStepId = printStep; break;
                case printStep: nextStepId = switchTeamStep; break;
            }
            return nextStepId;
        }
    }
}
