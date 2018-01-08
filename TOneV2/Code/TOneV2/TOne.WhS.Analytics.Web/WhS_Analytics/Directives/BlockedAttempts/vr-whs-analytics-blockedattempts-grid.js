"use strict";

app.directive("vrWhsAnalyticsBlockedattemptsGrid", ["UtilsService", "VRNotificationService", "WhS_Analytics_BlockedAttemptsAPIService", "WhS_BE_SwitchReleaseCauseService",
function (UtilsService, VRNotificationService, WhS_Analytics_BlockedAttemptsAPIService, WhS_BE_SwitchReleaseCauseService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
            shownumber: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new Grid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Analytics/Directives/BlockedAttempts/Templates/BlockedAttemptsGridTemplate.html"

    };

    function Grid($scope, ctrl, $attrs) {

        var gridAPI;
        var queryObject;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.blockedAttempts = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        queryObject = query;
                        return gridAPI.retrieveData(query);
                    };

                    return directiveAPI;
                }
            };
            $scope.onClickReleaseCode = function (dataItem) {
                var switchIds;
                if (queryObject != undefined && queryObject.Filter != undefined && queryObject.Filter.SwitchIds != undefined)
                    switchIds = queryObject.Filter.SwitchIds;
                WhS_BE_SwitchReleaseCauseService.openReleaseCodeDescriptions(dataItem.Entity.ReleaseCode, switchIds);
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                ctrl.showGrid = true;
                return WhS_Analytics_BlockedAttemptsAPIService.GetBlockedAttemptsData(dataRetrievalInput)
                                 .then(function (response) {
                                     $scope.shownumber = ctrl.shownumber;
                                     onResponseReady(response);
                                 })
                                 .catch(function (error) {
                                     VRNotificationService.notifyException(error, $scope);
                                 });
            };
        }
    }
    return directiveDefinitionObject;

}]);