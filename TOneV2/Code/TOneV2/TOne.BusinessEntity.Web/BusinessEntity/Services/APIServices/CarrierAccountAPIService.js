'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetCarriers: GetCarriers,
        //GetForAccountManager: GetForAccountManager,
        insertCarrierTest: insertCarrierTest,
        GetCarrierAccounts: GetCarrierAccounts,
        GetCarrierAccount: GetCarrierAccount,
        UpdateCarrierAccount: UpdateCarrierAccount,
        GetFilteredCarrierAccounts: GetFilteredCarrierAccounts
    });
    function GetCarriers(carrierType) {
        return BaseAPIService.get("/api/CarrierAccount/GetCarriers",
            {
                carrierType: carrierType
            });
    }
    //function GetForAccountManager(from, to) {
    //    return BaseAPIService.get("/api/Carrier/GetForAccountManager", {
    //        from: from,
    //        to: to
    //    });
    //}
    function insertCarrierTest(CarrierAccountID, Name) {
        return BaseAPIService.post("/api/CarrierAccount/insertCarrierTest",
            {
                CarrierAccountID: CarrierAccountID,
                Name: Name
            });
    }

    function GetFilteredCarrierAccounts(input) {
        return BaseAPIService.post("/api/CarrierAccount/GetFilteredCarrierAccounts", input);
    }

    function GetCarrierAccounts(ProfileName, ProfileCompanyName, from, to) {
        return BaseAPIService.get("/api/CarrierAccount/GetCarrierAccounts",
            {
                ProfileName: ProfileName,
                ProfileCompanyName: ProfileCompanyName,
                from: from,
                to: to
            });
    }

    function GetCarrierAccount(carrierAccountId) {
        return BaseAPIService.get("/api/CarrierAccount/GetCarrierAccount",
            {
                carrierAccountId: carrierAccountId
            });
    }
    function UpdateCarrierAccount(CarrierAccount) {
        return BaseAPIService.post("/api/CarrierAccount/UpdateCarrierAccount",

                CarrierAccount
            );
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CarrierAccountAPIService', serviceObj);