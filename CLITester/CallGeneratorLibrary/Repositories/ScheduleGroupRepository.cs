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
    public class ScheduleGroupRepository
    {
        public static ScheduleGroup Load(int ScheduleId)
        {
            ScheduleGroup log = new ScheduleGroup();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.ScheduleGroups.Where(l => l.Id == ScheduleId).FirstOrDefault<ScheduleGroup>();
                }
            }
            catch (System.Exception ex)
            {
              
                Logger.LogException(ex);
            }

            return log;
        }


        public static bool Save(ScheduleGroup ScheduleGroup)
        {
            bool success = false;
            if (ScheduleGroup.Id == default(int))
                success = Insert(ScheduleGroup);
            else
                success = Update(ScheduleGroup);
            return success;
        }

        private static bool Insert(ScheduleGroup ScheduleGroup)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.ScheduleGroups.InsertOnSubmit(ScheduleGroup);
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

        private static bool Update(ScheduleGroup ScheduleGroup)
        {
            bool success = false;
            ScheduleGroup look = new ScheduleGroup();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.ScheduleGroups.Single(l => l.Id == ScheduleGroup.Id);

                    look.ScheduleId = ScheduleGroup.ScheduleId;
                    look.GroupId = ScheduleGroup.GroupId;
                   
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
