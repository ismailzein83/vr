(function (appControllers) {

    'use strict';

    AnalyticService.$inject = ['VRModalService'];

    function AnalyticService(VRModalService) {
        return ({
            OpenAnalyticReport: OpenAnalyticReport,
            addWidget: addWidget,
            editWidget: editWidget
        });

        function OpenAnalyticReport() {
            var parameters = {
            };
            var modalSettings = {
                useModalTemplate: true,
                width: "80%",
                maxHeight: "800px",
                title: entityName
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/Reports/AnalyticReport.html', parameters, modalSettings);
        }

        function addWidget(onWidgetAdded, tableIds) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onWidgetAdded = onWidgetAdded;
            };
            var modalParameters = {
                tableIds: tableIds
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticWidgetEditor.html', modalParameters, modalSettings);
        }

        function editWidget(widgetEntity, onWidgetUpdated, tableIds) {
            var modalParameters = {
                tableIds: tableIds,
                widgetEntity: widgetEntity
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onWidgetUpdated = onWidgetUpdated;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticWidgetEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('Analytic_AnalyticService', AnalyticService);

})(appControllers);
