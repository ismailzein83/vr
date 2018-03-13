"use strict"
app.directive("demoModuleCompanyGrid", ["UtilsService", "VRNotificationService", "Demo_Module_CompanyAPIService", "Demo_Module_CompanyService", "VRUIUtilsService","VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_CompanyAPIService, Demo_Module_CompanyService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var companyGrid = new CompanyGrid($scope, ctrl, $attrs);
            companyGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/Company/Templates/CompanyGridTemplate.html"
    };

    function CompanyGrid($scope, ctrl) {

        var gridApi;
        var gridDrillDownTabsObj;
        var drillDownDefinitionsArray = [];
        this.initializeController = initializeController;

        function initializeController() {
            $scope.companies = [];

            $scope.onGridReady = function (api) {
                gridApi = api;
                
                registerObjectTrackingDrillDownToCompany();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitionsArray, gridApi, $scope.gridMenuActions);

                function registerObjectTrackingDrillDownToCompany() {
                    var drillDownDefinition = {};

                    drillDownDefinition.title = "Branch";
                    drillDownDefinition.directive = "demo-module-branch-grid";


                    drillDownDefinition.loadDirective = function (directiveAPI, companyItem) {

                        companyItem.objectTrackingGridAPI = directiveAPI;

                        var input = {
                            Name: null,
                            CompanyIds: [companyItem.CompanyId]
                        };
                       
                        return companyItem.objectTrackingGridAPI.loadGrid(input);
                    };

                    drillDownDefinitionsArray.push(drillDownDefinition);
                    
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {

                    ctrl.onReady(getDirectiveApi()); //  to use in management controller
                }

                function getDirectiveApi() {
                    var directiveApi = {};
                    directiveApi.loadGrid = function (query) {
                      
                        return gridApi.retrieveData(query); // internal function
                    };
                    directiveApi.onCompanyAdded = function (company) {

                        gridDrillDownTabsObj.setDrillDownExtensionObject(company);
                        gridApi.itemAdded(company); // internal function                        
                    };
                    return directiveApi;
                };
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_Module_CompanyAPIService.GetFilteredCompanies(dataRetrievalInput)
                .then(function (response) {
                    if (response.Data != undefined)
                    {                      
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
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editCompany,

            }];
        };
        function editCompany(company) {
            var onCompanyUpdated = function (company) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(company);
                gridApi.itemUpdated(company);
                
            };
            Demo_Module_CompanyService.editCompany(company.CompanyId, onCompanyUpdated);
        };


    };
    return directiveDefinitionObject;
}]);
