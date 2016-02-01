"use strict";

app.directive("vrQueueinstanceGrid", ["VR_Queueing_QueueInstanceAPIService", "VR_Queueing_ExecutionFlowService", 'VRNotificationService',
    function (VR_Queueing_QueueInstanceAPIService, VR_Queueing_ExecutionFlowService, VRNotificationService) {

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
            templateUrl: "/Client/Modules/Queueing/Directives/QueueInstance/Templates/QueueInstanceGrid.html"

        };

        function ExecutionFlowGrid($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {

                $scope.executionFlows = [];
                $scope.ongridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {

                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {

                            return gridAPI.retrieveData(query);
                        }

                        return directiveAPI;
                    }
                };


                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Queueing_QueueInstanceAPIService.GetFilteredQueueInstances(dataRetrievalInput)
                        .then(function (response) {
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        });
                };
            }
        }

        return directiveDefinitionObject;

    }]);