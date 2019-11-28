﻿"use strict";

app.directive("vrGenericdataDatatransformationdefinitionGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_DataTransformationDefinitionAPIService", "VR_GenericData_DataTransformationDefinitionService", "VRUIUtilsService", "VR_GenericData_GenericBusinessEntityService",
    function (UtilsService, VRNotificationService, VR_GenericData_DataTransformationDefinitionAPIService, VR_GenericData_DataTransformationDefinitionService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityService) {

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
            var gridDrillDownTabsObj;
            function initializeController() {

                $scope.dataTransformationDefinitions = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_GenericData_DataTransformationDefinitionService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onDataTransformationDefinitionAdded = function (onDataTransformationDefinitionObj) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(onDataTransformationDefinitionObj);
                            gridAPI.itemAdded(onDataTransformationDefinitionObj);
                        };
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_DataTransformationDefinitionAPIService.GetFilteredDataTransformationDefinitions(dataRetrievalInput)
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
                        clicked: editDataTransformationDefinition,
                        haspermission: hasEditDataTransformationDefinitionPermission
                    },
                    {
                        name: "Compile Project",
                        clicked: compileDevProject,
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
                    gridDrillDownTabsObj.setDrillDownExtensionObject(onDataTransformationDefinitionObj);
                    gridAPI.itemUpdated(onDataTransformationDefinitionObj);
                };

                VR_GenericData_DataTransformationDefinitionService.editDataTransformationDefinition(dataItem.Entity.DataTransformationDefinitionId, onDataTransformationDefinitionUpdated);
            }

            function compileDevProject(dataItem) {
                if (dataItem.Entity.DevProjectId == undefined)
                    return;
                VR_GenericData_GenericBusinessEntityService.CompileDevProject(dataItem.Entity.DevProjectId);
            }
        }

        return directiveDefinitionObject;

    }
]);