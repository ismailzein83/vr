"use strict";

AssignStrategyCasesProcessInputController.$inject = ['$scope', '$http', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function AssignStrategyCasesProcessInputController ($scope, $http, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {
    var pageLoaded = false;

    defineScope();

    function defineScope() {

        $scope.createProcessInputObjects = [];

        $scope.createProcessInput.getData = function () {

            $scope.createProcessInputObjects.length = 0;

            $scope.createProcessInputObjects.push({
                InputArguments: {
                    $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.AssignStrategyCasesProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments"
                }
            });

            return $scope.createProcessInputObjects;

        };


    }


}

appControllers.controller('FraudAnalysis_AssignStrategyCasesProcessInputController', AssignStrategyCasesProcessInputController)



