using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using SOM.Main.Entities;

namespace BPMExtended.Main.Business
{
    public class BillingRequestManager
    {
        public List<TelephoneServiceDetail> GetServices()
        {
            List<TelephoneService> telephoneServices = null;
            using (SOMClient client = new SOMClient())
            {
                telephoneServices = client.Get<List<TelephoneService>>(String.Format("api/SOM_Main/Billing/GetServices"));
            }
            List<TelephoneServiceDetail> result = new List<TelephoneServiceDetail>();
            if (telephoneServices != null)
            {
                foreach (var service in telephoneServices)
                {
                    var serviceDetail = new TelephoneServiceDetail
                    {
                        Description = service.Description,
                        Id = service.Id,
                        IsMain = service.IsMain,
                        Name = service.Name,
                        Package = service.Package,
                        SubscriptionFee = service.SubscriptionFee,
                        UsageFee = service.UsageFee
                    };
                    result.Add(serviceDetail);
                }
            }
            return result;

        }

        public List<RatePlanDetail> GetRatePlans()
        {
            List<RatePlan> ratePlans = null;
            using (SOMClient client = new SOMClient())
            {

                ratePlans = client.Get<List<RatePlan>>(String.Format("api/SOM_Main/Billing/GetRatePlans"));
            }
            List<RatePlanDetail> result = new List<RatePlanDetail>();
            if (ratePlans != null)
            {
                foreach (var ratePlan in ratePlans)
                {
                    var ratePlanDetail = new RatePlanDetail
                    {
                        Description = ratePlan.Description,
                        Id = ratePlan.Id,
                        Name = ratePlan.Name,
                        Type = (BPMExtended.Main.Entities.RatePlanType)ratePlan.Type
                    };
                    result.Add(ratePlanDetail);
                }
            }
            return result;

        }
    }
}
