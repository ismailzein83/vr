using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class UpdateOfficialAccountInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public long AddressSeq { get; set; }

        public string MainContactId { get; set; }
        public string Name { get; set; }
        public string MinistryName { get; set; }
        public string EndCustomerName { get; set; }
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
