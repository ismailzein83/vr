using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Diagnostics;
using CallGeneratorLibrary.Utilities;

namespace CallGeneratorLibrary.Repositories
{
    public class UserRepository
    {
        public static void DecreaseBalance()
        {
            User user = UserRepository.GetUser((int)Enums.UserRole.SuperUser);

            if (user.Balance > 0)
            {
                user.Balance = user.Balance - 1;
                Save(user);
            }
        }

        public static User GetUser(int RoleId)
        {
            User user = new User();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    user = context.Users.Where(l => l.Role == RoleId).FirstOrDefault();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return user;
        }

        public static string GetName(int userId)
        {
            string s = "";

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    s = context.Users.Where(l => l.Id == userId).FirstOrDefault<User>().Name;
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return s;
        }

        public static List<User> GetSubUsers()
        {
            List<User> users = new List<User>();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    users = context.Users.Where(l => l.Role == (int)CallGeneratorLibrary.Utilities.Enums.UserRole.User).ToList();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return users;
        }

        public static List<User> GetUsers()
        {
            List<User> users = new List<User>();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    users = context.Users.ToList();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return users;
        }
 
        public static User GetUser(string username)
        {
            User user = new User();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    //DataLoadOptions options = new DataLoadOptions();
                    //options.LoadWith<User>(e => e.UserRoles);
                    //context.LoadOptions = options;

                    var q = from u in context.Users
                            where u.UserName == username && (u.IsActive.HasValue && u.IsActive.Value)
                            select u;
                    user = q.FirstOrDefault<User>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return user;
        }

        public static bool UserNameExists(string username)
        {
            bool result = false;
            User user = new User();

            try
            {
                using (CallGeneratorModelDataContext dataContext = new CallGeneratorModelDataContext())
                {
                    user = dataContext.Users.Where(item => item.UserName.ToLower() == username.ToLower()).SingleOrDefault<User>();
                    if (user != null && user.Id > 0)
                        result = true;
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return result;
        }

        public static User Login(string username, string password)
        {
            User user = new User();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    //DataLoadOptions options = new DataLoadOptions();
                    //options.LoadWith<User>(e => e.ProjectNotifications);
                    //context.LoadOptions = options;

                    var q = from u in context.Users
                            where u.UserName == username && u.Password == password && (u.IsActive.HasValue && u.IsActive.Value)
                            select u;
                    return q.FirstOrDefault<User>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return user;
        }

        public static string GetPassword(string username)
        {
            string password = string.Empty;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    password = (context.Users.Where(u => u.UserName == username).SingleOrDefault<User>()).Password;
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return password;
        }

        public static User Load(int userId)
        {
            User user = new User();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    //DataLoadOptions options = new DataLoadOptions();
                    //options.LoadWith<User>(e => e.UserRoles);
                    
                    //context.LoadOptions = options;

                    user = context.Users.Where(u => u.Id == userId).FirstOrDefault<User>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return user;
        }

        public static bool Delete(int userId)
        {
            bool success = false;

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    User user = context.Users.Where(u => u.Id == userId).Single<User>();
                    context.Users.DeleteOnSubmit(user);
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

        public static void Save(User user)
        {
            if (user.Id == default(int))
                Insert(user);
            else
                Update(user);
        }

        private static void Insert(User user)
        {
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.Users.InsertOnSubmit(user);
                    context.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private static void Update(User user)
        {
            User _User = new User();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    _User = context.Users.Single(ul => ul.Id == user.Id);
                    _User.UserName = user.UserName;
                    _User.Name = user.Name;
                    _User.Password = user.Password;
                    _User.Email = user.Email;
                    _User.Guid = user.Guid;
                    _User.IsActive = user.IsActive;
                    _User.LastLoginDate = user.LastLoginDate;
                    _User.CreationDate = user.CreationDate;
                    _User.LastName = user.LastName;
                    _User.MobileNumber = user.MobileNumber;
                    _User.WebsiteURL = user.WebsiteURL;
                    _User.Balance = user.Balance;
                    _User.Role = user.Role;
                    _User.RemainingBalance = user.RemainingBalance;
                    _User.LastSetBalance = user.LastSetBalance;
                    context.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
