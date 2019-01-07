using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class LineMovingSameSwitchFlowManagement
    {
        const string startingProcess = "2B69B9AE-3C35-4B4C-9348-2031CA882521";
        const string print = "9329C030-8F61-4919-84CD-FCA35EEA2660";
        const string nearbyNumbers = "9716D9CF-791E-4DFA-93EC-C2F814C10A8C";
        const string temporaryReservation = "0048D6CF-A4C2-49DB-8512-4E8F24BDE9BA";
        const string mDFOld = "279E466D-39A8-4376-95F6-304032A8C3C2";
        const string mDFNew = "EE4F7148-613D-48B8-A0A0-CE8853A485F9";
        const string cabinetOld = "BD02B64F-6065-4A2C-A649-FFB81B18E46C";
        const string cabinetNew = "471A0856-236D-4E18-80A5-765107978E35";
        const string dPOld = "234568BA-4418-4592-8B64-877A09ECC4FB";
        const string dPNew = "1E3BA234-552A-4AFB-926A-18B621CBA11A";
        const string validation = "84E6996E-B7B9-400D-8114-BC1E0ED9B099";
        const string completed = "15F6B32E-978A-421E-9FDF-CCA45D422AB8";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = print; break;
                case print: nextStepId = nearbyNumbers; break;
                case nearbyNumbers: nextStepId = temporaryReservation; break;
                case temporaryReservation: nextStepId = mDFOld; break;
                case mDFOld: nextStepId = mDFNew; break;
                case mDFNew: nextStepId = cabinetOld; break;
                case cabinetOld: nextStepId = cabinetNew; break;
                case cabinetNew: nextStepId = dPOld; break;
                case dPOld: nextStepId = dPNew; break;
                case dPNew: nextStepId = validation; break;
                case validation: nextStepId = completed; break;
            }
            return nextStepId;
        }
    }
}
