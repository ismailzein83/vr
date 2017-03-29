"use strict";

app.directive("vrGenericdataDatastoreGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_DataStoreAPIService", "VR_GenericData_DataStoreService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VR_GenericData_DataStoreAPIService, VR_GenericData_DataStoreService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var datastoreGrid = new DataStoreGrid($scope, ctrl, $attrs);
                datastoreGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/DataStore/Templates/DataStoreGrid.html"

        };

        function DataStoreGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {

                $scope.datastore = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_GenericData_DataStoreService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onDataStoreAdded = function (onDataStoreObj) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(onDataStoreObj);
                            gridAPI.itemAdded(onDataStoreObj);
                        };
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_DataStoreAPIService.GetFilteredDataStores(dataRetrievalInput)
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
                    clicked: editDataStore,
                    haspermission: hasEditDataStorePermission
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function hasEditDataStorePermission() {
               return VR_GenericData_DataStoreAPIService.HasUpdateDataStore();
            };
            function editDataStore(dataItem) {
                var onDataStoreUpdated = function (dataStoreObj) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(dataStoreObj);
                    gridAPI.itemUpdated(dataStoreObj);
                };
                VR_GenericData_DataStoreService.editDataStore(dataItem.Entity.DataStoreId, onDataStoreUpdated);
            }
        }

        return directiveDefinitionObject;

    }
]);