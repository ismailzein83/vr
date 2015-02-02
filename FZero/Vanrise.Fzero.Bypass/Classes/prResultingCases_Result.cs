using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class prResultingCases_Result
    {
        public static List<prResultingCases_Result> prResultingCases
            (string caseID, Nullable<int> sourceID, Nullable<int> receivedSourceID, Nullable<int> mobileOperatorID, Nullable<int> statusID, Nullable<int> priorityID, Nullable<int> reportingStatusID, string a_number, string b_number, string cLI, string originationNetwork, Nullable<System.DateTime> fromAttemptDateTime, Nullable<System.DateTime> toAttemptDateTime, Nullable<int> clientID, string cliReported)
        {
            try
            {
                using (Entities context = new Entities())
                {
                    List<prResultingCases_Result> ListResultingCases=context.prResultingCases( caseID,  sourceID,   receivedSourceID,   mobileOperatorID,   statusID,   priorityID,   reportingStatusID,  a_number,  b_number,  cLI,  originationNetwork,  fromAttemptDateTime, toAttemptDateTime,   clientID).ToList();

                    List<string> CLIs = context.vwCLIs.Select(x=>x.CLI).ToList();


                    foreach (var i in ListResultingCases)
                    {
                        if (CLIs.Contains(i.CLI))
                        {
                            i.CLIReported = "true";
                        }
                    }

                    return ListResultingCases.Where(x => (x.CLIReported == cliReported || cliReported==string.Empty)).ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.prResultingCases_Result.prResultingCases()", err);
            }

            return null;
        }
    }
}
