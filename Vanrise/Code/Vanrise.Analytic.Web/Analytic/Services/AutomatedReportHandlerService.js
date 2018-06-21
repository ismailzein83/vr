(function (appControllers) {
    'use stict';
    automatedReportHandlerService.$inject = ['VRModalService'];
    function automatedReportHandlerService(VRModalService) {


        function addAttachementGenerator(onAttachementGeneratorAdded, context) {

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAttachementGeneratorAdded = onAttachementGeneratorAdded;
            };

            var modalParameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Handler/Templates/AutomatedReportHandlerEditor.html', modalParameters, modalSettings);
        }

        function editAttachementGenerator(object, onAttachementGeneratorUpdated, context) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modelScope) {
                modelScope.onAttachementGeneratorUpdated = onAttachementGeneratorUpdated;
            };
            var modalParameters = {
                Entity: object,
                context: context
            };
            VRModalService.showModal('/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Handler/Templates/AutomatedReportHandlerEditor.html', modalParameters, modalSettings);
        }

        return {
            addAttachementGenerator: addAttachementGenerator,
            editAttachementGenerator: editAttachementGenerator,
        };
    } 

    appControllers.service('VRAnalytic_AutomatedReportHandlerService', automatedReportHandlerService);

})(appControllers);
