using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.CDRAnalysis
{
    public partial class Permission
    {
        public static List<Permission> GetAll()
        {
            List<Permission> permissions = new List<Permission>();
            try
            {
                using (Entities context = new Entities())
                {
                    permissions = context.Permissions
                        .Include(p => p.Permissions1)//Children
                        .OrderBy(p => p.Name)
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("error in Permission.GetPermissions", err);
            }
            return permissions;
        }
    }
}
