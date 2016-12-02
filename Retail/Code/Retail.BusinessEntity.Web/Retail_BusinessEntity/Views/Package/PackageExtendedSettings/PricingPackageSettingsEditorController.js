(function (appControllers) {

    "use strict";

    PricingPackageSettingsEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function PricingPackageSettingsEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;

        var serviceTypeId;
        var serviceTypeName;
        var usageChargingPolicyId;
        var usageChargingPolicyName;
        var excludedServiceTypeIds; // passed for ServiceTypeSelector Filter

        var serviceTypeAPI;
        var serviceTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var serviceTypeSelectionChangedPromiseDeferred;

        var chargingPolicyAPI;
        var chargingPolicyReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                excludedServiceTypeIds = parameters.excludedServiceTypeIds;

                if (parameters.pricingPackageSetting != undefined) {
                    serviceTypeId = parameters.pricingPackageSetting.ServiceTypeId;
                    serviceTypeName = parameters.pricingPackageSetting.ServiceTypeName
                    usageChargingPolicyId = parameters.pricingPackageSetting.UsageChargingPolicyId;
                    usageChargingPolicyName = parameters.pricingPackageSetting.UsageChargingPolicyName;
                }
            }
            isEditMode = (serviceTypeId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.isServiceTypeSelectorDisabled = isEditMode ? true : false;

            $scope.scopeModel.onServiceTypeSelectorReady = function (api) {
                serviceTypeAPI = api;
                serviceTypeReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onChargingPolicySelectorReady = function (api) {
                chargingPolicyAPI = api;
                chargingPolicyReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onServiceTypeSelectionChanged = function () {
                var serviceTypeId = serviceTypeAPI.getSelectedIds();
                if (serviceTypeId != undefined) {
                    var setLoader = function (value) {
                        $scope.scopeModel.isChargingPolicySelectorLoading = value;
                    };
                    var payloadChargingPolicyDirective = {
                        filter: { ServiceTypeId: serviceTypeId },
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, chargingPolicyAPI, payloadChargingPolicyDirective, setLoader, serviceTypeSelectionChangedPromiseDeferred);
                }
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return updatePackage();
                }
                else {
                    return insertPackage();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadServiceTypeSelector, loadChargingPolicySelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }
        function setTitle() {

            $scope.title =
                isEditMode ? UtilsService.buildTitleForUpdateEditor(usageChargingPolicyName, 'Pricing Package') : UtilsService.buildTitleForAddEditor('Pricing Package');
        }
        function loadServiceTypeSelector() {

            if (serviceTypeId != undefined)
                serviceTypeSelectionChangedPromiseDeferred = UtilsService.createPromiseDeferred();

            var loadServiceTypeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            serviceTypeReadyPromiseDeferred.promise.then(function () {
                var serviceTypeDirectivePayload = {
                    selectedIds: serviceTypeId,
                    excludedServiceTypeIds: excludedServiceTypeIds
                };

                VRUIUtilsService.callDirectiveLoad(serviceTypeAPI, serviceTypeDirectivePayload, loadServiceTypeDirectivePromiseDeferred);
            });

            return loadServiceTypeDirectivePromiseDeferred.promise;
        }
        function loadChargingPolicySelector() {
            if (!isEditMode)
                return;

            var loadChargingPolicyDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([chargingPolicyReadyPromiseDeferred.promise, serviceTypeSelectionChangedPromiseDeferred.promise]).then(function () {
                serviceTypeSelectionChangedPromiseDeferred = undefined;

                var chargingPolicyDirectivePayload = {
                    selectedIds: usageChargingPolicyId,
                    filter: { ServiceTypeId: serviceTypeId }
                };

                VRUIUtilsService.callDirectiveLoad(chargingPolicyAPI, chargingPolicyDirectivePayload, loadChargingPolicyDirectivePromiseDeferred);

            });

            return loadChargingPolicyDirectivePromiseDeferred.promise;
        }

        function insertPackage() {
            var pricingPackageSettingsObj = buildPricingPackageSettingObjFromScope();

            if ($scope.onPricingPackageSettingsAdded != undefined && typeof ($scope.onPricingPackageSettingsAdded) == 'function') {
                $scope.onPricingPackageSettingsAdded(pricingPackageSettingsObj);
            }
            $scope.modalContext.closeModal();
        }
        function updatePackage() {
            var pricingPackageSettingsObj = buildPricingPackageSettingObjFromScope();

            if ($scope.onPricingPackageSettingsUpdated != undefined && typeof ($scope.onPricingPackageSettingsUpdated) == 'function') {
                $scope.onPricingPackageSettingsUpdated(pricingPackageSettingsObj);
            }
            $scope.modalContext.closeModal();
        }

        function buildPricingPackageSettingObjFromScope() {

            var obj = {
                ServiceTypeId: $scope.scopeModel.selectedServiceType.ServiceTypeId,
                ServiceTypeName: $scope.scopeModel.selectedServiceType.Title,
                UsageChargingPolicyId: $scope.scopeModel.selectedChargingPolicy.ChargingPolicyId,
                UsageChargingPolicyName: $scope.scopeModel.selectedChargingPolicy.Name
            };

            return obj;
        }
    }

    appControllers.controller('Retail_BE_PricingPackageSettingsEditorController', PricingPackageSettingsEditorController);
})(appControllers);
