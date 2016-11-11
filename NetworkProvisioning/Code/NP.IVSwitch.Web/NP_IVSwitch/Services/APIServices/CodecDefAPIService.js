
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
         

        return ({
            GetCodecDefsInfo: GetCodecDefsInfo,
         });
    }

    appControllers.service('NP_IVSwitch_CodecDefAPIService', CodecDefAPIService);

})(appControllers);