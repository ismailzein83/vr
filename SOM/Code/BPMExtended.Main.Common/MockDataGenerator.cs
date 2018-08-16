using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Common
{
    public static class MockDataGenerator
    {
        #region Constants

        #region Customers

        const string CUSTOMER_ID_1 = "2408EDDB-ABAB-4507-905C-20386B7EC106";

        const string CUSTOMER_ID_2 = "C0A5D017-9343-43B9-9820-0D813C74B1F4";

        const string CUSTOMER_ID_3 = "448C0FB8-536E-41D0-A178-1218261E6252";

        #endregion

        #region Contracts

        #region Contracts For CUSTOMER_ID_1

        #region Telephony

        const string CONTRACT_CUST1_TL1 = "1AE2BD12-E556-45FE-A432-270E47BDAC84";

        const string CONTRACT_CUST1_TL2 = "7A737546-4423-40C6-B05C-94E5417E7DE1";

        const string CONTRACT_CUST1_TL3 = "853F19B6-280A-4F1A-ADB6-325908B1EE20";

        #endregion

        #region Leased Line

        const string CONTRACT_CUST1_LL1 = "3E01B822-A97D-4BFB-ADDC-997F4C29DEF6";

        const string CONTRACT_CUST1_LL2 = "4D0D89FB-FED1-4CF3-B76C-2E621DDBA0F5";

        const string CONTRACT_CUST1_LL3 = "E3A123FE-689A-4E79-9EF7-6AA5DDAE7F4D";

        #endregion

        #endregion

        #region Contracts For CUSTOMER_ID_2

        #region Leased Line

        const string CONTRACT_CUST2_LL1 = "B14D5937-3660-4C41-A05E-5CD50F1398DA";

        const string CONTRACT_CUST2_LL2 = "1C50C5CB-AF8A-4CE9-ACB4-81A972A78AF0";

        #endregion

        #endregion

        #region Contracts For CUSTOMER_ID_3

        #region Telephony

        const string CONTRACT_CUST3_TL1 = "9CFB08E4-5648-413C-BC81-FC6AB3E17FEE";

        const string CONTRACT_CUST3_TL2 = "5DB582E3-4B06-4E62-B6B3-F573150C8DC7";

        const string CONTRACT_CUST3_TL3 = "57FE0491-7ECF-4F10-8AD5-7365673CA06E";

        const string CONTRACT_CUST3_TL4 = "356A7F17-79F0-4425-B10D-95B8B8EEED0E";

        const string CONTRACT_CUST3_TL5 = "C46F9C59-3E88-4A00-A5BE-D7F5D97797B8";

        const string CONTRACT_CUST3_TL6 = "BEBE12C5-8F83-467C-9103-6B17CB64429A";

        const string CONTRACT_CUST3_TL7 = "D8272FF9-A020-413C-B7FD-A58781934BAD";

        const string CONTRACT_CUST3_TL8 = "2B0E9565-6855-498F-BCEE-D80F4F08C707";

        const string CONTRACT_CUST3_TL9 = "6223ECFD-74D3-421A-AD30-399AF53F868B";

        const string CONTRACT_CUST3_TL10 = "0E9C3B15-8A97-41D6-95C7-D0BBF415711D";

        const string CONTRACT_CUST3_TL11 = "4ECE879B-2525-45A0-8C1A-8EE0B02A557E";

        const string CONTRACT_CUST3_TL12 = "B3ED25BE-93E6-4700-953F-9F4539B434F6";

        #endregion

        #endregion

        #endregion

        #region Phone Numbers

        const string PHONE_NUMBER_1 = "1111";

        #endregion

        #region Customer Categories

        #region Residential

        const string CUSTOMER_CAT_RES_NORMAL = "FE799067-7A29-4BEC-B7D5-06DB32990AA9";

        const string CUSTOMER_CAT_RES_ENG = "0F6CC8CB-4F69-42CB-B16B-563F2A8C2EB7";

        const string CUSTOMER_CAT_RES_MARTYR = "7B015DC2-D30F-4227-ADD1-979DCE67F35C";

        #endregion

        #endregion

        #region Rate Plans

        #region Telephony Rate Plans

        #region Residential

        #region Normal

        const string RP_TL_PSTN_RES_NORMAL = "A90A98EF-721D-4D31-ADDC-F2C99502314E";

        const string RP_TL_ISDN_RES_NORMAL = "27F668F2-34A8-416E-9434-D64CE6ED59EE";

        const string RP_LL_RES_NORMAL = "EA8BE377-EF09-44BB-86C8-0D2DA96D2A16";

        #endregion

        #region Engineer

        const string RP_TL_PSTN_RES_ENG = "99BAD44C-0F7A-4D51-8AA6-F48F1F70666E";

        #endregion

        #region Martyr

        const string RP_TL_PSTN_RES_MAR = "B49AA915-20A8-4B9A-9F37-231931ED9842";

        const string RP_TL_ISDN_RES_MAR = "B5F5458C-FBDD-4B33-8FFE-877C059643AB";

        #endregion

        #endregion

        #endregion

        #endregion

        #region Service Packages

        const string PCKG_CORE_TL = "F75545CD-6C39-449A-938D-EDC84601540E";

        const string PCKG_CORE_LL = "97DC76CF-4D5D-461B-92EE-D5412F025F31";

        const string PCKG_OPT_TL_1 = "8354E635-3ECB-4F29-A58D-F539500702E3";

        const string PCKG_OPT_TL_2 = "C0695121-2C4B-4E9E-B88D-DB759C0454FD";

        const string PCKG_OPT_LL_1 = "5208AB6F-7956-47EE-A1A7-19A407EE8FDD";

        #endregion

        #region Services

        const string SRV_CORE_TL_1 = "321A5488-113C-40AA-B90C-B208E95494A4";

        const string SRV_CORE_TL_2 = "0FE2AA0B-0297-4BD2-B7F8-D3594097A978";

        const string SRV_CORE_LL_1 = "FD329714-61BB-4501-AD8E-7A7824E38377";

        const string SRV_CORE_LL_2 = "E4F10D4B-0E24-46CE-BACD-353E601B075B";

        const string SRV_OPT_TL_1 = "1EFBEB50-C027-43A9-832E-A51CCAB93D91";

        const string SRV_OPT_TL_2 = "E9A4C84D-99ED-4E31-A67B-9B450D183C22";

        const string SRV_OPT_TL_3 = "EE85D0BC-CE96-441A-A0FD-3179026423F5";

        const string SRV_OPT_TL_4 = "759DED16-755B-4786-ADBE-68F9F1596C28";

        const string SRV_OPT_TL_5 = "91297D55-F29D-4E29-A13D-78F94545D063";

        const string SRV_OPT_LL_1 = "450BBD16-E948-4333-B027-130FEB3A7672";

        const string SRV_OPT_LL_2 = "B9E6A499-8086-4647-8ED9-FFB88FCE2420";

        #endregion

        #endregion

        #region Telephony Contract

        public static TelephonyContractDetail GetTelephonyContract(string contractId)
        {
            return GetAllTelephonyContracts().Find(x => x.ContractId.ToLower() == contractId.ToLower());
        }

        public static List<TelephonyContractDetail> GetTelephonyContracts(string customerId)
        {
            return GetAllTelephonyContracts().FindAll(x => x.CustomerId.ToLower() == customerId.ToLower());
        }

        private static List<TelephonyContractDetail> GetAllTelephonyContracts()
        {
            return new List<TelephonyContractDetail>
            {
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST1_TL1,
                     CustomerId = CUSTOMER_ID_1,
                     PhoneNumber= PHONE_NUMBER_1,
                     RatePlanId = RP_TL_PSTN_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     Status = ContractDetailStatus.Active,
                     CreatedTime = DateTime.Today.AddDays(-5),
                     LastModifiedTime = DateTime.Today.AddDays(-3)
                 },
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST1_TL2,
                     CustomerId = CUSTOMER_ID_1,
                     PhoneNumber= PHONE_NUMBER_1,
                     RatePlanId = RP_TL_PSTN_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     Status = ContractDetailStatus.Inactive,
                     CreatedTime = DateTime.Today.AddDays(-4),
                     LastModifiedTime = DateTime.Today
                 },                 
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST1_TL3,
                     CustomerId = CUSTOMER_ID_1,
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_1,
                     RatePlanId = RP_TL_ISDN_RES_NORMAL,
                     RatePlanName = "Normal Plan (ISDN)",
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today
                 },
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST3_TL1,
                     CustomerId = CUSTOMER_ID_3,
                     PhoneNumber= PHONE_NUMBER_1,
                     RatePlanId = RP_LL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     Status = ContractDetailStatus.Active,
                     CreatedTime = DateTime.Today.AddDays(-5),
                     LastModifiedTime = DateTime.Today.AddDays(-3)
                 },
                 new TelephonyContractDetail
                 {
                      ContractId = CONTRACT_CUST3_TL2,
                      CustomerId = CUSTOMER_ID_3,
                      PhoneNumber= PHONE_NUMBER_1,
                      RatePlanId = RP_TL_PSTN_RES_NORMAL,
                      RatePlanName = "Normal Plan",
                      Status = ContractDetailStatus.Inactive,
                      CreatedTime = DateTime.Today.AddDays(-4),
                      LastModifiedTime = DateTime.Today
                 },
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST3_TL3,
                     CustomerId = CUSTOMER_ID_3,
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_1,
                     RatePlanId = RP_TL_PSTN_RES_ENG,
                     RatePlanName = "Engineers Plan",
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today
                 },
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST3_TL4,
                     CustomerId = CUSTOMER_ID_3,
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_1,
                     RatePlanId = RP_TL_PSTN_RES_MAR,
                     RatePlanName = "Family of Martyr Plan",
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today
                 },
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST3_TL5,
                     CustomerId = CUSTOMER_ID_3,
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_1,
                     RatePlanId = RP_TL_ISDN_RES_MAR,
                     RatePlanName = "Family of Martyr Plan (ISDN)",
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today
                 },
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST3_TL6,
                     CustomerId = CUSTOMER_ID_3,
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_1,
                     RatePlanId = RP_LL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today
                 },
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST3_TL7,
                     CustomerId = CUSTOMER_ID_3,
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_1,
                     RatePlanId = RP_LL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today
                 },
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST3_TL8,
                     CustomerId = CUSTOMER_ID_3,
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_1,
                     RatePlanId = RP_LL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today
                 },
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST3_TL9,
                     CustomerId = CUSTOMER_ID_3,
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_1,
                     RatePlanId = RP_LL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today
                 },
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST3_TL10,
                     CustomerId = CUSTOMER_ID_3,
                     RatePlanId = RP_LL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_1,
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today
                 },
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST3_TL11,
                     CustomerId = CUSTOMER_ID_3,
                     RatePlanId = RP_LL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_1,
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today
                 },
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST3_TL12,
                     CustomerId = CUSTOMER_ID_3,
                     RatePlanId = RP_LL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_1,
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today
                 }

            };
        }

        #endregion

        #region Leased Line Contract

        public static LeasedLineContractDetail GetLeasedLineContract(string contractId)
        {
            return GetAllLeasedLineContracts().Find(x => x.ContractId.ToLower() == contractId.ToLower());
        }

        public static List<LeasedLineContractDetail> GetLeasedLineContracts(string customerId)
        {
            return GetAllLeasedLineContracts().FindAll(x => x.CustomerId.ToLower() == customerId.ToLower());
        }

        private static List<LeasedLineContractDetail> GetAllLeasedLineContracts()
        {
            return new List<LeasedLineContractDetail>
            {
                 new LeasedLineContractDetail
                 {
                     ContractId = CONTRACT_CUST1_LL1,
                     CustomerId = CUSTOMER_ID_1,
                     PhoneNumber= PHONE_NUMBER_1,
                     Status = ContractDetailStatus.Active,
                     CreatedTime = DateTime.Today.AddDays(-5),
                     LastModifiedTime = DateTime.Today.AddDays(-3)
                 },
                 new LeasedLineContractDetail
                 {
                     ContractId = CONTRACT_CUST1_LL2,
                     CustomerId = CUSTOMER_ID_1,
                     PhoneNumber= PHONE_NUMBER_1,
                     Status = ContractDetailStatus.Inactive,
                     CreatedTime = DateTime.Today.AddDays(-4),
                     LastModifiedTime = DateTime.Today
                 },
                 new LeasedLineContractDetail
                 {
                     ContractId = CONTRACT_CUST1_LL3,
                     CustomerId = CUSTOMER_ID_1,
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_1,
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today
                 },
                 new LeasedLineContractDetail
                 {
                     ContractId = CONTRACT_CUST2_LL1,
                     CustomerId = CUSTOMER_ID_2,
                     PhoneNumber= PHONE_NUMBER_1,
                     Status = ContractDetailStatus.Active,
                     CreatedTime = DateTime.Today.AddDays(-5),
                     LastModifiedTime = DateTime.Today.AddDays(-3)
                 },
                 new LeasedLineContractDetail
                 {
                     ContractId = CONTRACT_CUST2_LL2,
                     CustomerId = CUSTOMER_ID_2,
                     PhoneNumber= PHONE_NUMBER_1,
                     Status = ContractDetailStatus.Inactive,
                     CreatedTime = DateTime.Today.AddDays(-4),
                     LastModifiedTime = DateTime.Today
                 }
            };
        }

        #endregion

        #region Customer Category

        public static List<CustomerCategoryDetail> GetCustomerCategories(BPMCustomerType customerType)
        {
            return GetAllCustomerCategories().FindAllRecords(x => (int)x.CustomerType == (int)customerType).MapRecords(CustomerCategoryMapper).ToList();
        }

        private static SOM.Main.Entities.CustomerCategory GetCustomerCategory(string customerCategoryId)
        {
            return GetAllCustomerCategories().FindRecord(x => x.CategoryId == customerCategoryId);
        }

        private static IEnumerable<SOM.Main.Entities.CustomerCategory> GetAllCustomerCategories()
        {
            return new List<SOM.Main.Entities.CustomerCategory>
            {
                new SOM.Main.Entities.CustomerCategory
                {
                    CategoryId = CUSTOMER_CAT_RES_NORMAL,
                    Name = "Citizen",
                    CustomerType = SOM.Main.Entities.CustomerType.Residential,
                    IsNormal = true
                },
                new SOM.Main.Entities.CustomerCategory
                {
                    CategoryId = CUSTOMER_CAT_RES_ENG,
                    Name = "Engineer",
                    CustomerType = SOM.Main.Entities.CustomerType.Residential,
                    IsNormal = false
                },
                new SOM.Main.Entities.CustomerCategory
                {
                    CategoryId = CUSTOMER_CAT_RES_MARTYR,
                    Name = "Martyr Family",
                    CustomerType = SOM.Main.Entities.CustomerType.Residential,
                    IsNormal = false
                }
            };
        }

        private static CustomerCategoryDetail CustomerCategoryMapper(SOM.Main.Entities.CustomerCategory category)
        {
            return new CustomerCategoryDetail
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                IsNormal = category.IsNormal
            };
        }

        #endregion

        #region Customer Balance

        public static CustomerBalance GetCustomerBalance(string customerId)
        {
            return GetAllCustomerBalances().Find(x => x.CustomerId == customerId);
        }

        private static List<CustomerBalance> GetAllCustomerBalances()
        {
            return new List<CustomerBalance>
            {
                new CustomerBalance
                { 
                    CustomerId = CUSTOMER_ID_1,
                    Balance = 3520                    
                },
                new CustomerBalance
                {
                    CustomerId = CUSTOMER_ID_2,
                    Balance = 0
                },
                new CustomerBalance
                {
                    CustomerId = CUSTOMER_ID_3,
                    Balance = -500
                }
            };
        }

        #endregion

        #region Rate Plan


        public static List<SOM.Main.Entities.RatePlan> GetRatePlans(SOM.Main.Entities.LineOfBusiness lob, string customerCategoryId, string subType)
        {
            return GetAllRatePlans().FindAllRecords(x => x.LOB == lob && x.Category.CategoryId.ToLower() == customerCategoryId.ToLower()
                && x.SubType.ToLower() == subType.ToLower()).ToList();
        }

        public static SOM.Main.Entities.RatePlan GetRatePlan(string ratePlanId)
        {
            return GetAllRatePlans().FindRecord(x => x.RatePlanId == ratePlanId);
        }

        private static List<SOM.Main.Entities.RatePlan> GetAllRatePlans()
        {
            return new List<SOM.Main.Entities.RatePlan>
            {
                new SOM.Main.Entities.RatePlan
                {
                    RatePlanId = RP_TL_PSTN_RES_NORMAL,
                    Name = "Normal Plan",
                    LOB = SOM.Main.Entities.LineOfBusiness.Telephony,
                    SubType = "PSTN",
                    Category = GetCustomerCategory(CUSTOMER_CAT_RES_NORMAL),
                    CorePackage = GetServicePackage(PCKG_CORE_TL),
                    OptionalPackages = GetServicePackages(new List<string> { PCKG_OPT_TL_1 })
                },
                new SOM.Main.Entities.RatePlan
                {
                    RatePlanId = RP_TL_PSTN_RES_ENG,
                    Name = "Engineers Plan",
                    LOB = SOM.Main.Entities.LineOfBusiness.Telephony,
                    SubType = "PSTN",
                    Category = GetCustomerCategory(CUSTOMER_CAT_RES_ENG),
                    CorePackage = GetServicePackage(PCKG_CORE_TL),
                    OptionalPackages = GetServicePackages(new List<string> { PCKG_OPT_TL_1, PCKG_OPT_TL_2 })
                },
                new SOM.Main.Entities.RatePlan
                {
                    RatePlanId = RP_TL_PSTN_RES_MAR,
                    Name = "Family of Martyr Plan",
                    LOB = SOM.Main.Entities.LineOfBusiness.Telephony,
                    SubType = "PSTN",
                    Category = GetCustomerCategory(CUSTOMER_CAT_RES_MARTYR),
                    CorePackage = GetServicePackage(PCKG_CORE_TL),
                    OptionalPackages = GetServicePackages(new List<string> { PCKG_OPT_TL_1 })
                },
                new SOM.Main.Entities.RatePlan
                {
                    RatePlanId = RP_TL_ISDN_RES_NORMAL,
                    Name = "Normal Plan (ISDN)",
                    LOB = SOM.Main.Entities.LineOfBusiness.Telephony,
                    SubType = "ISDN",
                    Category = GetCustomerCategory(CUSTOMER_CAT_RES_NORMAL),
                    CorePackage = GetServicePackage(PCKG_CORE_TL),
                    OptionalPackages = GetServicePackages(new List<string> { PCKG_OPT_TL_1 })
                },
                new SOM.Main.Entities.RatePlan
                {
                    RatePlanId = RP_TL_ISDN_RES_MAR,
                    Name = "Family of Martyr Plan (ISDN)",
                    LOB = SOM.Main.Entities.LineOfBusiness.Telephony,
                    SubType = "ISDN",
                    Category = GetCustomerCategory(CUSTOMER_CAT_RES_MARTYR),
                    CorePackage = GetServicePackage(PCKG_CORE_TL),
                    OptionalPackages = GetServicePackages(new List<string> { PCKG_OPT_TL_1 })
                },
                new SOM.Main.Entities.RatePlan
                {
                    RatePlanId = RP_LL_RES_NORMAL,
                    Name = "Normal Plan",
                    LOB = SOM.Main.Entities.LineOfBusiness.LeasedLine,
                    Category = GetCustomerCategory(CUSTOMER_CAT_RES_NORMAL),
                    CorePackage = GetServicePackage(PCKG_CORE_LL),
                    OptionalPackages = GetServicePackages(new List<string> { PCKG_OPT_LL_1 })
                }
            };
        }

        #endregion

        #region Service Packages
        
        public static SOM.Main.Entities.ServicePackage GetServicePackage(string packageId)
        {
            return GetAllServicePackages().FindRecord(x => x.PackageId == packageId);
        }

        private static List<SOM.Main.Entities.ServicePackage> GetServicePackages(List<string> packageIds)
        {
            return GetAllServicePackages().FindAllRecords(x => packageIds.Contains(x.PackageId)).ToList();
        }
        
        private static IEnumerable<SOM.Main.Entities.ServicePackage> GetAllServicePackages()
        {
            return new List<SOM.Main.Entities.ServicePackage>
            {
                new SOM.Main.Entities.ServicePackage
                {
                    PackageId = PCKG_CORE_TL,
                    PackageName = "Telephony Line Core Package",
                    Services = GetServices(PCKG_CORE_TL)
                },
                new SOM.Main.Entities.ServicePackage
                {
                    PackageId = PCKG_CORE_LL,
                    PackageName = "Leased Line Core Package",
                    Services = GetServices(PCKG_CORE_LL)
                },
                new SOM.Main.Entities.ServicePackage
                {
                    PackageId = PCKG_OPT_TL_1,
                    PackageName = "Star Package",
                    Services = GetServices(PCKG_OPT_TL_1)
                },
                new SOM.Main.Entities.ServicePackage
                {
                    PackageId = PCKG_OPT_TL_2,
                    PackageName = "Advanced Package",
                    Services = GetServices(PCKG_OPT_TL_2)
                },
                new SOM.Main.Entities.ServicePackage
                {
                    PackageId = PCKG_OPT_LL_1,
                    PackageName = "Star Package",
                    Services = GetServices(PCKG_OPT_LL_1)
                }
            };
        }

        #endregion

        #region Services

        private static List<SOM.Main.Entities.Service> GetServices(string packageId)
        {
            return GetAllServices().FindAllRecords(x => x.PackageId == packageId).ToList();
        }

        public static IEnumerable<SOM.Main.Entities.Service> GetAllServices()
        {
            return new List<SOM.Main.Entities.Service>
            {
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_CORE_TL_1,
                    Name = "Telephony Line Subscription",
                    Description = "Telephony Line Subscription",
                    IsCore = true,
                    IsTelephony = true,
                    SubscriptionFee = 2000,
                    AccessFee = 1500,
                    PackageId = PCKG_CORE_TL
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_CORE_TL_2,
                    Name = "Clip",
                    Description = "Show Caller Id",
                    IsCore = true,
                    IsTelephony = true,
                    SubscriptionFee = 500,
                    AccessFee = 500,
                    PackageId = PCKG_CORE_TL
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_CORE_LL_1,
                    Name = "Leased Line Subscription",
                    Description = "Leased Line Subscription",
                    IsCore = true,
                    SubscriptionFee = 1500,
                    AccessFee = 1000,
                    PackageId = PCKG_CORE_LL
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_CORE_LL_2,
                    Name = "Clip",
                    Description = "Show Caller Id",
                    IsCore = true,
                    SubscriptionFee = 1500,
                    AccessFee = 1000,
                    PackageId = PCKG_CORE_LL
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_TL_1,
                    Name = "Forwarding",
                    Description = "Forwarding in case of no answer",
                    AccessFee = 200,
                    IsTelephony = true,
                    PackageId = PCKG_OPT_TL_1
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_TL_2,
                    Name = "Call Wait/Hold",
                    Description = "Put a call on hold",
                    AccessFee = 350,
                    IsTelephony = true,
                    PackageId = PCKG_OPT_TL_1
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_TL_3,
                    Name = "Call Barring",
                    Description = "Prevent the caller from calling specific area",
                    AccessFee = 400,
                    IsTelephony = true,
                    PackageId = PCKG_OPT_TL_1,
                    ServiceParams = new List<SOM.Main.Entities.ServiceParameter>
                    {
                        new SOM.Main.Entities.ServiceParameter
                        {
                            Id = "1",
                            Name = "National"
                        },
                        new SOM.Main.Entities.ServiceParameter
                        {
                            Id = "2",
                            Name = "International"
                        }
                    }
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_TL_4,
                    Name = "International Dial Block",
                    Description = "Block international calls",
                    AccessFee = 700,
                    IsTelephony = true,
                    PackageId = PCKG_OPT_TL_2
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_TL_5,
                    Name = "CLIR",
                    Description = "Caller Line Identification Restriction",
                    AccessFee = 600,
                    IsTelephony = true,
                    PackageId = PCKG_OPT_TL_2
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_LL_1,
                    Name = "Forwarding",
                    Description = "Forwarding in case of no answer",
                    AccessFee = 100,
                    PackageId = PCKG_OPT_LL_1
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_LL_2,
                    Name = "Call Wait/Hold",
                    Description = "Put a call on hold",
                    AccessFee = 150,
                    PackageId = PCKG_OPT_LL_1
                }
            };
        }

        #endregion

        #region Other Methods

        public static string GetRandomCustomerId()
        {
            List<string> customerIds = new List<string>();
            customerIds.Add(CUSTOMER_ID_1);
            customerIds.Add(CUSTOMER_ID_2);
            customerIds.Add(CUSTOMER_ID_3);

            Random rnd = new Random();
            int index = rnd.Next(0, 2);

            return customerIds[index];
        }

        #endregion

    }
}
