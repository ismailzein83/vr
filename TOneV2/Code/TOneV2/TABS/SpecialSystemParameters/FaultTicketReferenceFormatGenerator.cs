using System.Linq;
using System.Text;

namespace TABS.SpecialSystemParameters
{
    public class FaultTicketReferenceFormatGenerator
    {
        public static readonly string HelpString = @"
            {0} = Carrier Profile Name,  {1} = Zone Name, {2} = Current Date Time, {3} Counter;
            Date can be formatted in standard .NET formats for date";

        public static string GetFaultTicketReferenceFormat(FaultTicket faultTicket)
        {
            CarrierAccount account = faultTicket.CarrierAccount;

            Zone zone = faultTicket.Zone;

            string referenceFormat = TABS.SystemParameter.FaultTicketReferenceFormat.TextValue;

            StringBuilder sb = new StringBuilder();
            //try
            //{
            
            sb.AppendFormat(referenceFormat, account.CarrierProfile.Name, zone.Name, faultTicket.UpdateDate, ObjectAssembler.GetList<FaultTicket>().Count() + 1 );
            
            //sb.AppendFormat(referenceFormat, account.CarrierProfile.Name, zone.Name, faultTicket.UpdateDate, (faultTicket.TicketUpdates != null) ? faultTicket.TicketUpdates.Count() + 1 : 1 );
            //}
            //catch 
            //{
            //    sb.AppendFormat(referenceFormat, account.CarrierProfile.Name, zone.Name, faultTicket.UpdateDate, 1);
            //}
            return sb.ToString();
        }
    }
}
