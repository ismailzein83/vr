'use strict';

app.directive('retailBePackagedefinitionRecurchargeEvaluatorOnetime', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new OneTimeEvaluator($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageDefinition/MainExtensions/ExtendedSettings/MainExtensions/Templates/OneTimeEvaluatorTemplate.html'
        };

        function OneTimeEvaluator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var genericLKUPDefinitionSelectorApi;
            var genericLKUPDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var selectedGenericLKUPDefinitionSelectorDeferred;

            var genericLKUPItemSelectorApi;
            var genericLKUPItemSelectorPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGenericLKUPDefinitionSelectorReady = function (api) {
                    genericLKUPDefinitionSelectorApi = api;
                    genericLKUPDefinitionSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericLKUPDefinitionSelectionChanged = function (selectedValue) {
                    if (selectedValue != undefined) {
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingDirective = value;
                        };
                        var payloadSelector = {
                            filter: {
                                BusinessEntityDefinitionId: selectedValue.BusinessEntityDefinitionId
                            }
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, genericLKUPItemSelectorApi, payloadSelector, setLoader, selectedGenericLKUPDefinitionSelectorDeferred);
                    }
                };

                $scope.scopeModel.onGenericLKUPItemSelectorReady = function (api) {
                    genericLKUPItemSelectorApi = api;
                    genericLKUPItemSelectorPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([genericLKUPDefinitionSelectorPromiseDeferred.promise, genericLKUPItemSelectorPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var evaluatorDefinitionSettings;
                    if (payload != undefined) {
                        evaluatorDefinitionSettings = payload.evaluatorDefinitionSettings;
                        if (evaluatorDefinitionSettings != undefined)
                            selectedGenericLKUPDefinitionSelectorDeferred = UtilsService.createPromiseDeferred();
                    }

                    promises.push(loadGenericLKUPDefinitionSelector());
                    function loadGenericLKUPDefinitionSelector() {
                        var payloadGenericLKUPDefinitionSelector = {
                            selectedIds: evaluatorDefinitionSettings != undefined ? evaluatorDefinitionSettings.ChargeableEntityBEDefinitionId : undefined,
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.ChargeableEntityDefinitionFilter, Retail.BusinessEntity.Business"
                                }]
                            }
                        };
                        return genericLKUPDefinitionSelectorApi.load(payloadGenericLKUPDefinitionSelector);
                    }

                    if (evaluatorDefinitionSettings != undefined)
                        promises.push(loadGenericLKUPItemSelector());

                    function loadGenericLKUPItemSelector() {
                        var payloadGenericLKUPItemSelector = {
                            selectedIds: evaluatorDefinitionSettings.ChargeableEntityId,
                            filter: {
                                BusinessEntityDefinitionId: evaluatorDefinitionSettings.ChargeableEntityBEDefinitionId
                            }
                        };
                        var promise = UtilsService.createPromiseDeferred();
                        selectedGenericLKUPDefinitionSelectorDeferred.promise.then(function () {
                            genericLKUPItemSelectorApi.load(payloadGenericLKUPItemSelector).then(function () {
                                selectedGenericLKUPDefinitionSelectorDeferred = undefined;
                                promise.resolve();
                            }).catch(function (error) {
                                promise.reject(error);
                            });
                        });
                        return promise.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.RecurringChargeEvaluators.OneTimeRecurringChargeEvaluatorDefinitionSettings, Retail.BusinessEntity.MainExtensions',
                        ChargeableEntityBEDefinitionId: genericLKUPDefinitionSelectorApi.getSelectedIds(),
                        ChargeableEntityId: genericLKUPItemSelectorApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);