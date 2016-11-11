(function (appControllers) {

    'use strict';

    VRObjectVariableEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function VRObjectVariableEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var zoneServiceEntity;
        var selectedSellingNumberPlanId;
        var availableZoneIds;
        var  excludedZoneIds;

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var zoneServiceAPI;
        var zoneServiceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                zoneServiceEntity = parameters.zoneService;
                selectedSellingNumberPlanId = parameters.selectedSellingNumberPlanId;
                availableZoneIds = parameters.availableZoneIds;
                excludedZoneIds = parameters.excludedZoneIds;
            }

            isEditMode = (zoneServiceEntity != undefined);
        }
        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.selectedSaleZones = [];

            $scope.scopeModel.onSaleZoneSelectorReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onZoneServiceSelectorReady = function (api) {
                zoneServiceAPI = api;
                zoneServiceSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.save = function () {

                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            loadAllControls();
        }

        function loadAllControls() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadSaleZoneSelector, loadServiceZoneConfigSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });


        }

        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((zoneServiceEntity != undefined) ? zoneServiceEntity.ZoneNames : null, 'Zone Service') :
                UtilsService.buildTitleForAddEditor('Zone Service');
        }
        function loadSaleZoneSelector() {
            var saleZoneSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            saleZoneReadyPromiseDeferred.promise.then(function () {

                var payload = {};
                payload.sellingNumberPlanId = selectedSellingNumberPlanId;
                payload.availableZoneIds = availableZoneIds;
                payload.excludedZoneIds = excludedZoneIds;

                if (zoneServiceEntity)
                    payload.selectedIds = zoneServiceEntity.ZoneIds;

                VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, payload, saleZoneSelectorLoadDeferred);
            });

            return saleZoneSelectorLoadDeferred.promise;
        }
        function loadServiceZoneConfigSelector() {
            var serviceZoneConfigLoadDeferred = UtilsService.createPromiseDeferred();

            zoneServiceSelectorReadyPromiseDeferred.promise.then(function () {

                var payload;
                if (zoneServiceEntity != undefined) {
                    payload = { selectedIds: zoneServiceEntity.ServiceIds };
                }

                VRUIUtilsService.callDirectiveLoad(zoneServiceAPI, payload, serviceZoneConfigLoadDeferred);
            });

            return serviceZoneConfigLoadDeferred.promise;
        }

        function insert() {
            var routingProductZoneService = buildRoutingProductZoneServiceFromScope();
            if ($scope.onRoutingProductZoneServiceAdded != undefined && typeof ($scope.onRoutingProductZoneServiceAdded) == 'function') {
                $scope.onRoutingProductZoneServiceAdded(routingProductZoneService);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var routingProductZoneService = buildRoutingProductZoneServiceFromScope();
            if ($scope.onRoutingProductZoneServiceUpdated != undefined && typeof ($scope.onRoutingProductZoneServiceUpdated) == 'function') {
                $scope.onRoutingProductZoneServiceUpdated(routingProductZoneService);
            }
            $scope.modalContext.closeModal();
        }

        function buildRoutingProductZoneServiceFromScope() {

            var saleZones = buildRoutingProductSaleZones();
            var services = buildRoutingProductServices();

            var obj = {
                ZoneIds: saleZones != undefined ? saleZones.ZoneIds : undefined,
                ZoneNames: saleZones != undefined ? saleZones.ZoneNames.join(", ") : undefined,
                ServiceIds: services != undefined ? services.ServiceIds : undefined,
                ServiceNames: services != undefined ? services.ServiceNames.join(", ") : undefined,
            };
            return obj;
        }
        function buildRoutingProductSaleZones() {
            var zoneIds = [];
            var zoneNames = [];

            if ($scope.scopeModel.selectedSaleZones == undefined || $scope.scopeModel.selectedSaleZones.length == 0)
                return;

            var _selectedSaleZones = $scope.scopeModel.selectedSaleZones;

            for (var i = 0; i < _selectedSaleZones.length; i++) {
                zoneIds.push(_selectedSaleZones[i].SaleZoneId);
                zoneNames.push(_selectedSaleZones[i].Name);
            }

            return {
                ZoneIds: zoneIds,
                ZoneNames: zoneNames
            };
        }
        function buildRoutingProductServices() {
            var serviceIds = [];
            var serviceNames = [];

            if ($scope.scopeModel.selectedServices == undefined || $scope.scopeModel.selectedServices.length == 0)
                return;

            var _selectedServices = $scope.scopeModel.selectedServices;
            _selectedServices.sort(function (a, b) { return (a.ZoneServiceConfigId > b.ZoneServiceConfigId) ? 1 : ((b.ZoneServiceConfigId > a.ZoneServiceConfigId) ? -1 : 0); });

            for (var i = 0; i < _selectedServices.length; i++) {
                serviceIds.push(_selectedServices[i].ZoneServiceConfigId);
                serviceNames.push(_selectedServices[i].Symbol);
            }

            return {
                ServiceIds: serviceIds,
                ServiceNames: serviceNames
            };
        }
    }

    appControllers.controller('WhS_BE_RoutingProductZoneEditorController', VRObjectVariableEditorController);

})(appControllers);
