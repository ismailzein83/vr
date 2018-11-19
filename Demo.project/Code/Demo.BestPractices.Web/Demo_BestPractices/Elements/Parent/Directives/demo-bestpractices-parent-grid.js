"use strict";

app.directive("demoBestpracticesParentGrid", ["VRNotificationService", "Demo_BestPractices_ParentAPIService", "Demo_BestPractices_ParentService", "VRUIUtilsService",
    function (VRNotificationService, Demo_BestPractices_ParentAPIService, Demo_BestPractices_ParentService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var parentGrid = new ParentGrid($scope, ctrl, $attrs);
                parentGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_BestPractices/Elements/Parent/Directives/Templates/ParentGridTemplate.html"
        };

        function ParentGrid($scope, ctrl) {
            this.initializeController = initializeController;

            var gridApi;
            var gridDrillDownTabsObj;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.parents = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownDefinitions(), gridApi, $scope.scopeModel.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_BestPractices_ParentAPIService.GetFilteredParents(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                defineMenuActions();
            };

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridApi.retrieveData(query);
                };

                api.onParentAdded = function (parent) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(parent);
                    gridApi.itemAdded(parent);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function buildDrillDownDefinitions() {
                var drillDownDefinitions = [];
                drillDownDefinitions.push(buildChildDrillDownDefinition());
                return drillDownDefinitions;
            }

            function buildChildDrillDownDefinition() {
                var drillDownDefinition = {};

                drillDownDefinition.title = "Child";
                drillDownDefinition.directive = "demo-bestpractices-child-search";

                drillDownDefinition.loadDirective = function (directiveAPI, parentItem) {
                    parentItem.childGridAPI = directiveAPI;
                    var payload = {
                        parentId: parentItem.ParentId
                    };
                    return parentItem.childGridAPI.load(payload);
                };

                return drillDownDefinition;
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editParent,
                }];
            };

            function editParent(parent) {
                var onParentUpdated = function (parent) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(parent);
                    gridApi.itemUpdated(parent);
                };

                Demo_BestPractices_ParentService.editParent(onParentUpdated, parent.ParentId);
            };
        };

        return directiveDefinitionObject;
    }]);