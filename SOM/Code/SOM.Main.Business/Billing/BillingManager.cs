using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOM.Main.Entities;

namespace SOM.Main.Business
{
    public class BillingManager
    {
        public List<TelephoneService> GetServices()
        {
            List<TelephoneService> result = new List<TelephoneService>();

            result.Add(new TelephoneService
            {
                Id = "S1",
                IsMain = true,
                Package = "P1",
                Name = "Land Line Subscription",
                Description = "Land Line Subscription",
                SubscriptionFee = 1500,
                UsageFee = 0
            });

            result.Add(new TelephoneService
            {
                Id = "S2",
                IsMain = false,
                Package = "P1",
                Name = "Clip",
                Description = "Show Caller Id",
                SubscriptionFee = 0,
                UsageFee = 20
            });

            result.Add(new TelephoneService
            {
                Id = "S3",
                IsMain = false,
                Package = "P3",
                Name = "Forwarding",
                Description = "Forwarding in case of no answer",
                SubscriptionFee = 0,
                UsageFee = 10
            });

            result.Add(new TelephoneService
            {
                Id = "S4",
                IsMain = false,
                Package = "P1",
                Name = "International Dial Block",
                Description = "International Dial Block",
                SubscriptionFee = 0,
                UsageFee = 0
            });


            return result;
        }

        public List<RatePlan> GetRatePlans()
        {
            List<RatePlan> ratePlans = new List<RatePlan>();

            ratePlans.Add(new RatePlan
            {
                Id = "R1",
                Description = "Plan For ST Employess",
                Type = RatePlanType.ST_Employee,
                Name = "ST Employee"
            });

            ratePlans.Add(new RatePlan
            {
                Id = "R2",
                Description = "ISDN Plan",
                Type = RatePlanType.ISDN,
                Name = "ISDN"
            });

            ratePlans.Add(new RatePlan
            {
                Id = "R3",
                Description = "Coin Box",
                Type = RatePlanType.ISDN,
                Name = "Coin Box"
            });

            return ratePlans;
        }
    }
}
