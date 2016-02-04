﻿"use strict";

app.directive("vrQueueingExecutionflowGrid", ["VR_Queueing_ExecutionFlowAPIService", "VR_Queueing_ExecutionFlowService", 'VRNotificationService', 'VRUIUtilsService', 'VR_Queueing_QueueItemStatusEnum', 'LabelColorsEnum',
    function (VR_Queueing_ExecutionFlowAPIService, VR_Queueing_ExecutionFlowService, VRNotificationService, VRUIUtilsService, VR_Queueing_QueueItemStatusEnum, LabelColorsEnum) {

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
            templateUrl: "/Client/Modules/Queueing/Directives/ExecutionFlow/Templates/ExecutionFlowGrid.html"

        };

        function ExecutionFlowGrid($scope, ctrl, $attrs) {

            var gridAPI;
            var gridDrillDownTabsObj;
            var isFirstTimeLoaded = false;
            this.initializeController = initializeController;

            function initializeController() {


                $scope.getNotProcessedColor = function (dataItem) {
                    var color = undefined;
                    var notProcessedCount = dataItem.notProcessedCount;
                    if (notProcessedCount != undefined) {

                        if (notProcessedCount >= 15 && notProcessedCount < 30)
                            color = LabelColorsEnum.Warning.color;
                        else if (notProcessedCount >= 30)
                            color = LabelColorsEnum.Error.color;
                    }

                    return color;
                }


                $scope.getSuspendedColor = function (dataItem) {
                    var color = undefined;
                    var suspendedCount = dataItem.suspendedCount;
                    if (suspendedCount != undefined) {
                        if (suspendedCount > 0)
                            color = LabelColorsEnum.Error.color;
                    }

                    return color;
                }


                $scope.executionFlows = [];
                $scope.ongridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_Queueing_ExecutionFlowService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());



                    function getDirectiveAPI() {

                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        }

                        directiveAPI.onExecutionFlowAdded = function (executionFlowObject) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(executionFlowObject);
                            gridAPI.itemAdded(executionFlowObject);
                        }


                        return directiveAPI;
                    }
                };


                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Queueing_ExecutionFlowAPIService.GetFilteredExecutionFlows(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                                }
                            }

                            onResponseReady(response);
                            var timer;
                            if (!isFirstTimeLoaded) {
                                isFirstTimeLoaded = true;
                                refreshExecutionFlowGrid()
                                timer = setInterval(function(){
                                    refreshExecutionFlowGrid();                                
                                }, 5000);
                            }
                            $scope.$on('$destroy', function () {                               
                                clearInterval(timer);
                            });
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        });
                };
                defineMenuActions();
            }

            function refreshExecutionFlowGrid() {
                VR_Queueing_ExecutionFlowAPIService.GetExecutionFlowStatusSummary().then(function (response) {
                    if ($scope.executionFlows != undefined) {
                        var notProcessedCount;
                        var suspendedCount;
                        for (var i = 0; i < $scope.executionFlows.length; i++) {
                            notProcessedCount = 0;
                            suspendedCount = 0;
                            for (var j = 0; j < response.length; j++) {
                                if ($scope.executionFlows[i].Entity.ExecutionFlowId == response[j].ExecutionFlowId) {
                                    var status = response[j].Status;
                                    if (status == VR_Queueing_QueueItemStatusEnum.New.value || status == VR_Queueing_QueueItemStatusEnum.Processing.value || status == VR_Queueing_QueueItemStatusEnum.Failed.value) {
                                        notProcessedCount += response[j].Count;
                                    }
                                    else if (status == VR_Queueing_QueueItemStatusEnum.Suspended.value) {
                                        suspendedCount += response[j].Count;
                                    }

                                }
                            }
                            $scope.executionFlows[i].notProcessedCount = notProcessedCount;
                            $scope.executionFlows[i].suspendedCount = suspendedCount;

                        }
                    }
                })
            }

            function defineMenuActions() {
                $scope.gridMenuActions = [{
                    name: "Edit",
                    clicked: editExecutionFlow
                }];
            }

            function editExecutionFlow(executionFlowObj) {
                var onExecutionFlowUpdated = function (executionFlowObj) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(executionFlowObj);
                    gridAPI.itemUpdated(executionFlowObj);
                }

                VR_Queueing_ExecutionFlowService.editExecutionFlow(executionFlowObj.Entity.ExecutionFlowId, onExecutionFlowUpdated);
            }


        }

        return directiveDefinitionObject;

    }]);