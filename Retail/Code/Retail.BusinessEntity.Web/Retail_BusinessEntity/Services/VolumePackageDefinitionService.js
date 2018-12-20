(function (appControllers) {

    'use stict';

    VolumePackageDefinitionService.$inject = ['VRModalService'];

    function VolumePackageDefinitionService(VRModalService) {

        function addVolumePackageDefinitionItem(onVolumePackageDefinitionItemAdded, volumePackageDefinitionItemsNames) {
            var parameters = {
                volumePackageDefinitionItemsNames: volumePackageDefinitionItemsNames
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVolumePackageDefinitionItemAdded = onVolumePackageDefinitionItemAdded;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageDefinition/MainExtensions/ExtendedSettings/Templates/VolumePackageDefinitionSettingsEditorTemplate.html', parameters, modalSettings);
        }

        function editVolumePackageDefinitionItem(onVolumePackageDefinitionItemUpdated, updatedVolumePackageDefinitionItem, volumePackageDefinitionItemsNames) {

            var parameters = {
                VolumePackageDefinitionItem: updatedVolumePackageDefinitionItem, 
                volumePackageDefinitionItemsNames: volumePackageDefinitionItemsNames
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVolumePackageDefinitionItemUpdated = onVolumePackageDefinitionItemUpdated;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageDefinition/MainExtensions/ExtendedSettings/Templates/VolumePackageDefinitionSettingsEditorTemplate.html', parameters, modalSettings);
        }

        return ({
            addVolumePackageDefinitionItem: addVolumePackageDefinitionItem,
            editVolumePackageDefinitionItem: editVolumePackageDefinitionItem
        });
    }

    appControllers.service('Retail_BE_VolumePackageDefinitionService', VolumePackageDefinitionService);
})(appControllers);