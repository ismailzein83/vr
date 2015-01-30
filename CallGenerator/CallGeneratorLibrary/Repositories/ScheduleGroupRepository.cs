using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Repositories
{
    public class ScheduleGroupRepository
    {
        public static ScheduleGroup Load(int ScheduleGroupId)
        {
            ScheduleGroup log = new ScheduleGroup();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.ScheduleGroups.Where(l => l.Id == ScheduleGroupId).FirstOrDefault<ScheduleGroup>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return log;
        }

        public static ScheduleGroup LoadGroup(int ScheduleId)
        {
            ScheduleGroup log = new ScheduleGroup();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.ScheduleGroups.Where(l => l.ScheduleId == ScheduleId).FirstOrDefault<ScheduleGroup>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return log;
        }

        public static List<ScheduleNumbers> GetScheduleNumbers(int ScheduleId)
        {
            List<ScheduleNumbers> LstScheduleNumbers = new List<ScheduleNumbers>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstScheduleNumbers = context.GetScheduleNumbers1(ScheduleId).GetResult<ScheduleNumbers>().ToList<ScheduleNumbers>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return LstScheduleNumbers;
        }

        public static bool Save(ScheduleGroup GenCall)
        {
            bool success = false;
            if (GenCall.Id == default(int))
                success = Insert(GenCall);
            else
                success = Update(GenCall);
            return success;
        }

        public static bool Insert(ScheduleGroup GenCall)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.ScheduleGroups.InsertOnSubmit(GenCall);
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

        private static bool Update(ScheduleGroup GenCall)
        {
            bool success = false;
            ScheduleGroup look = new ScheduleGroup();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.ScheduleGroups.Single(l => l.Id == GenCall.Id);

                    look.ScheduleId = GenCall.ScheduleId;
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
                    ScheduleGroup GenCall = context.ScheduleGroups.Where(u => u.Id == Id).Single<ScheduleGroup>();
                    context.ScheduleGroups.DeleteOnSubmit(GenCall);
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
