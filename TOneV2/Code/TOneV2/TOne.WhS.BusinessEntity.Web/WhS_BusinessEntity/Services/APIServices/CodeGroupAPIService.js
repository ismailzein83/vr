(function (appControllers) {

    "use strict";
    codeGroupAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function codeGroupAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredCodeGroups(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CodeGroup", "GetFilteredCodeGroups"), input);
        }
        //function GetAllCountries() {
        //    return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Country", "GetAllCountries"));
        //}
        //function GetCountry(countryId) {
        //    return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Country", "GetCountry"), {
        //        countryId: countryId
        //    });

        //}
        //function UpdateCountry(countryObject) {
        //    return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Country", "UpdateCountry"), countryObject);
        //}
        //function AddCountry(countryObject) {
        //    return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Country", "AddCountry"), countryObject);
        //}
        return ({
            GetFilteredCodeGroups: GetFilteredCodeGroups
            //,
            //GetAllCountries: GetAllCountries,
            //GetCountry: GetCountry,
            //UpdateCountry: UpdateCountry,
            //AddCountry: AddCountry
        });
    }

    appControllers.service('WhS_BE_CodeGroupAPIService', codeGroupAPIService);

})(appControllers);