'use strict';

app.directive('partnerportalCustomeraccessRetailusersubaccountsdefinitionEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RetailUserSubaccountsBEDefinitionCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/PartnerPortal_CustomerAccess/Elements/RetailAccountInfo/Directives/Templates/RetailUserSubaccountsBEDefinitionTemplate.html'
        };

        function RetailUserSubaccountsBEDefinitionCtor(ctrl, $scope, $attrs) {
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

                    if (payload != undefined)
                    {
                        if (payload.businessEntityDefinitionSettings != undefined && payload.businessEntityDefinitionSettings.AccountTypeIds != undefined)
                        {
                            $scope.scopeModel.accountType = payload.businessEntityDefinitionSettings.AccountTypeIds[0];
                        }
                    }

                    function loadConnectionSelector()
                    {
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
                        $type: "PartnerPortal.CustomerAccess.Business.RetailUserSubaccountsBEDefinition, PartnerPortal.CustomerAccess.Business",
                        VRConnectionId: connectionSelectorApi.getSelectedIds(),
                        AccountTypeIds: $scope.scopeModel.accountType != undefined?[$scope.scopeModel.accountType]:undefined
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