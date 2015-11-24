(function (appControllers) {

    "use strict";

    carrierAccountManagementController.$inject = ['$scope',  'UtilsService', 'VRNotificationService', 'WhS_Be_CarrierAccountTypeEnum', 'VRUIUtilsService', 'WhS_BE_CarrierAccountService'];

    function carrierAccountManagementController($scope, UtilsService, VRNotificationService, WhS_Be_CarrierAccountTypeEnum, VRUIUtilsService, WhS_BE_CarrierAccountService) {
        var gridAPI;
        var carrierProfileDirectiveAPI;
        var carrierProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
                carrierProfileReadyPromiseDeferred.resolve();
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

            $scope.isLoading = true;
            defineCarrierAccountTypes();
            loadAllControls();


        }
        function loadAllControls() {
            return loadCarrierProfiles()
                .catch(function (error) {
                    $scope.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);

                })
               .finally(function () {
                   $scope.isLoading = false;
               });
        }
        function loadCarrierProfiles() {
            var loadCarrierProfilePromiseDeferred = UtilsService.createPromiseDeferred();

            carrierProfileReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = undefined;

                    VRUIUtilsService.callDirectiveLoad(carrierProfileDirectiveAPI, directivePayload, loadCarrierProfilePromiseDeferred);
                });

            return loadCarrierProfilePromiseDeferred.promise;
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

            WhS_BE_CarrierAccountService.addCarrierAccount(onCarrierAccountAdded);
        }
    }

    appControllers.controller('WhS_BE_CarrierAccountManagementController', carrierAccountManagementController);
})(appControllers);