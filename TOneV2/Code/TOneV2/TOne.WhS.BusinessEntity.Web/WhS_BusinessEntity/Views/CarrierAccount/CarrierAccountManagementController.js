(function (appControllers) {

    "use strict";

    carrierAccountManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'WhS_BE_CarrierAccountTypeEnum', 'VRUIUtilsService', 'WhS_BE_CarrierAccountService', 'WhS_BE_CarrierAccountAPIService'];

    function carrierAccountManagementController($scope, UtilsService, VRNotificationService, WhS_BE_CarrierAccountTypeEnum, VRUIUtilsService, WhS_BE_CarrierAccountService, WhS_BE_CarrierAccountAPIService) {
        var gridAPI;
        var carrierProfileDirectiveAPI;
        var carrierProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred;

        var serviceDirectiveAPI;

        var activationStatusSelectorAPI;
        var activationStatusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
            };

            $scope.onActivationStatusDirectiveReady = function (api) {
                activationStatusSelectorAPI = api;
                activationStatusSelectorReadyPromiseDeferred.resolve();
            };

            $scope.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
            };

            $scope.onZoneServiceConfigSelectorReady = function (api) {
                serviceDirectiveAPI = api;
            };

            $scope.onCarrierProfileDirectiveReady = function (api) {
                carrierProfileDirectiveAPI = api;
                carrierProfileReadyPromiseDeferred.resolve();
            };

            $scope.onCarrierTypeSelectionChanged = function () {
                if (UtilsService.contains($scope.selectedCarrierAccountTypes, WhS_BE_CarrierAccountTypeEnum.Customer) || UtilsService.contains($scope.selectedCarrierAccountTypes, WhS_BE_CarrierAccountTypeEnum.Exchange)) {
                    if (sellingNumberPlanDirectiveAPI != undefined) {
                        $scope.showSellingNumberPlan = true;

                        var setLoader = function (value) { $scope.isLoadingSellingNumberPlan = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sellingNumberPlanDirectiveAPI, undefined, setLoader);
                    }
                }
                else {
                    $scope.showSellingNumberPlan = false;
                    $scope.selectedSellingNumberPlans.length = 0;
                }

                if (UtilsService.contains($scope.selectedCarrierAccountTypes, WhS_BE_CarrierAccountTypeEnum.Supplier) || UtilsService.contains($scope.selectedCarrierAccountTypes, WhS_BE_CarrierAccountTypeEnum.Exchange)) {
                    if (serviceDirectiveAPI != undefined) {
                        $scope.showZoneService = true;

                        var setLoader = function (value) { $scope.isLoadingService = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, serviceDirectiveAPI, undefined, setLoader);
                    }
                }
                else {
                    $scope.showZoneService = false;
                    $scope.servicesValues.length = 0;
                }
            };

            $scope.selectedCarrierAccountTypes = [];
            $scope.selectedSellingNumberPlans = [];
            $scope.AddNewCarrierAccount = AddNewCarrierAccount;

            $scope.hadAddCarrierAccountPermission = function () {
                return WhS_BE_CarrierAccountAPIService.HasAddCarrierAccountPermission();
            };

            function getFilterObject() {
                var data = {
                    AccountsTypes: UtilsService.getPropValuesFromArray($scope.selectedCarrierAccountTypes, "value"),
                    CarrierProfilesIds: carrierProfileDirectiveAPI.getSelectedIds(),
                    Name: $scope.name,
                    SellingNumberPlanIds: sellingNumberPlanDirectiveAPI.getSelectedIds(),
                    ActivationStatusIds: activationStatusSelectorAPI.getSelectedIds(),
                    Services: serviceDirectiveAPI.getSelectedIds()
                };
                return data;
            }
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCarrierAccountType, loadCarrierProfiles, loadCarrierActivationStatusType])
                .catch(function (error) {
                    $scope.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.isLoading = false;
               });
        }

        function loadCarrierAccountType() {
            $scope.carrierAccountTypes = UtilsService.getArrayEnum(WhS_BE_CarrierAccountTypeEnum);
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

        function loadCarrierActivationStatusType() {
            var loadActivationStatusSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            activationStatusSelectorReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(activationStatusSelectorAPI, undefined, loadActivationStatusSelectorPromiseDeferred);
            });
            return loadActivationStatusSelectorPromiseDeferred.promise;
        }

        function AddNewCarrierAccount() {
            var onCarrierAccountAdded = function (carrierAccountObj) {
                gridAPI.onCarrierAccountAdded(carrierAccountObj);
            };

            WhS_BE_CarrierAccountService.addCarrierAccount(onCarrierAccountAdded);
        }
    }

    appControllers.controller('WhS_BE_CarrierAccountManagementController', carrierAccountManagementController);
})(appControllers);