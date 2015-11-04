(function (appControllers) {

    "use strict";

    carrierAccountManagementController.$inject = ['$scope', 'WhS_BE_MainService', 'UtilsService', 'VRNotificationService', 'WhS_Be_CarrierAccountTypeEnum'];

    function carrierAccountManagementController($scope, WhS_BE_MainService, UtilsService, VRNotificationService, WhS_Be_CarrierAccountTypeEnum) {
        var gridAPI;
        var carrierAccountDirectiveAPI;
        var carrierProfileDirectiveAPI;
        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (!$scope.isGettingData  && gridAPI != undefined)
                   return gridAPI.loadGrid(getFilterObject());
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {};
                api.loadGrid(filter);
            }

            $scope.onCarrierProfileDirectiveReady = function (api) {
                carrierProfileDirectiveAPI = api;
                load();
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            }
            $scope.name;

            $scope.selectedCarrierAccountTypes=[];

            $scope.AddNewCarrierAccount = AddNewCarrierAccount;
        }

        function load() {

            $scope.isGettingData = true;

            if ( carrierProfileDirectiveAPI==undefined)
                return;
            defineCarrierAccountTypes();

           loadCarrierProfiles().finally(function () {
               $scope.isGettingData = false;
           }).catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
               $scope.isGettingData = false;
           });

        }
        function loadCarrierProfiles() {
            return carrierProfileDirectiveAPI.load();
        }

        function getFilterObject() {
            var data = {
                AccountsTypes: UtilsService.getPropValuesFromArray($scope.selectedCarrierAccountTypes, "value"),
                CarrierProfilesIds:carrierProfileDirectiveAPI.getSelectedIds(),
                Name:$scope.name,

            };
            return data;
        }
        function defineCarrierAccountTypes() {
                $scope.carrierAccountTypes = [];
                for (var p in WhS_Be_CarrierAccountTypeEnum)
                    $scope.carrierAccountTypes.push(WhS_Be_CarrierAccountTypeEnum[p]);
            
        }
        function AddNewCarrierAccount() {
            var onCarrierAccountAdded = function (carrierAccountObj) {
                if (gridAPI != undefined)
                    gridAPI.onCarrierAccountAdded(carrierAccountObj);
            };

            WhS_BE_MainService.addCarrierAccount(onCarrierAccountAdded);
        }
    }

    appControllers.controller('WhS_BE_CarrierAccountManagementController', carrierAccountManagementController);
})(appControllers);