WFActionTemplateController.$inject = ['$scope', 'BusinessProcessAPIService', 'UtilsService'];

function WFActionTemplateController($scope, BusinessProcessAPIService, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.bpDefinitions = [];

        $scope.schedulerTaskAction.baseProcessInputArgument = {};

        $scope.schedulerTaskAction.getData = function () {
            return {
                $type: "Vanrise.BusinessProcess.Extensions.WFSchedulerTaskAction, Vanrise.BusinessProcess.Extensions",
                BPDefinitionID: $scope.selectedBPDefintion.BPDefinitionID,
                BaseProcessInputArgument: $scope.schedulerTaskAction.baseProcessInputArgument.getData()
            };
        };
    }

    function load() {

        UtilsService.waitMultipleAsyncOperations([loadDefinitions]).finally(function () {

            loadForm();

        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }

    function loadForm() {

        if ($scope.schedulerTaskAction.data == undefined)
            return;
        var data = $scope.schedulerTaskAction.data;
        if (data != null) {
            $scope.selectedBPDefintion = UtilsService.getItemByVal($scope.bpDefinitions, data.BPDefinitionID, "BPDefinitionID");
            $scope.schedulerTaskAction.baseProcessInputArgument.data = data.BaseProcessInputArgument;
        }
        else {
            $scope.selectedBPDefintion = undefined;
            $scope.schedulerTaskAction.baseProcessInputArgument.data = undefined;
        }
    }

    function loadDefinitions()
    {
        return BusinessProcessAPIService.GetDefinitions().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.bpDefinitions.push(item);
            });
        });
    }

}
appControllers.controller('Runtime_WFActionTemplateController', WFActionTemplateController);
