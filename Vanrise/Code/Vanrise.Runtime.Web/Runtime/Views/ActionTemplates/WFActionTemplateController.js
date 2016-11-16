WFActionTemplateController.$inject = ['$scope', 'BusinessProcess_BPDefinitionAPIService', 'UtilsService'];

function WFActionTemplateController($scope, BusinessProcess_BPDefinitionAPIService, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.bpDefinitions = [];

        $scope.schedulerTaskAction.processInputArguments = {};
        $scope.schedulerTaskAction.rawExpressions = {};

        $scope.schedulerTaskAction.processInputArguments.data = undefined;
        $scope.schedulerTaskAction.rawExpressions.data = undefined;
        $scope.selectedBPDefintion = undefined;

        $scope.schedulerTaskAction.getData = function () {
            return {
                $type: "Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.WFTaskActionArgument, Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments",
                RawExpressions: $scope.schedulerTaskAction.rawExpressions.getData(),
                BPDefinitionID: $scope.selectedBPDefintion.BPDefinitionID,
                ProcessInputArguments: $scope.schedulerTaskAction.processInputArguments.getData()
            };
        };

        $scope.schedulerTaskAction.loadTemplateData = function () {
            loadForm();
        };
    }

    var isFormLoaded;
    function loadForm() {

        if ($scope.schedulerTaskAction.additionalParameter != undefined)
        {
            $scope.selectedBPDefintion = UtilsService.getItemByVal($scope.bpDefinitions, $scope.schedulerTaskAction.additionalParameter.bpDefinitionID, "BPDefinitionID");
        }

        if ($scope.schedulerTaskAction.data == undefined || isFormLoaded)
            return;

        var data = $scope.schedulerTaskAction.data;
        if (data != null) {
            $scope.selectedBPDefintion = UtilsService.getItemByVal($scope.bpDefinitions, data.BPDefinitionID, "BPDefinitionID");

            $scope.schedulerTaskAction.rawExpressions.data = data.RawExpressions;
            $scope.schedulerTaskAction.processInputArguments.data = data.ProcessInputArguments;

            if ($scope.schedulerTaskAction.processInputArguments.loadTemplateData != undefined)
                $scope.schedulerTaskAction.processInputArguments.loadTemplateData();
        }

        isFormLoaded = true;
    }

    function load() {
        loadDefinitions().then(function () {

            loadForm();

        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }

    function loadDefinitions()
    {
        return BusinessProcess_BPDefinitionAPIService.GetBPDefinitions().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.bpDefinitions.push(item);
            });
        });
    }

}
appControllers.controller('Runtime_WFActionTemplateController', WFActionTemplateController);
