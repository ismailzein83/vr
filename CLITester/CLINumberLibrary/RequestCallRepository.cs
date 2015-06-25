using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLINumberLibrary
{
    public class RequestCallRepository
    {
        public static RequestCall Load(int requestCallId)
        {
            RequestCall log = new RequestCall();

            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    log = context.RequestCalls.Where(l => l.Id == requestCallId).FirstOrDefault<RequestCall>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return log;
        }

        public static List<RequestCall> GetRequestCalls()
        {
            List<RequestCall> LstRequestCalls = new List<RequestCall>();
            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<RequestCall>(c => c.Operator);
                    options.LoadWith<RequestCall>(c => c.PhoneNumber);
                    options.LoadWith<RequestCall>(c => c.Client);
                    context.LoadOptions = options;

                    LstRequestCalls = context.RequestCalls.OrderBy(x => x.CreationDate).ToList<RequestCall>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return LstRequestCalls;
        }

        public static bool Save(RequestCall requestCallObj)
        {
            bool success = false;
            if (requestCallObj.Id == default(int))
                success = Insert(requestCallObj);
            else
                success = Update(requestCallObj);
            return success;
        }

        private static bool Insert(RequestCall requestCallObj)
        {
            bool success = false;
            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    context.RequestCalls.InsertOnSubmit(requestCallObj);
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

        private static bool Update(RequestCall requestCallObj)
        {
            bool success = false;
            RequestCall requestCall = new RequestCall();

            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    requestCall = context.RequestCalls.Single(l => l.Id == requestCallObj.Id);

                    requestCall.CreationDate = requestCallObj.CreationDate;
                    requestCall.ReleaseDate = requestCallObj.ReleaseDate;
                    requestCall.OperatorId = requestCallObj.OperatorId;
                    requestCall.PhoneNumberId = requestCallObj.PhoneNumberId;
                    requestCall.ClientId = requestCallObj.ClientId;
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

        public static bool Delete(int requestCallId)
        {
            bool success = false;

            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    RequestCall phoneNumber = context.RequestCalls.Where(u => u.Id == requestCallId).Single<RequestCall>();
                    context.RequestCalls.DeleteOnSubmit(phoneNumber);
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

        public static List<RequestCallHistory> GetRequestCallHistory(DateTime? StartDate, DateTime? ReleaseDate, int? OperatorId, int? DisplayStart, int? DisplayLength)
        {
            List<RequestCallHistory> lstReqCallsHist= new List<RequestCallHistory>();
            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    lstReqCallsHist = context.GetRequestCalls1(StartDate, ReleaseDate, OperatorId, DisplayStart, DisplayLength).GetResult<RequestCallHistory>().ToList<RequestCallHistory>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return lstReqCallsHist;
        }
    }
}
