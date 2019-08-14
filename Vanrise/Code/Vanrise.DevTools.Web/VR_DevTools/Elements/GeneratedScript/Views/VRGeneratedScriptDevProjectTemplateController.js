(appControllers); (function (appControllers) {
    "use strict";
    generatedScriptDevProjectTemplateController.$inject = ['$scope', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService','VR_Devtools_DevProjectTemplateAPIService'];

    function generatedScriptDevProjectTemplateController($scope, VRNotificationService, UtilsService, VRUIUtilsService, VR_Devtools_DevProjectTemplateAPIService) {

        var connectionStringDirectiveApi;
        var connectionStringReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var devProjectDirectiveApi;
        var devProjectReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var templateTablesDirectiveAPI;
        var templateTablesReadyPromiseDeferred = UtilsService.createPromiseDeferred();
         $scope.scopeModel = {};
        defineScope();

        load();

        function defineScope() {

            $scope.scopeModel.generateScripts = function () {
                $scope.scopeModel.isLoading = true;
                VR_Devtools_DevProjectTemplateAPIService.GetDevProjectTemplates({
                    ConnectionId: connectionStringDirectiveApi.getSelectedIds(),
                    DevProjectId: devProjectDirectiveApi.getSelectedIds(),
                    TableNames: templateTablesDirectiveAPI.getSelectedIds()
                }).then(function (response) {
                    if ($scope.onTemplateAdded != undefined)
                        $scope.onTemplateAdded(response);
                }).finally(function () {
                    $scope.scopeModel.isLoading = true;
                    $scope.scopeModel.close();
                });
                
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onConnectionStringDirectiveReady = function (api) {
                connectionStringDirectiveApi = api;
                connectionStringReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onTemplateTablesDirectiveReady = function (api){

                templateTablesDirectiveAPI = api;
                templateTablesReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onDevProjectsDirectiveReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onConnectionStringChanged = function (value) {
                if (value != undefined) {
                    
                    var  devProjectPayload = {
                        connectionId: connectionStringDirectiveApi.getSelectedIds()
                    };
                    var setDevProjectLoader = function (value) { $scope.scopeModel.isDevProjectDirectiveloading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, devProjectDirectiveApi, devProjectPayload, setDevProjectLoader);
                }
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            loadAllControls();
        }

        function loadAllControls() {
            
            function loadConnectionStringDirective() {
                var connectionStringLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                connectionStringReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(connectionStringDirectiveApi, undefined, connectionStringLoadPromiseDeferred);
                });
                return connectionStringLoadPromiseDeferred.promise;
            }

            function loadTemplateTablesDirective() {
                var templateTablesLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                templateTablesReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(templateTablesDirectiveAPI, undefined, templateTablesLoadPromiseDeferred);
                });
                return templateTablesLoadPromiseDeferred.promise;
            }

            $scope.title = "Generate Template";

            return UtilsService.waitPromiseNode({ promises: [loadConnectionStringDirective(), loadTemplateTablesDirective()] })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
    }
    appControllers.controller('VR_Devtools_VRGeneratedScriptDevProjectTemplateController', generatedScriptDevProjectTemplateController);
})(appControllers);