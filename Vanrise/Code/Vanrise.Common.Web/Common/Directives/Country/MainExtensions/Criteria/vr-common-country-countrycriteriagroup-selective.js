'use strict';
app.directive('vrCommonCountryCountrycriteriagroupSelective', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new countryCriteriaCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return "/Client/Modules/Common/Directives/Country/MainExtensions/Criteria/Templates/SelectiveCountryCriteriaDirectiveTemplate.html"
            }
        };

        function countryCriteriaCtor(ctrl, $scope) {

            var countrySelectorAPI;
            var countrySelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCountrySelectorReady = function (api) {
                    countrySelectorAPI = api;
                    countrySelectorReadyDeferred.resolve();
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var selectedCountryIds;
                    if (payload != undefined) {
                        if (payload.countryGroupSettings != undefined) {
                            selectedCountryIds = payload.countryGroupSettings.CountryIds;
                        }
                    }

                    var loadCountrySelectorPromise = loadCountrySelector();
                    promises.push(loadCountrySelectorPromise);

                    function loadCountrySelector() {
                        var countrySelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        countrySelectorReadyDeferred.promise.then(function () {
                            var countrySelectorPayload;
                            if (selectedCountryIds != undefined) {
                                countrySelectorPayload = { selectedIds: selectedCountryIds };
                            }

                            VRUIUtilsService.callDirectiveLoad(countrySelectorAPI, countrySelectorPayload, countrySelectorLoadDeferred);
                        });

                        return countrySelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Common.MainExtensions.Country.SelectiveCountryCriteriaGroup, Vanrise.Common.MainExtensions",
                        CountryIds: countrySelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);