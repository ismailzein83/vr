"use strict";

app.directive("vrQueueingQueueitemheader2Grid", ["VR_Queueing_QueueItemHeaderAPIService",  'VRNotificationService', 'VRUIUtilsService',
    function (VR_Queueing_QueueItemHeaderAPIService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var grid = new QueueItemHeaderGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Queueing/Directives/QueueItemHeader2/Templates/QueueItemHeader2Grid.html"

        };

        function QueueItemHeaderGrid($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {

                $scope.queueItemHeader = [];
                $scope.ongridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());



                    function getDirectiveAPI() {

                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };

                        directiveAPI.onExecutionFlowAdded = function (executionFlowObject) {
                            gridAPI.itemAdded(executionFlowObject);
                        };


                        return directiveAPI;
                    }
                };


                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Queueing_QueueItemHeaderAPIService.GetFilteredQueueItemHeader(dataRetrievalInput)
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