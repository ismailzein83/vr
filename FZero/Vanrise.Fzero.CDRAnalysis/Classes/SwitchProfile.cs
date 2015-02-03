using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;




namespace Vanrise.Fzero.CDRAnalysis
{
    public partial class SwitchProfile
    {

        public string DatabaseConnection { get { return Switch_DatabaseConnections.ConnectionString; } }

        public static SwitchProfile Load(int id)
        {
            SwitchProfile switchProfile = new SwitchProfile();
            try 
            {
                using (Entities context = new Entities())
                {
                    switchProfile = context.SwitchProfiles
                        .Include(s => s.Switch_DatabaseConnections)
                        .Where(s => s.Id == id)
                        .FirstOrDefault();
                }
            }
            catch(Exception err)
            {
                FileLogger.Write("DataLayer.SwitchProfile.Load(" + id + ")", err);
            }
            return switchProfile;
        }

        public static List<SwitchProfile> GetAll()
        {
            List<SwitchProfile> switches = new List<SwitchProfile>();
            try
            {
                using (Entities context = new Entities())
                {
                    switches = context.SwitchProfiles
                        .Include(s => s.Switch_DatabaseConnections)
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.SwitchProfile.GetAll()", err);
            }
            return switches;
        }

        public static List<SwitchProfile> GetList(string name, string areaCode, string type)
        {
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(areaCode) && string.IsNullOrWhiteSpace(type))
                return GetAll();

            List<SwitchProfile> switches = new List<SwitchProfile>();
            try
            {
                using (Entities context = new Entities())
                {

                     //switches = context.SwitchProfiles
                     //   .Include(s => s.Switch_DatabaseConnection)
                     //   .Where(s =>
                     //       (s.Name.Contains(name) || s.FullName.Contains(name))
                     //       ||
                     //       (s.SwitchType.Contains(type))
                     //       ||
                     //       (s.AreaCode == areaCode)
                     //   )
                     //   .ToList();

                    
                        var query = context.SwitchProfiles
                        .Include(s => s.Switch_DatabaseConnections);

                         if (!string.IsNullOrWhiteSpace(name))
                         {
                            query = query.Where(s => (s.Name.Contains(name) || s.FullName.Contains(name)));

                         }

                         if (!string.IsNullOrWhiteSpace(areaCode) )
                         {
                             query = query.Where(s => s.AreaCode == areaCode);
                         }

                         if (!string.IsNullOrWhiteSpace(type))
                         {
                             query = query.Where(s => s.SwitchType.Contains(type));
                         }

                        switches=query.ToList();
                   


                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.SwitchProfile.GetList()", err);
            }
            return switches;
        }

        public static bool IsFullNameUsed(SwitchProfile switchProfile)
        {
            bool isUsed = false;
            try
            {
                using (Entities context = new Entities())
                {
                    isUsed = context.SwitchProfiles
                        .Where(p => p.FullName == switchProfile.FullName 
                            && p.Id != switchProfile.Id)
                        .Count() > 0;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.SwitchProfile.IsFullNameUnique(Id: " + switchProfile.Id + ", FullName: " + switchProfile.FullName + ")", err);
            }
            return isUsed;
        }

        public static bool Save(SwitchProfile switchProfile)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    if (switchProfile.Id == 0)
                    {
                        context.SwitchProfiles.Add(switchProfile);
                    }
                    else
                    {
                        context.Entry(switchProfile).State = System.Data.EntityState.Modified;
                        context.Entry(switchProfile.Switch_DatabaseConnections).State = System.Data.EntityState.Modified;
                    }
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.SwitchProfile.Save(Id: " + switchProfile.Id + ")", err);
            }
            return success;
        }

        public static bool Delete(int id)
        {
            SwitchProfile profile = new SwitchProfile() { Id = id};
            return Delete(profile);
        }

        private static bool Delete(SwitchProfile switchProfile)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {

                    SwitchProfile sp = SwitchProfile.Load(switchProfile.Id);
                    if (sp.NormalizationRules.Count() > 0 || sp.SwitchTruncks.Count() > 0)
                    {
                        success=false;
                    }


                    context.Entry(switchProfile).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                    success = true;
                }
            }
            catch(Exception err)
            {
                FileLogger.Write("DataLayer.SwitchProfile.Delete(Id: " + switchProfile.Id + ")", err);
            }
            return success;
        }

        public static SwitchProfile Load(string Name)
        {
            SwitchProfile switchProfile = new SwitchProfile();
            try
            {
                using (Entities context = new Entities())
                {
                    switchProfile = context.SwitchProfiles
                        .Where(s => s.Name == Name)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.SwitchProfile.Load(" + Name + ")", err);
            }
            return switchProfile;
        }
        
    }
}