using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;


namespace Vanrise.Fzero.Bypass
{
    public partial class RecievedCall
    {
        public static bool SaveBulk(string tableName,   List<RecievedCall> listRecievedCalls)
        {
            bool success = false;
            try
            {
                Manager.InsertData(listRecievedCalls.ToList(), tableName, "FMSConnectionString");
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.RecievedCall.SaveBulk(" + listRecievedCalls.Count.ToString() + ")", err);
            }
            return success;
        }
    }
}
