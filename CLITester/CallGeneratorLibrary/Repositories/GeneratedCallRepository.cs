using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;

namespace CallGeneratorLibrary.Repositories
{
    public class GeneratedCallRepository
    {
        public static GeneratedCall Load(int GeneratedCallId)
        {
            GeneratedCall log = new GeneratedCall();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<GeneratedCall>(c => c.SipAccount);
                    options.LoadWith<SipAccount>(c => c.User);
                    context.LoadOptions = options;

                    log = context.GeneratedCalls.Where(l => l.Id == GeneratedCallId).FirstOrDefault<GeneratedCall>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return log;
        }

        public static GeneratedCall GetTopGeneratedCall(int SipAccountId)
        {
            GeneratedCall GenCall = new GeneratedCall();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<GeneratedCall>(c => c.SipAccount);
                    context.LoadOptions = options;

                    GenCall = context.GeneratedCalls.Where(l => (l.EndDate == null) && (l.Status == null) && (l.SipAccountId == SipAccountId)).FirstOrDefault<GeneratedCall>();

                    if (GenCall != null)
                    {
                        GenCall.Status = "1";
                        GenCall.StartDate = DateTime.Now;
                        context.SubmitChanges();
                    }
                }
            }
            catch (System.Exception ex)
            {
                
                Logger.LogException(ex);
            }
            return GenCall;
        }

        public static bool Save(GeneratedCall GenCall)
        {
            bool success = false;
            if (GenCall.Id == default(int))
                success = Insert(GenCall);
            else
                success = Update(GenCall);
            return success;
        }

        public static bool Insert(GeneratedCall GenCall)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.GeneratedCalls.InsertOnSubmit(GenCall);
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

        private static bool Update(GeneratedCall GenCall)
        {
            bool success = false;
            GeneratedCall look = new GeneratedCall();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.GeneratedCalls.Single(l => l.Id == GenCall.Id);

                    look.Number = GenCall.Number;
                    look.StartDate = GenCall.StartDate;
                    look.StartCall = GenCall.StartCall;
                    look.EndDate = GenCall.EndDate;
                    look.Status = GenCall.Status;
                    look.ResponseCode = GenCall.ResponseCode;
                    look.SipAccountId = GenCall.SipAccountId;
                    look.ScheduleId = GenCall.ScheduleId;
                  
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
                    GeneratedCall GenCall = context.GeneratedCalls.Where(u => u.Id == Id).Single<GeneratedCall>();
                    context.GeneratedCalls.DeleteOnSubmit(GenCall);
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
