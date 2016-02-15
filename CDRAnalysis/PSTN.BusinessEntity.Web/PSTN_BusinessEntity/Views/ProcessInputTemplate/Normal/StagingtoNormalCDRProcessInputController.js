"use strict";

StagingtoCDRProcessInputController.$inject = ['$scope', '$http', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function StagingtoCDRProcessInputController($scope, $http, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {

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
                    $type: "Vanrise.Fzero.CDRImport.BP.Arguments.StagingtoCDRProcessInput, Vanrise.Fzero.CDRImport.BP.Arguments",
                    FromDate: new Date($scope.fromDate),
                    ToDate: new Date($scope.toDate)
                }
            });

            return $scope.createProcessInputObjects;
        };
    }
}

appControllers.controller('FraudAnalysis_StagingtoCDRProcessInputController', StagingtoCDRProcessInputController)



