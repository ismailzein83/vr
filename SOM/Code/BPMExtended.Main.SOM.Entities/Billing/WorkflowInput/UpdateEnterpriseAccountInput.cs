using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class UpdateEnterpriseAccountInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public long AddressSeq { get; set; }

        public string MainContactId { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Branch { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Nationality { get; set; }
        public string BusinessType { get; set; }
        public string NationalId { get; set; }
        public string DocumentIdType { get; set; }
        public string Region { get; set; }
        public string StateProvince { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string AddressNotes { get; set; }
        public string MobilePhone { get; set; }
        public string BusinessPhone { get; set; }
        public string FaxNumber { get; set; }
        public string Email { get; set; }

    }
}
