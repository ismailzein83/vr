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
                    LstOperators = context.TestNumberGroups.Where(x => (x.IsDeleted == null || x.IsDeleted == false)).ToList<TestNumberGroup>();
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
        public static TestNumberGroup Load(string Name)
        {
            TestNumberGroup log = new TestNumberGroup();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.TestNumberGroups.Where(l => l.Name == Name).FirstOrDefault<TestNumberGroup>();
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

        private static bool Update(TestNumberGroup testNumberGroup)
        {
            bool success = false;
            TestNumberGroup testNumberGroupObj = new TestNumberGroup();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    testNumberGroupObj = context.TestNumberGroups.Single(l => l.Id == testNumberGroup.Id);

                    testNumberGroupObj.Name = testNumberGroup.Name;
                    testNumberGroupObj.Code = testNumberGroup.Code;
                    testNumberGroupObj.IsDeleted = testNumberGroup.IsDeleted;
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
                    TestNumberGroup testNumberGroup = context.TestNumberGroups.Where(u => u.Id == Id).Single<TestNumberGroup>();
                    testNumberGroup.IsDeleted = true;
                    return Update(testNumberGroup);
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
