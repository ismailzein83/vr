(function (appControllers) {

    "use strict";
    BusinessProcess_BPVisualItemDefintionService.$inject = ['VRModalService'];
    function BusinessProcess_BPVisualItemDefintionService(VRModalService) {

        function openHumanTaskTrackingProgress(events) {

            var modalParameters = {
                events: events
            };

            var modalSettings = {
                size: "medium"
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Human Activity Tracking Progress";
            };
            VRModalService.showModal('/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/IconsEditors/Templates/WorkflowVisualItemDefintionHumanTaskIconTemplate.html', modalParameters, modalSettings);
        }   

        function openEventsTracking(events, title) {
            var modalParameters = {
                events: events
            };

            var modalSettings = {
                size: "medium"
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = title;
            };
            VRModalService.showModal('/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/IconsEditors/Templates/WorkflowVisualItemDefinitionCallHttpIconTemplate.html', modalParameters, modalSettings);
        }

        return ({
            openHumanTaskTrackingProgress: openHumanTaskTrackingProgress,
            openEventsTracking: openEventsTracking
        });
    }
    appControllers.service('BusinessProcess_BPVisualItemDefintionService', BusinessProcess_BPVisualItemDefintionService);

})(appControllers);