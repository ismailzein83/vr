using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class ApplicationUser
    {
        public static ApplicationUser LoadbyUserId(int userID)
        {
            ApplicationUser ApplicationUser = new ApplicationUser();
            try
            {
                using (Entities context = new Entities())
                {
                    ApplicationUser = context.ApplicationUsers
                     .Include(u => u.User)
                     .Where(u => u.UserID == userID)
                     .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ApplicationUser.Load(" + userID.ToString() + ")", err);
            }
            return ApplicationUser;
        }

        public static ApplicationUser Load(int ID)
        {
            ApplicationUser ApplicationUser = new ApplicationUser();
            try
            {
                using (Entities context = new Entities())
                {
                    ApplicationUser = context.ApplicationUsers
                     .Include(u => u.User)
                     .Include(u => u.User.UserPermissions)
                     .Where(u => u.ID == ID)
                     .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ApplicationUser.Load(" + ID.ToString() + ")", err);
            }
            return ApplicationUser;
        }

        public static List<ApplicationUser> GetApplicationUsers(string userName, string fullName, string emailAddress, string address, string mobileNumber, bool? isActive)
        {
            List<ApplicationUser> ApplicationUsersList= new List<ApplicationUser>();

            try
            {
              using (Entities context = new Entities())
                {
                    ApplicationUsersList = context.ApplicationUsers
                                      .Include(u => u.User)
                                       .Where(u => u.ID > 0
                                         && (u.User.FullName.Contains(fullName))
                                         && (u.User.EmailAddress.Contains(emailAddress))
                                         && (u.User.UserName.Contains(userName))
                                         && (!isActive.HasValue || u.User.IsActive == isActive)
                                         && (u.User.MobileNumber == null || (u.User.MobileNumber != null && u.User.MobileNumber.Contains(mobileNumber)))
                                        )
                                         .OrderBy(u => u.User.FullName)
                                         .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ApplicationUser.GetApplicationUsers(" + userName + ", " + fullName + ", " + emailAddress + ", " + address + ", " + mobileNumber + ", "  + isActive.ToString() + ")", err);
            }


            return ApplicationUsersList;
        }

        public static ApplicationUser Save(ApplicationUser ApplicationUser)
        {
            ApplicationUser CurrentApplicationUser = new ApplicationUser();
            try
            {
                using (Entities context = new Entities())
                {
                    if (ApplicationUser.ID == 0)
                    {
                        context.ApplicationUsers.Add(ApplicationUser);
                    }
                    else
                    {
                        User.Save(ApplicationUser.User);
                        ApplicationUser.User = null;
                        context.ApplicationUsers.Attach(ApplicationUser);
                        context.Entry(ApplicationUser).State = System.Data.EntityState.Modified;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ApplicationUser.Save(" + ApplicationUser.ID.ToString() + ")", err);
            }
            return CurrentApplicationUser;
        }

        public static bool ActivateDeactivate(int id)
        {
            bool success = false;
            User user = new User();
            ApplicationUser ApplicationUser = new ApplicationUser();
            try
            {
                using (Entities context = new Entities())
                {
                    ApplicationUser = ApplicationUser.Load(id);
                    ApplicationUser.User.IsActive = !ApplicationUser.User.IsActive;

                    context.Users.Attach(ApplicationUser.User);
                    context.Entry(ApplicationUser.User).State = System.Data.EntityState.Modified;

                    context.ApplicationUsers.Attach(ApplicationUser);
                    context.Entry(ApplicationUser).State = System.Data.EntityState.Modified;
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ApplicationUser.ActivateDeactivate(" + id.ToString() + ")", err);
            }
            return success;

        }
    }
}
