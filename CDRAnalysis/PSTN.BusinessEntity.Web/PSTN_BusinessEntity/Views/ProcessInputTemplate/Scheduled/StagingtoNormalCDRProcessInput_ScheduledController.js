"use strict";

StagingtoNormalCDRProcessInput_Scheduled.$inject = ['$scope', '$http', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function StagingtoNormalCDRProcessInput_Scheduled($scope, $http, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    defineScope();

    function defineScope() {

        $scope.processInputArguments = [];



        $scope.schedulerTaskAction.rawExpressions.getData = function () {
            return { "ScheduleTime": "ScheduleTime" };
        };



        $scope.schedulerTaskAction.processInputArguments.getData = function () {
            return {
                $type: "Vanrise.Fzero.CDRImport.BP.Arguments.StagingtoNormalCDRProcess, Vanrise.Fzero.CDRImport.BP.Arguments"
            };
        };

    };

   

}


appControllers.controller('FraudAnalysis_StagingtoNormalCDRProcessInput_Scheduled', StagingtoNormalCDRProcessInput_Scheduled)



