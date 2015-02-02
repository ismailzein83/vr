using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.CDRAnalysis
{
    public partial class Source
    {
        public static Source Load(string Name)
        {
            Source Source = new Source();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    Source = context.Sources
                       .Where(u => u.Name == Name)
                       .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Source.Load(" + Name.ToString() + ")", err);
            }
            return Source;
        }

        public static Source Load(int ID)
        {
            Source Source = new Source();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    Source = context.Sources
                       .Where(u => u.ID == ID)
                       .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Source.Load(" + ID.ToString() + ")", err);
            }
            return Source;
        }

       

        public static List<Source> GetAllSources()
        {
            List<Source> SourcesList = new List<Source>();

            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    SourcesList = context.Sources
                                            .OrderByDescending(u => u.Name)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Source.GetSources()", err);
            }

            return SourcesList;
        }

        public static Source Save(Source Source)
        {
            Source CurrentSource = new Source();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    context.Sources.Attach(Source);
                    context.Entry(Source).State = System.Data.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Source.Save(" + Source.ID.ToString() + ")", err);
            }
            return CurrentSource;
        }

        public static Source GetByEmail(string Email) // Get Folder of a Given Source Knowing its Email Address;
        {
        Source Source = new Source();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    Source = context.Sources
                       .Where(u => u.Email == Email)
                       .FirstOrDefault();
                    if (Source == null)
                    {
                        Source = new Source();
                        Source.ID = 0;
                        Source.Name = string.Empty;
                    }

                    return Source;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Source.Load(" + Email.ToString() + ")", err);
            }


            return Source;
        }
    }
}
