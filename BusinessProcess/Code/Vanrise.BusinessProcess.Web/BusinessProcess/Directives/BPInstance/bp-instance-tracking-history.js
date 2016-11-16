"use strict";
app.directive("businessprocessBpInstanceTrackingHistory", ["BusinessProcess_BPInstanceTrackingAPIService", "BusinessProcess_BPInstanceService", "VRNotificationService","UtilsService",
function (BusinessProcess_BPInstanceTrackingAPIService, BusinessProcess_BPInstanceService, VRNotificationService, UtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var bpInstanceGrid = new BPInstanceGrid($scope, ctrl, $attrs);
            bpInstanceGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/BusinessProcess/Directives/BPInstance/Templates/BPInstanceTrackingHistoryGridTemplate.html"

    };

    function BPInstanceGrid($scope, ctrl) {

        var gridAPI;
        var filter = {};

        var bpInstanceId;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.bpInstanceTracking = [];
            $scope.selectedTrackingSeverity = [];

            loadFilters();
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        bpInstanceId = query.BPInstanceID;
                        getFilterObject();
                        return gridAPI.retrieveData(filter);
                    };

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return BusinessProcess_BPInstanceTrackingAPIService.GetFilteredBPInstanceTracking(dataRetrievalInput)
                    .then(function (response) {

                        if (response != undefined && response.Data != undefined && response.Data.length > 0) {
                            filter.FromTrackingId = response.Data[response.Data.length - 1].Entity.Id;
                        }
                        
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

            $scope.searchClicked = function () {
                getFilterObject();
                return gridAPI.retrieveData(filter);
            };

        }

        $scope.getSeverityColor = function (dataItem, colDef) {
            return UtilsService.getLogEntryTypeColor(dataItem.Entity.Severity);
        };

        function loadFilters() {
            $scope.trackingSeverity = UtilsService.getLogEntryType();
        }

        function getFilterObject() {
            filter = {
                ProcessInstanceId: bpInstanceId,
                FromTrackingId: 0,
                Message: $scope.message,
                Severities: UtilsService.getPropValuesFromArray($scope.selectedTrackingSeverity, "value")
            };
        }
    }
    return directiveDefinitionObject;

}]);