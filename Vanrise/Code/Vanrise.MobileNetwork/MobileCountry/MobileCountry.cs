using System;
using System.Collections.Generic;

namespace Vanrise.MobileNetwork.Entities
{
    public class MobileCountry
    {
        public int Id { get; set; }
        public MobileCountrySettings MobileCountrySettings { get; set; }
        //public string Code { get; set; }
        public int CountryId { get; set; }
    }

    public class MobileCountrySettings
    {
        public List<MobileCountryCode> Codes { get; set; }
    }

    public class MobileCountryCode
    {
        public string Code { get; set; }
    }
}