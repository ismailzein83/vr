WFActionTemplateController.$inject = ['$scope'];

function WFActionTemplateController($scope) {

    defineScope();
    load();

    function defineScope() {

        $scope.schedulerTaskAction.getData = function () {
            return {
                $type: "Vanrise.Runtime.Entities.WFSchedulerTaskAction, Vanrise.Runtime.Entities",
                WorkflowName: $scope.workflowName,
            };
        };

        loadForm();
    }

    function loadForm() {

        if ($scope.schedulerTaskAction.data == undefined)
            return;
        var data = $scope.schedulerTaskAction.data;
        if (data != null) {
            $scope.workflowName = data.WorkflowName;
        }
        else {
            $scope.workflowName = '';
        }
    }

    function load() {
    }

}
appControllers.controller('Runtime_WFActionTemplateController', WFActionTemplateController);
