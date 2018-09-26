(function (appControllers) {

    "use strict";

    supplierServiceEditorController.$inject = ['$scope', 'WhS_BE_SupplierZoneServiceAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_BE_SupplierEntityServiceSourceEnum', 'VRValidationService'];

    function supplierServiceEditorController($scope, WhS_BE_SupplierZoneServiceAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_BE_SupplierEntityServiceSourceEnum, VRValidationService) {

        var supplierId;
        var editMode;
        var effectiveOn;
        var supplierServiceEntity;
        var zoneServiceConfigSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var zoneServiceConfigSelectorAPI;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                supplierId = parameters.SupplierId;
                supplierServiceEntity = parameters.SupplierServiceObj;
                effectiveOn = parameters.EffectiveOn;
            }
            editMode = (supplierServiceEntity != undefined);

        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.isSaveButtonDisabled = (editMode === true);

            $scope.scopeModel.onServiceItemSelected = function () {
                $scope.scopeModel.isSaveButtonDisabled = false;
            };
            $scope.scopeModel.onServiceItemDeSelected = function () {
                $scope.scopeModel.isSaveButtonDisabled = false;
            };
            $scope.scopeModel.bedChangedValue = function () {
                $scope.scopeModel.isSaveButtonDisabled = false;
            };
            $scope.scopeModel.onZoneServiceConfigSelectorReady = function (api) {
                zoneServiceConfigSelectorAPI = api;
                zoneServiceConfigSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.saveSupplierService = function () {
                if (editMode)
                    return updateSupplierService();
                else
                    return insertSupplierService();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.scopeModel.DateValid = function () {
                var errorMessage = VRValidationService.validateTimeRange(supplierServiceEntity.ZoneBED, $scope.scopeModel.bed);
                if (errorMessage !=null) {
                    return 'BED must be greater than or equal to zone BED :' + supplierServiceEntity.ZoneBED;
                }
                return null;
            };

        }

        function load() {

            $scope.isLoading = true;

            if (editMode) {

                loadAllControls()
                        .finally(function () {
                        });
            }
            else {
                loadAllControls();
            }
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadZoneServiceConfigSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }


        function setTitle() {
            if (editMode && supplierServiceEntity != undefined)
                $scope.title = UtilsService.buildTitleForChangeEditor(supplierServiceEntity.ZoneName, "Services For Zone ");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Zone Service");
        }

        function loadStaticData() {

            if (supplierServiceEntity == undefined)
                return;

            $scope.scopeModel.bed = effectiveOn;
        }

        function loadZoneServiceConfigSelector() {
            var zoneServiceConfigSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            zoneServiceConfigSelectorReadyDeferred.promise.then(function () {
                zoneServiceConfigSelectorReadyDeferred = undefined;
                var zoneServiceConfigSelectorPayload = {
                    selectedIds: (supplierServiceEntity.Source == WhS_BE_SupplierEntityServiceSourceEnum.SupplierZone.value) ? supplierServiceEntity.Services : null
                };
                VRUIUtilsService.callDirectiveLoad(zoneServiceConfigSelectorAPI, zoneServiceConfigSelectorPayload, zoneServiceConfigSelectorLoadDeferred);
            });

            return zoneServiceConfigSelectorLoadDeferred.promise;
        }
        function getSelectedServices() {
            var selectedServices = zoneServiceConfigSelectorAPI.getSelectedIds();
            var Services = [];
            if (selectedServices != undefined) {
                for (var i = 0; i < selectedServices.length ; i++) {
                    Services.push({ ServiceId: selectedServices[i] });
                }
                return Services;
            }
        }

        function buildSupplierZoneService() {

            var obj = {
                SupplierZoneId: (supplierServiceEntity != undefined) ? supplierServiceEntity.SupplierZoneId : null,
                SupplierZoneServiceId: (supplierServiceEntity != undefined) ? supplierServiceEntity.SupplierZoneServiceId : null,
                ZoneName: (supplierServiceEntity != undefined) ? supplierServiceEntity.ZoneName : null,
                SupplierId: supplierId,
                BED: $scope.scopeModel.bed,
                Services: zoneServiceConfigSelectorAPI != undefined ? getSelectedServices() : null,
                ZoneBED:supplierServiceEntity.ZoneBED,
                ZoneEED: (supplierServiceEntity != undefined) ? supplierServiceEntity.ZoneEED : null
            };
            return obj;
        }

        function insertSupplierService() {


        }
        function updateSupplierService() {
            $scope.isLoading = true;

            var serviceObject = buildSupplierZoneService();
            WhS_BE_SupplierZoneServiceAPIService.UpdateSupplierZoneService(serviceObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("SupplierZoneService", response, "Name")) {
                    if ($scope.onSupplierServiceUpdated != undefined) {
                        response.UpdatedObject.Services = zoneServiceConfigSelectorAPI.getSelectedIds();
                        $scope.onSupplierServiceUpdated(response.UpdatedObject);
                    }

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {

                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('WhS_BE_SupplierServiceEditorController', supplierServiceEditorController);
})(appControllers);
