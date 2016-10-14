(function (appControllers) {

    "use strict";
    TestCallAPIService.$inject = ['BaseAPIService', 'UtilsService', 'QM_CLITester_ModuleConfig', 'SecurityService'];

    function TestCallAPIService(BaseAPIService, UtilsService, QM_CLITester_ModuleConfig, SecurityService) {

        var controllerName = 'TestCall';

        function AddNewTestCall(testCallObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, controllerName, "AddNewTestCall"), testCallObject);
        }

        function GetUpdated(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, controllerName, "GetUpdated"), input);
        }

        function GetBeforeId(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, controllerName, "GetBeforeId"), input);
        }

        function GetTotalCallsByUserId() {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, controllerName, "GetTotalCallsByUserId"));
        }

        function GetFilteredTestCalls(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, controllerName, "GetFilteredTestCalls"), input);
        }

        function GetTestTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, controllerName, "GetTestTemplates"));
        }

        function HasAddTestCallPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(QM_CLITester_ModuleConfig.moduleName, controllerName, ['AddNewTestCall']));

        }

        function SendMail(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_CLITester_ModuleConfig.moduleName, controllerName, "SendMail"), input);
        }

        return ({
            AddNewTestCall: AddNewTestCall,
            HasAddTestCallPermission: HasAddTestCallPermission,
            GetFilteredTestCalls: GetFilteredTestCalls,
            GetTotalCallsByUserId: GetTotalCallsByUserId,
            GetTestTemplates: GetTestTemplates,
            GetUpdated: GetUpdated,
            GetBeforeId: GetBeforeId,
            SendMail: SendMail
        });
    }

    appControllers.service('Qm_CliTester_TestCallAPIService', TestCallAPIService);

})(appControllers);