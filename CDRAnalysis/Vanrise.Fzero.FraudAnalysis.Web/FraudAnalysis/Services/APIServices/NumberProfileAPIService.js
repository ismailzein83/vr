app.service('NumberProfileAPIService', function (BaseAPIService) {

    var controllerName = 'NumberProfile';

    function GetNumberProfiles(input) {
        return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, "GetNumberProfiles"), input);
    }

    return ({
        GetNumberProfiles: GetNumberProfiles
    });


});