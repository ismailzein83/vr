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
