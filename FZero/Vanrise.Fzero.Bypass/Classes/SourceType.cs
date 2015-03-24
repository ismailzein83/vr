using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class SourceType
    {
        public static List<SourceType> GetSourceTypes()
           
        {
            List<SourceType> SourceTypesList = new List<SourceType>();

            try
            {
                using (Entities context = new Entities())
                {
                    SourceTypesList = context.SourceTypes
                                            .OrderByDescending(u => u.Name)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Source.GetSourceTypes()", err);
            }

            return SourceTypesList;
        }
    }
}
