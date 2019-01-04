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
    public class LeasedLineTerminationFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "E89F9048-3ADB-413F-BEA3-B3962A37BBA4";
        const string printStep = "AFFBDD5C-AA1A-4AEB-A924-ABC5864A8FF8";
        const string reasonStep = "C684A324-664E-4893-B03D-B5EDCD10B788";
        const string billOnDemandStep = "9F42B63F-0677-43A4-A8C1-A3D025EA9B9A";

        const string site1FiberTeamStep = "0BE3AA25-8841-4FFB-A7E9-04C7065DE5CA";
        const string site1MDFTeamStep = "B237EC89-06A8-4B1D-8B92-C69156D9931D";
        const string site1CabinetTeamStep = "08D60CAA-817B-41BB-90A6-FF499A14DA13";
        const string site1DPTeamStep = "19AC8301-D58B-48F1-87BE-61F5C971436B";

        const string micStep = "1A2EFE3F-99D9-4D28-B557-3600FCF49B78";

        const string site2FiberTeamStep = "514041BD-BDC9-47DB-9278-29CBF9492AB8";
        const string site2MDFTeamStep = "FB1836A1-D0E6-4D37-B21E-E0B652618794";
        const string site2CabinetTeamStep = "C1CA0E2F-672E-40BC-8064-2BBB4AA005F5";
        const string site2DPTeamStep = "613D0BBA-C9DA-43FE-9293-0A30C9938202";

        const string completedStep = "94CA2192-D445-47D8-80C5-BF26E462CFC6";

        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = printStep; break;
                case printStep: nextStepId = reasonStep; break;
                case reasonStep: nextStepId = NBOfActiveContracts(id) == "1" ? billOnDemandStep : site1MDFTeamStep; break;
                case billOnDemandStep: nextStepId = site1FiberTeamStep; break;
                case site1FiberTeamStep: nextStepId = site1MDFTeamStep; break;
                case site1MDFTeamStep: nextStepId = site1CabinetTeamStep; break;
                case site1CabinetTeamStep: nextStepId = site1DPTeamStep; break;
                case site1DPTeamStep: nextStepId = GetFirstMic(id) != null ? micStep : site2MDFTeamStep; break;
                case micStep: nextStepId = GetNextMic(id) != null ? micStep:site2FiberTeamStep; break;
                case site2FiberTeamStep: nextStepId = site2MDFTeamStep; break;
                case site2MDFTeamStep: nextStepId = site2CabinetTeamStep; break;
                case site2CabinetTeamStep: nextStepId = site2DPTeamStep; break;
                case site2DPTeamStep: nextStepId = completedStep; break;
            }
            return nextStepId;
        }


        public string NBOfActiveContracts(string id)
        {

            //TODO : Get the count of active contracts for this customer (customerId)

            Random gen = new Random();
            int prob = gen.Next(10);
            return prob <= 6 ? "1" : "4";
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
                    var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLeasedLineTermination");
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
                        var update = new Update(BPM_UserConnection, "StLeasedLineTermination").Set("StMICSwitchName", Column.Parameter(currentmic.SwitchName))
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
                    var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StLeasedLineTermination");
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
                        var update = new Update(BPM_UserConnection, "StLeasedLineTermination").Set("StMICSwitchName", Column.Parameter(nextmic.SwitchName))
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
