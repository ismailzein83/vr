app.service('NumberProfileAPIService', function (BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig) {

    var controllerName = 'NumberProfile';

    function GetNumberProfiles(input) {
        return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, "GetNumberProfiles"), input);
    }

    return ({
        GetNumberProfiles: GetNumberProfiles
    });


});