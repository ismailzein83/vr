(function (appControllers) {

    'use stict';

    VolumePackageService.$inject = ['VRModalService'];

    function VolumePackageService(VRModalService) {

        function addVolumePackageItem(onVolumePackageItemAdded, volumePackageDefinitionItems, volumePackageDefinitionId) {
            var parameters = {
                volumePackageDefinitionId: volumePackageDefinitionId,
                volumePackageDefinitionItems: volumePackageDefinitionItems
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVolumePackageItemAdded = onVolumePackageItemAdded;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageRuntime/MainExtensions/PackageTypes/Templates/VolumePackageSettingsEditor.html', parameters, modalSettings);
        }

        function editVolumePackageItem(onVolumePackageItemUpdated, volumePackageItem, volumePackageDefinitionItems, volumePackageDefinitionId) {
            var parameters = {
                volumePackageDefinitionId: volumePackageDefinitionId,
                volumePackageDefinitionItems: volumePackageDefinitionItems,
                volumePackageItem: volumePackageItem
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVolumePackageItemUpdated = onVolumePackageItemUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageRuntime/MainExtensions/PackageTypes/Templates/VolumePackageSettingsEditor.html', parameters, modalSettings);
        }

        return ({
            addVolumePackageItem: addVolumePackageItem,
            editVolumePackageItem: editVolumePackageItem
        });
    }

    appControllers.service('Retail_BE_VolumePackageService', VolumePackageService);
})(appControllers);