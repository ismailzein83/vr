using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;



namespace Vanrise.Fzero.CDRAnalysis
{
    public partial class User
    {
        public static bool ActivateDeactivate(int id)
        {
            bool success = false;
            User user = new User();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    user = Load(id);
                    user.IsActive = !user.IsActive;

                    context.Users.Attach(user);
                    context.Entry(user).State = System.Data.EntityState.Modified;
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.User.ActivateDeactivate(" + id.ToString() + ")", err);
            }
            return success;

        }

        public static List<User> GetAllUsers()
        {
            List<User> UsersList = new List<User>();

            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    UsersList = context.Users
                        .OrderBy(x => x.FullName).ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.User.GetAllUsers()", err);
            }


            return UsersList;
        }

        public static User GetUser(string username, string password)
        {
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    return context.Users
                        .Include(u=>u.UserPermissions)
                        .Where(u => u.UserName == username 
                            && u.Password == password
                            && u.IsActive)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.User.GetUser(" + username + ")", err);
            }
            return null;
        }

        public static User Load(int ID)
        {
            User user = new User();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    user = context.Users
                        .Include(u => u.UserPermissions)
                        .Where(u => u.ID == ID)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.User.Load(" + ID.ToString() + ")", err);
            }
            return user;
        }

        public static int Save(User user)
        {
            int id = 0;
            try
            { 
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    if (user.ID == 0)
                    {
                        context.Users.Add(user);
                    }
                    else
                    {
                        context.Entry(user).State = System.Data.EntityState.Modified;
                    }
                    context.SaveChanges();
                    id = user.ID;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.User.Save(" + user.ID.ToString() + ")", err);
            }
            return id;
        }

        public static bool Delete(User user)
        {
            bool success = false;
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    context.Entry(user).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.User.Delete(" + user.ID.ToString() + ")", err);
            }
            return success;
        }

        public static bool Delete(int ID)
        {
            User user = new User();
            user.ID = ID;
            return Delete(user);
        }

        public static bool SaveLastLoginTime(User user)
        {
            bool success = false;
            try 
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    context.Users.Attach(user);
                    context.Entry(user).Property(u => u.LastLoginTime).IsModified = true;
                    context.SaveChanges();
                    success = true;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.User.SaveLastLoginTime(" + user.ID.ToString() + ")", err);
            }
            return success;
        }

        public static bool IsUserNameExists(string userName)
        {
            bool exists = false;
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    exists = context.Users.Where(u => u.UserName == userName).Count() > 0;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.User.IsUsernameExists(" + userName + ")", err);
            }
            return exists;
        }

        public static int UpdateVerificationCode(string verificationCode, string email, int appTypeId)
        {
            int ID = 0;
            try
            {
                using (DbConnection connection = new DbConnection())
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "UpdateVerificationCode";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@VerificationCode", verificationCode);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@AppTypeId", appTypeId);

                    cmd.Parameters.Add("@ID", SqlDbType.Int);
                    cmd.Parameters["@ID"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    if (cmd.Parameters["@ID"].Value == DBNull.Value)
                        ID = 0;
                    else
                        ID = Convert.ToInt32(cmd.Parameters["@ID"].Value);

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in User.UpdateVerificationCode", err);
            }
            return ID;
        }

        public static string GetUserName(int ID)
        {
            string UserName = "";

            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    UserName = context.Users
                        .Where(u => u.ID == ID)
                        .Select(u => u.UserName).FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.User.GetUserName()", err);
            }
            return UserName;
        }
        
        public static bool ResetPassword(int userId, string password)
        {
            bool success = false;
            User user = new User();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    user = User.Load(userId);
                    user.Password = password;
                    context.Users.Attach(user);
                    context.Entry(user).State = System.Data.EntityState.Modified;
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.User.ResetPassword(" + userId.ToString() + ")", err);
            }
            return success;

        }

        public static bool TruePassword(int userId, string password)
        {
            bool success = false;
            User user = new User();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    user = User.Load(userId);

                    if (user.Password == password)
                        success = true;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.User.TruePassword(" + userId.ToString() + ")", err);
            }
            return success;

        }

        public static List<User> GetUsers(string userName, string fullName, string emailAddress, string address, string mobileNumber, bool? isActive)
        {
            List<User> UsersList = new List<User>();

            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    UsersList = context.Users
                                       .Where(u => u.ID > 0
                                         && (u.FullName.Contains(fullName))
                                         && (u.EmailAddress.Contains(emailAddress))
                                         && (u.UserName.Contains(userName))
                                         && (!isActive.HasValue || u.IsActive == isActive)
                                         && (u.MobileNumber == null || (u.MobileNumber != null && u.MobileNumber.Contains(mobileNumber)))
                                        )
                                         .OrderBy(u => u.FullName)
                                         .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.User.GetUsers(" + userName + ", " + fullName + ", " + emailAddress + ", " + address + ", " + mobileNumber + ", " + isActive.ToString() + ")", err);
            }


            return UsersList;
        }
    }
}
