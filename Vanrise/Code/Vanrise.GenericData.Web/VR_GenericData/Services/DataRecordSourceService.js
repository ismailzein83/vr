(function (appControllers) {

    'use strict';

    DataRecordSourceService.$inject = ['VRModalService'];

    function DataRecordSourceService(VRModalService) {
        return {
            addDataRecordSource: addDataRecordSource,
            editDataRecordSource: editDataRecordSource
        };

        function addDataRecordSource(onDataRecordSourceAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordSourceAdded = onDataRecordSourceAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataRecordSource/DataRecordSourceEditor.html', undefined, modalSettings);
        }

        function editDataRecordSource(dataRecordSource, onDataRecordSourceUpdated) {
            var modalParameters = {
                DataRecordSource: dataRecordSource
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordSourceUpdated = onDataRecordSourceUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataRecordSource/DataRecordSourceEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VR_GenericData_DataRecordSourceService', DataRecordSourceService);

})(appControllers);