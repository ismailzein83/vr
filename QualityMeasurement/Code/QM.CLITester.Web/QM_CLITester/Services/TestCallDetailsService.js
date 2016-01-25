
(function (appControllers) {

    'use strict';

    TestCallDetailsService.$inject = ['Qm_CliTester_TestCallService', 'VRModalService', 'VRNotificationService'];

    function TestCallDetailsService(Qm_CliTester_TestCallService, VRModalService, VRNotificationService) {
        return {
            registerDrillDownToTestCallDetails: registerDrillDownToTestCallDetails
        };
        function registerDrillDownToTestCallDetails() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Test Call Details";
            drillDownDefinition.directive = "vr-qm-clitester-testcalldetails";

            drillDownDefinition.loadDirective = function (directiveAPI, testCallDetailsItem) {
                return directiveAPI.load(testCallDetailsItem);
            };

            Qm_CliTester_TestCallService.addDrillDownDefinition(drillDownDefinition);
        }
    }
    appControllers.service('Qm_CliTester_TestCallDetailsService', TestCallDetailsService);

})(appControllers);