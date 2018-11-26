(function (appControllers) {

    'use stict';

    VolumePackageSettingsService.$inject = ['VRModalService'];

    function VolumePackageSettingsService(VRModalService) {

        function addVolumePackageItem(onVolumePackageItemAdded) {
            var parameters = {};
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVolumePackageItemAdded = onVolumePackageItemAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Package/PackageExtendedSettings/VolumePackageSettingsEditor.html', parameters, modalSettings);
        }

        function editVolumePackageItem(volumePackageItem, onVolumePackageItemUpdated) {
            var parameters = {
                VolumePackageItem: volumePackageItem
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVolumePackageItemUpdated = onVolumePackageItemUpdated;
            };
            console.log(parameters);
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Package/PackageExtendedSettings/VolumePackageSettingsEditor.html', parameters, modalSettings);
        }

        return ({
            addVolumePackageItem: addVolumePackageItem,
            editVolumePackageItem: editVolumePackageItem
        });
    }

    appControllers.service('Retail_BE_VolumePackageSettingsService', VolumePackageSettingsService);

})(appControllers);