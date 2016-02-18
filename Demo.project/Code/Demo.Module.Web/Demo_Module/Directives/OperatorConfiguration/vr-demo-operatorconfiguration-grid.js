"use strict";

app.directive("vrDemoOperatorconfigurationGrid", ["UtilsService", "VRNotificationService", "Demo_OperatorConfigurationAPIService", "Demo_OperatorConfigurationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, Demo_OperatorConfigurationAPIService, Demo_OperatorConfigurationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var operatorConfigurationGrid = new OperatorConfigurationGrid($scope, ctrl, $attrs);
            operatorConfigurationGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_Module/Directives/OperatorConfiguration/Templates/OperatorConfigurationGridTemplate.html"

    };

    function OperatorConfigurationGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.operatorConfigurations = [];
            defineMenuActions();
            $scope.onGridReady = function (api) {
                gridAPI = api;


                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());


                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onOperatorConfigurationAdded = function (operatorConfigurationObject) {
                        gridAPI.itemAdded(operatorConfigurationObject);
                    }
                    return directiveAPI;
                }


            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Demo_OperatorConfigurationAPIService.GetFilteredOperatorConfigurations(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editOperatorConfiguration,
            }
            ];
        }

        function editOperatorConfiguration(operatorConfigurationObj) {
            var onOperatorConfigurationUpdated = function (operatorConfiguration) {
                gridAPI.itemUpdated(operatorConfiguration);

            }
            Demo_OperatorConfigurationService.editOperatorConfiguration(operatorConfigurationObj, onOperatorConfigurationUpdated);
        }

    }

    return directiveDefinitionObject;

}]);