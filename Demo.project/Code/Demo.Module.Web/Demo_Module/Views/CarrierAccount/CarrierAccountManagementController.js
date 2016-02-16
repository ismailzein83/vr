(function (appControllers) {

    "use strict";

    carrierAccountManagementController.$inject = ['$scope',  'UtilsService', 'VRNotificationService', 'Demo_CarrierAccountTypeEnum', 'VRUIUtilsService', 'Demo_CarrierAccountService'];

    function carrierAccountManagementController($scope, UtilsService, VRNotificationService, Demo_CarrierAccountTypeEnum, VRUIUtilsService, Demo_CarrierAccountService) {
        var gridAPI;
        var carrierProfileDirectiveAPI;
        var carrierProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred;

        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };
            
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {};
                api.loadGrid(filter);
            }

            $scope.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
            }

            $scope.onCarrierProfileDirectiveReady = function (api) {
                carrierProfileDirectiveAPI = api;
                carrierProfileReadyPromiseDeferred.resolve();
            }

            $scope.onCarrierTypeSelectionChanged = function () {
                if (UtilsService.contains($scope.selectedCarrierAccountTypes, Demo_CarrierAccountTypeEnum.Customer) || UtilsService.contains($scope.selectedCarrierAccountTypes, Demo_CarrierAccountTypeEnum.Exchange)) {
                       if (sellingNumberPlanDirectiveAPI != undefined) {
                            $scope.showSellingNumberPlan = true;
                            var setLoader = function (value) { $scope.isLoadingSellingNumberPlan = value };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sellingNumberPlanDirectiveAPI, undefined, setLoader);
                        }
                    }
                else
                {
                    $scope.showSellingNumberPlan = false;
                    $scope.selectedSellingNumberPlans.length = 0;
                }
                        
            }

            $scope.selectedCarrierAccountTypes=[];
            $scope.selectedSellingNumberPlans = [];
            $scope.AddNewCarrierAccount = AddNewCarrierAccount;

            function getFilterObject() {
                var data = {
                    AccountsTypes: UtilsService.getPropValuesFromArray($scope.selectedCarrierAccountTypes, "value"),
                    CarrierProfilesIds: carrierProfileDirectiveAPI.getSelectedIds(),
                    Name: $scope.name,
                    SellingNumberPlanIds: sellingNumberPlanDirectiveAPI.getSelectedIds()

                };
                return data;
            }
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCarrierAccountType, loadCarrierProfiles])
                .catch(function (error) {
                    $scope.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.isLoading = false;
               });
        }

        function loadCarrierAccountType() {
            $scope.carrierAccountTypes = UtilsService.getArrayEnum(Demo_CarrierAccountTypeEnum);
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

        function AddNewCarrierAccount() {
            var onCarrierAccountAdded = function (carrierAccountObj) {
                gridAPI.onCarrierAccountAdded(carrierAccountObj);
            };

            Demo_CarrierAccountService.addCarrierAccount(onCarrierAccountAdded);
        }
    }

    appControllers.controller('Demo_CarrierAccountManagementController', carrierAccountManagementController);
})(appControllers);