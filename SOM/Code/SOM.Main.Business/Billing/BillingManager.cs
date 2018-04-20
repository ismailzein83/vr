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
                Id = Guid.NewGuid().ToString(),
                IsMain = true,
                Package = "P1",
                Name = "Land Line Subscription",
                Description = "Land Line Subscription",
                SubscriptionFee = 1500,
                UsageFee = 0,
                RatePlanId = "9ADC523B-F11C-4A77-B336-7F2CDE2309EF",
                ServiceParams = new List<ServiceParameters> { 
                   new ServiceParameters{ Id = Guid.NewGuid().ToString(), Name="P1"},
                   new ServiceParameters{ Id = Guid.NewGuid().ToString(), Name="P2"},
                   new ServiceParameters{ Id = Guid.NewGuid().ToString(), Name="P3"}
                }
            });

            result.Add(new TelephoneService
            {
                Id = Guid.NewGuid().ToString(),
                IsMain = false,
                Package = "P1",
                Name = "Clip",
                Description = "Show Caller Id",
                SubscriptionFee = 0,
                UsageFee = 20,
                RatePlanId = "9ADC523B-F11C-4A77-B336-7F2CDE2309EF",
                ServiceParams = new List<ServiceParameters> { 
                   new ServiceParameters{ Id = Guid.NewGuid().ToString(), Name="P3"},
                   new ServiceParameters{ Id = Guid.NewGuid().ToString(), Name="P4"}
                }
            });

            result.Add(new TelephoneService
            {
                Id = Guid.NewGuid().ToString(),
                IsMain = false,
                Package = "P3",
                Name = "Forwarding",
                Description = "Forwarding in case of no answer",
                SubscriptionFee = 0,
                UsageFee = 10,
                RatePlanId = "BF096C04-15E7-44C5-BFD6-9A136053D62E",
                ServiceParams = new List<ServiceParameters> { 
                   new ServiceParameters{ Id = Guid.NewGuid().ToString(), Name="P5"},
                   new ServiceParameters{ Id = Guid.NewGuid().ToString(), Name="P6"},
                   new ServiceParameters{ Id = Guid.NewGuid().ToString(), Name="P7"}
                }
            });

            result.Add(new TelephoneService
            {
                Id = Guid.NewGuid().ToString(),
                IsMain = false,
                Package = "P1",
                Name = "International Dial Block",
                Description = "International Dial Block",
                SubscriptionFee = 0,
                UsageFee = 0,
                RatePlanId = "BF096C04-15E7-44C5-BFD6-9A136053D62E",
                ServiceParams = new List<ServiceParameters> {
                   new ServiceParameters{ Id = Guid.NewGuid().ToString(), Name="P8"}
                }
            });


            return result;
        }

        public List<RatePlan> GetRatePlans()
        {
            List<RatePlan> ratePlans = new List<RatePlan>();

            ratePlans.Add(new RatePlan
            {
                Id = "99D8001C-A219-4502-BCFB-FA5E7423D7EB",
                Description = "Plan For ST Employess",
                Type = RatePlanType.ST_Employee,
                Name = "ST Employee"
            });

            ratePlans.Add(new RatePlan
            {
                Id = "BF096C04-15E7-44C5-BFD6-9A136053D62E",
                Description = "ISDN Plan",
                Type = RatePlanType.ISDN,
                Name = "ISDN"
            });

            ratePlans.Add(new RatePlan
            {
                Id = "9ADC523B-F11C-4A77-B336-7F2CDE2309EF",
                Description = "Coin Box",
                Type = RatePlanType.ISDN,
                Name = "Coin Box"
            });

            return ratePlans;
        }
    }
}
