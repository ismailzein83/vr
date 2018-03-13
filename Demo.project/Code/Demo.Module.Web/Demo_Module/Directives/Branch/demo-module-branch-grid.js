"use strict"
app.directive("demoModuleBranchGrid", ["UtilsService", "VRNotificationService", "Demo_Module_BranchAPIService", "Demo_Module_BranchService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, Demo_Module_BranchAPIService, Demo_Module_BranchService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var branchGrid = new BranchGrid($scope, ctrl, $attrs);
            branchGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/Branch/Templates/BranchGridTemplate.html"
    };

    function BranchGrid($scope, ctrl) {

        var gridApi;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.branches = [];

            $scope.onGridReady = function (api) {
                gridApi = api;
        

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }
                function getDirectiveApi() {
                    var directiveApi = {};
                    directiveApi.loadGrid = function (query) {
                        
                        return gridApi.retrieveData(query);
                    };
                    directiveApi.onBranchAdded = function (branch) {
                        gridApi.itemAdded(branch);

                    };

                    return directiveApi;
                };
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                return Demo_Module_BranchAPIService.GetFilteredBranches(dataRetrievalInput)
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
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editBranch,

            }];
        };
        function editBranch(branch) {
            var onBranchUpdated = function (branch) {
                gridApi.itemUpdated(branch);

            };
            Demo_Module_BranchService.editBranch(branch.BranchId, onBranchUpdated);
        };
    };
    return directiveDefinitionObject;
}]);