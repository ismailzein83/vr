
namespace TABS.Components
{
    public class Event
    {
        public EventType Type { get; set; }
        public object Context { get; set; }

        public Event(EventType type, object context)
        {
            Type = type;
            this.Context = context;
        }

        public MailTemplateType MailType
        {
            get
            {
                MailTemplateType mailtype = MailTemplateType.Pricelist;

                switch (Type)
                {
                    case EventType.PricelistImport:
                        mailtype = MailTemplateType.PricelistImport;
                        break;
                    case EventType.InvoiceGeneration:
                        mailtype = MailTemplateType.InvoiceGeneration;
                        break;
                }

                return mailtype;
            }
        }

    }
}
