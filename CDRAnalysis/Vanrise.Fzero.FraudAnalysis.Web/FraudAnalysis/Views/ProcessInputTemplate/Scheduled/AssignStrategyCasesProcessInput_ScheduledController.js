﻿var FraudAnalysis_AssignStrategyCasesProcessInput_Scheduled = function ($scope, $http, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    var pageLoaded = false;
    defineScope();
    load();

    function defineScope() {

        $scope.processInputArguments = [];



        $scope.schedulerTaskAction.rawExpressions.getData = function () {
            return { "ScheduleTime": "ScheduleTime" };
        };



        $scope.schedulerTaskAction.processInputArguments.getData = function () {
            return {
                $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.AssignStrategyExecutionCasesProcess, Vanrise.Fzero.FraudAnalysis.BP.Arguments"
            };
        };

    };

    function load() {

    }

}

FraudAnalysis_AssignStrategyCasesProcessInput_Scheduled.$inject = ['$scope', '$http', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];
appControllers.controller('FraudAnalysis_FraudAnalysis_AssignStrategyCasesProcessInput_Scheduled', FraudAnalysis_AssignStrategyCasesProcessInput_Scheduled)



