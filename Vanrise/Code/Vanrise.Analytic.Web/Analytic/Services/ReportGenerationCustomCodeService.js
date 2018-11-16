(function (appControllers) {
    'use stict';
    ReportGenerationCustomCodeService.$inject = ['VRModalService'];
    function ReportGenerationCustomCodeService(VRModalService) {
        function showCustomCodeCompilationErrors(errorMessages, context) {
            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
            };

            var modalParameters = {
                errorMessages: errorMessages,
                context: context
            };
            VRModalService.showModal('/Client/Modules/Analytic/Directives/VRReportGeneration/Templates/CustomCodeCompilationErrorsEditorTemplate.html', modalParameters, modalSettings);
        }
        return {
            showCustomCodeCompilationErrors: showCustomCodeCompilationErrors
        };
    }
    appControllers.service('VR_Analytic_ReportGenerationCustomCodeService', ReportGenerationCustomCodeService);
})(appControllers);
