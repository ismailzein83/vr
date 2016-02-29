"use strict";

app.directive("vrGenericdataGenericbusinessentitydefinitionGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_BusinessEntityDefinitionAPIService", "VR_GenericData_GenericEditorService","VRUIUtilsService",
    function (UtilsService, VRNotificationService, VR_GenericData_BusinessEntityDefinitionAPIService, VR_GenericData_GenericEditorService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new GenericBusinessEntityDefinitionGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Templates/GenericBusinessEntityDefinitionGrid.html"

        };

        function GenericBusinessEntityDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {

                $scope.businessEntityDefinitions = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    var drillDownDefinitions = [];
                    var drillDownDefinition = {};

                    drillDownDefinition.title = "Generic Editor Definition";
                    drillDownDefinition.directive = "vr-genericdata-genericeditordefinition-grid";

                    drillDownDefinition.loadDirective = function (directiveAPI, genericEditorObj) {
                        genericEditorObj.genericEditorGridAPI = directiveAPI;
                        var payload = {
                                BusinessEntityDefinitionId: genericEditorObj.Entity.BusinessEntityDefinitionId
                        };
                        return genericEditorObj.genericEditorGridAPI.loadGrid(payload);
                    };
                    drillDownDefinitions.push(drillDownDefinition);
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);


                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        }
                        directiveAPI.onGenericBusinessEntityDefinitionAdded = function (genericBusinessEntityDefinitionObj) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(genericBusinessEntityDefinitionObj);
                            gridAPI.itemAdded(genericBusinessEntityDefinitionObj);
                        }
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_BusinessEntityDefinitionAPIService.GetFilteredBusinessEntityDefinitions(dataRetrievalInput)
                        .then(function (response) {
                            if (response && response.Data) {
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
                    name: "Add Extended Settings",
                    clicked: addExtendedSettings,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function addExtendedSettings(dataItem) {
                gridAPI.expandRow(dataItem);
                var onExtendedSettingsAdded = function (extendedSettingsObj) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(extendedSettingsObj);
                    dataItem.genericEditorGridAPI.onGenericEditorAdded(extendedSettingsObj);
                }

                VR_GenericData_GenericEditorService.addExtendedSettings(dataItem.Entity.BusinessEntityDefinitionId, onExtendedSettingsAdded);
            }
        }

        return directiveDefinitionObject;

    }
]);