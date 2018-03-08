using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Business;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public enum CustomerTicketDetailsField { NumberList = 0 ,Details = 1}
    public class CustomerTicketDetailsPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId
        {
            get { return new Guid("2647ede8-0ba5-4131-8f2f-eb047ff0b359"); } 
        }

        public CustomerTicketDetailsField TicketDetailsField { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            dynamic genericBusinessEntity = context.Object;

            if (genericBusinessEntity == null)
                throw new NullReferenceException("genericBusinessEntity");

           var ticketDetails =  genericBusinessEntity.TicketDetails as List<CustomerFaultTicketDescriptionSetting>;

            if(ticketDetails!= null)
            {
                switch (this.TicketDetailsField)
                {
                    case CustomerTicketDetailsField.NumberList:
                        return string.Join(",", ticketDetails.MapRecords(x => x.CodeNumber));
                    case CustomerTicketDetailsField.Details:

                        StringBuilder description = new StringBuilder();

                        GenericBusinessEntityManager genericBuinessEntityManager = new GenericBusinessEntityManager();

                        Guid reasonDefinitionId = new Guid("b8daa0fa-d381-4bb0-b772-2e6b24d199e4");
                        Guid internationalReleaseCodeDefinitionId = new Guid("6e7c2b68-3e8e-49a1-8922-796b2ce9cc1c");
                        foreach (var value in ticketDetails)
                        {
                            if (description.Length > 0)
                                description.Append(Utilities.NewLine());
                            description.Append(value.CodeNumber);
                            description.AppendFormat(" {0}", genericBuinessEntityManager.GetGenericBusinessEntityName(value.ReasonId, reasonDefinitionId));

                            if (value.InternationalReleaseCodeId.HasValue)
                            {
                                description.AppendFormat(" {0}", genericBuinessEntityManager.GetGenericBusinessEntityName(value.InternationalReleaseCodeId.Value, internationalReleaseCodeDefinitionId));
                            }
                        }
                        return description.ToString();
                }

            }
            return null;
        }
    }
}
