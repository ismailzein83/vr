﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class CustomerAddressInput
    {
        public long AddressSeq { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string MotherName { get; set; }
        public string Birthdate { get; set; }
        public string Nationality { get; set; }
        public  string Career { get; set; }
        public string NationalId { get; set; }
        public string DocumentIdType { get; set; }
        public string Region { get; set; }
        public string StateProvince { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string BusinessPhone { get; set; }
        public string FaxNumber { get; set; }
        public string Email { get; set; }
        public string Language { get; set; }
        public string Town { get; set; }
        public string AddressNotes { get; set; }    
        public CommonInputArgument CommonInputArgument { get; set; }

    }

}
