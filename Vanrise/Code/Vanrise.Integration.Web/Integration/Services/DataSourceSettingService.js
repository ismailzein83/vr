﻿(function (appControllers) {

    'use strict';

    DataSourceSettingService.$inject = ['VRModalService', 'UtilsService', 'VR_Integration_DataSourceSettingAPIService'];

    function DataSourceSettingService(VRModalService, UtilsService, VR_Integration_DataSourceSettingAPIService) {

        function addFileDataSourceDefinition(onFileDataSourceDefinitionAdded, isSingleInsert, fileImportExceptionNames) {
            var modalParameters = {
                fileImportExceptionNames: fileImportExceptionNames,
                isSingleInsert: isSingleInsert
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onFileDataSourceDefinitionAdded = onFileDataSourceDefinitionAdded;
            };
            VRModalService.showModal('/Client/Modules/Integration/Directives/Settings/FileDataSourceSettings/Templates/FileDataSourceDefinitionEditorTemplate.html', modalParameters, modalSettings);
        }

        function editFileDataSourceDefinition(onFileDataSourceDefinitionUpdated, fileDataSourceDefinitionEntity, fileImportExceptionNames) {

            var modalParameters = {
                fileImportExceptionNames: fileImportExceptionNames,
                fileDataSourceDefinitionEntity: fileDataSourceDefinitionEntity
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onFileDataSourceDefinitionUpdated = onFileDataSourceDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Integration/Directives/Settings/FileDataSourceSettings/Templates/FileDataSourceDefinitionEditorTemplate.html', modalParameters, modalSettings);
        }

        function viewFileDataSourceDefinition(fileDataSourceDefinitionId, viewMode) {
            VR_Integration_DataSourceSettingAPIService.GetFileDataSourceDefinition(fileDataSourceDefinitionId).then(function (response) {

                var parameters = {
                    fileDataSourceDefinitionEntity: response,
                    isReadOnly: true
                };

                var settings = {};
                settings.onScopeReady = function (modalScope) {
                    if (viewMode != undefined && viewMode == true) {
                        UtilsService.setContextReadOnly(modalScope);
                        modalScope.viewMode = true;
                    }
                };
                VRModalService.showModal('/Client/Modules/Integration/Directives/Settings/FileDataSourceSettings/Templates/FileDataSourceDefinitionEditorTemplate.html', parameters, settings);
            });
        }

        return {
            viewFileDataSourceDefinition: viewFileDataSourceDefinition,
            addFileDataSourceDefinition: addFileDataSourceDefinition,
            editFileDataSourceDefinition: editFileDataSourceDefinition
        };
    }

    appControllers.service('VR_Integration_DataSourceSettingService', DataSourceSettingService);
})(appControllers);