using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.CDRAnalysis
{
    public partial class SourceKind
    {
        public static List<SourceKind> GetAll()
        {
            List<SourceKind> SourceKindsList = new List<SourceKind>();

            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    SourceKindsList = context.SourceKinds
                                            .OrderByDescending(u => u.Name)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.SourceKind.GetSourceKinds()", err);
            }

            return SourceKindsList;
        }
    }
}
