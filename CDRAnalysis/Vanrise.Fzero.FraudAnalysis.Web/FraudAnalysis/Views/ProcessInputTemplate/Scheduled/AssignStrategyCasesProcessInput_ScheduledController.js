﻿"use strict";

AssignStrategyCasesProcessInput_Scheduled.$inject = ['$scope', '$http', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function AssignStrategyCasesProcessInput_Scheduled($scope, $http, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
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

appControllers.controller('FraudAnalysis_FraudAnalysis_AssignStrategyCasesProcessInput_Scheduled', AssignStrategyCasesProcessInput_Scheduled)



