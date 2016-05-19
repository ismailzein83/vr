(function (appControllers) {

    'use strict';

    RecordSearchService.$inject = ['VRModalService'];

    function RecordSearchService(VRModalService) {
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

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/DataRecordSourceEditor.html', modalParameters, modalSettings);
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

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/DataRecordSourceEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('Analytic_RecordSearchService', RecordSearchService);

})(appControllers);