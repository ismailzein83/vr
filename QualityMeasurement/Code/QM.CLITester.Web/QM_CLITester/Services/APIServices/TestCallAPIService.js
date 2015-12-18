(function (appControllers) {

    "use strict";
    TestCallAPIService.$inject = ['BaseAPIService', 'UtilsService', 'QM_CLITester_ModuleConfig'];

    function TestCallAPIService(BaseAPIService, UtilsService, QM_CLITester_ModuleConfig) {

        function AddNewTestCall(testCallObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "AddNewTestCall"), testCallObject);
        }

        function ReTestCall(testCallObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "ReTestCall"), testCallObject);
        }

        function GetUpdated(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "GetUpdated"), input);
        }

        function GetBeforeId(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, "TestCall", "GetBeforeId"), input);
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
            AddNewTestCall: AddNewTestCall,
            ReTestCall: ReTestCall,
            GetFilteredTestCalls: GetFilteredTestCalls,
            GetInitiateTestTemplates: GetInitiateTestTemplates,
            GetTestProgressTemplates: GetTestProgressTemplates,
            GetUpdated: GetUpdated,
            GetBeforeId: GetBeforeId
        });
    }

    appControllers.service('Qm_CliTester_TestCallAPIService', TestCallAPIService);

})(appControllers);