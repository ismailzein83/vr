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

        function GetBaseStringParserTemplateConfigs() {

            return BaseAPIService.get(UtilsService.getServiceURL(VR_DataParser_ModuleConfig.moduleName, controllerName, "GetBaseStringParserTemplateConfigs"));
        }
        function GetCompositeFieldsReaderTemplateConfigs() {

            return BaseAPIService.get(UtilsService.getServiceURL(VR_DataParser_ModuleConfig.moduleName, controllerName, "GetCompositeFieldsReaderTemplateConfigs"));
        }
        function GetPackageFieldsReaderTemplateConfigs() {

            return BaseAPIService.get(UtilsService.getServiceURL(VR_DataParser_ModuleConfig.moduleName, controllerName, "GetPackageFieldsReaderTemplateConfigs"));
        }
        return ({
            GetParserTypeTemplateConfigs: GetParserTypeTemplateConfigs,
            GetRecordeReaderTemplateConfigs: GetRecordeReaderTemplateConfigs,
            GetTagValueParserTemplateConfigs: GetTagValueParserTemplateConfigs,
            GetBinaryRecordReaderTemplateConfigs: GetBinaryRecordReaderTemplateConfigs,
            GetBaseStringParserTemplateConfigs: GetBaseStringParserTemplateConfigs,
            GetCompositeFieldsReaderTemplateConfigs: GetCompositeFieldsReaderTemplateConfigs,
            GetPackageFieldsReaderTemplateConfigs: GetPackageFieldsReaderTemplateConfigs
        });
    }

    appControllers.service('VR_DataParser_ParserTypeConfigsAPIService', parserTypeConfigsAPIService);

})(appControllers);