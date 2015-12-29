"use strict";

FindRelatedNumbersProcessInputController.$inject = ['$scope', '$http', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'VRValidationService'];

function FindRelatedNumbersProcessInputController($scope, $http, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, VRValidationService) {

    defineScope();

    function defineScope() {

        var yesterday = new Date();
        yesterday.setDate(yesterday.getDate() - 1);

        $scope.fromDate = yesterday;
        $scope.toDate = new Date();

        $scope.validateTimeRange = function () {
            return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
        }

        $scope.createProcessInputObjects = [];

        $scope.createProcessInput.getData = function () {

            $scope.createProcessInputObjects.length = 0;

            $scope.createProcessInputObjects.push({
                InputArguments: {
                    $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.FindRelatedNumbersProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                    FromDate: new Date($scope.fromDate),
                    ToDate: new Date($scope.toDate)
                }
            });

            return $scope.createProcessInputObjects;
        };
    }
}

appControllers.controller('FraudAnalysis_FindRelatedNumbersProcessInputController', FindRelatedNumbersProcessInputController)



