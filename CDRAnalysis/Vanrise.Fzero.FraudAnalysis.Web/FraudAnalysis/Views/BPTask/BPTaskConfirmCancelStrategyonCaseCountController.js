BPTaskConfirmCancelStrategyonCaseCountController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'BusinessProcess_BPTaskAPIService'];

function BPTaskConfirmCancelStrategyonCaseCountController($scope, VRNavigationService, VRNotificationService, BusinessProcess_BPTaskAPIService) {
    var bpTaskId;

    defineScope();
    loadParameters();

    function defineScope() {
        $scope.content;
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.accept = function () { executeTask(true); };
        $scope.reject = function () { executeTask(false); };

    }

    function executeTask(accept) {

        var executionInformation = {
            $type: "Vanrise.Fzero.FraudAnalysis.MainExtensions.BPTaskConfirmCancelStrategyonCaseCount, Vanrise.Fzero.FraudAnalysis.MainExtensions",
            Continue: accept
        };

        var input = {
            $type: "Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities",
            TaskId: bpTaskId,
            Notes: '',
            Decision: '',
            ExecutionInformation: executionInformation
        };

        BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function (response) {
            $scope.modalContext.closeModal();
        }).catch(function (error) {
            VRNotificationService.notifyException(error);
        });
    }

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters !== undefined && parameters !== null) {
            bpTaskId = parameters.TaskId;
            if (bpTaskId != undefined && bpTaskId != null)
                BusinessProcess_BPTaskAPIService.GetTask(bpTaskId).then(function (response) {
                    if (response != undefined && response != null && response.TaskData != undefined && response.TaskData != null)
                        $scope.content = response.TaskData.Content;

                }).catch(function (error) {
                    VRNotificationService.notifyException(error);
                    $scope.modalContext.closeModal();
                });


        }
    }
}
appControllers.controller('BusinessProcess_BPTaskConfirmCancelStrategyonCaseCountController', BPTaskConfirmCancelStrategyonCaseCountController);
