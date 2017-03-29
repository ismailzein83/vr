"use strict";

app.directive("vrGenericdataSummarytransformationdefinitionGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_SummaryTransformationDefinitionAPIService", "VR_GenericData_SummaryTransformationDefinitionService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VR_GenericData_SummaryTransformationDefinitionAPIService, VR_GenericData_SummaryTransformationDefinitionService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var summaryTransformationDefinitionGrid = new SummaryTransformationDefinitionGrid($scope, ctrl, $attrs);
                summaryTransformationDefinitionGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/SummaryTransformationDefinition/Templates/SummaryTransformationDefinitionGrid.html"

        };

        function SummaryTransformationDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {

                $scope.summarytransformationdefinitions = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_GenericData_SummaryTransformationDefinitionService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onSummaryTransformationDefinitionAdded = function (onSummaryTransformationDefinitionObj) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(onSummaryTransformationDefinitionObj);
                            gridAPI.itemAdded(onSummaryTransformationDefinitionObj);
                        };

                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_SummaryTransformationDefinitionAPIService.GetFilteredSummaryTransformationDefinitions(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                                }
                            }
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };

                defineMenuActions();
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editSummaryTransformationDefinition,
                    haspermission: hasEditSummaryTransformationDefinitionPermission
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function hasEditSummaryTransformationDefinitionPermission() {
                return VR_GenericData_SummaryTransformationDefinitionAPIService.HasUpdateSummaryTransformationDefinition()
            }
            function editSummaryTransformationDefinition(dataItem) {
                var onSummaryTransformationDefinitionUpdated = function (summaryTransformationDefinitionObj) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(summaryTransformationDefinitionObj);
                    gridAPI.itemUpdated(summaryTransformationDefinitionObj);
                };

                VR_GenericData_SummaryTransformationDefinitionService.editSummaryTransformationDefinition(dataItem.Entity.SummaryTransformationDefinitionId, onSummaryTransformationDefinitionUpdated);
            }
        }

        return directiveDefinitionObject;

    }
]);