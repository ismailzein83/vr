(function (appControllers) { 
    "use strict";

    pageDefinitionManagementController.$inject = ['$scope', 'Demo_Module_PageDefinitionService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function pageDefinitionManagementController($scope, Demo_Module_PageDefinitionService, VRNotificationService, UtilsService, VRUIUtilsService) {

        var gridApi;
        defineScope();
      load();

        function defineScope() {

            $scope.scopeModel = {};


            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                api.load(getFilter());
            };

            $scope.scopeModel.search = function () {
                return gridApi.load(getFilter());
            };

            $scope.scopeModel.addPageDefinition = function () {
                var onPageDefinitionAdded = function (pageDefinition) {
                    if (gridApi != undefined) {
                        gridApi.onPageDefinitionAdded(pageDefinition);
                    }
                };
           

                Demo_Module_PageDefinitionService.addPageDefinition(onPageDefinitionAdded);

            };
        };

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();

        };


        function loadAllControls() {
           
                 $scope.scopeModel.isLoading = false;
             
        };
   

        function getFilter() {
            return {
                query: {
                    Name: $scope.scopeModel.name,
                }
            };
        };







    };

    appControllers.controller('Demo_Module_PageDefinitionManagementController', pageDefinitionManagementController);
})(appControllers);