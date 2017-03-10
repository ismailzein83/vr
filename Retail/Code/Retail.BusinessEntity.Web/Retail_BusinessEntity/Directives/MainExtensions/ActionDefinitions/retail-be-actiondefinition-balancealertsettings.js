'use strict';

app.directive('retailBeActiondefinitionBalancealertsettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new balanceAlertActionDefinition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/ActionDefinitions/Templates/ActionDefinitionBalanceAlertSettings.html'
        };

        function balanceAlertActionDefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorApi = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };


                defineAPI();
            };

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettings;
                    if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                        extendedSettings = payload.Settings.ExtendedSettings;
                    }
                    var promises = [];

                    var businessEntityDefinitionSelectorLoadPromise = getBusinessEntityDefinitionSelectorLoadPromise();
                    promises.push(businessEntityDefinitionSelectorLoadPromise);


                    function getBusinessEntityDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorPromiseDeferred.promise.then(function () {

                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                },
                                selectedIds: extendedSettings != undefined ? extendedSettings.AccountBEDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.Business.Extensions.BalanceAlertActionDefinitionSettings,Retail.BusinessEntity.Business',
                        AccountBEDefinitionId: beDefinitionSelectorApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);