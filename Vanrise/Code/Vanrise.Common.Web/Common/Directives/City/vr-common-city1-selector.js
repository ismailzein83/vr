'use strict';
app.directive('vrCommonCity1Selector', ['VRCommon_CityAPIService', 'VRCommon_CountryAPIService', 'VRCommon_CityService', 'UtilsService', 'VRUIUtilsService',
    function (VRCommon_CityAPIService, VRCommon_CountryAPIService, VRCommon_CityService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: '=',
                isdisabled: "=",
                selectedvalues: "=",
                showaddbutton: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.filter;
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                $scope.addNewCity = function () {
                    var onCityAdded = function (cityObj) {
                        ctrl.datasource.push(cityObj.Entity);
                        if ($attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(cityObj.Entity);
                        else
                            ctrl.selectedvalues = cityObj.Entity;
                    };

                    if (ctrl.filter != undefined)
                        var countryId = ctrl.filter.CountryId;
                    VRCommon_CityService.addCity(onCityAdded, countryId);
                }
                $scope.datasource = [];
                var cityCtor = new CityCtor(ctrl, $scope, $attrs);
                cityCtor.initializeController();


            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: "/Client/Modules/Common/Directives/City/Templates/CitySelectorTemplate.html"

        };

        function CityCtor(ctrl, $scope, attrs) {
            var countrySelectorAPI;
            var citySelectorAPI;
            $scope.countries = [];
            $scope.cities = [];
            $scope.showCitySelector = false;
            this.initializeController = initializeController;

            function initializeController() {
                $scope.onCountrySelectorReady = function (api) {
                    countrySelectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                $scope.onCitySelectorReady = function (api) {
                    citySelectorAPI = api;
                };

                $scope.onCountrySelectionChanged = function (item) {
                    if (item != undefined) {
                        var payload = {};
                        payload.filter = { CountryId: item.CountryId }
                        getCitiesInfo($scope, payload)
                    }
                }

            }



            function getDirectiveAPI() {
                var api = {};
                var loadCountrySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                api.load = function (payload) {
                    var selectedIds;
                    var filter;
                    if (payload != undefined && payload.filter != undefined) {
                        $scope.showCountrySelector = false;
                        filter = payload.filter;
                        var serializedFilter = {};
                        serializedFilter = UtilsService.serializetoJson(filter);
                        getCitiesInfo($scope, serializedFilter);
                    }
                    else {
                        var countryPayload = {};
                        $scope.showCountrySelector = true;
                        VRUIUtilsService.callDirectiveLoad(countrySelectorAPI, countryPayload, loadCountrySelectorPromiseDeferred);
                    }
                };

                api.getData = function () {
                    return $scope.selectedCities;
                };


                return api;
            }
        }


        

        function getCitiesInfo(scope, serializedFilter) {
            scope.showCitySelector = true;
            return VRCommon_CityAPIService.GetCitiesInfo(serializedFilter).then(function (response) {
                scope.countries.length = 0;
                angular.forEach(response, function (itm) {
                    scope.countries.push(itm);
                });
            });
        }

        return directiveDefinitionObject;
    }]);

