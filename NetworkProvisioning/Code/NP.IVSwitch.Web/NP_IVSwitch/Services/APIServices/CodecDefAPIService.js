
(function (appControllers) {

    "use strict";
    CodecDefAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig'];

    function CodecDefAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig) {

        var controllerName = "CodecDef";


        function GetCodecDefsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetCodecDefsInfo"), {
                filter: filter
            });
        }    

        function GetCodecDef(CodecDefId) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetCodecDef"), {
                CodecDefId: CodecDefId
            });
        }

        function GetCodecDefList(input) {
            console.log(input)

            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetCodecDefList"), input);
        }


        return ({
            GetCodecDefsInfo: GetCodecDefsInfo,
            GetCodecDef: GetCodecDef,
            GetCodecDefList: GetCodecDefList
        });
    }

    appControllers.service('NP_IVSwitch_CodecDefAPIService', CodecDefAPIService);

})(appControllers);