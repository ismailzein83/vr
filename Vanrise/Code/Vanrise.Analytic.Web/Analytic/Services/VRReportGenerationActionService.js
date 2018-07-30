(function (appControllers) {

    'use strict';

    VRReportGenerationActionService.$inject = ['VRModalService', 'VR_Analytic_AdvancedExcelFileGeneratorAPIService', 'UtilsService', 'VRCommon_ObjectTrackingService'];

    function VRReportGenerationActionService(VRModalService, VR_Analytic_AdvancedExcelFileGeneratorAPIService, UtilsService, VRCommon_ObjectTrackingService) {
        var actionTypes = [];

        function registerActionType(actionType) {
            actionTypes.push(actionType);
        }

        function getActionTypeIfExistByName(actionTypeName) {
            for (var i = 0; i < actionTypes.length; i++) {
                var actionType = actionTypes[i];
                if (actionType.ActionTypeName == actionTypeName)
                    return actionType;
            }
        }

        function viewHistoryReportGeneration(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/VRReportGeneration/VRReportGenerationEditor.html', modalParameters, modalSettings);
        };
        function registerHistoryViewAction() {
            var actionHistory = {
                actionHistoryName: "VR_Analytic_ReportGeneration_ViewHistoryItem",
                actionMethod: function (payload) {
                    var context = {
                        historyId: payload.historyId
                    };
                    viewHistoryReportGeneration(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }
        function registerDownloadFileAction() {

            var actionType = {
                ActionTypeName: "DownloadFile",
                ExecuteAction: function (payload) {

                    var vRReportGeneration = payload.vRReportGeneration;
                    var input = {
                        FileGenerator: vRReportGeneration.Settings.ReportAction.FileGenerator,
                        Queries: vRReportGeneration.Settings.Queries
                    };
                    return VR_Analytic_AdvancedExcelFileGeneratorAPIService.DownloadAttachmentGenerator(input).then(function (response) {
                        if (response != undefined)
                            UtilsService.downloadFile(response.data, response.headers);
                    });
                }
            };

            registerActionType(actionType);
        }
        return ({
            registerActionType: registerActionType,
            getActionTypeIfExistByName: getActionTypeIfExistByName,
            registerDownloadFileAction: registerDownloadFileAction,
            registerHistoryViewAction: registerHistoryViewAction
        });
    };

    appControllers.service('VR_Analytic_ReportGenerationActionService', VRReportGenerationActionService);

})(appControllers);
