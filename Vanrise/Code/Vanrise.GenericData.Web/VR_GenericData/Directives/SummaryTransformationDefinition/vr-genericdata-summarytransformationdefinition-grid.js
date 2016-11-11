"use strict";

app.directive("vrGenericdataSummarytransformationdefinitionGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_SummaryTransformationDefinitionAPIService", "VR_GenericData_SummaryTransformationDefinitionService",
    function (UtilsService, VRNotificationService, VR_GenericData_SummaryTransformationDefinitionAPIService, VR_GenericData_SummaryTransformationDefinitionService) {

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
            function initializeController() {

                $scope.summarytransformationdefinitions = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onSummaryTransformationDefinitionAdded = function (onSummaryTransformationDefinitionObj) {
                            gridAPI.itemAdded(onSummaryTransformationDefinitionObj);
                        };

                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_SummaryTransformationDefinitionAPIService.GetFilteredSummaryTransformationDefinitions(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
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
                    gridAPI.itemUpdated(summaryTransformationDefinitionObj);
                };

                VR_GenericData_SummaryTransformationDefinitionService.editSummaryTransformationDefinition(dataItem.Entity.SummaryTransformationDefinitionId, onSummaryTransformationDefinitionUpdated);
            }
        }

        return directiveDefinitionObject;

    }
]);