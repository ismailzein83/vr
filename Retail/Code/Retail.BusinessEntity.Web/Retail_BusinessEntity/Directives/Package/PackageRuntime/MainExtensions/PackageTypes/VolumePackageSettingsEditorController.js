(function (appControllers) {

    "use strict";

    VolumePackageSettingsEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Retail_BE_VolumePackageDefinitionAPIService'];

    function VolumePackageSettingsEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_BE_VolumePackageDefinitionAPIService) {

        var isEditMode;
        var volumePackageItem;
        var volumePackageDefinitionId;
        var volumePackageDefinitionItems;
        var volumePackageDefinitionItemId;
        var compositeRecordConditionResolvedDataList;

        var volumePackageDefinitionItemSelectionDeferred;

        var compositeRecordConditionDirectiveAPI;
        var compositeRecordConditionDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                volumePackageDefinitionId = parameters.volumePackageDefinitionId;
                volumePackageDefinitionItems = parameters.volumePackageDefinitionItems;
                volumePackageItem = parameters.volumePackageItem;
            }

            isEditMode = (volumePackageItem != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onCompositeRecordConditionDirectiveReady = function (api) {
                compositeRecordConditionDirectiveAPI = api;
                compositeRecordConditionDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onVolumePackageDefinitionItemSelectionChanged = function (selectedVolumePackageDefinitionItem) {
                if (selectedVolumePackageDefinitionItem == undefined)
                    return;

                volumePackageDefinitionItemId = selectedVolumePackageDefinitionItem.VolumePackageDefinitionItemId;

                Retail_BE_VolumePackageDefinitionAPIService.GetCompositeRecordConditionResolvedDataList(volumePackageDefinitionId, volumePackageDefinitionItemId).then(function (response) {
                    compositeRecordConditionResolvedDataList = response;
                    if (volumePackageDefinitionItemSelectionDeferred != undefined) {
                        volumePackageDefinitionItemSelectionDeferred.resolve();
                    }
                    else {
                        var compositeRecordConditionDirectivePayload = {
                            compositeRecordConditionDefinitionGroup: selectedVolumePackageDefinitionItem.CompositeRecordConditionDefinitionGroup,
                            compositeRecordConditionResolvedDataList: compositeRecordConditionResolvedDataList
                        };
                        compositeRecordConditionDirectiveAPI.load(compositeRecordConditionDirectivePayload);
                    }
                });
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
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(volumePackageItem.VolumePackageDefinitionItemName, 'Volume Package') : UtilsService.buildTitleForAddEditor('Volume Package');
            }

            function loadStaticData() {
                if (volumePackageItem != undefined) {
                    $scope.scopeModel.volume = volumePackageItem.Volume;
                }
            }

            function loadVolumePackageDefinitionItemSelector() {
                $scope.scopeModel.volumePackageDefinitionItems = volumePackageDefinitionItems;

                if ($scope.scopeModel.volumePackageDefinitionItems != undefined && volumePackageItem != undefined) {
                    $scope.scopeModel.selectedVolumePackageDefinitionItem = UtilsService.getItemByVal($scope.scopeModel.volumePackageDefinitionItems,
                        volumePackageItem.VolumePackageDefinitionItemId, 'VolumePackageDefinitionItemId');
                }
            }

            function loadCompositeRecordConditionDirective() {
                if (!isEditMode)
                    return;

                volumePackageDefinitionItemSelectionDeferred = UtilsService.createPromiseDeferred();

                var loadCompositeRecordConditionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([compositeRecordConditionDirectiveReadyDeferred.promise, volumePackageDefinitionItemSelectionDeferred.promise]).then(function () {
                    volumePackageDefinitionItemSelectionDeferred = undefined;

                    var compositeRecordConditionDirectivePayload = {
                        compositeRecordConditionDefinitionGroup: $scope.scopeModel.selectedVolumePackageDefinitionItem.CompositeRecordConditionDefinitionGroup,
                        compositeRecordConditionResolvedDataList: compositeRecordConditionResolvedDataList
                    };
                    if (volumePackageItem != undefined) {
                        compositeRecordConditionDirectivePayload.conditionRecordDefinition = volumePackageItem.Condition;
                    }
                    VRUIUtilsService.callDirectiveLoad(compositeRecordConditionDirectiveAPI, compositeRecordConditionDirectivePayload, loadCompositeRecordConditionDirectivePromiseDeferred);
                });

                return loadCompositeRecordConditionDirectivePromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVolumePackageDefinitionItemSelector, loadCompositeRecordConditionDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insertPackage() {
            var volumePackageSettingsObj = buildVolumePackageSettingObjFromScope();

            if ($scope.onVolumePackageItemAdded != undefined)
                $scope.onVolumePackageItemAdded(volumePackageSettingsObj);

            $scope.modalContext.closeModal();
        }

        function updatePackage() {
            var volumePackageSettingsObj = buildVolumePackageSettingObjFromScope();

            if ($scope.onVolumePackageItemUpdated != undefined)
                $scope.onVolumePackageItemUpdated(volumePackageSettingsObj);

            $scope.modalContext.closeModal();
        }

        function buildVolumePackageSettingObjFromScope() {
            return {
                VolumePackageItemId: volumePackageItem != undefined ? volumePackageItem.VolumePackageItemId : UtilsService.guid(),
                VolumePackageDefinitionItemId: $scope.scopeModel.selectedVolumePackageDefinitionItem.VolumePackageDefinitionItemId,
                Condition: compositeRecordConditionDirectiveAPI.getData(),
                Volume: $scope.scopeModel.volume
            };
        }
    }

    appControllers.controller('Retail_BE_VolumePackageSettingsEditorController', VolumePackageSettingsEditorController);
})(appControllers);