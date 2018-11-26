(function (appControllers) {

    'use stict';

    VolumePackageDefinitionSettingsService.$inject = ['VRModalService'];

    function VolumePackageDefinitionSettingsService(VRModalService) {

        function addVolumePackageDefinitionItem(onVolumePackageDefinitionItemAdded) {
            var parameters = {};
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVolumePackageDefinitionItemAdded = onVolumePackageDefinitionItemAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageDefinition/MainExtensions/ExtendedSettings/Templates/VolumePackageDefinitionSettingsEditor.html', parameters, modalSettings);
        }

        function editVolumePackageDefinitionItem(updatedVolumePackageDefinitionItem, onVolumePackageDefinitionItemUpdated) {
            var parameters = {
                VolumePackageDEfinitionItem: updatedVolumePackageDefinitionItem
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVolumePackageDefinitionItemUpdated = onVolumePackageDefinitionItemUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageDefinition/MainExtensions/ExtendedSettings/Templates/VolumePackageDefinitionSettingsEditor.html', parameters, modalSettings);
        }

        return ({
            addVolumePackageDefinitionItem: addVolumePackageDefinitionItem,
            editVolumePackageDefinitionItem: editVolumePackageDefinitionItem
        });
    }

    appControllers.service('Retail_BE_VolumePackageDefinitionSettingsService', VolumePackageDefinitionSettingsService);

})(appControllers);