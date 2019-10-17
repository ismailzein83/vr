using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class InstallmentManager
    {
        public List<InstallmentTemplateInfo> GetCustomerCategoryInfo()
        {
            var installmentTemplateInfoItems = new List<InstallmentTemplateInfo>();
            using (SOMClient client = new SOMClient())
            {
                List<InstallmentTemplate> items = client.Get<List<InstallmentTemplate>>(String.Format("api/SOM.ST/Billing/GetPaymentPlanTemplates"));
                foreach (var item in items)
                {
                    var customerCatergoryInfoItem = InstallmentTemplateToInfoMapper(item);
                    installmentTemplateInfoItems.Add(customerCatergoryInfoItem);
                }
            }
            return installmentTemplateInfoItems;
        }
        public InstallmentTemplateInfo InstallmentTemplateToInfoMapper(InstallmentTemplate item)
        {
            return new InstallmentTemplateInfo
            {
                Name = item.Name,
                Id = item.Id

            };
        }
    }
}
