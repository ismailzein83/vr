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

            var statusBEDefinitionSelectorAPI;
            var statusBEDefinitionSelectorDeferred = UtilsService.createPromiseDeferred();

            var accountGridDefinitionDirectiveAPI;
            var accountGridDefinitionDirectiveDeferred = UtilsService.createPromiseDeferred();

            var accountViewDefinitionDirectiveAPI;
            var accountViewDefinitionDirectiveDeferred = UtilsService.createPromiseDeferred();

            var accountActionDefinitionDirectiveAPI;
            var accountActionDefinitionDirectiveDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                    statusBEDefinitionSelectorAPI = api;
                    statusBEDefinitionSelectorDeferred.resolve();
                };
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
                UtilsService.waitMultiplePromises([accountGridDefinitionDirectiveDeferred.promise, accountViewDefinitionDirectiveDeferred.promise, accountActionDefinitionDirectiveDeferred.promise, statusBEDefinitionSelectorDeferred.promise]).then(function () {
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
                    var statusBEDefinitionId;

                    if (payload != undefined && payload.businessEntityDefinitionSettings != undefined) {
                        accountGridDefinition = payload.businessEntityDefinitionSettings.GridDefinition;
                        accountViewDefinitions = payload.businessEntityDefinitionSettings.AccountViewDefinitions;
                        accountActionDefinitions = payload.businessEntityDefinitionSettings.ActionDefinitions;
                        statusBEDefinitionId = payload.businessEntityDefinitionSettings.StatusBEDefinitionId;
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

                    //Loading Status Definition Directive
                    var statusBEDefinitionSelectorLoadPromise = loadStatusDefinitionSelectorLoadPromise();
                    promises.push(statusBEDefinitionSelectorLoadPromise);


                    function getAccountGridDefinitionLoadPromise() {
                        var accountGridDefitnionLoadDeferred = UtilsService.createPromiseDeferred();
                        accountGridDefinitionDirectiveDeferred.promise.then(function () {
                            var accountGridDefinitionPayload = {
                                accountGridDefinition: accountGridDefinition
                            };
                            VRUIUtilsService.callDirectiveLoad(accountGridDefinitionDirectiveAPI, accountGridDefinitionPayload, accountGridDefitnionLoadDeferred);
                        });
                        return accountGridDefitnionLoadDeferred.promise;
                    }
                    function getAccountViewDefinitionLoadPromise() {
                        var accountViewDefitnionLoadDeferred = UtilsService.createPromiseDeferred();
                        accountViewDefinitionDirectiveDeferred.promise.then(function () {
                            var accountViewDefinitionPayload = {
                                accountViewDefinitions: accountViewDefinitions
                            };
                            VRUIUtilsService.callDirectiveLoad(accountViewDefinitionDirectiveAPI, accountViewDefinitionPayload, accountViewDefitnionLoadDeferred);
                        });
                        return accountViewDefitnionLoadDeferred.promise;
                    }
                    function getAccountActionDefinitionLoadPromise() {
                        var accountActionDefitnionLoadDeferred = UtilsService.createPromiseDeferred();
                        accountActionDefinitionDirectiveDeferred.promise.then(function () {
                            var accountActionDefinitionPayload = {
                                accountActionDefinitions: accountActionDefinitions
                            };
                            VRUIUtilsService.callDirectiveLoad(accountActionDefinitionDirectiveAPI, accountActionDefinitionPayload, accountActionDefitnionLoadDeferred);
                        });
                        return accountActionDefitnionLoadDeferred.promise;
                    }
                    function loadStatusDefinitionSelectorLoadPromise() {
                        var statusBEDefinitionLoadDeferred = UtilsService.createPromiseDeferred();

                        statusBEDefinitionSelectorDeferred.promise.then(function () {
                            var accountActionDefinitionPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.Common.Business.StatusDefinitionBEFilter, Vanrise.Common.Business"
                                    }]
                                },
                                selectedIds: statusBEDefinitionId
                            };
                            VRUIUtilsService.callDirectiveLoad(statusBEDefinitionSelectorAPI, accountActionDefinitionPayload, statusBEDefinitionLoadDeferred);
                        });

                        return statusBEDefinitionLoadDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Retail.BusinessEntity.Entities.AccountBEDefinitionSettings, Retail.BusinessEntity.Entities",
                        GridDefinition: accountGridDefinitionDirectiveAPI.getData(),
                        AccountViewDefinitions: accountViewDefinitionDirectiveAPI.getData(),
                        ActionDefinitions: accountActionDefinitionDirectiveAPI.getData(),
                        StatusBEDefinitionId: statusBEDefinitionSelectorAPI.getSelectedIds()
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