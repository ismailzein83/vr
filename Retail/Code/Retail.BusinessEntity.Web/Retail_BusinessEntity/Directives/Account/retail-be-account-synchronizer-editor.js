'use strict';

app.directive('retailBeAccountSynchronizerEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailBeAccountSynchronizerEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Account/Templates/AccountSynchronizerEditor.html"
        };

        function retailBeAccountSynchronizerEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;
            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var accountSynchronizerHandlersGridApi;
            var accountSynchronizerHandlersGridPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};

            $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                beDefinitionSelectorApi = api;
                beDefinitionSelectorPromiseDeferred.resolve();
            };

            $scope.scopeModel.onAccountSynchronizerHandlerGridReady = function (api) {
                accountSynchronizerHandlersGridApi = api;
                accountSynchronizerHandlersGridPromiseDeferred.resolve();
            }

            function initializeController() {
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var businessEntityDefinitionSelectorLoadPromise = getBusinessEntityDefinitionSelectorLoadPromise();
                    promises.push(businessEntityDefinitionSelectorLoadPromise);

                    promises.push(loadAccountSynchronizerGrid());

                    function getBusinessEntityDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };

                            if (payload != undefined) {
                                selectorPayload.selectedIds = payload.AccountBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    function loadAccountSynchronizerGrid() {
                        var loadAccountSynchronizerGridDeferred = UtilsService.createPromiseDeferred();

                        accountSynchronizerHandlersGridPromiseDeferred.promise.then(function () {
                            var gridPayload;

                            if (payload != undefined) {
                                gridPayload.AccountBEDefinitionId = payload.AccountBEDefinitionId;
                                gridPayload.AccountSynchronizerHandlers = payload.InsertHandlers;
                            }
                            VRUIUtilsService.callDirectiveLoad(accountSynchronizerHandlersGridApi, gridPayload, loadAccountSynchronizerGridDeferred);
                        });

                        return loadAccountSynchronizerGridDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.BusinessEntity.Business.AccountSynchronizer, Retail.BusinessEntity.Business",
                        Name: "Account Synchronizer",
                        AccountBEDefinitionId: beDefinitionSelectorApi.getSelectedIds(),
                        InsertHandlers: accountSynchronizerHandlersGridApi.getData()
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);