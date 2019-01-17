"use strict"

app.directive("demoBestpracticesChildGrid", ["UtilsService", "VRNotificationService", "Demo_BestPractices_ChildAPIService", "Demo_BestPractices_ChildService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
    function (UtilsService, VRNotificationService, Demo_BestPractices_ChildAPIService, Demo_BestPractices_ChildService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var childGrid = new ChildGrid($scope, ctrl, $attrs);
                childGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_BestPractices/Elements/Child/Directives/Templates/ChildGridTemplate.html"
        };

        function ChildGrid($scope, ctrl) {
            this.initializeController = initializeController;

            var gridApi;
            var parentId;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.childs = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                    return Demo_BestPractices_ChildAPIService.GetFilteredChilds(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                defineMenuActions();
            };

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var query;

                    if (payload != undefined) {
                        query = payload.query;
                        parentId = payload.parentId;

                        if (payload.hideParentColumn)
                            $scope.scopeModel.hideParentColumn = (payload.hideParentColumn != undefined) ;
                    }

                    return gridApi.retrieveData(query);
                };

                api.onChildAdded = function (child) {
                    gridApi.itemAdded(child);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editChild,
                }];
            };

            function editChild(child) {
                var onChildUpdated = function (child) {
                    gridApi.itemUpdated(child);
                };

                var parentIdItem;
                if (parentId != undefined) {
                    parentIdItem = { ParentId: parentId };
                }
                Demo_BestPractices_ChildService.editChild(onChildUpdated, child.ChildId, parentIdItem);
            };
        };

        return directiveDefinitionObject;
    }]);