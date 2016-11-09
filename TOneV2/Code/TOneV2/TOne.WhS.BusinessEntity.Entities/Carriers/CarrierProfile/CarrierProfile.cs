﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class BaseCarrierProfile
    {
        public int CarrierProfileId { get; set; }

        public string Name { get; set; }

        public CarrierProfileSettings Settings { get; set; }

        public string SourceId { get; set; }
    }

    public class CarrierProfileSettings
    {
        public bool CustomerInvoiceByProfile { get; set; }
        public string Company { get; set; }

        public int? CountryId { get; set; }

        public int? CityId { get; set; }

        public string RegistrationNumber { get; set; }

        public List<string> PhoneNumbers { get; set; }

        public List<string> Faxes { get; set; }

        public string Website { get; set; }

        public string Address { get; set; }
        public string DuePeriod { get; set; }

        public string PostalCode { get; set; }

        public string Town { get; set; }

        public long CompanyLogo { get; set; }

        public int CurrencyId { get; set; }

        public List<CarrierContact> Contacts { get; set; }

    }

    public class CarrierProfile : BaseCarrierProfile
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "WHS_BE_CarrierProfile";

        public bool IsDeleted { get; set; }
    }

    public class CarrierProfileToEdit : BaseCarrierProfile
    {

    }
}
