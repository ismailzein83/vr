DataSourceExecutionFlowEditorController.$inject = ['$scope', 'DataSourceAPIService', 'VRNotificationService'];

function DataSourceExecutionFlowEditorController($scope, DataSourceAPIService, VRNotificationService) {

    defineScope();
    load();

    function defineScope() {
        $scope.executionFlowDefinitions = [];
        $scope.selectedExecutionFlowDefinition = undefined;
        $scope.executionFlowName = undefined;

        $scope.saveExecutionFlow = function () {
            $scope.issaving = true;

            var execFlowObject = buildExecFlowObjFromScope();

            return DataSourceAPIService.AddExecutionFlow(execFlowObject)
                .then(function (response) {

                    if (VRNotificationService.notifyOnItemAdded('Execution Flow', response)) {
                        if ($scope.onExecutionFlowAdded != undefined)
                            $scope.onExecutionFlowAdded();

                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                })
                .finally(function () {
                    $scope.issaving = false;
                });
        }

        $scope.close = function () {
            $scope.modalContext.closeModal();
        }
    }

    function load() {
        DataSourceAPIService.GetExecutionFlowDefinitions()
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