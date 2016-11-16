"use strict";

app.directive("vrQueueingExecutionflowdefinitionGrid", ["VR_Queueing_ExecutionFlowDefinitionAPIService", 'VR_Queueing_ExecutionFlowDefinitionService', 'VRNotificationService', 'VRUIUtilsService',
    function (VR_Queueing_ExecutionFlowDefinitionAPIService, VR_Queueing_ExecutionFlowDefinitionService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var grid = new ExecutionFlowGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Queueing/Directives/ExecutionFlowDefinition/Templates/ExecutionFlowDefinitionGrid.html"

        };

        function ExecutionFlowGrid($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {

                $scope.executionFlowDefinitions = [];
                $scope.ongridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());



                    function getDirectiveAPI() {

                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };

                        directiveAPI.onExecutionFlowDefinitionAdded = function (executionFlowDefinitionObject) {
                            gridAPI.itemAdded(executionFlowDefinitionObject);
                        };
                        return directiveAPI;
                    }
                };


                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Queueing_ExecutionFlowDefinitionAPIService.GetFilteredExecutionFlowDefinitions(dataRetrievalInput)
                        .then(function (response) {
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        });
                };
                defineMenuActions();
            }

            function defineMenuActions() {
                $scope.gridMenuActions = [{
                    name: "Edit",
                    clicked: editExecutionFlowDefinition,
                    haspermission: hasEditExecutionFlowDefinitionPermission
                }];
            }

            function hasEditExecutionFlowDefinitionPermission() {
                return VR_Queueing_ExecutionFlowDefinitionAPIService.HasUpdateExecutionFlowDefinition();
            }

            function editExecutionFlowDefinition(executionFlowDefinitionObj) {
                var onExecutionFlowDefinitionUpdated = function (executionFlowDefinitionObj) {
                    gridAPI.itemUpdated(executionFlowDefinitionObj);
                };

                VR_Queueing_ExecutionFlowDefinitionService.editExecutionFlowDefiniton(executionFlowDefinitionObj.Entity.ID, onExecutionFlowDefinitionUpdated);
            }


        }

        return directiveDefinitionObject;

    }]);