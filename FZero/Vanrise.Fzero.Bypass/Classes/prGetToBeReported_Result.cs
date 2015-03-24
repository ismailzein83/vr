using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class prGetToBeReported_Result
    {
        public static List<prGetToBeReported_Result> GetToBeReportedCalls(int SourceID, int ReceivedSourceID, int CLIMobileOperatorID, int B_NumberMobileOperatorID, int StatusID, int ClientID)
           
        {
            try
            {
                using (Entities context = new Entities())
                {
                    return context.prGetToBeReported(B_NumberMobileOperatorID,CLIMobileOperatorID,SourceID,  ReceivedSourceID, StatusID,ClientID).ToList(); 
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ViewSourceRecieve.GetViewSourceRecieves()", err);
            }

            return null;
        }
    }
}
