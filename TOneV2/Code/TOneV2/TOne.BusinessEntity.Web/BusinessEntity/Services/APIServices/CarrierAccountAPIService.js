(function(appControllers) {

    "use strict";

    serviceObj.$inject = ['BaseAPIService', 'CarrierTypeEnum'];

    function serviceObj(BaseAPIService, CarrierTypeEnum) {
        
        function getCarriers(carrierType) {
            return BaseAPIService.get("/api/CarrierAccount/GetCarriers",
                {
                    carrierType: carrierType
                });
        }

        function getCustomers() {
            return BaseAPIService.get("/api/CarrierAccount/GetCarriers",
                {
                    carrierType: CarrierTypeEnum.Customer.value
                });
        }
    
        function insertCarrierTest(CarrierAccountID, Name) {
            return BaseAPIService.post("/api/CarrierAccount/insertCarrierTest",
                {
                    CarrierAccountID: CarrierAccountID,
                    Name: Name
                });
        }

        function getFilteredCarrierAccounts(input) {
            return BaseAPIService.post("/api/CarrierAccount/GetFilteredCarrierAccounts", input);
        }

        function getCarrierAccounts(ProfileName, ProfileCompanyName, from, to) {
            return BaseAPIService.get("/api/CarrierAccount/GetCarrierAccounts",
                {
                    ProfileName: ProfileName,
                    ProfileCompanyName: ProfileCompanyName,
                    from: from,
                    to: to
                });
        }

        function getCarrierAccount(carrierAccountId) {
            return BaseAPIService.get("/api/CarrierAccount/GetCarrierAccount",
                {
                    carrierAccountId: carrierAccountId
                });
        }

        function updateCarrierAccount(CarrierAccount) {
            return BaseAPIService.post("/api/CarrierAccount/UpdateCarrierAccount",CarrierAccount);
        }

        return ({
            GetCarriers: getCarriers,
            GetCustomers: getCustomers,
            insertCarrierTest: insertCarrierTest,
            GetCarrierAccounts: getCarrierAccounts,
            GetCarrierAccount: getCarrierAccount,
            UpdateCarrierAccount: updateCarrierAccount,
            GetFilteredCarrierAccounts: getFilteredCarrierAccounts
        });
    }

    
    appControllers.service('CarrierAccountAPIService', serviceObj);

})(appControllers);