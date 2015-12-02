(function (appControllers) {

    "use strict";
    TestCallAPIService.$inject = ['BaseAPIService', 'UtilsService', 'QM_CLITester_ModuleConfig'];

    function TestCallAPIService(BaseAPIService, UtilsService, QM_CLITester_ModuleConfig) {

        function GetCountries() {


            return BaseAPIService.get(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "GetCountries"));
        }


        function GetBreakouts(selectedCountry) {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "GetBreakouts"), { selectedCountry: selectedCountry });
        }

        function GetSuppliers() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "GetSuppliers"), {});
        }


        function GetTestCall(selectedCountry, selectedBreakout, selectedSupplier) {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "GetTestCall"),
            {
                selectedCountry: selectedCountry,
                selectedBreakout: selectedBreakout,
                selectedSupplier: selectedSupplier
            });
        }

        return ({
            GetCountries: GetCountries,
            GetBreakouts: GetBreakouts,
            GetSuppliers: GetSuppliers,
            GetTestCall: GetTestCall
        });
    }

    appControllers.service('QM_CLITester_TestCall', TestCallAPIService);

})(appControllers);