using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class CustomerRequestType
    {
        public Guid CustomerRequestTypeId { get; set; }

        public string Name { get; set; }

        public CustomerRequestTypeSettings Settings { get; set; }
    }

    public class CustomerRequestTypeSettings
    {
        public string PageURL { get; set; }

        public CustomerRequestTypeExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class CustomerRequestTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract bool CanCreateRequestOnCustomer(ICustomerRequestTypeCanCreateRequestOnCustomerContext context);
    }

    public interface ICustomerRequestTypeCanCreateRequestOnCustomerContext
    {
        CustomerObjectType CustomerObjectType { get; }

        Guid AccountOrContactId { get; set; }
    }
}
