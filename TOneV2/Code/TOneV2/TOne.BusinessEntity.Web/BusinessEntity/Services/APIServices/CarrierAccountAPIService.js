(function(appControllers) {

    "use strict";

    serviceObj.$inject = ['BaseAPIService', 'CarrierTypeEnum'];

    function serviceObj(baseApiService, carrierTypeEnum) {
        
        function getCarriers(carrierType) {
            return baseApiService.get("/api/CarrierAccount/GetCarriers",
                {
                    carrierType: carrierType
                });
        }

        function getCustomers() {
            return baseApiService.get("/api/CarrierAccount/GetCarriers",
                {
                    carrierType: carrierTypeEnum.Customer.value
                });
        }

        function getSuppliers() {
            return baseApiService.get("/api/CarrierAccount/GetCarriers",
                {
                    carrierType: carrierTypeEnum.Supplier.value
                });
        }
    
        function insertCarrierTest(carrierAccountId, name) {
            return baseApiService.post("/api/CarrierAccount/insertCarrierTest",
                {
                    CarrierAccountID: carrierAccountId,
                    Name: name
                });
        }

        function getFilteredCarrierAccounts(input) {
            return baseApiService.post("/api/CarrierAccount/GetFilteredCarrierAccounts", input);
        }

        function getCarrierAccounts(profileName, profileCompanyName, from, to) {
            return baseApiService.get("/api/CarrierAccount/GetCarrierAccounts",
                {
                    ProfileName: profileName,
                    ProfileCompanyName: profileCompanyName,
                    from: from,
                    to: to
                });
        }

        function getCarrierAccount(carrierAccountId) {
            return baseApiService.get("/api/CarrierAccount/GetCarrierAccount",
                {
                    carrierAccountId: carrierAccountId
                });
        }

        function updateCarrierAccount(carrierAccount) {
            return baseApiService.post("/api/CarrierAccount/UpdateCarrierAccount",carrierAccount);
        }

        return ({
            GetCarriers: getCarriers,
            GetCustomers: getCustomers,
            insertCarrierTest: insertCarrierTest,
            GetCarrierAccounts: getCarrierAccounts,
            GetCarrierAccount: getCarrierAccount,
            UpdateCarrierAccount: updateCarrierAccount,
            GetFilteredCarrierAccounts: getFilteredCarrierAccounts,
            GetSuppliers: getSuppliers
        });
    }

    
    appControllers.service('CarrierAccountAPIService', serviceObj);

})(appControllers);