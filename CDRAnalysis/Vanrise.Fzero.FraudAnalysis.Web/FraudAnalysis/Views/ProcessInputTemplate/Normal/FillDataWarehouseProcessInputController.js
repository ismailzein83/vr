"use strict";

FillDataWarehouseProcessInputController.$inject = ['$scope', '$http', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'VRValidationService'];

function FillDataWarehouseProcessInputController($scope, $http, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, VRValidationService) {
    var pageLoaded = false;

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
                    $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.FillDataWarehouseProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                    FromDate: $scope.fromDate,
                    ToDate: $scope.toDate
                }
            });

            return $scope.createProcessInputObjects;
        };
    }

}

appControllers.controller('FraudAnalysis_FillDataWarehouseProcessInputController', FillDataWarehouseProcessInputController)



