(function(appControllers) {

    "use strict";

    serviceObj.$inject = ['BaseAPIService', 'CarrierTypeEnum', 'UtilsService', 'BusinessEntity_ModuleConfig'];

    function serviceObj(baseApiService, carrierTypeEnum, UtilsService, BusinessEntity_ModuleConfig) {
        
        function getCarriers(carrierType, isAssignedCarrier) {
            if (isAssignedCarrier == undefined) {
                alert("Your Request function (getCarriers) should have new parameter (assignedCarrier = true or assignedCarrier=false);");
            }
               
            return baseApiService.get("/api/CarrierAccount/GetCarriers",
                {
                    carrierType: carrierType,
                    isAssignedCarrier: isAssignedCarrier
                });
        }

        function getCustomers(isAssignedCarrier) {
            if (isAssignedCarrier == undefined) {
                alert("Your Request function (getCarriers) should have new parameter (assignedCarrier = true or assignedCarrier=false);");
            }
            return baseApiService.get("/api/CarrierAccount/GetCarriers",
                {
                    carrierType: carrierTypeEnum.Customer.value,
                    isAssignedCarrier: isAssignedCarrier
                });
        }

        function getSuppliers(isAssignedCarrier) {
            if (isAssignedCarrier == undefined) {
                alert("Your Request function (getCarriers) should have new parameter (assignedCarrier = true or assignedCarrier=false);");
            }
            return baseApiService.get("/api/CarrierAccount/GetCarriers",
                {
                    carrierType: carrierTypeEnum.Supplier.value,
                    isAssignedCarrier: isAssignedCarrier
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
            return baseApiService.post(UtilsService.getRoute(BusinessEntity_ModuleConfig.moduleName, "CarrierAccount", "GetFilteredCarrierAccounts"), input);
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
            insertCarrierTest: insertCarrierTest,
            GetCarrierAccounts: getCarrierAccounts,
            GetCarrierAccount: getCarrierAccount,
            UpdateCarrierAccount: updateCarrierAccount,
            GetFilteredCarrierAccounts: getFilteredCarrierAccounts,
            getSuppliers: getSuppliers,
            getCustomers: getCustomers
        });
    }

    
    appControllers.service('CarrierAccountAPIService', serviceObj);

})(appControllers);