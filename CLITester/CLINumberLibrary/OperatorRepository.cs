using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLINumberLibrary
{
    public class OperatorRepository
    {
        public static Operator Load(int OperatorId)
        {
            Operator log = new Operator();

            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    log = context.Operators.Where(l => l.Id == OperatorId).FirstOrDefault<Operator>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return log;
        }

        public static Operator Load(string mcc, string mnc)
        {
            Operator operatorObj = new Operator();

            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    operatorObj = context.Operators.Where(l => l.mcc  == mcc && l.mnc == mnc).FirstOrDefault<Operator>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return operatorObj;
        }

        public static List<Operator> GetOperators()
        {
            List<Operator> LstOperators = new List<Operator>();
            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    LstOperators = context.Operators.OrderBy(x => x.Country).ToList<Operator>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return LstOperators;
        }

        public static bool Delete(int operatorId)
        {
            bool success = false;

            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    Operator ooperator = context.Operators.Where(u => u.Id == operatorId).Single<Operator>();
                    context.Operators.DeleteOnSubmit(ooperator);
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

        public static bool Save(Operator oper)
        {
            bool success = false;
            if (oper.Id == default(int))
                success = Insert(oper);
            else
                success = Update(oper);
            return success;
        }

        private static bool Insert(Operator oper)
        {
            bool success = false;
            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    context.Operators.InsertOnSubmit(oper);
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

        private static bool Update(Operator oper)
        {
            bool success = false;
            Operator look = new Operator();

            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    look = context.Operators.Single(l => l.Id == oper.Id);

                    look.Name = oper.Name;
                    look.mcc = oper.mcc;
                    look.mnc = oper.mnc;
                    look.Country = oper.Country;
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
