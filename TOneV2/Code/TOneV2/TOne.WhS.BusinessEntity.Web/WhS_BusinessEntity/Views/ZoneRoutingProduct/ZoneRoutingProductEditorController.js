(function (appControllers) {

    "use strict";

    zoneRoutingProductEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_BE_ZoneRoutingProductAPIService', 'VRValidationService'];

    function zoneRoutingProductEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_BE_ZoneRoutingProductAPIService, VRValidationService) {

        var ownerId;
        var ownerType;
        var zoneId;
        var zoneName;
        var zoneBED;
        var zoneEED;
        var sellingNumberPlanId;
        var currentRoutingProductId;
        var countryBED;
        var countryEED;
        var zoneRoutingProductSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var zoneRoutingProductSelectorAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                ownerId = parameters.OwnerId;
                ownerType = parameters.OwnerType;
                zoneId = parameters.ZoneId;
                zoneBED = parameters.ZoneBED;
                zoneEED = parameters.ZoneEED;
                zoneName = parameters.ZoneName;
                sellingNumberPlanId = parameters.SellingNumberPlanId;
                currentRoutingProductId = parameters.CurrentRoutingProductId;
                countryBED = parameters.CountryBED;
                countryEED = parameters.CountryEED;
            }
        }

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.onZoneRoutinProductSelectorReady = function (api) {
                zoneRoutingProductSelectorAPI = api;
                zoneRoutingProductSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.saveZoneRoutingProduct = function () {
                return updateZoneRoutingProduct();
            };
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.validateRange = function () {
                var errorMessageZoneBED = VRValidationService.validateTimeRange(zoneBED, $scope.scopeModel.bed);
                var errorMessageCountryBED = VRValidationService.validateTimeRange(countryBED, $scope.scopeModel.bed);
                if (errorMessageZoneBED != null)
                    return 'Zone routing product BED cannot be less than zone BED';
                if (errorMessageCountryBED != null)
                    return 'Zone routing product BED cannot be less than country BED';
                return null;
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadRoutingProductSelector])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }

        function loadRoutingProductSelector() {
            var loadRoutingProductPromiseDeferred = UtilsService.createPromiseDeferred();
            var selectorPayload = {
                selectedIds: currentRoutingProductId
            };
            selectorPayload.filter = {
                AssignableToZoneId: zoneId
            };
            zoneRoutingProductSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(zoneRoutingProductSelectorAPI, selectorPayload, loadRoutingProductPromiseDeferred);
            });
            return loadRoutingProductPromiseDeferred.promise;
        }

        function setTitle() {
            $scope.title = UtilsService.buildTitleForChangeEditor(zoneName, "Routing product For Zone ");
        }

        function updateZoneRoutingProduct() {
            $scope.isLoading = true;

            var routingProductObject = buildZoneRoutingProductObject();
            WhS_BE_ZoneRoutingProductAPIService.UpdateZoneRoutingProduct(routingProductObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Routing product for zone", response, "zoneName")) {
                    if ($scope.onZoneRoutingProductUpdated != undefined) {
                        $scope.onZoneRoutingProductUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {

                $scope.isLoading = false;
            });
        }
        function buildZoneRoutingProductObject() {

            var obj = {
                ChangedRoutingProductId: zoneRoutingProductSelectorAPI.getSelectedIds(),
                CurrentZoneRoutingProductId: currentRoutingProductId,
                OwnerId: ownerId,
                OwnerType: ownerType,
                BED: $scope.scopeModel.bed,
                ZoneId: zoneId,
                ZoneBED: zoneBED,
                ZoneEED: zoneEED,
                CountryBED: countryBED,
                CountryEED: countryEED
            };
            return obj;
        }
    }

    appControllers.controller('WhS_BE_ZoneRoutingProductEditorController', zoneRoutingProductEditorController);
})(appControllers);
