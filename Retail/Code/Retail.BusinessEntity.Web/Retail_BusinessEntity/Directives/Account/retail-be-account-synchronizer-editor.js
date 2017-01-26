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

            var synchronizerEntity;

            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var businessEntityDefinitionSelectionChangedDeferred;

            var accountSynchronizerHandlersGridApi;
            var accountSynchronizerHandlersGridPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};
            $scope.scopeModel.selectedBEDefinition;
            $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                beDefinitionSelectorApi = api;
                beDefinitionSelectorPromiseDeferred.resolve();
            };

            $scope.scopeModel.onAccountSynchronizerHandlerGridReady = function (api) {
                accountSynchronizerHandlersGridApi = api;
                accountSynchronizerHandlersGridPromiseDeferred.resolve();
            }

            $scope.scopeModel.onBusinessEntityDefinitionSelectionChanged = function (selectedItem) {
                if (selectedItem != undefined && businessEntityDefinitionSelectionChangedDeferred == undefined) {

                    accountSynchronizerHandlersGridApi.clear();

                    var selectorPayload = {
                        AccountBEDefinitionId: selectedItem.BusinessEntityDefinitionId
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isAccountTypeSelectorLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, accountSynchronizerHandlersGridApi, selectorPayload, setLoader, businessEntityDefinitionSelectionChangedDeferred);
                }
                else if (businessEntityDefinitionSelectionChangedDeferred != undefined) {
                    businessEntityDefinitionSelectionChangedDeferred.resolve();
                }
            };

            function initializeController() {
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    synchronizerEntity = payload;
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

                            if (synchronizerEntity != undefined) {
                                selectorPayload.selectedIds = synchronizerEntity.AccountBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    function loadAccountSynchronizerGrid() {
                        if (businessEntityDefinitionSelectionChangedDeferred == undefined)
                            businessEntityDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        var loadAccountSynchronizerGridDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([accountSynchronizerHandlersGridPromiseDeferred.promise, businessEntityDefinitionSelectionChangedDeferred.promise]).then(function () {
                            businessEntityDefinitionSelectionChangedDeferred = undefined;
                            var gridPayload = {};
                            gridPayload.AccountBEDefinitionId = $scope.scopeModel.selectedBEDefinition != undefined
                                                                    ? $scope.scopeModel.selectedBEDefinition.BusinessEntityDefinitionId
                                                                    : undefined;
                            if (synchronizerEntity != undefined) {
                                gridPayload.AccountBEDefinitionId = synchronizerEntity.AccountBEDefinitionId;
                                gridPayload.AccountSynchronizerHandlers = synchronizerEntity.InsertHandlers;
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