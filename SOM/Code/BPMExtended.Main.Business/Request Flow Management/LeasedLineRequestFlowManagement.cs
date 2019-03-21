using System;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class LeasedLineRequestFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string startingProcess = "59C93AF1-7B3E-4DBC-AB1A-770098F8204B";
        const string requestID = "3B9222AF-73F8-4F4F-BC3C-0BB0697DC795";
        const string address = "5237ECDF-8455-4720-A2CF-8CE3AC415921";
        const string technicalTeam = "53BD9DCE-7695-4007-97F0-0466CEAE356F";
        const string services = "AD7EB0E2-2B8D-4F3B-86E8-26FFEAF89409";

        const string discount = "BF730CB5-396F-4720-A663-E16A8A09C7FB";
        const string paymentMethod = "16A296DF-6695-4FD7-AA11-4381BC4A9874";
        const string print = "E7C4431A-615D-44A6-8811-8DD731A378DB"; // to technical
        const string attachmentStep = "013C02E5-203A-424C-8F7A-F10A82B1466E";
        const string technicalStep = "0545B38D-F1BA-4CD6-BC96-1BEFD85C9B3A";

        //Technical Teams
        const string site1MDFCabling = "48CE764E-C1DD-4B8B-A5FF-A111B43E6028";
        const string site1CabinetTeam = "D8EFC5C5-5480-4A76-A6BC-2E306D42B70C";
        const string site1DPTeam = "AA515839-89AC-47CC-BF9E-C95A346679A3";
        const string mic = "A0302F8A-91A5-43ED-8F22-FB37A1B044D2";
        const string site2MDFCabling = "25556706-7E48-4251-A9C0-3D809C3B104C";
        const string site2CabinetTeam = "A7EF2BCA-81F2-469D-9CEA-D382443D90D9";
        const string site2DPTeam = "48E70652-F3EE-46A4-9277-6FCB48768262";
        const string fiberTeam = "0ACA0696-5FC1-4DF8-9AE9-3FFA604C5A41";
        const string microwaveTeam = "A71308A3-8C7B-416A-8315-8A679142EB78";
        const string Completed = "DBEF5609-03E6-47D9-A901-6F48914A57F3";


        public string GetNextStep(string id, string currentStepId, bool isFiber = false, bool isMicrowave = false)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcess: nextStepId = requestID; break;
                case requestID: nextStepId = address; break;
                case address: nextStepId = technicalTeam; break;
                case technicalTeam: nextStepId = services; break;
                case services: nextStepId = isFiber ? discount : isMicrowave ? paymentMethod : print; break;
                case discount: nextStepId = paymentMethod; break;
                case paymentMethod: nextStepId = isMicrowave ? paymentMethod : print; break;
                case print: nextStepId = attachmentStep; break;
                case attachmentStep: nextStepId = technicalStep; break;

                //case print: nextStepId = site1MDFCabling; break;
                //case site1MDFCabling: nextStepId = site1CabinetTeam; break;
                //case site1CabinetTeam: nextStepId = site1DPTeam; break;
                //case site1DPTeam: nextStepId = mic; break;
                //case mic: nextStepId = site2MDFCabling; break;
                //case site2MDFCabling: nextStepId = site2CabinetTeam; break;
                //case site2CabinetTeam: nextStepId = site2DPTeam; break;
                //case site2DPTeam: nextStepId = fiberTeam; break;
                //case fiberTeam: nextStepId = microwaveTeam; break;
                //case microwaveTeam: nextStepId = Completed; break;
            }
            return nextStepId;
        }

        public bool IsFiberFounded(string id)
        {
            bool isFiberFounded = false;
            if (id != "")
            {
                Guid idd = new Guid(id.ToUpper());
                var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLeasedLine");

                esq.AddColumn("Id");
                esq.AddColumn("StIsFiberServiceSelected");

                var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);

                esq.Filters.Add(esqFirstFilter);

                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var fiber = entities[0].GetColumnValue("StIsFiberServiceSelected");
                    isFiberFounded = (bool)fiber==true ? true : false;
                }
            }
            return isFiberFounded;
        }

        public bool IsMicrowaveService(string id)
        {
            bool isMicrowaveService = false;
            if (id != "")
            {
                Guid idd = new Guid(id.ToUpper());
                var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLeasedLine");

                esq.AddColumn("Id");
                esq.AddColumn("StIsMicrowaveServiceSelected");

                var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);

                esq.Filters.Add(esqFirstFilter);

                var entities = esq.GetEntityCollection(BPM_UserConnection);
                if (entities.Count > 0)
                {
                    var microwaveService = entities[0].GetColumnValue("StIsMicrowaveServiceSelected");
                    isMicrowaveService = (bool)microwaveService== true ? true : false;
                }
            }
            return isMicrowaveService;
        }


    }
}
