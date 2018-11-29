using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Common
{
    public static class RatePlanMockDataGenerator
    {
        #region Constants

        #region ISPs

        const string ISP_ID_1 = "290998CB-5A66-46A5-869B-DBD35BF6588D";
        const string ISP_ID_2 = "DACD473D-85F0-4A73-B202-141094C4328D";
        const string ISP_ID_3 = "ADC2FE86-63E7-4170-BC07-4EBD66110E52";
        const string ISP_ID_4 = "F3DF641B-5D95-46B6-861C-8791207C3A60";

        #endregion

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

        const string CONTRACT_CUST1_TL4 = "41AD81A0-9FD1-4092-9473-355F8245A1B2}";

        const string CONTRACT_CUST1_TL5 = "7A94C45E-1E76-49A8-AE5B-FC906F70B8AD";

        const string CONTRACT_CUST1_TL6 = "F48899F0-A1EE-4FC1-9789-C5624AC7F47E";

        const string CONTRACT_CUST1_TL7 = "531A3E2F-A149-457F-8213-5D28CFDD052A";

        #endregion

        #region Leased Line

        const string CONTRACT_CUST1_LL1 = "3E01B822-A97D-4BFB-ADDC-997F4C29DEF6";

        const string CONTRACT_CUST1_LL2 = "4D0D89FB-FED1-4CF3-B76C-2E621DDBA0F5";

        const string CONTRACT_CUST1_LL3 = "E3A123FE-689A-4E79-9EF7-6AA5DDAE7F4D";

        #endregion

        #region ADSL

        const string CONTRACT_CUST1_ADSL1 = "DDC2177F-D398-4C2C-816F-72AD9750BE5B";

        const string CONTRACT_CUST1_ADSL2 = "C372E36B-ED71-4B63-8C77-C42F2CBEBB68";

        const string CONTRACT_CUST1_ADSL3 = "BF6FA071-BF08-4BF1-9689-06973D5BB5B5";

        #endregion

        #region GSHDSL

        const string CONTRACT_CUST1_GSHDSL1 = "BF21223A-50A2-4386-B63A-A1BCEB16C875";

        const string CONTRACT_CUST1_GSHDSL2 = "589A4F27-58A7-400D-A983-7A6C8D876FC0";


        #endregion


        #endregion

        #region Contracts For CUSTOMER_ID_2

        #region Leased Line

        const string CONTRACT_CUST2_LL1 = "B14D5937-3660-4C41-A05E-5CD50F1398DA";

        const string CONTRACT_CUST2_LL2 = "1C50C5CB-AF8A-4CE9-ACB4-81A972A78AF0";

        #endregion

        #region ADSL

        const string CONTRACT_CUST2_ADSL1 = "8B105141-9800-4922-9627-72B64DD21950";

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

        #region ADSL

        const string CONTRACT_CUST3_ADSL1 = "53D73D55-494F-4A78-A3D9-B632D95E1ECF";

        #endregion

        #endregion

        #endregion

        #region Phone Numbers

        const string PHONE_NUMBER_1 = "1111";
        const string PHONE_NUMBER_2 = "2222";
        const string PHONE_NUMBER_3 = "500500";
        const string PHONE_NUMBER_4 = "500501";
        const string PHONE_NUMBER_5 = "500502";
        const string PHONE_NUMBER_6 = "500504";
        const string PHONE_NUMBER_7 = "500503";

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

        const string RP_TL_RES_NORMAL = "EA8BE377-EF09-44BB-86C8-0D2DA96D2A16";

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

        #region Leased Line Rate Plans

        #region Residential

        #region Normal

        const string RP_LL_RES_NORMAL = "bbd6ee50-5ed7-4d93-8e9b-1a875213429c";

        #endregion

        #region Engineer

        const string RP_LL_RES_ENG = "6FEB17A6-C483-4136-B3CF-44EC998537A0";

        #endregion

        #region Martyr

        const string RP_LL_RES_MAR = "E3C008F3-3838-467C-9C70-EAC2ADF2D930";

        #endregion

        #endregion

        #endregion

        #region ADSL Rate Plans

        #region Residential

        #region Normal

        const string RP_ADSL_RES_NORMAL = "9A862D6E-6A40-409C-8677-618B44FD5BA3";

        #endregion

        #region Student

        const string RP_ADSL_RES_STUDENT = "8D2E19A3-C48F-4ED0-958C-7EE74EAAB1B0";

        #endregion

        #endregion

        #endregion

        #region GSHDSL Rate Plans

        #region Residential

        #region Normal

        const string RP_GSHDSL_RES_NORMAL = "C5B0FDAD-4F95-45CE-B0B3-07A7320E9EE6";

        #endregion

        #region Student

        const string RP_GSHDSL_RES_STUDENT = "E18CDA05-FB3F-417C-B9D1-FF2556325295";

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

        const string PCKG_CORE_ADSL_1 = "D8968BAD-340E-4F4E-800D-A6948FA009DE";

        const string PCKG_OPT_ADSL_1 = "47EED491-7787-4962-9373-6AE91883487E";

        const string PCKG_OPT_ADSL_2 = "BCEF4F11-F3A5-4F75-9D58-A5F36237B4FC";

        const string PCKG_CORE_GSHDSL_1 = "02519BD1-D8C9-481E-81E4-166CBE839F60";

        const string PCKG_OPT_GSHDSL_1 = "A968A7CD-91C6-47B1-9098-E44CD0C752F2";

        const string PCKG_OPT_GSHDSL_2 = "A4C71537-04BA-4988-B663-08904A7221B2";

        const string PCKG_OPT_GSHDSL_3 = "6F1498D7-5BAF-43E8-B260-69645825A688";

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

        const string SRV_OPT_TL_6 = "A75C1111-5953-46DB-B024-68CF8A34ECD3";

        const string SRV_OPT_LL_1 = "450BBD16-E948-4333-B027-130FEB3A7672";

        const string SRV_OPT_LL_2 = "B9E6A499-8086-4647-8ED9-FFB88FCE2420";

        const string SRV_OPT_LL_3 = "576D1A72-1227-457D-A04B-4B259C293E48";

        const string SRV_OPT_LL_4 = "2BE8E748-2DAC-4631-8FC0-7B8612FF39FC";

        const string SRV_CORE_ADSL_1 = "F91FA9AB-6CBE-47FB-845C-F1852B21C19C";

        const string SRV_CORE_ADSL_2 = "B6D2326E-4C6B-4BA9-A787-0919EEC12335";

        const string SRV_OPT_ADSL_1 = "873F612D-CB86-44D4-A2B0-25501F687067";

        const string SRV_OPT_ADSL_2 = "6EBB601B-2A76-4067-8EF7-27561813E511";

        const string SRV_OPT_ADSL_3 = "87CA1D3C-72FF-44B5-84AD-792DA0C6B3DF";

        const string SRV_CORE_GSHDSL_1 = "378314F2-5305-4C04-A0ED-F5A9398046AA";

        const string SRV_CORE_GSHDSL_2 = "88D9C20A-3EF5-4AD8-BF60-7207696FAE6D";

        const string SRV_OPT_GSHDSL_1 = "53B0E31B-BFF2-4331-8AE6-0ADDB524550F";

        const string SRV_OPT_GSHDSL_2 = "CEC79DC9-B119-416F-AC9E-E88ADD775610";

        const string SRV_OPT_GSHDSL_3 = "86C91744-FBEB-4A94-B85B-8B08767E8306";

        #endregion

        #region ADSL Speeds

        const string ADSL_SPEED_1 = "40 MB/s";

        const string ADSL_SPEED_2 = "10 MB/s";

        const string ADSL_SPEED_3 = "20 MB/s";

        const string ADSL_SPEED_4 = "30 MB/s";

        const string ADSL_SPEED_5 = "50 MB/s";

        const string ADSL_SPEED_6 = "60 MB/s";

        const string ADSL_SPEED_7 = "70 MB/s";

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
        public static List<TelephonyContractDetail> GetTelephonyContractsByNumber(string phoneNumber)
        {
            return GetAllTelephonyContracts().FindAll(x => x.PhoneNumber.ToLower() == phoneNumber.ToLower());
        }
        private static List<TelephonyContractDetail> GetAllTelephonyContracts()
        {
            return new List<TelephonyContractDetail>
            {
                 new TelephonyContractDetail
                 { 
                     ContractId = CONTRACT_CUST1_TL1,
                     CustomerId = CUSTOMER_ID_1,
                     PhoneNumber= PHONE_NUMBER_2,
                     RatePlanId = RP_TL_PSTN_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     Address = "Haret hreik",
                     ContractAddress = new Address () {Province ="prov1" , Building = "b1" , City="beirut" , Floor = "2", Region="R1", HouseNumber="2", Street="st1" , SubRegion="sub1"},
                     Status =  ContractDetailStatus.Active,
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     CreatedTime = DateTime.Today.AddDays(-5),
                     LastModifiedTime = DateTime.Today.AddDays(-3),
                     ContractBalance = 100,
                     UnbilledAmount = 2002,
                     Promotions = "promo1",
                     FreeUnit = "unit1"

                 },
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST1_TL2,
                     CustomerId = CUSTOMER_ID_1,
                     PhoneNumber= PHONE_NUMBER_1,
                     RatePlanId = RP_TL_PSTN_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     Address = "Beirut",
                     ContractAddress = new Address () {Province ="prov2" , Building = "b2" , City="beirut" , Floor = "2", Region="R2", HouseNumber="4", Street="st2" , SubRegion="sub2"},
                     Status = ContractDetailStatus.Inactive,
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     CreatedTime = DateTime.Today.AddDays(-4),
                     LastModifiedTime = DateTime.Today,
                      ContractBalance = 200,
                     UnbilledAmount = 4002,
                     Promotions = "promo2",
                     FreeUnit = "unit2"
                 },                 
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST1_TL3,
                     CustomerId = CUSTOMER_ID_1,
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_3,
                     RatePlanId = RP_TL_ISDN_RES_NORMAL,
                     Address = "Beirut",
                     ContractAddress = new Address () {Province ="prov3" , Building = "b3" , City="beirut" , Floor = "5", Region="R3", HouseNumber="6", Street="st3" , SubRegion="sub3"},
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     RatePlanName = "Normal Plan (ISDN)",
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today,
                      ContractBalance = 10,
                     UnbilledAmount = 200,
                     Promotions = "promo3",
                     FreeUnit = "unit3"
                 },
                  new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST1_TL4,
                     CustomerId = CUSTOMER_ID_1,
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_4,
                     RatePlanId = RP_TL_ISDN_RES_NORMAL,
                     Address = "Hamra",
                     ContractAddress = new Address () {Province ="prov4" , Building = "b4" , City="beirut" , Floor = "1", Region="R4", HouseNumber="7", Street="st4" , SubRegion="sub4"},
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     RatePlanName = "Normal Plan (ISDN)",
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today,
                      ContractBalance = 500,
                     UnbilledAmount = 1002,
                     Promotions = "promo4",
                     FreeUnit = "unit4"
                 },
                  new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST1_TL5,
                     CustomerId = CUSTOMER_ID_1,
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_5,
                     RatePlanId = RP_TL_PSTN_RES_NORMAL,
                     Address = "Tyr",
                     ContractAddress = new Address () {Province ="prov5" , Building = "b5" , City="beirut" , Floor = "1", Region="R5", HouseNumber="2", Street="st5" , SubRegion="sub5"},
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     RatePlanName = "Normal Plan (ISDN)",
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today,
                      ContractBalance = 150,
                     UnbilledAmount = 202,
                     Promotions = "promo5",
                     FreeUnit = "unit5"
                 },            
                 new TelephonyContractDetail
                 {
                     ContractId = CONTRACT_CUST3_TL1,
                     CustomerId = CUSTOMER_ID_3,
                     PhoneNumber= PHONE_NUMBER_6,
                     RatePlanId = RP_TL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     Address = "Tyr",
                     ContractAddress = new Address () {Province ="prov6" , Building = "b6" , City="beirut" , Floor = "9", Region="R6", HouseNumber="8", Street="st6" , SubRegion="sub6"},
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     Status = ContractDetailStatus.Active,
                     CreatedTime = DateTime.Today.AddDays(-5),
                     LastModifiedTime = DateTime.Today.AddDays(-3),
                     ContractBalance = 1700,
                     UnbilledAmount = 27002,
                     Promotions = "promo6",
                     FreeUnit = "unit6"
                 },
                 new TelephonyContractDetail
                 {
                      ContractId = CONTRACT_CUST3_TL2,
                      CustomerId = CUSTOMER_ID_3,
                      PhoneNumber= PHONE_NUMBER_7,
                      RatePlanId = RP_TL_PSTN_RES_NORMAL,
                      RatePlanName = "Normal Plan",
                      Address = "saida",
                      ContractAddress = new Address () {Province ="prov7" , Building = "b7" , City="beirut" , Floor = "9", Region="R7", HouseNumber="6", Street="st7" , SubRegion="sub7"},
                      ActivationDate = DateTime.Today.AddDays(-10),
                      StatusDate = DateTime.Today.AddDays(-9),
                      Status = ContractDetailStatus.Inactive,
                      CreatedTime = DateTime.Today.AddDays(-4),
                      LastModifiedTime = DateTime.Today,
                      ContractBalance = 110,
                      UnbilledAmount = 2092,
                      Promotions = "promo7",
                      FreeUnit = "unit7"
                 }

            };
        }

        public static List<TelephonyContractInfo> GetTelephonyContractsInfo(string customerId)
        {
            return GetAllTelephonyContractsInfo().FindAll(x => x.CustomerId.ToLower() == customerId.ToLower());
        }

        private static List<TelephonyContractInfo> GetAllTelephonyContractsInfo()
        {
            return new List<TelephonyContractInfo>
            {
                 new TelephonyContractInfo
                 {
                     ContractId = CONTRACT_CUST1_TL1,
                     CustomerId = CUSTOMER_ID_1,
                     PhoneNumber= PHONE_NUMBER_2
                 },
                 new TelephonyContractInfo
                 {
                     ContractId = CONTRACT_CUST1_TL2,
                     CustomerId = CUSTOMER_ID_1,
                     PhoneNumber= PHONE_NUMBER_1
                 },                 
                 new TelephonyContractInfo
                 {
                     ContractId = CONTRACT_CUST1_TL3,
                     CustomerId = CUSTOMER_ID_1,
                     PhoneNumber= PHONE_NUMBER_3
                 },
                 new TelephonyContractInfo
                 {
                     ContractId = CONTRACT_CUST3_TL1,
                     CustomerId = CUSTOMER_ID_3,
                     PhoneNumber= PHONE_NUMBER_6
                 },
                 new TelephonyContractInfo
                 {
                      ContractId = CONTRACT_CUST3_TL2,
                      CustomerId = CUSTOMER_ID_3,
                      PhoneNumber= PHONE_NUMBER_7
                 }

            };
        }

        #endregion


        #region GSHDSL Contract

        public static GSHDSLContractDetail GetGSHDSLContract(string contractId)
        {
            return GetAllGSHDSLContracts().Find(x => x.ContractId.ToLower() == contractId.ToLower());
        }

        public static List<GSHDSLContractDetail> GetGSHDSLContracts(string customerId)
        {
            return GetAllGSHDSLContracts().FindAll(x => x.CustomerId.ToLower() == customerId.ToLower());
        }


        private static List<GSHDSLContractDetail> GetAllGSHDSLContracts()
        {
            return new List<GSHDSLContractDetail>
            {
                 new GSHDSLContractDetail
                 {
                     ContractId = CONTRACT_CUST1_GSHDSL1,
                     CustomerId = CUSTOMER_ID_1,
                     PhoneNumber= PHONE_NUMBER_1,
                     RatePlanId = RP_GSHDSL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     CSO = "None",
                     Status = ContractDetailStatus.Active,
                     CreatedTime = DateTime.Today.AddDays(-5),
                     LastModifiedTime = DateTime.Today.AddDays(-3),
                     ContractAddress = new Address () {Province ="prov4" , Building = "b4" , City="beirut" , Floor = "1", Region="R4", HouseNumber="7", Street="st4" , SubRegion="sub4"},
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     ContractBalance = 110,
                     UnbilledAmount = 2092,
                     Promotions = "promo1",
                     FreeUnit = "unit1"
                 },
                  new GSHDSLContractDetail
                 {
                     ContractId = CONTRACT_CUST1_GSHDSL2,
                     CustomerId = CUSTOMER_ID_1,
                     PhoneNumber= PHONE_NUMBER_1,
                     RatePlanId = RP_GSHDSL_RES_STUDENT,
                     RatePlanName = "Student Plan",
                     CSO = "None",
                     Status = ContractDetailStatus.Active,
                     CreatedTime = DateTime.Today.AddDays(-5),
                     LastModifiedTime = DateTime.Today.AddDays(-3),
                     ContractAddress = new Address () {Province ="prov9" , Building = "b9" , City="beirut" , Floor = "9", Region="R4", HouseNumber="7", Street="st4" , SubRegion="sub4"},
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                      ContractBalance = 21,
                     UnbilledAmount = 44,
                     Promotions = "promo2",
                     FreeUnit = "unit2"
                 }

            };


        }

        #endregion


                #region Pabx

            public static List<PabxContractDetail> GetPabxContracts(string customerId)
        {
            return GetAllPabxContracts().FindAll(x => x.CustomerId.ToLower() == customerId.ToLower());
        }

        public static List<PabxContractDetail> GetFilteredPabxContracts(string customerId, string contractId)
        {
            return GetAllPabxContracts().FindAll(x => x.CustomerId.ToLower() == customerId.ToLower() && x.ContractId != contractId);
        }

        public static List<PabxContractDetail> checkIfContactPilot(string customerId, string contractId)
        {
            return GetAllPabxContracts().FindAll(x => x.CustomerId.ToLower() == customerId.ToLower() && x.ContractId.ToLower() == contractId.ToLower() && x.IsPilot == true);
        }

        private static List<PabxContractDetail> GetAllPabxContracts()
        {

            return new List<PabxContractDetail>
            {
                 new PabxContractDetail
                 {
                     ContractId = CONTRACT_CUST1_TL1,
                     CustomerId = CUSTOMER_ID_1,
                     Address = "Haret hreik",
                     IsPilot = true,
                     PhoneNumber =PHONE_NUMBER_2,
                     PabxOption = PabxPhoneNumberOption.MakeCall                    

                 },
                  new PabxContractDetail
                 {
                     ContractId = CONTRACT_CUST1_TL2,
                     CustomerId = CUSTOMER_ID_1,
                     Address = "Beirut",
                     IsPilot = false,
                     PhoneNumber = PHONE_NUMBER_1,
                     PabxOption = PabxPhoneNumberOption.Both                    

                 },
               
                  new PabxContractDetail
                 {
                     ContractId = CONTRACT_CUST3_TL1,
                     CustomerId = CUSTOMER_ID_3,
                     Address = "Tyr",
                     IsPilot = false,
                     PhoneNumber = PHONE_NUMBER_6,
                     PabxOption = PabxPhoneNumberOption.ReceiveCall   
                 },
                  new PabxContractDetail
                 {
                     ContractId = CONTRACT_CUST3_TL2,
                     CustomerId = CUSTOMER_ID_3,
                     Address = "Saida",
                     IsPilot = false,
                     PhoneNumber = PHONE_NUMBER_7,
                     PabxOption = PabxPhoneNumberOption.ReceiveCall   
                 }

            };

        }

        #endregion


        #region ISP


         public static List<ISPInfo> GetAllISPInfo()
        {
            return new List<ISPInfo>
            {
                 new ISPInfo
                 {
                     Id = ISP_ID_1,
                     Name = "Terranet"                
                 },
                 new ISPInfo
                 {
                     Id = ISP_ID_2,
                     Name = "BSNL"                
                 },
                 new ISPInfo
                 {
                     Id = ISP_ID_3,
                     Name = "Comcast"                
                 },
                 new ISPInfo
                 {
                     Id = ISP_ID_4,
                     Name = "Megapath"                
                 }

            };

         }

        #endregion

        #region ADSL Contract

        public static ADSLContractDetail GetADSLContract(string contractId)
        {
            return GetAllADSLContracts().Find(x => x.ContractId.ToLower() == contractId.ToLower());
        }

        public static List<ADSLContractDetail> GetADSLContracts(string customerId)
        {
            return GetAllADSLContracts().FindAll(x => x.CustomerId.ToLower() == customerId.ToLower());
        }
        public static List<ADSLContractDetail> GetADSLContractsByUsername(string userName)
        {
            return GetAllADSLContracts().FindAll(x => x.UserName.ToLower() == userName.ToLower());
        }
        public static List<ADSLSpeedInfo> GetAllADSLSpeedInfo()
        {
            return new List<ADSLSpeedInfo>
            {
                 new ADSLSpeedInfo
                 {
                     Id = "1",
                     Name = ADSL_SPEED_1
                   
                 },
                  new ADSLSpeedInfo
                 {
                     Id = "2",
                     Name = ADSL_SPEED_2
                   
                 },
                  new ADSLSpeedInfo
                 {
                     Id = "3",
                     Name = ADSL_SPEED_3
                   
                 },
                  new ADSLSpeedInfo
                 {
                     Id = "4",
                     Name = ADSL_SPEED_4
                   
                 },
                  new ADSLSpeedInfo
                 {
                     Id = "5",
                     Name = ADSL_SPEED_5
                   
                 },
                  new ADSLSpeedInfo
                 {
                     Id = "6",
                     Name = ADSL_SPEED_6
                   
                 },
                  new ADSLSpeedInfo
                 {
                     Id = "7",
                     Name = ADSL_SPEED_7
                   
                 }

            };
        }

        private static List<ADSLContractDetail> GetAllADSLContracts()
        {
            return new List<ADSLContractDetail>
            {
                 new ADSLContractDetail
                 {
                     ContractId = CONTRACT_CUST1_ADSL1,
                     TelephonyContractId = CONTRACT_CUST1_TL2,
                     CustomerId = CUSTOMER_ID_1,
                     PhoneNumber= PHONE_NUMBER_1,
                     RatePlanId = RP_ADSL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     UserName = "Test1",
                     Password = "pass1",
                     Speed = ADSL_SPEED_1,
                     CSO = "None",
                     DSLAMPort="2",
                     ProviderOfDSLAMPort="Huawei",
                     SubscriptionDate ="2018/02/02",
                     ReservedLinePath = "path1",
                     Status = ContractDetailStatus.Active,
                     CreatedTime = DateTime.Today.AddDays(-5),
                     LastModifiedTime = DateTime.Today.AddDays(-3),
                     ContractAddress = new Address () {Province ="prov4" , Building = "b4" , City="beirut" , Floor = "1", Region="R4", HouseNumber="7", Street="st4" , SubRegion="sub4"},
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     ContractBalance = 500,
                     UnbilledAmount = 1002,
                     Promotions = "promo41",
                     FreeUnit = "unit41"
                 },
                 new ADSLContractDetail
                 {
                     ContractId = CONTRACT_CUST1_ADSL2,
                     TelephonyContractId = CONTRACT_CUST1_TL1,
                     CustomerId = CUSTOMER_ID_1,
                     PhoneNumber= PHONE_NUMBER_2,
                     RatePlanId = RP_ADSL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     UserName = "Test2",
                     Password = "pass2",
                     Speed = ADSL_SPEED_2,
                     Status = ContractDetailStatus.Inactive,
                     CreatedTime = DateTime.Today.AddDays(-4),
                     LastModifiedTime = DateTime.Today,
                     ContractAddress = new Address () {Province ="prov44" , Building = "b44" , City="beirut" , Floor = "1", Region="R4", HouseNumber="2", Street="st4" , SubRegion="sub4"},
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     ContractBalance = 500,
                     UnbilledAmount = 1002,
                     Promotions = "promo45",
                     FreeUnit = "unit45"
                 },                 
                 new ADSLContractDetail
                 {
                     ContractId = CONTRACT_CUST1_ADSL3,
                     TelephonyContractId = CONTRACT_CUST1_TL3,
                     CustomerId = CUSTOMER_ID_1,
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_3,
                     UserName = "Test3",
                     Password = "pass3",
                     Speed = ADSL_SPEED_3,
                     RatePlanId = RP_ADSL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today,
                     ContractAddress = new Address () {Province ="prov4" , Building = "b4" , City="beirut" , Floor = "2", Region="R4", HouseNumber="7", Street="st4" , SubRegion="sub4"},
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     ContractBalance = 500,
                     UnbilledAmount = 1002,
                     Promotions = "promo74",
                     FreeUnit = "unit74"
                 },
                 new ADSLContractDetail
                 {
                     ContractId = CONTRACT_CUST3_ADSL1,
                     TelephonyContractId = CONTRACT_CUST3_TL1,
                     CustomerId = CUSTOMER_ID_3,
                     PhoneNumber= PHONE_NUMBER_6,
                     RatePlanId = RP_ADSL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     UserName = "Test4",
                     Password = "pass4",
                     Speed = ADSL_SPEED_4,
                     Status = ContractDetailStatus.Active,
                     CreatedTime = DateTime.Today.AddDays(-5),
                     LastModifiedTime = DateTime.Today.AddDays(-3),
                     ContractAddress = new Address () {Province ="prov4" , Building = "b4" , City="beirut" , Floor = "1", Region="R4", HouseNumber="7", Street="st4" , SubRegion="sub4"},
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     ContractBalance = 500,
                     UnbilledAmount = 1002,
                     Promotions = "promo9",
                     FreeUnit = "unit9"
                 },
                 new ADSLContractDetail
                 {
                      ContractId = CONTRACT_CUST2_ADSL1,
                      TelephonyContractId = CONTRACT_CUST3_TL2,
                      CustomerId = CUSTOMER_ID_3,
                      PhoneNumber= PHONE_NUMBER_7,
                      RatePlanId = RP_ADSL_RES_STUDENT,
                      RatePlanName = "Student Plan",
                      UserName = "Test5",
                      Password = "pass5",
                      Speed = ADSL_SPEED_5,
                      Status = ContractDetailStatus.Inactive,
                      CreatedTime = DateTime.Today.AddDays(-4),
                      LastModifiedTime = DateTime.Today,
                      ContractAddress = new Address () {Province ="prov4" , Building = "b4" , City="beirut" , Floor = "7", Region="R4", HouseNumber="8", Street="st4" , SubRegion="sub4"},
                      ActivationDate = DateTime.Today.AddDays(-10),
                      StatusDate = DateTime.Today.AddDays(-9),
                      ContractBalance = 500,
                      UnbilledAmount = 1002,
                      Promotions = "promo87",
                      FreeUnit = "unit87"
                 }

            };
        }

        #endregion

        #region Leased Line Contract

        public static LeasedLineContractDetail  GetLeasedLineContract(string contractId)
        {
            return GetAllLeasedLineContracts().Find(x => x.ContractId.ToLower() == contractId.ToLower());
        }

        public static List<LeasedLineContractDetail> GetLeasedLineContracts(string customerId)
        {
            return GetAllLeasedLineContracts().FindAll(x => x.CustomerId.ToLower() == customerId.ToLower());
        }
        public static List<LeasedLineContractDetail> GetLeasedLineContractsByNumber(string phoneNumber)
        {
            return GetAllLeasedLineContracts().FindAll(x => x.PhoneNumber.ToLower() == phoneNumber.ToLower());
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
                     LastModifiedTime = DateTime.Today.AddDays(-3),
                     RatePlanId = RP_LL_RES_ENG,
                     RatePlanName = "Engineers Plan",
                     ContractAddress = new Address () {Province ="prov1" , Building = "b1" , City="beirut" , Floor = "9", Region="R1", HouseNumber="6", Street="st1" , SubRegion="su1"},
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     ContractBalance = 110,
                     UnbilledAmount = 2092,
                     Promotions = "promo1",
                     FreeUnit = "unit1"
                 },
                 new LeasedLineContractDetail
                 {
                     ContractId = CONTRACT_CUST1_LL2,
                     CustomerId = CUSTOMER_ID_1,
                     PhoneNumber= PHONE_NUMBER_1,
                     Status = ContractDetailStatus.Inactive,
                     CreatedTime = DateTime.Today.AddDays(-4),
                     LastModifiedTime = DateTime.Today,
                     RatePlanId = RP_LL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     ContractAddress = new Address () {Province ="prov2" , Building = "b2" , City="beirut" , Floor = "9", Region="R2", HouseNumber="6", Street="st2" , SubRegion="sub2"},
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     ContractBalance = 22,
                     UnbilledAmount = 1111,
                     Promotions = "promo2",
                     FreeUnit = "unit2"
                 },
                 new LeasedLineContractDetail
                 {
                     ContractId = CONTRACT_CUST1_LL3,
                     CustomerId = CUSTOMER_ID_1,
                     Status = ContractDetailStatus.Active,
                     PhoneNumber= PHONE_NUMBER_1,
                     CreatedTime = DateTime.Today,
                     LastModifiedTime = DateTime.Today,
                     RatePlanId = RP_LL_RES_MAR,
                     RatePlanName = "Family of Martyr Plan (ISDN)",
                     ContractAddress = new Address () {Province ="prov21" , Building = "b21" , City="beirut" , Floor = "9", Region="R21", HouseNumber="6", Street="st21" , SubRegion="sub21"},
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     ContractBalance = 90,
                     UnbilledAmount = 99,
                     Promotions = "promo21",
                     FreeUnit = "unit21"
                 },
                 new LeasedLineContractDetail
                 {
                     ContractId = CONTRACT_CUST2_LL1,
                     CustomerId = CUSTOMER_ID_2,
                     PhoneNumber= PHONE_NUMBER_1,
                     Status = ContractDetailStatus.Active,
                     CreatedTime = DateTime.Today.AddDays(-5),
                     LastModifiedTime = DateTime.Today.AddDays(-3),
                     RatePlanId = RP_LL_RES_MAR,
                     RatePlanName = "Family of Martyr Plan (ISDN)",
                     ContractAddress = new Address () {Province ="prov211" , Building = "b211" , City="beirut" , Floor = "9", Region="R211", HouseNumber="6", Street="st211" , SubRegion="sub211"},
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     ContractBalance = 90,
                     UnbilledAmount = 99,
                     Promotions = "promo211",
                     FreeUnit = "unit211"
                 },
                 new LeasedLineContractDetail
                 {
                     ContractId = CONTRACT_CUST2_LL2,
                     CustomerId = CUSTOMER_ID_2,
                     PhoneNumber= PHONE_NUMBER_1,
                     Status = ContractDetailStatus.Inactive,
                     CreatedTime = DateTime.Today.AddDays(-4),
                     LastModifiedTime = DateTime.Today,
                     RatePlanId = RP_LL_RES_NORMAL,
                     RatePlanName = "Normal Plan",
                     ContractAddress = new Address () {Province ="prov9" , Building = "b9" , City="beirut" , Floor = "9", Region="R9", HouseNumber="6", Street="st9" , SubRegion="sub9"},
                     ActivationDate = DateTime.Today.AddDays(-10),
                     StatusDate = DateTime.Today.AddDays(-9),
                     ContractBalance = 2290,
                     UnbilledAmount = 2222,
                     Promotions = "promo9",
                     FreeUnit = "unit9"
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
            if (subType == null)
                return GetAllRatePlans().FindAllRecords(x => x.LOB == lob && x.Category.CategoryId.ToLower() == customerCategoryId.ToLower()).ToList();
            else
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
                },
                new SOM.Main.Entities.RatePlan
                {
                    RatePlanId = RP_LL_RES_ENG,
                    Name = "Engineers Plan",
                    LOB = SOM.Main.Entities.LineOfBusiness.LeasedLine,
                    Category = GetCustomerCategory(CUSTOMER_CAT_RES_ENG),
                    CorePackage = GetServicePackage(PCKG_CORE_LL),
                    OptionalPackages = GetServicePackages(new List<string> { PCKG_OPT_LL_1 })
                },
                new SOM.Main.Entities.RatePlan
                {
                    RatePlanId = RP_LL_RES_MAR,
                    Name = "Family of Martyr Plan",
                    LOB = SOM.Main.Entities.LineOfBusiness.LeasedLine,
                    Category = GetCustomerCategory(CUSTOMER_CAT_RES_MARTYR),
                    CorePackage = GetServicePackage(PCKG_CORE_LL),
                    OptionalPackages = GetServicePackages(new List<string> { PCKG_OPT_LL_1 })
                },
                new SOM.Main.Entities.RatePlan
                {
                    RatePlanId = RP_ADSL_RES_NORMAL,
                    Name = "Normal Plan",
                    LOB = SOM.Main.Entities.LineOfBusiness.ADSL,
                    Category = GetCustomerCategory(CUSTOMER_CAT_RES_NORMAL),
                    CorePackage = GetServicePackage(PCKG_CORE_ADSL_1),
                    OptionalPackages = GetServicePackages(new List<string> { PCKG_OPT_ADSL_1 })
                },
                 new SOM.Main.Entities.RatePlan
                {
                    RatePlanId = RP_ADSL_RES_NORMAL,
                    Name = "Normal Plan",
                    LOB = SOM.Main.Entities.LineOfBusiness.ADSL,
                    Category = GetCustomerCategory(CUSTOMER_CAT_RES_ENG),
                    CorePackage = GetServicePackage(PCKG_CORE_ADSL_1),
                    OptionalPackages = GetServicePackages(new List<string> { PCKG_OPT_ADSL_1 })
                },
                new SOM.Main.Entities.RatePlan
                {
                    RatePlanId = RP_ADSL_RES_STUDENT,
                    Name = "Student Plan",
                    LOB = SOM.Main.Entities.LineOfBusiness.ADSL,
                    Category = GetCustomerCategory(CUSTOMER_CAT_RES_ENG),
                    CorePackage = GetServicePackage(PCKG_CORE_ADSL_1),
                    OptionalPackages = GetServicePackages(new List<string> { PCKG_OPT_ADSL_1, PCKG_OPT_ADSL_2 })
                },
                new SOM.Main.Entities.RatePlan
                {
                    RatePlanId = RP_GSHDSL_RES_NORMAL,
                    Name = "Student Plan",
                    LOB = SOM.Main.Entities.LineOfBusiness.GSHDSL,
                    Category = GetCustomerCategory(CUSTOMER_CAT_RES_NORMAL),
                    CorePackage = GetServicePackage(PCKG_CORE_GSHDSL_1),
                    OptionalPackages = GetServicePackages(new List<string> { PCKG_OPT_GSHDSL_1 , PCKG_OPT_GSHDSL_3})
                },
                new SOM.Main.Entities.RatePlan
                {
                    RatePlanId = RP_GSHDSL_RES_STUDENT,
                    Name = "Student Plan",
                    LOB = SOM.Main.Entities.LineOfBusiness.GSHDSL,
                    Category = GetCustomerCategory(CUSTOMER_CAT_RES_ENG),
                    CorePackage = GetServicePackage(PCKG_CORE_GSHDSL_1),
                    OptionalPackages = GetServicePackages(new List<string> { PCKG_OPT_GSHDSL_1, PCKG_OPT_GSHDSL_2,PCKG_OPT_GSHDSL_3 })
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
                },
                new SOM.Main.Entities.ServicePackage
                {
                    PackageId = PCKG_CORE_ADSL_1,
                    PackageName = "ADSL Core Package",
                    Services = GetServices(PCKG_CORE_ADSL_1)
                },
                new SOM.Main.Entities.ServicePackage
                {
                    PackageId = PCKG_OPT_ADSL_1,
                    PackageName = "Premium",
                    Services = GetServices(PCKG_OPT_ADSL_1)
                },
                new SOM.Main.Entities.ServicePackage
                {
                    PackageId = PCKG_OPT_ADSL_2,
                    PackageName = "Gold",
                    Services = GetServices(PCKG_OPT_ADSL_2)
                },
                 new SOM.Main.Entities.ServicePackage
                {
                    PackageId = PCKG_CORE_GSHDSL_1,
                    PackageName = "GSHDSL Core Package",
                    Services = GetServices(PCKG_CORE_GSHDSL_1)
                },
                new SOM.Main.Entities.ServicePackage
                {
                    PackageId = PCKG_OPT_GSHDSL_1,
                    PackageName = "Premium",
                    Services = GetServices(PCKG_OPT_GSHDSL_1)
                },
                new SOM.Main.Entities.ServicePackage
                {
                    PackageId = PCKG_OPT_GSHDSL_2,
                    PackageName = "Gold",
                    Services = GetServices(PCKG_OPT_GSHDSL_2)
                },
                 new SOM.Main.Entities.ServicePackage
                {
                    PackageId = PCKG_OPT_GSHDSL_3,
                    PackageName = "Gold",
                    Services = GetServices(PCKG_OPT_GSHDSL_3)
                },

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
                },
                 new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_LL_3,
                    Name = "LM Fiber Selected",
                    Description = "LM Fiber",
                    AccessFee = 200,
                    PackageId = PCKG_OPT_LL_1
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_LL_4,
                    Name = "LM Microwave Selected",
                    Description = "LM Microwave",
                    AccessFee = 450,
                    PackageId = PCKG_OPT_LL_1
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_CORE_ADSL_1,
                    Name = "ADSL Subscription",
                    Description = "ADSL Subscription",
                    SubscriptionFee = 1500,
                    AccessFee = 1000,
                    IsCore = true,
                    PackageId = PCKG_CORE_ADSL_1
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_CORE_ADSL_2,
                    Name = "War Taxes",
                    Description = "War Taxes",
                    SubscriptionFee = 2500,
                    AccessFee = 150,
                    IsCore = true,
                    PackageId = PCKG_CORE_ADSL_1
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_ADSL_1,
                    Name = "Internet Speed",
                    Description = "Internet Speed",
                    AccessFee = 150,
                    SubscriptionFee = 300,
                    PackageId = PCKG_OPT_ADSL_1,
                    ServiceParams = new List<SOM.Main.Entities.ServiceParameter>
                    {
                        new SOM.Main.Entities.ServiceParameter
                        {
                            Id = "1",
                            Name = "2 MB/s"
                        },
                        new SOM.Main.Entities.ServiceParameter
                        {
                            Id = "2",
                            Name = "50 MB/s"
                        }
                    }
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_ADSL_1,
                    Name = "Quota",
                    Description = "Quota",
                    AccessFee = 150,
                    PackageId = PCKG_OPT_ADSL_1,
                    ServiceParams = new List<SOM.Main.Entities.ServiceParameter>
                    {
                        new SOM.Main.Entities.ServiceParameter
                        {
                            Id = "1",
                            Name = "20 GB"
                        },
                        new SOM.Main.Entities.ServiceParameter
                        {
                            Id = "2",
                            Name = "No Limit"
                        }
                    }
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_ADSL_2,
                    Name = "Paternal Control",
                    Description = "Paternal Control",
                    AccessFee = 200,
                    PackageId = PCKG_OPT_ADSL_2
                },
                 new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_TL_6,
                    Name = "ISP ADSL",
                    Description = "ISP ADSL",
                    AccessFee = 200,
                    PackageId = PCKG_OPT_TL_1
                },
                  new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_CORE_GSHDSL_1,
                    Name = "GSHDSL Subscription",
                    Description = "GSHDSL Subscription",
                    SubscriptionFee = 1500,
                    AccessFee = 1000,
                    IsCore = true,
                    PackageId = PCKG_CORE_GSHDSL_1
                },
                new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_CORE_GSHDSL_2,
                    Name = "War Taxes",
                    Description = "War Taxes",
                    SubscriptionFee = 2500,
                    AccessFee = 150,
                    IsCore = true,
                    PackageId = PCKG_CORE_GSHDSL_1
                },
                 new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_GSHDSL_1,
                    Name = "Paternal Control",
                    Description = "Paternal Control",
                    AccessFee = 200,
                    PackageId = PCKG_OPT_GSHDSL_1
                },
                  new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_GSHDSL_2,
                    Name = "Paternal Control",
                    Description = "Paternal Control",
                    AccessFee = 200,
                    PackageId = PCKG_OPT_GSHDSL_2
                },
               new SOM.Main.Entities.Service
                {
                    ServiceId = SRV_OPT_GSHDSL_3,
                    Name = "Speed",
                    Description = "Speed",
                    AccessFee = 150,
                    SubscriptionFee = 300,
                    PackageId = PCKG_OPT_GSHDSL_3,
                    ServiceParams = new List<SOM.Main.Entities.ServiceParameter>
                    {
                        new SOM.Main.Entities.ServiceParameter
                        {
                            Id = "1",
                            Name = "2 MB/s"
                        },
                        new SOM.Main.Entities.ServiceParameter
                        {
                            Id = "2",
                            Name = "50 MB/s"
                        }
                    }
                },
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
