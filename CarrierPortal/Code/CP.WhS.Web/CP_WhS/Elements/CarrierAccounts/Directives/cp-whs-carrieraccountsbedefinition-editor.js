'use strict';

app.directive('cpWhsCarrieraccountsbedefinitionEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new WhSUserSubaccountsBEDefinitionCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/CP_WhS/Elements/CarrierAccounts/Directives/Templates/WhSUserSubaccountsBEDefinitionTemplate.html'
        };

        function WhSUserSubaccountsBEDefinitionCtor(ctrl, $scope, $attrs) {
            var connectionSelectorApi;
            var connectionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorApi = api;
                    connectionSelectorPromiseDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([connectionSelectorPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        if (payload.businessEntityDefinitionSettings != undefined) {
                            $scope.scopeModel.includeSuppliers = payload.businessEntityDefinitionSettings.GetSuppliers;
                            $scope.scopeModel.includeCustomers = payload.businessEntityDefinitionSettings.GetCustomers;
                        }
                    }

                    function loadConnectionSelector() {
                        var payloadSelector = {
                            filter: { Filters: [] }
                        };
                        payloadSelector.filter.Filters.push({
                            $type: "Vanrise.Common.Business.VRInterAppRestConnectionFilter, Vanrise.Common.Business"
                        });
                        if (payload != undefined && payload.businessEntityDefinitionSettings != undefined) {
                            payloadSelector.selectedIds = payload.businessEntityDefinitionSettings.VRConnectionId;
                        };
                        return connectionSelectorApi.load(payloadSelector);
                    }

                    promises.push(loadConnectionSelector());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "CP.WhS.Business.WhSCarrierAccountsBEDefinition, CP.WhS.Business",
                        VRConnectionId: connectionSelectorApi.getSelectedIds(),
                        GetCustomers: $scope.scopeModel.includeCustomers,
                        GetSuppliers:$scope.scopeModel.includeSuppliers
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