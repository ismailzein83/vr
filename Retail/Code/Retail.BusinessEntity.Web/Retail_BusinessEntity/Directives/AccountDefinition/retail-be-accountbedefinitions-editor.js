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

            var accountGridDefinitionExportExcelDirectiveAPI;
            var accountGridDefinitionExportExcelDirectiveDeferred = UtilsService.createPromiseDeferred();

            var accountViewDefinitionDirectiveAPI;
            var accountViewDefinitionDirectiveDeferred = UtilsService.createPromiseDeferred();

            var accountActionDefinitionDirectiveAPI;
            var accountActionDefinitionDirectiveDeferred = UtilsService.createPromiseDeferred();

            var accountExtraFieldDefinitionsDirectiveAPI;
            var accountExtraFieldDefinitionsDirectiveDeferred = UtilsService.createPromiseDeferred();

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
                $scope.scopeModel.onAccountGridDefinitionExportExcelReady = function (api) {
                    accountGridDefinitionExportExcelDirectiveAPI = api;
                    accountGridDefinitionExportExcelDirectiveDeferred.resolve();
                };
                $scope.scopeModel.onAccountViewDefinitionsReady = function (api) {
                    accountViewDefinitionDirectiveAPI = api;
                    accountViewDefinitionDirectiveDeferred.resolve();
                }
                $scope.scopeModel.onAccountActionDefinitionsReady = function (api) {
                    accountActionDefinitionDirectiveAPI = api;
                    accountActionDefinitionDirectiveDeferred.resolve();
                }
                $scope.scopeModel.onAccountExtraFieldDefinitionsReady = function (api) {
                    accountExtraFieldDefinitionsDirectiveAPI = api;
                    accountExtraFieldDefinitionsDirectiveDeferred.resolve();
                }
                UtilsService.waitMultiplePromises([statusBEDefinitionSelectorDeferred.promise, accountGridDefinitionDirectiveDeferred.promise, accountViewDefinitionDirectiveDeferred.promise, accountActionDefinitionDirectiveDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountBEDefinitionId;
                    var statusBEDefinitionId;
                    var accountGridDefinition;
                    var accountGridDefinitionExportExcel;
                    var accountViewDefinitions;
                    var accountActionDefinitions;
                    var accountExtraFieldDefinitions;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.businessEntityDefinitionId;

                        if (payload.businessEntityDefinitionSettings != undefined) {
                            statusBEDefinitionId = payload.businessEntityDefinitionSettings.StatusBEDefinitionId;
                            accountGridDefinition = payload.businessEntityDefinitionSettings.GridDefinition.ColumnDefinitions;
                            accountGridDefinitionExportExcel = payload.businessEntityDefinitionSettings.GridDefinition.ExportColumnDefinitions;
                            accountViewDefinitions = payload.businessEntityDefinitionSettings.AccountViewDefinitions;
                            accountActionDefinitions = payload.businessEntityDefinitionSettings.ActionDefinitions;
                            accountExtraFieldDefinitions = payload.businessEntityDefinitionSettings.AccountExtraFieldDefinitions;
                        }
                    }

                    //Loading Status Definition Directive
                    var statusBEDefinitionSelectorLoadPromise = getStatusDefinitionSelectorLoadPromise();
                    promises.push(statusBEDefinitionSelectorLoadPromise);

                    //Loading AccountGridDefinition Directive
                    var accountGridDefinitionLoadPromise = getAccountGridDefinitionLoadPromise();
                    promises.push(accountGridDefinitionLoadPromise);

                    //Loading AccountGridDefinitionExportExcel Directive
                    var accountGridDefinitionExportExcelLoadPromise = getAccountGridDefinitionExportExcelLoadPromise();
                    promises.push(accountGridDefinitionExportExcelLoadPromise);

                    //Loading AccountViewDefinition Directive
                    var accountViewDefinitionLoadPromise = getAccountViewDefinitionLoadPromise();
                    promises.push(accountViewDefinitionLoadPromise);

                    //Loading AccountActionDefinition Directive
                    var accountActionDefinitionLoadPromise = getAccountActionDefinitionLoadPromise();
                    promises.push(accountActionDefinitionLoadPromise);

                    var accountExtraFieldDefinitionsLoadPromise = getAccountExtraFieldDefinitionsLoadPromise();
                    promises.push(accountExtraFieldDefinitionsLoadPromise);

                    function getStatusDefinitionSelectorLoadPromise() {
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
                    function getAccountGridDefinitionLoadPromise() {
                        var accountGridDefitnionLoadDeferred = UtilsService.createPromiseDeferred();

                        accountGridDefinitionDirectiveDeferred.promise.then(function () {
                            var accountGridDefinitionPayload = {
                                accountBEDefinitionId: accountBEDefinitionId,
                                accountGridDefinition: accountGridDefinition
                            };
                            VRUIUtilsService.callDirectiveLoad(accountGridDefinitionDirectiveAPI, accountGridDefinitionPayload, accountGridDefitnionLoadDeferred);
                        });

                        return accountGridDefitnionLoadDeferred.promise;
                    }

                    function getAccountGridDefinitionExportExcelLoadPromise() {
                        var accountGridDefitnionExportExcelLoadDeferred = UtilsService.createPromiseDeferred();

                        accountGridDefinitionExportExcelDirectiveDeferred.promise.then(function () {
                            var accountGridDefinitionExportExcelPayload = {
                                accountBEDefinitionId: accountBEDefinitionId,
                                accountGridDefinitionExportExcel: accountGridDefinitionExportExcel
                            };
                            VRUIUtilsService.callDirectiveLoad(accountGridDefinitionExportExcelDirectiveAPI, accountGridDefinitionExportExcelPayload, accountGridDefitnionExportExcelLoadDeferred);
                        });

                        return accountGridDefitnionExportExcelLoadDeferred.promise;
                    }

                    function getAccountViewDefinitionLoadPromise() {
                        var accountViewDefitnionLoadDeferred = UtilsService.createPromiseDeferred();

                        accountViewDefinitionDirectiveDeferred.promise.then(function () {
                            var accountViewDefinitionPayload = {
                                accountBEDefinitionId: accountBEDefinitionId,
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
                                accountBEDefinitionId: accountBEDefinitionId,
                                accountActionDefinitions: accountActionDefinitions
                            };
                            VRUIUtilsService.callDirectiveLoad(accountActionDefinitionDirectiveAPI, accountActionDefinitionPayload, accountActionDefitnionLoadDeferred);
                        });

                        return accountActionDefitnionLoadDeferred.promise;
                    }
                    function getAccountExtraFieldDefinitionsLoadPromise() {
                        var accountExtraFieldDefinitionsLoadDeferred = UtilsService.createPromiseDeferred();

                        accountExtraFieldDefinitionsDirectiveDeferred.promise.then(function () {
                            var accountExtraFieldDefinitionsDefinitionPayload = {
                                accountBEDefinitionId: accountBEDefinitionId,
                                accountExtraFieldDefinitions: accountExtraFieldDefinitions
                            };
                            VRUIUtilsService.callDirectiveLoad(accountExtraFieldDefinitionsDirectiveAPI, accountExtraFieldDefinitionsDefinitionPayload, accountExtraFieldDefinitionsLoadDeferred);
                        });

                        return accountExtraFieldDefinitionsLoadDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var gridDefinition = {
                        ColumnDefinitions: accountGridDefinitionDirectiveAPI.getData(),
                        ExportColumnDefinitions: accountGridDefinitionExportExcelDirectiveAPI.getData()
                    }

                    var obj = {
                        $type: "Retail.BusinessEntity.Entities.AccountBEDefinitionSettings, Retail.BusinessEntity.Entities",
                        StatusBEDefinitionId: statusBEDefinitionSelectorAPI.getSelectedIds(),
                        GridDefinition: gridDefinition,
                        AccountViewDefinitions: accountViewDefinitionDirectiveAPI.getData(),
                        ActionDefinitions: accountActionDefinitionDirectiveAPI.getData(),
                        AccountExtraFieldDefinitions: accountExtraFieldDefinitionsDirectiveAPI.getData()
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