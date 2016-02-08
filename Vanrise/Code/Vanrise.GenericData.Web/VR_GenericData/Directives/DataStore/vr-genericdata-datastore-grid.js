"use strict";

app.directive("vrGenericdataDatastoreGrid", ["UtilsService", "VRNotificationService", 
    function (UtilsService, VRNotificationService) {

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
            function initializeController() {

                $scope.datastore = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        }
                        directiveAPI.onDataStoreAdded = function (onDataStoreObj) {
                            gridAPI.itemAdded(onDataStoreObj);
                        }
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    //return VR_GenericData_DataTransformationDefinitionAPIService.GetFilteredDataTransformationDefinitions(dataRetrievalInput)
                    //    .then(function (response) {
                    //        if (response.Data != undefined) {
                    //        }
                    //        onResponseReady(response);
                    //    })
                    //    .catch(function (error) {
                    //        VRNotificationService.notifyException(error, $scope);
                    //    });
                };

                defineMenuActions();
            }


            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit"//,
                   // clicked: editDataTransformationDefinition,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            //function editDataTransformationDefinition(dataItem) {
            //    var onDataTransformationDefinitionUpdated = function (onDataTransformationDefinitionObj) {
            //        gridAPI.itemUpdated(onDataTransformationDefinitionObj);
            //    }

            //    VR_GenericData_DataTransformationDefinitionService.editDataTransformationDefinition(dataItem.Entity.DataTransformationDefinitionId, onDataTransformationDefinitionUpdated);
            //}
        }

        return directiveDefinitionObject;

    }
]);