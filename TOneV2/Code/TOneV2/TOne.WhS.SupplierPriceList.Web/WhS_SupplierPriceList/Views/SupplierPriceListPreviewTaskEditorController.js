SupplierPriceListPreviewTaskEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'BusinessProcess_BPTaskAPIService'];

function SupplierPriceListPreviewTaskEditorController($scope, VRNavigationService, VRNotificationService, BusinessProcess_BPTaskAPIService) {
    var bpTaskId;

    defineScope();
    loadParameters();

    function defineScope() {

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.executeTask = function () {

            var executionInformation = {
                $type: "Vanrise.Fzero.FraudAnalysis.MainExtensions.TestBPTaskExecutionInformation, Vanrise.Fzero.FraudAnalysis.MainExtensions",
                Continue: true
            };

            var input = {
                $type: "Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities",
                TaskId: bpTaskId,
                Notes: $scope.notes,
                Decision: 'Dummy Decision',
                ExecutionInformation: executionInformation
            };

            BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function (response) {
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error);
            });
        };
    }

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters !== undefined && parameters !== null) {
            bpTaskId = parameters.TaskId;
        }
    }
}
appControllers.controller('WhS_SupPL_SupplierPriceListPreviewTaskEditorController', SupplierPriceListPreviewTaskEditorController);
