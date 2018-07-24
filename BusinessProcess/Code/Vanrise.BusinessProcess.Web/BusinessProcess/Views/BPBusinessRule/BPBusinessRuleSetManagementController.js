(function (appControllers) {

    "use strict";

    BusinessProcess_BP_BusinessRuleSetManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPBusinessRuleSetService', 'BusinessProcess_BPBusinessRuleSetAPIService'];

    function BusinessProcess_BP_BusinessRuleSetManagementController($scope, UtilsService, VRUIUtilsService, BusinessProcess_BPBusinessRuleSetService, BusinessProcess_BPBusinessRuleSetAPIService) {
        var gridAPI;
        var filter = {};

        var bpDefinitionDirectiveApi;
        var bpDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        defineScope();
        loadAllControls();
        function defineScope() {
            $scope.onBPDefinitionDirectiveReady = function (api) {
                bpDefinitionDirectiveApi = api;
                bpDefinitionReadyPromiseDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            $scope.searchClicked = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.addBusinessRuleSet = function () {
                var onBusinessRuleSetAdded = function (addedItem) {
                    gridAPI.onBusinessRuleSetAdded(addedItem);
                };

                BusinessProcess_BPBusinessRuleSetService.addBusinessRuleSet(onBusinessRuleSetAdded);
            };

            $scope.hasAddBusinessRuleSet = function () {
                return BusinessProcess_BPBusinessRuleSetAPIService.HasAddBusinessRuleSet();
            };
        }

        function getFilterObject() {
            filter = {
                DefinitionsId: bpDefinitionDirectiveApi.getSelectedIds(),
            };
        }

        function loadAllControls() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadBPDefinitions])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = 'Business Rule';
        }

        function loadBPDefinitions() {
            var loadBPDefinitionsPromiseDeferred = UtilsService.createPromiseDeferred();
            bpDefinitionReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    filter: {
                        $type: "Vanrise.BusinessProcess.Entities.BPDefinitionInfoFilter,Vanrise.BusinessProcess.Entities",
                        Filters: [{
                            $type: "Vanrise.BusinessProcess.Business.BPDefinitionRuleSetFilter,Vanrise.BusinessProcess.Business"
                        }]
                    }
                }
                VRUIUtilsService.callDirectiveLoad(bpDefinitionDirectiveApi, payload, loadBPDefinitionsPromiseDeferred);
            });

            return loadBPDefinitionsPromiseDeferred.promise;


        }
    }

    appControllers.controller('BusinessProcess_BP_BusinessRuleSetManagementController', BusinessProcess_BP_BusinessRuleSetManagementController);
})(appControllers);