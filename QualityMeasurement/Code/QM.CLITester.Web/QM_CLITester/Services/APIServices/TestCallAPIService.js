(function (appControllers) {

    "use strict";
    TestCallAPIService.$inject = ['BaseAPIService', 'UtilsService', 'QM_CLITester_ModuleConfig'];

    function TestCallAPIService(BaseAPIService, UtilsService, QM_CLITester_ModuleConfig) {

        function GetCountries() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "GetCountries"), {});
        }


        function GetBreakouts(selectedCountry) {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "GetBreakouts"), {selectedCountry});
        }

        return ({
            GetCountries: GetCountries,
            GetBreakouts: GetBreakouts
        });
    }

    appControllers.service('QM_CLITester_TestCall', TestCallAPIService);

})(appControllers);