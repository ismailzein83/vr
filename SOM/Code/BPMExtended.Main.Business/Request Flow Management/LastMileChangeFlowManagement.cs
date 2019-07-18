using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class LastMileChangeFlowManagement
    {
        const string startingProcess = "F5D511B7-77D0-47B3-B202-0BEE25BD84DE";
        const string adminOrderId = "0B91570A-DCC9-401F-8B40-4288CC58E20A";
        const string address = "25FF3115-1B5B-4C28-991E-3DFF92F5D0D3";
        const string payment = "0AF4B588-12A9-4720-B541-31FCFF359231";
        const string technicalStep = "839286F1-B57D-4543-811B-3BBBC0967703";
        const string submitToOM = "F815C59D-487D-4E0B-9B6D-F52EC3E8E14B";
        const string endProcess = "BDFC6820-9932-4FE1-A628-D3537EFF100E";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = adminOrderId; break;
                case adminOrderId: nextStepId = address; break;
                case address: nextStepId = payment; break;
                case payment: nextStepId = technicalStep; break;
            }
            return nextStepId;
        }
    }
}
