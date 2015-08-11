DataSourceExecutionFlowEditorController.$inject = ['$scope'];

function DataSourceExecutionFlowEditorController($scope) {

    defineScope();
    load();

    function defineScope() {
        $scope.executionFlowDefinitions = [];
        $scope.selectedExecutionFlowDefinition = undefined;
        $scope.executionFlowName = undefined;

        $scope.saveExecutionFlow = function () {

        }

        $scope.close = function () {
            $scope.modalContext.closeModal();
        }
    }

    function load() {

    }
}

appControllers.controller('Integration_DataSourceExecutionFlowEditorController', DataSourceExecutionFlowEditorController);