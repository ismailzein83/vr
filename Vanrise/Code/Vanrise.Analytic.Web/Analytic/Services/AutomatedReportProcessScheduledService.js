(function (appControllers) {
    'use stict';
    automatedReportProcessScheduledService.$inject = ['VRModalService'];
    function automatedReportProcessScheduledService(VRModalService) {


        function addQuery(onQueryAdded) {

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onQueryAdded = onQueryAdded;
            };

            var modalParameters = {
            };
            VRModalService.showModal('/Client/Modules/Analytic/Directives/AutomatedReport/Queries/Templates/AutomatedReportProcessScheduledQueryEditor.html', modalParameters, modalSettings);
        }


        function editQuery(object, onQueryUpdated) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modelScope) {
                modelScope.onQueryUpdated = onQueryUpdated;
            };
            var modalParameters = {
                Entity: object
            };
            VRModalService.showModal('/Client/Modules/Analytic/Directives/AutomatedReport/Queries/Templates/AutomatedReportProcessScheduledQueryEditor.html', modalParameters, modalSettings);
        }

        return {
           addQuery: addQuery,
           editQuery: editQuery,
        };
    }

    appControllers.service('VRAnalytic_AutomatedReportProcessScheduledService', automatedReportProcessScheduledService);

})(appControllers);
