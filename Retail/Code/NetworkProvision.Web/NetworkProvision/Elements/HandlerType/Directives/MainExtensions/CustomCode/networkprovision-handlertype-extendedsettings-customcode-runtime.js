'use strict';

app.directive('networkprovisionHandlertypeExtendedsettingsCustomcodeRuntime', ['UtilsService', 'NetworkProvision_HandlerTypeAPIService', 'VRNotificationService',
    function (UtilsService, NetworkProvision_HandlerTypeAPIService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CustomCode($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/NetworkProvision/Elements/HandlerType/Directives/MainExtensions/CustomCode/Templates/CustomCodeHandlerTypeRuntimeTemplate.html'
        };

        function CustomCode($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var customCodeErrors;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.tryCompileCustomCode = function () {
                    return tryCompileCustomCode();
                };

                $scope.scopeModel.customCodeHasErrorsValidator = function () {
                    if (customCodeErrors != undefined && customCodeErrors.length > 0) {
                        var errorMessage = '';

                        for (var i = 0; i < customCodeErrors.length; i++) {
                            errorMessage += (i + 1) + ') ' + customCodeErrors[i] + '\n';
                        }

                        return errorMessage;
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var extendedSettings;

                    if (payload != undefined)
                        extendedSettings = payload.extendedSettings;

                    if (extendedSettings != undefined) {
                        $scope.scopeModel.namespaceMembers = extendedSettings.NamespaceMembers;
                        $scope.scopeModel.customCode = extendedSettings.CustomCode;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "NetworkProvision.Business.CustomCodeNetworkProvisionHandlerType, NetworkProvision.Business",
                        NamespaceMembers: $scope.scopeModel.namespaceMembers,
                        CustomCode: $scope.scopeModel.customCode
                    }
                };

                if (ctrl.onReady != undefined)
                    ctrl.onReady(api);
            }

            function tryCompileCustomCode() {
                customCodeErrors = undefined;
                var promiseDeferred = UtilsService.createPromiseDeferred();
                $scope.scopeModel.isLoading = true;

                var customCodeObj = buildCustomCodeObjFromScope();
                NetworkProvision_HandlerTypeAPIService.TryCompileCustomCode(customCodeObj).then(function (response) {
                    if (response.Result) {
                        VRNotificationService.showSuccess("Custom code compiled successfully.");
                        promiseDeferred.resolve(true);
                    }
                    else {
                        customCodeErrors = response.CustomCodeErrors;
                        promiseDeferred.resolve(false);
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                    promiseDeferred.reject(error);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });

                return promiseDeferred.promise;
            }

            function buildCustomCodeObjFromScope() {
                return {
                    NamespaceMembers: $scope.scopeModel.namespaceMembers,
                    CustomCode: $scope.scopeModel.customCode
                };
            }
        }
    }
]);