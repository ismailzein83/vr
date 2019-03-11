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
        const string waitingList = "5C2ED88E-B523-464B-84ED-202B3B742E75";
        const string paymentValidation = "FF84E115-B9B2-436A-A908-5A2A2F7CD8F7";
        const string address = "EC1F8C4D-E236-4196-9B20-44F18E2BB3B6";
        const string payment = "3D82D4D9-3A76-4B05-9C11-3801691E87DE";
        const string attachment = "922FEEC3-038A-4192-8D48-DEB62D2FB880";
        const string technical = "B544A0CB-35B5-4B79-B526-AE5B7EB7BBFE";
        const string completed = "15F6B32E-978A-421E-9FDF-CCA45D422AB8";

        public string GetNextStep(string id, string currentStepId, bool isWaitingList , bool isAdvantageous)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = nearbyNumbers; break;
                case nearbyNumbers: nextStepId = print; break;
                case print: nextStepId = temporaryReservation; break;
                case temporaryReservation: nextStepId = isWaitingList? waitingList : address; break;
                case waitingList: nextStepId = temporaryReservation; break;
                case address: nextStepId = isAdvantageous ? attachment : payment; break;
                case payment: nextStepId = attachment; break;
                case attachment: nextStepId = technical; break;
            }
            return nextStepId;
        }
    }
}
