using System;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    
    public class LineMovingNewSwitchFlowManagement //To be completed
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        const string startingProcess = "41E98EE2-5F2D-4DC2-9A48-74B3E2FC3CB8";
        const string print = "B6E3319E-2DFC-450C-AE9D-52376C174E44";
        const string lineDescription = "440D0312-56EF-43FB-8D69-7A24ADF1621A";
        const string freeReservation = "BC91A824-24B2-48F5-992B-9B7DCB161F93";
        const string address = "91F109E6-486E-4024-AF3C-1A8498E3EA2F";
        const string payment = "00A0E8E2-565F-44FD-9056-B77244A2E956";
        const string print2 = "4ECB28B2-9E20-4B42-BD9B-99AFC0349452";
        const string oldSwitch = "1082C816-1665-443B-BD0C-F5AEB095FA10";
        const string oldMDF = "471A0856-236D-4E18-80A5-765107978E35";
        const string oldCabinet = "755B0334-012A-45EE-A796-E3A0E607207E";
        const string oldDP = "54715C3C-481A-4BEF-BA2D-E66F2F72C048";
        const string newSwitch = "EEF2A6C3-B2B0-4FBC-AE25-756A2874A869";
        const string newMDF = "346A034A-650B-4969-929E-4287E66E0302";
        const string newCabinet = "0A912BEE-1B24-42F4-BC07-BA08A8E2392B";//Port Change UI
        const string mDFValidate = "B216D8DE-DC61-400B-9324-AA71E78B8F81";
        const string newDP = "739EAD7A-6ED1-47CE-94EF-8705559E79B1";//Port Change UI
        const string cabinetValidate = "A28457D2-599A-46BB-847C-E98D296C54AC";
        const string validation = "5A1B3225-0CE5-483E-B580-FE498AE2C2B9";
        const string completed = "9D780792-3EB0-4AF7-A455-70CE2A64FCCE";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId.ToUpper())
            {
                case startingProcess: nextStepId = print; break;
                case print: nextStepId = lineDescription; break;
                case lineDescription: nextStepId = freeReservation; break;
                case freeReservation: nextStepId = address; break;
                case address: nextStepId = payment; break;
                case payment: nextStepId = print2; break;
                case print2: nextStepId = ManualSwitch(id)? oldSwitch : oldMDF ; break;
                case oldSwitch: nextStepId = oldMDF; break;
                case oldMDF: nextStepId = oldCabinet; break;
                case oldCabinet: nextStepId = oldDP; break;
                case oldDP: nextStepId = IsAutomaticSwitchType(id)?newMDF:newSwitch; break;
                case newSwitch: nextStepId = newMDF; break;
                case newMDF: nextStepId = newCabinet; break;
                //case newCabinet: nextStepId = ()? mDFValidate: newDP; break;
                case mDFValidate: nextStepId = newDP; break;
                // case newDP: nextStepId = ()? validation: cabinetValidate; break;
                case cabinetValidate: nextStepId = validation; break;
                case validation: nextStepId = completed; break;
            }
            return nextStepId;
        }

        public bool ManualSwitch(string id)
        {
            bool ismanualswitch = false;
            if (id != "")
            {
                Guid idd = new Guid(id.ToUpper());
                var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineMovingNewSwitch");

                esq.AddColumn("Id");
                esq.AddColumn("StContractId");

                var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);

                esq.Filters.Add(esqFirstFilter);

                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var contractId = entities[0].GetColumnValue("StContractId");
                    InventoryManager manager = new InventoryManager();
                    ismanualswitch = manager.IsManualSwitch(contractId.ToString());
                }
            }
            return ismanualswitch;
        }
        public bool IsAutomaticSwitchType(string id)
        {
            bool isAutomaticSwitch = false;
            if (id != "")
            {
                Guid idd = new Guid(id.ToUpper());
                var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLineMovingNewSwitch");

                esq.AddColumn("Id");
                esq.AddColumn("StReservedSwitchType");

                var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);

                esq.Filters.Add(esqFirstFilter);

                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var reservedSwitchType = entities[0].GetColumnValue("StReservedSwitchType");
                    isAutomaticSwitch = reservedSwitchType.ToString() == "Automatic"? true: false;
                }
            }
            return isAutomaticSwitch;
        }

    }
}
