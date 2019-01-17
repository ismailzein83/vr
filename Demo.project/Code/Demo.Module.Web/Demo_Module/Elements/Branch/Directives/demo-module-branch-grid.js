"use strict";

app.directive("demoModuleBranchGrid", ["VRNotificationService", "Demo_Module_BranchService", "Demo_Module_BranchAPIService", "VRUIUtilsService",
    function (VRNotificationService, Demo_Module_BranchService, Demo_Module_BranchAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var branchGrid = new BranchGrid($scope, ctrl, $attrs);
                branchGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Elements/Branch/Directives/Templates/BranchGridTemplate.html"
        };

        function BranchGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var companyId;

            var gridApi;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.branches = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_Module_BranchAPIService.GetFilteredBranches(dataRetrievalInput).then(function (response) {
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
                        companyId = payload.companyId;

                        if (payload.hideCompanyColumn)
                            $scope.scopeModel.hideCompanyColumn = (payload.hideCompanyColumn != undefined);
                    }
                    return gridApi.retrieveData(query);
                };

                api.onBranchAdded = function (branch) {
                    gridApi.itemAdded(branch);
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            };

            function defineMenuActions() { 
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editBranch
                }];
            };

            function editBranch(branch) {
                var onBranchUpdated = function (branch) {
                    gridApi.itemUpdated(branch);
                };

                var companyIdItem;
                if (companyId != undefined) {
                    companyIdItem = { CompanyId: companyId };
                }
                Demo_Module_BranchService.editBranch(onBranchUpdated, branch.BranchId, companyIdItem);
            };
        };

        return directiveDefinitionObject;
    }]);