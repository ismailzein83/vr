'use strict';

app.directive('cpWhsSupplierzonesbedefinitionEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new WhSSupplierZonesBEDefinitionCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/CP_WhS/Elements/SupplierZones/Directives/Templates/WhSSupplierZonesBEDefinitionTemplate.html'
        };

        function WhSSupplierZonesBEDefinitionCtor(ctrl, $scope, $attrs) {
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
                        $type: "CP.WhS.Business.WhSSupplierZonesBEDefinition, CP.WhS.Business",
                        VRConnectionId: connectionSelectorApi.getSelectedIds()
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