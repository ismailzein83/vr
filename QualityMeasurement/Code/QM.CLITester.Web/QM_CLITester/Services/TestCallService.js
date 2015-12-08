
app.service('Qm_CliTester_TestCallService', ['VRModalService',
    function (VRModalService) {
        var drillDownDefinitions = [];
        return ({
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition

        });

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

    }]);
