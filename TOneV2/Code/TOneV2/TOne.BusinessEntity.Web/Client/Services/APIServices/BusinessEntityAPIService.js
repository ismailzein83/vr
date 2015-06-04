'use strict'
var serviceObj = function (BaseAPIService) {

    return ({
        GetCarriers: GetCarriers,
        GetCodeGroups: GetCodeGroups,
        GetSwitches: GetSwitches,
        insertCarrierTest: insertCarrierTest,
        GetCarrierAccounts: GetCarrierAccounts,
        GetCarrierAccount: GetCarrierAccount,
        UpdateCarrierAccount: UpdateCarrierAccount
    });

    function GetCarriers(carrierType) {
        return BaseAPIService.get("/api/BusinessEntity/GetCarriers",
            {
                carrierType: carrierType
            });
    }
    function insertCarrierTest(CarrierAccountID, Name) {
        return BaseAPIService.post("/api/BusinessEntity/insertCarrierTest",
            {
                CarrierAccountID: CarrierAccountID,
                Name: Name
            });
    }
    function GetCodeGroups() {
        return BaseAPIService.get("/api/BusinessEntity/GetCodeGroups",
            {
            });
    }

    function GetSwitches() {
        return BaseAPIService.get("/api/BusinessEntity/GetSwitches",
            {
            });
    }
    function GetCarrierAccounts(ProfileName, ProfileCompanyName, from, to) {
        return BaseAPIService.get("/api/BusinessEntity/GetCarrierAccounts",
            {
                ProfileName: ProfileName,
                ProfileCompanyName: ProfileCompanyName,
                from: from,
                to: to
            });
    }
    function GetCarrierAccount(carrierAccountId) {
        return BaseAPIService.get("/api/BusinessEntity/GetCarrierAccount",
            {
                carrierAccountId: carrierAccountId
            });
    }
    function UpdateCarrierAccount(CarrierAccount) {
        return BaseAPIService.post("/api/BusinessEntity/UpdateCarrierAccount",

                CarrierAccount
            );
    }
}

serviceObj.$inject = ['BaseAPIService'];
appControllers.service('BusinessEntityAPIService', serviceObj);
