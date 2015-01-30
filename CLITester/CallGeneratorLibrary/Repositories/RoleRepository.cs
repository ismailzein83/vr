using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CallGeneratorLibrary.Repositories
{
    public class RoleRepository
    {
        public static List<Role> GetRoles()
        {
            List<Role> roles = new List<Role>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    roles = context.Roles.ToList<Role>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return roles;
        }

        //public static List<Lookup> SearchLookups(string name)
        //{
        //    List<Lookup> lookups = new List<Lookup>();
        //    try
        //    {
        //        using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
        //        {
        //            lookups = context.SearchLookups(name).ToList();
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Logger.LogException(ex);
        //    }

        //    return lookups;
        //}

        public static Role Load(int roleId)
        {
            Role role = new Role();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    role = context.Roles.Where(r => r.Id == roleId).FirstOrDefault<Role>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return role;
        }

        public static bool RoleNameExists(string name)
        {
            bool result = false;
            Role project = new Role();

            try
            {
                using (CallGeneratorModelDataContext dataContext = new CallGeneratorModelDataContext())
                {
                    project = dataContext.Roles.Where(item => item.Name.ToLower() == name.ToLower()).SingleOrDefault<Role>();
                    if (project != null && project.Id > 0)
                        result = true;
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return result;
        }

        public static List<Role> GetUserRoles(int userId)
        {
            List<Role> roles = new List<Role>();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    roles = context.GetUserRoles(userId).ToList<Role>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return roles;
        }

        public static bool DeleteUserRoles(int userId)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    List<UserRole> userRoles = context.UserRoles.Where(u => u.UserId == userId).ToList<UserRole>();
                    foreach (UserRole up in userRoles)
                        context.UserRoles.DeleteOnSubmit(up);
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

        public static bool SaveUserRoles(List<UserRole> userRoles)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    foreach (UserRole up in userRoles)
                        context.UserRoles.InsertOnSubmit(up);
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

        //public static List<Menus> GetMenus(string roleIds)
        //{
        //    List<Menus> menus = new List<Menus>();

        //    try
        //    {
        //        using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
        //        {
        //            menus = context.GetRoleMenus(roleIds).ToList<Menus>();
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Logger.LogException(ex);
        //    }

        //    return menus;
        //}

        //public static List<Menus> GetMenus(List<Role> roles)
        //{
        //    List<Menus> menus = new List<Menus>();

        //    try
        //    {
        //        using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
        //        {
        //            if (roles != null && roles.Count > 0)
        //            {
        //                string rolesIds = string.Empty;
        //                foreach (Role role in roles)
        //                    rolesIds += role.Id.ToString() + ",";
        //                rolesIds = rolesIds.TrimEnd(',');

        //                menus = context.GetRoleMenus(rolesIds).ToList<Menus>();
        //            }
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Logger.LogException(ex);
        //    }

        //    return menus;
        //}

        public static List<Page> GetPages(string roleIds)
        {
            List<Page> pages = new List<Page>();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    pages = context.GetRolePages(roleIds).ToList<Page>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return pages;
        }

        public static List<Page> GetPages(List<Role> roles)
        {
            List<Page> pages = new List<Page>();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    if (roles != null && roles.Count > 0)
                    {
                        string rolesIds = string.Empty;
                        foreach (Role role in roles)
                            rolesIds += role.Id.ToString() + ",";
                        rolesIds = rolesIds.TrimEnd(',');

                        pages = context.GetRolePages(rolesIds).ToList<Page>();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return pages;
        }

        public static List<Permission> GetPermissions(Role role)
        {
            List<Permission> permissions = new List<Permission>();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    permissions = context.GetRolePermissions(role.Id).ToList<Permission>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return permissions;
        }

        public static List<User> GetRoleUsers(Role role)
        {
            List<User> users = new List<User>();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    users = context.GetRoleUsers(role.Id).ToList<User>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return users;
        }

        public static List<RolePermission> GetRolePermissions(int roleId)
        {
            List<RolePermission> permissions = new List<RolePermission>();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    permissions = context.RolePermissions.Where(rp => rp.RoleId.HasValue && rp.RoleId.Value == roleId).ToList<RolePermission>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return permissions;
        }

        public static List<RolePermission> GetRolePermissions(string pageName, string roleIds)
        {
            List<RolePermission> rolePermissions = new List<RolePermission>();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    rolePermissions = context.GetRolePermissionActions(roleIds, pageName).ToList<RolePermission>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return rolePermissions;
        }

        public static bool DeleteRolePermissions(int roleId)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    List<RolePermission> rolePermissions = context.RolePermissions.Where(u => u.RoleId == roleId).ToList<RolePermission>();
                    foreach (RolePermission up in rolePermissions)
                        context.RolePermissions.DeleteOnSubmit(up);
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

        public static bool SaveRolePermissions(List<RolePermission> rolePermissions)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    foreach (RolePermission up in rolePermissions)
                        context.RolePermissions.InsertOnSubmit(up);
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

        public static bool Delete(int roleId)
        {
            bool success = false;

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    Role role = context.Roles.Where(lk => lk.Id == roleId).Single<Role>();
                    context.Roles.DeleteOnSubmit(role);
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

        public static bool Save(Role role)
        {
            bool success = false;
            if (role.Id == default(int))
                success = Insert(role);
            else
                success = Update(role);
            return success;
        }

        private static bool Insert(Role role)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.Roles.InsertOnSubmit(role);
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

        private static bool Update(Role role)
        {
            bool success = false;
            Role rol = new Role();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    rol = context.Roles.Single(r => r.Id == role.Id);
                    rol.Name = role.Name;
                    rol.Description = role.Description;
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

