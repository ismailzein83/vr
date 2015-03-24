using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class GMT
    {
        public static List<GMT> GetGMTs()
           
        {
            List<GMT> GMTsList = new List<GMT>();

            try
            {
                using (Entities context = new Entities())
                {
                    GMTsList = context.GMTs
                                            .OrderByDescending(u => u.Value)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GMT.GetGMTs()", err);
            }

            return GMTsList;
        }
    }
}
