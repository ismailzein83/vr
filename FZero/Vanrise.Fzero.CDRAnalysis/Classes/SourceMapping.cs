using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.CDRAnalysis
{
    public partial class SourceMapping
    {
        public static List<SourceMapping> GetSwitchMappings(int SwitchID)
           
        {
            List<SourceMapping> SourceMappingsList = new List<SourceMapping>();

            try
            {
                using (Entities context = new Entities())
                {
                    SourceMappingsList = context.SourceMappings.Where(x => x.SwitchID == SwitchID).Include(u => u.PredefinedColumn)
                                            .OrderByDescending(u => u.ID)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.SourceMapping.GetSwitchMappings()", err);
            }

            return SourceMappingsList;
        }

        public static SourceMapping Load(int ID)
        {
            SourceMapping SourceMapping = new SourceMapping();
            try
            {
                using (Entities context = new Entities())
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
                using (Entities context = new Entities())
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
                using (Entities context = new Entities())
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
                using (Entities context = new Entities())
                {
                    int Count;
                    if (SourceMapping.SwitchID == 0)
                    {
                        Count = context.SourceMappings
                           .Where(u => u.SwitchID == SourceMapping.SwitchID && (u.ColumnName == SourceMapping.ColumnName 
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
                           .Where(u => u.SwitchID == SourceMapping.SwitchID && u.ID != SourceMapping.ID && (u.ColumnName == SourceMapping.ColumnName 
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
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.SourceMapping.CheckIfExists(" + SourceMapping.SwitchID.ToString() + ", " + SourceMapping.ColumnName.ToString() + ", " + SourceMapping.MappedtoColumnNumber.ToString() + ")", err);
            }
            return true;
        }
    }
}
