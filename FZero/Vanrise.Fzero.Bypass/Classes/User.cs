﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;


namespace Vanrise.Fzero.Bypass
{
    public partial class User
    {
        #region Properties

        public MobileOperator MobileOperator 
        {
            get 
            {
                if(MobileOperators != null && MobileOperators.Count > 0)
                    return MobileOperators.ElementAt(0);
                return null;
            }
        }

        public ApplicationUser ApplicationUser
        {
            get
            {
                if (ApplicationUsers != null && ApplicationUsers.Count > 0)
                    return ApplicationUsers.ElementAt(0);
                return null;
            }
        }

        #endregion
        
        public static List<User> GetAllUsers()
        {
            List<User> UsersList = new List<User>();

            try
            {
                using (Entities context = new Entities())
                {
                    UsersList = context.Users
                        .OrderBy(x => x.FullName).ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.User.GetAllUsers()", err);
            }


            return UsersList;
        }

        public static User GetUser(string username, string password, int appTypeID)
        {
            User user = new User();
            try
            {
                using (Entities context = new Entities())
                {
                    user = context.Users
                        .Include(u=>u.UserPermissions)
                        .Include(u => u.MobileOperators)
                        .Include(u => u.ApplicationUsers)
                        .Where(u => u.UserName == username 
                            && u.Password == password
                            && u.AppTypeID == appTypeID
                            && u.IsActive)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.User.GetUser(" + username + ")", err);
            }
            return user;
        }

        public static User Load(int ID)
        {
            User user = new User();
            try
            {
                using (Entities context = new Entities())
                {
                    user = context.Users
                        .Include(u => u.ApplicationUsers)
                        .Include(u => u.MobileOperators)
                        .Include(u => u.UserPermissions)
                        .Where(u => u.ID == ID)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.User.Load(" + ID.ToString() + ")", err);
            }
            return user;
        }

        public static int Save(User user)
        {
            int id = 0;
            try
            { 
                user.MobileOperators = null;
                user.ApplicationUsers = null;

                using (Entities context = new Entities())
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
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.User.Save(" + user.ID.ToString() + ")", err);
            }
            return id;
        }

        public static bool Delete(User user)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    context.Entry(user).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.User.Delete(" + user.ID.ToString() + ")", err);
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
                using (Entities context = new Entities())
                {
                    context.Users.Attach(user);
                    context.Entry(user).Property(u => u.LastLoginTime).IsModified = true;
                    context.SaveChanges();
                    success = true;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.User.SaveLastLoginTime(" + user.ID.ToString() + ")", err);
            }
            return success;
        }

        public static bool IsUserNameExists(string userName)
        {
            bool exists = false;
            try
            {
                using (Entities context = new Entities())
                {
                    exists = context.Users.Where(u => u.UserName == userName).Count() > 0;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.User.IsUsernameExists(" + userName + ")", err);
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
                using (Entities context = new Entities())
                {
                    UserName = context.Users
                        .Where(u => u.ID == ID)
                        .Select(u => u.UserName).FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.User.GetUserName()", err);
            }
            return UserName;
        }
        
        public static bool ResetPassword(int userId, string password)
        {
            bool success = false;
            User user = new User();
            try
            {
                using (Entities context = new Entities())
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
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.User.ResetPassword(" + userId.ToString() + ")", err);
            }
            return success;

        }

        public static bool TruePassword(int userId, string password)
        {
            bool success = false;
            User user = new User();
            try
            {
                using (Entities context = new Entities())
                {
                    user = User.Load(userId);

                    if (user.Password == password)
                        success = true;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.User.TruePassword(" + userId.ToString() + ")", err);
            }
            return success;

        }
    }
}
