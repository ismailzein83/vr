﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
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
                        .Where(p => p.AppTypeID == 2)
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
