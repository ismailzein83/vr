(function (appControllers) {

    'use strict';

    AnalyticService.$inject = ['VRModalService'];

    function AnalyticService(VRModalService) {
        return ({
            OpenAnalyticReport: OpenAnalyticReport,
            addWidget: addWidget,
            editWidget: editWidget,
            editRealTimeWidget: editRealTimeWidget,
            addRealTimeWidget: addRealTimeWidget,
            addMeasureStyle: addMeasureStyle,
            editMeasureStyle: editMeasureStyle,
            openGridWidgetSettings: openGridWidgetSettings
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

        function addRealTimeWidget(onWidgetAdded, tableIds) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onWidgetAdded = onWidgetAdded;
            };
            var modalParameters = {
                tableIds: tableIds
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/RealTimeWidgetEditor.html', modalParameters, modalSettings);
        }

        function editRealTimeWidget(widgetEntity, onWidgetUpdated, tableIds) {
            var modalParameters = {
                tableIds: tableIds,
                widgetEntity: widgetEntity
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onWidgetUpdated = onWidgetUpdated;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/RealTimeWidgetEditor.html', modalParameters, modalSettings);
        }

        function addMeasureStyle(onMeasureStyleAdded, selectedMeasure, context, analyticTableId) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onMeasureStyleAdded = onMeasureStyleAdded;
            };
            var modalParameters = {
                selectedMeasure: selectedMeasure,
                context: context,
                analyticTableId: analyticTableId
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/MeasureStyleEditor.html', modalParameters, modalSettings);
        }

        function editMeasureStyle(measureStyle, onMeasureStyleUpdated, selectedMeasure, context, analyticTableId) {
            var modalParameters = {
                selectedMeasure: selectedMeasure,
                measureStyle: measureStyle,
                context: context,
                analyticTableId: analyticTableId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onMeasureStyleUpdated = onMeasureStyleUpdated;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/MeasureStyleEditor.html', modalParameters, modalSettings);
        }

        function openGridWidgetSettings(onSaveSettings, context, measureStyleRules, analyticTableId)
        {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSaveSettings = onSaveSettings;
            };
            var parameters = {
                context: context,
                measureStyleRules: measureStyleRules,
                analyticTableId: analyticTableId
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticGridWidgetSettings.html', parameters, settings);
        }
    }

    appControllers.service('Analytic_AnalyticService', AnalyticService);

})(appControllers);
