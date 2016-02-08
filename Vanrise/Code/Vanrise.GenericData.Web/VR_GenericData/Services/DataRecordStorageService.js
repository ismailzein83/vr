(function (appControllers) {

    'use strict';

    DataRecordStorageService.$inject = ['VR_GenericData_GenericRuleDefinitionAPIService', 'VRModalService', 'VRNotificationService'];

    function DataRecordStorageService(VR_GenericData_GenericRuleDefinitionAPIService, VRModalService, VRNotificationService) {
        return {
            addDataRecordStorage: addDataRecordStorage,
            editDataRecordStorage: editDataRecordStorage
        };

        function addDataRecordStorage(onDataRecordStorageAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordStorageAdded = onDataRecordStorageAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataRecordStorage/DataRecordStorageEditor.html', undefined, modalSettings);
        }

        function editDataRecordStorage(dataRecordStorageId, onDataRecordStorageUpdated) {
            var modalParameters = {
                DataRecordStorageId: dataRecordStorageId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordStorageUpdated = onDataRecordStorageUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataRecordStorage/DataRecordStorageEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VR_GenericData_DataRecordStorageService', DataRecordStorageService);

})(appControllers);