"use strict";

FillDataWarehouseProcessInput_Scheduled.$inject = ['$scope', '$http', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function FillDataWarehouseProcessInput_Scheduled($scope, $http, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    defineScope();

    function defineScope() {

        $scope.processInputArguments = [];
       
        $scope.schedulerTaskAction.rawExpressions.getData = function () {
            return { "ScheduleTime": "ScheduleTime" };
        };

        $scope.schedulerTaskAction.processInputArguments.getData = function () {
            return {
                $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.FillDataWarehouseProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments"               
            };
        };
    }


}

appControllers.controller('FraudAnalysis_FillDataWarehouseProcessInput_Scheduled', FillDataWarehouseProcessInput_Scheduled)



