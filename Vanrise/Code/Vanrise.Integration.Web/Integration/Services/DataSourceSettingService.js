(function (appControllers) {

    'use strict';

    DataSourceSettingService.$inject = ['VRModalService', 'UtilsService'];

    function DataSourceSettingService(VRModalService, UtilsService) {

        function addFileDataSourceDefinition(onFileDataSourceDefinitionAdded) {
            var modalParameters = {
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onFileDataSourceDefinitionAdded = onFileDataSourceDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/Integration/Directives/Settings/FileDataSourceSettings/Templates/FileDataSourceDefinitionEditor.html', modalParameters, modalSettings);
        }

        function editFileDataSourceDefinition(onFileDataSourceDefinitionUpdated, fileDataSourceDefinitionEntity) {

            var modalParameters = {
                fileDataSourceDefinitionEntity: fileDataSourceDefinitionEntity
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onFileDataSourceDefinitionUpdated = onFileDataSourceDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Integration/Directives/Settings/FileDataSourceSettings/Templates/FileDataSourceDefinitionEditor.html', modalParameters, modalSettings);
        }

        return {
            addFileDataSourceDefinition: addFileDataSourceDefinition,
            editFileDataSourceDefinition: editFileDataSourceDefinition
        };
    }

    appControllers.service('VR_Integration_DataSourceSettingService', DataSourceSettingService);
})(appControllers);