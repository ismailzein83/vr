(function (appControllers) {

    'use strict';

    DataSourceSettingService.$inject = ['VRModalService'];

    function DataSourceSettingService(VRModalService) {

        function addFileDataSourceDefinition(onFileDataSourceDefinitionAdded, isSingleInsert) {
            var modalParameters = {
                isSingleInsert: isSingleInsert
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onFileDataSourceDefinitionAdded = onFileDataSourceDefinitionAdded;
            };
            VRModalService.showModal('/Client/Modules/Integration/Directives/Settings/FileDataSourceSettings/Templates/FileDataSourceDefinitionEditorTemplate.html', modalParameters, modalSettings);
        }

        function editFileDataSourceDefinition(onFileDataSourceDefinitionUpdated, fileDataSourceDefinitionEntity) {

            var modalParameters = {
                fileDataSourceDefinitionEntity: fileDataSourceDefinitionEntity
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onFileDataSourceDefinitionUpdated = onFileDataSourceDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Integration/Directives/Settings/FileDataSourceSettings/Templates/FileDataSourceDefinitionEditorTemplate.html', modalParameters, modalSettings);
        }

        return {
            addFileDataSourceDefinition: addFileDataSourceDefinition,
            editFileDataSourceDefinition: editFileDataSourceDefinition
        };
    }

    appControllers.service('VR_Integration_DataSourceSettingService', DataSourceSettingService);
})(appControllers);