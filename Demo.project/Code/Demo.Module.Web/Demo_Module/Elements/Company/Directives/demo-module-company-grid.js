"use strict";

app.directive("demoModuleCompanyGrid", ["VRNotificationService", "Demo_Module_CompanyService", "Demo_Module_CompanyAPIService", "VRUIUtilsService",
    function (VRNotificationService, Demo_Module_CompanyService, Demo_Module_CompanyAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var companyGrid = new CompanyGrid($scope, ctrl, $attrs);
                companyGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Elements/Company/Directives/Templates/CompanyGridTemplate.html"
        };

        function CompanyGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridApi;
            var gridDrillDownTabsObj;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.companies = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownDefinitions(), gridApi); // create the drill downgrid and put the menu action to it 
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_Module_CompanyAPIService.GetFilteredCompanies(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]); // add the drillDownObject to each response.Data[i]
                            }
                        }
                        onResponseReady(response); // put response.Data in $scope.scopeModel.companies[]
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

                api.onCompanyAdded = function (company) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(company);
                    gridApi.itemAdded(company);
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            };

            function buildDrillDownDefinitions() {
                var drillDownDefinitions = [];
                drillDownDefinitions.push(buildBranchDrillDownDefinition());
                return drillDownDefinitions;
            };

            function buildBranchDrillDownDefinition() { // define an object pass the directive and the load function 
                var drillDownDefinitions = {};

                drillDownDefinitions.title = "Branch";
                drillDownDefinitions.directive = "demo-module-branch-management";

                drillDownDefinitions.loadDirective = function (directiveAPI, companyItem) {
                    companyItem.branchGridAPI = directiveAPI;
                    var payload = {
                        companyId: companyItem.CompanyId
                    }
                    return companyItem.branchGridAPI.load(payload);
                }
                return drillDownDefinitions;
            };

            function defineMenuActions() { // define the edit button with it corresponding method 
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editCompany
                }];

            };

            function editCompany(company) {

                var onCompanyUpdated = function (company) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(company);
                    gridApi.itemUpdated(company);
                };

                Demo_Module_CompanyService.editCompany(onCompanyUpdated, company.CompanyId);
            };
        };

        return directiveDefinitionObject;
    }]);