testController.$inject = ['$scope', 'StrategyAPIService', 'SuspicionAnalysisAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'CaseManagementAPIService'];

function testController($scope, StrategyAPIService, SuspicionAnalysisAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, CaseManagementAPIService) {
  

    loadParameters();
    defineScope();


    function loadParameters() {

        var parameters = VRNavigationService.getParameters($scope);
       
        $scope.subscriberNumber = undefined;

        if (parameters != undefined && parameters != null)
        {
            //if (parameters.validTill == null) {
            //    $scope.endDate = new Date();
            //}
            //else {
            //    $scope.validTill = parameters.validTill;
            //}
        }
            
        
    }

    function defineScope() {

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
    }





}
appControllers.controller('SuspiciousAnalysis_testController', testController);
