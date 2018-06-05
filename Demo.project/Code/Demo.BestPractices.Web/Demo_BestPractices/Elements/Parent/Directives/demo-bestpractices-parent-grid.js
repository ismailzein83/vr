"use strict"
app.directive("demoBestpracticesParentGrid", ["UtilsService", "VRNotificationService", "Demo_BestPractices_ParentAPIService", "Demo_BestPractices_ParentService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_BestPractices_ParentAPIService, Demo_BestPractices_ParentService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

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

        var gridApi;
        var gridDrillDownTabsObj;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.parents = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                var drillDownDefinitions = [];
                AddChildDrillDown();
                function AddChildDrillDown() {
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
                    drillDownDefinitions.push(drillDownDefinition);
                }

                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridApi, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi()); 
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (query) {
                        return gridApi.retrieveData(query);
                    };

                    directiveApi.onParentAdded = function (parent) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(parent);
                        gridApi.itemAdded(parent);
                    };
                    return directiveApi;
                };
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_BestPractices_ParentAPIService.GetFilteredParents(dataRetrievalInput)
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
        };

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
            Demo_BestPractices_ParentService.editParent(parent.ParentId, onParentUpdated);
        };


    };
    return directiveDefinitionObject;
}]);
