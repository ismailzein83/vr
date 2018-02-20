"use strict";

app.directive("vrGenericdataExtensiblebeitemGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_ExtensibleBEItemAPIService", "VR_GenericData_ExtensibleBEItemService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VR_GenericData_ExtensibleBEItemAPIService, VR_GenericData_ExtensibleBEItemService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ExtensibleBEItemGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Templates/ExtensibleBEItemGrid.html"

        };

        function ExtensibleBEItemGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {

                $scope.extensibleBEItems = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onExtensibleBEItemAdded = function (extensibleBEItemObj) {
                            gridAPI.itemAdded(extensibleBEItemObj);
                        };
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_ExtensibleBEItemAPIService.GetFilteredExtensibleBEItems(dataRetrievalInput)
                        .then(function (response) {
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
                    clicked: editExtensibleBEItem,
                    haspermission: hasEditExtensibleBEItem

                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editExtensibleBEItem(dataItem) {
                var onExtensibleBEItemUpdated = function (extensibleBEItem) {
                    gridAPI.itemUpdated(extensibleBEItem);
                };

                VR_GenericData_ExtensibleBEItemService.editExtensibleBEItem(dataItem.Entity.ExtensibleBEItemId, onExtensibleBEItemUpdated);
            }

            function hasEditExtensibleBEItem() {
                return VR_GenericData_ExtensibleBEItemAPIService.HasUpdateExtensibleBEItem();
            }
        }

        return directiveDefinitionObject;

    }
]);