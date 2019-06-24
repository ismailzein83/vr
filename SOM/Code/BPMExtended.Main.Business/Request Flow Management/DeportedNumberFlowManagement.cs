using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Entities;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class DeportedNumberFlowManagement
    {

        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string startingProcessStep = "C4509023-33CD-454F-A427-62B2538E947C";
        const string printStep = "6074BB55-EF09-4FFC-B633-D1902397AC2C";
        const string nearByNumberStep = "0EA15672-4E18-4A85-94C5-BD96ECC1154D";
        const string freeReservationStep = "17F57607-72D1-4155-8892-8C038A43029E";
        const string micReservationStep = "5DEC9AFF-87D8-497A-AABF-0F702D5D8C13";
        const string networkAdminStep = "8664DD40-00FC-43D5-A72D-6DC28A7F4FA4";
        const string paymentStep = "37858CF1-30F2-4AEA-9F48-1B3A28876679";
        const string attachmentStep = "B3287838-1DB8-493E-9C6D-5A0BDA21C99A";
        const string technicalStep = "F360681D-F055-49AB-A04B-6A1913548B3A";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case startingProcessStep: nextStepId = nearByNumberStep; break;
                case nearByNumberStep: nextStepId = freeReservationStep; break;
                case freeReservationStep: nextStepId = technicalStep; break;
                case micReservationStep: nextStepId = paymentStep; break;
                case paymentStep: nextStepId = printStep; break;
                case printStep: nextStepId = attachmentStep; break;
                case attachmentStep: nextStepId = technicalStep; break;
            }
            return nextStepId;
        }


        public MIC GetFirstMic(string id)
        {
            //TODO: get next mic
            MIC currentmic = null;
            try
            {
                if (id != "")
                {
                    //get request from bpm
                    Guid idd = new Guid(id.ToUpper());
                    // Creation of query instance with "City" root schema. 
                    var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StDeportedNumber");
                    esq.AddColumn("Id");
                    esq.AddColumn("StPhoneNumber");
                    // Creation of the first filter instance.
                    var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);
                    // Adding created filters to query collection. 
                    esq.Filters.Add(esqFirstFilter);
                    // Objects, i.e. query results, filtered by two filters, will be included into this collection.
                    var entities = esq.GetEntityCollection(BPM_UserConnection);
                    if (entities.Count > 0)
                    {
                        var phoneNumber = entities[0].GetColumnValue("StPhoneNumber");
                        currentmic = new MicsManager().GetMics(phoneNumber.ToString()).First();

                        //update mic switch name
                        var update = new Update(BPM_UserConnection, "StDeportedNumber").Set("StMICSwitchName", Column.Parameter(currentmic.SwitchName))
                            .Where("Id").IsEqual(Column.Parameter(id));
                        update.Execute();
                    }
                }

            }
            catch
            {
            }
            return currentmic;
        }

        public MIC GetNextMic(string id)
        {
            MIC nextmic = null;

            //TODO: get next mic
            try
            {

                if (id != "")
                {
                    //get request from bpm
                    Guid idd = new Guid(id.ToUpper());
                    // Creation of query instance with "City" root schema. 
                    var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StDeportedNumber");
                    esq.AddColumn("Id");
                    esq.AddColumn("StPhoneNumber");
                    esq.AddColumn("StSwitch");
                    // Creation of the first filter instance.
                    var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);
                    // Adding created filters to query collection. 
                    esq.Filters.Add(esqFirstFilter);
                    // Objects, i.e. query results, filtered by two filters, will be included into this collection.
                    var entities = esq.GetEntityCollection(BPM_UserConnection);
                    if (entities.Count > 0)
                    {
                        var phoneNumber = entities[0].GetColumnValue("StPhoneNumber");
                        var switchName = entities[0].GetColumnValue("StSwitch");
                        List<MIC> allmics = new MicsManager().GetMics(phoneNumber.ToString());
                        MIC currentmic = allmics.Where(x => x.SwitchName == switchName.ToString()).FirstOrDefault();
                        nextmic = allmics[currentmic.MicNumber];

                        //update mic switch name
                        var update = new Update(BPM_UserConnection, "StDeportedNumber").Set("StMICSwitchName", Column.Parameter(nextmic.SwitchName))
                            .Where("Id").IsEqual(Column.Parameter(id));
                        update.Execute();

                    }
                }

            }
            catch
            {
            }

            return nextmic;
        }

    }
}
