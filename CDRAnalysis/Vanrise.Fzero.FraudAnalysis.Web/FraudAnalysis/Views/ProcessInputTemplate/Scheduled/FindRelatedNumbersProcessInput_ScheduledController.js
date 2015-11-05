"use strict";

FindRelatedNumbersProcessInput_ScheduledController.$inject = ['$scope', '$http', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function FindRelatedNumbersProcessInput_ScheduledController($scope, $http, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    defineScope();

    function defineScope() {

        $scope.processInputArguments = [];

        $scope.schedulerTaskAction.rawExpressions.getData = function () {
            return { "ScheduleTime": "ScheduleTime" };
        };



        $scope.schedulerTaskAction.processInputArguments.getData = function () {
            return {
                $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.FindRelatedNumbersProcess, Vanrise.Fzero.FraudAnalysis.BP.Arguments"
            };
        };

    };
}

appControllers.controller('FraudAnalysis_FindRelatedNumbersProcessInput_ScheduledController', FindRelatedNumbersProcessInput_ScheduledController)



