using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.MobileCDRAnalysis


{
    public partial class Criteria_Profile
    {

        public static Criteria_Profile Load(int id)
        {
            Criteria_Profile criteria_profile = new Criteria_Profile();
            try
            {
                using (MobileEntities context = new MobileEntities())
                {

                    criteria_profile = context.Criteria_Profile
                        .Where(s => s.Id == id)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Criteria_profile.Load(" + id + ")", err);
            }
            return criteria_profile;
        }

        //----------------------------------------

        public static List<Criteria_Profile> GetList(int id, string description_)
        {
            if (id == 0 && string.IsNullOrWhiteSpace(description_))
                return GetAll();

            List<Criteria_Profile> criteria_profiles = new List<Criteria_Profile>();
            try
            {
                using (MobileEntities context = new MobileEntities())
                {
                    criteria_profiles = context.Criteria_Profile
                        //.Include(s => s.Switch_DatabaseConnection)
                        .Where(s =>
                            (s.Id==id  && s.Description.Contains(description_))


                        )
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Criteria_profiles.GetList()", err);
            }
            return criteria_profiles;
        }

        //----------------------------------------

        public static List<Criteria_Profile> GetAll()
        {
            List<Criteria_Profile> criteria_profiles = new List<Criteria_Profile>();
            try
            {
                using (MobileEntities context = new MobileEntities())
                {
                    criteria_profiles = context.Criteria_Profile
                       .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Criteria_profiles.GetAll()", err);
            }
            return criteria_profiles;
        }

        //----------------------------------------









    }
}
