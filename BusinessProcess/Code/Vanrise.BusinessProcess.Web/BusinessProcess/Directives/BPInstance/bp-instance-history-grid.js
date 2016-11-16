"use strict";

app.directive("businessprocessBpInstanceHistoryGrid", ["BusinessProcess_BPInstanceAPIService", "BusinessProcess_BPInstanceService","VRNotificationService",
function (BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, VRNotificationService) {

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
        templateUrl: "/Client/Modules/BusinessProcess/Directives/BPInstance/Templates/BPInstanceHistoryGridTemplate.html"

    };

    function BPInstanceGrid($scope, ctrl) {

        var gridAPI;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.bpInstances = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return BusinessProcess_BPInstanceAPIService.GetFilteredBPInstances(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

        }

        $scope.getStatusColor = function (dataItem) {
            return BusinessProcess_BPInstanceService.getStatusColor(dataItem.Entity.Status);
        };

        $scope.processInstanceClicked = function (dataItem) {
            BusinessProcess_BPInstanceService.openProcessTracking(dataItem.Entity.ProcessInstanceID);
        }
    }
    return directiveDefinitionObject;

}]);