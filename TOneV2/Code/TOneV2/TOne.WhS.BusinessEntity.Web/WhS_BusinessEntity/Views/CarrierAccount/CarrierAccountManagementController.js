(function (appControllers) {

    "use strict";

    carrierAccountManagementController.$inject = ['$scope', 'WhS_BE_CarrierAccountAPIService', 'WhS_BE_MainService', 'UtilsService', 'VRModalService', 'VRNotificationService','WhS_Be_CarrierAccountEnum'];

    function carrierAccountManagementController($scope, WhS_BE_CarrierAccountAPIService, WhS_BE_MainService, UtilsService, VRModalService, VRNotificationService,WhS_Be_CarrierAccountEnum) {
        var gridAPI;
        var carrierAccountDirectiveAPI;
        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (carrierAccountDirectiveAPI != undefined && gridAPI != undefined)
                    gridAPI.loadGrid(getFilterObject());
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {
                }
                api.loadGrid(filter);
            }
            $scope.name;
            $scope.selectedCarriers;
            $scope.onCarrierAccountDirectiveLoaded = function (api) {
                carrierAccountDirectiveAPI = api;
                
            }
            $scope.onCarrierAccountSelectionChanged = function () {
                if (carrierAccountDirectiveAPI != undefined)
                    $scope.selectedCarriers = carrierAccountDirectiveAPI.getData();
            }
            $scope.onGridReady = function (api) {
                console.log(api);
                gridAPI = api;
                api.loadGrid({});
            }
            $scope.selectedCarrierAccountType;
            $scope.AddNewCarrierAccount = AddNewCarrierAccount;
        }

        function load() {
            defineCarrierAccountTypes();
        }

        function getFilterObject() {
            var selectedCarrier;
            var carrierAccountType;
            if ($scope.selectedCarriers != undefined)
                selectedCarrier= $scope.selectedCarriers.CarrierAccountId;
            if ($scope.selectedCarrierAccountType != undefined)
                carrierAccountType=  $scope.selectedCarrierAccountType.value;
            var data = {
                CarrierAccountId: selectedCarrier,
                AccountType: carrierAccountType,
                Name:$scope.name,

            };
            return data;
        }
        function defineCarrierAccountTypes() {
                $scope.carrierAccountTypes = [];
                for (var p in WhS_Be_CarrierAccountEnum)
                    $scope.carrierAccountTypes.push(WhS_Be_CarrierAccountEnum[p]);
            
        }
        function AddNewCarrierAccount() {
            var onCarrierAccountAdded = function (carrierAccountObj) {
                //if (gridAPI != undefined)
                //    gridAPI.onCustomerPricingProductAdded(customerPricingProductObj);
            };

            WhS_BE_MainService.addCarrierAccount(onCarrierAccountAdded);
        }
    }

    appControllers.controller('WhS_BE_CarrierAccountManagementController', carrierAccountManagementController);
})(appControllers);