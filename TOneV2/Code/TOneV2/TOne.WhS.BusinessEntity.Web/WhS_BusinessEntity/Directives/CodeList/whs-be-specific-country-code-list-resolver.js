'use strict';
app.directive('whsBeSpecificCountryCodeListResolver', ['UtilsService', 'VRUIUtilsService',
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
                return "/Client/Modules/WhS_BusinessEntity/Directives/CodeList/templates/SpecificCountryCodeListResolverTemplate.html";
            }
        };

        function countryCriteriaCtor(ctrl, $scope) {

            var countrySelectorAPI;
            var countrySelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var sellingNumberPlanSelectorAPI;
            var sellingNumberPlanSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var excludedDestinationDirectiveAPI;
            var excludedDestinationDefferedReady = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};
                $scope.onSellingnumberPlanDirectiveReady = function (api) {
                    sellingNumberPlanSelectorAPI = api;
                    sellingNumberPlanSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onCountrySelectorReady = function (api) {
                    countrySelectorAPI = api;
                    countrySelectorReadyDeferred.resolve();
                };

                $scope.onExcludedDestinationDirectiveReady = function (api) {
                    excludedDestinationDirectiveAPI = api;
                    excludedDestinationDefferedReady.resolve();
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
                    var loadSellingNumberPlanPromise = loadSellingNumberPlan();
                    var excludedDirectivePromise = excludedDirective();
                    promises.push(loadCountrySelectorPromise);
                    promises.push(loadSellingNumberPlanPromise);
                    promises.push(excludedDirectivePromise);


                    function excludedDirective() {
                        return excludedDestinationDefferedReady.promise.then(function () {

                            var directivePayload;
                            VRUIUtilsService.callDirectiveLoad(excludedDestinationDirectiveAPI, directivePayload, undefined)
                        })


                    }

                    function loadSellingNumberPlan() {
                        var sellingnumberPlanSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        countrySelectorReadyDeferred.promise.then(function () {


                            VRUIUtilsService.callDirectiveLoad(sellingNumberPlanSelectorAPI, undefined, sellingnumberPlanSelectorLoadDeferred);
                        });

                        return sellingnumberPlanSelectorLoadDeferred.promise;
                    }



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
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.CodeList.SpecificCountryCodeListResolver,TOne.WhS.BusinessEntity.MainExtensions",
                        CountryIds: countrySelectorAPI.getSelectedIds(),
                        SellingNumberPlanId: sellingNumberPlanSelectorAPI.getSelectedIds(),
                        ExcludedDestinations: excludedDestinationDirectiveAPI.getData()

                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);