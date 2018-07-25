(function (appControllers) {

    'use strict';

    VRReportGenerationActionService.$inject = ['VRModalService', 'VR_Analytic_AdvancedExcelFileGeneratorAPIService', 'UtilsService'];

    function VRReportGenerationActionService(VRModalService, VR_Analytic_AdvancedExcelFileGeneratorAPIService, UtilsService) {
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
            registerDownloadFileAction: registerDownloadFileAction
        });
    };

    appControllers.service('VR_Analytic_ReportGenerationActionService', VRReportGenerationActionService);

})(appControllers);
