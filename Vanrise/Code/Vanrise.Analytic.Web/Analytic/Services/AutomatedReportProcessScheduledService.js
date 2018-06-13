(function (appControllers) {
    'use stict';
    automatedReportProcessScheduledService.$inject = ['VRModalService'];
    function automatedReportProcessScheduledService(VRModalService) {


        function addQuery(onQueryAdded, context) {

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onQueryAdded = onQueryAdded;
            };

            var modalParameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/Analytic/Directives/AutomatedReport/Queries/Templates/AutomatedReportProcessScheduledQueryEditor.html', modalParameters, modalSettings);
        }


        function editQuery(object, onQueryUpdated, context) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modelScope) {
                modelScope.onQueryUpdated = onQueryUpdated;
            };
            var modalParameters = {
                Entity: object,
                context: context
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
