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
    public class TestNumberRepository
    {
        public static List<TestNumber> GetTestNumber(int GroupId)
        {
            List<TestNumber> LstOperators = new List<TestNumber>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstOperators = context.TestNumbers.Where(x => (x.GroupId.Value == GroupId) && x.IsDeleted == null).ToList<TestNumber>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return LstOperators;
        }


        public static bool Save(TestNumber TestNumber)
        {
            bool success = false;
            if (TestNumber.Id == default(int))
                success = Insert(TestNumber);
            else
                success = Update(TestNumber);
            return success;
        }

        private static bool Insert(TestNumber TestNumber)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.TestNumbers.InsertOnSubmit(TestNumber);
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

        private static bool Update(TestNumber TestNumber)
        {
            bool success = false;
            TestNumber look = new TestNumber();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.TestNumbers.Single(l => l.Id == TestNumber.Id);

                    look.Number = TestNumber.Number;
                    look.GroupId = TestNumber.GroupId;
                    look.CreatedBy = TestNumber.CreatedBy;
                    look.CreationDate = TestNumber.CreationDate;
                    look.DeletedBy = TestNumber.DeletedBy;
                    look.DeletionDate = TestNumber.DeletionDate;
                    look.IsDeleted = TestNumber.IsDeleted;
                    look.Source = TestNumber.Source;
                    look.PrefixId = TestNumber.PrefixId;
                    look.CounterUsage = TestNumber.CounterUsage;
                    look.SuccessfulAttempts = TestNumber.SuccessfulAttempts;
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

        public static bool Delete(int NumberId)
        {
            bool success = false;

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    TestNumber carrier = context.TestNumbers.Where(u => u.Id == NumberId).Single<TestNumber>();
                    context.TestNumbers.DeleteOnSubmit(carrier);
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
