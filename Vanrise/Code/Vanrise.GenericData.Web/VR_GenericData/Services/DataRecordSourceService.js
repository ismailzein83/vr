(function (appControllers) {

    'use strict';

    DataRecordSourceService.$inject = ['VRModalService'];

    function DataRecordSourceService(VRModalService) {
        return {
            addDataRecordSource: addDataRecordSource,
            editDataRecordSource: editDataRecordSource
        };

        function addDataRecordSource(onDataRecordSourceAdded, existingSources) {
            var modalParameters = {
                ExistingSources: existingSources
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordSourceAdded = onDataRecordSourceAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/DataRecordSource/DataRecordSourceEditor.html', modalParameters, modalSettings);
        }

        function editDataRecordSource(dataRecordSource, existingSources, onDataRecordSourceUpdated) {
            var modalParameters = {
                DataRecordSource: dataRecordSource,
                ExistingSources: existingSources
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