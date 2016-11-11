"use strict";

app.directive("vrGenericdataDatatransformationdefinitionGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_DataTransformationDefinitionAPIService", "VR_GenericData_DataTransformationDefinitionService",
    function (UtilsService, VRNotificationService, VR_GenericData_DataTransformationDefinitionAPIService, VR_GenericData_DataTransformationDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var dataTransformationDefinitionGrid = new DataTransformationDefinitionGrid($scope, ctrl, $attrs);
                dataTransformationDefinitionGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/DataTransformationDefinition/Templates/DataTransformationDefinitionGrid.html"

        };

        function DataTransformationDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            function initializeController() {

                $scope.dataTransformationDefinitions = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onDataTransformationDefinitionAdded = function (onDataTransformationDefinitionObj) {
                            gridAPI.itemAdded(onDataTransformationDefinitionObj);
                        };
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_DataTransformationDefinitionAPIService.GetFilteredDataTransformationDefinitions(dataRetrievalInput)
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
                    clicked: editDataTransformationDefinition,
                    haspermission: hasEditDataTransformationDefinitionPermission
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function hasEditDataTransformationDefinitionPermission() {                
                return VR_GenericData_DataTransformationDefinitionAPIService.HasUpdateDataTransformationDefinition();
            }

            function editDataTransformationDefinition(dataItem) {
                var onDataTransformationDefinitionUpdated = function (onDataTransformationDefinitionObj) {
                    gridAPI.itemUpdated(onDataTransformationDefinitionObj);
                };

                VR_GenericData_DataTransformationDefinitionService.editDataTransformationDefinition(dataItem.Entity.DataTransformationDefinitionId, onDataTransformationDefinitionUpdated);
            }
        }

        return directiveDefinitionObject;

    }
]);