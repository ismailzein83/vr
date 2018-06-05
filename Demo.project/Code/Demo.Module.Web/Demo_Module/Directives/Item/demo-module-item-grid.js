"use strict"
app.directive("demoModuleItemGrid", ["VRNotificationService", "Demo_Module_ItemAPIService", "Demo_Module_ItemService",
    function (VRNotificationService, Demo_Module_ItemAPIService, Demo_Module_ItemService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var itemGrid = new ItemGrid($scope, ctrl, $attrs);
                itemGrid.initializeController();
            },

            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Demo_Module/Directives/Item/templates/ItemGridTemplate.html"
        };

        function ItemGrid($scope, ctrl, $attrs) {
            $scope.items = [];
            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveAPI());
                    }

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onItemAdded = function (item) {
                            gridAPI.itemAdded(item);
                        };
                        return directiveAPI;
                    }
                };
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_Module_ItemAPIService.GetFilteredItems(dataRetrievalInput)
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
                $scope.gridMenuActions = [{
                    name: "Edit",
                    clicked: editItem,

                }, {
                    name: "Delete",
                    clicked: deleteItem,
                }];
            }

            function editItem(item) {
                var onItemUpdated = function (item) {
                    gridAPI.itemUpdated(item);
                };
                Demo_Module_ItemService.editItem(item.ItemId, item.ProductName, onItemUpdated);
            }

            function deleteItem(item) {
                var oItemDeleted = function (item) {
                    gridAPI.itemDeleted(item);
                };
                Demo_Module_ItemService.deleteItem($scope, item, onItemDeleted)
            }


        }
        return directiveDefinitionObject;
    }]
    );