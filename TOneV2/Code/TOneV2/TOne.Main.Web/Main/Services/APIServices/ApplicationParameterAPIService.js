app.service('ApplicationParameterAPIService', function (BaseAPIService) {
    return ({
        GetApplicationParameterById: GetApplicationParameterById,
        UpdateApplicationParameter: UpdateApplicationParameter
    });

    function GetApplicationParameterById(parameterId) {
        return BaseAPIService.get('api/ApplicationParameter/GetApplicationParameterById', {
            parameterId: parameterId
        });
    }

    function UpdateApplicationParameter(appParamObj) {
        return BaseAPIService.post('api/ApplicationParameter/UpdateApplicationParameter', {
            appParamObj: appParamObj
        })
    }
});