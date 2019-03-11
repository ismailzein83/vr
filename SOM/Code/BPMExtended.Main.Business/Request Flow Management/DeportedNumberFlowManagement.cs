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

        string welcomeStep = "C4509023-33CD-454F-A427-62B2538E947C";
        string printStep = "6074BB55-EF09-4FFC-B633-D1902397AC2C";
        string nearByNumberStep = "0EA15672-4E18-4A85-94C5-BD96ECC1154D";
        string reservationStep = "17F57607-72D1-4155-8892-8C038A43029E";
        string networkAdminStep = "8664DD40-00FC-43D5-A72D-6DC28A7F4FA4";
        string technicalStep = "F360681D-F055-49AB-A04B-6A1913548B3A";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId.ToLower())
            {
                case "c4509023-33cd-454f-a427-62b2538e947c": nextStepId = printStep; break;
                case "6074bb55-ef09-4ffc-b633-d1902397ac2c": nextStepId = nearByNumberStep; break;
                case "0ea15672-4e18-4a85-94c5-bd96ecc1154d": nextStepId = reservationStep; break;
                case "17f57607-72d1-4155-8892-8c038a43029e": nextStepId = technicalStep; break;
                    //case "17f57607-72d1-4155-8892-8c038a43029e": nextStepId = networkAdminStep; break;
                    // case "a7fb4f5e-5d2c-4259-8be4-e6506c8e1dc3": nextStepId = GetFirstMic(id) != null ? micTeamStep : newMDFStep; break;
                    //case "6bd597b3-54a4-4b51-b500-ffaeb7d2224b": nextStepId = GetNextMic(id) !=null ? micTeamStep : newMDFStep; break;
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
