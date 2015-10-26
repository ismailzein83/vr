using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Diagnostics;

namespace CallGeneratorLibrary.Repositories
{
    public class UserBalanceRepository
    {
        public static List<UserBalance> GetUserBalances(int userId)
        {
            List<UserBalance> users = new List<UserBalance>();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    users = context.UserBalances.Where(l => l.UserId == userId).OrderByDescending(l => l.Id).Take(10).ToList();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return users;
        }

        public static void Save(UserBalance UserBalance)
        {
            if (UserBalance.Id == default(int))
                Insert(UserBalance);
            else
                Update(UserBalance);
        }

        private static void Insert(UserBalance UserBalance)
        {
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.UserBalances.InsertOnSubmit(UserBalance);
                    context.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private static void Update(UserBalance UserBalance)
        {
            UserBalance _UserBalance = new UserBalance();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    _UserBalance = context.UserBalances.Single(ul => ul.Id == UserBalance.Id);
                    _UserBalance.UserId = UserBalance.UserId;
                    _UserBalance.OldBalance = UserBalance.OldBalance;
                    _UserBalance.CountCalls = UserBalance.CountCalls;
                    _UserBalance.CreationDate = UserBalance.CreationDate;
                    context.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
