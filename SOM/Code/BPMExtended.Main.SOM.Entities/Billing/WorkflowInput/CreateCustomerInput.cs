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
        public Guid? ContactId { get; set; }

        public Guid? AccountId { get; set; }

        public CreateCustomerContactInfo ContactInfo { get; set; }

        public CreateCustomerAccountInfo AccountInfo { get; set; }

        public class CreateCustomerContactInfo
        {
            public int CustomerCategoryId { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public int CountryId { get; set; }

            public string CityName { get; set; }
        }

        public class CreateCustomerAccountInfo
        {
            public int CustomerCategoryId { get; set; }

            public int CountryId { get; set; }

            public string CityName { get; set; }
        }

        //public string Test()
        //{
        //    var customerNewElement = new CustomerCreateRequest.CustomerCreateRequestCustomerElement();
        //    customerNewElement.prgCode = ContactInfo.CustomerCategoryId;
        //    customerNewElement.paymentResp = true;

        //    var addressWriteElement = new CustomerCreateRequest.CustomerCreateRequestAddressElement();
        //    addressWriteElement.adrSeq = 0;
        //    addressWriteElement.adrFname = ContactInfo.FirstName;
        //    addressWriteElement.adrLname = ContactInfo.LastName;
        //    addressWriteElement.countryId = ContactInfo.CountryId;
        //    addressWriteElement.adrCity = ContactInfo.CityName;

        //    CustomerCreateRequest request = new CustomerCreateRequest();
        //    request.inputAttributes = new CustomerCreateRequest.CustomerCreateInputAttributes();
        //    request.inputAttributes.customerNew = customerNewElement;
        //    request.inputAttributes.addresses = new List<CustomerCreateRequest.CustomerCreateRequestAddressElement>();
        //    request.inputAttributes.addresses.Add(addressWriteElement);

        //    return "serialized request";
        //}
    }

    #region To Be Removed

    //public class CustomerCreateRequest : BaseRequest
    //{
    //    public CustomerCreateInputAttributes inputAttributes { get; set; }

    //    public class CustomerCreateInputAttributes
    //    {
    //        public CustomerCreateRequestCustomerElement customerNew { get; set; }

    //        public List<CustomerCreateRequestAddressElement> addresses { get; set; }
    //    }

    //    public class CustomerCreateRequestCustomerElement
    //    {
    //        public bool paymentResp { get; set; }

    //        public int prgCode { get; set; }
    //    }

    //    public class CustomerCreateRequestAddressElement
    //    {
    //        public long adrSeq { get; set; }

    //        public string adrFname { get; set; }

    //        public string adrLname { get; set; }

    //        public int countryId { get; set; }

    //        public string adrCity { get; set; }

    //    }
    //}

    //public class laMemberCreateRequest : BaseRequest
    //{

    //}

    //public class BaseRequest
    //{
    //    public Dictionary<string,string> sessionChangeRequest { get; set; }

    //    public BaseRequest()
    //    {
    //        this.sessionChangeRequest = new Dictionary<string, string>();
    //        this.sessionChangeRequest.Add("BU_ID", "2");
    //    }
    //}

    //public class CustomerCreateResponse : BaseResponse
    //{
    //    public CustomerCreateResponseCustomerElement customerNew { get; set; }

    //    public List<CustomerCreateResponseAddressElement> addresses { get; set; }

    //    public class CustomerCreateResponseCustomerElement
    //    {
    //        public string csId { get; set; }

    //        public string csIdPub { get; set; }
    //    }

    //    public class CustomerCreateResponseAddressElement
    //    {
    //        public long adrSeq { get; set; }
    //    }
    //}

    //public class BaseResponse
    //{

    //}

    #endregion
}
