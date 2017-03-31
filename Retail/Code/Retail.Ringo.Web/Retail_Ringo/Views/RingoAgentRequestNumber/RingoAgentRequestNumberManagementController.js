(function (appControllers) {
    "use strict";

    AgentRequestNumberManagementController.$inject = ['$scope',  'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function AgentRequestNumberManagementController($scope, UtilsService, vrUIUtilsService, vrNotificationService) {

        var gridAPI;

        var agentDirectiveApi;
        var agentReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();
        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.onAgentSelectorDirectiveReady = function (api) {
                agentDirectiveApi = api;
                agentReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

            $scope.scopeModel.onAgentRequestNumberUpdated = function (updateItem) {
                gridAPI.onRuleAdded(updateItem);
            };
        }

        function load() {
            $scope.scopeModel.isloading = true;
            loadAllControls().finally(function () {
                $scope.scopeModel.isloading = false;
            }).catch(function (error) {
                vrNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isloading = false;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadAgentSelector]).catch(function (error) {
                vrNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function loadAgentSelector() {
            var agentPayload = {
                AccountBEDefinitionId: "C7085851-44A6-47A6-8632-0C8E224D4226"
            };
            var selectorLoadDeferred = UtilsService.createPromiseDeferred();
            agentReadyPromiseDeferred.promise.then(function () {
                vrUIUtilsService.callDirectiveLoad(agentDirectiveApi, agentPayload, selectorLoadDeferred);
            });
            return selectorLoadDeferred.promise;
        }

        function buildGridQuery() {
            return {
                AgentIds: agentDirectiveApi.getSelectedIds()
            };
        }
    }

    appControllers.controller('Retail_Ringo_AgentRequestNumberManagementController', AgentRequestNumberManagementController);
})(appControllers);