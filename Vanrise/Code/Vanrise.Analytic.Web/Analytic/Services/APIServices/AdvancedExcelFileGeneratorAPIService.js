(function (appControllers) {

    'use strict';

    AdvancedExcelFileGeneratorAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

    function AdvancedExcelFileGeneratorAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = 'AdvancedExcelFileGenerator';

        function DownloadAttachmentGenerator(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "DownloadAttachmentGenerator"), input, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }


        return {
            DownloadAttachmentGenerator: DownloadAttachmentGenerator
        };
    }

    appControllers.service('VR_Analytic_AdvancedExcelFileGeneratorAPIService', AdvancedExcelFileGeneratorAPIService);

})(appControllers);