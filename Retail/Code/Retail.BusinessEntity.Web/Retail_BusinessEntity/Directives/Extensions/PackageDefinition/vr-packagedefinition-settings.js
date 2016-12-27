'use strict';

app.directive('vrPackagedefinitionSettings', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Extensions/PackageDefinition/Templates/VRPackageDefinitionSettings.html'
        };

        function PackageDefinitionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var packageDefinitionEntity;
            var businessEntityDefinitionSelectorAPI;
            var businessEntityDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    businessEntityDefinitionSelectorAPI = api;
                    businessEntityDefinitionSelectorReadyDeferred.resolve();
                };
                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        packageDefinitionEntity = payload.componentType;
                        if(packageDefinitionEntity != undefined)
                        {
                            $scope.scopeModel.name = packageDefinitionEntity.Name;
                        }
                    }
                    promises.push(loadBusinessEntityDefinitionSelector());
                    function loadBusinessEntityDefinitionSelector() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        businessEntityDefinitionSelectorReadyDeferred.promise.then(function () {
                            var payload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Entities.AccountBEDefinitionFilter, Retail.BusinessEntity.Entities",
                                    }]
                                },
                                selectedIds: packageDefinitionEntity != undefined && packageDefinitionEntity.Settings != undefined ? packageDefinitionEntity.Settings.AccountBEDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(businessEntityDefinitionSelectorAPI, payload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
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
                     AccountBEDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds()
                };
            }
        }
    }]);
