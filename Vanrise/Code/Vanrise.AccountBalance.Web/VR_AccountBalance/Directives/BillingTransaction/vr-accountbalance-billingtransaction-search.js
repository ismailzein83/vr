"use strict";

app.directive("vrAccountbalanceBillingtransactionSearch", [ 'VRNotificationService', 'UtilsService', 'VRUIUtilsService','VRValidationService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var logSearch = new LogSearch($scope, ctrl, $attrs);
            logSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_AccountBalance/Directives/BillingTransaction/Templates/BillingTranactionSearch.html"

    };

    function LogSearch($scope, ctrl, $attrs) {


        var gridAPI;
        var accountTypeId;
        var accountsIds;
        this.initializeController = initializeController;
        function initializeController() {
            
            defineScope();

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(getDirectiveAPI());
            function getDirectiveAPI() {

                var directiveAPI = {};
                directiveAPI.loadDirective = function (payload) {
                    if (payload != undefined) {
                        accountTypeId = payload.AccountTypeId;
                        accountsIds = payload.AccountsIds;
                    }
                };
                return directiveAPI;
            }
        }

        function defineScope() 
        {
            var fromTime = new Date();
            fromTime.setHours(0, 0, 0);
            $scope.fromTime = fromTime;
            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };
            $scope.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.fromTime, $scope.toTime);
            };
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(getFilterObject());
            };
        }
        function getFilterObject() {
            var filter = {
                AccountTypeId: accountTypeId,
                AccountsIds: accountsIds,
                FromTime: $scope.fromTime,
                ToTime: $scope.toTime
            };
           return filter;
        }
    }

    return directiveDefinitionObject;

}]);
