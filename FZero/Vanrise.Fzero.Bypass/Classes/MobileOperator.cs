using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public enum FTPTypeEnum { FTP = 0, SFTP = 1 }

    public partial class MobileOperator
    {
        public static MobileOperator Save(MobileOperator MobileOperator)
        {
            MobileOperator CurrentMobileOperator = new MobileOperator();
            try
            {
                using (Entities context = new Entities())
                {
                    if (MobileOperator.ID == 0)
                    {
                        context.MobileOperators.Add(MobileOperator);
                    }
                    else
                    {
                        User.Save(MobileOperator.User);
                        MobileOperator.User = null;
                        context.Entry(MobileOperator).State = System.Data.EntityState.Modified;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.MobileOperator.Save(" + MobileOperator.ID.ToString() + ")", err);
            }
            return CurrentMobileOperator;
        }

        public static MobileOperator Load(int ID)
        {
            MobileOperator MobileOperator = new MobileOperator();
            try
            {
                using (Entities context = new Entities())
                {
                    MobileOperator = context.MobileOperators
                       .Include(u => u.User)
                       .Where(u => u.ID == ID)
                       .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.MobileOperator.Load(" + ID.ToString() + ")", err);
            }
            return MobileOperator;
        }

        public static List<MobileOperator> GetMobileOperators(string name, string email, string prefixes, string website, int GMT)
        {
            List<MobileOperator> MobileOperatorsList = new List<MobileOperator>();

            try
            {
                using (Entities context = new Entities())
                {
                    MobileOperatorsList = context.MobileOperators
                                         .Include(u => u.User)
                                         .Include(u => u.User.Client)
                                          .Where(u => u.ID > 0
                                            && (GMT == 0 || u.User.GMT == GMT)
                                            && (u.User.FullName.Contains(name))
                                            && (u.User.EmailAddress.Contains(email))
                                            && (u.User.Website == null || u.User.Website.Contains(website))
                                            && (u.User.Prefix == null || u.User.Prefix.Contains(prefixes))
                                            )

                                            .OrderBy(u => u.User.UserName)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.MobileOperator.GetMobileOperators()", err);
            }

            return MobileOperatorsList;
        }

        public static List<MobileOperator> GetMobileOperators()
        {
            return GetMobileOperators(string.Empty, string.Empty, string.Empty, string.Empty,0);
        }

        public static List<MobileOperator> GetMobileOperatorsList()
        {
            List<MobileOperator> MobileOperatorsList = new List<MobileOperator>();

            try
            {
                using (Entities context = new Entities())
                {
                    MobileOperatorsList = context.MobileOperators
                                            .Include(u=>u.User)
                                            .OrderBy(u => u.User.FullName)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.MobileOperator.GetMobileOperatorsList()", err);
            }

            return MobileOperatorsList;
        }

    }
}
