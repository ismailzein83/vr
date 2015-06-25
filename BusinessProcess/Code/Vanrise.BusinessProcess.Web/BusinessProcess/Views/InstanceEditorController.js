InstanceEditorController.$inject = ['$scope', 'BusinessProcessAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function InstanceEditorController($scope, BusinessProcessAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    defineScope();
    loadParameters();

    function defineScope() {
        $scope.subViewExecuteStrategyProcessInput = {};
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.startInstance = function () {

            $scope.issaving = true;
            var createProcessInput = buildInstanceObjFromScope();
            console.log('createProcessInput')
            console.log(createProcessInput)
            BusinessProcessAPIService.CreateNewProcess(createProcessInput).then(function (response) {
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error);
            });


        };
    }


    function buildInstanceObjFromScope() {
        var inputArguments = $scope.subViewExecuteStrategyProcessInput.getData();
        var createProcessInputObject = {
            InputArguments: inputArguments
        };

        return createProcessInputObject;
    }



    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.BPDefinitionObj = undefined;

        if (parameters != undefined && parameters != null)
            $scope.BPDefinitionObj = parameters.BPDefinitionObj;

      
    }

}
appControllers.controller('FraudAnalysis_InstanceEditorController', InstanceEditorController);
