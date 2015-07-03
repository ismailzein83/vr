using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Repositories
{
    public class CarrierRepository
    {
        public static Carrier Load(int CarrierId)
        {
            Carrier carrier = new Carrier();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    carrier = context.Carriers.Where(l => l.Id == CarrierId).FirstOrDefault<Carrier>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return carrier;
        }

        public static bool ExistShortName(string ShortName, int id)
        {
            bool Exist = false;
            List<Carrier> LstCarriers = new List<Carrier>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstCarriers = context.Carriers.Where(l => l.ShortName == ShortName).ToList<Carrier>();
                    if (LstCarriers.Count > 0)
                    {
                        if (LstCarriers.Count == 1 && id == LstCarriers[0].Id)
                        {
                            Exist = false;
                        }
                        else
                            Exist = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return Exist;
        }


        public static List<Carrier> GetCarriers()
        {
            List<Carrier> LstCarriers = new List<Carrier>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstCarriers = context.Carriers.ToList<Carrier>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return LstCarriers;
        }

        public static List<Carrier> GetCarriers(string name, string country)
        {
            List<Carrier> LstCarriers = new List<Carrier>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstCarriers = context.Carriers.Where(x => (name == "" || x.Name.Contains(name))).ToList<Carrier>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return LstCarriers;
        }

        public static bool Save(Carrier carrier)
        {
            bool success = false;
            if (carrier.Id == default(int))
                success = Insert(carrier);
            else
                success = Update(carrier);
            return success;
        }

        private static bool Insert(Carrier carrier)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.Carriers.InsertOnSubmit(carrier);
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

        private static bool Update(Carrier carrier)
        {
            bool success = false;
            Carrier carrierObj = new Carrier();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    carrierObj = context.Carriers.Single(l => l.Id == carrier.Id);
                    carrierObj.Name = carrier.Name;
                    carrierObj.Prefix = carrier.Prefix;
                    carrierObj.ShortName = carrier.ShortName;
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

        public static bool Delete(int id)
        {
            bool success = false;

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    Carrier carrier = context.Carriers.Where(u => u.Id == id).Single<Carrier>();
                    context.Carriers.DeleteOnSubmit(carrier);
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
