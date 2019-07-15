using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class LineMovingFlowManagement
    {
        const string startingProcess = "EB1EBAC2-1144-4254-92D9-E9AD63D44FE2";
        const string nearbyNumber= "686EAE7A-F4D6-4043-A381-DA2E8F4CE7ED";
        const string print = "36DBA738-559E-4C0D-BCBC-02FCA872FBAA";
        const string address = "810A0B83-67F3-4875-A8CD-9ECDDE4E7ABE";
        const string freeReservation = "B6833EFF-5653-4DC8-AB32-F3973A86278C";

        const string adslStep = "4745C0D7-747D-4C5F-AABB-6F0343B4D917";
        const string waitingList = "B12DB4FE-5EF9-4CD7-96CA-B21E7C007EA3";
        const string adslWaitingList = "FA2E6C40-2607-4AC0-ABDC-6EA3B4EAB500";
        const string adslFreeReservation = "FE8A035A-C803-483D-9E41-61EE3243F778";


        const string payment = "E4DCA68F-FDC7-454F-9DA9-8E922DAF88FD";
        const string printNewDocument = "08355BEC-A39D-4721-8E8B-969A68B1996E";
        const string attachments = "ACFE97C5-A396-45A1-A5F7-435E0D1850EB";
        const string technicalStep = "B2B4E3AF-B99D-42E9-9E6E-717D145F3258";

        public string GetNextStep(string id, string currentStepId, bool isWaitingList, bool isADSLWaitingList, bool hasADSL)
        {

            string nextStepId = "";
            switch (currentStepId.ToUpper())
            {
                case startingProcess: nextStepId = nearbyNumber; break;
                case nearbyNumber: nextStepId = print; break;
                case print: nextStepId = freeReservation; break;
                case freeReservation: nextStepId = isWaitingList ? waitingList : hasADSL? adslStep:address; break;
                case waitingList: nextStepId = freeReservation; break;
                case adslStep: nextStepId = hasADSL?adslFreeReservation:address; break;
                case adslFreeReservation: nextStepId = isADSLWaitingList ? adslWaitingList :address; break;
                case adslWaitingList: nextStepId = adslFreeReservation; break;
                case address: nextStepId = payment; break;
                case payment: nextStepId = printNewDocument; break;
                case printNewDocument: nextStepId = attachments; break;
                case attachments: nextStepId = technicalStep; break;
                default: throw new InvalidOperationException(string.Format("Step not found. Id = {0}, current step id= {1}", id, currentStepId));

            }
            return nextStepId;
        }
    }
}
