"use strict";

FindRelatedNumbersProcessInput_Scheduled.$inject = ['$scope', '$http', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

FindRelatedNumbersProcessInput_Scheduled($scope, $http, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
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


appControllers.controller('FraudAnalysis_FindRelatedNumbersProcessInput_Scheduled', FindRelatedNumbersProcessInput_Scheduled)



