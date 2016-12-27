'use strict';

app.directive('retailBeAccountbedefinitionsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountBeDefinitionsSettingsEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/Templates/AccountBEDefinitionEditorTemplate.html'
        };

        function AccountBeDefinitionsSettingsEditorCtor(ctrl, $scope, $attrs) {

            var accountGridDefinitionDirectiveAPI;
            var accountGridDefinitionDirectiveDeferred = UtilsService.createPromiseDeferred();

            var accountViewDefinitionDirectiveAPI;
            var accountViewDefinitionDirectiveDeferred = UtilsService.createPromiseDeferred();

            var accountActionDefinitionDirectiveAPI;
            var accountActionDefinitionDirectiveDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAccountGridDefinitionReady = function (api) {
                    accountGridDefinitionDirectiveAPI = api;
                    accountGridDefinitionDirectiveDeferred.resolve();
                };
                $scope.scopeModel.onAccountViewDefinitionsReady = function (api) {
                    accountViewDefinitionDirectiveAPI = api;
                    accountViewDefinitionDirectiveDeferred.resolve();
                }
                $scope.scopeModel.onAccountActionDefinitionsReady = function (api) {
                    accountActionDefinitionDirectiveAPI = api;
                    accountActionDefinitionDirectiveDeferred.resolve();
                }
                UtilsService.waitMultiplePromises([accountGridDefinitionDirectiveDeferred.promise, accountViewDefinitionDirectiveDeferred.promise, accountActionDefinitionDirectiveDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountGridDefinition;
                    var accountViewDefinitions;
                    var accountActionDefinitions;
                    if (payload != undefined && payload.businessEntityDefinitionSettings != undefined) {
                        accountGridDefinition = payload.businessEntityDefinitionSettings.GridDefinition;
                        accountViewDefinitions = payload.businessEntityDefinitionSettings.AccountViewDefinitions;
                        accountActionDefinitions = payload.businessEntityDefinitionSettings.ActionDefinitions;
                    }

                    //Loading AccountGridDefinition Directive
                    var accountGridDefinitionLoadPromise = getAccountGridDefinitionLoadPromise();
                    promises.push(accountGridDefinitionLoadPromise);

                    //Loading AccountViewDefinition Directive
                    var accountViewDefinitionLoadPromise = getAccountViewDefinitionLoadPromise();
                    promises.push(accountViewDefinitionLoadPromise);

                    //Loading AccountActionDefinition Directive
                    var accountActionDefinitionLoadPromise = getAccountActionDefinitionLoadPromise();
                    promises.push(accountActionDefinitionLoadPromise);

                    function getAccountGridDefinitionLoadPromise() {
                        var accountGridDefitnionLoadDeferred = UtilsService.createPromiseDeferred();

                        var accountGridDefinitionPayload = {
                            accountGridDefinition: accountGridDefinition
                        };
                        VRUIUtilsService.callDirectiveLoad(accountGridDefinitionDirectiveAPI, accountGridDefinitionPayload, accountGridDefitnionLoadDeferred);

                        return accountGridDefitnionLoadDeferred.promise;
                    }
                    function getAccountViewDefinitionLoadPromise() {
                        var accountViewDefitnionLoadDeferred = UtilsService.createPromiseDeferred();

                        var accountViewDefinitionPayload = {
                            accountViewDefinitions: accountViewDefinitions
                        };
                        VRUIUtilsService.callDirectiveLoad(accountViewDefinitionDirectiveAPI, accountViewDefinitionPayload, accountViewDefitnionLoadDeferred);

                        return accountViewDefitnionLoadDeferred.promise;
                    }
                    function getAccountActionDefinitionLoadPromise() {
                        var accountActionDefitnionLoadDeferred = UtilsService.createPromiseDeferred();

                        var accountActionDefinitionPayload = {
                            accountActionDefinitions: accountActionDefinitions
                        };
                        VRUIUtilsService.callDirectiveLoad(accountActionDefinitionDirectiveAPI, accountActionDefinitionPayload, accountActionDefitnionLoadDeferred);

                        return accountActionDefitnionLoadDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Retail.BusinessEntity.Entities.AccountBEDefinitionSettings, Retail.BusinessEntity.Entities",
                        GridDefinition: accountGridDefinitionDirectiveAPI.getData(),
                        AccountViewDefinitions: accountViewDefinitionDirectiveAPI.getData(),
                        ActionDefinitions: accountActionDefinitionDirectiveAPI.getData(),
                    };

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);