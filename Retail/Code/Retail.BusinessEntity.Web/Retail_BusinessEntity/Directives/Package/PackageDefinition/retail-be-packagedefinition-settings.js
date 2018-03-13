'use strict';

app.directive('retailBePackagedefinitionSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var packageDefinitionSettings = new PackageDefinitionSettings($scope, ctrl, $attrs);
                packageDefinitionSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageDefinition/Templates/PackageDefinitionSettings.html'
        };

        function PackageDefinitionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var packageDefinitionEntity;

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
                    if (payload != undefined) {
                        packageDefinitionEntity = payload.componentType;
                        if (packageDefinitionEntity != undefined) {
                            $scope.scopeModel.name = packageDefinitionEntity.Name;
                        }
                    }
                    promises.push(loadBusinessEntityDefinitionSelector());
                    promises.push(loadExtendedSettings());
                    function loadBusinessEntityDefinitionSelector() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        businessEntityDefinitionSelectorReadyDeferred.promise.then(function () {
                            var payload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business",
                                    }]
                                },
                                selectedIds: packageDefinitionEntity != undefined && packageDefinitionEntity.Settings != undefined ? packageDefinitionEntity.Settings.AccountBEDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(businessEntityDefinitionSelectorAPI, payload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }
                    function loadExtendedSettings() {
                        var extendedSettingsLoadDeferred = UtilsService.createPromiseDeferred();
                        extendedSettingsReadyDeferred.promise.then(function () {
                            var extendedSettingsPayload = packageDefinitionEntity != undefined && packageDefinitionEntity.Settings != undefined ? { extendedSettings: packageDefinitionEntity.Settings.ExtendedSettings } : undefined;
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
                    $type: "Retail.BusinessEntity.Entities.PackageDefinitionSettings,  Retail.BusinessEntity.Entities",
                    AccountBEDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds(),
                    ExtendedSettings: extendedSettingsAPI.getData()
                };
            }
        }
    }]);
