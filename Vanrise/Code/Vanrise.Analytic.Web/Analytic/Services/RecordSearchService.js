(function (appControllers) {

    'use strict';

    RecordSearchService.$inject = ['VRModalService'];

    function RecordSearchService(VRModalService) {

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

        function addDRSearchPageSubviewDefinition(onSubviewDefinitionAdded, dataRecordTypeId) {

            var modalParameters = {
                dataRecordTypeId: dataRecordTypeId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSubviewDefinitionAdded = onSubviewDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Directives/Definition/AnalyticReport/RecordSearch/SourceSettings/Templates/DRSearchPageSubviewDefinitionEditor.html', modalParameters, modalSettings);
        }
        function editDRSearchPageSubviewDefinition(onSubviewDefinitionUpdated, subviewDefinition, dataRecordTypeId) {

            var modalParameters = {
                subviewDefinition: subviewDefinition,
                dataRecordTypeId: dataRecordTypeId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSubviewDefinitionUpdated = onSubviewDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Directives/Definition/AnalyticReport/RecordSearch/SourceSettings/Templates/DRSearchPageSubviewDefinitionEditor.html', modalParameters, modalSettings);
        }

        return {
            addDataRecordSource: addDataRecordSource,
            editDataRecordSource: editDataRecordSource,
            addDRSearchPageSubviewDefinition: addDRSearchPageSubviewDefinition,
            editDRSearchPageSubviewDefinition: editDRSearchPageSubviewDefinition
        };
    }

    appControllers.service('Analytic_RecordSearchService', RecordSearchService);

})(appControllers);