using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class Status
    {
        public static List<Status> GetStatuses()
           
        {
            List<Status> StatussList = new List<Status>();

            try
            {
                using (Entities context = new Entities())
                {
                    StatussList = context.Statuses
                                            .OrderByDescending(u => u.Name)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Status.GetStatuses()", err);
            }

            return StatussList;
        }
    }
}
