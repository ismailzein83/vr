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


            //$scope.issaving = true;
            //var routeRuleObject = buildRouteRuleObjFromScope();
            //return RoutingRulesAPIService.InsertRouteRule(routeRuleObject).then(function (response) {
            //    if (VRNotificationService.notifyOnItemAdded("RouteRule", response)) {
            //        if ($scope.onRouteRuleAdded != undefined)
            //            $scope.onRouteRuleAdded(response.InsertedObject);
            //        $scope.modalContext.closeModal();
            //    }
            //}).catch(function (error) {
            //    VRNotificationService.notifyException(error);
            //});


        };
    }

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.BPDefinitionObj = undefined;

        if (parameters != undefined && parameters != null)
            $scope.BPDefinitionObj = parameters.BPDefinitionObj;

      
    }

}
appControllers.controller('FraudAnalysis_InstanceEditorController', InstanceEditorController);
