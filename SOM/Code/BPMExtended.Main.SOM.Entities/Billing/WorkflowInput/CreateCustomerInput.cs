using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class CreateCustomerInput
    {
        public CreateCustomerInputArgument InputArguments { get; set; }
    }

    public class CreateCustomerInputArgument
    {
        public BillingCommonInputArgument CommonArgument { get; set; }

        public int CustomerCategoryId { get; set; }

        public int CountryId { get; set; }

        public string CityName { get; set; }

        public CreateCustomerContactInput ContactInput { get; set; }

        public CreateCustomerAccountInput AccountInput { get; set; }

        public class CreateCustomerContactInput
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }
        }

        public class CreateCustomerAccountInput
        {

        }
    }
}
