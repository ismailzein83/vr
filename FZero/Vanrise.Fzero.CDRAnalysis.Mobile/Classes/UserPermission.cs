﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.CDRAnalysis.Mobile
{
    public partial class UserPermission
    {
        public static bool DeleteByUserId(int userID)
        {

            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    foreach (UserPermission userPermission in context.UserPermissions.Where(x => x.UserID == userID).ToList())
                    {
                        context.Entry(userPermission).State = System.Data.EntityState.Deleted;
                        context.SaveChanges();
                    }

                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.Mobile.UserPermission.DeleteByUserId(" + userID.ToString() + ")", err);
            }
            return success;
        }

        public static bool Save(List<UserPermission> permissions){
              bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    foreach (UserPermission rolePermission in permissions)
                    {
                        context.Entry(rolePermission).State = (rolePermission.ID == 0) ? System.Data.EntityState.Added : System.Data.EntityState.Modified;
                    }
                    
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.CDRAnalysis.Mobile.UserPermission.Save()", err);
            }
            return success;
        }
    }
}
