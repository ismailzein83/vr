(function (app) {

    'use strict';

    CountryZoneSelector.$inject = [ 'UtilsService', 'VRUIUtilsService'];

    function CountryZoneSelector( UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var countryZoneSelector = new CountryZoneSelectorDirective(ctrl, $scope, $attrs);
                countryZoneSelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Zone/Templates/CountryZoneTemplate.html"

        };

        function CountryZoneSelectorDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var countryAPI;
            var countryReadyDeferred = UtilsService.createPromiseDeferred();

            var zoneAPI;
            var zoneReadyDeferred = UtilsService.createPromiseDeferred();
            var countrySelectedPromiseDeferred;
            function initializeController() {

                $scope.onCountrySelectorReady = function (api) {
                    countryAPI = api;
                    countryReadyDeferred.resolve();
                };
                $scope.onZoneSelectorReady = function (api) {
                    zoneAPI = api;
                    if (zoneReadyDeferred != undefined)
                        zoneReadyDeferred.resolve();
                };

                $scope.onCountrySelectionChanged = function () {
                    if (countryAPI != undefined) {
                        var selectedCountries = countryAPI.getSelectedIds();
                        if (selectedCountries != undefined) {
                            var setLoader = function (value) {
                                $scope.isLoadingDirective = value;
                            };
                            var payload = { filter: { CountryId: selectedCountries } };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, zoneAPI, payload, setLoader, countrySelectedPromiseDeferred);
                        }
                    }
                };
                defineAPI();

            }
            function defineAPI() {
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(getDirectiveAPI());
                }

                function getDirectiveAPI() {
                    var api = {};

                    api.load = function (payload) {
                        var promises = [];

                        var loadCountryDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        if (payload != undefined && payload.CountryId != undefined)
                            countrySelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                        countryReadyDeferred.promise.then(function () {
                            var payloadCountryDirective = {
                                selectedIds: payload != undefined ? payload.CountryId : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(countryAPI, payloadCountryDirective, loadCountryDirectivePromiseDeferred);
                        });
                        promises.push(loadCountryDirectivePromiseDeferred.promise);
                        var loadZoneDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        if (countrySelectedPromiseDeferred != undefined) {
                            UtilsService.waitMultiplePromises([zoneReadyDeferred.promise, countrySelectedPromiseDeferred.promise]).then(function () {
                                var payloadZoneDirective;
                                if (payload != undefined) {
                                    payloadZoneDirective = {
                                        filter: { CountryId: payload.CountryId },
                                        selectedIds: payload.ZonesIds
                                    };
                                }
                                VRUIUtilsService.callDirectiveLoad(zoneAPI, payloadZoneDirective, loadZoneDirectivePromiseDeferred);
                                countrySelectedPromiseDeferred = undefined;
                            });
                        } else {
                            loadZoneDirectivePromiseDeferred.resolve();
                        }

                        promises.push(loadZoneDirectivePromiseDeferred.promise);
                        return UtilsService.waitMultiplePromises(promises);
                    };
                    api.getData = function () {
                        var data = {
                            CountryId: countryAPI.getSelectedIds(),
                            ZonesIds: zoneAPI.getSelectedIds()
                        };
                        return data;
                    };

                    return api;
                }
            }
        }
    }

    app.directive('retailBeCountryzoneSelector', CountryZoneSelector);

})(app);
