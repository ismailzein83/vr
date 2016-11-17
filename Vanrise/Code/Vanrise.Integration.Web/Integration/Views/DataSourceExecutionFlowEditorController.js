DataSourceExecutionFlowEditorController.$inject = ['$scope', 'VR_Integration_DataSourceAPIService', 'VRNotificationService'];

function DataSourceExecutionFlowEditorController($scope, VR_Integration_DataSourceAPIService, VRNotificationService) {

    defineScope();
    load();

    function defineScope() {
        $scope.executionFlowDefinitions = [];

        $scope.saveExecutionFlow = function () {
            var execFlowObject = buildExecFlowObjFromScope();

            return VR_Integration_DataSourceAPIService.AddExecutionFlow(execFlowObject)
                .then(function (response) {

                    if (VRNotificationService.notifyOnItemAdded('Execution Flow', response)) {
                        if ($scope.onExecutionFlowAdded != undefined)
                            $scope.onExecutionFlowAdded();

                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        };

        $scope.close = function () {
            $scope.modalContext.closeModal();
        };
    }

    function load() {
        VR_Integration_DataSourceAPIService.GetExecutionFlowDefinitions()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.executionFlowDefinitions.push(item);
                });
            })
    }

    function buildExecFlowObjFromScope() {
        return {
            DefinitionId: $scope.selectedExecutionFlowDefinition.ID,
            ExecutionFlowId: undefined,
            Name: $scope.executionFlowName,
            Tree: undefined
        };
    }
}

appControllers.controller('Integration_DataSourceExecutionFlowEditorController', DataSourceExecutionFlowEditorController);