﻿var CDRImportProcessInputController = function ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {
    defineScope();
   
    function defineScope() {
        $scope.createProcessInput.getData = function () {
            return {
                $type: "Vanrise.Fzero.CDRImport.BP.Arguments.CDRImportProcessInput, Vanrise.Fzero.CDRImport.BP.Arguments"
                //StrategyIds: $scope.selectedStrategyIds,
                //FromDate: $scope.fromDate != undefined ? $scope.fromDate : '',
                //ToDate: $scope.toDate != undefined ? $scope.toDate : '',
                //PeriodId: $scope.selectedPeriod.Id
            };
        };
    }

}

CDRImportProcessInputController.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];
appControllers.controller('FraudAnalysis_CDRImportProcessInputController', CDRImportProcessInputController)



