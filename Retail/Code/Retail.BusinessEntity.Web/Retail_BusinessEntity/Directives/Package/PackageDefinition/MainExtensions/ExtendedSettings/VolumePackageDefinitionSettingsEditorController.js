(function (appControllers) {

    "use strict";

    VolumePackageDefinitionSettingsEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function VolumePackageDefinitionSettingsEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var volumePackageDefinitionItem;
        var serviceTypeIds;
        var context;

        var serviceTypeSelectorAPI;
        var serviceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                volumePackageDefinitionItem = parameters.VolumePackageDEfinitionItem;
                serviceTypeIds = volumePackageDefinitionItem != undefined ? volumePackageDefinitionItem.ServiceTypeIds : undefined;
                context = parameters.context;
            }

            isEditMode = volumePackageDefinitionItem != undefined;
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onServiceTypeSelectorReady = function (api) {
                serviceTypeSelectorAPI = api;
                serviceTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return isEditMode ? updateVolumePackageDefinitionItem() : addVolumePackageDefinitionItem();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && volumePackageDefinitionItem != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(volumePackageDefinitionItem.Name, 'Volume Package Definition Item');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Volume Package Definition Item');
            }

            function loadStaticData() {
                if (volumePackageDefinitionItem != undefined)
                    $scope.scopeModel.name = volumePackageDefinitionItem.Name;
            }

            function loadServiceTypeSelector() {
                var loadServiceTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                serviceTypeSelectorReadyDeferred.promise.then(function () {
                    var filters;
                    if (context != undefined && context.getServiceTypeFilter != undefined) {
                        filters = [];
                        filters.push(context.getServiceTypeFilter());
                    }
                    var serviceTypeSelectorPayload = {
                        selectedIds: serviceTypeIds,
                        //excludedServiceTypeIds: excludedServiceTypeIds,
                        filter: {
                            Filters: filters
                        }
                    };

                    VRUIUtilsService.callDirectiveLoad(serviceTypeSelectorAPI, serviceTypeSelectorPayload, loadServiceTypeSelectorPromiseDeferred);
                });
                return loadServiceTypeSelectorPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadServiceTypeSelector]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function addVolumePackageDefinitionItem() {
            var volumePackageDefinitionItem = buildVolumePackageDefinitionItemFromScope();
            if ($scope.onVolumePackageDefinitionItemAdded != undefined) {
                $scope.onVolumePackageDefinitionItemAdded(volumePackageDefinitionItem);
            }
            $scope.modalContext.closeModal();
        }

        function updateVolumePackageDefinitionItem() {
            var volumePackageDefinitionItem = buildVolumePackageDefinitionItemFromScope();
            if ($scope.onVolumePackageDefinitionItemUpdated != undefined) {
                $scope.onVolumePackageDefinitionItemUpdated(volumePackageDefinitionItem);
            }
            $scope.modalContext.closeModal();
        }

        function buildVolumePackageDefinitionItemFromScope() {
            return {
                VolumePackageDefinitionItemId: volumePackageDefinitionItem != undefined ? volumePackageDefinitionItem.VolumePackageDefinitionItemId : UtilsService.guid(),
                ServiceTypeIds: serviceTypeSelectorAPI.getSelectedIds(),
                Name: $scope.scopeModel.name
            };
        }

    }

    appControllers.controller('Retail_BE_VolumePackageDefinitionSettingsEditorController', VolumePackageDefinitionSettingsEditorController);
})(appControllers);