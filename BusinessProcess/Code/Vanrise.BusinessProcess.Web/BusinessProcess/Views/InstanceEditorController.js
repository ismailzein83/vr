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
            var instanceObject = buildInstanceObjFromScope();
            console.log(instanceObject)
            BusinessProcessAPIService.StartInstance($scope.BPDefinitionObj.Title).then(function (response) {
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error);
            });


        };
    }


    function buildInstanceObjFromScope() {
        return $scope.subViewExecuteStrategyProcessInput.getData();
    }



    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.BPDefinitionObj = undefined;

        if (parameters != undefined && parameters != null)
            $scope.BPDefinitionObj = parameters.BPDefinitionObj;

      
    }

}
appControllers.controller('FraudAnalysis_InstanceEditorController', InstanceEditorController);
