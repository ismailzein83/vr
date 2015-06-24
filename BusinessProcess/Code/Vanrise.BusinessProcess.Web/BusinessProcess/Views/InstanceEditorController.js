InstanceEditorController.$inject = ['$scope', 'BusinessProcessAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function InstanceEditorController($scope, BusinessProcessAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {

    loadParameters();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.BPDefinitionObj = undefined;

        if (parameters != undefined && parameters != null)
            $scope.BPDefinitionObj = parameters.BPDefinitionObj;

      
    }

}
appControllers.controller('FraudAnalysis_InstanceEditorController', InstanceEditorController);
