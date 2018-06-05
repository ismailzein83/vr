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

        var gridApi;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.childs = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi()); 
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (query) {
                        return gridApi.retrieveData(query);
                    };

                    directiveApi.onChildAdded = function (child) {
                        gridApi.itemAdded(child);
                    };
                    return directiveApi;
                };
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_BestPractices_ChildAPIService.GetFilteredChilds(dataRetrievalInput)
                .then(function (response) {
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
                clicked: editChild,

            }];
        };
        function editChild(child) {
            var onChildUpdated = function (child) {
                gridApi.itemUpdated(child);
            };
            Demo_BestPractices_ChildService.editChild(child.ChildId, onChildUpdated);
        };


    };
    return directiveDefinitionObject;
}]);
