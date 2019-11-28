
app.service('VRCommon_CompilationService', ['VRModalService', 'VRNotificationService', 'UtilsService',
    function (VRModalService, VRNotificationService, UtilsService) {
        var drillDownDefinitions = [];
        return ({
            tryCompilationResult: tryCompilationResult
        });

        function tryCompilationResult(errorMessages, title) {
            var modalSettings = {};
            var modalParameters = {
                errorMessages: errorMessages,
                title: title
            };
            modalSettings.onScopeReady = function (modalScope) {
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRCompilation/VRCompilationCompileResult.html', modalParameters, modalSettings);
        }
    }]);
