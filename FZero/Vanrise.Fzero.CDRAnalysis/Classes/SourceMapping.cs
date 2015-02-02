using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.CDRAnalysis
{
    public partial class SourceMapping
    {
        public static List<SourceMapping> GetSourceMappings(int SourceID)
           
        {
            List<SourceMapping> SourceMappingsList = new List<SourceMapping>();

            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    SourceMappingsList = context.SourceMappings.Where(x=>x.SourceID == SourceID).Include(u => u.PredefinedColumn)
                                            .OrderByDescending(u => u.ID)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.SourceMapping.GetSourceMappings()", err);
            }

            return SourceMappingsList;
        }

        public static SourceMapping Load(int ID)
        {
            SourceMapping SourceMapping = new SourceMapping();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    SourceMapping = context.SourceMappings
                     .Where(u => u.ID == ID)
                     .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.SourceMapping.Load(" + ID.ToString() + ")", err);
            }
            return SourceMapping;
        }

        public static bool Delete(int ID)
        {

            bool success = false;
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    SourceMapping sourceMapping = SourceMapping.Load(ID);
                    context.Entry(sourceMapping).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.SourceMapping.Delete(" + ID.ToString() + ")", err);
            }
            return success;




        }

        public static SourceMapping Save(SourceMapping SourceMapping)
        {
            SourceMapping CurrentSourceMapping = new SourceMapping();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    if (SourceMapping.ID == 0)
                    {
                        context.SourceMappings.Add(SourceMapping);
                    }
                    else
                    {
                        context.SourceMappings.Attach(SourceMapping);
                        context.Entry(SourceMapping).State = System.Data.EntityState.Modified;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.SourceMapping.Save(" + SourceMapping.ID.ToString() + ")", err);
            }
            return CurrentSourceMapping;
        }

        public static bool CheckIfExists(SourceMapping SourceMapping)
        {
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    int Count;
                    if (SourceMapping.SourceID == 0)
                    {
                        Count = context.SourceMappings
                           .Where(u => u.SourceID == SourceMapping.SourceID && (u.ColumnName == SourceMapping.ColumnName 
                               || u.MappedtoColumnNumber == SourceMapping.MappedtoColumnNumber))
                           .Count();
                        if (Count == 0)
                            return false;
                        else
                            return true;
                    }
                    else
                    {
                        Count = context.SourceMappings
                           .Where(u => u.SourceID == SourceMapping.SourceID && u.ID != SourceMapping.ID && (u.ColumnName == SourceMapping.ColumnName 
                               || u.MappedtoColumnNumber == SourceMapping.MappedtoColumnNumber))
                           .Count();
                        if (Count == 0)
                            return false;
                        else
                            return true;
                    }

                  
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.SourceMapping.CheckIfExists(" + SourceMapping.SourceID.ToString() + ", " + SourceMapping.ColumnName.ToString() + ", " + SourceMapping.MappedtoColumnNumber.ToString() + ")", err);
            }
            return true;
        }
    }
}
