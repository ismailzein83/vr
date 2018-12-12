(function (appControllers) {

    "use strict";

    VolumePackageSettingsEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function VolumePackageSettingsEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var volumePackageItem;

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                volumePackageItem = parameters.VolumePackageItem;
            }
            console.log(volumePackageItem);

            isEditMode = (volumePackageItem != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

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
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }
        function setTitle() {

            $scope.title =
                isEditMode ? UtilsService.buildTitleForUpdateEditor(volumePackageItem.Volume, 'Volume Package') : UtilsService.buildTitleForAddEditor('Volume Package');
        }
        function loadStaticData() {
            if (volumePackageItem != undefined)
                $scope.scopeModel.volume = volumePackageItem.Volume;
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
                //VolumePackageDefinitionItemId: $scope.scopeModel.,
                //Condition: $scope.scopeModel.,
                Volume: $scope.scopeModel.volume
            };
        }
    }

    appControllers.controller('Retail_BE_VolumePackageSettingsEditorController', VolumePackageSettingsEditorController);
})(appControllers);

//(function (appControllers) {

//    "use strict";

//    VolumePackageSettingsEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Retail_BE_VolumePackageAPIService'];

//    function VolumePackageSettingsEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_BE_VolumePackageAPIService) {

//        var isEditMode;
//        var volumePackageItem;
//        var volumePackageDefinitionId;
//        var volumePackageDefinitionItems;
//        var compositeRecordConditionResolvedDataList;
//       // var selectedVolumePackageItem;

//        var compositeRecordConditionDirectiveAPI;
//        var compositeRecordConditionDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

//        var volumePackageDefinitionItemSelectionDeferred;

//        loadParameters();
//        defineScope();
//        load();

//        function loadParameters() {
//            var parameters = VRNavigationService.getParameters($scope);

//            if (parameters != undefined) {
//                volumePackageDefinitionId = parameters.volumePackageDefinitionId;
//                volumePackageDefinitionItems = parameters.volumePackageDefinitionItems;
//                volumePackageItem = parameters.volumePackageItem;
//            }
//            isEditMode = (volumePackageItem != undefined);
//        }

//        function defineScope() {
//            $scope.scopeModel = {};
//            $scope.scopeModel.volumePackageDefinitionItems = volumePackageDefinitionItems;

//            $scope.scopeModel.onCompositeRecordConditionDirectiveReady = function (api) {
//                compositeRecordConditionDirectiveAPI = api;
//                compositeRecordConditionDirectiveReadyDeferred.resolve();
//            };

//            $scope.scopeModel.onVolumePackageDefinitionItemSelectionChanged = function (selectedVolumePackageDefinitionItem) {
//                if (selectedVolumePackageDefinitionItem == undefined)
//                    return;

//                var volumePackageDefinitionItemId = selectedVolumePackageDefinitionItem.VolumePackageDefinitionItemId;
//                Retail_BE_VolumePackageAPIService.GetCompositeRecordConditionResolvedDataList(volumePackageDefinitionId, volumePackageDefinitionItemId).then(function (response) {
//                    compositeRecordConditionResolvedDataList = response;
//                    //volumePackageItem = selectedVolumePackageDefinitionItem;
//                    volumePackageDefinitionItemSelectionDeferred.resolve();
//                    loadCompositeRecordConditionDirective();
//                });
//            };

//            $scope.scopeModel.save = function () {
//                if (isEditMode) {
//                    return updatePackage();
//                }
//                else {
//                    return insertPackage();
//                }
//            };
//            $scope.scopeModel.close = function () {
//                $scope.modalContext.closeModal();
//            };
//        }

//        function load() {
//            $scope.isLoading = true;
//            loadAllControls();
//        }

//        function loadAllControls() {
//            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCompositeRecordConditionDirective])
//                .catch(function (error) {
//                    VRNotificationService.notifyExceptionWithClose(error, $scope);
//                })
//                .finally(function () {
//                    $scope.isLoading = false;
//                });
//        }

//        function setTitle() {
//            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(volumePackageItem.VolumePackageDefinitionItemName, 'Volume Package') : UtilsService.buildTitleForAddEditor('Volume Package');
//        }

//        function loadStaticData() {
//            if (volumePackageItem != undefined) {
//                $scope.scopeModel.volume = volumePackageItem.Volume;
//                $scope.scopeModel.selectedVolumePackageDefinitionItem = UtilsService.getItemByVal(volumePackageDefinitionItems, volumePackageItem.VolumePackageDefinitionItemId, 'VolumePackageDefinitionItemId');
//            }
//        }

//        function loadCompositeRecordConditionDirective() {
//            var loadCompositeRecordConditionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
//            volumePackageDefinitionItemSelectionDeferred = UtilsService.createPromiseDeferred();

//            compositeRecordConditionDirectiveReadyDeferred.promise.then(function () {
//                volumePackageDefinitionItemSelectionDeferred.promise.then(function () {

//                    var compositeRecordConditionDirectivePayload = {
//                        volumePackageDefinitionItems: $scope.scopeModel.selectedVolumePackageDefinitionItem.CompositeRecordConditionDefinitionGroup,
//                        compositeRecordConditionResolvedDataList: compositeRecordConditionResolvedDataList
//                    };

//                    if (volumePackageItem != undefined) {
//                        compositeRecordConditionDirectivePayload.volumePackageItem = volumePackageItem;
//                    }

//                    VRUIUtilsService.callDirectiveLoad(compositeRecordConditionDirectiveAPI, compositeRecordConditionDirectivePayload, loadCompositeRecordConditionDirectivePromiseDeferred);
//                });
//            });
//            return loadCompositeRecordConditionDirectivePromiseDeferred.promise;
//        }

//        function insertPackage() {
//            var volumePackageSettingsObj = buildVolumePackageSettingObjFromScope();

//            if ($scope.onVolumePackageItemAdded != undefined)
//                $scope.onVolumePackageItemAdded(volumePackageSettingsObj);

//            $scope.modalContext.closeModal();
//        }
//        function updatePackage() {
//            var volumePackageSettingsObj = buildVolumePackageSettingObjFromScope();

//            if ($scope.onVolumePackageItemUpdated != undefined)
//                $scope.onVolumePackageItemUpdated(volumePackageSettingsObj);

//            $scope.modalContext.closeModal();
//        }
//        function buildVolumePackageSettingObjFromScope() {
//            return {
//                VolumePackageItemId: volumePackageItem != undefined ? volumePackageItem.VolumePackageItemId : UtilsService.guid(),
//                VolumePackageDefinitionItemId: $scope.scopeModel.selectedVolumePackageDefinitionItem.VolumePackageDefinitionItemId,
//                Condition: compositeRecordConditionDirectiveAPI.getData(),
//                Volume: $scope.scopeModel.volume
//            };
//        }
//    }
//    appControllers.controller('Retail_BE_VolumePackageSettingsEditorController', VolumePackageSettingsEditorController);
//})(appControllers);
