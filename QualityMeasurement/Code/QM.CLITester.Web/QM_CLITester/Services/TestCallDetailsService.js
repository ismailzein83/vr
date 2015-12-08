
app.service('Qm_CliTester_TestCallDetailsService', ['Qm_CliTester_TestCallService',
    function (Qm_CliTester_TestCallService) {

       
        return ({
            registerDrillDownToTestCallDetails: registerDrillDownToTestCallDetails
        });
        function registerDrillDownToTestCallDetails() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Test Call Details";
            drillDownDefinition.directive = "vr-qm-clitester-testcalldetails";

            drillDownDefinition.loadDirective = function (directiveAPI, testCallDetailsItem) {
                return directiveAPI.load(testCallDetailsItem);
            };

            Qm_CliTester_TestCallService.addDrillDownDefinition(drillDownDefinition);
        }

    }]);
