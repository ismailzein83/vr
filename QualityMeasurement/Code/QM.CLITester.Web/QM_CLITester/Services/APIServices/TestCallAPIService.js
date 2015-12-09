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

        function AddNewTestCall(testCallObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "AddNewTestCall"), testCallObject);
        }

        function GetUpdatedTestCalls(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "GetUpdatedTestCalls"), input);
        }

        return ({
            GetCountries: GetCountries,
            GetBreakouts: GetBreakouts,
            GetSuppliers: GetSuppliers,
            AddNewTestCall: AddNewTestCall,
            GetUpdatedTestCalls: GetUpdatedTestCalls
        });
    }

    appControllers.service('Qm_CliTester_TestCallAPIService', TestCallAPIService);

})(appControllers);