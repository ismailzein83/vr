(function (appControllers) {

    "use strict";
    parserTypeConfigsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_DataParser_ModuleConfig'];

    function parserTypeConfigsAPIService(BaseAPIService, UtilsService, VR_DataParser_ModuleConfig) {

        var controllerName = 'ParserTypeConfigs';

        function GetParserTypeTemplateConfigs() {

            return BaseAPIService.get(UtilsService.getServiceURL(VR_DataParser_ModuleConfig.moduleName, controllerName, "GetParserTypeTemplateConfigs"));
        }
        function GetRecordeReaderTemplateConfigs() {

            return BaseAPIService.get(UtilsService.getServiceURL(VR_DataParser_ModuleConfig.moduleName, controllerName, "GetRecordeReaderTemplateConfigs"));
        }
        function GetTagValueParserTemplateConfigs() {

            return BaseAPIService.get(UtilsService.getServiceURL(VR_DataParser_ModuleConfig.moduleName, controllerName, "GetTagValueParserTemplateConfigs"));
        }

        function GetBinaryRecordReaderTemplateConfigs() {

            return BaseAPIService.get(UtilsService.getServiceURL(VR_DataParser_ModuleConfig.moduleName, controllerName, "GetBinaryRecordReaderTemplateConfigs"));
        }

        return ({
            GetParserTypeTemplateConfigs: GetParserTypeTemplateConfigs,
            GetRecordeReaderTemplateConfigs: GetRecordeReaderTemplateConfigs,
            GetTagValueParserTemplateConfigs: GetTagValueParserTemplateConfigs,
            GetBinaryRecordReaderTemplateConfigs: GetBinaryRecordReaderTemplateConfigs
        });
    }

    appControllers.service('VR_DataParser_ParserTypeConfigsAPIService', parserTypeConfigsAPIService);

})(appControllers);