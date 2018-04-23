"use strict";
app.directive("vrRuntimeRuntimenodeconfigurationGrid", ["UtilsService", "VRNotificationService", "VRRuntime_RuntimeNodeConfigurationAPIService", "VRRuntime_RuntimeNodeConfigurationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRRuntime_RuntimeNodeConfigurationAPIService, VRRuntime_RuntimeNodeConfigurationService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var nodeConfigGrid = new RuntimeNodeConfigurationGrid($scope, ctrl, $attrs);
            nodeConfigGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Runtime/Directives/RuntimeNodeConfiguration/Templates/RuntimeNodeConfigurationTemplate.html"
    };

    function RuntimeNodeConfigurationGrid($scope, ctrl, $attrs) {
        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.runttimeNodesConfigurations = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
              

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onRuntimeNodeConfigurationAdded = function (nodeConfigObject) {     
                        gridAPI.itemAdded(nodeConfigObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                return VRRuntime_RuntimeNodeConfigurationAPIService.GetFilteredRuntimeNodesConfigurations(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editRuntimeNodeConfiguration,
            }];
        }

        function editRuntimeNodeConfiguration(nodeConfigObj) {
            var onRuntimeNodeConfigurationUpdated = function (nodeConfigObj) {
                gridAPI.itemUpdated(nodeConfigObj);
            };

            VRRuntime_RuntimeNodeConfigurationService.editRuntimeNodeConfiguration(nodeConfigObj.RuntimeNodeConfigurationId, onRuntimeNodeConfigurationUpdated);
        }
    }
    return directiveDefinitionObject;

}]);