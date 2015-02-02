using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;


namespace Vanrise.Fzero.Bypass
{
    public partial class prVwGeneratedCall_Result
    {
        public static prVwGeneratedCall_Result View(int ID)
        {
            prVwGeneratedCall_Result prVwGeneratedCall_Result = new prVwGeneratedCall_Result();
            try
            {
                using (Entities context = new Entities())
                {
                    prVwGeneratedCall_Result = context.prVwGeneratedCall(ID).ToList()
                       .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.prVwGeneratedCall_Result.View(" + ID.ToString() + ")", err);
            }
            return prVwGeneratedCall_Result;
        }
    }
}
