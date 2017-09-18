using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.processing;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Business
{
   public class PriceListCountryManager
    {
       public void ProcessCountries(IProcessCountriesContext context)
       {
           IEnumerable<ZoneToProcess> zonesToProcess = context.ZonesToProcess;
           int numberOfClosedZones=0;
           DateTime lastEED =DateTime.MinValue;
           foreach (var zone in zonesToProcess)
           {
               if (zone.ChangeType == ZoneChangeType.Deleted)
               {
                   if (!zone.EED.HasValue)
                       throw new Exception(string.Format("Zone {0} is deleted and does not have an EED", zone.ZoneName));

                   numberOfClosedZones++;
                   if (zone.EED.VRGreaterThan(lastEED))
                       lastEED = zone.EED.Value;
               }
           }

           
               context.ChangedCustomerCountries = CloseCustomerCountries(lastEED, context.SellingNumberPlanId, context.CountryId, zonesToProcess.Count(), numberOfClosedZones);
       }

       private List<ChangedCustomerCountry> CloseCustomerCountries(DateTime lastEED, int SellingNumberPlanId, int CountryId,int zonesToProcesscount,int numberOfClosedZones)
       {
           List<ChangedCustomerCountry> changedCustomerCountries = new List<ChangedCustomerCountry>();
           if (zonesToProcesscount == numberOfClosedZones)
           {
               CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
               IEnumerable<CarrierAccountInfo> customersBySellingNumberPlanId = carrierAccountManager.GetCustomersBySellingNumberPlanId(SellingNumberPlanId, false);
               CustomerCountryManager customerCountryManager = new CustomerCountryManager();
               foreach (var customer in customersBySellingNumberPlanId)
               {
                   var customerCountry = customerCountryManager.GetCustomerCountry(customer.CarrierAccountId, CountryId, DateTime.Today, true);
                   if (customerCountry.EED.VRGreaterThan(lastEED))
                   {
                       ChangedCustomerCountry changedCustomerCountry = new ChangedCustomerCountry()
                       {
                           CustomerCountryId = customerCountry.CustomerCountryId,
                           EED = lastEED
                       };
                       changedCustomerCountries.Add(changedCustomerCountry);
                   }
               }
           }
          
           return changedCustomerCountries;
       }
    }
}
