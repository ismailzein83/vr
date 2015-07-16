﻿'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetCarriers: GetCarriers,
        //GetForAccountManager: GetForAccountManager,
        insertCarrierTest: insertCarrierTest,
        GetCarrierAccounts: GetCarrierAccounts,
        GetCarrierAccount: GetCarrierAccount,
        UpdateCarrierAccount: UpdateCarrierAccount
    });
    function GetCarriers(carrierType) {
        return BaseAPIService.get("/api/Carrier/GetCarriers",
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
        return BaseAPIService.post("/api/Carrier/insertCarrierTest",
            {
                CarrierAccountID: CarrierAccountID,
                Name: Name
            });
    }
    function GetCarrierAccounts(ProfileName, ProfileCompanyName, from, to) {
        return BaseAPIService.get("/api/Carrier/GetCarrierAccounts",
            {
                ProfileName: ProfileName,
                ProfileCompanyName: ProfileCompanyName,
                from: from,
                to: to
            });
    }
    function GetCarrierAccount(carrierAccountId) {
        return BaseAPIService.get("/api/Carrier/GetCarrierAccount",
            {
                carrierAccountId: carrierAccountId
            });
    }
    function UpdateCarrierAccount(CarrierAccount) {
        return BaseAPIService.post("/api/Carrier/UpdateCarrierAccount",

                CarrierAccount
            );
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CarrierAPIService', serviceObj);