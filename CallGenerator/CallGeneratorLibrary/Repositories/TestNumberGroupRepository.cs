using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static TestNumberGroup Load(int TestNumberGroupId)
        {
            TestNumberGroup log = new TestNumberGroup();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.TestNumberGroups.Where(l => l.Id == TestNumberGroupId).FirstOrDefault<TestNumberGroup>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return log;
        }

        public static bool Save(TestNumberGroup GenCall)
        {
            bool success = false;
            if (GenCall.Id == default(int))
                success = Insert(GenCall);
            else
                success = Update(GenCall);
            return success;
        }

        public static bool Insert(TestNumberGroup GenCall)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.TestNumberGroups.InsertOnSubmit(GenCall);
                    context.SubmitChanges();
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return success;
        }

        private static bool Update(TestNumberGroup GenCall)
        {
            bool success = false;
            TestNumberGroup look = new TestNumberGroup();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.TestNumberGroups.Single(l => l.Id == GenCall.Id);

                    look.Name = GenCall.Name;
                    look.Code = GenCall.Code;

                    context.SubmitChanges();
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return success;
        }

        public static bool Delete(int Id)
        {
            bool success = false;

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    TestNumberGroup GenCall = context.TestNumberGroups.Where(u => u.Id == Id).Single<TestNumberGroup>();
                    context.TestNumberGroups.DeleteOnSubmit(GenCall);
                    context.SubmitChanges();
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return success;
        }
    }
}
