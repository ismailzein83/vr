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

        function AddNewTestCall(testCallObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "AddNewTestCall"), testCallObject);
        }

        function GetUpdatedTestCalls(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "GetUpdatedTestCalls"), input);
        }

        function GetFilteredTestCalls(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "GetFilteredTestCalls"), input);
        }

        function GetInitiateTestTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "GetInitiateTestTemplates"));
        }

        function GetTestProgressTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "GetTestProgressTemplates"));
        }

        return ({
            GetCountries: GetCountries,
            GetBreakouts: GetBreakouts,
            AddNewTestCall: AddNewTestCall,
            GetUpdatedTestCalls: GetUpdatedTestCalls,
            GetFilteredTestCalls: GetFilteredTestCalls,
            GetInitiateTestTemplates: GetInitiateTestTemplates,
            GetTestProgressTemplates: GetTestProgressTemplates
        });
    }

    appControllers.service('Qm_CliTester_TestCallAPIService', TestCallAPIService);

})(appControllers);