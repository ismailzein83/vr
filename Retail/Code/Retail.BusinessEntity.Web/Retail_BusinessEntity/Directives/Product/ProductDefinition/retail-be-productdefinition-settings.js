'use strict';

app.directive('retailBeProductdefinitionSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var productDefinitionSettings = new ProductDefinitionSettings($scope, ctrl, $attrs);
                productDefinitionSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Product/ProductDefinition/Templates/ProductDefinitionSettingsTemplate.html'
        };

        function ProductDefinitionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var businessEntityDefinitionSelectorAPI;
            var businessEntityDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var extendedSettingsAPI;
            var extendedSettingsReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    businessEntityDefinitionSelectorAPI = api;
                    businessEntityDefinitionSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onExtendedSettingsReady = function (api) {
                    extendedSettingsAPI = api;
                    extendedSettingsReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var productDefinitionSettings;

                    if (payload != undefined) {
                        var productDefinitionEntity = payload.componentType;

                        if (productDefinitionEntity != undefined) {
                            $scope.scopeModel.name = productDefinitionEntity.Name;
                            productDefinitionSettings = productDefinitionEntity.Settings;
                        }
                    }

                    //Loading BusinessEntityDefinition selector
                    var businessEntityDefinitionSelectorLoadPromise = loadBusinessEntityDefinitionSelector();
                    promises.push(businessEntityDefinitionSelectorLoadPromise);

                    //Loading ExtendedSettings directive
                    var extendedSettingsDirectiveLoadPromise = loadExtendedSettings();
                    promises.push(extendedSettingsDirectiveLoadPromise);


                    function loadBusinessEntityDefinitionSelector() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        businessEntityDefinitionSelectorReadyDeferred.promise.then(function () {
                            var payload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business",
                                    }]
                                },
                                selectedIds: productDefinitionSettings != undefined ? productDefinitionSettings.AccountBEDefinitionId : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(businessEntityDefinitionSelectorAPI, payload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }
                    function loadExtendedSettings() {
                        var extendedSettingsLoadDeferred = UtilsService.createPromiseDeferred();

                        extendedSettingsReadyDeferred.promise.then(function () {
                            var extendedSettingsPayload = productDefinitionSettings != undefined ? { extendedSettings: productDefinitionSettings.ExtendedSettings } : undefined;
                            VRUIUtilsService.callDirectiveLoad(extendedSettingsAPI, extendedSettingsPayload, extendedSettingsLoadDeferred);
                        });

                        return extendedSettingsLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        Name: $scope.scopeModel.name,
                        Settings: GetAccountSettings()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function GetAccountSettings() {
                return {
                    $type: "Retail.BusinessEntity.Entities.ProductDefinitionSettings,  Retail.BusinessEntity.Entities",
                    AccountBEDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds(),
                    ExtendedSettings: extendedSettingsAPI.getData()
                };
            }
        }
    }]);
