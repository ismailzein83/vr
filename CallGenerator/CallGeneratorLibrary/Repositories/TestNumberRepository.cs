using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Repositories
{
    public class TestNumberRepository
    {
        public static TestNumber Load(int TestNumberId)
        {
            TestNumber log = new TestNumber();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.TestNumbers.Where(l => l.Id == TestNumberId).FirstOrDefault<TestNumber>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return log;
        }

        public static TestNumber LoadbyNumber(string number)
        {
            TestNumber log = new TestNumber();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.TestNumbers.Where(l => l.Number == number).FirstOrDefault<TestNumber>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return log;
        }

        public static List<TestNumber> GetTestNumberbyGroup(int GroupId)
        {
            List<TestNumber> LstTestNumbers = new List<TestNumber>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstTestNumbers = context.TestNumbers.Where(l => (l.GroupId == GroupId)).ToList<TestNumber>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return LstTestNumbers;
        }


        public static bool Save(TestNumber GenCall)
        {
            bool success = false;
            if (GenCall.Id == default(int))
                success = Insert(GenCall);
            else
                success = Update(GenCall);
            return success;
        }

        public static bool Insert(TestNumber GenCall)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.TestNumbers.InsertOnSubmit(GenCall);
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

        private static bool Update(TestNumber GenCall)
        {
            bool success = false;
            TestNumber look = new TestNumber();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.TestNumbers.Single(l => l.Id == GenCall.Id);

                    look.Number = GenCall.Number;
                    look.GroupId = GenCall.GroupId;

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
                    TestNumber GenCall = context.TestNumbers.Where(u => u.Id == Id).Single<TestNumber>();
                    context.TestNumbers.DeleteOnSubmit(GenCall);
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
