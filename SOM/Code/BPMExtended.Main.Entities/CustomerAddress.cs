using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class CustomerAddress
    {
        public long Sequence { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string Nationality { get; set; }
        public string DocumentId { get; set; }
        public string DocumentTypeId { get; set; }
        public string CountryId { get; set; }
        public string Region { get; set; }
        public string StateProvince { get; set; }
        public string Town { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string StreetNb { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Notes { get; set; }
        public string LocationType { get; set; }
        public string HomeNumber { get; set; }
        public string MobileNumber { get; set; }
        public string FaxNumber { get; set; }
        public string BusinessPhone { get; set; }
        public string Email { get; set; }
        public string Language { get; set; }
        public string Career { get; set; }
        public bool isEmployee { get; set; }
        public bool Mailbox { get; set; }
    }
}
