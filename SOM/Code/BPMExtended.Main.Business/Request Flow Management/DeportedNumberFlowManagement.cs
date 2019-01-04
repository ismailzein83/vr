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
        string oldMDFStep = "51048372-093E-4FC4-831D-469ACD98C500";
        string oldCabinetStep = "AE5C7F87-D2A0-48DB-ADF4-20402ED023A3";
        string oldDPStep = "A7FB4F5E-5D2C-4259-8BE4-E6506C8E1DC3";
        string micTeamStep = "6BD597B3-54A4-4B51-B500-FFAEB7D2224B";
        string newMDFStep = "BB3B210E-B1E1-46DD-9E8E-3C3EF6F00D86";
        string newCabinetStep = "342AA912-73D5-4FD9-A309-1A8C5A541A80";
        string newDPStep = "56C0D285-4C99-4090-B3BF-DD7182F96E89";
        string completedStep = "5442DDB1-82C0-4763-9BE8-DB8C6AD58B7A";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId.ToLower())
            {
                case "c4509023-33cd-454f-a427-62b2538e947c": nextStepId = printStep; break;
                case "6074bb55-ef09-4ffc-b633-d1902397ac2c": nextStepId = nearByNumberStep; break;
                case "0ea15672-4e18-4a85-94c5-bd96ecc1154d": nextStepId = reservationStep; break;
                case "17f57607-72d1-4155-8892-8c038a43029e": nextStepId = networkAdminStep; break;
                case "8664dd40-00fc-43d5-a72d-6dc28a7f4fa4": nextStepId = oldMDFStep; break;
                case "51048372-093e-4fc4-831d-469acd98c500": nextStepId = oldCabinetStep; break;
                case "ae5c7f87-d2a0-48db-adf4-20402ed023a3": nextStepId = oldDPStep; break;
                case "a7fb4f5e-5d2c-4259-8be4-e6506c8e1dc3": nextStepId = GetFirstMic(id) != null ? micTeamStep : newMDFStep; break;
                case "6bd597b3-54a4-4b51-b500-ffaeb7d2224b": nextStepId = GetNextMic(id) !=null ? micTeamStep : newMDFStep; break;
                case "bb3b210e-b1e1-46dd-9e8e-3c3ef6f00d86": nextStepId = newCabinetStep; break;
                case "342aa912-73d5-4fd9-a309-1a8c5a541a80": nextStepId = newDPStep; break;
                case "56c0d285-4c99-4090-b3bf-dd7182f96e89": nextStepId = completedStep; break;
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
