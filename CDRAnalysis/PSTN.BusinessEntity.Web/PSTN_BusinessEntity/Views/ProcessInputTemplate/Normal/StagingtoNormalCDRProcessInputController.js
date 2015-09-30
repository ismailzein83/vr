"use strict";

StagingtoNormalCDRProcessInputController.$inject = ['$scope', '$http', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function StagingtoNormalCDRProcessInputController($scope, $http, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {

    defineScope();

    function defineScope() {

        var yesterday = new Date();
        yesterday.setDate(yesterday.getDate() - 1);

        $scope.fromDate = yesterday;
        $scope.toDate = new Date();

        $scope.createProcessInputObjects = [];

        $scope.createProcessInput.getData = function () {

            $scope.createProcessInputObjects.length = 0;

            $scope.createProcessInputObjects.push({
                InputArguments: {
                    $type: "Vanrise.Fzero.CDRImport.BP.Arguments.StagingtoNormalCDRProcessInput, Vanrise.Fzero.CDRImport.BP.Arguments",
                    FromDate: new Date($scope.fromDate),
                    ToDate: new Date($scope.toDate)
                }
            });

            return $scope.createProcessInputObjects;
        };
    }
}

appControllers.controller('FraudAnalysis_StagingtoNormalCDRProcessInputController', StagingtoNormalCDRProcessInputController)



