(appControllers); (function (appControllers) {
    "use strict";
    generatedScriptDevProjectTemplateController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Devtools_GeneratedScriptService'];

    function generatedScriptDevProjectTemplateController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Devtools_GeneratedScriptService) {

        var connectionStringDirectiveApi;
        var connectionStringReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var devProjectDirectiveApi;
        var devProjectReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        $scope.scopeModel = {};

        defineScope();

        load();

        function defineScope() {

            $scope.scopeModel.generateScripts = function () {

            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onConnectionStringDirectiveReady = function (api) {
                connectionStringDirectiveApi = api;
                connectionStringReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onDevProjectsDirectiveReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onConnectionStringChanged = function (value) {
                if (value != undefined) {
                    var payload = {
                        connectionId: connectionStringDirectiveApi.getSelectedIds()
                    };
                    var setLoader = function (value) { $scope.scopeModel.isDevProjectDirectiveloading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, devProjectDirectiveApi, payload, setLoader);
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

            $scope.title = UtilsService.buildTitleForUpdateEditor("", "Default Scripts");

            return UtilsService.waitPromiseNode({ promises: [loadConnectionStringDirective()] })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
    }
    appControllers.controller('VR_Devtools_VRGeneratedScriptDevProjectTemplateController', generatedScriptDevProjectTemplateController);
})(appControllers);