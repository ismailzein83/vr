(function (appControllers) {

    "use strict";
    parserTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_DataParser_ModuleConfig'];

    function parserTypeAPIService(BaseAPIService, UtilsService, VR_DataParser_ModuleConfig) {

        var controllerName = 'ParserType';

        function GetFilteredParserTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_DataParser_ModuleConfig.moduleName, controllerName, "GetFilteredParserTypes"), input);
        }

        function GetParserTypesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_DataParser_ModuleConfig.moduleName, controllerName, "GetParserTypesInfo"));
        }

        function GetParserType(parserTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_DataParser_ModuleConfig.moduleName, controllerName, "GetParserType"), {
                parserTypeId: parserTypeId
            });
        }

        function UpdateParserType(parserTypeObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_DataParser_ModuleConfig.moduleName, controllerName, "UpdateParserType"), parserTypeObject);
        }

        function AddParserType(parserTypeObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_DataParser_ModuleConfig.moduleName, controllerName, "AddParserType"), parserTypeObject);
        }

    return ({
        GetFilteredParserTypes: GetFilteredParserTypes,
        GetParserTypesInfo: GetParserTypesInfo,
        GetParserType:GetParserType,
        UpdateParserType:UpdateParserType,
        AddParserType:AddParserType
        });
    }

    appControllers.service('VR_DataParser_ParserTypeAPIService', parserTypeAPIService);

})(appControllers);