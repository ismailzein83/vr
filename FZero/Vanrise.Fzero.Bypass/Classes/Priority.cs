using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class Priority
    {
        public static List<Priority> GetPriorities()
           
        {
            List<Priority> PrioritysList = new List<Priority>();

            try
            {
                using (Entities context = new Entities())
                {
                    PrioritysList = context.Priorities
                                            .OrderByDescending(u => u.Name)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Priority.Priorities()", err);
            }

            return PrioritysList;
        }
    }
}
