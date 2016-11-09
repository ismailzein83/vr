using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class NotificationManager
    {
        public void BuildNotifications(int sellingNumberPlanId, IEnumerable<int> customerIds, IEnumerable<SalePLZoneChange> zoneChanges, DateTime effectiveDate)
        {
            //TODO: Get All Sale Codes by Selling Number Plan Id

            //TODO: Get Connected Entities from Sale Code

            //TODO: Structure Existing Sale Codes by Country ID

            //TODO: Create reader for rate locator (no cache) sending customer ids as info

            //TODO: For each Customer get settings to check if AtoZ

            //TODO: get sold countries for this customer
                    
                    //TODO: If AtoZ
                    
                            //TODO: for each sold country got from all sale codes
                    
                            //TODO: Create Sale PL Zone Notification
                    
                    //TODO: Country change
                            
                            //TODO: Get Distinct Country IDs from zone changes
                            
                            //TODO: Intersection with Sold Countries

                            //TODO: loop on intersection list
                            
                                    //TODO: create Sale PL Zone Notification

                    //TODO: Rate Change
                            
                            //Get Codes of this Zone by Zone Name

            //TODO: for all Sale PL Zone Notification get Rate using rate locator


        }
    }
}
