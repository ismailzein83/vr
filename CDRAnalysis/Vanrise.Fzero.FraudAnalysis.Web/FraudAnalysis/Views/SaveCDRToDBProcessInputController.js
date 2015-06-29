var SaveCDRToDBProcessInputController = function ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {
    defineScope();
   
    function defineScope() {
        $scope.createProcessInput.getData = function () {
            console.log({
                $type: "Vanrise.Fzero.CDRImport.BP.Arguments.SaveCDRToDBProcessInput, Vanrise.Fzero.CDRImport.BP.Arguments"
                //StrategyIds: $scope.selectedStrategyIds,
                //FromDate: $scope.fromDate != undefined ? $scope.fromDate : '',
                //ToDate: $scope.toDate != undefined ? $scope.toDate : '',
                //PeriodId: $scope.selectedPeriod.Id
            })
            return {
                $type: "Vanrise.Fzero.CDRImport.BP.Arguments.SaveCDRToDBProcessInput, Vanrise.Fzero.CDRImport.BP.Arguments"
                //StrategyIds: $scope.selectedStrategyIds,
                //FromDate: $scope.fromDate != undefined ? $scope.fromDate : '',
                //ToDate: $scope.toDate != undefined ? $scope.toDate : '',
                //PeriodId: $scope.selectedPeriod.Id
            };
        };
    }

}

SaveCDRToDBProcessInputController.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];
appControllers.controller('FraudAnalysis_SaveCDRToDBProcessInputController', SaveCDRToDBProcessInputController)



