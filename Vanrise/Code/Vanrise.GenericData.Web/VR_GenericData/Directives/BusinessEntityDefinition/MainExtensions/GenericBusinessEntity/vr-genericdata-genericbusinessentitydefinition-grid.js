"use strict";

app.directive("vrGenericdataGenericbusinessentitydefinitionGrid", [
    "UtilsService",
    "VRNotificationService",
    "VR_GenericData_BusinessEntityDefinitionAPIService",
    "VR_GenericData_ExtensibleBEItemService",
    "VRUIUtilsService",
    "VR_GenericData_BusinessEntityDefinitionService",
    "VR_GenericData_ExtensibleBEItemAPIService",
    function (UtilsService, VRNotificationService, VR_GenericData_BusinessEntityDefinitionAPIService, VR_GenericData_ExtensibleBEItemService, VRUIUtilsService, VR_GenericData_BusinessEntityDefinitionService, extensibleBEItemAPIService) {

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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Templates/GenericBusinessEntityDefinitionGrid.html"

        };

        function GenericBusinessEntityDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;

            function initializeController() {

                $scope.businessEntityDefinitions = [];

                $scope.isExpandable = function (dataItem) {
                    if (dataItem.IsExtensible)
                        return true;
                    return false
                };

                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    var drillDownDefinitions = [];
                    var drillDownDefinition = {};

                    drillDownDefinition.title = "Extensible Types";
                    drillDownDefinition.directive = "vr-genericdata-extensiblebeitem-grid";

                    drillDownDefinition.loadDirective = function (directiveAPI, extensibleBEItem) {
                        extensibleBEItem.genericEditorGridAPI = directiveAPI;
                        var payload = {
                                BusinessEntityDefinitionId: extensibleBEItem.Entity.BusinessEntityDefinitionId
                        };
                        return extensibleBEItem.genericEditorGridAPI.loadGrid(payload);
                    };
                    drillDownDefinitions.push(drillDownDefinition);
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);


                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onGenericBusinessEntityDefinitionAdded = function (genericBusinessEntityDefinitionObj) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(genericBusinessEntityDefinitionObj);
                            gridAPI.itemAdded(genericBusinessEntityDefinitionObj);
                        };
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_BusinessEntityDefinitionAPIService.GetFilteredBusinessEntityDefinitions(dataRetrievalInput)
                        .then(function (response) {
                            if (response && response.Data) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    if (response.Data[i].IsExtensible) {
                                        gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                                   }
                                    
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
                var extensibleMenuActions = [
                {
                    name: "Add Extensible Type",
                    clicked: addExtendedSettings,
                    haspermission: hasAddExtendedSettings
                }];
                var genericMenuActions = [
                {
                     name: "Edit",
                     clicked: editBusinessEntityDefinition,
                     haspermission: hasEditBusinessEntityDefinition

                 }];
                $scope.gridMenuActions = function (dataItem) {
                    if (dataItem.IsExtensible)
                        return extensibleMenuActions;
                    else if (dataItem.Entity.Settings.DefinitionEditor != undefined) {
                        return genericMenuActions;
                    }
                };
            }

            function hasEditBusinessEntityDefinition() {
                return VR_GenericData_BusinessEntityDefinitionAPIService.HasUpdateBusinessEntityDefinition();
            }

            function hasAddExtendedSettings() {
                return extensibleBEItemAPIService.HasAddExtensibleBEItem();
            }

            function addExtendedSettings(dataItem) {
                gridAPI.expandRow(dataItem);
                var onExtendedSettingsAdded = function (extendedSettingsObj) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(extendedSettingsObj);
                    dataItem.genericEditorGridAPI.onExtensibleBEItemAdded(extendedSettingsObj);
                };

                VR_GenericData_ExtensibleBEItemService.addExtendedSettings(dataItem.Entity.BusinessEntityDefinitionId, onExtendedSettingsAdded);
            }

            function editBusinessEntityDefinition(dataItem)
            {
                var onBusinessEntityDefinitionUpdated = function (businessEntityDefinition) {
                    gridAPI.itemUpdated(businessEntityDefinition);
                };

                VR_GenericData_BusinessEntityDefinitionService.editBusinessEntityDefinition(dataItem.Entity.BusinessEntityDefinitionId, onBusinessEntityDefinitionUpdated);
            }
        }

        return directiveDefinitionObject;

    }
]);