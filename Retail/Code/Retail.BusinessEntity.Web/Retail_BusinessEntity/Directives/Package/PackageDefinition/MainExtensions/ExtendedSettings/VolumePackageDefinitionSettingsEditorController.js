(function (appControllers) {

    "use strict";

    VolumePackageDefinitionSettingsEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function VolumePackageDefinitionSettingsEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var volumePackageDefinitionItem;
        var volumePackageDefinitionItemsNames;

        var serviceTypeSelectorAPI;
        var serviceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var compositeRecordConditionDefinitionGroupDirectiveAPI;
        var compositeRecordConditionDefinitionGroupDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                volumePackageDefinitionItem = parameters.VolumePackageDefinitionItem;
                volumePackageDefinitionItemsNames = parameters.volumePackageDefinitionItemsNames;
            }

            isEditMode = volumePackageDefinitionItem != undefined;
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onServiceTypeSelectorReady = function (api) {
                serviceTypeSelectorAPI = api;
                serviceTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onCompositeRecordConditionDefinitionGroupDirectiveReady = function (api) {
                compositeRecordConditionDefinitionGroupDirectiveAPI = api;
                compositeRecordConditionDefinitionGroupDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.isNameValid = function () {
                var volumePackageDefinitionName = $scope.scopeModel.name != undefined ? $scope.scopeModel.name.toLowerCase() : undefined;
                if (isEditMode && volumePackageDefinitionName == volumePackageDefinitionItem.Name.toLowerCase())
                    return null;

                for (var i = 0; i < volumePackageDefinitionItemsNames.length; i++) {
                    if (volumePackageDefinitionName == volumePackageDefinitionItemsNames[i].toLowerCase())
                        return 'Name already exists';
                }
                return null;
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
                if (isEditMode)
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

                    var serviceTypeSelectorPayload;
                    if (volumePackageDefinitionItem != undefined) {
                        serviceTypeSelectorPayload = { selectedIds: volumePackageDefinitionItem.ServiceTypeIds };
                    }
                    VRUIUtilsService.callDirectiveLoad(serviceTypeSelectorAPI, serviceTypeSelectorPayload, loadServiceTypeSelectorPromiseDeferred);
                });

                return loadServiceTypeSelectorPromiseDeferred.promise;
            }

            function loadCompositeRecordConditionDefinitionGroupDirective() {
                var loadCompositeRecordConditionDefinitionGroupDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                compositeRecordConditionDefinitionGroupDirectiveReadyDeferred.promise.then(function () {

                    var payload;
                    if (volumePackageDefinitionItem != undefined) {
                        payload = {
                            compositeRecordConditionDefinitionGroup: volumePackageDefinitionItem.CompositeRecordConditionDefinitionGroup
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(compositeRecordConditionDefinitionGroupDirectiveAPI, payload, loadCompositeRecordConditionDefinitionGroupDirectivePromiseDeferred);
                });
                return loadCompositeRecordConditionDefinitionGroupDirectivePromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadServiceTypeSelector, loadCompositeRecordConditionDefinitionGroupDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
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
                Name: $scope.scopeModel.name,
                ServiceTypeIds: serviceTypeSelectorAPI.getSelectedIds(),
                CompositeRecordConditionDefinitionGroup: compositeRecordConditionDefinitionGroupDirectiveAPI.getData()
            };
        }
    }

    appControllers.controller('Retail_BE_VolumePackageDefinitionSettingsEditorController', VolumePackageDefinitionSettingsEditorController);
})(appControllers);