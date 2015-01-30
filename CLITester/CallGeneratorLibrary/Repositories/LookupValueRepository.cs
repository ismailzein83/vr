using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CallGeneratorLibrary.Models;
using System.Data.Linq;
using CallGeneratorLibrary;

namespace CallGeneratorLibrary.Repositories
{
    public class LookupValueRepository
    {
        public static List<LookupValue> GetLookupValues(bool withLookup)
        {
            List<LookupValue> values = new List<LookupValue>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    if (withLookup)
                    {
                        DataLoadOptions options = new DataLoadOptions();
                        options.LoadWith<LookupValue>(lk => lk.Lookup);
                        context.LoadOptions = options;
                    }

                    var q = from lk in context.LookupValues
                            select lk;
                    values = q.ToList<LookupValue>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return values;
        }

        public static List<LookupValue> GetLookupValues(int lookupId)
        {
            List<LookupValue> values = new List<LookupValue>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<LookupValue>(lk => lk.Lookup);
                    context.LoadOptions = options;

                    var q = from lk in context.LookupValues
                            where lk.Lookup.Id == lookupId
                            orderby lk.OrderNumber
                            select lk;
                    values = q.ToList<LookupValue>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return values;
        }

        public static List<LookupValue> GetLookupValuesByCode(string LookupCode)
        {
            Lookup lookup = new Lookup();
            List<LookupValue> values = new List<LookupValue>();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    lookup = context.Lookups.Where(l => l.Code == LookupCode).FirstOrDefault<Lookup>();

                    if (lookup != null)
                    {
                        var q = from lk in context.LookupValues
                                where lk.Lookup.Id == lookup.Id
                                select lk;
                        values = q.ToList<LookupValue>();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return values;
        }

        public static string GetLookupValuebyName(int LookupValueId, string valuename)
        {
            LookupValue lv = new LookupValue();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    var q = from lk in context.LookupValues
                            where lk.Id == LookupValueId
                            select lk;
                    lv = q.SingleOrDefault<LookupValue>();

                    List<LookupValue> values = GetLookupValues(lv.LookupId.Value);

                    foreach (LookupValue v in values)
                    {
                        if (v.LookValue == valuename)
                            return v.LookValue;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return "";
        }

        public static LookupValue Load(int valueId)
        {
            LookupValue value = new LookupValue();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    value = context.LookupValues.Where(l => l.Id == valueId).FirstOrDefault<LookupValue>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return value;
        }
        public static LookupValue Load(string LookupValueCode)
        {
            LookupValue value = new LookupValue();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    value = context.LookupValues.Where(l => l.LookValue == LookupValueCode).FirstOrDefault<LookupValue>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return value;
        }
        public static bool Delete(int valueId)
        {
            bool success = false;

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LookupValue value = context.LookupValues.Where(lk => lk.Id == valueId).Single<LookupValue>();
                    context.LookupValues.DeleteOnSubmit(value);
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

        public static bool Save(LookupValue value)
        {
            bool success = false;
            if (value.Id == default(int))
                success = Insert(value);
            else
                success = Update(value);
            return success;
        }

        private static bool Insert(LookupValue value)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.LookupValues.InsertOnSubmit(value);
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

        private static bool Update(LookupValue value)
        {
            bool success = false;
            LookupValue lkValue = new LookupValue();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    lkValue = context.LookupValues.Single(l => l.Id == value.Id);
                    lkValue.LookupId = value.LookupId;
                    lkValue.NameAr = value.NameAr;
                    lkValue.NameEn = value.NameEn;
                    lkValue.NameFr = value.NameFr;
                    lkValue.OrderNumber = value.OrderNumber;
                    lkValue.LookValue = value.LookValue;
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
