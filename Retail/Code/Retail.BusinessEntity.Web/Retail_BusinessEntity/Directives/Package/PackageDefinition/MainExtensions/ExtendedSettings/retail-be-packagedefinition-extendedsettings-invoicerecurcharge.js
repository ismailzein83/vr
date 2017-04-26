'use strict';

app.directive('retailBePackagedefinitionExtendedsettingsInvoicerecurcharge', ['UtilsService','VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new InvoiceRecurchargePackageDefinitionSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageDefinition/MainExtensions/ExtendedSettings/Templates/InvoiceRecurchargePackageDefinitionSettingsTemplate.html'
        };

        function InvoiceRecurchargePackageDefinitionSettings($scope, ctrl, $attrs) {
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
                    if (selectedValue != undefined)
                    {
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
                    var extendedSettings;
                    if (payload != undefined)
                    {
                        extendedSettings = payload.extendedSettings;
                        if (extendedSettings != undefined)
                         selectedGenericLKUPDefinitionSelectorDeferred = UtilsService.createPromiseDeferred();
                    }

                    promises.push(loadGenericLKUPDefinitionSelector());
                    function loadGenericLKUPDefinitionSelector() {
                        var payloadGenericLKUPDefinitionSelector = {
                            selectedIds: extendedSettings != undefined ? extendedSettings.ChargeableEntityBEDefinitionId : undefined,
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.ChargeableEntityDefinitionFilter, Retail.BusinessEntity.Business"
                                }]
                            }
                        };
                        return genericLKUPDefinitionSelectorApi.load(payloadGenericLKUPDefinitionSelector);
                    }

                    if (extendedSettings != undefined)
                     promises.push(loadGenericLKUPItemSelector());

                    function loadGenericLKUPItemSelector() {
                        var payloadGenericLKUPItemSelector = {
                            selectedIds: extendedSettings.ChargeableEntityId,
                            filter: {
                                BusinessEntityDefinitionId: extendedSettings.ChargeableEntityBEDefinitionId
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
                        $type: 'Retail.BusinessEntity.MainExtensions.PackageTypes.InvoiceRecurChargePackageDefinitionSettings, Retail.BusinessEntity.MainExtensions',
                        ChargeableEntityBEDefinitionId: genericLKUPDefinitionSelectorApi.getSelectedIds(),
                        ChargeableEntityId: genericLKUPItemSelectorApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);