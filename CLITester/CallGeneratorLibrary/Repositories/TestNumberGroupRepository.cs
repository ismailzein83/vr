using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallGeneratorLibrary.Utilities;
using System.Diagnostics;

namespace CallGeneratorLibrary.Repositories
{
    public class TestNumberGroupRepository
    {
        public static List<TestNumberGroup> GetTestNumberGroups()
        {
            List<TestNumberGroup> LstOperators = new List<TestNumberGroup>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstOperators = context.TestNumberGroups.Where(x => x.IsDeleted == null).ToList<TestNumberGroup>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return LstOperators;
        }
    }
}
